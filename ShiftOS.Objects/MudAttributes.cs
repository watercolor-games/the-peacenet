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

using attribute = System.Attribute;

namespace ShiftOS.Objects
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MudRequestAttribute : attribute
    {
        /// <summary>
        /// This attribute can be used on a static method to make the multi-user domain server software see this method as a MUD request handler.
        /// </summary>
        /// <param name="rName">The header ID of the request this method should handle.</param>
        public MudRequestAttribute(string rName, Type expected)
        {
            RequestName = rName;
            ExpectedType = expected;
        }

        public string RequestName { get; private set; }
        public Type ExpectedType { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MudResponseAttribute : attribute
    {
        /// <summary>
        /// Clients will look for static methods marked with this attribute and run them first. If no attribute is found with the given header ID, the client may invoke a delegate with the message information.
        /// </summary>
        /// <param name="rName">The header ID of the response that this method will handle.</param>
        public MudResponseAttribute(string rName)
        {
            ResponseName = rName;
        }

        public string ResponseName { get; private set; }
    }

    public class ChatLogRequest
    {
        public ChatLogRequest(string chan, int backtrack = 0)
        {
            Channel = chan;
            Backtrack = backtrack;
        }

        public int Backtrack { get; set; }
        public string Channel { get; set; }
    }


}

