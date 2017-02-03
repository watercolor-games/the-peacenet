<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SkinConverter
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.txtin = New System.Windows.Forms.TextBox()
        Me.txtout = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnin = New System.Windows.Forms.Button()
        Me.btnout = New System.Windows.Forms.Button()
        Me.btnconvert = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnconvert)
        Me.Panel1.Controls.Add(Me.btnout)
        Me.Panel1.Controls.Add(Me.btnin)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.txtout)
        Me.Panel1.Controls.Add(Me.txtin)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(397, 183)
        Me.Panel1.TabIndex = 0
        '
        'txtin
        '
        Me.txtin.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtin.Location = New System.Drawing.Point(81, 21)
        Me.txtin.Name = "txtin"
        Me.txtin.ReadOnly = True
        Me.txtin.Size = New System.Drawing.Size(221, 20)
        Me.txtin.TabIndex = 0
        '
        'txtout
        '
        Me.txtout.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtout.Location = New System.Drawing.Point(81, 51)
        Me.txtout.Name = "txtout"
        Me.txtout.ReadOnly = True
        Me.txtout.Size = New System.Drawing.Size(221, 20)
        Me.txtout.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Input file:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(17, 54)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(58, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Output file:"
        '
        'btnin
        '
        Me.btnin.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnin.Location = New System.Drawing.Point(308, 19)
        Me.btnin.Name = "btnin"
        Me.btnin.Size = New System.Drawing.Size(75, 23)
        Me.btnin.TabIndex = 4
        Me.btnin.Text = "Browse"
        Me.btnin.UseVisualStyleBackColor = True
        '
        'btnout
        '
        Me.btnout.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnout.Location = New System.Drawing.Point(308, 48)
        Me.btnout.Name = "btnout"
        Me.btnout.Size = New System.Drawing.Size(75, 23)
        Me.btnout.TabIndex = 5
        Me.btnout.Text = "Browse"
        Me.btnout.UseVisualStyleBackColor = True
        '
        'btnconvert
        '
        Me.btnconvert.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnconvert.AutoSize = True
        Me.btnconvert.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnconvert.Location = New System.Drawing.Point(168, 151)
        Me.btnconvert.Name = "btnconvert"
        Me.btnconvert.Size = New System.Drawing.Size(54, 23)
        Me.btnconvert.TabIndex = 6
        Me.btnconvert.Text = "Convert"
        Me.btnconvert.UseVisualStyleBackColor = True
        '
        'SkinConverter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Panel1)
        Me.Name = "SkinConverter"
        Me.Size = New System.Drawing.Size(397, 183)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnconvert As System.Windows.Forms.Button
    Friend WithEvents btnout As System.Windows.Forms.Button
    Friend WithEvents btnin As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtout As System.Windows.Forms.TextBox
    Friend WithEvents txtin As System.Windows.Forms.TextBox
End Class
