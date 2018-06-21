using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using Peacenet.GameState;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Engine.TextRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Applications
{
    [AppLauncher("World Map", "System", "View a map of your current Peacenet country and interact with other members in The Peacenet.")]
    public class WorldMap : Window
    {
        private Panel _selectedSentiencePanel = new Panel();
        private Label _selectedHostname = new Label();
        private Label _selectedIP = new Label();
        private Label _selectedMetadata = new Label();


        private Texture2D _arrowUpLeft = null;
        private Texture2D _arrowUpRight = null;
        private Texture2D _arrowDownLeft = null;
        private Texture2D _arrowDownRight = null;
        private Texture2D _arrowUp = null;
        private Texture2D _arrowDown = null;
        private Texture2D _arrowLeft = null;
        private Texture2D _arrowRight = null;
        private Texture2D _chevron = null;
        private Button _connect = new Button();

        private const int _sentienceRadius = 16;
        private const int _factionRadius = 32;
        private const int _terrainBorderWidth = 2;
        private const float _zoomIn = 0.0025f;

        private Button _exit = new Button();

        private float _zoom = 1;

        private Label _title = new Label();
        private Label _country = new Label();

        private Vector2 _pan = Vector2.Zero;
        private Color _terrainColor = new Color(0x22, 0x22, 0x22);

        [Dependency]
        private GameManager _game = null;

        private bool _panning = false;
        private Vector2 _mouseStart = Vector2.Zero;

        private Label _hostname = new Label();

        [Dependency]
        private GameLoop _gameloop = null;

        public WorldMap(WindowSystem _winsys) : base(_winsys)
        {
            _arrowUpLeft = _gameloop.Content.Load<Texture2D>("UIIcons/arrow-circle-o-upleft");
            _arrowUpRight = _gameloop.Content.Load<Texture2D>("UIIcons/arrow-circle-o-upright");
            _arrowDownLeft = _gameloop.Content.Load<Texture2D>("UIIcons/arrow-circle-o-downleft");
            _arrowDownRight = _gameloop.Content.Load<Texture2D>("UIIcons/arrow-circle-o-downright");
            _arrowLeft = _gameloop.Content.Load<Texture2D>("UIIcons/arrow-circle-o-left");
            _arrowRight = _gameloop.Content.Load<Texture2D>("UIIcons/arrow-circle-o-right");
            _arrowUp = _gameloop.Content.Load<Texture2D>("UIIcons/arrow-circle-o-up");
            _arrowDown = _gameloop.Content.Load<Texture2D>("UIIcons/arrow-circle-o-down");
            _chevron = _gameloop.Content.Load<Texture2D>("ThemeAssets/Arrows/chevron-down");


            SetWindowStyle(WindowStyle.NoBorder);
            _zoom = 1.25f;
            _pan = GetPanFromLocation(_game.State.Player.MapLocation);
            
            AddChild(_hostname);
            _hostname.Opacity = 0;
            _hostname.Visible = false;
            _hostname.AutoSize = true;
            _hostname.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            AddChild(_title);
            AddChild(_country);
            _title.AutoSize = true;
            _country.AutoSize = true;
            _title.Text = "World Map";
            _title.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
            _country.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;

            _exit.Text = "Return to Peacegate";
            _exit.Click += (o, a) =>
            {
                Close();
            };
            AddChild(_exit);

            AddChild(_selectedSentiencePanel);
            _selectedSentiencePanel.DrawBackground = true;
            _selectedSentiencePanel.AddChild(_selectedHostname);
            _selectedSentiencePanel.AddChild(_selectedIP);
            _selectedSentiencePanel.AddChild(_selectedMetadata);
            _selectedSentiencePanel.AddChild(_connect);

            _connect.Text = "Connect";

            _selectedHostname.AutoSize = true;
            _selectedIP.AutoSize = true;
            _selectedMetadata.AutoSize = true;

            _selectedIP.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
            _selectedHostname.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _connect.Click += (o, a) =>
            {
                var term = new CustomCommandTerminal("connect " + _selected.IPAddress.ToIPv4String(), WindowSystem);
                term.Show();
                Close();
            };
        }

        private string _hostnameText = null;

        private Vector2 GetPanFromLocation(Vector2 loc)
        {
            var center = new Vector2(_game.State.CountryTexture.Width / 2, _game.State.CountryTexture.Height / 2);
            return center - loc;
        }

        private string _hover = null;

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            float z = _zoom * 16; //add a lot of spacing between entities

            gfx.Clear(Color.Black * 0.5F);

            if (_game.State.CountryTexture == null)
            {
                string text = "Retrieving terrain data from The Peacenet...";
                var measure = TextRenderer.MeasureText(text, Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Header1), Width / 2, Plex.Engine.TextRenderers.WrapMode.Words);
                gfx.DrawString(text, new Vector2((gfx.Width / 2) / 2, (gfx.Height - measure.Y) / 2), Theme.GetFontColor(Plex.Engine.Themes.TextFontStyle.Header1), Theme.GetFont(Plex.Engine.Themes.TextFontStyle.Header1), TextAlignment.Center, Width / 2, WrapMode.Words);
            }

            var terrain = _game.State.CountryTexture;
            var terrainStartX = ((gfx.Width - (terrain.Width * z)) / 2) + ((_pan.X * z));
            var terrainStartY = ((gfx.Height - (terrain.Height * z)) / 2) + ((_pan.Y * z));
            var terrainWidth = terrain.Width * z;
            var terrainHeight = terrain.Height * z;

            var terrainPos = new Vector2(terrainStartX, terrainStartY);

            gfx.FillRectangle(terrainPos - new Vector2(_terrainBorderWidth * _zoom, _terrainBorderWidth * _zoom), new Vector2(terrainWidth + (_terrainBorderWidth * _zoom), terrainHeight + (_terrainBorderWidth * _zoom)), terrain, Theme.GetAccentColor());
            gfx.FillRectangle(terrainPos, new Vector2(terrainWidth, terrainHeight), terrain, _terrainColor);

            foreach (var sentience in _game.State.SingularSentiences)
            {
                float radius = _sentienceRadius * _zoom;
                var loc = terrainPos + ((sentience.MapLocation * z));
                if (_game.State.Highlighted == null || _game.State.Highlighted.Id != sentience.Id)
                {
                    if (loc.X + radius < 0)
                        continue;
                    if (loc.X - radius > gfx.Width)
                        continue;
                    if (loc.Y + radius < 0)
                        continue;
                    if (loc.Y - radius > gfx.Height)
                        continue;
                }


                Color neutral = Color.White;
                Color target = Color.Green;
                float lerpValue = 0;
                if (sentience.Reputation < 0)
                {
                    target = Color.Red;
                    lerpValue = (-sentience.Reputation + 1) / 2;
                }
                else
                {
                    target = Color.Green;
                    lerpValue = (sentience.Reputation + 1) / 2;
                }

                gfx.FillCircle(loc, radius, Color.Lerp(neutral, target, lerpValue));

                if (_game.State.Highlighted != null && _game.State.Highlighted.Id == sentience.Id)
                {
                    var clamped = Vector2.Clamp(loc, new Vector2(75, 75), new Vector2(gfx.Width - (75 * 2), gfx.Height - (75 * 2)));
                    gfx.FillCircle(loc, radius * 2, Theme.GetAccentColor() * 0.5F);
                    if (clamped == loc)
                    {
                        var chevronLoc = new Vector2(loc.X - (_arrowUp.Width / 2), loc.Y - (radius * 3) - (_arrowUp.Height/2));
                        gfx.FillRectangle(chevronLoc, new Vector2(_arrowUp.Width, _arrowUp.Height), _chevron, Color.White);
                    }
                    else
                    {
                        Texture2D arrow = null;
                        var direction = Vector2.Normalize(loc - clamped);

                        if(direction.X < -0.25F)
                        {
                            if (direction.Y < -0.25F)
                                arrow = _arrowUpLeft;
                            else if (direction.Y > 0.25F)
                                arrow = _arrowDownLeft;
                            else
                                arrow = _arrowLeft;
                        }
                        else if(direction.X > 0.25F)
                        {
                            if (direction.Y < -0.25F)
                                arrow = _arrowUpRight;
                            else if (direction.Y > 0.25F)
                                arrow = _arrowDownRight;
                            else
                                arrow = _arrowRight;
                        }
                        else if(direction.Y < -0.25F)
                        {
                            arrow = _arrowUp;
                        }
                        else
                        {
                            arrow = _arrowDown;
                        }

                        gfx.FillRectangle(new Vector2(clamped.X - (arrow.Width / 2), clamped.Y - (arrow.Height / 2)), new Vector2(arrow.Width, arrow.Height), arrow, Color.White);
                    }
                }


                
            }
        }


        public Sentience Selected => _selected;

        protected override void OnUpdate(GameTime time)
        {
            _exit.Visible = CanClose;

            Width = Manager.ScreenWidth;
            Height = Manager.ScreenHeight;
            Parent.X = 0;
            Parent.Y = 0;

            _title.X = 15;
            _title.Y = 15;
            _country.X = 15;
            _country.Y = _title.Y + _title.Height + 5;
            _country.Text = _game.State.CurrentCountry.ToString();

            _exit.Y = _title.Y + (((_title.Height + _country.Height + 5) - _exit.Height) / 2);
            _exit.X = (Width - _exit.Width) - 15;

            if(_hover != null)
            {
                _hostname.Visible = true;
                _hostname.Opacity = MathHelper.Clamp(_hostname.Opacity + ((float)time.ElapsedGameTime.TotalSeconds * 3), 0, 1);
                _hostname.Text = _hostnameText.Substring(0, (int)(_hostnameText.Length * _hostname.Opacity));
                var sys = _game.State.SingularSentiences.FirstOrDefault(x => x.Id == _hover);

                float z = _zoom * 16; //add a lot of spacing between entities

                var terrain = _game.State.CountryTexture;
                var terrainStartX = ((Width - (terrain.Width * z)) / 2) + ((_pan.X * z));
                var terrainStartY = ((Height - (terrain.Height * z)) / 2) + ((_pan.Y * z));
                var terrainWidth = terrain.Width * z;
                var terrainHeight = terrain.Height * z;

                var terrainPos = new Vector2(terrainStartX, terrainStartY);

                var center = terrainPos + ((sys.MapLocation * z));
                var radius = _sentienceRadius * _zoom;
                var topleft = new Vector2(center.X - radius, center.Y - radius);

                _hostname.X = (int)center.X + (int)radius + 7;
                _hostname.Y = (int)topleft.Y + ((int)(radius * 2) - _hostname.Height) / 2;

            }
            else
            {
                if (_hostname.Visible)
                {
                    _hostname.Opacity = MathHelper.Clamp(_hostname.Opacity - ((float)time.ElapsedGameTime.TotalSeconds * 3), 0, 1);
                    if (_hostname.Opacity <= 0)
                    {
                        _hostname.Visible = false;
                        _hostnameText = null;
                    }
                    else
                    {
                        _hostname.Text = _hostnameText.Substring(0, (int)(_hostnameText.Length * _hostname.Opacity));
                    }
                }
            }

            if (_selected != null)
            {
                _sentiencePanProgress = MathHelper.Clamp(_sentiencePanProgress + ((float)time.ElapsedGameTime.TotalSeconds * 4), 0, 1);
                _pan = Vector2.Lerp(_sentiencePanStart, _sentiencePanEnd, _sentiencePanProgress);

                _selectedHostname.X = 7;
                _selectedHostname.Y = 7;
                _selectedHostname.MaxWidth = _selectedSentiencePanel.Width - 14;
                _selectedIP.MaxWidth = _selectedHostname.MaxWidth;
                _selectedIP.X = _selectedHostname.X;
                _selectedMetadata.X = _selectedHostname.X;
                _selectedMetadata.MaxWidth = _selectedHostname.MaxWidth;
                _selectedHostname.Text = _selected.Hostname ?? _selected.IPAddress.ToIPv4String();
                if(_selected.Hostname == null)
                {
                    _selectedIP.Visible = false;
                    _selectedMetadata.Y = _selectedHostname.Y + _selectedHostname.Height + 4;
                }
                else
                {
                    _selectedIP.Visible = true;
                    _selectedIP.Text = _selected.IPAddress.ToIPv4String();
                    _selectedIP.Y = _selectedHostname.Y + _selectedHostname.Height + 4;
                    _selectedMetadata.Y = _selectedIP.Y + _selectedIP.Height + 4;
                }

                string repName = "Neutral";
                
                if(_selected.Reputation>0)
                {
                    repName = $"{Math.Round(_selected.Reputation * 100)}% Heroic";
                }
                else if(_selected.Reputation<0)
                {
                    repName = $"{Math.Round(-_selected.Reputation * 100)}% Malicious";
                }

                _selectedMetadata.Text = $@"Country: {_selected.Country}
Skill level: {_selected.SkillLevel}
XP: {_selected.XP}
Reputation: {repName}";
            }
            else
            {
                _sentiencePanProgress = MathHelper.Clamp(_sentiencePanProgress - ((float)time.ElapsedGameTime.TotalSeconds * 4), 0, 1);
            }

            _selectedSentiencePanel.Opacity = _sentiencePanProgress;
            _selectedSentiencePanel.Height = (int)MathHelper.Lerp(0, Height / 2, _sentiencePanProgress);
            _selectedSentiencePanel.Width = 350;
            _selectedSentiencePanel.X = (Width - _selectedSentiencePanel.Width) - 30;
            _selectedSentiencePanel.Y = _exit.Y + _exit.Height + 45;

            _connect.X = _selectedMetadata.X;
            _connect.Y = _selectedSentiencePanel.Height - _connect.Height - 7;

            base.OnUpdate(time);
        }

        protected override void OnMouseScroll(MouseEventArgs e)
        {
            _zoom = MathHelper.Clamp(_zoom + ((float)e.ScrollWheelDelta * _zoomIn), 1f / 16, 4f);
            base.OnMouseScroll(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if(_panning == false)
            {
                var mouse = e.Position.ToVector2();

                if (_game.State.CountryTexture == null)
                    return;

                float z = _zoom * 16; //add a lot of spacing between entities

                var terrain = _game.State.CountryTexture;
                var terrainStartX = ((Width - (terrain.Width * z)) / 2) + ((_pan.X * z));
                var terrainStartY = ((Height - (terrain.Height * z)) / 2) + ((_pan.Y * z));
                var terrainWidth = terrain.Width * z;
                var terrainHeight = terrain.Height * z;

                var terrainPos = new Vector2(terrainStartX, terrainStartY);

                _hover = null;

                foreach (var sentience in _game.State.SingularSentiences)
                {
                    float radius = _sentienceRadius * _zoom;
                    var loc = terrainPos + ((sentience.MapLocation * z));
                    if (loc.X + radius < 0)
                        continue;
                    if (loc.X - radius > Width)
                        continue;
                    if (loc.Y + radius < 0)
                        continue;
                    if (loc.Y - radius > Height)
                        continue;
                    var topleft = new Vector2(loc.X - radius, loc.Y - radius);
                    if (mouse.X >= topleft.X && mouse.X <= topleft.X + (radius * 2) && mouse.Y >= topleft.Y && mouse.Y <= topleft.Y + (radius * 2))
                    {
                        _hostnameText = (string.IsNullOrWhiteSpace(sentience.Hostname)) ? sentience.IPAddress.ToIPv4String() : sentience.Hostname;
                        _hover = sentience.Id;
                        break;
                    }
                }
            }


            base.OnMouseMove(e);
        }

        private Sentience _selected = null;
        private Vector2 _sentiencePanStart = Vector2.Zero;
        private Vector2 _sentiencePanEnd = Vector2.Zero;
        private float _sentiencePanProgress = 0f;

        protected override void OnClick(MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                var sentience = _game.State.SingularSentiences.FirstOrDefault(x => x.Id == _hover);
                if (sentience == _selected)
                    return;
                if (sentience != null)
                {
                    _selected = sentience;
                    _sentiencePanStart = _pan;
                    _sentiencePanEnd = GetPanFromLocation(_selected.MapLocation);
                    _sentiencePanProgress = 0;
                }
                else
                {
                    _selected = null;
                }
            }
            base.OnClick(e);
        }

        protected override void OnMouseDragStart(MouseEventArgs e)
        {
            if(e.Button == MouseButton.Left && _selected==null)
            {
                _panning = true;
                _mouseStart = e.Position.ToVector2();
            }
            base.OnMouseDragStart(e);
        }

        protected override void OnMouseDrag(MouseEventArgs e)
        {
            if (_panning)
            {
                var pos = e.Position.ToVector2();
                var diff = pos - _mouseStart;
                _pan += diff / (_zoom * 16);
                _mouseStart = pos;
            }
            base.OnMouseDrag(e);
        }

        protected override void OnMouseDragEnd(MouseEventArgs e)
        {
            _panning = false;
            base.OnMouseDragEnd(e);
        }
    }

    public static class UsefulIntegerStuff
    {
        public static string ToIPv4String(this uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            return $"{bytes[3]}.{bytes[2]}.{bytes[1]}.{bytes[0]}";
        }
    }
}
