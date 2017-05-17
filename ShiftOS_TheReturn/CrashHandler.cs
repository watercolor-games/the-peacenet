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
using System.Reflection;
using System.Management;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading;
using System.IO;

namespace ShiftOS.Engine
{
    public class GetHardwareInfo
    {
        // returns the processor's name for the crash
        public static string GetProcessorName()
        {
            string ProcessorName = ""; // put the processors name in here sometime later
            ManagementObjectSearcher mos
    = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor"); // OI CPU TELL ME YOUR NAME PLZ

            foreach (ManagementObject mo in mos.Get())
                ProcessorName = mo["Name"].ToString(); // see told you it puts in it there

            return ProcessorName;
        }
        // same as above but instead for the gpu's name for the crash as well
        public static string GetGPUName()
        {
            string GPUName = "";
            ManagementObjectSearcher mos
    = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController"); //now can you tell me your name cpu kthx

            foreach (ManagementObject mo in mos.Get())
                GPUName = mo["Name"].ToString();

            return GPUName;
        }
        // oh wow even more same, but this time its RAM AMOUNT oooh nice
        public static string GetRAMAmount()
        {
            var RAMAmount = "";
            ManagementObjectSearcher mos
    = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory"); //ram you too how much of you exists

            foreach (ManagementObject mo in mos.Get())
                RAMAmount = mo["Capacity"].ToString();

            RAMAmount = (RAMAmount + " B"); // ooh and now we add "bytes" to the end

            return RAMAmount;
        }
    }


    public partial class CrashHandler : Form
    {
        //fuck it crashed
        public CrashHandler()
        {
            InitializeComponent();

            
            //Send the bug to Unite as a bug report
            // or alternatively, send to reportbug@shiftos.ml
            // or just on the discord that works too
            
        }

        public static Exception HandledException = null; // this value determines if we can try to set the game back on track or we cant do anything about it

        public static void Start(Exception e)
        {
            if(SaveSystem.CurrentSave != null)
                TerminalBackend.InvokeCommand("sos.save");  // SAVE BEFORE CRASHING
            ServerManager.Disconnect();

            while (Application.OpenForms.Count > 0)
                Application.OpenForms[0].Close();

            //Set our global exception variable, and show the exception dialog.
            HandledException = e;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;

            // put all this in a text document
            string rtbcrash_Text = $@" === {AssemblyName} has crashed. === 

Game:           {AssemblyName}
Description:    {AssemblyDescription}

Basic Information For User:
---------------------------------

When:   {DateTime.Now.ToString()}
Why:    {HandledException.Message}
What:   {HandledException.GetType().Name}

We, at the ShiftOS Development Team, apologise for your game crash,
we will take this bug report seriously - and it has been emailed
to the development team of ShiftOS, thank you for enjoying our game!

Advanced Information (for experts and developers):
----------------------------------------------------

Host system information:
---------------------------------

Operating system:   {Environment.OSVersion.Platform.ToString()}
Version:            {Environment.OSVersion.VersionString}
Is 64-bit:          {Environment.Is64BitOperatingSystem}
ShiftOS exec path:  {Application.ExecutablePath}

Advanced Host Information:
---------------------------------

CPU Name:           {GetHardwareInfo.GetProcessorName()}
Physical RAM Installed:         {GetHardwareInfo.GetRAMAmount()}
GPU Name:           {GetHardwareInfo.GetGPUName()}

ShiftOS basic information:
---------------------------------

ShiftOS Version:    {Assembly.GetExecutingAssembly().GetName().Version}
ShiftOS Date:       {lastModified.ToString()}

ShiftOS environment information:
---------------------------------

Is Save loaded:             {(SaveSystem.CurrentSave != null)}
Paths loaded in system:     {JsonConvert.SerializeObject(Paths.GetAll())}


Crash: {HandledException.GetType().Name}
--------------------------------------------

Exception message:              {HandledException.Message}
HResult (this is technical):    {HandledException.HResult}
Has inner exception:            {(HandledException.InnerException != null)}
Stack trace:
{HandledException.StackTrace}";

            if (HandledException.InnerException != null)
            {
                var i = HandledException.InnerException;
                rtbcrash_Text += $@"

Inner: {i.GetType().Name}
--------------------------------------------

Exception message:              {i.Message}
HResult (this is technical):    {i.HResult}
Stack trace:
{i.StackTrace}";

            }

            File.WriteAllText("crash.txt", rtbcrash_Text); // make that text document and put above super long string in it
            var result = MessageBox.Show(caption: "ShiftOS - Fatal error", text: "ShiftOS has encountered a fatal error and has been shut down. Info about the error has been saved to a file called crash.txt in the same folder as the active executable. Would you like to try and recover the game session?", buttons: MessageBoxButtons.YesNo);
            if(result == DialogResult.Yes)
            {
                Application.Restart(); // tries to restart if user clicks yes, who wouldve guessed
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnjump_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Restart();
        }
        
        // make both of those variables that appear in the long string above
        public static string AssemblyName { get; private set; }
        public static string AssemblyDescription { get; private set; }

        // get info about the game itself
        public static void SetGameMetadata(Assembly assembly)
        {
            AssemblyName = assembly.GetName().Name; // name of game
            foreach(var attr in assembly.GetCustomAttributes(true))
            {
                if(attr is AssemblyDescriptionAttribute)
                {
                    AssemblyDescription = (attr as AssemblyDescriptionAttribute).Description; // description of the game
                }
            }

        }
    }
}
