/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Runtime;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Security.Cryptography;

namespace ShiftOS.Engine
{
    /// <summary>
    /// The C# side of the ShiftOS Python API.
    /// </summary>
    public static class PythonHelper
    {
        /// <summary>
        /// Creates and sets up a new Python engine.
        /// </summary>
        /// <returns>The Python engine.</returns>
        private static ScriptEngine NewEngine()
        {
            var pyengine = Python.CreateEngine();
            var search = pyengine.GetSearchPaths();
            search.Add("Lib");
            pyengine.SetSearchPaths(search);
            pyengine.Runtime.LoadAssembly(typeof(IronPython.Modules.PythonErrorNumber).Assembly);
            return pyengine;
        }

        /// <summary>
        /// Runs Python code passed as a string.
        /// </summary>
        /// <param name="code">The code you want to run.</param>
        /// <returns>The code's scope.</returns>
        public static ScriptScope RunCode(string code)
        {
            var pyengine = NewEngine();
            var source = pyengine.CreateScriptSourceFromString(code);
            var scope = pyengine.CreateScope();
            source.Execute(scope);
            return scope;
        }
    }

    /// <summary>
    /// The interpreter for the meta-language in Python ShiftOS mods.
    /// </summary>
    public static class PythonAPI
    {
        private static class AsmCache
        {
            private static byte[] magic = Encoding.UTF8.GetBytes("ASMC");
            public static Dictionary<string, AsmCacheEntry> Load(Stream fobj)
            {
                var ret = new Dictionary<string, AsmCacheEntry>();
                var header = new byte[4];
                fobj.Read(header, 0, 4);
                if (!header.SequenceEqual(magic))
                    throw new Exception("This is not an assembly cache.");
                var read = new BinaryReader(fobj);
                var num = read.ReadInt32();
                for (int i = 0; i < num; i++)
                {
                    var entry = new AsmCacheEntry();

                    // read filename (stored as Pascal string)
                    var flen = fobj.ReadByte();
                    var fbytes = new byte[flen];
                    fobj.Read(fbytes, 0, flen);
                    string fname = Encoding.UTF8.GetString(fbytes);

                    // read SHA-512 checksum
                    fobj.Read(entry.checksum, 0, 64);

                    // read assemblies
                    var numasm = read.ReadInt32();
                    for (int j = 0; j < numasm; j++)
                    {
                        var asmlen = read.ReadInt32();
                        var asmdata = new byte[asmlen];
                        fobj.Read(asmdata, 0, asmlen);
                        entry.asms.Add(asmdata);

                    }

                    ret.Add(fname, entry);
                }
                return ret;
            }

