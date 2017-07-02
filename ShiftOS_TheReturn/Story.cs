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
    public class StoryContext
    {
        public string Id { get; set; }
        public MethodInfo Method { get; set; }
        public bool AutoComplete = false;

        public StoryContext()
        {
            AutoComplete = true;
        }

        public void MarkComplete()
        {
            SaveSystem.CurrentSave.StoriesExperienced.Add(Id);
            OnComplete?.Invoke();
        }

        public event Action OnComplete;
    }

    public class Objective
    {
        private Func<bool> _completeFunc = null;

        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsComplete
        {
            get
            {
                return (bool)_completeFunc?.Invoke();
            }
        }

        public Objective(string name, string desc, Func<bool> completeFunc, Action onComplete)
        {
            _completeFunc = completeFunc;
            Name = name;
            Description = desc;
            this.onComplete = onComplete;
        }

        private Action onComplete = null;

        public void Complete()
        {
            Thread.Sleep(20);
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// Storyline management class.
    /// </summary>
    public static class Story
    {
        public static StoryContext Context { get; private set; }
        public static event Action<string> StoryComplete;
        public static List<Objective> CurrentObjectives { get; private set; }

        public static void PushObjective(string name, string desc, Func<bool> completeFunc, Action onComplete)
        {
            if (CurrentObjectives == null)
                CurrentObjectives = new List<Objective>();

            var currentObjective = new Objective(name, desc, completeFunc, onComplete);
            CurrentObjectives.Add(currentObjective);
            var t = new Thread(() =>
            {
                var obj = currentObjective;
                while (!obj.IsComplete)
                {
                    Thread.Sleep(5000);
                }
                Thread.Sleep(500);
                CurrentObjectives.Remove(obj);
                obj.Complete();
            });
            t.IsBackground = true;
            t.Start();

            Console.WriteLine();
            ConsoleEx.ForegroundColor = ConsoleColor.Red;
            ConsoleEx.Bold = true;
            Console.WriteLine("NEW OBJECTIVE:");
            Console.WriteLine();

            ConsoleEx.ForegroundColor = ConsoleColor.White;
            ConsoleEx.Bold = false;
            Console.WriteLine("A new objective has been added to your system.");
            ConsoleEx.Bold = true;
            Console.WriteLine(name);
            ConsoleEx.Bold = false;
            Console.WriteLine();
            Console.WriteLine(desc);
            Console.WriteLine();
            Console.WriteLine("Run 'status' at any time to view your current objectives.");
            TerminalBackend.PrintPrompt();
        }
        
        
        /// <summary>
        /// Starts the storyline with the specified Storyline ID.
        /// </summary>
        /// <param name="stid">The storyline ID to start.</param>
        public static void Start(string stid)
        {
            if (SaveSystem.CurrentSave.StoriesExperienced == null)
                SaveSystem.CurrentSave.StoriesExperienced = new List<string>();
            foreach (var type in ReflectMan.Types)
            {
                foreach (var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    foreach (var attrib in Array.FindAll(mth.GetCustomAttributes(false), a => a is StoryAttribute || a is MissionAttribute))
                    {
                        var story = attrib as StoryAttribute;
                        if (story.StoryID == stid)
                        {
                            new Thread(() =>
                            {
                                Context = new Engine.StoryContext
                                {
                                    Id = stid,
                                    Method = mth,
                                    AutoComplete = true,
                                };
                                SaveSystem.CurrentSave.PickupPoint = Context.Id;
                                Context.OnComplete += () =>
                                {
                                    if(story is MissionAttribute)
                                    {
                                        var mission = story as MissionAttribute;
                                        ConsoleEx.ForegroundColor = ConsoleColor.Yellow;
                                        ConsoleEx.Bold = true;
                                        Console.WriteLine(" - mission complete - ");
                                        ConsoleEx.Bold = false;
                                        ConsoleEx.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine($"{mission.Name} successfully finished. You have earned {mission.CodepointAward} Codepoints for your efforts.");
                                        SaveSystem.CurrentSave.Codepoints += mission.CodepointAward;
                                        TerminalBackend.PrintPrompt();
                                        TerminalBackend.PrefixEnabled = true;
                                    }
                                    StoryComplete?.Invoke(stid);
                                    SaveSystem.CurrentSave.PickupPoint = null;
                                };
                                mth.Invoke(null, null);
                                if (Context.AutoComplete)
                                {
                                    Context.MarkComplete();
                                }
                            }).Start();
                            return;
                        }
                    }
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

        public ulong CodepointAward { get; protected set; }
    }
}
