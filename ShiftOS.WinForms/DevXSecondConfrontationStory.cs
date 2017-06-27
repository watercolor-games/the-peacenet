using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShiftOS.Engine;
using static ShiftOS.WinForms.Stories.LegionStory;

namespace ShiftOS.WinForms
{
    public static class DevXSecondConfrontationStory
    {
#if BETA_4
        [RequiresUpgrade("appscape_troubles_end")]
        [Mission("sentiences_like_gods", "Sentiences Like Gods", "You're just a script-kiddie with that puny brute app. Let's do some true hacking.", 850l, "maureen_fenn")]
        public static void SentiencesLikeGods()
        {
            Story.Context.AutoComplete = false;
            Applications.Chat chat = null;
            Desktop.InvokeOnWorkerThread(() =>
            {
                chat = OpenChat();
            });
            while (chat == null)
                Thread.Sleep(10);
            CurrentChat = chat;
            chat.ChatID = "devx@system";
            chat.ShowChat();
            SendChatMessage("maureen_fenn", "Hey, you remember when I told you to come see me if you wanted to become a real hacker?");
            SendChatMessage("maureen_fenn", "Well, it's now time you learn about port scanners, firewalls, and malware injection.");
            SendChatMessage("maureen_fenn", "You know, not everything involves password cracking and stealing from wide-open FTP servers.");
            SendChatMessage("maureen_fenn", "That's baby talk.")                    ;
            SendChatMessage("maureen_fenn", "Have you ever felt the rush of breaching the root account of a Codepoint bank and going on a 60-second long withdrawl-spree to see how much Codepoints you can get from as many users as possible?");
            SendChatMessage("maureen_fenn", "Or maybe you felt like droppin' a little script on your buddy's Shiftnet website that makes it so everytime someone buys something, half the cash goes to you?");
            SendChatMessage("maureen_fenn", "Or maybe you want to spy on someone, see what they're doing, without them knowing?");
            SendChatMessage("maureen_fenn", "Whatever the case may be, brute is not going to help with that.");
            SendChatMessage("maureen_fenn", "What you need is my much more advanced and sophisticated toolset, consisting of a port scanner, network monitor, IP spoofer, and yes, an unreasonably convoluted user-interface.");
            SendChatMessage("maureen_fenn", "Also, you may want to get yourself a better window manager if you're still on the tiling WM. The WM Free Placement upgrade should do the trick.");
            SendChatMessage("maureen_fenn", "If you want to download my toolset, head to this underground Shiftnet URL: downunder/maureen_fenn/hacker_suite");
            SendChatMessage("maureen_fenn", "Actually, you'll need it for our DevX exposition quest, so go get it now.");

            Action onGotWMFreePlacement = () =>
            {
                Story.PushObjective("Sentiences Like Gods: Go down under.", "Head to the Shiftnet URL that Maureen sent you to get the advanced hacker's tool suite.", () =>
                {
                    return Shiftorium.UpgradeInstalled("mf_hackertools");
                },
                () =>
                {
                    SendChatMessage("maureen_fenn", "Alright. When you're ready for a crash course, let me know.");
                    Story.Context.MarkComplete();
                });
            };
            if (Shiftorium.UpgradeInstalled("wm_free_placement"))
            {
                onGotWMFreePlacement.Invoke();
            }
            else
            {
                SendChatMessage("maureen_fenn", "First, let's get you a better window manager.");
                Story.PushObjective("Sentiences Like Gods: A new window manager", "An application is only as good as its user interface allows it to be. That's the same for an operating system. Eventually, you'll need to be able to run more than just 4 programs at once, and you'll need a window manager that can place program windows of any size anywhere on screen. There are Shiftorium Upgrades for that. Let's get some.", () => { return Shiftorium.UpgradeInstalled("wm_free_placement") && Shiftorium.UpgradeInstalled("wm_unlimited_windows"); }, onGotWMFreePlacement);

            }

        }
#endif

        [RequiresUpgrade("appscape_troubles_end")]
        [Mission("devx_first_confrontation", "Progress Report", "DevX wants to see how much you've upgraded ShiftOS. He also has something to tell you.", 0l, "devx")]
        public static void DevXProgressReport()
        {
            Applications.Chat chat = null;
            Desktop.InvokeOnWorkerThread(() =>
            {
                chat = OpenChat();
            });
            while (chat == null)
                Thread.Sleep(10);
            CurrentChat = chat;
            chat.ChatID = "devx@system";
            chat.ShowChat();
            SendChatMessage("devx", "Greetings, " + SaveSystem.CurrentUser.Username);
            SendChatMessage("devx", "I guess it is time to assess your system.");
            SendChatMessage("devx", "After all, you've gotten " + SaveSystem.CurrentSave.CountUpgrades() + " Shiftorium Upgrades since I've last contacted you.");
            SendChatMessage("devx", "And you have a Codepoint balance of " + SaveSystem.CurrentSave.Codepoints + ".");
            SendChatMessage("devx", "And...oh wait... what!? You found the... How did you find the Shiftnet!?");
            SendChatMessage("devx", "Naughty, naughty user. You're not supposed to be on there.");
            SendChatMessage("devx", "Whatever. I've got a task for you.");
            SendChatMessage("devx", "Your logs show me you're in contact with Maureen Fenn.");
            SendChatMessage("devx", "I have a document to send her. You're my messanger.");
            SendChatMessage("devx", "Though, like any good messanger, I urge you not to open it.");
            SendChatMessage("devx", "<sent a file: unknown>");
            SendChatMessage("devx", "When you get it, just send it to her. Do not delete it.");
            bool fileWritten = false;
            Desktop.InvokeOnWorkerThread(() =>
            {
                FileSkimmerBackend.GetFile(new[] { ".rtf" }, FileOpenerStyle.Save, (loc) =>
                 {
                     var bytes = Convert.FromBase64String(Properties.Resources.DevXMaureenLetter);
                     Objects.ShiftFS.Utils.WriteAllBytes(loc, bytes);
                     fileWritten = true;
                 });
            });
            while (fileWritten == false)
                Thread.Sleep(10);
        }
    }
}