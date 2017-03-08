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

namespace ShiftOS.WinForms.Applications
{
    partial class FormatEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelEditor = new System.Windows.Forms.Panel();
            this.btnAddText = new System.Windows.Forms.Button();
            this.btnAddOptionalText = new System.Windows.Forms.Button();
            this.btnAddRegexText = new System.Windows.Forms.Button();
            this.btnAddCommand = new System.Windows.Forms.Button();
            this.lblExampleCommand = new System.Windows.Forms.Label();
            this.btnAddColor = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panelEditor
            // 
            this.panelEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelEditor.AutoScroll = true;
            this.panelEditor.Location = new System.Drawing.Point(4, 4);
            this.panelEditor.Name = "panelEditor";
            this.panelEditor.Size = new System.Drawing.Size(589, 46);
            this.panelEditor.TabIndex = 1;
            // 
            // btnAddText
            // 
            this.btnAddText.Location = new System.Drawing.Point(4, 56);
            this.btnAddText.Name = "btnAddText";
            this.btnAddText.Size = new System.Drawing.Size(75, 23);
            this.btnAddText.TabIndex = 2;
            this.btnAddText.Text = "Add Text";
            this.btnAddText.UseVisualStyleBackColor = true;
            this.btnAddText.Click += new System.EventHandler(this.btnAddText_Click);
            // 
            // btnAddOptionalText
            // 
            this.btnAddOptionalText.Location = new System.Drawing.Point(85, 56);
            this.btnAddOptionalText.Name = "btnAddOptionalText";
            this.btnAddOptionalText.Size = new System.Drawing.Size(107, 23);
            this.btnAddOptionalText.TabIndex = 3;
            this.btnAddOptionalText.Text = "Add Optional Text";
            this.btnAddOptionalText.UseVisualStyleBackColor = true;
            this.btnAddOptionalText.Click += new System.EventHandler(this.btnAddOptionalText_Click);
            // 
            // btnAddRegexText
            // 
            this.btnAddRegexText.Location = new System.Drawing.Point(198, 56);
            this.btnAddRegexText.Name = "btnAddRegexText";
            this.btnAddRegexText.Size = new System.Drawing.Size(107, 23);
            this.btnAddRegexText.TabIndex = 4;
            this.btnAddRegexText.Text = "Add Regex Text";
            this.btnAddRegexText.UseVisualStyleBackColor = true;
            this.btnAddRegexText.Click += new System.EventHandler(this.btnAddRegexText_Click);
            // 
            // btnAddCommand
            // 
            this.btnAddCommand.Location = new System.Drawing.Point(4, 85);
            this.btnAddCommand.Name = "btnAddCommand";
            this.btnAddCommand.Size = new System.Drawing.Size(75, 23);
            this.btnAddCommand.TabIndex = 5;
            this.btnAddCommand.Text = "+ Command";
            this.btnAddCommand.UseVisualStyleBackColor = true;
            this.btnAddCommand.Click += new System.EventHandler(this.btnAddCommand_Click);
            // 
            // lblExampleCommand
            // 
            this.lblExampleCommand.AutoSize = true;
            this.lblExampleCommand.Location = new System.Drawing.Point(4, 115);
            this.lblExampleCommand.Name = "lblExampleCommand";
            this.lblExampleCommand.Size = new System.Drawing.Size(290, 13);
            this.lblExampleCommand.TabIndex = 8;
            this.lblExampleCommand.Text = "Create a command and an example usage will show up here";
            // 
            // btnAddColor
            // 
            this.btnAddColor.Location = new System.Drawing.Point(311, 56);
            this.btnAddColor.Name = "btnAddColor";
            this.btnAddColor.Size = new System.Drawing.Size(64, 23);
            this.btnAddColor.TabIndex = 9;
            this.btnAddColor.Text = "Add Color";
            this.btnAddColor.UseVisualStyleBackColor = true;
            this.btnAddColor.Click += new System.EventHandler(this.btnAddColor_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(7, 132);
            this.richTextBox1.Multiline = false;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(476, 23);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(490, 132);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(47, 23);
            this.btnTest.TabIndex = 11;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(7, 161);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(47, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(60, 161);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(47, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // FormatEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnAddColor);
            this.Controls.Add(this.lblExampleCommand);
            this.Controls.Add(this.btnAddText);
            this.Controls.Add(this.btnAddOptionalText);
            this.Controls.Add(this.btnAddRegexText);
            this.Controls.Add(this.btnAddCommand);
            this.Controls.Add(this.panelEditor);
            this.Name = "FormatEditor";
            this.Size = new System.Drawing.Size(596, 426);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelEditor;
        private System.Windows.Forms.Button btnAddText;
        private System.Windows.Forms.Button btnAddOptionalText;
        private System.Windows.Forms.Button btnAddRegexText;
        private System.Windows.Forms.Button btnAddCommand;
        private System.Windows.Forms.Label lblExampleCommand;
        private System.Windows.Forms.Button btnAddColor;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
