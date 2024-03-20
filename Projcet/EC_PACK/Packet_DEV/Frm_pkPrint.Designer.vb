<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Frm_pkPrint
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
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

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意: 以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請不要使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim ReportDataSource2 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.VwRptPickListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.PRODDataSet = New Packet_DEV.PRODDataSet()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.pokey_Text = New System.Windows.Forms.TextBox()
        Me.Btn_Close = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.ReportViewer2 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.Vw_Rpt_PickListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.Vw_Rpt_PickListTableAdapter = New Packet_DEV.PRODDataSetTableAdapters.Vw_Rpt_PickListTableAdapter()
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument()
        CType(Me.VwRptPickListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PRODDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.Vw_Rpt_PickListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'VwRptPickListBindingSource
        '
        Me.VwRptPickListBindingSource.DataMember = "Vw_Rpt_PickList"
        Me.VwRptPickListBindingSource.DataSource = Me.PRODDataSet
        '
        'PRODDataSet
        '
        Me.PRODDataSet.DataSetName = "PRODDataSet"
        Me.PRODDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("微軟正黑體", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label2.Location = New System.Drawing.Point(5, 34)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(105, 24)
        Me.Label2.TabIndex = 101
        Me.Label2.Text = "密盆號碼："
        '
        'pokey_Text
        '
        Me.pokey_Text.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.pokey_Text.Font = New System.Drawing.Font("微軟正黑體", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.pokey_Text.Location = New System.Drawing.Point(116, 29)
        Me.pokey_Text.MaxLength = 20
        Me.pokey_Text.Name = "pokey_Text"
        Me.pokey_Text.Size = New System.Drawing.Size(242, 39)
        Me.pokey_Text.TabIndex = 100
        '
        'Btn_Close
        '
        Me.Btn_Close.Enabled = False
        Me.Btn_Close.Font = New System.Drawing.Font("微軟正黑體", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Btn_Close.Location = New System.Drawing.Point(133, 91)
        Me.Btn_Close.Name = "Btn_Close"
        Me.Btn_Close.Size = New System.Drawing.Size(225, 51)
        Me.Btn_Close.TabIndex = 102
        Me.Btn_Close.Text = "列印揀取單"
        Me.Btn_Close.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Button1.Enabled = False
        Me.Button1.Font = New System.Drawing.Font("微軟正黑體", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Button1.Location = New System.Drawing.Point(12, 100)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(70, 40)
        Me.Button1.TabIndex = 103
        Me.Button1.Text = "離開"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(12, 148)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(568, 209)
        Me.TabControl1.TabIndex = 105
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.ReportViewer2)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(560, 183)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "TabPage1"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'ReportViewer2
        '
        Me.ReportViewer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer2.DocumentMapWidth = 66
        ReportDataSource2.Name = "PRODDataSet_Vw_Rpt_PickList"
        ReportDataSource2.Value = Me.VwRptPickListBindingSource
        Me.ReportViewer2.LocalReport.DataSources.Add(ReportDataSource2)
        Me.ReportViewer2.LocalReport.ReportEmbeddedResource = "Packet_DEV.Rpt_PickList_EC210416.rdlc"
        Me.ReportViewer2.Location = New System.Drawing.Point(3, 3)
        Me.ReportViewer2.Name = "ReportViewer2"
        Me.ReportViewer2.Size = New System.Drawing.Size(554, 177)
        Me.ReportViewer2.TabIndex = 105
        '
        'TabPage2
        '
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(560, 183)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Vw_Rpt_PickListBindingSource
        '
        Me.Vw_Rpt_PickListBindingSource.DataMember = "Vw_Rpt_PickList"
        Me.Vw_Rpt_PickListBindingSource.DataSource = Me.PRODDataSet
        '
        'Vw_Rpt_PickListTableAdapter
        '
        Me.Vw_Rpt_PickListTableAdapter.ClearBeforeFill = True
        '
        'PrintDocument1
        '
        '
        'Frm_pkPrint
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(592, 369)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Btn_Close)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.pokey_Text)
        Me.Name = "Frm_pkPrint"
        Me.Text = "Frm_pkPrint"
        CType(Me.VwRptPickListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PRODDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        CType(Me.Vw_Rpt_PickListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents pokey_Text As System.Windows.Forms.TextBox
    Friend WithEvents Btn_Close As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Vw_Rpt_PickListBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents PRODDataSet As Packet_DEV.PRODDataSet
    Friend WithEvents Vw_Rpt_PickListTableAdapter As Packet_DEV.PRODDataSetTableAdapters.Vw_Rpt_PickListTableAdapter
    Friend WithEvents VwRptPickListBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents ReportViewer2 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PrintDocument1 As System.Drawing.Printing.PrintDocument
End Class
