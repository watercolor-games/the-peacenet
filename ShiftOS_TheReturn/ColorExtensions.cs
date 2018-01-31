using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine
{
    /// <summary>
    /// Provides simple methods for altering MonoGame color objects.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Decrease the brightness of a color by the specified percentage.
        /// </summary>
        /// <param name="color">The color to alter</param>
        /// <param name="amount">The amount to decrease by</param>
        /// <returns>The new color</returns>
        public static Color Darken(this Color color, float amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            byte newR = (byte)(color.R * amount);
            byte newG = (byte)(color.G * amount);
            byte newB = (byte)(color.B * amount);
            return new Color(color.R - newR, color.G - newG, color.B - newB, color.A);
        }

        /// <summary>
        /// Increase the brightness of a color by the specified percentage.
        /// </summary>
        /// <param name="color">The color to alter</param>
        /// <param name="amount">The amount to increase by</param>
        /// <returns>The new color</returns>
        public static Color Lighten(this Color color, float amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            byte newR = (byte)(color.R * amount);
            byte newG = (byte)(color.G * amount);
            byte newB = (byte)(color.B * amount);
            return new Color(color.R + newR, color.G + newG, color.B + newB, color.A);
        }
    }
}
