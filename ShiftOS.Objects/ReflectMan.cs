using System;using System.Collections.Generic;using System.Linq;using System.Reflection;using System.Text;using System.Threading.Tasks;namespace Plex.Objects{    /// <summary>    /// Mirror, mirror on the wall. Manages a list of types found in Plex-related assemblies.    /// </summary>    public static class ReflectMan    {        private static Assembly[] asms = null;        /// <summary>        /// The list of found assemblies.        /// </summary>        public static Assembly[] Asms        {            get            {                if (asms == null)                    LoadFiles();                return asms;            }        }        private static Type[] types = null;        /// <summary>        /// The list of found types.        /// </summary>        public static Type[] Types        {            get            {                if (types == null)                {                    FindTypes();                }                return types;            }        }        public static bool Contains(this AssemblyName[] names, string name)
        {
            return names.FirstOrDefault(x => x.Name == name) != null;
        }        private static void LoadFiles()        {            var ret = new List<Assembly>();            foreach (var exe in Array.FindAll(System.IO.Directory.GetFiles(Environment.CurrentDirectory), n => n.EndsWith(".exe", true, null) || n.EndsWith(".dll", true, null)))                try                {                    var asm = Assembly.LoadFile(exe);                    ret.Add(asm);                }                catch { } // probably not a managed assembly            asms = ret.ToArray();        }        private static void FindTypes()        {            var ret = new List<Type>();            foreach (var asm in Asms)
            {                try
                {
                    ret.AddRange(asm.GetTypes());
                }
                catch (ReflectionTypeLoadException rtlex)
                {
                    bool suppressLoaderErrors = UserConfig.Get().SuppressTypeLoadErrors;
                    bool cont = suppressLoaderErrors;
                    if (!suppressLoaderErrors)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        messageBuilder.AppendLine($"An error has occured while loading the following assembly: {asm.FullName}");
                        messageBuilder.AppendLine();
                        messageBuilder.AppendLine(rtlex.Message);
                        messageBuilder.AppendLine();
                        messageBuilder.AppendLine("The following internal errors occurred:");
                        messageBuilder.AppendLine();
                        foreach(var cex in rtlex.LoaderExceptions)
                        {
                            if(cex != null)
                            {
                                messageBuilder.AppendLine(cex.GetType().Name + ": " + cex.Message);
                                messageBuilder.AppendLine();
                            }
                        }
                        messageBuilder.AppendLine($"Would you like to attempt to continue loading modules? Clicking 'No' will terminate the process while clicking 'Yes' will ignore the failing types and continue to load.");
                        var result = System.Windows.Forms.MessageBox.Show(
                            caption: "ReflectMan - Error loading types",
                            text: messageBuilder.ToString(),
                            buttons: System.Windows.Forms.MessageBoxButtons.YesNo,
                            icon: System.Windows.Forms.MessageBoxIcon.Error
                            );
                        cont = result == System.Windows.Forms.DialogResult.Yes;
                    }
                    if(cont == false)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        ret.AddRange(rtlex.Types.Where(x => x != null));
                    }
                }
            }
            types = ret.ToArray();        }        /// <summary>        /// Add extra types to the ReflectMan array after the scan is complete.        /// Shouldn't be public, but C# doesn't support "friend".        /// </summary>        /// <param name="newtypes">An array of types to append.</param>        public static void AddTypes(Type[] newtypes)        {            var oldlength = types.Length;            Array.Resize(ref types, oldlength + newtypes.Length);            newtypes.CopyTo(types, oldlength);        }    }}