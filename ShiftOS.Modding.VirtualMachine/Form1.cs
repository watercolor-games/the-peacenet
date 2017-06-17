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
using ShiftOS.Engine;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.Modding.VirtualMachine
{
    [Launcher("ShiftOS Virtual Machine", false, null, "Programming")]
    public partial class Form1 : UserControl, IShiftOSWindow
    {
        private byte[] memory = null; //program binary
        private int programCounter = 0; //current byte
        private Stack<int> stack = null; //stack
        private int[] registers = null; //registers
        private int currentRegister = 0;


        public Form1()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
        }

        public void Execute()
        {
            switch ((OpCodes)memory[programCounter])
            {
                case OpCodes.DoNothing:

                    break;
                case OpCodes.Add:
                    registers[currentRegister] = registers[currentRegister] + registers[currentRegister + 1];
                    break;
                case OpCodes.Subtract:
                    registers[currentRegister] = registers[currentRegister] - registers[currentRegister + 1];
                    break;
                case OpCodes.Multiply:
                    registers[currentRegister] = registers[currentRegister] * registers[currentRegister + 1];
                    break;
                case OpCodes.Divide:
                    registers[currentRegister] = registers[currentRegister] / registers[currentRegister + 1];
                    break;
                case OpCodes.Increment:
                    registers[currentRegister]++;
                    break;
                case OpCodes.Decrement:
                    registers[currentRegister]--;
                    break;
                case OpCodes.MoveUp:
                    currentRegister++;
                    break;
                case OpCodes.MoveDown:
                    currentRegister--;
                    break;
                case OpCodes.Error:
                    Infobox.Show("Error", "An error has occurred inside the VM, we will now halt it.");
                    break;
                case OpCodes.Jump:
                    stack.Push(programCounter);
                    programCounter = registers[currentRegister];
                    break;
                case OpCodes.Zero:
                    registers[currentRegister] = 0;
                    break;
                case OpCodes.Return:
                    programCounter = memory.Length;
                    break;
                case OpCodes.JumpBack:
                    programCounter = stack.Pop();
                    break;
                case OpCodes.Out:
                    Console.Write(registers[currentRegister]);
                    break;
            }
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppearanceManager.Close(this);
        }

        public void Start()
        {
            var th = new System.Threading.Thread(() =>
            {
                while(programCounter < memory.Length)
                {
                    Execute();
                    programCounter++;
                }
            });
            th.IsBackground = true;
            th.Start();
        }

        private void openBinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".svm" }, FileOpenerStyle.Open, new Action<string>((program) =>
             {
                 memory = Utils.ReadAllBytes(program);
                 programCounter = 0;
                 stack = new Stack<int>();
                 registers = new int[64000];
                 currentRegister = 0;
                 Start();
             }));
        }
    }

    public static class Compiler
    {
        public static byte[] Compile(string prg)
        {
            List<OpCodes> program = new List<OpCodes>();
            foreach(var statement in prg.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrEmpty(statement))
                {
                    string[] statements = statement.Split(' ');
                    switch (statements[0])
                    {
                        case "increment":
                            program.Add(OpCodes.Increment);
                            break;
                        case "decrement":
                            program.Add(OpCodes.Decrement);
                            break;
                        case "mvup":
                            program.Add(OpCodes.MoveUp);
                            break;
                        case "mvdn":
                            program.Add(OpCodes.MoveDown);
                            break;
                        case "set":
                            program.Add(OpCodes.Zero);
                            for(int i = 0; i <= Convert.ToInt32(statements[1]); i++)
                            {
                                program.Add(OpCodes.Increment);
                            }
                            break;
                        case "+":
                            program.Add(OpCodes.Add);
                            break;
                        case "-":
                            program.Add(OpCodes.Subtract);
                            break;
                        case "*":
                            program.Add(OpCodes.Multiply);
                            break;
                        case "/":
                            program.Add(OpCodes.Divide);
                            break;
                        case "out":
                            program.Add(OpCodes.Out);
                            break;
                        case "return":
                            program.Add(OpCodes.Return);
                            break;
                        case "jump":
                            program.Add(OpCodes.Jump);
                            break;
                        case "back":
                            program.Add(OpCodes.JumpBack);
                            break;
                    }
                }
            }

            byte[] newPrg = new byte[program.Count];
            foreach(var oc in program)
            {
                newPrg[program.IndexOf(oc)] = (byte)oc;
            }
            return newPrg;

        }

        [Command("compile")]
        [RequiresArgument("input")]
        [RequiresArgument("output")]
        public static bool CompileCommand(Dictionary<string, object> args)
        {
            string input = args["input"] as string;
            string output = args["output"] as string;
            if (!Utils.FileExists(input))
            {
                Console.WriteLine("The file you requested does not exist.");
                return true;
            }

            if (!output.EndsWith(".svm"))
                output += ".svm";

            byte[] program = Compile(Utils.ReadAllText(input));
            Utils.WriteAllBytes(output, program);
            return true;
        }

        [Command("read")]
        [RequiresArgument("input")]
        public static bool ReadFile(Dictionary<string, object> args)
        {
            string input = args["input"] as string;
            if (Utils.FileExists(input))
            {
                foreach(var b in Utils.ReadAllBytes(input))
                {
                    Console.WriteLine((OpCodes)b);
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist.");
            }

            return true;
        }
    }

    

    public enum OpCodes : byte
    {
        DoNothing = 0x00,
        Zero = 0x11,
        Add = 0x01,
        Subtract = 0x02,
        Multiply = 0x03,
        Divide = 0x04,
        MoveUp = 0x05,
        MoveDown = 0x051,
        Increment = 0x06,
        Decrement = 0x061,
        Out = 0x07,
        Jump = 0x08,
        Return = 0x081,
        JumpBack = 0x082,
        Error = 0x9,

    }
}
