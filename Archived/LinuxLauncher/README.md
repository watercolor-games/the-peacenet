This is an attempt to get ShiftOS working well on Linux using Wine and
Mono.

The launcher script needs a copy of the game (try extracting Debug.zip
from the AppVeyor project) in a directory called "data". It will set up
a new Wine prefix in ~/.local/share/ShiftOS and the game itself can be
found in ~/.local/share/ShiftOS/drive_c/ShiftOS. Ultimately I want to
make an AUR package of this script if all goes to plan.

## known bugs

* the first time you start the game, the Wine virtual desktop doesn't
enable properly
* text input boxes are white on white
* the terminal puts an extra newline after displaying the prompt
* Aiden Nirh's cutscene has weird overlapping text
* ShiftLetters seems to have no available word lists. Clicking Play
crashes the game.
* the ShiftOS desktop can go in front of applications
* there is a blue border from the Wine desktop (I want to change that
to black, but I also don't want it showing through while the game is
running)
* CSharpCodeProvider doesn't seem to work, so Python mods won't load

## anticipated tricky bits

* Web browser
* Unity mode
