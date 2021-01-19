<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmNowLoading
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
	Public WithEvents picBar As System.Windows.Forms.PictureBox
	Public WithEvents Label1 As System.Windows.Forms.Label
	'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
	'Windows フォーム デザイナを使って変更できます。
	'コード エディタを使用して、変更しないでください。
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmNowLoading))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.picBar = New System.Windows.Forms.PictureBox
		Me.Label1 = New System.Windows.Forms.Label
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.Text = "SRC"
		Me.ClientSize = New System.Drawing.Size(214, 88)
		Me.Location = New System.Drawing.Point(76, 107)
		Me.ForeColor = System.Drawing.Color.Black
		Me.Icon = CType(resources.GetObject("frmNowLoading.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ControlBox = True
		Me.Enabled = True
		Me.KeyPreview = False
		Me.MinimizeBox = True
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.ShowInTaskbar = True
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "frmNowLoading"
		Me.picBar.BackColor = System.Drawing.Color.White
		Me.picBar.ForeColor = System.Drawing.Color.FromARGB(0, 0, 128)
		Me.picBar.Size = New System.Drawing.Size(183, 13)
		Me.picBar.Location = New System.Drawing.Point(16, 56)
		Me.picBar.TabIndex = 1
		Me.picBar.Dock = System.Windows.Forms.DockStyle.None
		Me.picBar.CausesValidation = True
		Me.picBar.Enabled = True
		Me.picBar.Cursor = System.Windows.Forms.Cursors.Default
		Me.picBar.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picBar.TabStop = True
		Me.picBar.Visible = True
		Me.picBar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.picBar.Name = "picBar"
		Me.Label1.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.Label1.Text = "Now Loading ..."
		Me.Label1.Font = New System.Drawing.Font("Times New Roman", 18!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.ForeColor = System.Drawing.Color.Black
		Me.Label1.Size = New System.Drawing.Size(169, 33)
		Me.Label1.Location = New System.Drawing.Point(24, 16)
		Me.Label1.TabIndex = 0
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.Label1.Enabled = True
		Me.Label1.Cursor = System.Windows.Forms.Cursors.Default
		Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.Label1.UseMnemonic = True
		Me.Label1.Visible = True
		Me.Label1.AutoSize = False
		Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.Label1.Name = "Label1"
		Me.Controls.Add(picBar)
		Me.Controls.Add(Label1)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class