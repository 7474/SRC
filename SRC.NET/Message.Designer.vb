<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmMessage
#Region "Windows フォーム デザイナによって生成されたコード "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'この呼び出しは、Windows フォーム デザイナで必要です。
		InitializeComponent()
	End Sub
	'Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Windows フォーム デザイナで必要です。
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
	Public WithEvents picFace As System.Windows.Forms.PictureBox
	Public WithEvents picUnit1 As System.Windows.Forms.PictureBox
	Public WithEvents picUnit2 As System.Windows.Forms.PictureBox
	Public WithEvents txtHP2 As System.Windows.Forms.TextBox
	Public WithEvents picHP2 As System.Windows.Forms.PictureBox
	Public WithEvents picEN2 As System.Windows.Forms.PictureBox
	Public WithEvents txtEN2 As System.Windows.Forms.TextBox
	Public WithEvents txtEN1 As System.Windows.Forms.TextBox
	Public WithEvents picEN1 As System.Windows.Forms.PictureBox
	Public WithEvents picHP1 As System.Windows.Forms.PictureBox
	Public WithEvents txtHP1 As System.Windows.Forms.TextBox
	Public WithEvents picMessage As System.Windows.Forms.PictureBox
	Public WithEvents labHP2 As System.Windows.Forms.Label
	Public WithEvents labEN2 As System.Windows.Forms.Label
	Public WithEvents labEN1 As System.Windows.Forms.Label
	Public WithEvents labHP1 As System.Windows.Forms.Label
	'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
	'Windows フォーム デザイナを使って変更できます。
	'コード エディタを使用して、変更しないでください。
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmMessage))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.picFace = New System.Windows.Forms.PictureBox
		Me.picUnit1 = New System.Windows.Forms.PictureBox
		Me.picUnit2 = New System.Windows.Forms.PictureBox
		Me.txtHP2 = New System.Windows.Forms.TextBox
		Me.picHP2 = New System.Windows.Forms.PictureBox
		Me.picEN2 = New System.Windows.Forms.PictureBox
		Me.txtEN2 = New System.Windows.Forms.TextBox
		Me.txtEN1 = New System.Windows.Forms.TextBox
		Me.picEN1 = New System.Windows.Forms.PictureBox
		Me.picHP1 = New System.Windows.Forms.PictureBox
		Me.txtHP1 = New System.Windows.Forms.TextBox
		Me.picMessage = New System.Windows.Forms.PictureBox
		Me.labHP2 = New System.Windows.Forms.Label
		Me.labEN2 = New System.Windows.Forms.Label
		Me.labEN1 = New System.Windows.Forms.Label
		Me.labHP1 = New System.Windows.Forms.Label
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Text = "メッセージ"
		Me.ClientSize = New System.Drawing.Size(508, 118)
		Me.Location = New System.Drawing.Point(93, 101)
		Me.Font = New System.Drawing.Font("ＭＳ Ｐ明朝", 12!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.ForeColor = System.Drawing.Color.Black
		Me.Icon = CType(resources.GetObject("frmMessage.Icon"), System.Drawing.Icon)
		Me.KeyPreview = True
		Me.MaximizeBox = False
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ControlBox = True
		Me.Enabled = True
		Me.MinimizeBox = True
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.ShowInTaskbar = True
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "frmMessage"
		Me.picFace.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.picFace.Size = New System.Drawing.Size(68, 68)
		Me.picFace.Location = New System.Drawing.Point(8, 43)
		Me.picFace.TabIndex = 15
		Me.picFace.Dock = System.Windows.Forms.DockStyle.None
		Me.picFace.CausesValidation = True
		Me.picFace.Enabled = True
		Me.picFace.ForeColor = System.Drawing.SystemColors.ControlText
		Me.picFace.Cursor = System.Windows.Forms.Cursors.Default
		Me.picFace.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picFace.TabStop = True
		Me.picFace.Visible = True
		Me.picFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picFace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picFace.Name = "picFace"
		Me.picUnit1.BackColor = System.Drawing.SystemColors.Window
		Me.picUnit1.ForeColor = System.Drawing.SystemColors.WindowText
		Me.picUnit1.Size = New System.Drawing.Size(32, 32)
		Me.picUnit1.Location = New System.Drawing.Point(8, 4)
		Me.picUnit1.TabIndex = 14
		Me.picUnit1.Visible = False
		Me.picUnit1.Dock = System.Windows.Forms.DockStyle.None
		Me.picUnit1.CausesValidation = True
		Me.picUnit1.Enabled = True
		Me.picUnit1.Cursor = System.Windows.Forms.Cursors.Default
		Me.picUnit1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picUnit1.TabStop = True
		Me.picUnit1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picUnit1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.picUnit1.Name = "picUnit1"
		Me.picUnit2.BackColor = System.Drawing.SystemColors.Window
		Me.picUnit2.ForeColor = System.Drawing.SystemColors.WindowText
		Me.picUnit2.Size = New System.Drawing.Size(32, 32)
		Me.picUnit2.Location = New System.Drawing.Point(260, 5)
		Me.picUnit2.TabIndex = 13
		Me.picUnit2.Visible = False
		Me.picUnit2.Dock = System.Windows.Forms.DockStyle.None
		Me.picUnit2.CausesValidation = True
		Me.picUnit2.Enabled = True
		Me.picUnit2.Cursor = System.Windows.Forms.Cursors.Default
		Me.picUnit2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picUnit2.TabStop = True
		Me.picUnit2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picUnit2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.picUnit2.Name = "picUnit2"
		Me.txtHP2.AutoSize = False
		Me.txtHP2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtHP2.Font = New System.Drawing.Font("ＭＳ 明朝", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtHP2.ForeColor = System.Drawing.Color.Black
		Me.txtHP2.Size = New System.Drawing.Size(88, 13)
		Me.txtHP2.Location = New System.Drawing.Point(323, 10)
		Me.txtHP2.TabIndex = 10
		Me.txtHP2.Text = "99999/99999"
		Me.txtHP2.AcceptsReturn = True
		Me.txtHP2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtHP2.CausesValidation = True
		Me.txtHP2.Enabled = True
		Me.txtHP2.HideSelection = True
		Me.txtHP2.ReadOnly = False
		Me.txtHP2.Maxlength = 0
		Me.txtHP2.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtHP2.MultiLine = False
		Me.txtHP2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtHP2.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtHP2.TabStop = True
		Me.txtHP2.Visible = True
		Me.txtHP2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtHP2.Name = "txtHP2"
		Me.picHP2.BackColor = System.Drawing.Color.FromARGB(192, 0, 0)
		Me.picHP2.ForeColor = System.Drawing.Color.FromARGB(0, 192, 0)
		Me.picHP2.Size = New System.Drawing.Size(116, 8)
		Me.picHP2.Location = New System.Drawing.Point(297, 28)
		Me.picHP2.TabIndex = 9
		Me.picHP2.Dock = System.Windows.Forms.DockStyle.None
		Me.picHP2.CausesValidation = True
		Me.picHP2.Enabled = True
		Me.picHP2.Cursor = System.Windows.Forms.Cursors.Default
		Me.picHP2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picHP2.TabStop = True
		Me.picHP2.Visible = True
		Me.picHP2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picHP2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picHP2.Name = "picHP2"
		Me.picEN2.BackColor = System.Drawing.Color.FromARGB(192, 0, 0)
		Me.picEN2.ForeColor = System.Drawing.Color.FromARGB(0, 192, 0)
		Me.picEN2.Size = New System.Drawing.Size(82, 8)
		Me.picEN2.Location = New System.Drawing.Point(418, 28)
		Me.picEN2.TabIndex = 8
		Me.picEN2.Dock = System.Windows.Forms.DockStyle.None
		Me.picEN2.CausesValidation = True
		Me.picEN2.Enabled = True
		Me.picEN2.Cursor = System.Windows.Forms.Cursors.Default
		Me.picEN2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picEN2.TabStop = True
		Me.picEN2.Visible = True
		Me.picEN2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picEN2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picEN2.Name = "picEN2"
		Me.txtEN2.AutoSize = False
		Me.txtEN2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtEN2.Font = New System.Drawing.Font("ＭＳ 明朝", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtEN2.ForeColor = System.Drawing.Color.Black
		Me.txtEN2.Size = New System.Drawing.Size(57, 13)
		Me.txtEN2.Location = New System.Drawing.Point(443, 10)
		Me.txtEN2.TabIndex = 7
		Me.txtEN2.Text = "999/999"
		Me.txtEN2.AcceptsReturn = True
		Me.txtEN2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtEN2.CausesValidation = True
		Me.txtEN2.Enabled = True
		Me.txtEN2.HideSelection = True
		Me.txtEN2.ReadOnly = False
		Me.txtEN2.Maxlength = 0
		Me.txtEN2.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtEN2.MultiLine = False
		Me.txtEN2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtEN2.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtEN2.TabStop = True
		Me.txtEN2.Visible = True
		Me.txtEN2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtEN2.Name = "txtEN2"
		Me.txtEN1.AutoSize = False
		Me.txtEN1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtEN1.Font = New System.Drawing.Font("ＭＳ 明朝", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtEN1.ForeColor = System.Drawing.Color.Black
		Me.txtEN1.Size = New System.Drawing.Size(57, 13)
		Me.txtEN1.Location = New System.Drawing.Point(192, 10)
		Me.txtEN1.TabIndex = 6
		Me.txtEN1.Text = "999/999"
		Me.txtEN1.AcceptsReturn = True
		Me.txtEN1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtEN1.CausesValidation = True
		Me.txtEN1.Enabled = True
		Me.txtEN1.HideSelection = True
		Me.txtEN1.ReadOnly = False
		Me.txtEN1.Maxlength = 0
		Me.txtEN1.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtEN1.MultiLine = False
		Me.txtEN1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtEN1.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtEN1.TabStop = True
		Me.txtEN1.Visible = True
		Me.txtEN1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtEN1.Name = "txtEN1"
		Me.picEN1.BackColor = System.Drawing.Color.FromARGB(192, 0, 0)
		Me.picEN1.ForeColor = System.Drawing.Color.FromARGB(0, 192, 0)
		Me.picEN1.Size = New System.Drawing.Size(83, 8)
		Me.picEN1.Location = New System.Drawing.Point(166, 28)
		Me.picEN1.TabIndex = 5
		Me.picEN1.Dock = System.Windows.Forms.DockStyle.None
		Me.picEN1.CausesValidation = True
		Me.picEN1.Enabled = True
		Me.picEN1.Cursor = System.Windows.Forms.Cursors.Default
		Me.picEN1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picEN1.TabStop = True
		Me.picEN1.Visible = True
		Me.picEN1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picEN1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picEN1.Name = "picEN1"
		Me.picHP1.BackColor = System.Drawing.Color.FromARGB(192, 0, 0)
		Me.picHP1.ForeColor = System.Drawing.Color.FromARGB(0, 192, 0)
		Me.picHP1.Size = New System.Drawing.Size(116, 8)
		Me.picHP1.Location = New System.Drawing.Point(45, 28)
		Me.picHP1.TabIndex = 3
		Me.picHP1.Dock = System.Windows.Forms.DockStyle.None
		Me.picHP1.CausesValidation = True
		Me.picHP1.Enabled = True
		Me.picHP1.Cursor = System.Windows.Forms.Cursors.Default
		Me.picHP1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picHP1.TabStop = True
		Me.picHP1.Visible = True
		Me.picHP1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picHP1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picHP1.Name = "picHP1"
		Me.txtHP1.AutoSize = False
		Me.txtHP1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtHP1.Font = New System.Drawing.Font("ＭＳ 明朝", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtHP1.ForeColor = System.Drawing.Color.Black
		Me.txtHP1.Size = New System.Drawing.Size(88, 13)
		Me.txtHP1.Location = New System.Drawing.Point(72, 10)
		Me.txtHP1.TabIndex = 2
		Me.txtHP1.Text = "99999/99999"
		Me.txtHP1.AcceptsReturn = True
		Me.txtHP1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtHP1.CausesValidation = True
		Me.txtHP1.Enabled = True
		Me.txtHP1.HideSelection = True
		Me.txtHP1.ReadOnly = False
		Me.txtHP1.Maxlength = 0
		Me.txtHP1.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtHP1.MultiLine = False
		Me.txtHP1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtHP1.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtHP1.TabStop = True
		Me.txtHP1.Visible = True
		Me.txtHP1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtHP1.Name = "txtHP1"
		Me.picMessage.BackColor = System.Drawing.Color.White
		Me.picMessage.Font = New System.Drawing.Font("ＭＳ Ｐ明朝", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.picMessage.ForeColor = System.Drawing.Color.Black
		Me.picMessage.Size = New System.Drawing.Size(417, 70)
		Me.picMessage.Location = New System.Drawing.Point(84, 42)
		Me.picMessage.TabIndex = 0
		Me.picMessage.Dock = System.Windows.Forms.DockStyle.None
		Me.picMessage.CausesValidation = True
		Me.picMessage.Enabled = True
		Me.picMessage.Cursor = System.Windows.Forms.Cursors.Default
		Me.picMessage.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picMessage.TabStop = True
		Me.picMessage.Visible = True
		Me.picMessage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picMessage.Name = "picMessage"
		Me.labHP2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labHP2.Text = "HP"
		Me.labHP2.ForeColor = System.Drawing.Color.Black
		Me.labHP2.Size = New System.Drawing.Size(22, 17)
		Me.labHP2.Location = New System.Drawing.Point(296, 8)
		Me.labHP2.TabIndex = 12
		Me.labHP2.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labHP2.Enabled = True
		Me.labHP2.Cursor = System.Windows.Forms.Cursors.Default
		Me.labHP2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labHP2.UseMnemonic = True
		Me.labHP2.Visible = True
		Me.labHP2.AutoSize = False
		Me.labHP2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labHP2.Name = "labHP2"
		Me.labEN2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labEN2.Text = "EN"
		Me.labEN2.ForeColor = System.Drawing.Color.Black
		Me.labEN2.Size = New System.Drawing.Size(22, 17)
		Me.labEN2.Location = New System.Drawing.Point(417, 8)
		Me.labEN2.TabIndex = 11
		Me.labEN2.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labEN2.Enabled = True
		Me.labEN2.Cursor = System.Windows.Forms.Cursors.Default
		Me.labEN2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labEN2.UseMnemonic = True
		Me.labEN2.Visible = True
		Me.labEN2.AutoSize = False
		Me.labEN2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labEN2.Name = "labEN2"
		Me.labEN1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labEN1.Text = "EN"
		Me.labEN1.ForeColor = System.Drawing.Color.Black
		Me.labEN1.Size = New System.Drawing.Size(22, 17)
		Me.labEN1.Location = New System.Drawing.Point(165, 8)
		Me.labEN1.TabIndex = 4
		Me.labEN1.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labEN1.Enabled = True
		Me.labEN1.Cursor = System.Windows.Forms.Cursors.Default
		Me.labEN1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labEN1.UseMnemonic = True
		Me.labEN1.Visible = True
		Me.labEN1.AutoSize = False
		Me.labEN1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labEN1.Name = "labEN1"
		Me.labHP1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labHP1.Text = "HP"
		Me.labHP1.ForeColor = System.Drawing.Color.Black
		Me.labHP1.Size = New System.Drawing.Size(22, 17)
		Me.labHP1.Location = New System.Drawing.Point(44, 8)
		Me.labHP1.TabIndex = 1
		Me.labHP1.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labHP1.Enabled = True
		Me.labHP1.Cursor = System.Windows.Forms.Cursors.Default
		Me.labHP1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labHP1.UseMnemonic = True
		Me.labHP1.Visible = True
		Me.labHP1.AutoSize = False
		Me.labHP1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labHP1.Name = "labHP1"
		Me.Controls.Add(picFace)
		Me.Controls.Add(picUnit1)
		Me.Controls.Add(picUnit2)
		Me.Controls.Add(txtHP2)
		Me.Controls.Add(picHP2)
		Me.Controls.Add(picEN2)
		Me.Controls.Add(txtEN2)
		Me.Controls.Add(txtEN1)
		Me.Controls.Add(picEN1)
		Me.Controls.Add(picHP1)
		Me.Controls.Add(txtHP1)
		Me.Controls.Add(picMessage)
		Me.Controls.Add(labHP2)
		Me.Controls.Add(labEN2)
		Me.Controls.Add(labEN1)
		Me.Controls.Add(labHP1)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class