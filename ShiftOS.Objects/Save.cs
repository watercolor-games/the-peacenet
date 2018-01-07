/*
 * Project: Plex
 * 
 * Copyright (c) 2017 Watercolor Games. All rights reserved. For internal use only.
 * 






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
using System.Dynamic;
using System.Linq;
using System.Threading;
using Whoa;

namespace Plex.Objects
{
    /// <summary>
    /// A class which contains information about a virtual file system.
    /// </summary>
    public class MountInformation
    {
        /// <summary>
        /// The drive number for the mount - for backwards compatibility with older clients.
        /// </summary>
        [Order]
        public int DriveNumber { get; set; }

        /// <summary>
        /// The specification of the mount for backwards compatibility with older clients.
        /// </summary>
        [Order]
        public DriveSpec Specification { get; set; }

        /// <summary>
        /// The volume label of the mount.
        /// </summary>
        [Order]
        public string VolumeLabel { get; set; }

        /// <summary>
        /// The location of the file system (on the real computer's drive).
        /// </summary>
        [Order]
        public string ImageFilePath { get; set; }
    }

    /// <summary>
    /// Represents a virtual file system disk image format.
    /// </summary>
    public enum DriveSpec
    {
        /// <summary>
        /// A JSON-based file system used by ShiftOS 1.0 and early Peacenet builds.
        /// </summary>
        ShiftFS,
        /// <summary>
        /// The Plex File Allocation Table format by Declan Hoare, used in more recent Peacenet builds. The disk image is split into sectors of binary data and the format is streamable. Absolutely NO use of JSON.
        /// </summary>
        PlexFAT
    }
    
}
