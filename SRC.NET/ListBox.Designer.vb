<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmListBox
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
	Public WithEvents Timer2 As System.Windows.Forms.Timer
	Public WithEvents txtComment As System.Windows.Forms.TextBox
	Public WithEvents txtMorale2 As System.Windows.Forms.TextBox
	Public WithEvents txtMorale1 As System.Windows.Forms.TextBox
	Public WithEvents txtLevel2 As System.Windows.Forms.TextBox
	Public WithEvents txtLevel1 As System.Windows.Forms.TextBox
	Public WithEvents txtHP1 As System.Windows.Forms.TextBox
	Public WithEvents picHP1 As System.Windows.Forms.PictureBox
	Public WithEvents picEN1 As System.Windows.Forms.PictureBox
	Public WithEvents txtEN1 As System.Windows.Forms.TextBox
	Public WithEvents txtEN2 As System.Windows.Forms.TextBox
	Public WithEvents picEN2 As System.Windows.Forms.PictureBox
	Public WithEvents picHP2 As System.Windows.Forms.PictureBox
	Public WithEvents txtHP2 As System.Windows.Forms.TextBox
	Public WithEvents picUnit2 As System.Windows.Forms.PictureBox
	Public WithEvents picUnit1 As System.Windows.Forms.PictureBox
	Public WithEvents Timer1 As System.Windows.Forms.Timer
	Public WithEvents lstItems As System.Windows.Forms.ListBox
	Public WithEvents picBar As System.Windows.Forms.PictureBox
	Public WithEvents labCaption As System.Windows.Forms.Label
	Public WithEvents labMorale2 As System.Windows.Forms.Label
	Public WithEvents labMorale1 As System.Windows.Forms.Label
	Public WithEvents labLevel2 As System.Windows.Forms.Label
	Public WithEvents imgPilot2 As System.Windows.Forms.PictureBox
	Public WithEvents labLevel1 As System.Windows.Forms.Label
	Public WithEvents imgPilot1 As System.Windows.Forms.PictureBox
	Public WithEvents labHP1 As System.Windows.Forms.Label
	Public WithEvents labEN1 As System.Windows.Forms.Label
	Public WithEvents labEN2 As System.Windows.Forms.Label
	Public WithEvents labHP2 As System.Windows.Forms.Label
	'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
	'Windows フォーム デザイナを使って変更できます。
	'コード エディタを使用して、変更しないでください。
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmListBox))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.Timer2 = New System.Windows.Forms.Timer(components)
		Me.txtComment = New System.Windows.Forms.TextBox
		Me.txtMorale2 = New System.Windows.Forms.TextBox
		Me.txtMorale1 = New System.Windows.Forms.TextBox
		Me.txtLevel2 = New System.Windows.Forms.TextBox
		Me.txtLevel1 = New System.Windows.Forms.TextBox
		Me.txtHP1 = New System.Windows.Forms.TextBox
		Me.picHP1 = New System.Windows.Forms.PictureBox
		Me.picEN1 = New System.Windows.Forms.PictureBox
		Me.txtEN1 = New System.Windows.Forms.TextBox
		Me.txtEN2 = New System.Windows.Forms.TextBox
		Me.picEN2 = New System.Windows.Forms.PictureBox
		Me.picHP2 = New System.Windows.Forms.PictureBox
		Me.txtHP2 = New System.Windows.Forms.TextBox
		Me.picUnit2 = New System.Windows.Forms.PictureBox
		Me.picUnit1 = New System.Windows.Forms.PictureBox
		Me.Timer1 = New System.Windows.Forms.Timer(components)
		Me.lstItems = New System.Windows.Forms.ListBox
		Me.picBar = New System.Windows.Forms.PictureBox
		Me.labCaption = New System.Windows.Forms.Label
		Me.labMorale2 = New System.Windows.Forms.Label
		Me.labMorale1 = New System.Windows.Forms.Label
		Me.labLevel2 = New System.Windows.Forms.Label
		Me.imgPilot2 = New System.Windows.Forms.PictureBox
		Me.labLevel1 = New System.Windows.Forms.Label
		Me.imgPilot1 = New System.Windows.Forms.PictureBox
		Me.labHP1 = New System.Windows.Forms.Label
		Me.labEN1 = New System.Windows.Forms.Label
		Me.labEN2 = New System.Windows.Forms.Label
		Me.labHP2 = New System.Windows.Forms.Label
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Text = "ListBox"
		Me.ClientSize = New System.Drawing.Size(654, 137)
		Me.Location = New System.Drawing.Point(72, 116)
		Me.Icon = CType(resources.GetObject("frmListBox.Icon"), System.Drawing.Icon)
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
		Me.Name = "frmListBox"
		Me.Timer2.Interval = 100
		Me.Timer2.Enabled = True
		Me.txtComment.AutoSize = False
		Me.txtComment.Enabled = False
		Me.txtComment.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtComment.Size = New System.Drawing.Size(637, 38)
		Me.txtComment.Location = New System.Drawing.Point(6, 141)
		Me.txtComment.MultiLine = True
		Me.txtComment.TabIndex = 24
		Me.txtComment.TabStop = False
		Me.txtComment.Visible = False
		Me.txtComment.AcceptsReturn = True
		Me.txtComment.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtComment.BackColor = System.Drawing.SystemColors.Window
		Me.txtComment.CausesValidation = True
		Me.txtComment.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtComment.HideSelection = True
		Me.txtComment.ReadOnly = False
		Me.txtComment.Maxlength = 0
		Me.txtComment.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtComment.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtComment.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtComment.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtComment.Name = "txtComment"
		Me.txtMorale2.AutoSize = False
		Me.txtMorale2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtMorale2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtMorale2.Size = New System.Drawing.Size(25, 13)
		Me.txtMorale2.Location = New System.Drawing.Point(385, 23)
		Me.txtMorale2.TabIndex = 22
		Me.txtMorale2.Text = "100"
		Me.txtMorale2.Visible = False
		Me.txtMorale2.AcceptsReturn = True
		Me.txtMorale2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtMorale2.CausesValidation = True
		Me.txtMorale2.Enabled = True
		Me.txtMorale2.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtMorale2.HideSelection = True
		Me.txtMorale2.ReadOnly = False
		Me.txtMorale2.Maxlength = 0
		Me.txtMorale2.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtMorale2.MultiLine = False
		Me.txtMorale2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtMorale2.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtMorale2.TabStop = True
		Me.txtMorale2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMorale2.Name = "txtMorale2"
		Me.txtMorale1.AutoSize = False
		Me.txtMorale1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtMorale1.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtMorale1.Size = New System.Drawing.Size(25, 13)
		Me.txtMorale1.Location = New System.Drawing.Point(59, 22)
		Me.txtMorale1.TabIndex = 20
		Me.txtMorale1.Text = "100"
		Me.txtMorale1.Visible = False
		Me.txtMorale1.AcceptsReturn = True
		Me.txtMorale1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtMorale1.CausesValidation = True
		Me.txtMorale1.Enabled = True
		Me.txtMorale1.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtMorale1.HideSelection = True
		Me.txtMorale1.ReadOnly = False
		Me.txtMorale1.Maxlength = 0
		Me.txtMorale1.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtMorale1.MultiLine = False
		Me.txtMorale1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtMorale1.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtMorale1.TabStop = True
		Me.txtMorale1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMorale1.Name = "txtMorale1"
		Me.txtLevel2.AutoSize = False
		Me.txtLevel2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtLevel2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtLevel2.Size = New System.Drawing.Size(17, 13)
		Me.txtLevel2.Location = New System.Drawing.Point(391, 7)
		Me.txtLevel2.TabIndex = 19
		Me.txtLevel2.Text = "99"
		Me.txtLevel2.Visible = False
		Me.txtLevel2.AcceptsReturn = True
		Me.txtLevel2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtLevel2.CausesValidation = True
		Me.txtLevel2.Enabled = True
		Me.txtLevel2.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtLevel2.HideSelection = True
		Me.txtLevel2.ReadOnly = False
		Me.txtLevel2.Maxlength = 0
		Me.txtLevel2.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtLevel2.MultiLine = False
		Me.txtLevel2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtLevel2.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtLevel2.TabStop = True
		Me.txtLevel2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtLevel2.Name = "txtLevel2"
		Me.txtLevel1.AutoSize = False
		Me.txtLevel1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtLevel1.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtLevel1.Size = New System.Drawing.Size(17, 13)
		Me.txtLevel1.Location = New System.Drawing.Point(66, 6)
		Me.txtLevel1.TabIndex = 17
		Me.txtLevel1.Text = "99"
		Me.txtLevel1.Visible = False
		Me.txtLevel1.AcceptsReturn = True
		Me.txtLevel1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtLevel1.CausesValidation = True
		Me.txtLevel1.Enabled = True
		Me.txtLevel1.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtLevel1.HideSelection = True
		Me.txtLevel1.ReadOnly = False
		Me.txtLevel1.Maxlength = 0
		Me.txtLevel1.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtLevel1.MultiLine = False
		Me.txtLevel1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtLevel1.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtLevel1.TabStop = True
		Me.txtLevel1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtLevel1.Name = "txtLevel1"
		Me.txtHP1.AutoSize = False
		Me.txtHP1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtHP1.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtHP1.Size = New System.Drawing.Size(88, 13)
		Me.txtHP1.Location = New System.Drawing.Point(148, 8)
		Me.txtHP1.TabIndex = 11
		Me.txtHP1.Text = "99999/99999"
		Me.txtHP1.Visible = False
		Me.txtHP1.AcceptsReturn = True
		Me.txtHP1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtHP1.CausesValidation = True
		Me.txtHP1.Enabled = True
		Me.txtHP1.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtHP1.HideSelection = True
		Me.txtHP1.ReadOnly = False
		Me.txtHP1.Maxlength = 0
		Me.txtHP1.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtHP1.MultiLine = False
		Me.txtHP1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtHP1.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtHP1.TabStop = True
		Me.txtHP1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtHP1.Name = "txtHP1"
		Me.picHP1.BackColor = System.Drawing.Color.FromARGB(192, 0, 0)
		Me.picHP1.ForeColor = System.Drawing.Color.FromARGB(0, 192, 0)
		Me.picHP1.Size = New System.Drawing.Size(114, 8)
		Me.picHP1.Location = New System.Drawing.Point(122, 26)
		Me.picHP1.TabIndex = 10
		Me.picHP1.Visible = False
		Me.picHP1.Dock = System.Windows.Forms.DockStyle.None
		Me.picHP1.CausesValidation = True
		Me.picHP1.Enabled = True
		Me.picHP1.Cursor = System.Windows.Forms.Cursors.Default
		Me.picHP1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picHP1.TabStop = True
		Me.picHP1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picHP1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picHP1.Name = "picHP1"
		Me.picEN1.BackColor = System.Drawing.Color.FromARGB(192, 0, 0)
		Me.picEN1.ForeColor = System.Drawing.Color.FromARGB(0, 192, 0)
		Me.picEN1.Size = New System.Drawing.Size(79, 8)
		Me.picEN1.Location = New System.Drawing.Point(240, 26)
		Me.picEN1.TabIndex = 9
		Me.picEN1.Visible = False
		Me.picEN1.Dock = System.Windows.Forms.DockStyle.None
		Me.picEN1.CausesValidation = True
		Me.picEN1.Enabled = True
		Me.picEN1.Cursor = System.Windows.Forms.Cursors.Default
		Me.picEN1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picEN1.TabStop = True
		Me.picEN1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picEN1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picEN1.Name = "picEN1"
		Me.txtEN1.AutoSize = False
		Me.txtEN1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtEN1.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtEN1.Size = New System.Drawing.Size(57, 13)
		Me.txtEN1.Location = New System.Drawing.Point(263, 8)
		Me.txtEN1.TabIndex = 8
		Me.txtEN1.Text = "999/999"
		Me.txtEN1.Visible = False
		Me.txtEN1.AcceptsReturn = True
		Me.txtEN1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtEN1.CausesValidation = True
		Me.txtEN1.Enabled = True
		Me.txtEN1.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtEN1.HideSelection = True
		Me.txtEN1.ReadOnly = False
		Me.txtEN1.Maxlength = 0
		Me.txtEN1.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtEN1.MultiLine = False
		Me.txtEN1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtEN1.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtEN1.TabStop = True
		Me.txtEN1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtEN1.Name = "txtEN1"
		Me.txtEN2.AutoSize = False
		Me.txtEN2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtEN2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtEN2.Size = New System.Drawing.Size(57, 13)
		Me.txtEN2.Location = New System.Drawing.Point(587, 8)
		Me.txtEN2.TabIndex = 7
		Me.txtEN2.Text = "999/999"
		Me.txtEN2.Visible = False
		Me.txtEN2.AcceptsReturn = True
		Me.txtEN2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtEN2.CausesValidation = True
		Me.txtEN2.Enabled = True
		Me.txtEN2.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtEN2.HideSelection = True
		Me.txtEN2.ReadOnly = False
		Me.txtEN2.Maxlength = 0
		Me.txtEN2.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtEN2.MultiLine = False
		Me.txtEN2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtEN2.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtEN2.TabStop = True
		Me.txtEN2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtEN2.Name = "txtEN2"
		Me.picEN2.BackColor = System.Drawing.Color.FromARGB(192, 0, 0)
		Me.picEN2.ForeColor = System.Drawing.Color.FromARGB(0, 192, 0)
		Me.picEN2.Size = New System.Drawing.Size(78, 8)
		Me.picEN2.Location = New System.Drawing.Point(565, 27)
		Me.picEN2.TabIndex = 6
		Me.picEN2.Visible = False
		Me.picEN2.Dock = System.Windows.Forms.DockStyle.None
		Me.picEN2.CausesValidation = True
		Me.picEN2.Enabled = True
		Me.picEN2.Cursor = System.Windows.Forms.Cursors.Default
		Me.picEN2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picEN2.TabStop = True
		Me.picEN2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picEN2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picEN2.Name = "picEN2"
		Me.picHP2.BackColor = System.Drawing.Color.FromARGB(192, 0, 0)
		Me.picHP2.ForeColor = System.Drawing.Color.FromARGB(0, 192, 0)
		Me.picHP2.Size = New System.Drawing.Size(112, 8)
		Me.picHP2.Location = New System.Drawing.Point(449, 27)
		Me.picHP2.TabIndex = 5
		Me.picHP2.Visible = False
		Me.picHP2.Dock = System.Windows.Forms.DockStyle.None
		Me.picHP2.CausesValidation = True
		Me.picHP2.Enabled = True
		Me.picHP2.Cursor = System.Windows.Forms.Cursors.Default
		Me.picHP2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picHP2.TabStop = True
		Me.picHP2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picHP2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picHP2.Name = "picHP2"
		Me.txtHP2.AutoSize = False
		Me.txtHP2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.txtHP2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtHP2.Size = New System.Drawing.Size(88, 13)
		Me.txtHP2.Location = New System.Drawing.Point(473, 8)
		Me.txtHP2.TabIndex = 4
		Me.txtHP2.Text = "99999/99999"
		Me.txtHP2.Visible = False
		Me.txtHP2.AcceptsReturn = True
		Me.txtHP2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtHP2.CausesValidation = True
		Me.txtHP2.Enabled = True
		Me.txtHP2.ForeColor = System.Drawing.SystemColors.WindowText
		Me.txtHP2.HideSelection = True
		Me.txtHP2.ReadOnly = False
		Me.txtHP2.Maxlength = 0
		Me.txtHP2.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtHP2.MultiLine = False
		Me.txtHP2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtHP2.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtHP2.TabStop = True
		Me.txtHP2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtHP2.Name = "txtHP2"
		Me.picUnit2.BackColor = System.Drawing.SystemColors.Window
		Me.picUnit2.ForeColor = System.Drawing.SystemColors.WindowText
		Me.picUnit2.Size = New System.Drawing.Size(32, 32)
		Me.picUnit2.Location = New System.Drawing.Point(412, 5)
		Me.picUnit2.TabIndex = 3
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
		Me.picUnit1.BackColor = System.Drawing.SystemColors.Window
		Me.picUnit1.ForeColor = System.Drawing.SystemColors.WindowText
		Me.picUnit1.Size = New System.Drawing.Size(32, 32)
		Me.picUnit1.Location = New System.Drawing.Point(85, 4)
		Me.picUnit1.TabIndex = 2
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
		Me.Timer1.Enabled = False
		Me.Timer1.Interval = 100
		Me.lstItems.BackColor = System.Drawing.Color.White
		Me.lstItems.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.lstItems.ForeColor = System.Drawing.Color.Black
		Me.lstItems.Size = New System.Drawing.Size(643, 103)
		Me.lstItems.Location = New System.Drawing.Point(6, 32)
		Me.lstItems.TabIndex = 0
		Me.lstItems.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.lstItems.CausesValidation = True
		Me.lstItems.Enabled = True
		Me.lstItems.IntegralHeight = True
		Me.lstItems.Cursor = System.Windows.Forms.Cursors.Default
		Me.lstItems.SelectionMode = System.Windows.Forms.SelectionMode.One
		Me.lstItems.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.lstItems.Sorted = False
		Me.lstItems.TabStop = True
		Me.lstItems.Visible = True
		Me.lstItems.MultiColumn = False
		Me.lstItems.Name = "lstItems"
		Me.picBar.BackColor = System.Drawing.Color.White
		Me.picBar.ForeColor = System.Drawing.Color.FromARGB(0, 0, 128)
		Me.picBar.Size = New System.Drawing.Size(643, 13)
		Me.picBar.Location = New System.Drawing.Point(6, 123)
		Me.picBar.TabIndex = 25
		Me.picBar.Visible = False
		Me.picBar.Dock = System.Windows.Forms.DockStyle.None
		Me.picBar.CausesValidation = True
		Me.picBar.Enabled = True
		Me.picBar.Cursor = System.Windows.Forms.Cursors.Default
		Me.picBar.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picBar.TabStop = True
		Me.picBar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picBar.Name = "picBar"
		Me.labCaption.BackColor = System.Drawing.Color.White
		Me.labCaption.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labCaption.ForeColor = System.Drawing.Color.Black
		Me.labCaption.Size = New System.Drawing.Size(643, 23)
		Me.labCaption.Location = New System.Drawing.Point(6, 5)
		Me.labCaption.TabIndex = 1
		Me.labCaption.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labCaption.Enabled = True
		Me.labCaption.Cursor = System.Windows.Forms.Cursors.Default
		Me.labCaption.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labCaption.UseMnemonic = True
		Me.labCaption.Visible = True
		Me.labCaption.AutoSize = False
		Me.labCaption.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.labCaption.Name = "labCaption"
		Me.labMorale2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labMorale2.Text = "M"
		Me.labMorale2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 11.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labMorale2.ForeColor = System.Drawing.Color.Black
		Me.labMorale2.Size = New System.Drawing.Size(12, 17)
		Me.labMorale2.Location = New System.Drawing.Point(372, 22)
		Me.labMorale2.TabIndex = 23
		Me.labMorale2.Visible = False
		Me.labMorale2.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labMorale2.Enabled = True
		Me.labMorale2.Cursor = System.Windows.Forms.Cursors.Default
		Me.labMorale2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labMorale2.UseMnemonic = True
		Me.labMorale2.AutoSize = False
		Me.labMorale2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labMorale2.Name = "labMorale2"
		Me.labMorale1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labMorale1.Text = "M"
		Me.labMorale1.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 11.25!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labMorale1.ForeColor = System.Drawing.Color.Black
		Me.labMorale1.Size = New System.Drawing.Size(12, 17)
		Me.labMorale1.Location = New System.Drawing.Point(46, 20)
		Me.labMorale1.TabIndex = 21
		Me.labMorale1.Visible = False
		Me.labMorale1.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labMorale1.Enabled = True
		Me.labMorale1.Cursor = System.Windows.Forms.Cursors.Default
		Me.labMorale1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labMorale1.UseMnemonic = True
		Me.labMorale1.AutoSize = False
		Me.labMorale1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labMorale1.Name = "labMorale1"
		Me.labLevel2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labLevel2.Text = "Lv"
		Me.labLevel2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labLevel2.ForeColor = System.Drawing.Color.Black
		Me.labLevel2.Size = New System.Drawing.Size(20, 17)
		Me.labLevel2.Location = New System.Drawing.Point(370, 4)
		Me.labLevel2.TabIndex = 18
		Me.labLevel2.Visible = False
		Me.labLevel2.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labLevel2.Enabled = True
		Me.labLevel2.Cursor = System.Windows.Forms.Cursors.Default
		Me.labLevel2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labLevel2.UseMnemonic = True
		Me.labLevel2.AutoSize = False
		Me.labLevel2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labLevel2.Name = "labLevel2"
		Me.imgPilot2.Size = New System.Drawing.Size(36, 36)
		Me.imgPilot2.Location = New System.Drawing.Point(331, 3)
		Me.imgPilot2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.imgPilot2.Visible = False
		Me.imgPilot2.Enabled = True
		Me.imgPilot2.Cursor = System.Windows.Forms.Cursors.Default
		Me.imgPilot2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.imgPilot2.Name = "imgPilot2"
		Me.labLevel1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labLevel1.Text = "Lv"
		Me.labLevel1.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labLevel1.ForeColor = System.Drawing.Color.Black
		Me.labLevel1.Size = New System.Drawing.Size(20, 17)
		Me.labLevel1.Location = New System.Drawing.Point(45, 4)
		Me.labLevel1.TabIndex = 16
		Me.labLevel1.Visible = False
		Me.labLevel1.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labLevel1.Enabled = True
		Me.labLevel1.Cursor = System.Windows.Forms.Cursors.Default
		Me.labLevel1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labLevel1.UseMnemonic = True
		Me.labLevel1.AutoSize = False
		Me.labLevel1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labLevel1.Name = "labLevel1"
		Me.imgPilot1.Size = New System.Drawing.Size(36, 36)
		Me.imgPilot1.Location = New System.Drawing.Point(6, 3)
		Me.imgPilot1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.imgPilot1.Visible = False
		Me.imgPilot1.Enabled = True
		Me.imgPilot1.Cursor = System.Windows.Forms.Cursors.Default
		Me.imgPilot1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.imgPilot1.Name = "imgPilot1"
		Me.labHP1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labHP1.Text = "HP"
		Me.labHP1.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labHP1.ForeColor = System.Drawing.Color.Black
		Me.labHP1.Size = New System.Drawing.Size(23, 17)
		Me.labHP1.Location = New System.Drawing.Point(121, 6)
		Me.labHP1.TabIndex = 15
		Me.labHP1.Visible = False
		Me.labHP1.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labHP1.Enabled = True
		Me.labHP1.Cursor = System.Windows.Forms.Cursors.Default
		Me.labHP1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labHP1.UseMnemonic = True
		Me.labHP1.AutoSize = False
		Me.labHP1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labHP1.Name = "labHP1"
		Me.labEN1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labEN1.Text = "EN"
		Me.labEN1.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labEN1.ForeColor = System.Drawing.Color.Black
		Me.labEN1.Size = New System.Drawing.Size(22, 17)
		Me.labEN1.Location = New System.Drawing.Point(238, 6)
		Me.labEN1.TabIndex = 14
		Me.labEN1.Visible = False
		Me.labEN1.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labEN1.Enabled = True
		Me.labEN1.Cursor = System.Windows.Forms.Cursors.Default
		Me.labEN1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labEN1.UseMnemonic = True
		Me.labEN1.AutoSize = False
		Me.labEN1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labEN1.Name = "labEN1"
		Me.labEN2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labEN2.Text = "EN"
		Me.labEN2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labEN2.ForeColor = System.Drawing.Color.Black
		Me.labEN2.Size = New System.Drawing.Size(25, 17)
		Me.labEN2.Location = New System.Drawing.Point(563, 6)
		Me.labEN2.TabIndex = 13
		Me.labEN2.Visible = False
		Me.labEN2.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labEN2.Enabled = True
		Me.labEN2.Cursor = System.Windows.Forms.Cursors.Default
		Me.labEN2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labEN2.UseMnemonic = True
		Me.labEN2.AutoSize = False
		Me.labEN2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labEN2.Name = "labEN2"
		Me.labHP2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.labHP2.Text = "HP"
		Me.labHP2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labHP2.ForeColor = System.Drawing.Color.Black
		Me.labHP2.Size = New System.Drawing.Size(25, 17)
		Me.labHP2.Location = New System.Drawing.Point(448, 6)
		Me.labHP2.TabIndex = 12
		Me.labHP2.Visible = False
		Me.labHP2.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.labHP2.Enabled = True
		Me.labHP2.Cursor = System.Windows.Forms.Cursors.Default
		Me.labHP2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.labHP2.UseMnemonic = True
		Me.labHP2.AutoSize = False
		Me.labHP2.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.labHP2.Name = "labHP2"
		Me.Controls.Add(txtComment)
		Me.Controls.Add(txtMorale2)
		Me.Controls.Add(txtMorale1)
		Me.Controls.Add(txtLevel2)
		Me.Controls.Add(txtLevel1)
		Me.Controls.Add(txtHP1)
		Me.Controls.Add(picHP1)
		Me.Controls.Add(picEN1)
		Me.Controls.Add(txtEN1)
		Me.Controls.Add(txtEN2)
		Me.Controls.Add(picEN2)
		Me.Controls.Add(picHP2)
		Me.Controls.Add(txtHP2)
		Me.Controls.Add(picUnit2)
		Me.Controls.Add(picUnit1)
		Me.Controls.Add(lstItems)
		Me.Controls.Add(picBar)
		Me.Controls.Add(labCaption)
		Me.Controls.Add(labMorale2)
		Me.Controls.Add(labMorale1)
		Me.Controls.Add(labLevel2)
		Me.Controls.Add(imgPilot2)
		Me.Controls.Add(labLevel1)
		Me.Controls.Add(imgPilot1)
		Me.Controls.Add(labHP1)
		Me.Controls.Add(labEN1)
		Me.Controls.Add(labEN2)
		Me.Controls.Add(labHP2)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class