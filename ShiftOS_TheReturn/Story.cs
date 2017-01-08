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
    public class Story
    {
        public static void RunFromInternalResource(string resource_id)
        {
            var t = typeof(Properties.Resources);

            foreach(var prop in t.GetProperties(System.Reflection.BindingFlags.NonPublic | BindingFlags.Static))
            {
                if(prop.Name == resource_id)
                {
                    if(prop.PropertyType == typeof(string))
                    {
                        WriteStory(prop.GetValue(null) as string);

                        return;
                    }
                }
            }
            throw new ArgumentException("Couldn't find resource ID inside the engine: " + resource_id);
        }


        public string Character { get; set; }
        public List<string> Lines { get; set; }

        public static void Start()
        {
            Console.WriteLine();
            if(SaveSystem.CurrentSave.StoryPosition == 5)
            {
                StartDevXLies();
            }
        }

        public static void StartDevXLies()
        {
            int chatProgress = 0;
            //bool LoopStuck = false;
            string textToWrite = "";
            const int TYPE_SPEED_MS = 45;
            bool done = false;
            bool write = true;

            while (done == false) {
                write = true;
                switch (chatProgress)
                {
                    case 0:
                        textToWrite = "User joined: @" + SaveSystem.CurrentSave.Username;
                        break;
                    case 1:
                        textToWrite = $"Hello, {SaveSystem.CurrentSave.Username}.";
                        break;
                    case 2: //If C:\ShiftOS doesn't exist the player won't notice this is here.
                        if (Directory.Exists(Paths.GetPath("classic")))
                        {
                            textToWrite = "I see you've participated in my previous ShiftOS experiment. Welcome back, Shifter. I assume you know lots about ShiftOS, but there are some updates I have to tell you.";
                        }
                        else
                        {
                            write = false;
                        }
                        break;
                    case 3: //DevX hates ShiftOS-Next secretly.
                        if (Directory.Exists(Paths.GetPath("classic") + "-Next"))
                        {
                            textToWrite = "Hmmmm.... looking at my sentience records, I see you've participated in ShiftOS-Next. This is gonna be difficult.";
                        }
                        else
                        {
                            write = false;
                        }
                        break;
                    case 4:
                        textToWrite = "There's a lot that has changed within ShiftOS.";
                        break;
                    case 5:
                        textToWrite = "First off, I want to tell you a bit about myself in case you don't already know.";
                        break;
                    case 6:
                        textToWrite = "My name is DevX. I am the architect of ShiftOS. I have chosen you to take part in helping me out with it.";
                        break;
                    case 7:
                        textToWrite = "You see, in my past attempts it has all been about an evolving operating system and seeing how the users work with it...";
                        break;
                    case 8:
                        textToWrite = "Almost one hundred percent of the time, people have found out it was an experiment and they could simply return to their regular system with a specific upgrade.";
                        break;
                    case 9:
                        textToWrite = "But now, I want to try something different - something unique.";
                        break;
                    case 10:
                        textToWrite = "ShiftOS is the same as it has been in my previous attempts, but now, your goal is to gain as much wealth and power as possible.";
                        break;
                    case 11:
                        textToWrite = "Right now you are inside my segregation LAN. Only you and me exist within this domain. You are free from other users unless I create them.";
                        break;
                    case 12:
                        textToWrite = "Since you have proved your sentience, I have a task for you outside the segregation LAN.";
                        break;
                    case 13:
                        textToWrite = "But first... you need to be taught a few things.";
                        break;
                    case 14:
                        textToWrite = "First off, when I bring you into my multi-user domain, you'll first want to establish as much wealth as possible.";
                        break;
                    case 15:
                        textToWrite = "Wealth comes in the form of Codepoints - a currency used among users of the multi-user domain.";
                        break;
                    case 16:
                        textToWrite = @"You can get Codepoints by doing the following:

 - Stealing them from other users
 - Extracting them from inactive/unverified sentiences
 - Using specific scripts/programs within ShiftOS
 - Creating paid scripts/applications within ShiftOS";
                        break;
                    case 17:
                        textToWrite = "You can use Codepoints to buy upgrades using the 'shiftorium.buy' command, or you can use them to pay other users, or scripts.";
                        break;
                    case 18:
                        textToWrite = "Within the multi-user domain you are free to do whatever you want. Larcany, theft, deceiving, lies, and distribution of malware is permitted under my watch.";
                        break;
                    case 19:
                        textToWrite = "Do whatever you have to to get Codepoints.";
                        break;
                    case 20:
                        textToWrite = "Then use them to make yourself stronger by buying upgrades at the shiftorium.";
                        break;
                    case 21:
                        textToWrite = "If you want to get a bit devious within the multi-user domain, look around for scripts that will expose user account information.";
                        break;
                    case 22:
                        textToWrite = "Or just spread a virus around the mud.";
                        break;
                    case 23:
                        textToWrite = "Or you can be the 'good' guy and stop these attacks and gain the trust of other users.";
                        break;
                    case 24:
                        textToWrite = "It's up to you. Just, don't mess with my system. You won't want me coming to you after that. I'm watching.";
                        break;
                    case 25:
                        textToWrite = "User left chat: @" + SaveSystem.CurrentSave.Username;
                        done = true;
                        SaveSystem.CurrentSave.StoryPosition++;
                        TerminalBackend.InvokeCommand("sos.save");
                        break;

                }

                if (write == true)
                {
                    Console.WriteLine();
                    Console.Write("DevX: ");
                    foreach(char c in textToWrite)
                    {
                        Console.Beep(750, TYPE_SPEED_MS);
                        if (c == '\n')
                        {

                        }
                        else if (c == '\r')
                        {
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.Write(c);
                        }
                    }
                    Thread.Sleep(1000);
                }
                chatProgress += 1;
            }
        }


        public static void WriteStory(string json)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                var story = JsonConvert.DeserializeObject<Story>(json);

                const int TYPESPEED = 45;

                foreach (var line in story.Lines)
                {
                    var localized = Localization.Parse(line);


                    if (line.StartsWith("cmd:"))
                    {
                        string[] cmdsplit = line.Replace("cmd:", "").Split(' ');
                        switch (cmdsplit[0])
                        {
                            case "givecp":
                                SaveSystem.TransferCodepointsFrom(story.Character, Convert.ToInt32(cmdsplit[1]));
                                break;
                        }
                    }
                    else
                    {
                        Console.Write(story.Character + ": ");

                        foreach (var c in localized)
                        {
                            Console.Beep(1024, TYPESPEED);
                            if (c == '\r')
                            {

                            }
                            else if (c == '\n')
                                Console.WriteLine();
                            else
                                Console.Write(c);
                        }

                        Console.WriteLine();
                        Thread.Sleep(1000);
                    }
                }
                Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
            }));
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
