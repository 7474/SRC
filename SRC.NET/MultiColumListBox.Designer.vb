<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmMultiColumnListBox
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
	Public WithEvents lstItems As System.Windows.Forms.ListBox
	Public WithEvents labCaption As System.Windows.Forms.Label
	'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
	'Windows フォーム デザイナを使って変更できます。
	'コード エディタを使用して、変更しないでください。
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmMultiColumnListBox))
		Me.components = New System.ComponentModel.Container()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(components)
		Me.lstItems = New System.Windows.Forms.ListBox
		Me.labCaption = New System.Windows.Forms.Label
		Me.SuspendLayout()
		Me.ToolTip1.Active = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.BackColor = System.Drawing.Color.FromARGB(192, 192, 192)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Text = "MultiColumListBox"
		Me.ClientSize = New System.Drawing.Size(670, 446)
		Me.Location = New System.Drawing.Point(72, 116)
		Me.Icon = CType(resources.GetObject("frmMultiColumnListBox.Icon"), System.Drawing.Icon)
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
		Me.Name = "frmMultiColumnListBox"
		Me.lstItems.BackColor = System.Drawing.Color.White
		Me.lstItems.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.lstItems.ForeColor = System.Drawing.Color.Black
		Me.lstItems.Size = New System.Drawing.Size(654, 407)
		Me.lstItems.Location = New System.Drawing.Point(8, 8)
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
		Me.lstItems.MultiColumn = True
		Me.lstItems.ColumnWidth = 164
		Me.lstItems.Name = "lstItems"
		Me.labCaption.BackColor = System.Drawing.Color.White
		Me.labCaption.Font = New System.Drawing.Font("Invalid_string_refer_to_original_code", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
		Me.labCaption.ForeColor = System.Drawing.Color.Black
		Me.labCaption.Size = New System.Drawing.Size(654, 23)
		Me.labCaption.Location = New System.Drawing.Point(8, 416)
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
		Me.Controls.Add(lstItems)
		Me.Controls.Add(labCaption)
		Me.ResumeLayout(False)
		Me.PerformLayout()
	End Sub
#End Region 
End Class