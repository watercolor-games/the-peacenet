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
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Wpf
{
    /// <summary>
    /// Interaction logic for WpfWindowBorder.xaml
    /// </summary>
    public partial class WpfWindowBorder : UserControl, IWindowBorder
    {
        private bool _isWPF = true;

        public WpfWindowBorder(IShiftOSWindow ctrl)
        {
            InitializeComponent();
            if (ctrl is UserControl)
            {
                _parent = ctrl as UserControl;
                _isWPF = true;
            }
            else if (ctrl is System.Windows.Forms.UserControl)
            {
                _isWPF = false;
                _wParent = ctrl as System.Windows.Forms.UserControl;
                _wParent.Show();
            }
            Shiftorium.Installed += () =>
            {
                SetupUpgradeables();
                ParentWindow.OnUpgrade();
            };
            SkinEngine.SkinLoaded += () =>
            {
                SetupSkin();
                ParentWindow.OnSkinLoad();
            };
            SaveSystem.GameReady += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    SetupUpgradeables();
                    SetupSkin();
                });
            };
            if (_isWPF)
            {
                this.Width = borderleft.Width + _parent.Width + borderright.Width;
                this.Height = titlemaster.Height + _parent.Height + borderbottom.Height;
                pgcontents.Content = _parent;
            }
            else
            {
                pgcontents.Width = _wParent.Width;
                pgcontents.Height = _wParent.Height;

                this.Width = borderleft.Width + _wParent.Width + borderright.Width;
                this.Height = titlemaster.Height + _wParent.Height + borderbottom.Height;
                pgcontents.Content = new WindowsFormsHost();
                (pgcontents.Content as WindowsFormsHost).Child = _wParent;
                _wParent.DoWinformsSkinningMagicOnWpf();
            }
            Desktop.ShowWindow(this);
            SetupUpgradeables();
            Loaded += (o,a) =>
            {
                SetupSkin();
                ParentWindow.OnSkinLoad();
                ParentWindow.OnLoad();
            };

            if (!_isWPF)
            {
                _wParent.TextChanged += (o, a) =>
                {
                    this.Text = _wParent.Text;
                };
                this.Text = _wParent.Text;
            }
        }

        private System.Windows.Forms.UserControl _wParent = null;

        public void SetupUpgradeables()
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

        public void SetupSkin()
        {
            if (LoadedSkin.ShowTitleCorners)
            {
                titleleft.Visibility = Visibility.Visible;
                titleright.Visibility = Visibility.Visible;
            }
            else
            {
                titleleft.Visibility = Visibility.Hidden;
                titleright.Visibility = Visibility.Hidden;
            }

            try
            {
                _parent.Background = LoadedSkin.ControlColor.CreateBrush();
                _parent.Foreground = LoadedSkin.ControlTextColor.CreateBrush();
            }
            catch
            {
                _wParent.BackColor = LoadedSkin.ControlColor;
                _wParent.ForeColor = LoadedSkin.ControlTextColor;
            }


            titlemaster.Height = LoadedSkin.TitlebarHeight;
            if(LoadedSkin.TitleTextCentered == true)
            {
                titletext.SetValue(Canvas.LeftProperty, (double)(titlemaster.ActualWidth - titletext.ActualWidth) / 2);
                titletext.SetValue(Canvas.TopProperty, (double)LoadedSkin.TitleTextLeft.Y);
            }
            else
            {
                titletext.SetValue(Canvas.LeftProperty, (double)LoadedSkin.TitleTextLeft.X);
                titletext.SetValue(Canvas.TopProperty, (double)LoadedSkin.TitleTextLeft.Y);
            }

            titletext.SetFont(LoadedSkin.TitleFont);

            titletext.Foreground = LoadedSkin.TitleTextColor.CreateBrush();

            titlebar.Background = LoadedSkin.TitleBackgroundColor.CreateBrush();

            close.SetValue(Canvas.LeftProperty, this.ActualWidth - LoadedSkin.CloseButtonSize.Width - LoadedSkin.CloseButtonFromSide.X);
            close.SetValue(Canvas.TopProperty, (double)LoadedSkin.CloseButtonFromSide.Y);

            min.SetValue(Canvas.LeftProperty, this.ActualWidth - LoadedSkin.MinimizeButtonSize.Width - LoadedSkin.MinimizeButtonFromSide.X);
            min.SetValue(Canvas.TopProperty, (double)LoadedSkin.MinimizeButtonFromSide.Y);

            max.SetValue(Canvas.LeftProperty, this.ActualWidth - LoadedSkin.MaximizeButtonSize.Width - LoadedSkin.MaximizeButtonFromSide.X);
            max.SetValue(Canvas.TopProperty, (double)LoadedSkin.MaximizeButtonFromSide.Y);

            close.Background = LoadedSkin.CloseButtonColor.CreateBrush();
            min.Background = LoadedSkin.MinimizeButtonColor.CreateBrush();
            max.Background = LoadedSkin.MaximizeButtonColor.CreateBrush();

            close.BorderThickness = new Thickness(0.0);
            min.BorderThickness = new Thickness(0.0);
            max.BorderThickness = new Thickness(0.0);

            borderleft.Background = LoadedSkin.BorderLeftBackground.CreateBrush();
            borderright.Background = LoadedSkin.BorderRightBackground.CreateBrush();
            borderbottom.Background = LoadedSkin.BorderBottomBackground.CreateBrush();
            borderbottoml.Background = LoadedSkin.BorderBottomLeftBackground.CreateBrush();
            borderbottomr.Background = LoadedSkin.BorderBottomRightBackground.CreateBrush();

            borderleft.Width = LoadedSkin.LeftBorderWidth;
            borderright.Width = LoadedSkin.RightBorderWidth;
            bottommaster.Height = LoadedSkin.BottomBorderWidth;
            
        }


        protected override Size MeasureOverride(Size constraint)
        {
            int topHeight = 0;
            int bottomHeight = 0;
            int leftHeight = 0;
            int rightHeight = 0;

            if (Shiftorium.UpgradeInstalled("wm_titlebar"))
                topHeight = LoadedSkin.TitlebarHeight;
            if (Shiftorium.UpgradeInstalled("wm_free_placement"))
            {
                bottomHeight = LoadedSkin.BottomBorderWidth;
                leftHeight = LoadedSkin.LeftBorderWidth;
                rightHeight = LoadedSkin.RightBorderWidth;
            }

            if (_isWPF)
            {
                return new Size(
                leftHeight + _parent.Width + rightHeight,
                topHeight + _parent.Height + bottomHeight);

            }
            else
            {
                return new Size(
                leftHeight + _wParent.Width + rightHeight,
                topHeight + _wParent.Height + bottomHeight);
            }
        }

        public double ActualActualHeight
        {
            get
            {
                int topHeight = 0;
                int bottomHeight = 0;
                
                if (Shiftorium.UpgradeInstalled("wm_titlebar"))
                    topHeight = LoadedSkin.TitlebarHeight;
                if (Shiftorium.UpgradeInstalled("wm_free_placement"))
                {
                    bottomHeight = LoadedSkin.BottomBorderWidth;
                }

                if (_isWPF)
                {
                    return (topHeight + _parent.Height + bottomHeight);

                }
                else
                {
                    return (topHeight + _wParent.Height + bottomHeight);
                }
            }
        }

        private bool _isDialog = false;

        public bool IsDialog
        {
            get
            {
                return _isDialog;
            }
            set
            {
                _isDialog = value;
                SetupDialog();
            }
        }

        public void SetupDialog()
        {
            if (IsDialog)
            {
                min.Upgrade("minimize_button");
                max.Upgrade("maximize_button");
            }
            else
            {
                min.Visibility = Visibility.Collapsed;
                max.Visibility = Visibility.Collapsed;
            }
        }

        private UserControl _parent = null;

        public IShiftOSWindow ParentWindow
        {
            get
            {
                if (_isWPF)
                {
                    return (IShiftOSWindow)_parent;
                }
                else
                {
                    return (IShiftOSWindow)_wParent;
                }
            }

            set
            {
                if (_isWPF)
                {
                    _parent = (UserControl)value;
                }
                else
                {
                    _wParent = (System.Windows.Forms.UserControl)value;
                    (this.pgcontents.Content as WindowsFormsHost).Child = _wParent;
                }
            }
        }

        public string Text
        {
            get
            {
                return titletext.Text;
            }

            set
            {
                titletext.Text = value;
            }
        }

        public void Close()
        {
            Desktop.RemoveWindow(this);
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void max_Click(object sender, RoutedEventArgs e)
        {
            AppearanceManager.Maximize(this);
        }

        private void min_Click(object sender, RoutedEventArgs e)
        {
            AppearanceManager.Minimize(this);
        }

        private Point currentPoint;
        private TranslateTransform transform = new TranslateTransform();
        private Point anchorPoint;

        private void titlebar_MouseDown(object sender, MouseEventArgs e)
        {
            if (Shiftorium.UpgradeInstalled("draggable_windows") && isInDrag == true)
            {
                var element = sender as FrameworkElement;
                currentPoint = e.GetPosition(null);

                transform.X += (currentPoint.X - anchorPoint.X);
                transform.Y += (currentPoint.Y - anchorPoint.Y);
                this.RenderTransform = transform;
                anchorPoint = currentPoint;
                if(_isWPF == false)
                {
                    _wParent.Refresh();
                    (pgcontents.Content as WindowsFormsHost).InvalidateVisual();
                }
            }
        }

        bool isInDrag = false;
        
        private void titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            anchorPoint = e.GetPosition(null);
            isInDrag = true;
            e.Handled = true;
        }

        private void titlebar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isInDrag = false;
        }
    }
}
