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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Storyline management class.
    /// </summary>
    public static class Story
    {
        /// <summary>
        /// Starts the storyline with the specified Storyline ID.
        /// </summary>
        /// <param name="stid">The storyline ID to start.</param>
        public static void Start(string stid)
        {
            foreach (var exec in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
            {
                if(exec.EndsWith(".exe") || exec.EndsWith(".dll"))
                {
                    try
                    {
                        if (SaveSystem.CurrentSave.StoriesExperienced == null)
                            SaveSystem.CurrentSave.StoriesExperienced = new List<string>();
                        var asm = Assembly.LoadFile(exec);
                        foreach(var type in asm.GetTypes())
                        {
                            foreach(var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                            {
                                foreach(var attrib in mth.GetCustomAttributes(false))
                                {
                                    if(attrib is StoryAttribute)
                                    {
                                        var story = attrib as StoryAttribute;
                                        if(story.StoryID == stid)
                                        {
                                            new Thread(() =>
                                            {
                                                mth.Invoke(null, null);
                                                SaveSystem.CurrentSave.StoriesExperienced.Add(stid);
                                            }).Start();
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
#if DEBUG
            throw new ArgumentException("Story ID not found: " + stid + " - Talk to Michael. NOW.");
#else
            Debug.Print("No such story: " + stid);
#endif
        }

        [Obsolete("Please use Story.Start() in tandem with [StoryAttribute].")]
        public static void RunFromInternalResource(string resource_id)
        {
        }


    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StoryAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StoryAttribute"/> attribute. 
        /// </summary>
        /// <param name="id">The ID of this story plot.</param>
        /// <remarks>
        ///     <para>
        ///         The <see cref="StoryAttribute"/> is used to turn a static, public method into a story element. Using the specified <paramref name="id"/> argument, the ShiftOS Engine can determine whether this plot has already been experienced, and using the <see cref="Shiftorium"/> classes, the ID is treated as a special Shiftorium upgrade, and you can use the <see cref="RequiresUpgradeAttribute"/> attribute as well as the various other ways of determining whether a Shiftorium upgrade is installed to determine if this plot has been experienced.        
        /// </para>
        /// </remarks>
        public StoryAttribute(string id)
        {
            StoryID = id;
        }

        /// <summary>
        /// Gets the storyline ID stored in this attribute.
        /// </summary>
        public string StoryID { get; private set; }

    }
}