            public static void Save(Stream fobj, Dictionary<string, AsmCacheEntry> data)
            {
                fobj.Write(magic, 0, 4);
                var write = new BinaryWriter(fobj);
                write.Write(data.Count);
                foreach (var entry in data)
                {
                    // write filename
                    var fbytes = Encoding.UTF8.GetBytes(entry.Key);
                    fobj.WriteByte((byte) fbytes.Length);
                    fobj.Write(fbytes, 0, fbytes.Length);

                    // write SHA-512 checksum
                    fobj.Write(entry.Value.checksum, 0, 64);

                    // write assemblies
                    write.Write(entry.Value.asms.Count);
                    foreach (var asm in entry.Value.asms)
                    {
                        var asmlen = asm.Length;
                        write.Write(asmlen);
                        fobj.Write(asm, 0, asmlen);
                    }
                }
            }
        }
        private class AsmCacheEntry
        {
            public byte[] checksum;
            public List<byte[]> asms;
            public AsmCacheEntry()
            {
                checksum = new byte[64];
                asms = new List<byte[]>();
            }
        }
        private static bool scanned = false;
        public static Dictionary<string, ScriptScope> scopes;
        /// <summary>
        /// Finds Python mods and adds them to ReflectMan's Types list.
        /// </summary>
        public static void Scan()
        {
            if (scanned)
                throw new Exception("PythonAPI.Scan() called multiple times");
            scopes = new Dictionary<string, ScriptScope>();
            var resman = new System.Resources.ResourceManager("ShiftOS.Engine.Properties.Resources", typeof(Properties.Resources).Assembly);
            var provider = new CSharpCodeProvider();
            var refs = AppDomain.CurrentDomain.GetAssemblies().Select(f => f.Location).Concat(new string[] { "Microsoft.CSharp.dll" }).ToArray();
            var types = new List<Type>();
            var sha = new SHA512Managed();
            var oldcache = new Dictionary<string, AsmCacheEntry>();
            var newcache = new Dictionary<string, AsmCacheEntry>();
            if (File.Exists("pyasmcache.dat"))
                using (var stream = File.OpenRead("pyasmcache.dat"))
                    try
                    {
                        oldcache = AsmCache.Load(stream);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine("[dev] Failed to read the assembly cache.");
                        Console.WriteLine(ex.ToString());
#endif
                    }
            foreach (var fname in Directory.GetFiles(Environment.CurrentDirectory, "*.py").Select(Path.GetFileName))
            {
                byte[] checksum;
                using (FileStream stream = File.OpenRead(fname))
                    checksum = sha.ComputeHash(stream);
                var script = File.ReadAllText(fname);
                try
                {
                    scopes[fname] = PythonHelper.RunCode(script);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[dev] Failed to execute Python script " + fname);
                    Console.WriteLine(ex.ToString());
                    continue;
                }
                if (oldcache.ContainsKey(fname))
                {
                    var oldentry = oldcache[fname];
                    if (checksum.SequenceEqual(oldentry.checksum))
                    {
                        try
                        {
                            foreach (var asm in oldentry.asms)
                                types.AddRange(Assembly.Load(asm).GetTypes());
                            newcache.Add(fname, oldentry);
                            continue;
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Console.WriteLine("[dev] Failed to load cached assembly for " + fname);
                            Console.WriteLine(ex.ToString());
#endif
                        }
                    }
                }
                var scriptlines = script.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n'); // line-ending independent...
                int pos = 0;
                var entry = new AsmCacheEntry();
                entry.checksum = checksum;
                var parameters = new CompilerParameters();
                parameters.ReferencedAssemblies.AddRange(refs);
                parameters.GenerateInMemory = false; // We need to keep the temporary file around long enough to copy it to the cache.
                parameters.GenerateExecutable = false;
                try
                {
                    while (pos < scriptlines.Length)
                    {
                        while (!scriptlines[pos].StartsWith("#ShiftOS"))
                            pos++;
                        var templatename = scriptlines[pos].Split(':')[1];
                        pos++;
                        string decorators = "";
                        while (scriptlines[pos].StartsWith("#"))
                        {
                            decorators += scriptlines[pos].Substring(1) + Environment.NewLine; // remove # and add to string
                            pos++;
                        }
                        if (!scriptlines[pos].StartsWith("class "))
                            throw new Exception("ShiftOS decorators without matching global class");
                        var classname = scriptlines[pos].Split(' ')[1];
                        if (classname.Contains("(")) // derived class
                            classname = classname.Split('(')[0];
                        else
                            classname = classname.Remove(classname.Length - 1); // remove :
                        var code = String.Format(resman.GetString(templatename), decorators, classname, fname.Replace("\\", "\\\\")); // generate the C# wrapper class from template
#if DEBUG
                        Console.WriteLine(code);
#endif
                        var results = provider.CompileAssemblyFromSource(parameters, code);
                        if (results.Errors.HasErrors)
                        {
                            string except = "The wrapper class failed to compile.";
                            foreach (CompilerError error in results.Errors)
                                except += Environment.NewLine + error.ErrorText;
                            throw new Exception(except);
                        }
                        types.AddRange(results.CompiledAssembly.GetTypes()); // We did it!
                        entry.asms.Add(File.ReadAllBytes(results.PathToAssembly));
                        pos++; // keep scanning the file for more classes
                    }
                }
                catch (Exception ex) // Skip any file that has issues
                {
#if DEBUG
                    Console.WriteLine("[dev] Exception in the Python API: file " + fname + ", line " + pos.ToString() + ".");
                    Console.WriteLine(ex.ToString());
#endif
                }
                newcache.Add(fname, entry);
            }
#if DEBUG
            Console.WriteLine("[dev] " + types.Count.ToString() + " Python mods loaded successfully.");
#endif
            if (types.Count > 0)
            {
                ReflectMan.AddTypes(types.ToArray());
                using (var stream = File.OpenWrite("pyasmcache.dat"))
                    AsmCache.Save(stream, newcache);
            }
            scanned = true;
        }
    }

#if DEBUG
    public static class PythonCmds
    {
        [Command("runpystring", description = "Run some Python code. (Only present in DEBUG builds of ShiftOS)")]
        [RequiresArgument("script")]
        public static bool RunPyString(Dictionary<string, object> args)
        {
            try
            {
                PythonHelper.RunCode(args["script"].ToString());
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return true;
        }
        
        [Command("runpyfile", description = "Run some Python code from a file. (Only present in DEBUG builds of ShiftOS)")]
        [RequiresArgument("script")]
        public static bool RunPyFile(Dictionary<string, object> args)
        {
            try
            {
                PythonHelper.RunCode(Objects.ShiftFS.Utils.ReadAllText(args["script"].ToString()));
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return true;
        }
    }
#endif
}
