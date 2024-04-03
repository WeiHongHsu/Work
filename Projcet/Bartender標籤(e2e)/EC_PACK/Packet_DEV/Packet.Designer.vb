<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Packet
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource2 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource3 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim ReportDataSource4 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Packet))
        Me.Vw_Case_Addr_InvoiceBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.PRODDataSet = New Packet_DEV.PRODDataSet()
        Me.AGVPickCarListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.FWKDataSet = New Packet_DEV.FWKDataSet()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.CheckBox2 = New System.Windows.Forms.CheckBox()
        Me.AGV_Txt = New System.Windows.Forms.TextBox()
        Me.poInfo_lb = New System.Windows.Forms.Label()
        Me.rePack_Btn = New System.Windows.Forms.Button()
        Me.LAB_CASEID = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Btn_Close_test = New System.Windows.Forms.Button()
        Me.Btn_Close = New System.Windows.Forms.Button()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.Txt_Pcs = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Txt_SKU = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.pokey_Text = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RadioButton3 = New System.Windows.Forms.RadioButton()
        Me.RadioButton1 = New System.Windows.Forms.RadioButton()
        Me.RadioButton2 = New System.Windows.Forms.RadioButton()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TabControl2 = New System.Windows.Forms.TabControl()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.DGV_PickDetail = New System.Windows.Forms.DataGridView()
        Me.STORERKEYDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ROUTEDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ORDERKEYDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ORDERLINENUMBERDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SKUDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PICKQTYDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PACKQTYDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DESCRDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BUSR3DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BUSR2DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CASEIDDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTYDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.LOCDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.STATUSDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PARTDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PrintMarkDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.WAVEKEYDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EDITDATEDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PICKDETAILKEYDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NOTESDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EXTERNORDERKEYDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.VwPICKDETAILRateBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.DGV_CaseDetail = New System.Windows.Forms.DataGridView()
        Me.ItemNoDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.StorerkeyDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ROUTEDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OrderkeyDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OrderkeyLineNumberDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DescrDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SkuDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DOORDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.LocDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.QTYDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GUINODataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NOTESDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ExternorderkeyDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.WAVEKEYDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EDITDATEDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EDITWHODataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PICKDETAILKEYDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CURCYDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TAX01DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PRICEDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.UNITPRICEDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Recovery = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.CombinationCaseDetailBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TabPage7 = New System.Windows.Forms.TabPage()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.TabPage8 = New System.Windows.Forms.TabPage()
        Me.ReportViewer2 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.ReportViewer3 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.TabPage9 = New System.Windows.Forms.TabPage()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.repokey_Text = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.SplitContainer5 = New System.Windows.Forms.SplitContainer()
        Me.DGV_reAddr = New System.Windows.Forms.DataGridView()
        Me.addrPrintBtn = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.SplitContainer6 = New System.Windows.Forms.SplitContainer()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.DGV_reInvoice = New System.Windows.Forms.DataGridView()
        Me.invicePrintBtn = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.DGV_WAVE = New System.Windows.Forms.DataGridView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.wavekey_Text = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.UploadCase_txt = New System.Windows.Forms.TextBox()
        Me.UploadPo_txt = New System.Windows.Forms.TextBox()
        Me.TabPage10 = New System.Windows.Forms.TabPage()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.ComboBox2 = New System.Windows.Forms.ComboBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.TabPage11 = New System.Windows.Forms.TabPage()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.ReportViewer4 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.AGVBOX_J = New System.Windows.Forms.Label()
        Me.AGVBOX_I = New System.Windows.Forms.Label()
        Me.AGVBOX_L = New System.Windows.Forms.Label()
        Me.AGVBOX_K = New System.Windows.Forms.Label()
        Me.AGVBOX_A = New System.Windows.Forms.Label()
        Me.AGVBOX_B = New System.Windows.Forms.Label()
        Me.AGVBOX_C = New System.Windows.Forms.Label()
        Me.AGVBOX_D = New System.Windows.Forms.Label()
        Me.AGVBOX_E = New System.Windows.Forms.Label()
        Me.AGVBOX_F = New System.Windows.Forms.Label()
        Me.AGVBOX_H = New System.Windows.Forms.Label()
        Me.AGVBOX_G = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.AGVBoxNo = New System.Windows.Forms.TextBox()
        Me.AGVCarClose = New System.Windows.Forms.Button()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.CarLocNo = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.CarNo = New System.Windows.Forms.TextBox()
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument()
        Me.PrintDocument2 = New System.Drawing.Printing.PrintDocument()
        Me.CASEDETAILBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CASEDETAILTableAdapter = New Packet_DEV.PRODDataSetTableAdapters.CASEDETAILTableAdapter()
        Me.Vw_PICKDETAIL_RateTableAdapter = New Packet_DEV.PRODDataSetTableAdapters.Vw_PICKDETAIL_RateTableAdapter()
        Me.Vw_Case_Addr_InvoiceTableAdapter = New Packet_DEV.PRODDataSetTableAdapters.Vw_Case_Addr_InvoiceTableAdapter()
        Me.Combination_CaseDetailTableAdapter = New Packet_DEV.PRODDataSetTableAdapters.Combination_CaseDetailTableAdapter()
        Me.AGV_PickCarListTableAdapter = New Packet_DEV.FWKDataSetTableAdapters.AGV_PickCarListTableAdapter()
        CType(Me.Vw_Case_Addr_InvoiceBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PRODDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AGVPickCarListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.FWKDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TabControl2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        CType(Me.DGV_PickDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.VwPICKDETAILRateBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage4.SuspendLayout()
        CType(Me.DGV_CaseDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CombinationCaseDetailBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage7.SuspendLayout()
        Me.TabPage8.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.TabPage9.SuspendLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer4.Panel1.SuspendLayout()
        Me.SplitContainer4.Panel2.SuspendLayout()
        Me.SplitContainer4.SuspendLayout()
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer5.Panel1.SuspendLayout()
        Me.SplitContainer5.Panel2.SuspendLayout()
        Me.SplitContainer5.SuspendLayout()
        CType(Me.DGV_reAddr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer6.Panel1.SuspendLayout()
        Me.SplitContainer6.Panel2.SuspendLayout()
        Me.SplitContainer6.SuspendLayout()
        CType(Me.DGV_reInvoice, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage2.SuspendLayout()
        CType(Me.DGV_WAVE, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.TabPage10.SuspendLayout()
        Me.TabPage11.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.CASEDETAILBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Vw_Case_Addr_InvoiceBindingSource
        '
        Me.Vw_Case_Addr_InvoiceBindingSource.DataMember = "Vw_Case_Addr_Invoice"
        Me.Vw_Case_Addr_InvoiceBindingSource.DataSource = Me.PRODDataSet
        '
        'PRODDataSet
        '
        Me.PRODDataSet.DataSetName = "PRODDataSet"
        Me.PRODDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'AGVPickCarListBindingSource
        '
        Me.AGVPickCarListBindingSource.DataMember = "AGV_PickCarList"
        Me.AGVPickCarListBindingSource.DataSource = Me.FWKDataSet
        '
        'FWKDataSet
        '
        Me.FWKDataSet.DataSetName = "FWKDataSet"
        Me.FWKDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage9)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage10)
        Me.TabControl1.Controls.Add(Me.TabPage11)
        Me.TabControl1.Font = New System.Drawing.Font("Microsoft JhengHei", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.TabControl1.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1374, 865)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.SplitContainer1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 34)
        Me.TabPage1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage1.Size = New System.Drawing.Size(1366, 827)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "包裝作業"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(4, 5)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.GroupBox2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.GroupBox1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(1358, 817)
        Me.SplitContainer1.SplitterDistance = 275
        Me.SplitContainer1.SplitterWidth = 8
        Me.SplitContainer1.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.FloralWhite
        Me.GroupBox2.Controls.Add(Me.CheckBox2)
        Me.GroupBox2.Controls.Add(Me.AGV_Txt)
        Me.GroupBox2.Controls.Add(Me.poInfo_lb)
        Me.GroupBox2.Controls.Add(Me.rePack_Btn)
        Me.GroupBox2.Controls.Add(Me.LAB_CASEID)
        Me.GroupBox2.Controls.Add(Me.Label14)
        Me.GroupBox2.Controls.Add(Me.Btn_Close_test)
        Me.GroupBox2.Controls.Add(Me.Btn_Close)
        Me.GroupBox2.Controls.Add(Me.CheckBox1)
        Me.GroupBox2.Controls.Add(Me.Txt_Pcs)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.Txt_SKU)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.pokey_Text)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft JhengHei", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(0, 77)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox2.Size = New System.Drawing.Size(1358, 198)
        Me.GroupBox2.TabIndex = 3
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "作業資訊"
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.Location = New System.Drawing.Point(988, 54)
        Me.CheckBox2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(184, 35)
        Me.CheckBox2.TabIndex = 112
        Me.CheckBox2.Text = "重量較驗模式"
        Me.CheckBox2.UseVisualStyleBackColor = True
        Me.CheckBox2.Visible = False
        '
        'AGV_Txt
        '
        Me.AGV_Txt.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.AGV_Txt.Font = New System.Drawing.Font("Microsoft JhengHei", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGV_Txt.Location = New System.Drawing.Point(476, 42)
        Me.AGV_Txt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.AGV_Txt.MaxLength = 20
        Me.AGV_Txt.Name = "AGV_Txt"
        Me.AGV_Txt.Size = New System.Drawing.Size(222, 45)
        Me.AGV_Txt.TabIndex = 111
        '
        'poInfo_lb
        '
        Me.poInfo_lb.AutoSize = True
        Me.poInfo_lb.Font = New System.Drawing.Font("Microsoft JhengHei", 32.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.poInfo_lb.ForeColor = System.Drawing.Color.Red
        Me.poInfo_lb.Location = New System.Drawing.Point(608, 108)
        Me.poInfo_lb.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.poInfo_lb.Name = "poInfo_lb"
        Me.poInfo_lb.Size = New System.Drawing.Size(149, 82)
        Me.poInfo_lb.TabIndex = 110
        Me.poInfo_lb.Text = "123"
        '
        'rePack_Btn
        '
        Me.rePack_Btn.Location = New System.Drawing.Point(794, 215)
        Me.rePack_Btn.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.rePack_Btn.Name = "rePack_Btn"
        Me.rePack_Btn.Size = New System.Drawing.Size(146, 49)
        Me.rePack_Btn.TabIndex = 109
        Me.rePack_Btn.Text = "重新打包"
        Me.rePack_Btn.UseVisualStyleBackColor = True
        '
        'LAB_CASEID
        '
        Me.LAB_CASEID.AutoSize = True
        Me.LAB_CASEID.Location = New System.Drawing.Point(162, 168)
        Me.LAB_CASEID.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LAB_CASEID.Name = "LAB_CASEID"
        Me.LAB_CASEID.Size = New System.Drawing.Size(0, 31)
        Me.LAB_CASEID.TabIndex = 108
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(63, 168)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(86, 31)
        Me.Label14.TabIndex = 107
        Me.Label14.Text = "箱號："
        '
        'Btn_Close_test
        '
        Me.Btn_Close_test.Location = New System.Drawing.Point(794, 57)
        Me.Btn_Close_test.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Btn_Close_test.Name = "Btn_Close_test"
        Me.Btn_Close_test.Size = New System.Drawing.Size(146, 49)
        Me.Btn_Close_test.TabIndex = 6
        Me.Btn_Close_test.Text = "單據結束"
        Me.Btn_Close_test.UseVisualStyleBackColor = True
        '
        'Btn_Close
        '
        Me.Btn_Close.Enabled = False
        Me.Btn_Close.Location = New System.Drawing.Point(609, 215)
        Me.Btn_Close.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Btn_Close.Name = "Btn_Close"
        Me.Btn_Close.Size = New System.Drawing.Size(146, 49)
        Me.Btn_Close.TabIndex = 6
        Me.Btn_Close.Text = "封箱"
        Me.Btn_Close.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.BackColor = System.Drawing.Color.Yellow
        Me.CheckBox1.Checked = True
        Me.CheckBox1.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox1.Location = New System.Drawing.Point(405, 218)
        Me.CheckBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(88, 35)
        Me.CheckBox1.TabIndex = 98
        Me.CheckBox1.Text = "鎖定"
        Me.CheckBox1.UseVisualStyleBackColor = False
        '
        'Txt_Pcs
        '
        Me.Txt_Pcs.Location = New System.Drawing.Point(166, 215)
        Me.Txt_Pcs.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Txt_Pcs.MaxLength = 4
        Me.Txt_Pcs.Name = "Txt_Pcs"
        Me.Txt_Pcs.Size = New System.Drawing.Size(228, 39)
        Me.Txt_Pcs.TabIndex = 5
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft JhengHei", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label13.Location = New System.Drawing.Point(15, 118)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(134, 31)
        Me.Label13.TabIndex = 99
        Me.Label13.Text = "商品條碼："
        '
        'Txt_SKU
        '
        Me.Txt_SKU.Font = New System.Drawing.Font("Microsoft JhengHei", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Txt_SKU.Location = New System.Drawing.Point(166, 112)
        Me.Txt_SKU.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Txt_SKU.MaxLength = 20
        Me.Txt_SKU.Name = "Txt_SKU"
        Me.Txt_SKU.Size = New System.Drawing.Size(288, 39)
        Me.Txt_SKU.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(15, 57)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(134, 31)
        Me.Label2.TabIndex = 99
        Me.Label2.Text = "單據號碼："
        '
        'pokey_Text
        '
        Me.pokey_Text.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.pokey_Text.Enabled = False
        Me.pokey_Text.Font = New System.Drawing.Font("Microsoft JhengHei", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.pokey_Text.Location = New System.Drawing.Point(166, 42)
        Me.pokey_Text.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pokey_Text.MaxLength = 20
        Me.pokey_Text.Name = "pokey_Text"
        Me.pokey_Text.Size = New System.Drawing.Size(288, 45)
        Me.pokey_Text.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(15, 220)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(134, 31)
        Me.Label3.TabIndex = 99
        Me.Label3.Text = "數量鍵入："
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FloralWhite
        Me.GroupBox1.Controls.Add(Me.RadioButton3)
        Me.GroupBox1.Controls.Add(Me.RadioButton1)
        Me.GroupBox1.Controls.Add(Me.RadioButton2)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Size = New System.Drawing.Size(1358, 77)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "作業模式"
        '
        'RadioButton3
        '
        Me.RadioButton3.AutoSize = True
        Me.RadioButton3.Location = New System.Drawing.Point(334, 29)
        Me.RadioButton3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RadioButton3.Name = "RadioButton3"
        Me.RadioButton3.Size = New System.Drawing.Size(142, 31)
        Me.RadioButton3.TabIndex = 101
        Me.RadioButton3.Text = "列印揀取單"
        Me.RadioButton3.UseVisualStyleBackColor = True
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.BackColor = System.Drawing.Color.FloralWhite
        Me.RadioButton1.Location = New System.Drawing.Point(22, 29)
        Me.RadioButton1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(121, 31)
        Me.RadioButton1.TabIndex = 100
        Me.RadioButton1.Text = "單據作業"
        Me.RadioButton1.UseVisualStyleBackColor = False
        '
        'RadioButton2
        '
        Me.RadioButton2.AutoSize = True
        Me.RadioButton2.Checked = True
        Me.RadioButton2.Location = New System.Drawing.Point(158, 29)
        Me.RadioButton2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RadioButton2.Name = "RadioButton2"
        Me.RadioButton2.Size = New System.Drawing.Size(166, 31)
        Me.RadioButton2.TabIndex = 99
        Me.RadioButton2.TabStop = True
        Me.RadioButton2.Text = "AGV密盆作業"
        Me.RadioButton2.UseVisualStyleBackColor = True
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.GroupBox3)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.BackColor = System.Drawing.Color.FloralWhite
        Me.SplitContainer2.Panel2.Controls.Add(Me.TabControl2)
        Me.SplitContainer2.Size = New System.Drawing.Size(1358, 534)
        Me.SplitContainer2.SplitterDistance = 63
        Me.SplitContainer2.SplitterWidth = 8
        Me.SplitContainer2.TabIndex = 0
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.LightGray
        Me.GroupBox3.Controls.Add(Me.Label12)
        Me.GroupBox3.Controls.Add(Me.Label11)
        Me.GroupBox3.Controls.Add(Me.Label10)
        Me.GroupBox3.Controls.Add(Me.Label8)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox3.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox3.Size = New System.Drawing.Size(1358, 63)
        Me.GroupBox3.TabIndex = 0
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "訂單資訊"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.AntiqueWhite
        Me.Label12.Location = New System.Drawing.Point(645, 43)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(54, 27)
        Me.Label12.TabIndex = 1
        Me.Label12.Text = "－－"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.AntiqueWhite
        Me.Label11.Location = New System.Drawing.Point(402, 43)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(54, 27)
        Me.Label11.TabIndex = 1
        Me.Label11.Text = "－－"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.AntiqueWhite
        Me.Label10.Location = New System.Drawing.Point(153, 43)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(54, 27)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "－－"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(537, 43)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(96, 27)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "檢核量："
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(284, 43)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(108, 27)
        Me.Label7.TabIndex = 0
        Me.Label7.Text = "總pcs數："
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(27, 43)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(117, 27)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "總品項數："
        '
        'TabControl2
        '
        Me.TabControl2.Controls.Add(Me.TabPage3)
        Me.TabControl2.Controls.Add(Me.TabPage4)
        Me.TabControl2.Controls.Add(Me.TabPage7)
        Me.TabControl2.Controls.Add(Me.TabPage8)
        Me.TabControl2.Controls.Add(Me.TabPage6)
        Me.TabControl2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl2.Location = New System.Drawing.Point(0, 0)
        Me.TabControl2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabControl2.Name = "TabControl2"
        Me.TabControl2.SelectedIndex = 0
        Me.TabControl2.Size = New System.Drawing.Size(1358, 463)
        Me.TabControl2.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.DGV_PickDetail)
        Me.TabPage3.Location = New System.Drawing.Point(4, 34)
        Me.TabPage3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage3.Size = New System.Drawing.Size(1350, 425)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "包裝清單"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'DGV_PickDetail
        '
        Me.DGV_PickDetail.AllowUserToAddRows = False
        Me.DGV_PickDetail.AllowUserToDeleteRows = False
        Me.DGV_PickDetail.AllowUserToOrderColumns = True
        Me.DGV_PickDetail.AutoGenerateColumns = False
        Me.DGV_PickDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DGV_PickDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV_PickDetail.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.STORERKEYDataGridViewTextBoxColumn, Me.ROUTEDataGridViewTextBoxColumn, Me.ORDERKEYDataGridViewTextBoxColumn, Me.ORDERLINENUMBERDataGridViewTextBoxColumn, Me.SKUDataGridViewTextBoxColumn, Me.PICKQTYDataGridViewTextBoxColumn, Me.PACKQTYDataGridViewTextBoxColumn, Me.DESCRDataGridViewTextBoxColumn, Me.BUSR3DataGridViewTextBoxColumn, Me.BUSR2DataGridViewTextBoxColumn, Me.CASEIDDataGridViewTextBoxColumn, Me.QTYDataGridViewTextBoxColumn, Me.LOCDataGridViewTextBoxColumn, Me.STATUSDataGridViewTextBoxColumn, Me.PARTDataGridViewTextBoxColumn, Me.PrintMarkDataGridViewTextBoxColumn, Me.WAVEKEYDataGridViewTextBoxColumn, Me.EDITDATEDataGridViewTextBoxColumn, Me.PICKDETAILKEYDataGridViewTextBoxColumn, Me.NOTESDataGridViewTextBoxColumn, Me.EXTERNORDERKEYDataGridViewTextBoxColumn})
        Me.DGV_PickDetail.DataSource = Me.VwPICKDETAILRateBindingSource
        Me.DGV_PickDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGV_PickDetail.Location = New System.Drawing.Point(4, 5)
        Me.DGV_PickDetail.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DGV_PickDetail.Name = "DGV_PickDetail"
        Me.DGV_PickDetail.ReadOnly = True
        Me.DGV_PickDetail.RowHeadersWidth = 51
        Me.DGV_PickDetail.RowTemplate.Height = 24
        Me.DGV_PickDetail.Size = New System.Drawing.Size(1342, 415)
        Me.DGV_PickDetail.TabIndex = 0
        '
        'STORERKEYDataGridViewTextBoxColumn
        '
        Me.STORERKEYDataGridViewTextBoxColumn.DataPropertyName = "STORERKEY"
        Me.STORERKEYDataGridViewTextBoxColumn.HeaderText = "貨主"
        Me.STORERKEYDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.STORERKEYDataGridViewTextBoxColumn.Name = "STORERKEYDataGridViewTextBoxColumn"
        Me.STORERKEYDataGridViewTextBoxColumn.ReadOnly = True
        Me.STORERKEYDataGridViewTextBoxColumn.Width = 90
        '
        'ROUTEDataGridViewTextBoxColumn
        '
        Me.ROUTEDataGridViewTextBoxColumn.DataPropertyName = "ROUTE"
        Me.ROUTEDataGridViewTextBoxColumn.HeaderText = "貨運路線"
        Me.ROUTEDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.ROUTEDataGridViewTextBoxColumn.Name = "ROUTEDataGridViewTextBoxColumn"
        Me.ROUTEDataGridViewTextBoxColumn.ReadOnly = True
        Me.ROUTEDataGridViewTextBoxColumn.Width = 132
        '
        'ORDERKEYDataGridViewTextBoxColumn
        '
        Me.ORDERKEYDataGridViewTextBoxColumn.DataPropertyName = "ORDERKEY"
        Me.ORDERKEYDataGridViewTextBoxColumn.HeaderText = "單號"
        Me.ORDERKEYDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.ORDERKEYDataGridViewTextBoxColumn.Name = "ORDERKEYDataGridViewTextBoxColumn"
        Me.ORDERKEYDataGridViewTextBoxColumn.ReadOnly = True
        Me.ORDERKEYDataGridViewTextBoxColumn.Width = 90
        '
        'ORDERLINENUMBERDataGridViewTextBoxColumn
        '
        Me.ORDERLINENUMBERDataGridViewTextBoxColumn.DataPropertyName = "ORDERLINENUMBER"
        Me.ORDERLINENUMBERDataGridViewTextBoxColumn.HeaderText = "Item"
        Me.ORDERLINENUMBERDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.ORDERLINENUMBERDataGridViewTextBoxColumn.Name = "ORDERLINENUMBERDataGridViewTextBoxColumn"
        Me.ORDERLINENUMBERDataGridViewTextBoxColumn.ReadOnly = True
        Me.ORDERLINENUMBERDataGridViewTextBoxColumn.Width = 93
        '
        'SKUDataGridViewTextBoxColumn
        '
        Me.SKUDataGridViewTextBoxColumn.DataPropertyName = "SKU"
        Me.SKUDataGridViewTextBoxColumn.HeaderText = "商品碼"
        Me.SKUDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.SKUDataGridViewTextBoxColumn.Name = "SKUDataGridViewTextBoxColumn"
        Me.SKUDataGridViewTextBoxColumn.ReadOnly = True
        Me.SKUDataGridViewTextBoxColumn.Width = 111
        '
        'PICKQTYDataGridViewTextBoxColumn
        '
        Me.PICKQTYDataGridViewTextBoxColumn.DataPropertyName = "PICKQTY"
        Me.PICKQTYDataGridViewTextBoxColumn.HeaderText = "預期量"
        Me.PICKQTYDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.PICKQTYDataGridViewTextBoxColumn.Name = "PICKQTYDataGridViewTextBoxColumn"
        Me.PICKQTYDataGridViewTextBoxColumn.ReadOnly = True
        Me.PICKQTYDataGridViewTextBoxColumn.Width = 111
        '
        'PACKQTYDataGridViewTextBoxColumn
        '
        Me.PACKQTYDataGridViewTextBoxColumn.DataPropertyName = "PACKQTY"
        Me.PACKQTYDataGridViewTextBoxColumn.HeaderText = "已驗量"
        Me.PACKQTYDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.PACKQTYDataGridViewTextBoxColumn.Name = "PACKQTYDataGridViewTextBoxColumn"
        Me.PACKQTYDataGridViewTextBoxColumn.ReadOnly = True
        Me.PACKQTYDataGridViewTextBoxColumn.Width = 111
        '
        'DESCRDataGridViewTextBoxColumn
        '
        Me.DESCRDataGridViewTextBoxColumn.DataPropertyName = "DESCR"
        Me.DESCRDataGridViewTextBoxColumn.HeaderText = "品名"
        Me.DESCRDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.DESCRDataGridViewTextBoxColumn.Name = "DESCRDataGridViewTextBoxColumn"
        Me.DESCRDataGridViewTextBoxColumn.ReadOnly = True
        Me.DESCRDataGridViewTextBoxColumn.Width = 90
        '
        'BUSR3DataGridViewTextBoxColumn
        '
        Me.BUSR3DataGridViewTextBoxColumn.DataPropertyName = "BUSR3"
        Me.BUSR3DataGridViewTextBoxColumn.HeaderText = "ISBN"
        Me.BUSR3DataGridViewTextBoxColumn.MinimumWidth = 6
        Me.BUSR3DataGridViewTextBoxColumn.Name = "BUSR3DataGridViewTextBoxColumn"
        Me.BUSR3DataGridViewTextBoxColumn.ReadOnly = True
        Me.BUSR3DataGridViewTextBoxColumn.Width = 96
        '
        'BUSR2DataGridViewTextBoxColumn
        '
        Me.BUSR2DataGridViewTextBoxColumn.DataPropertyName = "BUSR2"
        Me.BUSR2DataGridViewTextBoxColumn.HeaderText = "EAN"
        Me.BUSR2DataGridViewTextBoxColumn.MinimumWidth = 6
        Me.BUSR2DataGridViewTextBoxColumn.Name = "BUSR2DataGridViewTextBoxColumn"
        Me.BUSR2DataGridViewTextBoxColumn.ReadOnly = True
        Me.BUSR2DataGridViewTextBoxColumn.Width = 91
        '
        'CASEIDDataGridViewTextBoxColumn
        '
        Me.CASEIDDataGridViewTextBoxColumn.DataPropertyName = "CASEID"
        Me.CASEIDDataGridViewTextBoxColumn.HeaderText = "箱號"
        Me.CASEIDDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.CASEIDDataGridViewTextBoxColumn.Name = "CASEIDDataGridViewTextBoxColumn"
        Me.CASEIDDataGridViewTextBoxColumn.ReadOnly = True
        Me.CASEIDDataGridViewTextBoxColumn.Width = 90
        '
        'QTYDataGridViewTextBoxColumn
        '
        Me.QTYDataGridViewTextBoxColumn.DataPropertyName = "QTY"
        Me.QTYDataGridViewTextBoxColumn.HeaderText = "數量"
        Me.QTYDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.QTYDataGridViewTextBoxColumn.Name = "QTYDataGridViewTextBoxColumn"
        Me.QTYDataGridViewTextBoxColumn.ReadOnly = True
        Me.QTYDataGridViewTextBoxColumn.Width = 90
        '
        'LOCDataGridViewTextBoxColumn
        '
        Me.LOCDataGridViewTextBoxColumn.DataPropertyName = "LOC"
        Me.LOCDataGridViewTextBoxColumn.HeaderText = "儲位"
        Me.LOCDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.LOCDataGridViewTextBoxColumn.Name = "LOCDataGridViewTextBoxColumn"
        Me.LOCDataGridViewTextBoxColumn.ReadOnly = True
        Me.LOCDataGridViewTextBoxColumn.Width = 90
        '
        'STATUSDataGridViewTextBoxColumn
        '
        Me.STATUSDataGridViewTextBoxColumn.DataPropertyName = "STATUS"
        Me.STATUSDataGridViewTextBoxColumn.HeaderText = "狀態"
        Me.STATUSDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.STATUSDataGridViewTextBoxColumn.Name = "STATUSDataGridViewTextBoxColumn"
        Me.STATUSDataGridViewTextBoxColumn.ReadOnly = True
        Me.STATUSDataGridViewTextBoxColumn.Width = 90
        '
        'PARTDataGridViewTextBoxColumn
        '
        Me.PARTDataGridViewTextBoxColumn.DataPropertyName = "PART"
        Me.PARTDataGridViewTextBoxColumn.HeaderText = "拆包狀態"
        Me.PARTDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.PARTDataGridViewTextBoxColumn.Name = "PARTDataGridViewTextBoxColumn"
        Me.PARTDataGridViewTextBoxColumn.ReadOnly = True
        Me.PARTDataGridViewTextBoxColumn.Width = 132
        '
        'PrintMarkDataGridViewTextBoxColumn
        '
        Me.PrintMarkDataGridViewTextBoxColumn.DataPropertyName = "PrintMark"
        Me.PrintMarkDataGridViewTextBoxColumn.HeaderText = "列印狀態"
        Me.PrintMarkDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.PrintMarkDataGridViewTextBoxColumn.Name = "PrintMarkDataGridViewTextBoxColumn"
        Me.PrintMarkDataGridViewTextBoxColumn.ReadOnly = True
        Me.PrintMarkDataGridViewTextBoxColumn.Width = 132
        '
        'WAVEKEYDataGridViewTextBoxColumn
        '
        Me.WAVEKEYDataGridViewTextBoxColumn.DataPropertyName = "WAVEKEY"
        Me.WAVEKEYDataGridViewTextBoxColumn.HeaderText = "群組單"
        Me.WAVEKEYDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.WAVEKEYDataGridViewTextBoxColumn.Name = "WAVEKEYDataGridViewTextBoxColumn"
        Me.WAVEKEYDataGridViewTextBoxColumn.ReadOnly = True
        Me.WAVEKEYDataGridViewTextBoxColumn.Width = 111
        '
        'EDITDATEDataGridViewTextBoxColumn
        '
        Me.EDITDATEDataGridViewTextBoxColumn.DataPropertyName = "EDITDATE"
        Me.EDITDATEDataGridViewTextBoxColumn.HeaderText = "作業日"
        Me.EDITDATEDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.EDITDATEDataGridViewTextBoxColumn.Name = "EDITDATEDataGridViewTextBoxColumn"
        Me.EDITDATEDataGridViewTextBoxColumn.ReadOnly = True
        Me.EDITDATEDataGridViewTextBoxColumn.Width = 111
        '
        'PICKDETAILKEYDataGridViewTextBoxColumn
        '
        Me.PICKDETAILKEYDataGridViewTextBoxColumn.DataPropertyName = "PICKDETAILKEY"
        Me.PICKDETAILKEYDataGridViewTextBoxColumn.HeaderText = "系統ID"
        Me.PICKDETAILKEYDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.PICKDETAILKEYDataGridViewTextBoxColumn.Name = "PICKDETAILKEYDataGridViewTextBoxColumn"
        Me.PICKDETAILKEYDataGridViewTextBoxColumn.ReadOnly = True
        Me.PICKDETAILKEYDataGridViewTextBoxColumn.Width = 112
        '
        'NOTESDataGridViewTextBoxColumn
        '
        Me.NOTESDataGridViewTextBoxColumn.DataPropertyName = "NOTES"
        Me.NOTESDataGridViewTextBoxColumn.HeaderText = "NOTES"
        Me.NOTESDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.NOTESDataGridViewTextBoxColumn.Name = "NOTESDataGridViewTextBoxColumn"
        Me.NOTESDataGridViewTextBoxColumn.ReadOnly = True
        Me.NOTESDataGridViewTextBoxColumn.Width = 117
        '
        'EXTERNORDERKEYDataGridViewTextBoxColumn
        '
        Me.EXTERNORDERKEYDataGridViewTextBoxColumn.DataPropertyName = "EXTERNORDERKEY"
        Me.EXTERNORDERKEYDataGridViewTextBoxColumn.HeaderText = "EXTERNORDERKEY"
        Me.EXTERNORDERKEYDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.EXTERNORDERKEYDataGridViewTextBoxColumn.Name = "EXTERNORDERKEYDataGridViewTextBoxColumn"
        Me.EXTERNORDERKEYDataGridViewTextBoxColumn.ReadOnly = True
        Me.EXTERNORDERKEYDataGridViewTextBoxColumn.Width = 231
        '
        'VwPICKDETAILRateBindingSource
        '
        Me.VwPICKDETAILRateBindingSource.DataMember = "Vw_PICKDETAIL_Rate"
        Me.VwPICKDETAILRateBindingSource.DataSource = Me.PRODDataSet
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.DGV_CaseDetail)
        Me.TabPage4.Location = New System.Drawing.Point(4, 34)
        Me.TabPage4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage4.Size = New System.Drawing.Size(1350, 425)
        Me.TabPage4.TabIndex = 1
        Me.TabPage4.Text = "作業箱明細"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'DGV_CaseDetail
        '
        Me.DGV_CaseDetail.AllowUserToAddRows = False
        Me.DGV_CaseDetail.AllowUserToDeleteRows = False
        Me.DGV_CaseDetail.AllowUserToOrderColumns = True
        Me.DGV_CaseDetail.AutoGenerateColumns = False
        Me.DGV_CaseDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV_CaseDetail.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ItemNoDataGridViewTextBoxColumn, Me.StorerkeyDataGridViewTextBoxColumn1, Me.ROUTEDataGridViewTextBoxColumn1, Me.OrderkeyDataGridViewTextBoxColumn1, Me.OrderkeyLineNumberDataGridViewTextBoxColumn, Me.DescrDataGridViewTextBoxColumn1, Me.SkuDataGridViewTextBoxColumn1, Me.DOORDataGridViewTextBoxColumn1, Me.LocDataGridViewTextBoxColumn1, Me.QTYDataGridViewTextBoxColumn1, Me.GUINODataGridViewTextBoxColumn1, Me.NOTESDataGridViewTextBoxColumn1, Me.ExternorderkeyDataGridViewTextBoxColumn1, Me.WAVEKEYDataGridViewTextBoxColumn1, Me.EDITDATEDataGridViewTextBoxColumn1, Me.EDITWHODataGridViewTextBoxColumn1, Me.PICKDETAILKEYDataGridViewTextBoxColumn1, Me.CURCYDataGridViewTextBoxColumn1, Me.TAX01DataGridViewTextBoxColumn1, Me.PRICEDataGridViewTextBoxColumn1, Me.UNITPRICEDataGridViewTextBoxColumn1, Me.Recovery})
        Me.DGV_CaseDetail.DataSource = Me.CombinationCaseDetailBindingSource
        Me.DGV_CaseDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGV_CaseDetail.Location = New System.Drawing.Point(4, 5)
        Me.DGV_CaseDetail.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DGV_CaseDetail.Name = "DGV_CaseDetail"
        Me.DGV_CaseDetail.ReadOnly = True
        Me.DGV_CaseDetail.RowHeadersWidth = 51
        Me.DGV_CaseDetail.RowTemplate.Height = 24
        Me.DGV_CaseDetail.Size = New System.Drawing.Size(1342, 415)
        Me.DGV_CaseDetail.TabIndex = 0
        '
        'ItemNoDataGridViewTextBoxColumn
        '
        Me.ItemNoDataGridViewTextBoxColumn.DataPropertyName = "Item_No"
        Me.ItemNoDataGridViewTextBoxColumn.HeaderText = "Item_No"
        Me.ItemNoDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.ItemNoDataGridViewTextBoxColumn.Name = "ItemNoDataGridViewTextBoxColumn"
        Me.ItemNoDataGridViewTextBoxColumn.ReadOnly = True
        Me.ItemNoDataGridViewTextBoxColumn.Width = 125
        '
        'StorerkeyDataGridViewTextBoxColumn1
        '
        Me.StorerkeyDataGridViewTextBoxColumn1.DataPropertyName = "Storerkey"
        Me.StorerkeyDataGridViewTextBoxColumn1.HeaderText = "貨主"
        Me.StorerkeyDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.StorerkeyDataGridViewTextBoxColumn1.Name = "StorerkeyDataGridViewTextBoxColumn1"
        Me.StorerkeyDataGridViewTextBoxColumn1.ReadOnly = True
        Me.StorerkeyDataGridViewTextBoxColumn1.Width = 125
        '
        'ROUTEDataGridViewTextBoxColumn1
        '
        Me.ROUTEDataGridViewTextBoxColumn1.DataPropertyName = "ROUTE"
        Me.ROUTEDataGridViewTextBoxColumn1.HeaderText = "貨運路線"
        Me.ROUTEDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.ROUTEDataGridViewTextBoxColumn1.Name = "ROUTEDataGridViewTextBoxColumn1"
        Me.ROUTEDataGridViewTextBoxColumn1.ReadOnly = True
        Me.ROUTEDataGridViewTextBoxColumn1.Width = 125
        '
        'OrderkeyDataGridViewTextBoxColumn1
        '
        Me.OrderkeyDataGridViewTextBoxColumn1.DataPropertyName = "Orderkey"
        Me.OrderkeyDataGridViewTextBoxColumn1.HeaderText = "單號"
        Me.OrderkeyDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.OrderkeyDataGridViewTextBoxColumn1.Name = "OrderkeyDataGridViewTextBoxColumn1"
        Me.OrderkeyDataGridViewTextBoxColumn1.ReadOnly = True
        Me.OrderkeyDataGridViewTextBoxColumn1.Width = 125
        '
        'OrderkeyLineNumberDataGridViewTextBoxColumn
        '
        Me.OrderkeyLineNumberDataGridViewTextBoxColumn.DataPropertyName = "OrderkeyLineNumber"
        Me.OrderkeyLineNumberDataGridViewTextBoxColumn.HeaderText = "Item"
        Me.OrderkeyLineNumberDataGridViewTextBoxColumn.MinimumWidth = 6
        Me.OrderkeyLineNumberDataGridViewTextBoxColumn.Name = "OrderkeyLineNumberDataGridViewTextBoxColumn"
        Me.OrderkeyLineNumberDataGridViewTextBoxColumn.ReadOnly = True
        Me.OrderkeyLineNumberDataGridViewTextBoxColumn.Width = 125
        '
        'DescrDataGridViewTextBoxColumn1
        '
        Me.DescrDataGridViewTextBoxColumn1.DataPropertyName = "descr"
        Me.DescrDataGridViewTextBoxColumn1.HeaderText = "品名"
        Me.DescrDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.DescrDataGridViewTextBoxColumn1.Name = "DescrDataGridViewTextBoxColumn1"
        Me.DescrDataGridViewTextBoxColumn1.ReadOnly = True
        Me.DescrDataGridViewTextBoxColumn1.Width = 125
        '
        'SkuDataGridViewTextBoxColumn1
        '
        Me.SkuDataGridViewTextBoxColumn1.DataPropertyName = "Sku"
        Me.SkuDataGridViewTextBoxColumn1.HeaderText = "商品碼"
        Me.SkuDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.SkuDataGridViewTextBoxColumn1.Name = "SkuDataGridViewTextBoxColumn1"
        Me.SkuDataGridViewTextBoxColumn1.ReadOnly = True
        Me.SkuDataGridViewTextBoxColumn1.Width = 125
        '
        'DOORDataGridViewTextBoxColumn1
        '
        Me.DOORDataGridViewTextBoxColumn1.DataPropertyName = "DOOR"
        Me.DOORDataGridViewTextBoxColumn1.HeaderText = "DOOR"
        Me.DOORDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.DOORDataGridViewTextBoxColumn1.Name = "DOORDataGridViewTextBoxColumn1"
        Me.DOORDataGridViewTextBoxColumn1.ReadOnly = True
        Me.DOORDataGridViewTextBoxColumn1.Width = 125
        '
        'LocDataGridViewTextBoxColumn1
        '
        Me.LocDataGridViewTextBoxColumn1.DataPropertyName = "Loc"
        Me.LocDataGridViewTextBoxColumn1.HeaderText = "儲位"
        Me.LocDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.LocDataGridViewTextBoxColumn1.Name = "LocDataGridViewTextBoxColumn1"
        Me.LocDataGridViewTextBoxColumn1.ReadOnly = True
        Me.LocDataGridViewTextBoxColumn1.Width = 125
        '
        'QTYDataGridViewTextBoxColumn1
        '
        Me.QTYDataGridViewTextBoxColumn1.DataPropertyName = "QTY"
        Me.QTYDataGridViewTextBoxColumn1.HeaderText = "數量"
        Me.QTYDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.QTYDataGridViewTextBoxColumn1.Name = "QTYDataGridViewTextBoxColumn1"
        Me.QTYDataGridViewTextBoxColumn1.ReadOnly = True
        Me.QTYDataGridViewTextBoxColumn1.Width = 125
        '
        'GUINODataGridViewTextBoxColumn1
        '
        Me.GUINODataGridViewTextBoxColumn1.DataPropertyName = "GUINO"
        Me.GUINODataGridViewTextBoxColumn1.HeaderText = "發票號"
        Me.GUINODataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.GUINODataGridViewTextBoxColumn1.Name = "GUINODataGridViewTextBoxColumn1"
        Me.GUINODataGridViewTextBoxColumn1.ReadOnly = True
        Me.GUINODataGridViewTextBoxColumn1.Width = 125
        '
        'NOTESDataGridViewTextBoxColumn1
        '
        Me.NOTESDataGridViewTextBoxColumn1.DataPropertyName = "NOTES"
        Me.NOTESDataGridViewTextBoxColumn1.HeaderText = "NOTES"
        Me.NOTESDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.NOTESDataGridViewTextBoxColumn1.Name = "NOTESDataGridViewTextBoxColumn1"
        Me.NOTESDataGridViewTextBoxColumn1.ReadOnly = True
        Me.NOTESDataGridViewTextBoxColumn1.Width = 125
        '
        'ExternorderkeyDataGridViewTextBoxColumn1
        '
        Me.ExternorderkeyDataGridViewTextBoxColumn1.DataPropertyName = "Externorderkey"
        Me.ExternorderkeyDataGridViewTextBoxColumn1.HeaderText = "Externorderkey"
        Me.ExternorderkeyDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.ExternorderkeyDataGridViewTextBoxColumn1.Name = "ExternorderkeyDataGridViewTextBoxColumn1"
        Me.ExternorderkeyDataGridViewTextBoxColumn1.ReadOnly = True
        Me.ExternorderkeyDataGridViewTextBoxColumn1.Width = 125
        '
        'WAVEKEYDataGridViewTextBoxColumn1
        '
        Me.WAVEKEYDataGridViewTextBoxColumn1.DataPropertyName = "WAVEKEY"
        Me.WAVEKEYDataGridViewTextBoxColumn1.HeaderText = "群組單"
        Me.WAVEKEYDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.WAVEKEYDataGridViewTextBoxColumn1.Name = "WAVEKEYDataGridViewTextBoxColumn1"
        Me.WAVEKEYDataGridViewTextBoxColumn1.ReadOnly = True
        Me.WAVEKEYDataGridViewTextBoxColumn1.Width = 125
        '
        'EDITDATEDataGridViewTextBoxColumn1
        '
        Me.EDITDATEDataGridViewTextBoxColumn1.DataPropertyName = "EDITDATE"
        Me.EDITDATEDataGridViewTextBoxColumn1.HeaderText = "作業日"
        Me.EDITDATEDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.EDITDATEDataGridViewTextBoxColumn1.Name = "EDITDATEDataGridViewTextBoxColumn1"
        Me.EDITDATEDataGridViewTextBoxColumn1.ReadOnly = True
        Me.EDITDATEDataGridViewTextBoxColumn1.Width = 125
        '
        'EDITWHODataGridViewTextBoxColumn1
        '
        Me.EDITWHODataGridViewTextBoxColumn1.DataPropertyName = "EDITWHO"
        Me.EDITWHODataGridViewTextBoxColumn1.HeaderText = "作業人員"
        Me.EDITWHODataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.EDITWHODataGridViewTextBoxColumn1.Name = "EDITWHODataGridViewTextBoxColumn1"
        Me.EDITWHODataGridViewTextBoxColumn1.ReadOnly = True
        Me.EDITWHODataGridViewTextBoxColumn1.Width = 125
        '
        'PICKDETAILKEYDataGridViewTextBoxColumn1
        '
        Me.PICKDETAILKEYDataGridViewTextBoxColumn1.DataPropertyName = "PICKDETAILKEY"
        Me.PICKDETAILKEYDataGridViewTextBoxColumn1.HeaderText = "系統ID"
        Me.PICKDETAILKEYDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.PICKDETAILKEYDataGridViewTextBoxColumn1.Name = "PICKDETAILKEYDataGridViewTextBoxColumn1"
        Me.PICKDETAILKEYDataGridViewTextBoxColumn1.ReadOnly = True
        Me.PICKDETAILKEYDataGridViewTextBoxColumn1.Width = 125
        '
        'CURCYDataGridViewTextBoxColumn1
        '
        Me.CURCYDataGridViewTextBoxColumn1.DataPropertyName = "CURCY"
        Me.CURCYDataGridViewTextBoxColumn1.HeaderText = "幣別"
        Me.CURCYDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.CURCYDataGridViewTextBoxColumn1.Name = "CURCYDataGridViewTextBoxColumn1"
        Me.CURCYDataGridViewTextBoxColumn1.ReadOnly = True
        Me.CURCYDataGridViewTextBoxColumn1.Width = 125
        '
        'TAX01DataGridViewTextBoxColumn1
        '
        Me.TAX01DataGridViewTextBoxColumn1.DataPropertyName = "TAX01"
        Me.TAX01DataGridViewTextBoxColumn1.HeaderText = "稅額"
        Me.TAX01DataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.TAX01DataGridViewTextBoxColumn1.Name = "TAX01DataGridViewTextBoxColumn1"
        Me.TAX01DataGridViewTextBoxColumn1.ReadOnly = True
        Me.TAX01DataGridViewTextBoxColumn1.Width = 125
        '
        'PRICEDataGridViewTextBoxColumn1
        '
        Me.PRICEDataGridViewTextBoxColumn1.DataPropertyName = "PRICE"
        Me.PRICEDataGridViewTextBoxColumn1.HeaderText = "價錢"
        Me.PRICEDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.PRICEDataGridViewTextBoxColumn1.Name = "PRICEDataGridViewTextBoxColumn1"
        Me.PRICEDataGridViewTextBoxColumn1.ReadOnly = True
        Me.PRICEDataGridViewTextBoxColumn1.Width = 125
        '
        'UNITPRICEDataGridViewTextBoxColumn1
        '
        Me.UNITPRICEDataGridViewTextBoxColumn1.DataPropertyName = "UNITPRICE"
        Me.UNITPRICEDataGridViewTextBoxColumn1.HeaderText = "單價"
        Me.UNITPRICEDataGridViewTextBoxColumn1.MinimumWidth = 6
        Me.UNITPRICEDataGridViewTextBoxColumn1.Name = "UNITPRICEDataGridViewTextBoxColumn1"
        Me.UNITPRICEDataGridViewTextBoxColumn1.ReadOnly = True
        Me.UNITPRICEDataGridViewTextBoxColumn1.Width = 125
        '
        'Recovery
        '
        Me.Recovery.HeaderText = ""
        Me.Recovery.MinimumWidth = 6
        Me.Recovery.Name = "Recovery"
        Me.Recovery.ReadOnly = True
        Me.Recovery.Text = "回收"
        Me.Recovery.UseColumnTextForButtonValue = True
        Me.Recovery.Width = 80
        '
        'CombinationCaseDetailBindingSource
        '
        Me.CombinationCaseDetailBindingSource.DataMember = "Combination_CaseDetail"
        Me.CombinationCaseDetailBindingSource.DataSource = Me.PRODDataSet
        '
        'TabPage7
        '
        Me.TabPage7.Controls.Add(Me.ReportViewer1)
        Me.TabPage7.Location = New System.Drawing.Point(4, 34)
        Me.TabPage7.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage7.Size = New System.Drawing.Size(1350, 425)
        Me.TabPage7.TabIndex = 3
        Me.TabPage7.Text = "地址條"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "DataSet1"
        ReportDataSource1.Value = Me.Vw_Case_Addr_InvoiceBindingSource
        Me.ReportViewer1.LocalReport.DataSources.Add(ReportDataSource1)
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_711.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(4, 5)
        Me.ReportViewer1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.Size = New System.Drawing.Size(1342, 415)
        Me.ReportViewer1.TabIndex = 0
        '
        'TabPage8
        '
        Me.TabPage8.Controls.Add(Me.ReportViewer2)
        Me.TabPage8.Location = New System.Drawing.Point(4, 34)
        Me.TabPage8.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage8.Size = New System.Drawing.Size(1350, 425)
        Me.TabPage8.TabIndex = 4
        Me.TabPage8.Text = "發票"
        Me.TabPage8.UseVisualStyleBackColor = True
        '
        'ReportViewer2
        '
        Me.ReportViewer2.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource2.Name = "DataSet1"
        ReportDataSource2.Value = Me.Vw_Case_Addr_InvoiceBindingSource
        Me.ReportViewer2.LocalReport.DataSources.Add(ReportDataSource2)
        Me.ReportViewer2.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_invoiceA4.rdlc"
        Me.ReportViewer2.Location = New System.Drawing.Point(4, 5)
        Me.ReportViewer2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ReportViewer2.Name = "ReportViewer2"
        Me.ReportViewer2.Size = New System.Drawing.Size(1342, 415)
        Me.ReportViewer2.TabIndex = 0
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.ReportViewer3)
        Me.TabPage6.Location = New System.Drawing.Point(4, 34)
        Me.TabPage6.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage6.Size = New System.Drawing.Size(1350, 425)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "出貨明細"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'ReportViewer3
        '
        Me.ReportViewer3.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource3.Name = "DataSet1"
        ReportDataSource3.Value = Me.Vw_Case_Addr_InvoiceBindingSource
        Me.ReportViewer3.LocalReport.DataSources.Add(ReportDataSource3)
        Me.ReportViewer3.LocalReport.ReportEmbeddedResource = "Packet_DEV.Rpt_PickList_EC.rdlc"
        Me.ReportViewer3.Location = New System.Drawing.Point(4, 5)
        Me.ReportViewer3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ReportViewer3.Name = "ReportViewer3"
        Me.ReportViewer3.Size = New System.Drawing.Size(1342, 415)
        Me.ReportViewer3.TabIndex = 0
        '
        'TabPage9
        '
        Me.TabPage9.Controls.Add(Me.SplitContainer4)
        Me.TabPage9.Location = New System.Drawing.Point(4, 34)
        Me.TabPage9.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage9.Size = New System.Drawing.Size(1366, 827)
        Me.TabPage9.TabIndex = 2
        Me.TabPage9.Text = "補印"
        Me.TabPage9.UseVisualStyleBackColor = True
        '
        'SplitContainer4
        '
        Me.SplitContainer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer4.Location = New System.Drawing.Point(4, 5)
        Me.SplitContainer4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.SplitContainer4.Name = "SplitContainer4"
        Me.SplitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer4.Panel1
        '
        Me.SplitContainer4.Panel1.BackColor = System.Drawing.Color.FloralWhite
        Me.SplitContainer4.Panel1.Controls.Add(Me.Button2)
        Me.SplitContainer4.Panel1.Controls.Add(Me.Label15)
        Me.SplitContainer4.Panel1.Controls.Add(Me.repokey_Text)
        Me.SplitContainer4.Panel1.Controls.Add(Me.Label4)
        '
        'SplitContainer4.Panel2
        '
        Me.SplitContainer4.Panel2.Controls.Add(Me.SplitContainer5)
        Me.SplitContainer4.Size = New System.Drawing.Size(1358, 817)
        Me.SplitContainer4.SplitterDistance = 126
        Me.SplitContainer4.SplitterWidth = 8
        Me.SplitContainer4.TabIndex = 0
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(411, 42)
        Me.Button2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(170, 38)
        Me.Button2.TabIndex = 103
        Me.Button2.Text = "補印箱明細"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(4, 118)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(117, 27)
        Me.Label15.TabIndex = 102
        Me.Label15.Text = "補印地址條"
        '
        'repokey_Text
        '
        Me.repokey_Text.Location = New System.Drawing.Point(148, 40)
        Me.repokey_Text.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.repokey_Text.Name = "repokey_Text"
        Me.repokey_Text.Size = New System.Drawing.Size(250, 35)
        Me.repokey_Text.TabIndex = 101
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(22, 45)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(117, 27)
        Me.Label4.TabIndex = 100
        Me.Label4.Text = "單據號碼："
        '
        'SplitContainer5
        '
        Me.SplitContainer5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer5.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer5.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.SplitContainer5.Name = "SplitContainer5"
        Me.SplitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer5.Panel1
        '
        Me.SplitContainer5.Panel1.Controls.Add(Me.DGV_reAddr)
        '
        'SplitContainer5.Panel2
        '
        Me.SplitContainer5.Panel2.Controls.Add(Me.SplitContainer6)
        Me.SplitContainer5.Size = New System.Drawing.Size(1358, 683)
        Me.SplitContainer5.SplitterDistance = 255
        Me.SplitContainer5.SplitterWidth = 8
        Me.SplitContainer5.TabIndex = 0
        '
        'DGV_reAddr
        '
        Me.DGV_reAddr.AllowUserToAddRows = False
        Me.DGV_reAddr.AllowUserToDeleteRows = False
        Me.DGV_reAddr.AllowUserToOrderColumns = True
        Me.DGV_reAddr.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.DGV_reAddr.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV_reAddr.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.addrPrintBtn})
        Me.DGV_reAddr.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGV_reAddr.Location = New System.Drawing.Point(0, 0)
        Me.DGV_reAddr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DGV_reAddr.Name = "DGV_reAddr"
        Me.DGV_reAddr.ReadOnly = True
        Me.DGV_reAddr.RowHeadersWidth = 51
        Me.DGV_reAddr.RowTemplate.Height = 24
        Me.DGV_reAddr.Size = New System.Drawing.Size(1358, 255)
        Me.DGV_reAddr.TabIndex = 1
        '
        'addrPrintBtn
        '
        Me.addrPrintBtn.HeaderText = ""
        Me.addrPrintBtn.MinimumWidth = 6
        Me.addrPrintBtn.Name = "addrPrintBtn"
        Me.addrPrintBtn.ReadOnly = True
        Me.addrPrintBtn.Text = "列印"
        Me.addrPrintBtn.UseColumnTextForButtonValue = True
        Me.addrPrintBtn.Width = 6
        '
        'SplitContainer6
        '
        Me.SplitContainer6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer6.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer6.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.SplitContainer6.Name = "SplitContainer6"
        Me.SplitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer6.Panel1
        '
        Me.SplitContainer6.Panel1.BackColor = System.Drawing.Color.FloralWhite
        Me.SplitContainer6.Panel1.Controls.Add(Me.Label16)
        '
        'SplitContainer6.Panel2
        '
        Me.SplitContainer6.Panel2.Controls.Add(Me.DGV_reInvoice)
        Me.SplitContainer6.Size = New System.Drawing.Size(1358, 420)
        Me.SplitContainer6.SplitterDistance = 38
        Me.SplitContainer6.SplitterWidth = 8
        Me.SplitContainer6.TabIndex = 0
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(0, 12)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(96, 27)
        Me.Label16.TabIndex = 103
        Me.Label16.Text = "補印發票"
        '
        'DGV_reInvoice
        '
        Me.DGV_reInvoice.AllowUserToAddRows = False
        Me.DGV_reInvoice.AllowUserToDeleteRows = False
        Me.DGV_reInvoice.AllowUserToOrderColumns = True
        Me.DGV_reInvoice.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.DGV_reInvoice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV_reInvoice.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.invicePrintBtn})
        Me.DGV_reInvoice.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGV_reInvoice.Location = New System.Drawing.Point(0, 0)
        Me.DGV_reInvoice.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DGV_reInvoice.Name = "DGV_reInvoice"
        Me.DGV_reInvoice.ReadOnly = True
        Me.DGV_reInvoice.RowHeadersWidth = 51
        Me.DGV_reInvoice.RowTemplate.Height = 24
        Me.DGV_reInvoice.Size = New System.Drawing.Size(1358, 374)
        Me.DGV_reInvoice.TabIndex = 1
        '
        'invicePrintBtn
        '
        Me.invicePrintBtn.HeaderText = ""
        Me.invicePrintBtn.MinimumWidth = 6
        Me.invicePrintBtn.Name = "invicePrintBtn"
        Me.invicePrintBtn.ReadOnly = True
        Me.invicePrintBtn.Text = "列印"
        Me.invicePrintBtn.UseColumnTextForButtonValue = True
        Me.invicePrintBtn.Width = 6
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.DGV_WAVE)
        Me.TabPage2.Controls.Add(Me.Panel1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 34)
        Me.TabPage2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage2.Size = New System.Drawing.Size(1366, 827)
        Me.TabPage2.TabIndex = 3
        Me.TabPage2.Text = "預購"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'DGV_WAVE
        '
        Me.DGV_WAVE.AllowUserToAddRows = False
        Me.DGV_WAVE.AllowUserToDeleteRows = False
        Me.DGV_WAVE.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.DGV_WAVE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV_WAVE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGV_WAVE.Location = New System.Drawing.Point(4, 173)
        Me.DGV_WAVE.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DGV_WAVE.Name = "DGV_WAVE"
        Me.DGV_WAVE.ReadOnly = True
        Me.DGV_WAVE.RowHeadersWidth = 51
        Me.DGV_WAVE.RowTemplate.Height = 24
        Me.DGV_WAVE.Size = New System.Drawing.Size(1358, 649)
        Me.DGV_WAVE.TabIndex = 1
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FloralWhite
        Me.Panel1.Controls.Add(Me.wavekey_Text)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(4, 5)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1358, 168)
        Me.Panel1.TabIndex = 0
        '
        'wavekey_Text
        '
        Me.wavekey_Text.Location = New System.Drawing.Point(99, 49)
        Me.wavekey_Text.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.wavekey_Text.Name = "wavekey_Text"
        Me.wavekey_Text.Size = New System.Drawing.Size(240, 35)
        Me.wavekey_Text.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 55)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 27)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "批次號："
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.Panel2)
        Me.TabPage5.Location = New System.Drawing.Point(4, 34)
        Me.TabPage5.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage5.Size = New System.Drawing.Size(1366, 827)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "7-11上傳確認"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FloralWhite
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.UploadCase_txt)
        Me.Panel2.Controls.Add(Me.UploadPo_txt)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(4, 5)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1358, 817)
        Me.Panel2.TabIndex = 0
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(98, 208)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(75, 27)
        Me.Label9.TabIndex = 3
        Me.Label9.Text = "條碼："
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(98, 98)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(75, 27)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "單號："
        '
        'UploadCase_txt
        '
        Me.UploadCase_txt.Location = New System.Drawing.Point(182, 202)
        Me.UploadCase_txt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.UploadCase_txt.Name = "UploadCase_txt"
        Me.UploadCase_txt.Size = New System.Drawing.Size(202, 35)
        Me.UploadCase_txt.TabIndex = 1
        '
        'UploadPo_txt
        '
        Me.UploadPo_txt.Location = New System.Drawing.Point(182, 92)
        Me.UploadPo_txt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.UploadPo_txt.Name = "UploadPo_txt"
        Me.UploadPo_txt.Size = New System.Drawing.Size(202, 35)
        Me.UploadPo_txt.TabIndex = 0
        '
        'TabPage10
        '
        Me.TabPage10.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TabPage10.Controls.Add(Me.Button3)
        Me.TabPage10.Controls.Add(Me.Label18)
        Me.TabPage10.Controls.Add(Me.ComboBox2)
        Me.TabPage10.Controls.Add(Me.Button1)
        Me.TabPage10.Controls.Add(Me.Label17)
        Me.TabPage10.Controls.Add(Me.ComboBox1)
        Me.TabPage10.Location = New System.Drawing.Point(4, 34)
        Me.TabPage10.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage10.Size = New System.Drawing.Size(1366, 827)
        Me.TabPage10.TabIndex = 5
        Me.TabPage10.Text = "作業監控設備"
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(16, 9)
        Me.Button3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(400, 98)
        Me.Button3.TabIndex = 5
        Me.Button3.Text = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(12, 197)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(96, 27)
        Me.Label18.TabIndex = 4
        Me.Label18.Text = "人員設定"
        '
        'ComboBox2
        '
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Location = New System.Drawing.Point(12, 232)
        Me.ComboBox2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(403, 33)
        Me.ComboBox2.TabIndex = 3
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 285)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(112, 38)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "確定"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(12, 112)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(96, 27)
        Me.Label17.TabIndex = 1
        Me.Label17.Text = "站別設定"
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {"無", "IP401", "IP402", "IP403", "IP404", "IP405", "IP406", "IP407", "IP408", "IP409", "IP410", "IP411", "IP412", "IP501", "IP502", "IP503", "IP504", "IP505", "IP506", "IP507", "IP508", "IP509", "IP510", "IP511", "IP512"})
        Me.ComboBox1.Location = New System.Drawing.Point(12, 148)
        Me.ComboBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(403, 33)
        Me.ComboBox1.TabIndex = 0
        '
        'TabPage11
        '
        Me.TabPage11.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.TabPage11.Controls.Add(Me.Panel4)
        Me.TabPage11.Controls.Add(Me.Panel3)
        Me.TabPage11.Location = New System.Drawing.Point(4, 34)
        Me.TabPage11.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabPage11.Name = "TabPage11"
        Me.TabPage11.Size = New System.Drawing.Size(1366, 827)
        Me.TabPage11.TabIndex = 6
        Me.TabPage11.Text = "揀貨台車"
        '
        'Panel4
        '
        Me.Panel4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel4.Controls.Add(Me.ReportViewer4)
        Me.Panel4.Location = New System.Drawing.Point(12, 445)
        Me.Panel4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1338, 355)
        Me.Panel4.TabIndex = 3
        '
        'ReportViewer4
        '
        Me.ReportViewer4.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource4.Name = "CarPickV_DTS"
        ReportDataSource4.Value = Me.AGVPickCarListBindingSource
        Me.ReportViewer4.LocalReport.DataSources.Add(ReportDataSource4)
        Me.ReportViewer4.LocalReport.ReportEmbeddedResource = "Packet_DEV.Rpt_CarPickV.rdlc"
        Me.ReportViewer4.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ReportViewer4.Name = "ReportViewer4"
        Me.ReportViewer4.Size = New System.Drawing.Size(1338, 355)
        Me.ReportViewer4.TabIndex = 0
        '
        'Panel3
        '
        Me.Panel3.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel3.Controls.Add(Me.Panel5)
        Me.Panel3.Controls.Add(Me.Label21)
        Me.Panel3.Controls.Add(Me.AGVBoxNo)
        Me.Panel3.Controls.Add(Me.AGVCarClose)
        Me.Panel3.Controls.Add(Me.Label20)
        Me.Panel3.Controls.Add(Me.CarLocNo)
        Me.Panel3.Controls.Add(Me.Label19)
        Me.Panel3.Controls.Add(Me.CarNo)
        Me.Panel3.Location = New System.Drawing.Point(12, 25)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1338, 409)
        Me.Panel3.TabIndex = 2
        '
        'Panel5
        '
        Me.Panel5.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel5.Controls.Add(Me.TableLayoutPanel1)
        Me.Panel5.Location = New System.Drawing.Point(261, 5)
        Me.Panel5.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(1072, 400)
        Me.Panel5.TabIndex = 7
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_J, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_I, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_L, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_K, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_A, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_B, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_C, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_D, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_E, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_F, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_H, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.AGVBOX_G, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1072, 400)
        Me.TableLayoutPanel1.TabIndex = 7
        '
        'AGVBOX_J
        '
        Me.AGVBOX_J.AutoSize = True
        Me.AGVBOX_J.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_J.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_J.Location = New System.Drawing.Point(4, 300)
        Me.AGVBOX_J.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_J.Name = "AGVBOX_J"
        Me.AGVBOX_J.Size = New System.Drawing.Size(349, 100)
        Me.AGVBOX_J.TabIndex = 12
        Me.AGVBOX_J.Text = "J"
        '
        'AGVBOX_I
        '
        Me.AGVBOX_I.AutoSize = True
        Me.AGVBOX_I.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_I.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_I.Location = New System.Drawing.Point(718, 200)
        Me.AGVBOX_I.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_I.Name = "AGVBOX_I"
        Me.AGVBOX_I.Size = New System.Drawing.Size(350, 100)
        Me.AGVBOX_I.TabIndex = 11
        Me.AGVBOX_I.Text = "I"
        '
        'AGVBOX_L
        '
        Me.AGVBOX_L.AutoSize = True
        Me.AGVBOX_L.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_L.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_L.Location = New System.Drawing.Point(718, 300)
        Me.AGVBOX_L.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_L.Name = "AGVBOX_L"
        Me.AGVBOX_L.Size = New System.Drawing.Size(350, 100)
        Me.AGVBOX_L.TabIndex = 10
        Me.AGVBOX_L.Text = "L"
        '
        'AGVBOX_K
        '
        Me.AGVBOX_K.AutoSize = True
        Me.AGVBOX_K.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_K.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_K.Location = New System.Drawing.Point(361, 300)
        Me.AGVBOX_K.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_K.Name = "AGVBOX_K"
        Me.AGVBOX_K.Size = New System.Drawing.Size(349, 100)
        Me.AGVBOX_K.TabIndex = 8
        Me.AGVBOX_K.Text = "K"
        '
        'AGVBOX_A
        '
        Me.AGVBOX_A.AutoSize = True
        Me.AGVBOX_A.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_A.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_A.Location = New System.Drawing.Point(4, 0)
        Me.AGVBOX_A.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_A.Name = "AGVBOX_A"
        Me.AGVBOX_A.Size = New System.Drawing.Size(349, 100)
        Me.AGVBOX_A.TabIndex = 1
        Me.AGVBOX_A.Text = "A"
        '
        'AGVBOX_B
        '
        Me.AGVBOX_B.AutoSize = True
        Me.AGVBOX_B.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_B.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_B.Location = New System.Drawing.Point(361, 0)
        Me.AGVBOX_B.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_B.Name = "AGVBOX_B"
        Me.AGVBOX_B.Size = New System.Drawing.Size(349, 100)
        Me.AGVBOX_B.TabIndex = 2
        Me.AGVBOX_B.Text = "B"
        '
        'AGVBOX_C
        '
        Me.AGVBOX_C.AutoSize = True
        Me.AGVBOX_C.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_C.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_C.Location = New System.Drawing.Point(718, 0)
        Me.AGVBOX_C.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_C.Name = "AGVBOX_C"
        Me.AGVBOX_C.Size = New System.Drawing.Size(350, 100)
        Me.AGVBOX_C.TabIndex = 5
        Me.AGVBOX_C.Text = "C"
        '
        'AGVBOX_D
        '
        Me.AGVBOX_D.AutoSize = True
        Me.AGVBOX_D.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_D.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_D.Location = New System.Drawing.Point(4, 100)
        Me.AGVBOX_D.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_D.Name = "AGVBOX_D"
        Me.AGVBOX_D.Size = New System.Drawing.Size(349, 100)
        Me.AGVBOX_D.TabIndex = 4
        Me.AGVBOX_D.Text = "D"
        '
        'AGVBOX_E
        '
        Me.AGVBOX_E.AutoSize = True
        Me.AGVBOX_E.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_E.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_E.Location = New System.Drawing.Point(361, 100)
        Me.AGVBOX_E.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_E.Name = "AGVBOX_E"
        Me.AGVBOX_E.Size = New System.Drawing.Size(349, 100)
        Me.AGVBOX_E.TabIndex = 3
        Me.AGVBOX_E.Text = "E"
        '
        'AGVBOX_F
        '
        Me.AGVBOX_F.AutoSize = True
        Me.AGVBOX_F.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_F.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_F.Location = New System.Drawing.Point(718, 100)
        Me.AGVBOX_F.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_F.Name = "AGVBOX_F"
        Me.AGVBOX_F.Size = New System.Drawing.Size(350, 100)
        Me.AGVBOX_F.TabIndex = 6
        Me.AGVBOX_F.Text = "F"
        '
        'AGVBOX_H
        '
        Me.AGVBOX_H.AutoSize = True
        Me.AGVBOX_H.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_H.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_H.Location = New System.Drawing.Point(361, 200)
        Me.AGVBOX_H.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_H.Name = "AGVBOX_H"
        Me.AGVBOX_H.Size = New System.Drawing.Size(349, 100)
        Me.AGVBOX_H.TabIndex = 9
        Me.AGVBOX_H.Text = "H"
        '
        'AGVBOX_G
        '
        Me.AGVBOX_G.AutoSize = True
        Me.AGVBOX_G.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AGVBOX_G.Font = New System.Drawing.Font("Microsoft JhengHei", 25.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AGVBOX_G.Location = New System.Drawing.Point(4, 200)
        Me.AGVBOX_G.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.AGVBOX_G.Name = "AGVBOX_G"
        Me.AGVBOX_G.Size = New System.Drawing.Size(349, 100)
        Me.AGVBOX_G.TabIndex = 7
        Me.AGVBOX_G.Text = "G"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(4, 168)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(120, 27)
        Me.Label21.TabIndex = 5
        Me.Label21.Text = "AGV密盆號"
        '
        'AGVBoxNo
        '
        Me.AGVBoxNo.Location = New System.Drawing.Point(0, 202)
        Me.AGVBoxNo.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.AGVBoxNo.Name = "AGVBoxNo"
        Me.AGVBoxNo.Size = New System.Drawing.Size(250, 35)
        Me.AGVBoxNo.TabIndex = 6
        '
        'AGVCarClose
        '
        Me.AGVCarClose.Location = New System.Drawing.Point(0, 255)
        Me.AGVCarClose.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.AGVCarClose.Name = "AGVCarClose"
        Me.AGVCarClose.Size = New System.Drawing.Size(252, 149)
        Me.AGVCarClose.TabIndex = 1
        Me.AGVCarClose.Text = "封車"
        Me.AGVCarClose.UseVisualStyleBackColor = True
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(4, 83)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(117, 27)
        Me.Label20.TabIndex = 2
        Me.Label20.Text = "台車格口號"
        '
        'CarLocNo
        '
        Me.CarLocNo.Location = New System.Drawing.Point(0, 118)
        Me.CarLocNo.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CarLocNo.Name = "CarLocNo"
        Me.CarLocNo.Size = New System.Drawing.Size(250, 35)
        Me.CarLocNo.TabIndex = 3
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(4, 0)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(117, 27)
        Me.Label19.TabIndex = 0
        Me.Label19.Text = "台車流水號"
        '
        'CarNo
        '
        Me.CarNo.Location = New System.Drawing.Point(0, 35)
        Me.CarNo.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CarNo.Name = "CarNo"
        Me.CarNo.Size = New System.Drawing.Size(250, 35)
        Me.CarNo.TabIndex = 1
        '
        'PrintDocument1
        '
        '
        'PrintDocument2
        '
        '
        'CASEDETAILBindingSource
        '
        Me.CASEDETAILBindingSource.DataMember = "CASEDETAIL"
        Me.CASEDETAILBindingSource.DataSource = Me.PRODDataSet
        '
        'CASEDETAILTableAdapter
        '
        Me.CASEDETAILTableAdapter.ClearBeforeFill = True
        '
        'Vw_PICKDETAIL_RateTableAdapter
        '
        Me.Vw_PICKDETAIL_RateTableAdapter.ClearBeforeFill = True
        '
        'Vw_Case_Addr_InvoiceTableAdapter
        '
        Me.Vw_Case_Addr_InvoiceTableAdapter.ClearBeforeFill = True
        '
        'Combination_CaseDetailTableAdapter
        '
        Me.Combination_CaseDetailTableAdapter.ClearBeforeFill = True
        '
        'AGV_PickCarListTableAdapter
        '
        Me.AGV_PickCarListTableAdapter.ClearBeforeFill = True
        '
        'Packet
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1374, 865)
        Me.Controls.Add(Me.TabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "Packet"
        Me.Text = "e2e_1024_v4"
        CType(Me.Vw_Case_Addr_InvoiceBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PRODDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AGVPickCarListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FWKDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.TabControl2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        CType(Me.DGV_PickDetail, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.VwPICKDETAILRateBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage4.ResumeLayout(False)
        CType(Me.DGV_CaseDetail, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CombinationCaseDetailBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage7.ResumeLayout(False)
        Me.TabPage8.ResumeLayout(False)
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage9.ResumeLayout(False)
        Me.SplitContainer4.Panel1.ResumeLayout(False)
        Me.SplitContainer4.Panel1.PerformLayout()
        Me.SplitContainer4.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer4.ResumeLayout(False)
        Me.SplitContainer5.Panel1.ResumeLayout(False)
        Me.SplitContainer5.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer5.ResumeLayout(False)
        CType(Me.DGV_reAddr, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer6.Panel1.ResumeLayout(False)
        Me.SplitContainer6.Panel1.PerformLayout()
        Me.SplitContainer6.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer6.ResumeLayout(False)
        CType(Me.DGV_reInvoice, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage2.ResumeLayout(False)
        CType(Me.DGV_WAVE, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.TabPage5.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.TabPage10.ResumeLayout(False)
        Me.TabPage10.PerformLayout()
        Me.TabPage11.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.CASEDETAILBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Btn_Close As System.Windows.Forms.Button
    Friend WithEvents Txt_Pcs As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents pokey_Text As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Txt_SKU As System.Windows.Forms.TextBox
    Friend WithEvents TabControl2 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents Btn_Close_test As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents DGV_PickDetail As System.Windows.Forms.DataGridView
    Friend WithEvents DGV_CaseDetail As System.Windows.Forms.DataGridView
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents TabPage7 As TabPage
    Friend WithEvents CASEDETAILBindingSource As BindingSource
    Friend WithEvents PRODDataSet As Packet_DEV.PRODDataSet
    Friend WithEvents CASEDETAILTableAdapter As Packet_DEV.PRODDataSetTableAdapters.CASEDETAILTableAdapter
    Friend WithEvents PrintDocument1 As Printing.PrintDocument
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents LAB_CASEID As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents TabPage8 As TabPage
    Friend WithEvents ReportViewer2 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents PrintDocument2 As Printing.PrintDocument
    Friend WithEvents VwPICKDETAILRateBindingSource As BindingSource
    Friend WithEvents Vw_PICKDETAIL_RateTableAdapter As PRODDataSetTableAdapters.Vw_PICKDETAIL_RateTableAdapter
    Friend WithEvents rePack_Btn As Button
    Friend WithEvents Vw_Case_Addr_InvoiceTableAdapter As PRODDataSetTableAdapters.Vw_Case_Addr_InvoiceTableAdapter
    Friend WithEvents Vw_Case_Addr_InvoiceBindingSource As BindingSource
    Friend WithEvents TabPage9 As TabPage
    Friend WithEvents SplitContainer4 As SplitContainer
    Friend WithEvents repokey_Text As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents SplitContainer5 As SplitContainer
    Friend WithEvents Label15 As Label
    Friend WithEvents SplitContainer6 As SplitContainer
    Friend WithEvents Label16 As Label
    Friend WithEvents CombinationCaseDetailBindingSource As BindingSource
    Friend WithEvents Combination_CaseDetailTableAdapter As PRODDataSetTableAdapters.Combination_CaseDetailTableAdapter
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents DGV_WAVE As DataGridView
    Friend WithEvents Panel1 As Panel
    Friend WithEvents wavekey_Text As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TabPage5 As TabPage
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label9 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents UploadCase_txt As TextBox
    Friend WithEvents UploadPo_txt As TextBox
    Friend WithEvents poInfo_lb As Label
    Friend WithEvents TabPage6 As TabPage
    Friend WithEvents ReportViewer3 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents TabPage10 As System.Windows.Forms.TabPage
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label18 As Label
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents AGV_Txt As System.Windows.Forms.TextBox
    Friend WithEvents RadioButton3 As System.Windows.Forms.RadioButton
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents TabPage11 As System.Windows.Forms.TabPage
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents ReportViewer4 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents CarNo As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents CarLocNo As System.Windows.Forms.TextBox
    Friend WithEvents AGVCarClose As System.Windows.Forms.Button
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents AGVBoxNo As System.Windows.Forms.TextBox
    Friend WithEvents AGVBOX_J As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_I As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_L As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_K As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_A As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_B As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_C As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_D As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_E As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_F As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_G As System.Windows.Forms.Label
    Friend WithEvents AGVBOX_H As System.Windows.Forms.Label
    Friend WithEvents FWKDataSet As Packet_DEV.FWKDataSet
    Friend WithEvents AGVPickCarListBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents AGV_PickCarListTableAdapter As Packet_DEV.FWKDataSetTableAdapters.AGV_PickCarListTableAdapter
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents STORERKEYDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents ROUTEDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents ORDERKEYDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents ORDERLINENUMBERDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents SKUDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PICKQTYDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PACKQTYDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents DESCRDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents BUSR3DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents BUSR2DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents CASEIDDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents QTYDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents LOCDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents STATUSDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PARTDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PrintMarkDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents WAVEKEYDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents EDITDATEDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PICKDETAILKEYDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents NOTESDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents EXTERNORDERKEYDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents ItemNoDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents StorerkeyDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents ROUTEDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents OrderkeyDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents OrderkeyLineNumberDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents DescrDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents SkuDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents DOORDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents LocDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents QTYDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents GUINODataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents NOTESDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents ExternorderkeyDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents WAVEKEYDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents EDITDATEDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents EDITWHODataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents PICKDETAILKEYDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents CURCYDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents TAX01DataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents PRICEDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents UNITPRICEDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents Recovery As DataGridViewButtonColumn
    Friend WithEvents DGV_reAddr As DataGridView
    Friend WithEvents addrPrintBtn As DataGridViewButtonColumn
    Friend WithEvents DGV_reInvoice As DataGridView
    Friend WithEvents invicePrintBtn As DataGridViewButtonColumn
    Friend WithEvents CheckBox2 As CheckBox
End Class
