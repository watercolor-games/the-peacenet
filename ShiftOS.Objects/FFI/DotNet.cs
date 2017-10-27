using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Plex.Objects.FFI
{
    /// <summary>
    /// The .NET 'foreign' function interface.
    /// This is the only special case implementation of the interface
    /// in the game, being explicitly specified in ReflectMan's static
    /// initialiser. As a result, it is guaranteed to be the first one
    /// used.
    /// All .NET assemblies in the same directory as the game executable
    /// will be loaded by this FFI and have their types placed in the
    /// returned list.
    /// </summary>
    internal class DotNet: IFFI
    {
        public IEnumerable<Type> GetTypes()
        {
            var ret = new List<Type>();
            // We find possible assemblies by extension. EXE files are
            // pretty much only loaded because part of the game itself
            // is located in one. Distributing mods as executables
            // sounds like a pretty dumb idea.
            foreach (string fname in Array.FindAll(System.IO.Directory.GetFiles(Environment.CurrentDirectory), n => n.EndsWith(".exe", true, null) || n.EndsWith(".dll", true, null)))
            {
                try
                {
                    // I use GetExportedTypes() now instead of
                    // GetTypes(). It's kind of surprising that the
                    // latter finds internal and private classes by
                    // default. You will notice that this very class is
                    // internal, so this means that it won't find
                    // itself.
                    ret.AddRange(Assembly.LoadFile(fname).GetExportedTypes());
                    Console.WriteLine($"Successfully loaded all types from {fname}.");
                }
                catch (ReflectionTypeLoadException rtlex)
                {
                    // This part is adapted from Mike's modifications to
                    // the original, monolithic version of ReflectMan,
                    // but it no longer waits for the user's response to
                    // a MessageBox (which would be a terrible idea on
                    // the server!).
                    Console.WriteLine($"Some types failed to load from {fname}:");
                    foreach (var cex in rtlex.LoaderExceptions.Where(c => c != null))
                    {
                        Console.WriteLine(cex);
                    }
                    Console.WriteLine("Attempting to append any successful types.");
                    try
                    {
                        // Somehow, we might still be able to load some
                        // types from the assembly.
                        ret.AddRange(rtlex.Types.Where(x => x != null));
                        Console.WriteLine("Success.");
                    }
                    catch (Exception ex)
                    {
                        // It doesn't seem likely that an exception
                        // would be thrown here unless the user ran out
                        // of memory or something but it can't hurt to
                        // catch it.
                        Console.WriteLine("Something else went wrong.  Oh well, you can't win them all, I guess.");
                        Console.WriteLine(ex);
                    }
                }
                catch (Exception ex)
                {
                    // The error handling in Plex's reflection API is
                    // fairly carefree. The monolithic version didn't
                    // even print the exception details but I decided it
                    // may be useful for modders.
                    // The original cause of exceptions here was the
                    // presence of non-.NET DLL files in the game
                    // directory, so it's usually nothing to worry
                    // about.
                    Console.WriteLine($"Something went wrong while loading {fname}, but it's probably nothing to worry about."); 
                    Console.WriteLine(ex);
                }
                Console.WriteLine("---");
            }
            return ret;
        }
    }
}
