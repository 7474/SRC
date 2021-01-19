<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmMultiSelectListBox
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
	Public WithEvents cmdResume As System.Windows.Forms.Button
	Public WithEvents cmdSort As System.Windows.Forms.Button
	Public WithEvents cmdSelectAll2 As System.Windows.Forms.Button
	Public WithEvents cmdSelectAll As System.Windows.Forms.Button
	Public WithEvents Timer1 As System.Windows.Forms.Timer
	Public WithEvents cmdFinish As System.Windows.Forms.Button
	Public WithEvents lstItems As System.Windows.Forms.ListBox
	Public WithEvents lblNumber As System.Windows.Forms.Label
	Public WithEvents lblCaption As System.Windows.Forms.Label
	'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
	'Windows フォーム デザイナを使って変更できます。
	'コード エディタを使用して、変更しないでください。
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmMultiSelectListBox))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.cmdResume = New System.Windows.Forms.Button
		Me.cmdSort = New System.Windows.Forms.Button
		Me.cmdSelectAll2 = New System.Windows.Forms.Button
		Me.cmdSelectAll = New System.Windows.Forms.Button
		Me.Timer1 = New System.Windows.Forms.Timer(components)
		Me.cmdFinish = New System.Windows.Forms.Button
		Me.lstItems = New System.Windows.Forms.ListBox
		Me.lblNumber = New System.Windows.Forms.Label
		Me.lblCaption = New System.Windows.Forms.Label
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.Text = "MultiSelectListBox"
		Me.ClientSize = New System.Drawing.Size(497, 331)
		Me.Location = New System.Drawing.Point(60, 165)
		Me.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.Icon = CType(resources.GetObject("frmMultiSelectListBox.Icon"), System.Drawing.Icon)
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
		Me.Name = "frmMultiSelectListBox"
		Me.cmdResume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.cmdResume.BackColor = System.Drawing.SystemColors.Control
		Me.cmdResume.Text = "繝槭ャ繝励ｒ隕九ｋ"
		Me.cmdResume.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.cmdResume.Size = New System.Drawing.Size(137, 29)
		Me.cmdResume.Location = New System.Drawing.Point(96, 296)
		Me.cmdResume.TabIndex = 7
		Me.cmdResume.TabStop = False
		Me.cmdResume.CausesValidation = True
		Me.cmdResume.Enabled = True
		Me.cmdResume.ForeColor = System.Drawing.SystemColors.ControlText
		Me.cmdResume.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdResume.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.cmdResume.Name = "cmdResume"
		Me.cmdSort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.cmdSort.BackColor = System.Drawing.SystemColors.Control
		Me.cmdSort.Text = "Invalid_string_refer_to_original_code"
		Me.cmdSort.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.cmdSort.Size = New System.Drawing.Size(145, 29)
		Me.cmdSort.Location = New System.Drawing.Point(336, 264)
		Me.cmdSort.TabIndex = 6
		Me.cmdSort.TabStop = False
		Me.cmdSort.CausesValidation = True
		Me.cmdSort.Enabled = True
		Me.cmdSort.ForeColor = System.Drawing.SystemColors.ControlText
		Me.cmdSort.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdSort.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.cmdSort.Name = "cmdSort"
		Me.cmdSelectAll2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.cmdSelectAll2.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.cmdSelectAll2.Text = "Invalid_string_refer_to_original_code"
		Me.cmdSelectAll2.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.cmdSelectAll2.Size = New System.Drawing.Size(161, 29)
		Me.cmdSelectAll2.Location = New System.Drawing.Point(168, 264)
		Me.cmdSelectAll2.TabIndex = 5
		Me.cmdSelectAll2.TabStop = False
		Me.cmdSelectAll2.CausesValidation = True
		Me.cmdSelectAll2.Enabled = True
		Me.cmdSelectAll2.ForeColor = System.Drawing.SystemColors.ControlText
		Me.cmdSelectAll2.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdSelectAll2.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.cmdSelectAll2.Name = "cmdSelectAll2"
		Me.cmdSelectAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.cmdSelectAll.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.cmdSelectAll.Text = "Invalid_string_refer_to_original_code"
		Me.cmdSelectAll.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.cmdSelectAll.Size = New System.Drawing.Size(153, 29)
		Me.cmdSelectAll.Location = New System.Drawing.Point(8, 264)
		Me.cmdSelectAll.TabIndex = 4
		Me.cmdSelectAll.TabStop = False
		Me.cmdSelectAll.CausesValidation = True
		Me.cmdSelectAll.Enabled = True
		Me.cmdSelectAll.ForeColor = System.Drawing.SystemColors.ControlText
		Me.cmdSelectAll.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdSelectAll.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.cmdSelectAll.Name = "cmdSelectAll"
		Me.Timer1.Interval = 100
		Me.Timer1.Enabled = True
		Me.cmdFinish.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.cmdFinish.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.cmdFinish.Text = "Invalid_string_refer_to_original_code"
		Me.cmdFinish.Enabled = False
		Me.cmdFinish.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 9.75!, System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.cmdFinish.Size = New System.Drawing.Size(137, 29)
		Me.cmdFinish.Location = New System.Drawing.Point(264, 296)
		Me.cmdFinish.TabIndex = 1
		Me.cmdFinish.TabStop = False
		Me.cmdFinish.CausesValidation = True
		Me.cmdFinish.ForeColor = System.Drawing.SystemColors.ControlText
		Me.cmdFinish.Cursor = System.Windows.Forms.Cursors.Default
		Me.cmdFinish.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.cmdFinish.Name = "cmdFinish"
		Me.lstItems.BackColor = System.Drawing.Color.White
		Me.lstItems.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.lstItems.ForeColor = System.Drawing.Color.Black
		Me.lstItems.Size = New System.Drawing.Size(479, 231)
		Me.lstItems.Location = New System.Drawing.Point(8, 32)
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
		Me.lblNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter
		Me.lblNumber.BackColor = System.Drawing.Color.White
		Me.lblNumber.Text = "Label1"
		Me.lblNumber.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.lblNumber.ForeColor = System.Drawing.Color.Black
		Me.lblNumber.Size = New System.Drawing.Size(57, 29)
		Me.lblNumber.Location = New System.Drawing.Point(424, 296)
		Me.lblNumber.TabIndex = 3
		Me.lblNumber.Enabled = True
		Me.lblNumber.Cursor = System.Windows.Forms.Cursors.Default
		Me.lblNumber.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.lblNumber.UseMnemonic = True
		Me.lblNumber.Visible = True
		Me.lblNumber.AutoSize = False
		Me.lblNumber.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.lblNumber.Name = "lblNumber"
		Me.lblCaption.BackColor = System.Drawing.Color.White
		Me.lblCaption.Text = "Label1"
		Me.lblCaption.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.lblCaption.ForeColor = System.Drawing.Color.Black
		Me.lblCaption.Size = New System.Drawing.Size(479, 22)
		Me.lblCaption.Location = New System.Drawing.Point(8, 6)
		Me.lblCaption.TabIndex = 2
		Me.lblCaption.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.lblCaption.Enabled = True
		Me.lblCaption.Cursor = System.Windows.Forms.Cursors.Default
		Me.lblCaption.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.lblCaption.UseMnemonic = True
		Me.lblCaption.Visible = True
		Me.lblCaption.AutoSize = False
		Me.lblCaption.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.lblCaption.Name = "lblCaption"
		Me.Controls.Add(cmdResume)
		Me.Controls.Add(cmdSort)
		Me.Controls.Add(cmdSelectAll2)
		Me.Controls.Add(cmdSelectAll)
		Me.Controls.Add(cmdFinish)
		Me.Controls.Add(lstItems)
		Me.Controls.Add(lblNumber)
		Me.Controls.Add(lblCaption)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class