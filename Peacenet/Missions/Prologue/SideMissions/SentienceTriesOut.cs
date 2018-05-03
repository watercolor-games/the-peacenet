using Microsoft.Xna.Framework.Audio;
using Peacenet.Applications;
using Peacenet.Filesystem;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Engine.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Missions.Prologue.SideMissions
{
    public class SentienceTriesOut : Mission
    {
        public override string Name => "Sentience Tries Out";

        public override string Description => "Welcome to Peacegate OS. To help you get acustomed to your new environment, we have prepared 5 simple tasks for you to complete within the operating system. Can you complete them in a timely manner?";

        public override bool Available => true;

        //What time is it?
        private SoundEffectInstance _philTask1 = null;
        private bool _task1Started = false;

        //Write, save, open and delete a text document (Part 1: Write)
        private SoundEffectInstance _philTask2 = null;
        private bool _task2Started = false;

        //Write, save, open and delete a text document (Part 2: Save)
        private SoundEffectInstance _philTask3 = null;
        private bool _task3Started = false;

        //Write, save, open and delete a text document (Part 4: Delete) - Phil keeps messing up the 5th voice clip so we'll fix that later
        private SoundEffectInstance _philTask5 = null;
        private bool _task5Started = false;
        
        //Calculate 8 + 5 ^ 2 / 3
        private SoundEffectInstance _philTask6 = null;
        private bool _task6Started = false;

        //Personalize the Desktop
        private SoundEffectInstance _philTask7 = null;
        private bool _task7Started = false;

        //Install a program.
        private SoundEffectInstance _philTask8 = null;
        private bool _task8Started = false;

        public override string AfterCompleteCutscene => "sto_osft_outro";

        private bool _clockOpened = false; //change to false when clock exists and can be detected as being open.
        private bool _textWritten = true; //change to false when text editor lets you see document text.
        private bool _textSaved = true; //change to false when text can be seen being saved
        private bool _textOpened = true; //change to false when text can be seen being opened
        private bool _textDeleted = true; //change to false when text can be seen being deleted
        private bool _calculationDone = true; //change to false when calculator is finished and answers can be seen
        private bool _desktopCustomized = true; //change to false when desktop customizations are being watched.
        private bool _appInstalled = true; //change to false when PPM is implemented and can be watched

        private string _textPath = null;
        private TextEditorApp _textEditor = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private Plexgate _plexgate = null;

        public override void OnStart()
        {
            _philTask1 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/2").CreateInstance();
            _philTask2 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/3").CreateInstance();
            _philTask3 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/4").CreateInstance();
            _philTask5 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/6").CreateInstance();
            _philTask6 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/7").CreateInstance();
            _philTask7 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/9").CreateInstance(); //Why was 8 not invited? Windows 8, of course.
            _philTask8 = _plexgate.Content.Load<SoundEffect>("Audio/SentienceTriesOut/10").CreateInstance();

            _clockOpened = false;
            _textWritten = false;
            _textOpened = false;
            _textSaved = false;
            _textDeleted = false;
            _calculationDone = false;
            _desktopCustomized = false;
            _appInstalled = false;

            _task1Started = false;
            _task2Started = false;
            _task3Started = false;
            _task5Started = false;
            _task6Started = false;
            _task7Started = false;
            _task8Started = false;

            _terminal.CommandSucceeded += _terminal_CommandSucceeded;
            _winsys.WindowListUpdated += _winsys_WindowListUpdated;
            _fs.WriteOperation += _fs_WriteOperation;

            _os.WallpaperChanged += _os_WallpaperChanged;
            _pn.PanelThemeChanged += _os_WallpaperChanged;
            _theme.ThemeChanged += _os_WallpaperChanged;

        }

        [Dependency]
        private ThemeManager _theme = null;

        [Dependency]
        private PeacenetThemeManager _pn = null;

        private void _os_WallpaperChanged()
        {
            if (_desktopCustomized == false)
                _desktopCustomized = true;
        }

        private Calculator[] _calcs = null;

        private void _fs_WriteOperation(string path)
        {
            if(_textWritten == true)
            {
                if(_textSaved == false)
                {
                    if(_fs.FileExists(path) && path.StartsWith("/home/Documents"))
                    {
                        string fText = _fs.ReadAllText(path);
                        if(fText == _textEditor.Text)
                        {
                            _textSaved = true;
                            _textPath = path;
                            _textEditor.CanClose = true;
                            _textEditor.Close();
                            _textEditor = null;
                        }
                    }
                }
                else
                {
                    if(_textOpened==true && _textDeleted==false)
                    {
                        if(!_fs.FileExists(path) && path == _textPath)
                        {
                            _textDeleted = true;
                        }
                    }
                }
            }
        }

        private void _winsys_WindowListUpdated(object sender, EventArgs e)
        {
            if (_winsys.WindowList.Any(x => x.Border.Window is Clock) && _clockOpened==false)
            {
                _clockOpened = true;
            }
            else
            {
                if(_textEditor == null)
                {
                    var border = _winsys.WindowList.FirstOrDefault(x => x.Border.Window is TextEditorApp);
                    if (border != null)
                        _textEditor = border.Border.Window as TextEditorApp;
                    if (_textEditor != null)
                    {
                        if (_textSaved == false)
                        {
                            _textEditor.CanClose = false;
                        }
                        else if (_textOpened == false)
                        {
                            if (_textEditor.FilePath == _textPath)
                            {
                                _textOpened = true;
                            }
                        }
                    }
                }
            }

            if(_textDeleted==true && _calculationDone==false)
            {
                var calcs = _winsys.WindowList.Where(x => x.Border.Window is Calculator).Select(x => x.Border.Window as Calculator);
                _calcs = calcs.ToArray();
            }
        }

        private const double _calculation = 18.7; //The result of the calculation in the 4th task is a recurring decimal.

        private void _terminal_CommandSucceeded(string name)
        {
            switch(name)
            {
                case "time":
                    _clockOpened = true;
                    break;
            }
        }

        [Dependency]
        private WindowSystem _winsys = null;

        [Dependency]
        private TerminalManager _terminal = null;

        [Dependency]
        private OS _os = null;

        public override string PrerollCutscene => "sto_osft_peacegateIntro";

        public override IEnumerable<Objective> ObjectiveList
        {
            get
            {
                yield return new Objective("What time is it?", "First, a very simple task. See if you can find a clock program.", (time) =>
                {
                    if(_task1Started==false)
                    {
                        _philTask1.Play();
                        _task1Started = true;
                    }
                    return (_philTask1.State == SoundState.Playing || _clockOpened == false) ? ObjectiveState.Active : ObjectiveState.Complete;
                });
                yield return new Objective("Write a Text Document.", "You won't get far in Peacegate if you can't view or edit documents. See if you can find a GUI-based text editor to help you out, then write some text in it.", (time) =>
                {
                    if (_task2Started == false)
                    {
                        _philTask2.Play();
                        _task2Started = true;
                    }
                    if(_textEditor!=null)
                    {
                        if(_textWritten == false)
                            _textWritten = _textEditor.Text?.Length > 5;
                    }
                    return (_philTask2.State != SoundState.Playing && _textWritten) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Save the document.", "Can you save this text document to your Documents folder?", (time) =>
                {
                    if(_task3Started == false)
                    {
                        _philTask3.Play();
                        _task3Started = true;
                    }
                    return (_philTask3.State != SoundState.Playing && _textSaved) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Open the document again.", "We've closed the Text Editor on you. See if you can reopen the file you just saved.", (time) =>
                {
                    return (_textOpened) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Delete the text document.", "Disk space is limited and precious in Peacegate. Which means, you may want to delete that text file.", (time) =>
                {
                    if(_task5Started == false)
                    {
                        _philTask5.Play();
                        _task5Started = true;
                    }
                    return (_philTask5.State != SoundState.Playing && _textDeleted) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Calculate 8 + 2 ^ 5 / 3", "No, that's not a math question. This isn't school. We just want to see if you can calculate things in Peacegate OS.", (time) =>
                {
                    if(_task6Started == false)
                    {
                        _philTask6.Play();
                        _task6Started = true;
                    }
                    if(_calcs != null)
                    {
                        if(_calculationDone==false)
                        {
                            _calculationDone = _calcs.FirstOrDefault(x => Math.Round(x.Value, 1) == _calculation) != null;
                        }
                    }
                    return (_philTask6.State != SoundState.Playing && _calculationDone) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Personalize the Desktop", "Most normal operating systems usually allow some form of customization/appearance settings. See if Peacegate has the same.", (time) =>
                {
                    if(_task7Started==false)
                    {
                        _philTask7.Play();
                        _task7Started = true;
                    }
                    return (_philTask7.State != SoundState.Playing && _desktopCustomized) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
                yield return new Objective("Install a new program", "You can't get far with the base Peacegate programs, although they're user-friendly. See if there's a way to install new programs to your computer.", (time) =>
                {
                    if(_task8Started==false)
                    {
                        _philTask8.Play();
                        _task8Started = true;
                    }
                    return (_philTask8.State != SoundState.Playing && _appInstalled) ? ObjectiveState.Complete : ObjectiveState.Active;
                });
            }
        }
    }
}
