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
            this.btnAddColor = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.txtentersyntax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnloaddefault = new System.Windows.Forms.Button();
            this.panelEditor.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelEditor
            // 
            this.panelEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelEditor.AutoScroll = true;
            this.panelEditor.Controls.Add(this.label1);
            this.panelEditor.Controls.Add(this.txtentersyntax);
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
            // 
            // btnAddOptionalText
            // 
            this.btnAddOptionalText.Location = new System.Drawing.Point(85, 56);
            this.btnAddOptionalText.Name = "btnAddOptionalText";
            this.btnAddOptionalText.Size = new System.Drawing.Size(107, 23);
            this.btnAddOptionalText.TabIndex = 3;
            this.btnAddOptionalText.Text = "Add Optional Text";
            this.btnAddOptionalText.UseVisualStyleBackColor = true;
            // 
            // btnAddRegexText
            // 
            this.btnAddRegexText.Location = new System.Drawing.Point(198, 56);
            this.btnAddRegexText.Name = "btnAddRegexText";
            this.btnAddRegexText.Size = new System.Drawing.Size(107, 23);
            this.btnAddRegexText.TabIndex = 4;
            this.btnAddRegexText.Text = "Add Regex Text";
            this.btnAddRegexText.UseVisualStyleBackColor = true;
            // 
            // btnAddCommand
            // 
            this.btnAddCommand.Location = new System.Drawing.Point(4, 85);
            this.btnAddCommand.Name = "btnAddCommand";
            this.btnAddCommand.Size = new System.Drawing.Size(85, 23);
            this.btnAddCommand.TabIndex = 5;
            this.btnAddCommand.Text = "+ Namespace";
            this.btnAddCommand.UseVisualStyleBackColor = true;
            // 
            // btnAddColor
            // 
            this.btnAddColor.Location = new System.Drawing.Point(311, 56);
            this.btnAddColor.Name = "btnAddColor";
            this.btnAddColor.Size = new System.Drawing.Size(64, 23);
            this.btnAddColor.TabIndex = 9;
            this.btnAddColor.Text = "Add Color";
            this.btnAddColor.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 23);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "{GEN_SAVE}";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.AutoSize = true;
            this.btnLoad.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLoad.Location = new System.Drawing.Point(91, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(83, 23);
            this.btnLoad.TabIndex = 14;
            this.btnLoad.Text = "{GEN_LOAD}";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.AutoSize = true;
            this.btnApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApply.Location = new System.Drawing.Point(318, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(88, 23);
            this.btnApply.TabIndex = 15;
            this.btnApply.Text = "{GEN_APPLY}";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // txtentersyntax
            // 
            this.txtentersyntax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtentersyntax.Location = new System.Drawing.Point(3, 23);
            this.txtentersyntax.Name = "txtentersyntax";
            this.txtentersyntax.Size = new System.Drawing.Size(583, 20);
            this.txtentersyntax.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "{FORMATEDITOR_ENTERSYNTAX}";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Controls.Add(this.btnLoad);
            this.flowLayoutPanel1.Controls.Add(this.btnloaddefault);
            this.flowLayoutPanel1.Controls.Add(this.btnApply);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(11, 123);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(409, 29);
            this.flowLayoutPanel1.TabIndex = 16;
            // 
            // btnloaddefault
            // 
            this.btnloaddefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnloaddefault.AutoSize = true;
            this.btnloaddefault.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnloaddefault.Location = new System.Drawing.Point(180, 3);
            this.btnloaddefault.Name = "btnloaddefault";
            this.btnloaddefault.Size = new System.Drawing.Size(132, 23);
            this.btnloaddefault.TabIndex = 16;
            this.btnloaddefault.Text = "{GEN_LOADDEFAULT}";
            this.btnloaddefault.UseVisualStyleBackColor = true;
            // 
            // FormatEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.btnAddColor);
            this.Controls.Add(this.btnAddText);
            this.Controls.Add(this.btnAddOptionalText);
            this.Controls.Add(this.btnAddRegexText);
            this.Controls.Add(this.btnAddCommand);
            this.Controls.Add(this.panelEditor);
            this.Name = "FormatEditor";
            this.Size = new System.Drawing.Size(596, 164);
            this.panelEditor.ResumeLayout(false);
            this.panelEditor.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelEditor;
        private System.Windows.Forms.Button btnAddText;
        private System.Windows.Forms.Button btnAddOptionalText;
        private System.Windows.Forms.Button btnAddRegexText;
        private System.Windows.Forms.Button btnAddCommand;
        private System.Windows.Forms.Button btnAddColor;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtentersyntax;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnloaddefault;
    }
}
