<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmErrorMessage
#Region "Windows �t�H�[�� �f�U�C�i�ɂ���Đ������ꂽ�R�[�h "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'���̌Ăяo���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
		InitializeComponent()
	End Sub
	'Form �́A�R���|�[�l���g�ꗗ�Ɍ㏈�������s���邽�߂� dispose ���I�[�o�[���C�h���܂��B
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
	Public WithEvents txtMessage As System.Windows.Forms.TextBox
	'����: �ȉ��̃v���V�[�W���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
	'Windows �t�H�[�� �f�U�C�i���g���ĕύX�ł��܂��B
	'�R�[�h �G�f�B�^���g�p���āA�ύX���Ȃ��ł��������B
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmErrorMessage))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.txtMessage = New System.Windows.Forms.TextBox
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.Text = "エラー"
		Me.ClientSize = New System.Drawing.Size(671, 118)
		Me.Location = New System.Drawing.Point(3, 22)
		Me.Icon = CType(resources.GetObject("frmErrorMessage.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.ShowInTaskbar = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ControlBox = True
		Me.Enabled = True
		Me.KeyPreview = False
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "frmErrorMessage"
		Me.txtMessage.AutoSize = False
		Me.txtMessage.BackColor = System.Drawing.Color.White
		Me.txtMessage.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.txtMessage.ForeColor = System.Drawing.Color.Black
		Me.txtMessage.Size = New System.Drawing.Size(659, 106)
		Me.txtMessage.Location = New System.Drawing.Point(6, 6)
		Me.txtMessage.MultiLine = True
		Me.txtMessage.TabIndex = 0
		Me.txtMessage.Text = "Text1" & Chr(13) & Chr(10)
		Me.txtMessage.AcceptsReturn = True
		Me.txtMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
		Me.txtMessage.CausesValidation = True
		Me.txtMessage.Enabled = True
		Me.txtMessage.HideSelection = True
		Me.txtMessage.ReadOnly = False
		Me.txtMessage.Maxlength = 0
		Me.txtMessage.Cursor = System.Windows.Forms.Cursors.IBeam
		Me.txtMessage.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.txtMessage.TabStop = True
		Me.txtMessage.Visible = True
		Me.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.txtMessage.Name = "txtMessage"
		Me.Controls.Add(txtMessage)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class