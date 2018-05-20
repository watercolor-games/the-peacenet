using Microsoft.Xna.Framework;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.DesktopUI
{
    public class XPDisplay : Control
    {
        private ProgressBar _levelBar = new ProgressBar();
        private Label _head = new Label();

        private float _percentage = 1f;

        public int TotalXP { get; set; }
        public int SkillLevel { get; set; }
        public float SkillLevelPercentage { get; set; }
        public bool HideOnFocusLoss { get; set; }

        public void ApplyAnim()
        {
            _percentage = 0;
        }

        public XPDisplay()
        {
            AddChild(_head);
            AddChild(_levelBar);

            HideOnFocusLoss = false;

            _head.AutoSize = true;
            _head.FontStyle = Plex.Engine.Themes.TextFontStyle.Highlight;
        }

        protected override void OnUpdate(GameTime time)
        {
            if (Visible && !HasFocused && HideOnFocusLoss)
            {
                Visible = false;
                return;
            }

            if (Width < 275)
                Width = 275;

            _head.MaxWidth = Width - 14;
            _head.X = 7;
            _head.Y = 7;
            _head.Text = $"Skill level: {SkillLevel}";

            _levelBar.Value = SkillLevelPercentage * _percentage;
            _levelBar.Text = $"Total XP: {Math.Round(TotalXP * _percentage)}";

            _percentage = MathHelper.Clamp(_percentage + ((float)time.ElapsedGameTime.TotalSeconds*2), 0, 1);

            _levelBar.X = 7;
            _levelBar.Width = Width - 14;
            _levelBar.Y = _head.Y + _head.Height + 6;

            Height = _levelBar.Y + _levelBar.Height + 7;

            base.OnUpdate(time);
        }
    }
}
