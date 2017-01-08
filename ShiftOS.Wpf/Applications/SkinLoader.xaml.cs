/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using ShiftOS.Objects.ShiftFS;
using ShiftOS.Engine;

namespace ShiftOS.Wpf.Applications
{
    /// <summary>
    /// Interaction logic for SkinLoader.xaml
    /// </summary>
    [Launcher("Skin Loader", true, "al_skin_loader")]
    [RequiresUpgrade("skinning")]
    public partial class SkinLoader : UserControl, IShiftOSWindow
    {
        public SkinLoader()
        {
            InitializeComponent();
        }

        private Skin _mySkin = new Skin();

        private void close_Click(object sender, RoutedEventArgs e)
        {
            AppearanceManager.Close(this);
        }

        private void loaddefault_Click(object sender, RoutedEventArgs e)
        {
            _mySkin = new Skin();
            Setup();
        }

        private void import_Click(object sender, RoutedEventArgs e)
        {

        }

        private void export_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Setup()
        {
            SetupWindowUpgradeables();
            SetupWindowSkin();
        }

        public void SetupWindowSkin()
        {
            if (_mySkin.ShowTitleCorners)
            {
                titleleft.Visibility = Visibility.Visible;
                titleright.Visibility = Visibility.Visible;
            }
            else
            {
                titleleft.Visibility = Visibility.Hidden;
                titleright.Visibility = Visibility.Hidden;
            }

                pgcontents.Background = _mySkin.ControlColor.CreateBrush();
                pgcontents.Foreground = _mySkin.ControlTextColor.CreateBrush();


            titlemaster.Height = _mySkin.TitlebarHeight;
            if (_mySkin.TitleTextCentered == true)
            {
                titletext.SetValue(Canvas.LeftProperty, (double)(titlemaster.ActualWidth - titletext.ActualWidth) / 2);
                titletext.SetValue(Canvas.TopProperty, (double)_mySkin.TitleTextLeft.Y);
            }
            else
            {
                titletext.SetValue(Canvas.LeftProperty, (double)_mySkin.TitleTextLeft.X);
                titletext.SetValue(Canvas.TopProperty, (double)_mySkin.TitleTextLeft.Y);
            }

            titletext.SetFont(_mySkin.TitleFont);

            titletext.Foreground = _mySkin.TitleTextColor.CreateBrush();

            titlebar.Background = _mySkin.TitleBackgroundColor.CreateBrush();

            close.SetValue(Canvas.LeftProperty, this.ActualWidth - _mySkin.CloseButtonSize.Width - _mySkin.CloseButtonFromSide.X);
            close.SetValue(Canvas.TopProperty, (double)_mySkin.CloseButtonFromSide.Y);

            min.SetValue(Canvas.LeftProperty, this.ActualWidth - _mySkin.MinimizeButtonSize.Width - _mySkin.MinimizeButtonFromSide.X);
            min.SetValue(Canvas.TopProperty, (double)_mySkin.MinimizeButtonFromSide.Y);

            max.SetValue(Canvas.LeftProperty, this.ActualWidth - _mySkin.MaximizeButtonSize.Width - _mySkin.MaximizeButtonFromSide.X);
            max.SetValue(Canvas.TopProperty, (double)_mySkin.MaximizeButtonFromSide.Y);

            close.Background = _mySkin.CloseButtonColor.CreateBrush();
            min.Background = _mySkin.MinimizeButtonColor.CreateBrush();
            max.Background = _mySkin.MaximizeButtonColor.CreateBrush();

            close.BorderThickness = new Thickness(0.0);
            min.BorderThickness = new Thickness(0.0);
            max.BorderThickness = new Thickness(0.0);

            borderleft.Background = _mySkin.BorderLeftBackground.CreateBrush();
            borderright.Background = _mySkin.BorderRightBackground.CreateBrush();
            borderbottom.Background = _mySkin.BorderBottomBackground.CreateBrush();
            borderbottoml.Background = _mySkin.BorderBottomLeftBackground.CreateBrush();
            borderbottomr.Background = _mySkin.BorderBottomRightBackground.CreateBrush();

            borderleft.Width = _mySkin.LeftBorderWidth;
            borderright.Width = _mySkin.RightBorderWidth;
            bottommaster.Height = _mySkin.BottomBorderWidth;

        }


        public void SetupWindowUpgradeables()
        {
            close.Upgrade("close_button");
            max.Upgrade("maximize_button");
            min.Upgrade("minimize_button");

            titlemaster.Upgrade("wm_titlebar");

            borderleft.Upgrade("wm_free_placement");
            borderright.Upgrade("wm_free_placement");
            borderbottom.Upgrade("wm_free_placement");
            borderbottoml.Upgrade("wm_free_placement");
            borderbottomr.Upgrade("wm_free_placement");


        }


        private void apply_Click(object sender, RoutedEventArgs e)
        {
            Utils.WriteAllText(Paths.GetPath("skin.json"), JsonConvert.SerializeObject(_mySkin));
            SkinEngine.LoadSkin();
        }

        public void OnLoad()
        {
            this.SetTitle("Skin Loader");
            _mySkin = JsonConvert.DeserializeObject<Skin>(JsonConvert.SerializeObject(SkinEngine.LoadedSkin));
        }

        public void OnSkinLoad()
        {
            Setup();
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void apps_Click(object s, RoutedEventArgs a)
        {

        }
    }
}
