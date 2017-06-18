using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Stories
{
    public class DevXSkinningStory
    {
        [Story("devx_1000_codepoints")]
        public static void DevX1000CPStory()
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                var t = new Applications.Terminal();
                AppearanceManager.SetupWindow(t);
            });
            TerminalBackend.InStory = true;
            TerminalBackend.PrefixEnabled = false;
            Thread.Sleep(3000);
            Engine.AudioManager.PlayStream(Properties.Resources._3beepvirus);
            Console.WriteLine("devx@anon_127420: Connection established.");
            Thread.Sleep(1500);
            Console.WriteLine("DevX: Hello, " + SaveSystem.CurrentUser.Username + "@" + SaveSystem.CurrentSave.SystemName + "! I see you've gotten a decent amount of Codepoints.");
            Thread.Sleep(2000);
            Console.WriteLine("DevX: Have you gotten a chance to install my \"Shifter\" application yet?");
            Thread.Sleep(1500);
            if (!Shiftorium.UpgradeInstalled("shifter"))
            {
                Console.WriteLine("You: Not yet. What's it for?");
                Thread.Sleep(2000);
                Console.WriteLine("DevX: The Shifter is a very effective way to make ShiftOS look however you want it to.");
                Thread.Sleep(2000);
                Console.WriteLine("DevX: It can even be adapted to support other applications, features and upgrades.");
                Thread.Sleep(2000);
            }
            else
            {
                Console.WriteLine("You: Yeah. Just seems kinda lackluster to me. What is it supposed to do?");
                Thread.Sleep(2000);
                Console.WriteLine("DevX: The Shifter is a very effective way to make ShiftOS look however you want it to.");
                Thread.Sleep(2000);
                Console.WriteLine("DevX: It can even be adapted to support other applications, features and upgrades.");
                Thread.Sleep(2000);
                Console.WriteLine("DevX: I haven't finished it just yet. Keep upgrading it and you'll notice it gets a lot better.");
                Thread.Sleep(2000);
            }
            Console.WriteLine("DevX: I'd also recommend going for the Skin Loader, that way you can save your creations to the disk. Also, go for the Skinning upgrade - which will allow more rich customization of certain elements.");
            Thread.Sleep(2000);
            Console.WriteLine("You: This still ain't gonna help me get back to my old system and out of this stupid Digital Society, is it?");
            Thread.Sleep(2000);
            Console.WriteLine("DevX: How the...How do you know about the Digital Societ....Who the hell talked!?");
            Thread.Sleep(2000);
            Console.WriteLine("You: Whoa! Just... calm down, will ya? I heard about it in the news, shortly before I got infected by this damn virus of an operating system.");
            Thread.Sleep(2000);
            Console.WriteLine("DevX: Whatever. That doesn't matter yet. Just focus on upgrading ShiftOS and earning Codepoints. I'll let you know when we're done. I've gotta go...work on something else.");
            Thread.Sleep(1500);
            Console.WriteLine("User disconnected.");
            Thread.Sleep(2000);
            Console.WriteLine("You: Something doesn't seem right about DevX. I wonder what he's really up to.");

            if (Shiftorium.UpgradeInstalled("shifter"))
                PushObjectives();
            else
                Story.PushObjective("Buy the Shifter.", "The Shifter is a super-effective way to earn more Codepoints. It is an essential buy if you haven't already bought it. Save up for the Shifter, and buy it using shiftorium.buy{upgrade:\"shifter\"}.", () =>
                {
                    return Shiftorium.UpgradeInstalled("shifter");
                }, () =>
                {
                    PushObjectives();
                });
            Story.Context.AutoComplete = false;
            TerminalBackend.InStory = false;
            TerminalBackend.PrefixEnabled = false;
            TerminalBackend.PrintPrompt();
        }

        public static void PushObjectives()
        {
            Story.PushObjective("Buy the Skinning upgrade", "The Skinning upgrade will allow you to set pictures in place of solid colors for most UI elements. If you want richer customization and more Codepoints, this upgrade is a necessity.", () =>
            {
                return Shiftorium.UpgradeInstalled("skinning");
            }, ()=>
            {
                Story.PushObjective("Buy the Skin Loader.", "The Skin Loader is an application that allows you to save and load .skn files containing Shifter skin data. These files can be loaded in to the Skin Loader and applied to the system to give ShiftOS a completely different feel. It's Shiftorium upgrade ID is \"skin_loader\".", () =>
                {
                    return Shiftorium.UpgradeInstalled("skin_loader");
                },
                () =>
                {
                    Story.Context.MarkComplete();
                });
            });
        }
    }
}
