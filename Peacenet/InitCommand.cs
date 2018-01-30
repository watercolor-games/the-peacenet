using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.Filesystem;
using Plex.Engine.Saves;
using WatercolorGames.CommandLine;
using Peacenet.RichPresence;
using Peacenet.Server;

namespace Peacenet
{
    /// <summary>
    /// A command which runs when the in-game Peacegate OS boots.
    /// </summary>
    [HideInHelp]
    public class InitCommand : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Initiates the Peacegate userland. Cannot be run outside of kernel!";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "init";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        [Dependency]
        private Plexgate _plexgate = null;

        private IEnumerable<string> startupLines
        {
            get
            {
                yield return "Identifying CPU...";
                yield return "Counting available RAM...";
                yield return "Identifying graphics adapters...";
                yield return $"{GraphicsAdapter.Adapters.Count} Graphics Adapters Found";
                yield return "Looking for kernel modules...\n\n";
                yield return "Loaded module: Terminal";
                yield return "Loaded module: Network";
                yield return "Loaded module: Bluetooth";
                yield return "Loaded module: Filesystem";
                yield return (GraphicsAdapter.DefaultAdapter.IsWideScreen) ? "Loaded module: High Definition Graphics Device" : "Standard Definition Graphics Device";
                yield return "Loaded module: Peacegate Desktop";
                yield return "Loaded module: Integrated NoSQL database";
                yield return "<filesystem> Checking files and directories...";
                yield return $"<filesystem> {_countDirs("/")} directories and {_countFiles("/")} files found.\n";
                yield return "Loading userland...";
                yield return (_api.LoggedIn) ? $"User: {_api.User.username}" : "User: <unknown>";
                yield return (_fs.FileExists("/etc/hostname")) ? "Hostname: 127.0.0.1" : "Hostname: " + _fs.ReadAllText("/etc/hostname");
                yield return "Peacegate Kernel Loaded Successfully!";
            }
        }

        [Dependency]
        private ItchOAuthClient _api = null;

        [Dependency]
        private FSManager _fs = null;

        private int _countFiles(string dir)
        {
            int dircount = 0;
            foreach (var subdir in _fs.GetDirectories(dir))
            {
                if (subdir.EndsWith("."))
                    continue;
                dircount += _countFiles(subdir);
            }
            foreach(var file in _fs.GetFiles(dir))
            {
                dircount++;
            }
            return dircount;
        }

        [Dependency]
        private OS _os = null;

        private int _countDirs(string dir)
        {
            int dircount = 0;
            foreach(var subdir in _fs.GetDirectories(dir))
            {
                if (subdir.EndsWith("."))
                    continue;
                dircount++;
                dircount += _countDirs(subdir);
            }
            return dircount;
        }

        [Dependency]
        private AsyncServerManager _server = null;

        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private TerminalManager _terminal = null;

        [Dependency]
        private DiscordRPCModule _discord = null;

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            if (_os.IsDesktopOpen)
            {
                console.WriteLine("Error: Attempted to initiate kernel inside userland.");
                return;
            }
            bool hasDoneTutorial = false;
            bool isSinglePlayer = !_server.IsMultiplayer;
            if (isSinglePlayer)
            {
                hasDoneTutorial = _save.GetValue<bool>("boot.hasDoneCmdTutorial", hasDoneTutorial);
            }
            console.SlowWrite("Peacegate Kernel v1.4");
            Thread.Sleep(60);
            console.WriteLine("");
            console.SlowWrite("-------------------------------------");
            console.WriteLine("");
            console.WriteLine("");
            Thread.Sleep(500);
            foreach(var line in startupLines)
            {
                Thread.Sleep(100);
                console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {line}");
            }

