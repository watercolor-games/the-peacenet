using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.TextRenderers;

namespace Plex.Engine.GraphicsSubsystem
{
    /// <summary>
    /// Encapsulates a <see cref="GraphicsDevice"/> and <see cref="SpriteBatch"/> and contains methods for easily rendering various objects using those encapsulated objects. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    ///     <para>The <see cref="GraphicsContext"/> class employs scissor testing in all of its draw calls. This makes it so that any data rendering outside the scissor rectangle (defined by the <see cref="X"/>, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height"/> properties) will be clipped and not rendered to the screen.</para>
    ///     <para>Also, apart from the <see cref="X"/> and <see cref="Y"/> properties of the graphics context, any X/Y coordinate pairs are relative to the coordinates of the scissor rectangle. So, the coordinates (0,5) refer to <see cref="X"/>+0,<see cref="Y"/>+5.</para>
    /// </remarks>
    /// <seealso cref="RasterizerState.ScissorTestEnable"/>
    /// <seealso cref="GraphicsDevice.ScissorRectangle"/>
    /// <seealso cref="GraphicsDevice"/>
    /// <seealso cref="SpriteBatch"/>
    /// <threadsafety static="true" instance="false"/>
    public sealed class GraphicsContext
    {
        private static Texture2D white = null;

        /// <summary>
        /// Retrieves the sprite batch associated with this graphics context.
        /// </summary>
        public SpriteBatch Batch
        {
            get
            {
                return _spritebatch;
            }
        }

