What the hell is ModLauncher doing...?

Well - it's a mod launcher. Simple as that. When debugging
ShiftOS, if you want to write a mod for the game, set this
project as startup - then add a reference to the mod's project.
Then when you hit F5, ShiftOS will launch with the mod you added
loaded up. This is useful because without it, you would have to
make your mod an EXE file and bootstrap the ShiftOS launch sequence
yourself. This takes care of that for you.