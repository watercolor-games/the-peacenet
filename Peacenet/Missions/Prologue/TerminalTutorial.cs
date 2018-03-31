using Microsoft.Xna.Framework.Audio;
using Peacenet.Applications;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Engine.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.Threading;

namespace Peacenet.Missions.Prologue
{
    /// <summary>
    /// The very first Peacenet mission.
    /// </summary>
    public class TerminalTutorial : Mission
    {
        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private WindowSystem _winsys = null;

        [Dependency]
        private Plexgate _plexgate = null;

        private SoundEffect _bgm = null;
        private SoundEffectInstance _bgmInstance = null;

        /// <inheritdoc/>
        public override bool Available
        {
            get
            {
                return _save.GetValue("boot.hasDoneCmdTutorial", false);
            }
        }

        /// <inheritdoc/>
        public override string Description
        {
            get
            {
                return "Welcome to your new Peacegate environment. Now that you know the basics of the Peacegate OS GUI, it is time for you to learn how to use the Terminal.";
            }
        }

        /// <inheritdoc/>
        public override string Name
        {
            get
            {
                return "The Terminal";
            }
        }

        /// <inheritdoc/>
        public override void OnStart()
        {
            timeWasted = 0;
            _bgm = _plexgate.Content.Load<SoundEffect>("Audio/Mission1");
            _bgmInstance = _bgm.CreateInstance();
            _bgmInstance.IsLooped = true;
            _bgmInstance.Play();
            
            base.OnStart();
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            if(_tutorialTerminal != null)
            {
                _tutorialTerminal.CanClose = true;
                _tutorialTerminal.Close();
                _tutorialTerminal = null;
            }
            _shell = null;
            _bgmInstance.Stop();
            base.OnEnd();
        }

        private double timeWasted = 0;

        private Terminal _tutorialTerminal = null;

        [Dependency]
        private TerminalManager _terminalManager = null;

        [Dependency]
        private WindowSystem _Winsys = null;

        private TutorialShell _shell = null;

        

        public override void OnObjectiveStart(int index, Objective data)
        {
            if (index > 0)
                return;
            _terminalManager.LoadCommand(_shell = new TutorialShell());
            _tutorialTerminal = new TutorialTerminal(_winsys, this);
            _tutorialTerminal.CanClose = false;
            _tutorialTerminal.Show();
            _terminalManager.UnloadCommand("tutorial_cli");

        }