        /// <summary>
        /// Retrieves the graphics device associated with this graphics context.
        /// </summary>
        public GraphicsDevice Device
        {
            get
            {
                return _graphicsDevice;
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the scissor rectangle.
        /// </summary>
        public int X
        {
            get
            {
                return Device.ScissorRectangle.X;
            }
            set
            {
                Device.ScissorRectangle = new Rectangle(value, Y, Width, Height);
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the scissor rectangle.
        /// </summary>
        public int Y
        {
            get
            {
                return Device.ScissorRectangle.Y;
            }
            set
            {
                Device.ScissorRectangle = new Rectangle(X, value, Width, Height);
            }
        }

        /// <summary>
        /// Gets or sets the width of the scissor rectangle.
        /// </summary>
        public int Width
        {
            get
            {
                return Device.ScissorRectangle.Width;
            }
            set
            {
                Device.ScissorRectangle = new Rectangle(X, Y, value, Height);
            }
        }

        /// <summary>
        /// Gets or sets the height of the scissor rectangle.
        /// </summary>
        public int Height
        {
            get
            {
                return Device.ScissorRectangle.Height;
            }
            set
            {
                Device.ScissorRectangle = new Rectangle(X, Y, Width, value);
            }
        }

        /// <summary>
        /// Draw an outlined polygon.
        /// </summary>
        /// <param name="c">The color of the polygon's outlines</param>
        /// <param name="locs">The various X and Y coordinates relative to the scissor rectangle of the polygon. The size of this array must be a multiple of 2.</param>
        /// <exception cref="Exception">The <paramref name="locs"/> array does not have a length which is a multiple of 2.</exception> 
        public void DrawPolygon(Color c, params int[] locs)
        {
            if ((locs.Length % 2) != 0)
                throw new Exception("The locs argument count must be a multiple of 2.");
            for(int i = 0; i < locs.Length; i+= 2)
            {
                int x = locs[i];
                int y = locs[i + 1];
                int x1 = locs[0];
                int y1 = locs[1];

                if (i < locs.Length - 2)
                {
                    x1 = locs[i + 2];
                    y1 = locs[i + 3];
                }
                DrawLine(x, y, x1, y1, 1, c);
            }
        }

        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spritebatch;

        /// <summary>
        /// Creates a new instance of the <see cref="GraphicsContext"/> class. 
        /// </summary>
        /// <param name="device">The graphics device where rendering will take place.</param>
        /// <param name="batch">The sprite batch to associate with the graphics context.</param>
        /// <param name="x">The starting X coordinate of the scissor rectangle.</param>
        /// <param name="y">The starting Y coordinate of the scissor rectangle.</param>
        /// <param name="width">The starting width of the scissor rectangle.</param>
        /// <param name="height">The starting height of the scissor rectangle.</param>
        public GraphicsContext(GraphicsDevice device, SpriteBatch batch, int x, int y, int width, int height)
        {
            if (device == null || batch == null)
                throw new ArgumentNullException();
            
            _graphicsDevice = device;
            _spritebatch = batch;
            if(white == null)
            {
                white = new Texture2D(_graphicsDevice, 1, 1);
                white.SetData<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            }
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Clears the canvas with the specified color.
        /// </summary>
        /// <param name="c">The color to render</param>
        public void Clear(Color c)
        {
            DrawRectangle(0, 0, Width, Height, c);
        }

        /// <summary>
        /// Draw a line between two separate points on the canvas
        /// </summary>
        /// <param name="x">The X coordinate of the first point</param>
        /// <param name="y">The Y coordinate of the first point</param>
        /// <param name="x1">The X coordinate of the second point</param>
        /// <param name="y1">The Y coordinate of the second point</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="tex2">The line's texture</param>
        public void DrawLine(int x, int y, int x1, int y1, int thickness, Texture2D tex2)
        {
            DrawLine(x, y, x1, y1, thickness, tex2, Color.White);
        }

        /// <summary>
        /// Draw a line with a tint between two separate points on the canvas
        /// </summary>
        /// <param name="x">The X coordinate of the first point</param>
        /// <param name="y">The Y coordinate of the first point</param>
        /// <param name="x1">The X coordinate of the second point</param>
        /// <param name="y1">The Y coordinate of the second point</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="tex2">The line's texture</param>
        /// <param name="tint">The tint of the texture</param>
        public void DrawLine(int x, int y, int x1, int y1, int thickness, Texture2D tex2, Color tint)
        {
            if (tint.A == 0)
                return; //no sense rendering if you CAN'T SEE IT
            x += X;
            y += Y;
            x1 += X;
            y1 += Y;
            int distance = (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
            float rotation = getRotation(x, y, x1, y1);
            _spritebatch.Draw(tex2, new Rectangle(x, y, distance, thickness), null, tint, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a line with a tint between two separate points on the canvas
        /// </summary>
        /// <param name="x">The X coordinate of the first point</param>
        /// <param name="y">The Y coordinate of the first point</param>
        /// <param name="x1">The X coordinate of the second point</param>
        /// <param name="y1">The Y coordinate of the second point</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="color">The color of the line</param>
        public void DrawLine(int x, int y, int x1, int y1, int thickness, Color color)
        {
            if (color.A == 0)
                return; //no sense rendering if you CAN'T SEE IT
            x += X;
            y += Y;
            x1 += X;
            y1 += Y;
            int distance = (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
            float rotation = getRotation(x, y, x1, y1);
            _spritebatch.Draw(white, new Rectangle(x, y, distance, thickness), null, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a rectangle with the specified color to the canvas.
        /// </summary>
        /// <param name="x">The X coordinate of the rectangle</param>
        /// <param name="y">The Y coordinate of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        /// <param name="color">The color of the rectangle</param>
        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            if (color.A == 0)
                return; //no sense rendering if you CAN'T SEE IT
            x += X;
            y += Y;
            _spritebatch.Draw(white, new Rectangle(x, y, width, height), color);
        }

        /// <summary>
        /// Begin a draw call.
        /// </summary>
        public void BeginDraw()
        {
            _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.LinearClamp, Device.DepthStencilState,
                        RasterizerState);

        }

        /// <summary>
        /// End the current draw call.
        /// </summary>
        public void EndDraw()
        {
            _spritebatch.End();
        }

        /// <summary>
        /// Draw a circle to the canvas.
        /// </summary>
        /// <param name="x">The X coordinate of the circle</param>
        /// <param name="y">The Y coordinate of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="color">The color of the circle</param>
        public void DrawCircle(int x, int y, int radius, Color color)
        {
            if (color.A == 0)
                return; //no sense rendering if you CAN'T SEE IT
            float step = (float) Math.PI / (radius * 4);
            var rect = new Rectangle(x, y, radius, 1);
            for (float theta = 0; theta < 2 * Math.PI; theta += step)
                _spritebatch.Draw(white, rect, null, color, theta, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a rectangle with the specified texture and tint to the canvas.
        /// </summary>
        /// <param name="x">The X coordinate of the rectangle</param>
        /// <param name="y">The Y coordinate of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        /// <param name="tex2">The texture of the rectangle</param>
        /// <param name="layout">The tint of the rectangle</param>
        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2, ImageLayout layout = ImageLayout.Stretch)
        {
            DrawRectangle(x, y, width, height, tex2, Color.White, layout);
        }

        /// <summary>
        /// Retrieves a new <see cref="RasterizerState"/> preferred to be used by the graphics context. 
        /// </summary>
        public readonly RasterizerState RasterizerState = new RasterizerState { ScissorTestEnable = true, MultiSampleAntiAlias = true };

        /// <summary>
        /// Draw a rectangle with the specified texture, tint and <see cref="System.Windows.Forms.ImageLayout"/> to the canvas.
        /// </summary>
        /// <param name="x">The X coordinate of the rectangle</param>
        /// <param name="y">The Y coordinate of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        /// <param name="tex2">The texture of the rectangle</param>
        /// <param name="tint">The tint of the texture</param>
        /// <param name="layout">The layout of the texture</param>
        /// <param name="opaque">Whether the rectangle should be opaque regardless of the texture data or tint's alpha value.</param>
        /// <param name="premultiplied">Whether the texture data is already pre-multiplied.</param>
        public void DrawRectangle(int x, int y, int width, int height, Texture2D tex2, Color tint, ImageLayout layout = ImageLayout.Stretch, bool opaque = false, bool premultiplied=true)
        {
            if (tint.A == 0)
                return; //no sense rendering if you CAN'T SEE IT
            if (tex2 == null)
                return;
            x += X;
            y += Y;
            _spritebatch.End();
            var state = SamplerState.LinearClamp;
            if (layout == ImageLayout.Tile)
                state = SamplerState.LinearWrap;
            if (opaque)
            {
                _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
                                        state, Device.DepthStencilState,
                                        RasterizerState);
            }
            else
            {
                if (premultiplied)
                {
                    _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                                            state, Device.DepthStencilState,
                                            RasterizerState);
                }
                else
                {
                    _spritebatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                            state, Device.DepthStencilState,
                                            RasterizerState);

                }
            }
            switch (layout)
            {
                case ImageLayout.Tile:
                    _spritebatch.Draw(tex2, new Vector2(x,y), new Rectangle(0, 0, width, height), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    break;
                case ImageLayout.Stretch:
                    _spritebatch.Draw(tex2, new Rectangle(x, y, width, height), tint);
                    break;
                case ImageLayout.None:
                    _spritebatch.Draw(tex2, new Rectangle(x, y, tex2.Width, tex2.Height), tint);
                    break;
                case ImageLayout.Center:
                    _spritebatch.Draw(tex2, new Rectangle(x+((width - tex2.Width) / 2), y+((height - tex2.Height) / 2), tex2.Width, tex2.Height), tint);
                    break;
                case ImageLayout.Zoom:
                    float scale = Math.Min(width / (float)tex2.Width, height / (float)tex2.Height);
                    
                    var scaleWidth = (int)(tex2.Width * scale);
                    var scaleHeight = (int)(tex2.Height * scale);

                    _spritebatch.Draw(tex2, new Rectangle(x+(((int)width - scaleWidth) / 2), y+(((int)height - scaleHeight) / 2), scaleWidth, scaleHeight), tint);
                    break;
                    ;
            }
            _spritebatch.End();
            BeginDraw();
        }

        /// <summary>
        /// Measure a string. Note that this method is a stub and just calls <see cref="TextRenderer.MeasureText(string, System.Drawing.Font, int, TextAlignment, WrapMode)"/>. This stub will be removed soon. 
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <param name="font">The font to measure with</param>
        /// <param name="alignment">The alignment of the text</param>
        /// <param name="wrapWidth">The maximum width text can be before it is wrapped</param>
        /// <param name="wrapMode">The wrap mode of the text</param>
        /// <returns>The size of the text in pixels</returns>
        public static Vector2 MeasureString(string text, System.Drawing.Font font, TextAlignment alignment, int wrapWidth = int.MaxValue, WrapMode wrapMode = WrapMode.Words)
        {
            return Plex.Engine.TextRenderer.MeasureText(text, font, wrapWidth, alignment, wrapMode);

        }

        /// <summary>
        /// Draw a string of text.
        /// </summary>
        /// <param name="text">The text to render</param>
        /// <param name="x">The X coordinate of the text</param>
        /// <param name="y">The Y coordinate of the text</param>
        /// <param name="color">The color of the text</param>
        /// <param name="font">The font of the text</param>
        /// <param name="alignment">The alignment of the text</param>
        /// <param name="wrapWidth">The maximum width text can be before it is wrapped.</param>
        /// <param name="wrapMode">The wrap mode of the text</param>
        public void DrawString(string text, int x, int y, Color color, System.Drawing.Font font, TextAlignment alignment, int wrapWidth = int.MaxValue, WrapMode wrapMode = WrapMode.Words)
        {
            if (color.A == 0)
                return; //no sense rendering if you CAN'T SEE IT
            if (string.IsNullOrEmpty(text))
                return;
            Plex.Engine.TextRenderer.DrawText(this, x, y, text, font, color, wrapWidth, alignment, wrapMode);
        }

        private float getRotation(float x, float y, float x2, float y2)
        {
            float adj = x - x2;
            float opp = y - y2;
            return (float) Math.Atan2(opp, adj) - (float) Math.PI;
        }
    }
}
