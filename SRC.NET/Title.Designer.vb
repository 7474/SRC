<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmTitle
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
	Public WithEvents Picture1 As System.Windows.Forms.PictureBox
	Public WithEvents Image1 As System.Windows.Forms.PictureBox
	Public WithEvents labAuthor As System.Windows.Forms.Label
	Public WithEvents labVersion As System.Windows.Forms.Label
	Public WithEvents Frame1 As System.Windows.Forms.GroupBox
	Public WithEvents labLicense As System.Windows.Forms.Label
	'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
	'Windows フォーム デザイナを使って変更できます。
	'コード エディタを使用して、変更しないでください。
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmTitle))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.Picture1 = New System.Windows.Forms.PictureBox
		Me.Frame1 = New System.Windows.Forms.GroupBox
		Me.Image1 = New System.Windows.Forms.PictureBox
		Me.labAuthor = New System.Windows.Forms.Label
		Me.labVersion = New System.Windows.Forms.Label
		Me.labLicense = New System.Windows.Forms.Label
		Me.Frame1.SuspendLayout()
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
		Me.Text = "SRC"
		Me.ClientSize = New System.Drawing.Size(386, 233)
		Me.Location = New System.Drawing.Point(180, 197)
		Me.Icon = CType(resources.GetObject("frmTitle.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.ShowInTaskbar = False
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.ControlBox = True
		Me.Enabled = True
		Me.KeyPreview = False
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "frmTitle"
		Me.Picture1.Size = New System.Drawing.Size(200, 40)
		Me.Picture1.Location = New System.Drawing.Point(168, 88)
		Me.Picture1.Image = CType(resources.GetObject("Picture1.Image"), System.Drawing.Image)
		Me.Picture1.TabIndex = 0
		Me.Picture1.Dock = System.Windows.Forms.DockStyle.None
		Me.Picture1.BackColor = System.Drawing.SystemColors.Control
		Me.Picture1.CausesValidation = True
		Me.Picture1.Enabled = True
		Me.Picture1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Picture1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Picture1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Picture1.TabStop = True
		Me.Picture1.Visible = True
		Me.Picture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
		Me.Picture1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Picture1.Name = "Picture1"
		Me.Frame1.Size = New System.Drawing.Size(337, 201)
		Me.Frame1.Location = New System.Drawing.Point(24, 8)
		Me.Frame1.TabIndex = 1
		Me.Frame1.BackColor = System.Drawing.SystemColors.Control
		Me.Frame1.Enabled = True
		Me.Frame1.ForeColor = System.Drawing.SystemColors.ControlText
		Me.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Frame1.Visible = True
		Me.Frame1.Padding = New System.Windows.Forms.Padding(0)
		Me.Frame1.Name = "Frame1"
		Me.Image1.Size = New System.Drawing.Size(96, 96)
		Me.Image1.Location = New System.Drawing.Point(16, 56)
		Me.Image1.Image = CType(resources.GetObject("Image1.Image"), System.Drawing.Image)
		Me.Image1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.Image1.Enabled = True
		Me.Image1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Image1.Visible = True
		Me.Image1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Image1.Name = "Image1"
		Me.labAuthor.TextAlign = System.Drawing.ContentAlignment.TopRight
		Me.labAuthor.Text = "Kei Sakamoto / Inui Tetsuyuki"
		Me.labAuthor.Font = New System.Drawing.Font("Times New Roman", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.labAuthor.Size = New System.Drawing.Size(193, 17)
		Me.labAuthor.Location = New System.Drawing.Point(128, 168)
		Me.labAuthor.TabIndex = 3
		Me.labAuthor.BackColor = System.Drawing.SystemColors.Control
		Me.labAuthor.Enabled = True
		Me.labAuthor.ForeColor = System.Drawing.SystemColors.ControlText
		Me.labAuthor.Cursor = System.Windows.Forms.Cursors.Default
		Me.labAuthor.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labAuthor.UseMnemonic = True
		Me.labAuthor.Visible = True
		Me.labAuthor.AutoSize = False
		Me.labAuthor.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labAuthor.Name = "labAuthor"
		Me.labVersion.TextAlign = System.Drawing.ContentAlignment.TopRight
		Me.labVersion.Text = "Ver 1.7.*"
		Me.labVersion.Font = New System.Drawing.Font("Times New Roman", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.labVersion.Size = New System.Drawing.Size(177, 25)
		Me.labVersion.Location = New System.Drawing.Point(144, 136)
		Me.labVersion.TabIndex = 2
		Me.labVersion.BackColor = System.Drawing.SystemColors.Control
		Me.labVersion.Enabled = True
		Me.labVersion.ForeColor = System.Drawing.SystemColors.ControlText
		Me.labVersion.Cursor = System.Windows.Forms.Cursors.Default
		Me.labVersion.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labVersion.UseMnemonic = True
		Me.labVersion.Visible = True
		Me.labVersion.AutoSize = False
		Me.labVersion.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labVersion.Name = "labVersion"
		Me.labLicense.TextAlign = System.Drawing.ContentAlignment.TopCenter
		Me.labLicense.Text = "This program is distributed under the terms of GPL"
		Me.labLicense.Size = New System.Drawing.Size(369, 17)
		Me.labLicense.Location = New System.Drawing.Point(8, 216)
		Me.labLicense.TabIndex = 4
		Me.labLicense.BackColor = System.Drawing.SystemColors.Control
		Me.labLicense.Enabled = True
		Me.labLicense.ForeColor = System.Drawing.SystemColors.ControlText
		Me.labLicense.Cursor = System.Windows.Forms.Cursors.Default
		Me.labLicense.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labLicense.UseMnemonic = True
		Me.labLicense.Visible = True
		Me.labLicense.AutoSize = False
		Me.labLicense.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labLicense.Name = "labLicense"
		Me.Controls.Add(Picture1)
		Me.Controls.Add(Frame1)
		Me.Controls.Add(labLicense)
		Me.Frame1.Controls.Add(Image1)
		Me.Frame1.Controls.Add(labAuthor)
		Me.Frame1.Controls.Add(labVersion)
		Me.Frame1.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class