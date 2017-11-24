# The Peacenet

Welcome to The Peacenet. Well, not entirely. Instead, welcome to the Peacenet's source code.

This repository contains the code for the Peacenet backend (aka the multiplayer and campaign server software), engine, and frontend (the game itself). It also contains a custom **Pango** wrapper for native text rendering support.

## Compiling the game

### Dependencies

This game depends on several third-party libraries. Most of them are available through **NuGet** - so if you are using Visual Studio or MonoDevelop, these dependencies should automatically be set up.

Some other dependencies - such as those for the native text renderer - may require you to install additional packages on your system.

#### Unmanaged dependencies

For the native text renderer, see the [PlexNative readme](PlexNative/README.md) for compile/usage instructions. For [Discord](https://discordapp.com) RPC support, see the readme for the [Discord Rich Presence library.](https://github.com/discordapp/discord-rpc).

Once you have the libraries compiled and ready, you can copy them to the build output directory of the game and the relevant features should begin to work.

**Linux users rejoice:** You can copy all your libraries to the `/usr/lib` directory and the game will still find them. Windows won't do that.

#### Managed dependencies

You should just be able to run a NuGet restore in the root of the repository and all the packages should all be downloaded and set up for you. If you are on Linux, you may run the `build.sh` file and it'll do everything for you.

#### MonoGame SDK

You can get the MonoGame SDK (as well as the Pipeline Tool GUI - this is very helpful!!) on the MonoGame site. In the case that your IDE doesn't want to play nice with an install from the website, we also have the relevant NuGet packages added for redundancy as a fallback. You'll just need to install the pipeline tool GUI separately.

##### A note on the Pipeline Tool GUI

In the days of ShiftOS, storing resources in the native C# resources system would suffice. But, MonoGame is insanely cumbersome to use when doing it that way. Things like audio, spritefonts, and other special content files won't be able to load easily from a C# resource. The Pipeline Tool allows you to add resources to the MonoGame content pipeline system - which will allow you to load content into the game much more easily. Needless to say, it's a necessity.

**Do NOT manually edit a MonoGame content builder file.**

### Actually building the game

If you're using an IDE (Visual Studio/MonoDevelop), this is easy. Just start either the `Plex` or `Plex.Server` projects with the debugger (aka - hit your F5 key.)

If not, in the root of the repo, run `xbuild` (for Mono users) or `msbuild` (for Windows users) and everything should build.