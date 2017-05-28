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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.Engine
{
    public class Infobox
    {
        // Set the infobox's interface to null
        private static IInfobox _infobox = null;
        
        /// <summary>
        /// Creates a new infobox
        /// </summary>
        /// <param name="title">Infobox title</param>
        /// <param name="message">Infobox message</param>
        [Obsolete("Please use Infobox.Show instead.")]
        public Infobox(string title, string message)
        {
            Infobox.Show(title, message);
        }
        
        /// <summary>
        /// Shows an infobox
        /// </summary>
        /// <param name="title">Infobox title</param>
        /// <param name="message">Infobox message</param>
        public static void Show(string title, string message, Action callback = null)
        {
            title = Localization.Parse(title);
            message = Localization.Parse(message);
            Desktop.InvokeOnWorkerThread(() =>
            {
                _infobox.Open(title, message, callback);
            });
        }

        public static void PromptText(string title, string message, Action<string> callback, bool isPassword = false)
        {
            title = Localization.Parse(title);
            message = Localization.Parse(message);
            Desktop.InvokeOnWorkerThread(() =>
            {
                _infobox.PromptText(title, message, callback, isPassword);
            });
        }

        public static void PromptYesNo(string title, string message, Action<bool> callback)
        {
            title = Localization.Parse(title);
            message = Localization.Parse(message);
            Desktop.InvokeOnWorkerThread(() =>
            {
                _infobox.PromptYesNo(title, message, callback);
            });
        }

        /// <summary>
        /// Inits an infobox
        /// </summary>
        /// <param name="info">Interface for infobox</param>
        public static void Init(IInfobox info)
        {
            _infobox = info;
        }
    }
    
    // Infobox Interface
    public interface IInfobox
    {
        void Open(string title, string msg, Action callback = null);
        void PromptText(string title, string message, Action<string> callback, bool isPassword);
        void PromptYesNo(string title, string message, Action<bool> callback);
    }
}