        /// <inheritdoc/>
        public override IEnumerable<Objective> ObjectiveList
        {
            get
            {
                yield return new Objective("Learn how to run commands", "Follow the instructions on this Terminal to learn how to execute commands.", (time) =>
                {
                    return (_shell.HasLearnedHelp) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Learn how to run commands with arguments", "Running commands with bash in your Terminal is great, but only if the command doesn't need extra input. Arguments are a way to provide that input.", (time) =>
                {
                    return (_shell.HasLearnedArguments) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Learn how to use the 'man' command.", "Sometimes, you just need to read the freakin' manual. Well, we don't mean that in a rude way. Sometimes, you just need to know the right way to use a command. The Unix manual ('man') command will help you out with that.", (time) =>
                {
                    return (_shell.HasLearnedManPages) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Learn Unix console redirection", "Ever wanted to string multiple commands together? Or write the output of a command to a text file? Or maybe both at the same time? Unix console redirection is how you do that!", (time) =>
                {
                    return (_shell.HasLearnedUnixRedirection) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
            }
        }
    }

    public class TutorialTerminal : Terminal
    {
        private TerminalTutorial _mission = null;

        protected override string Shell
        {
            get
            {
                return "tutorial_cli";
            }
        }

        public TutorialTerminal(WindowSystem winsys, TerminalTutorial mission) : base(winsys)
        {

        }
    }

    [TerminalSkipAutoload]
    [HideInHelp]
    public class TutorialShell : ITerminalCommand
    {
        [Dependency]
        private Plexgate _plexgate = null;
        
        private ShellCommand _shell = new ShellCommand();

        public string Description
        {
            get
            {
                return "";
            }
        }

        public string Name
        {
            get
            {
                return "tutorial_cli";
            }
        }

        private bool _hasLearnedHelp = false;
        private bool _hasLearnedArguments = false;
        private bool _hasLearnedUnixRedirection = false;

        public bool HasLearnedUnixRedirection
        {
            get
            {
                return _hasLearnedUnixRedirection;
            }
        }

        public bool HasLearnedArguments
        {
            get
            {
                return _hasLearnedArguments;
            }
        }

        public bool HasLearnedHelp
        {
            get
            {
                return _hasLearnedHelp;
            }
        }

        private bool _hasLearnedManPages = false;

        public bool HasLearnedManPages
        {
            get
            {
                return _hasLearnedManPages;
            }
        }

        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            _plexgate.Inject(_shell);
            console.SlowWrite("Welcome to the Peacegate OS Terminal.\n");
            Thread.Sleep(1000);
            console.SlowWrite("You are currently running the Tutorial Command Interpreter.\n");
            Thread.Sleep(1000);
            console.SlowWrite("This program will show you the basics of using bash, Peacegate OS's command interpreter program.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Bash is a command-line tool used by many Unix-based operating systems. Its ubiquity means it is very important to learn it if you want to be a poweruser.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Since it is used by Peacegate OS, Bash is an essential skill if you want to help save The Peacenet.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Below is the Bash command line. You'll be able to tell when you are in Bash as the command line will end with a '$'. To continue, run the 'help' command to see a list of commands.\n");
            string commandInput = null;
            console.Write("$ ");
            while (!(commandInput = console.ReadLine()).ToLower().StartsWith("help"))
            {
                console.SlowWrite("That wasn't the right command. Please type 'help'.");
                console.Write("$ ");
            }
            _shell.ProcessCommand(console, commandInput);

            _hasLearnedHelp = true;

            console.WriteLine("");

            console.SlowWrite("Running commands is great, but what if they need extra input from you?\n");
            Thread.Sleep(1000);
            console.SlowWrite("Well, bash has you covered. After you type the name of your command (all one word), everything after it is considered arguments.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Each argument is separated by a space. But if you need to put a space in your argument, you can surround it in double-quotes or put a single back-slash before your space.\n");
            Thread.Sleep(1000);
            console.SlowWrite("For example: '\"This is all one argument\"' and 'This\\ is\\ all\\ one\\ argument' mean the same thing.\n");
            Thread.Sleep(1000);
            console.SlowWrite("You can test this out using the 'echo' command. Try making the Terminal say something using the 'echo' command!\n");
            Thread.Sleep(1000);

            echo:
            console.Write("$ ");
            string echoInput = console.ReadLine();
            if(!echoInput.ToLower().StartsWith("echo"))
            {
                console.SlowWrite("That's not the right command. You need to use the 'echo' command.");
                goto echo;
            }
            var parsed = WatercolorGames.CommandLine.Tokenizer.TokenizeString(echoInput);
            if(parsed.Length != 2)
            {
                console.SlowWrite("You didn't specify the right amount of arguments for the command! Please try again!");
                goto echo;
            }
            _shell.ProcessCommand(console, echoInput);

            _hasLearnedArguments = true;

            console.WriteLine("");

            console.SlowWrite("Good work! You're starting to learn the basics. Of course, not all commands will be that simple to use.\n");
            Thread.Sleep(1000);

            console.SlowWrite("Fortunately, as part of the peacegate.coreutils package, there is a special command called 'man'. This command will show you useful information about a given Peacegate command, such as what it does and what arguments it needs.\n");
            Thread.Sleep(1000);

            console.SlowWrite("'man' will come in handy as you find more and more programs for your Peacegate OS. Luckily, it is just as easy to use as 'echo', and only takes one argument - the name of the command to get info about.\n");
            Thread.Sleep(1000);

            console.SlowWrite("Why not give it a try? Check out the manual entry for the 'themeconf' command. This is the command-line utility that was used by Peacegate OS Setup to set your wallpaper and accent color.\n");
            Thread.Sleep(1000);

            man:
            console.Write("$ ");
            string manInput = console.ReadLine();
            if(!manInput.ToLower().StartsWith("man"))
            {
                console.SlowWrite("That wasn't the man command! Try again.");
                goto man;
            }
            var manparsed = WatercolorGames.CommandLine.Tokenizer.TokenizeString(manInput);
            if(manparsed.Length != 2)
            {
                console.SlowWrite("You didn't specify the right amount of arguments for the 'man' command! Try again.");
                goto man;
            }
            if(manparsed[1] != "themeconf")
            {
                console.SlowWrite("You didn't request info for the 'themeconf' command! Try again.");
                goto man;
            }
            _shell.ProcessCommand(console, manInput);
            console.WriteLine("");

            console.SlowWrite("As you can see, 'help' and 'man' are both very useful for learning the different commands and programs you can run in Peacegate OS.\n");
            Thread.Sleep(1000);

            console.SlowWrite("'help' will show you a list of all available commands, and 'man' will tell you more information about a single command.\n");
            Thread.Sleep(1000);

            console.SlowWrite("Use them together, and you'll be a Peacegate power user in no time.\n");
            Thread.Sleep(5000);

            _hasLearnedManPages = true;

            console.Clear();
            console.SlowWrite("The next thing you need to know before you can start being a Bash pro is Unix console redirection and piping.\n");
            Thread.Sleep(5000);

            console.SlowWrite("Piping allows you to string a bunch of commands together, having the output of one command used as the input for the next, and so on, until the final output is displayed to your screen.\n");
            Thread.Sleep(1000);
            console.SlowWrite("You'd be surprised how handy this will come in your adventures.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Console redirection allows you to save the final output of a single command or a sequence of piped commands to a file.\n");
            Thread.Sleep(1000);
            console.SlowWrite("You can have the final output either appended to an existing file or have it overwrite a file. In either case, if the file doesn't exist, it will be created and filled with the final command output.\n");
            Thread.Sleep(1000);
            console.SlowWrite("First, let's cover piping. You can pipe two or more commands together by separating each command with a '|' character.\n");
            Thread.Sleep(1000);
            console.SlowWrite("The '|' character tells Bash to take the output of the previous command and use it as input for the next command, as if you manually typed it in.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Give it a try! Try piping a command into the 'cowsay' command and see what it does.\n");
            Thread.Sleep(1000);
            console.SlowWrite("(Side-note: You have unlocked the full bash shell. Remember that you can type 'help' for a list of commands! The shell will automatically exit when you complete the objective.)\n");
            Thread.Sleep(1000);
            _shell.AllowExit = false;
            _shell.CommandRun = (command) =>
            {
                return (command.Commands.Length > 1 && command.Commands.Last() == "cowsay");
            };
            _shell.Run(console, new Dictionary<string, object>());

            console.SlowWrite("Good job. I hope that cow looks cute enough. Anyway, now that you know how to pipe, let's learn redirection - so you can save that cow to disk.\n");
            Thread.Sleep(1000);
            console.SlowWrite("First, let's talk about file paths in Peacegate OS.\n");
            Thread.Sleep(1000);
            console.SlowWrite("In Peacegate OS, your hard drive is split into files and directories.\n");
            Thread.Sleep(1000);
            console.SlowWrite("It is a hierarchial filesystem, meaning you can store directories inside directories.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Directories contain files, and files contain data that can be read into programs or written by programs.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Paths are a way to tell Peacegate OS where to find a file.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Paths can either be absolute or relative. Absolute paths always start with the '/' character, because '/' is the root directory of your hard drive and you can't go any higher in the hierarchy than /.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Relative paths do not start with '/', and they are usually relative to a 'working' directory. Bash has its own working directory, which you can change using the 'cd' command.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Each directory also has a '..' and a '.' directory inside it (except for '/', it doesn't have '..'). The '..' directory points to the parent directory in the hierarchy, and the '.' directory just loops back into the directory it's in.\n");
            Thread.Sleep(1000);
            console.SlowWrite("So, in bash, running 'cd ..' means \"Take me up one level in the file hierarchy and set that as my working directory.\"\n");
            Thread.Sleep(1000);
            console.SlowWrite("To see a list of directories and files in the current working directory, you can use 'ls'. Use 'ls -a' to show hidden files and directories (including '.' and '..'). Any file or directory with a name that starts with '.' is hidden.\n");
            Thread.Sleep(1000);
            console.SlowWrite("With console redirection, you can specify an absolute or relative path to where your file should be written. So, if I wanted to write to a file called 'hello.txt' in my working directory, I would simply set my output path to 'hello.txt'. But if I wanted to write to a file somewhere else, I would specify an absolute path like '/home/Documents/readme.txt'.\n");
            Thread.Sleep(1000);
            console.SlowWrite("The output path for console redirection can be set by adding a '>' at the end of the command sequence, followed by a file path.\n");
            Thread.Sleep(1000);
            console.SlowWrite("If you want bash to append the console output to the end of an existing file, simply replace the '>' with a '>>', then specify the path.\n");
            Thread.Sleep(1000);
            console.SlowWrite("Give it a try! Try making the cowsay cow say something and write it to a file!\n");
            Thread.Sleep(1000);

            _shell.CommandRun = (command) =>
            {
                return (command.Commands.Length > 1 && command.Commands.Last() == "cowsay") && command.OutputFile != null;
            };
            _shell.Run(console, new Dictionary<string, object>());

            console.SlowWrite("Great! Now, if you ever want to see the output of those commands, you can just read the file you just saved!\n");
            Thread.Sleep(1000);
            console.SlowWrite("See how useful those features are once you know how to use them?\n");
            Thread.Sleep(1000);
            console.SlowWrite("The next thing you need to learn is how to secure your system for use in The Peacenet...but that's for another time.\n");
            Thread.Sleep(1000);
            console.SlowWrite("For now, this concludes the Peacegate OS Terminal Tutorial. Explore the OS with your new-found skills, and check back when you're ready to learn some more.\n");
            Thread.Sleep(5000);
            _hasLearnedUnixRedirection = true;
        }
    }
}
