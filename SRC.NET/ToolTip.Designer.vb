<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmToolTip
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
	Public WithEvents picMessage As System.Windows.Forms.PictureBox
	'����: �ȉ��̃v���V�[�W���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
	'Windows �t�H�[�� �f�U�C�i���g���ĕύX�ł��܂��B
	'�R�[�h �G�f�B�^���g�p���āA�ύX���Ȃ��ł��������B
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmToolTip))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.picMessage = New System.Windows.Forms.PictureBox
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
		Me.ClientSize = New System.Drawing.Size(231, 114)
		Me.Location = New System.Drawing.Point(0, 0)
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.ShowInTaskbar = False
		Me.Visible = False
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.ControlBox = True
		Me.Enabled = True
		Me.KeyPreview = False
		Me.Cursor = System.Windows.Forms.Cursors.Default
		Me.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.HelpButton = False
		Me.WindowState = System.Windows.Forms.FormWindowState.Normal
		Me.Name = "frmToolTip"
		Me.picMessage.BackColor = System.Drawing.SystemColors.Info
		Me.picMessage.ForeColor = System.Drawing.Color.Black
		Me.picMessage.Size = New System.Drawing.Size(193, 104)
		Me.picMessage.Location = New System.Drawing.Point(0, 0)
		Me.picMessage.TabIndex = 0
		Me.picMessage.Dock = System.Windows.Forms.DockStyle.None
		Me.picMessage.CausesValidation = True
		Me.picMessage.Enabled = True
		Me.picMessage.Cursor = System.Windows.Forms.Cursors.Default
		Me.picMessage.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.picMessage.TabStop = True
		Me.picMessage.Visible = True
		Me.picMessage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal
		Me.picMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.picMessage.Name = "picMessage"
		Me.Controls.Add(picMessage)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class