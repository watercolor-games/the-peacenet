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
        public static string GetProcessorName()
        {
            string ProcessorName = "";
            ManagementObjectSearcher mos
    = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

            foreach (ManagementObject mo in mos.Get())
                ProcessorName = mo["Name"].ToString();

            return ProcessorName;
        }
        public static string GetGPUName()
        {
            string GPUName = "";
            ManagementObjectSearcher mos
    = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");

            foreach (ManagementObject mo in mos.Get())
                GPUName = mo["Name"].ToString();

            return GPUName;
        }
        public static string GetRAMAmount()
        {
            var RAMAmount = "";
            ManagementObjectSearcher mos
    = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");

            foreach (ManagementObject mo in mos.Get())
                RAMAmount = mo["Capacity"].ToString();

            RAMAmount = (RAMAmount + " B");

            return RAMAmount;
        }
    }


    public partial class CrashHandler : Form
    {
        public CrashHandler()
        {
            InitializeComponent();

            
            //Send the bug to Debugle
            // or alternatively, send to reportbug@shiftos.ml OR narodgaming@shiftos.ml
            
        }

        public static Exception HandledException = null;

        public static void Start(Exception e)
        {
            if(SaveSystem.CurrentSave != null)
                TerminalBackend.InvokeCommand("sos.save");
            AudioManager.Kill();
            ServerManager.Disconnect();

            while (Application.OpenForms.Count > 0)
                Application.OpenForms[0].Close();

            //Set our global exception variable, and show the exception dialog.
            HandledException = e;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;

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

            File.WriteAllText("crash.txt", rtbcrash_Text);
            var result = MessageBox.Show(caption: "ShiftOS - Fatal error", text: "ShiftOS has encountered a fatal error and has been shut down. Info about the error has been saved to a file called crash.txt in the same folder as the active executable. Would you like to try and recover the game session?", buttons: MessageBoxButtons.YesNo);
            if(result == DialogResult.Yes)
            {
                Application.Restart();
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

        public static string AssemblyName { get; private set; }
        public static string AssemblyDescription { get; private set; }

        public static void SetGameMetadata(Assembly assembly)
        {
            AssemblyName = assembly.GetName().Name;
            foreach(var attr in assembly.GetCustomAttributes(true))
            {
                if(attr is AssemblyDescriptionAttribute)
                {
                    AssemblyDescription = (attr as AssemblyDescriptionAttribute).Description;
                }
            }

        }
    }
}
