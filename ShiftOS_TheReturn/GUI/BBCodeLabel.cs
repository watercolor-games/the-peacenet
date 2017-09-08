using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Frontend.GraphicsSubsystem;
using CodeKicker.BBCode;
using CodeKicker.BBCode.SyntaxTree;
using Plex.Engine;
using Microsoft.Xna.Framework.Graphics;
using Plex.Frontend;

namespace Plex.Frontend.GUI
{
    public class BBCodeLabel : TextControl
    {
        private BBCodeParser parser = null;
        private Dictionary<string, Texture2D> _imageCaches = new Dictionary<string, Texture2D>();

        protected override void OnDisposing()
        {
            parser = null;
            while(_imageCaches.Count > 0)
            {
                var cache = _imageCaches.First();
                cache.Value.Dispose();
                _imageCaches.Remove(cache.Key);
            }
            base.OnDisposing();
        }

        public BBCodeLabel()
        {
            parser = new BBCodeParser(new[]
                {
                    new BBTag("b", "<b>", "</b>"),
                    new BBTag("i", "<span style=\"font-style:italic;\">", "</span>"),
                    new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"),
                    new BBTag("code", "<pre class=\"prettyprint\">", "</pre>"),
                    new BBTag("img", "<img src=\"${content}\" />", "", false, true),
                    new BBTag("quote", "<blockquote>", "</blockquote>"),
                    new BBTag("list", "<ul>", "</ul>"),
                    new BBTag("*", "<li>", "</li>", true, false),
                    new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
                });
        }

        int indent = 0;


        protected override void RenderText(GraphicsContext gfx)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;
            int text_x = 0;
            int text_y = 0;
                WalkTheFuckingTree(gfx, parser.ParseSyntaxTree(Text), ref text_x, ref text_y, true);
        }

        public void WalkTheFuckingTree(GraphicsContext gfx, SyntaxTreeNode tree, ref int text_x, ref int text_y, bool drawText, bool b = false, bool i = false, bool u = false, bool c = false, bool q = false, bool item = false, bool l = false, bool h = false, bool m = false)
        {
            foreach(var node in tree.SubNodes)
            {
                if (node is TagNode && !c)
                {
                    bool temp_b = b;
                    bool temp_i = i;
                    bool temp_u = u;
                    bool temp_c = c;
                    bool temp_q = q;
                    bool temp_l = l;
                    bool temp_h = h;
                    bool temp_item = item;
                    bool temp_m = m;

                    var tag = node as TagNode;
                    var name = tag.Tag.Name;
                    if (c == false)
                    {
                        m = m || name == "img";
                        b = b || name == "b";
                        i = i || name == "i";
                        u = u || name == "u";
                        q = q || name == "quote";
                        c = c || name == "code";
                        l = l || name == "list";
                        item = item || name == "*";
                        h = h || name == "url";
                    }
                    WalkTheFuckingTree(gfx, node, ref text_x, ref text_y, drawText, b, i, u, c, q, l, item, h, m);
                    b = temp_b;
                    i = temp_i;
                    u = temp_u;
                    c = temp_c;
                    q = temp_q;
                    l = temp_l;
                    item = temp_item;
                    m = temp_m;
                    h = temp_h;
                }
                else
                {
                    System.Drawing.FontStyle fs = System.Drawing.FontStyle.Regular;
                    string fname = Font.Name;
                    if (b)
                        fs |= System.Drawing.FontStyle.Bold;
                    if (i)
                        fs |= System.Drawing.FontStyle.Italic;
                    if (u || h)
                        fs |= System.Drawing.FontStyle.Underline;
                    if (c)
                        fname = SkinEngine.LoadedSkin.TerminalFont.Name;
                    if (q)
                        indent += 25;
                    if (item)
                        indent += 25;
                    var font = new System.Drawing.Font(fname, Font.Size, fs);
                    if (m)
                    {
                        var img = DownloadImage(node.ToText());
                        //image will draw at the very left
                        text_x = 0;
                        //and a line below the current line's position
                        text_y += font.Height;
                        int imgwidth = img.Width;
                        int imgheight = img.Height;
                        while(imgwidth > Width)
                        {
                            imgwidth /= 4;
                            imgheight /= 4;
                        }
                        if (drawText)
                        {
                            gfx.DrawRectangle(text_x, text_y, img.Width, img.Height, img);
                        }
                        //and the rest of the line will draw below the image
                        text_y += img.Height;
                        continue;
                    }
                    foreach (var line in node.ToString().Split('\n'))
                    {
                        
                        var words = line.Split(' ');
                        if (c)
                        {
                            var linemeasure = GraphicsContext.MeasureString(line, font, Engine.GUI.TextAlignment.TopLeft, Width);
                            if(!drawText)
                                gfx.DrawRectangle(0, text_y, Width, (int)linemeasure.Y, SkinEngine.LoadedSkin.TerminalBackColorCC.ToColor().ToMonoColor());
                        }
                        foreach (var word in words)
                        {
                            var measure = GraphicsContext.MeasureString(word, font, Engine.GUI.TextAlignment.TopLeft);
                            if(drawText)
                                gfx.DrawString(word + "  ", text_x, text_y, Microsoft.Xna.Framework.Color.White, font, Engine.GUI.TextAlignment.TopLeft);

                            text_x += (int)measure.X;
                            if (text_x > Width)
                            {
                                text_x = 0;
                                text_y += (int)measure.Y;
                            }
                            if (word.EndsWith("\r"))
                            {
                                text_x = 0;
                                text_y += (int)measure.Y;
                            }
                        }
                    }
                }
            }
        }

        public Texture2D DownloadImage(string imgurl)
        {
            if (_imageCaches.ContainsKey(imgurl))
            {
                return _imageCaches[imgurl];
            }
            else
            {
                var wc = new System.Net.WebClient();
                var bytes = wc.DownloadData(imgurl);
                using (var img = (System.Drawing.Bitmap)SkinEngine.ImageFromBinary(bytes))
                {
                    var tex2 = img.ToTexture2D(UIManager.GraphicsDevice);
                    _imageCaches.Add(imgurl, tex2);
                    return tex2;
                }
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            //The first pass draws non-text elements on screen (code blocks etc).
            //This is done in the OnPaint() method.

            //The second pass draws the actual text. This is done in TextControl.RenderText() to take advantage of its caching features.
            //Soon I plan to make it so we don't parse the BBCode syntax tree twice...
            if (string.IsNullOrWhiteSpace(Text))
                return;
            int text_x = 0;
            int text_y = 0;
            WalkTheFuckingTree(gfx, parser.ParseSyntaxTree(Text), ref text_x, ref text_y, false);

            base.OnPaint(gfx, target);
        }

    }
    public static class Shithead
    {
        public static string Repeat(this string value, int count)
        {
            string r = "";
            for (int i = 0; i < count; i++)
            {
                r += value;
            }
            return r;
        }
    }
}