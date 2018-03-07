# The Peacenet

[itch.io](https://watercolorgames.itch.io/the-peacenet)

The Peacenet is a unique open-world game built on The Peace Engine. The Peacenet features both a single-player and online multiplayer mode, both of which have different goals.

In Single-player, you are an undercover operative recruited by the government whose goal is to enter The Peacenet posing as a sentient program to find out what caused a massive war in the world, and put an end to it. In Multiplayer, you are an actual sentient program participating in this war effort, trying to gain as many resources as you can.

Both modes feature a natural AI based off the Kaizen algorithm, which allows you to interact with NPCs through text chat as if they were real people. Thanks to this algorithm, each NPC has their own unique personality and the things you say and do can affect their emotions and feelings toward you as a player, thus affecting how they respond to you.

The world is interacted with through the Peacegate OS, a rebranded fork of the ShiftOS operating system built by The Peace Foundation to be more user-friendly, better looking to the eye, and follow POSIX standards more closely. Peacegate OS uses a derivative of the MATE desktop environment, known simply as the Peacegate Desktop. Peacegate OS also features many Linux utilities such as `bash`, `nmap`, `cowsay`, `man` and `ssh`, all usable in the command-line interface just like in Linux or any Unix-like.

## Contributing to the project

See `CONTRIBUTING.md` in the root of the repository.

## Compiling the game

First, you will need the [MonoGame 3.6 SDK](http://www.monogame.net/2017/03/01/monogame-3-6/). This is a free download, and should work on both Windows and Linux as well as the Macintosh operating system.

Once that is downloaded, in most cases, you can get away with just opening the solution in Visual Studio or your favorite C# IDE and build the *Peacenet* project. Some users of MonoDevelop on Linux may need to use the `xbuild` command-line tool to get the game to compile properly, this is just so that the MonoGame Content Builder will kick in and compile the game's assets. You should only need to do this when the Content Project is modified - once it's compiled, you can compile the game within your IDE.
