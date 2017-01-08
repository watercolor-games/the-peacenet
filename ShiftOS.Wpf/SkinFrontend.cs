using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;
namespace ShiftOS.Wpf
{
    public class SkinFrontend : FrameworkElement
    {
        public Brush GetDesktopBrush()
        {
            try
            {
                return new ImageBrush(LoadedSkin.DesktopBackgroundImage.ToBitmapImage());
            }
            catch
            {
                if (LoadedSkin != null)
                {
                    return LoadedSkin.DesktopColor.CreateBrush();
                }
                else
                {
                    return new Skin().DesktopColor.CreateBrush();
                }
            }
        }
    }
}