            console.WriteLine("");
            console.WriteLine("Starting Peacegate Session!");
            Thread.Sleep(75);
            console.WorkingDirectory = "/home";
            if (!hasDoneTutorial)
            {
                _discord.GameState = "Peacegate OS 1.4: IN MISSION";
                _discord.GameDetails = "Prologue - Command-line Tutorial";
                var interpreter = _plexgate.New<TutorialBash>();
                interpreter.ShowHostInfo = false;
                interpreter.AllowCommands = false;
                console.SlowWrite("Welcome to Peacegate OS.\n");
                console.SlowWrite("It seems this is your first time using Peacegate.\n");
                console.SlowWrite("If that's the case, it'll help to actually know how to use it.\n");
                console.SlowWrite("If not, then treat this as a practice test.\n");
                console.SlowWrite("We'll start with a simple lesson on how to control your system.\n");
                console.SlowWrite("Below is a Peacegate Command Shell. To continue, exit the shell.\n");
                console.SlowWrite("Exiting the shell will cause the operating system to continue executing this tutorial program. Until then, this guide will cease to continue.\n");
                console.SlowWrite("Starting Command Shell...\n");
                interpreter.Run(console, new Dictionary<string, object>());
                console.WriteLine("Objective complete.");
                console.SlowWrite("You have demonstrated barebones knowledge of how to control your system through a Terminal.\n");
                console.SlowWrite("However, it is not all about entering a single command and watching what it does.\n");
                console.SlowWrite("In Peacegate OS, commands can be strung together, with each command piping its output to the next command until you see the result of the final command in the sequence.\n");

                cowsay:
                console.SlowWrite("You can pipe two or more commands together by separating them with a Pipe character \"|\".\n");
                console.SlowWrite("Give it a try. Try making a cow say something. When you're finished, exit the shell to continue.\n");
                console.SlowWrite("Remember that you can always type 'help' for a list of commands, and 'man <command>' for details on how to use a specific command.\n");
                interpreter.AllowCommands = true;
                interpreter.Run(console, new Dictionary<string, object>());
                var cowsaySuccess = interpreter.CommandsRun.Any(x => x.OutputFile == null && x.Commands.Length > 1 && x.Commands.Last() == "cowsay");
                if (!cowsaySuccess)
                {
                    console.SlowWrite("Objective failure!\nYou were unable to get a cow to say something using a pipe. Please try again!\n");
                    console.SlowWrite("Reminder: ");
                    goto cowsay;
                }
                console.SlowWrite("Good job! Who knew making something like that happen could be so easy!\n");
                console.SlowWrite("Do note that making cows talk isn't the only thing you can do with pipe sequences. You will encounter many commands which will require a pipe sequence to work properly.\n");
                console.SlowWrite("Your shell has been upgraded to allow you to see some information about where you are in Peacegate.\n");
                console.SlowWrite("This includes your current username, system hostname, and working directory.\n");
                console.SlowWrite("Your shell will show a prompt in the format of \"username@hostname:workingdir$ \".\n");
                WorkDir:
                console.SlowWrite("Your working directory represents a folder on your hard drive where commands can access files in. Using the 'cd <folderpath>' command, you can change your working directory.\n");
                console.SlowWrite("The 'ls' command lists the files and folders in your working directory.\n");
                console.SlowWrite("Using these two commands, change to the '/etc' folder on your hard drive, and list its contents. Be sure to stay in the '/etc' folder when you exit the shell!\n");
                interpreter.ShowHostInfo = true;
                interpreter.Run(console, new Dictionary<string, object>());
                if (!(console.WorkingDirectory == "/etc" && interpreter.CommandsRun.Any(x => x.OutputFile == null && x.Commands.Any(y => y.StartsWith("ls")))))
                {
                    console.SlowWrite("Objective failure!\n");
                    console.SlowWrite("You didn't list the contents of the /etc directory! Try again!\n");
                    console.SlowWrite("Reminder: ");
                    goto WorkDir;
                }
                console.SlowWrite("Good work. Now let's talk about output redirection.\n");
                console.SlowWrite("Output Redirection allows you to tell a command to write its console output to a file rather than the console itself.\n");
                console.SlowWrite("This allows you to edit the contents of any file on your hard drive with relative ease, right from your command line!\n");
                console.WriteLine("");
                console.SlowWrite("There are two types of output redirection you can perform - overwriting redirection and appending redirection.\n");
                console.SlowWrite("Overwriting Redirection will erase the contents of the specified file if it already exists, replacing them with the command's output.\n");
                console.SlowWrite("Appending Redirection will simply add the command's output to the end of the file's contents.\n");
                console.SlowWrite("To redirect command output to a file, simply add '> filepath' to the end of the command line - where 'filepath' is an absolute path to a file, or a path to a file relative to your working directory.\n");
                console.SlowWrite("For an Appending Redirection, add '>> filepath' instead.\n");
                console.SlowWrite("Peacegate OS has a file in your 'etc' folder called 'hostname'. The contents of this file tells Peacegate OS what to call your computer when other computers ask it what it's name is.\n");
                console.SlowWrite("It is also displayed in your command shell.\n");
                OverwritingRedirection:
                console.SlowWrite("Try overwriting your '/etc/hostname' file using the 'echo' command and an Overwriting Redirection to change your computer's name. Exit the shell once you're done.\n");
                interpreter.Run(console, new Dictionary<string, object>());
                if(!interpreter.CommandsRun.Any(x=>x.OutputFile.Contains("hostname") && x.OutputFileType == OutputFileType.Overwrite && x.Commands.Any(y=>y.StartsWith("echo "))))
                {
                    console.SlowWrite("Objective failure!\n");
                    console.SlowWrite("You didn't change your computer's name using command redirection!\n");
                    console.SlowWrite("Reminder: ");
                    goto OverwritingRedirection;
                }
                console.SlowWrite("You've done a good job. You know the basics of how to use a Peacegate OS command shell. This will certainly come in handy in your adventures.\n");
                console.SlowWrite("We'll continue setting up your system from here. Once we're done, we'll teach you how to use the Peacegate Desktop GUI.\n");
                console.SlowWrite("END OF TUTORIAL . . . . . . . . . . . . . .");
                _save.SetValue<bool>("boot.hasDoneCmdTutorial", true);
                console.WriteLine("Database updated.");
                Thread.Sleep(75);
            }
        }
    }

    /// <summary>
    /// Provides a shell for the Terminal which can be arbitrarily locked down as part of a tutorial.
    /// </summary>
    [HideInHelp]
    [TerminalSkipAutoload]
    public class TutorialBash : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Dependency]
        private Plexgate _plexgate = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private ItchOAuthClient _Api = null;

        private bool _showHostInfo = false;
        private bool _allowCommands = false;

        /// <summary>
        /// Retrieves a list of all commands run by the player in this shell.
        /// </summary>
        public readonly List<CommandInstruction> CommandsRun = new List<CommandInstruction>();

        /// <summary>
        /// Gets or sets whether system commands may be run in the shell.
        /// </summary>
        public bool AllowCommands
        {
            get
            {
                return _allowCommands;
            }
            set
            {
                _allowCommands = value;
            }
        }
        
        /// <summary>
        /// Gets or sets whether the shell should show info about the player's system in the prompt text.
        /// </summary>
        public bool ShowHostInfo
        {
            get
            {
                return _showHostInfo;
            }
            set
            {
                _showHostInfo = value;
            }
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            CommandsRun.Clear();
            var realInterpreter = _plexgate.New<Applications.ShellCommand>();
            string hostname = "127.0.0.1";
            if (_fs.FileExists("/etc/hostname"))
                hostname = _fs.ReadAllText("/etc/hostname");
            string user = "user";
            string workdir = "/home";
            while (true)
            {
                if (_Api.LoggedIn)
                    user = _Api.User.username;
                else
                    user = "user";
                console.SetColors(Plex.Objects.ConsoleColor.Black, Plex.Objects.ConsoleColor.Gray);
                if (_showHostInfo)
                {
                    console.Write($"{user}@{hostname}:{console.WorkingDirectory.Replace("/home", "~")}$ ");
                }
                else
                {
                    console.Write("$ ");
                }
                try
                {
                    string cmdstr = console.ReadLine();
                    if (string.IsNullOrWhiteSpace(cmdstr))
                        continue;
                    if (cmdstr == "exit")
                        return;
                    if (_allowCommands)
                    {
                        realInterpreter.ProcessCommand(console, cmdstr);
                        CommandsRun.Add(Tokenizer.GetCommandList(cmdstr));
                    }
                    else
                    {
                        console.WriteLine("You have not yet unlocked the ability to run system commands yet!");
                    }
                }
                catch (Exception ex)
                {
                    console.WriteLine(ex.Message);
                }
            }

        }
    }
}
