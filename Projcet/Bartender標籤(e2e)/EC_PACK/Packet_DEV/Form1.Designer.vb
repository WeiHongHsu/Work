<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
    '請勿使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Txt_Weight = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.w_btn = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.wpokey_Lb = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.wcaseID_Lb = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Cb_packmod = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'Txt_Weight
        '
        Me.Txt_Weight.Location = New System.Drawing.Point(112, 91)
        Me.Txt_Weight.Name = "Txt_Weight"
        Me.Txt_Weight.Size = New System.Drawing.Size(153, 22)
        Me.Txt_Weight.TabIndex = 100
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 94)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 12)
        Me.Label4.TabIndex = 101
        Me.Label4.Text = "重量鍵入(KG)："
        '
        'w_btn
        '
        Me.w_btn.Location = New System.Drawing.Point(98, 213)
        Me.w_btn.Name = "w_btn"
        Me.w_btn.Size = New System.Drawing.Size(75, 23)
        Me.w_btn.TabIndex = 102
        Me.w_btn.Text = "確定"
        Me.w_btn.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 21)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(41, 12)
        Me.Label2.TabIndex = 105
        Me.Label2.Text = "單號："
        '
        'wpokey_Lb
        '
        Me.wpokey_Lb.AutoSize = True
        Me.wpokey_Lb.Location = New System.Drawing.Point(110, 21)
        Me.wpokey_Lb.Name = "wpokey_Lb"
        Me.wpokey_Lb.Size = New System.Drawing.Size(13, 12)
        Me.wpokey_Lb.TabIndex = 106
        Me.wpokey_Lb.Text = "N"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 56)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 12)
        Me.Label1.TabIndex = 107
        Me.Label1.Text = "箱號："
        '
        'wcaseID_Lb
        '
        Me.wcaseID_Lb.AutoSize = True
        Me.wcaseID_Lb.Location = New System.Drawing.Point(110, 56)
        Me.wcaseID_Lb.Name = "wcaseID_Lb"
        Me.wcaseID_Lb.Size = New System.Drawing.Size(13, 12)
        Me.wcaseID_Lb.TabIndex = 108
        Me.wcaseID_Lb.Text = "N"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 132)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(53, 12)
        Me.Label3.TabIndex = 109
        Me.Label3.Text = "箱類型："
        '
        'Cb_packmod
        '
        Me.Cb_packmod.FormattingEnabled = True
        Me.Cb_packmod.Items.AddRange(New Object() {"袋", "迷你、小、中、十字", "大"})
        Me.Cb_packmod.Location = New System.Drawing.Point(112, 129)
        Me.Cb_packmod.Name = "Cb_packmod"
        Me.Cb_packmod.Size = New System.Drawing.Size(121, 20)
        Me.Cb_packmod.TabIndex = 110
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 261)
        Me.Controls.Add(Me.Cb_packmod)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.wcaseID_Lb)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.wpokey_Lb)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.w_btn)
        Me.Controls.Add(Me.Txt_Weight)
        Me.Controls.Add(Me.Label4)
        Me.Name = "Form1"
        Me.Text = "重量確認"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Txt_Weight As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents w_btn As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents wpokey_Lb As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents wcaseID_Lb As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Cb_packmod As ComboBox
End Class
