<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Packet
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
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Btn_Close = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.Txt_Weight = New System.Windows.Forms.TextBox()
        Me.Txt_Pcs = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Txt_SKU = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Txt_Ext = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RadioButton2 = New System.Windows.Forms.RadioButton()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TabControl2 = New System.Windows.Forms.TabControl()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.DGV_PickDetail = New System.Windows.Forms.DataGridView()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.DGV_CaseDetail = New System.Windows.Forms.DataGridView()
        Me.Recovery = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabControl3 = New System.Windows.Forms.TabControl()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
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
        Me.TabPage4.SuspendLayout()
        CType(Me.DGV_CaseDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage2.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.TabControl3.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Font = New System.Drawing.Font("微軟正黑體", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(926, 604)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.SplitContainer1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 52)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(918, 548)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "包裝作業"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 3)
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
        Me.SplitContainer1.Size = New System.Drawing.Size(912, 542)
        Me.SplitContainer1.SplitterDistance = 111
        Me.SplitContainer1.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.FloralWhite
        Me.GroupBox2.Controls.Add(Me.Btn_Close)
        Me.GroupBox2.Controls.Add(Me.Button1)
        Me.GroupBox2.Controls.Add(Me.CheckBox1)
        Me.GroupBox2.Controls.Add(Me.Txt_Weight)
        Me.GroupBox2.Controls.Add(Me.Txt_Pcs)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.Txt_SKU)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Txt_Ext)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox2.Location = New System.Drawing.Point(0, -90)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(912, 201)
        Me.GroupBox2.TabIndex = 3
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "作業資訊"
        '
        'Btn_Close
        '
        Me.Btn_Close.Location = New System.Drawing.Point(332, 26)
        Me.Btn_Close.Name = "Btn_Close"
        Me.Btn_Close.Size = New System.Drawing.Size(97, 30)
        Me.Btn_Close.TabIndex = 6
        Me.Btn_Close.Text = "單據結束"
        Me.Btn_Close.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(332, 120)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(97, 30)
        Me.Button1.TabIndex = 6
        Me.Button1.Text = "封箱"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.BackColor = System.Drawing.Color.Yellow
        Me.CheckBox1.Checked = True
        Me.CheckBox1.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox1.Location = New System.Drawing.Point(270, 174)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(126, 47)
        Me.CheckBox1.TabIndex = 98
        Me.CheckBox1.Text = "鎖定"
        Me.CheckBox1.UseVisualStyleBackColor = False
        '
        'Txt_Weight
        '
        Me.Txt_Weight.Location = New System.Drawing.Point(111, 120)
        Me.Txt_Weight.Name = "Txt_Weight"
        Me.Txt_Weight.Size = New System.Drawing.Size(153, 53)
        Me.Txt_Weight.TabIndex = 4
        '
        'Txt_Pcs
        '
        Me.Txt_Pcs.Location = New System.Drawing.Point(111, 171)
        Me.Txt_Pcs.MaxLength = 4
        Me.Txt_Pcs.Name = "Txt_Pcs"
        Me.Txt_Pcs.Size = New System.Drawing.Size(153, 53)
        Me.Txt_Pcs.TabIndex = 5
        Me.Txt_Pcs.Text = "1"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(10, 81)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(189, 43)
        Me.Label13.TabIndex = 99
        Me.Label13.Text = "商品條碼："
        '
        'Txt_SKU
        '
        Me.Txt_SKU.Location = New System.Drawing.Point(111, 73)
        Me.Txt_SKU.Name = "Txt_SKU"
        Me.Txt_SKU.Size = New System.Drawing.Size(153, 53)
        Me.Txt_SKU.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 34)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(189, 43)
        Me.Label2.TabIndex = 99
        Me.Label2.Text = "單據號碼："
        '
        'Txt_Ext
        '
        Me.Txt_Ext.Location = New System.Drawing.Point(111, 26)
        Me.Txt_Ext.MaxLength = 10
        Me.Txt_Ext.Name = "Txt_Ext"
        Me.Txt_Ext.Size = New System.Drawing.Size(153, 53)
        Me.Txt_Ext.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 175)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(189, 43)
        Me.Label3.TabIndex = 99
        Me.Label3.Text = "數量鍵入："
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(10, 128)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(189, 43)
        Me.Label4.TabIndex = 99
        Me.Label4.Text = "重量鍵入："
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FloralWhite
        Me.GroupBox1.Controls.Add(Me.RadioButton2)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(912, 65)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "作業模式"
        '
        'RadioButton2
        '
        Me.RadioButton2.AutoSize = True
        Me.RadioButton2.Checked = True
        Me.RadioButton2.Location = New System.Drawing.Point(14, 20)
        Me.RadioButton2.Name = "RadioButton2"
        Me.RadioButton2.Size = New System.Drawing.Size(193, 47)
        Me.RadioButton2.TabIndex = 99
        Me.RadioButton2.TabStop = True
        Me.RadioButton2.Text = "單據作業"
        Me.RadioButton2.UseVisualStyleBackColor = True
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
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
        Me.SplitContainer2.Size = New System.Drawing.Size(912, 427)
        Me.SplitContainer2.SplitterDistance = 47
        Me.SplitContainer2.TabIndex = 0
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.LightGray
        Me.GroupBox3.Controls.Add(Me.Label12)
        Me.GroupBox3.Controls.Add(Me.Label11)
        Me.GroupBox3.Controls.Add(Me.Label10)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Controls.Add(Me.Label8)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox3.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(912, 47)
        Me.GroupBox3.TabIndex = 0
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "訂單資訊"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.AntiqueWhite
        Me.Label12.Location = New System.Drawing.Point(582, 26)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(87, 43)
        Me.Label12.TabIndex = 1
        Me.Label12.Text = "－－"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.AntiqueWhite
        Me.Label11.Location = New System.Drawing.Point(420, 26)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(87, 43)
        Me.Label11.TabIndex = 1
        Me.Label11.Text = "－－"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.AntiqueWhite
        Me.Label10.Location = New System.Drawing.Point(254, 26)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(87, 43)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "－－"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.AntiqueWhite
        Me.Label9.Location = New System.Drawing.Point(91, 26)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(87, 43)
        Me.Label9.TabIndex = 1
        Me.Label9.Text = "－－"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(510, 26)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(155, 43)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "檢核量："
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(341, 26)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(176, 43)
        Me.Label7.TabIndex = 0
        Me.Label7.Text = "總pcs數："
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(170, 26)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(189, 43)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "總品項數："
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(7, 26)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(189, 43)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "客戶名稱："
        '
        'TabControl2
        '
        Me.TabControl2.Controls.Add(Me.TabPage3)
        Me.TabControl2.Controls.Add(Me.TabPage4)
        Me.TabControl2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl2.Location = New System.Drawing.Point(0, 0)
        Me.TabControl2.Name = "TabControl2"
        Me.TabControl2.SelectedIndex = 0
        Me.TabControl2.Size = New System.Drawing.Size(912, 376)
        Me.TabControl2.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.DGV_PickDetail)
        Me.TabPage3.Location = New System.Drawing.Point(4, 52)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(894, 169)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "包裝清單"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'DGV_PickDetail
        '
        Me.DGV_PickDetail.AllowUserToAddRows = False
        Me.DGV_PickDetail.AllowUserToDeleteRows = False
        Me.DGV_PickDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DGV_PickDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV_PickDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGV_PickDetail.Location = New System.Drawing.Point(3, 3)
        Me.DGV_PickDetail.Name = "DGV_PickDetail"
        Me.DGV_PickDetail.ReadOnly = True
        Me.DGV_PickDetail.RowTemplate.Height = 24
        Me.DGV_PickDetail.Size = New System.Drawing.Size(888, 163)
        Me.DGV_PickDetail.TabIndex = 0
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.DGV_CaseDetail)
        Me.TabPage4.Location = New System.Drawing.Point(4, 52)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(904, 320)
        Me.TabPage4.TabIndex = 1
        Me.TabPage4.Text = "作業箱明細"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'DGV_CaseDetail
        '
        Me.DGV_CaseDetail.AllowUserToAddRows = False
        Me.DGV_CaseDetail.AllowUserToDeleteRows = False
        Me.DGV_CaseDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV_CaseDetail.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Recovery})
        Me.DGV_CaseDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGV_CaseDetail.Location = New System.Drawing.Point(3, 3)
        Me.DGV_CaseDetail.Name = "DGV_CaseDetail"
        Me.DGV_CaseDetail.ReadOnly = True
        Me.DGV_CaseDetail.RowTemplate.Height = 24
        Me.DGV_CaseDetail.Size = New System.Drawing.Size(898, 314)
        Me.DGV_CaseDetail.TabIndex = 0
        '
        'Recovery
        '
        Me.Recovery.HeaderText = "回收"
        Me.Recovery.Name = "Recovery"
        Me.Recovery.ReadOnly = True
        Me.Recovery.Text = ""
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.SplitContainer3)
        Me.TabPage2.Location = New System.Drawing.Point(4, 52)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(908, 548)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "台車列印"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.BackColor = System.Drawing.Color.FloralWhite
        Me.SplitContainer3.Panel1.Controls.Add(Me.TextBox1)
        Me.SplitContainer3.Panel1.Controls.Add(Me.Label1)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.TabControl3)
        Me.SplitContainer3.Size = New System.Drawing.Size(902, 542)
        Me.SplitContainer3.SplitterDistance = 108
        Me.SplitContainer3.TabIndex = 0
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(124, 20)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(153, 53)
        Me.TextBox1.TabIndex = 9
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(23, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(189, 43)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "台車編號："
        '
        'TabControl3
        '
        Me.TabControl3.Controls.Add(Me.TabPage5)
        Me.TabControl3.Controls.Add(Me.TabPage6)
        Me.TabControl3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl3.Location = New System.Drawing.Point(0, 0)
        Me.TabControl3.Name = "TabControl3"
        Me.TabControl3.SelectedIndex = 0
        Me.TabControl3.Size = New System.Drawing.Size(902, 430)
        Me.TabControl3.TabIndex = 0
        '
        'TabPage5
        '
        Me.TabPage5.Location = New System.Drawing.Point(4, 52)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(894, 374)
        Me.TabPage5.TabIndex = 0
        Me.TabPage5.Text = "地址條"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'TabPage6
        '
        Me.TabPage6.Location = New System.Drawing.Point(4, 52)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(894, 374)
        Me.TabPage6.TabIndex = 1
        Me.TabPage6.Text = "揀取單"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'Packet
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(916, 604)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "Packet"
        Me.Text = "包裝作業"
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
        Me.TabPage4.ResumeLayout(False)
        CType(Me.DGV_CaseDetail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage2.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel1.PerformLayout()
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.TabControl3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Txt_Weight As System.Windows.Forms.TextBox
    Friend WithEvents Txt_Pcs As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Txt_Ext As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Txt_SKU As System.Windows.Forms.TextBox
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TabControl2 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents Btn_Close As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabControl3 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents DGV_PickDetail As System.Windows.Forms.DataGridView
    Friend WithEvents DGV_CaseDetail As System.Windows.Forms.DataGridView
    Friend WithEvents Recovery As DataGridViewButtonColumn
End Class
