<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
        Me.p_btn = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pokey_Lb = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Txt_count = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'p_btn
        '
        Me.p_btn.Location = New System.Drawing.Point(103, 160)
        Me.p_btn.Name = "p_btn"
        Me.p_btn.Size = New System.Drawing.Size(75, 23)
        Me.p_btn.TabIndex = 0
        Me.p_btn.Text = "確定"
        Me.p_btn.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 60)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "件數："
        '
        'pokey_Lb
        '
        Me.pokey_Lb.AutoSize = True
        Me.pokey_Lb.Location = New System.Drawing.Point(82, 18)
        Me.pokey_Lb.Name = "pokey_Lb"
        Me.pokey_Lb.Size = New System.Drawing.Size(13, 12)
        Me.pokey_Lb.TabIndex = 108
        Me.pokey_Lb.Text = "N"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 18)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(41, 12)
        Me.Label2.TabIndex = 107
        Me.Label2.Text = "單號："
        '
        'Txt_count
        '
        Me.Txt_count.Location = New System.Drawing.Point(84, 57)
        Me.Txt_count.Name = "Txt_count"
        Me.Txt_count.Size = New System.Drawing.Size(153, 22)
        Me.Txt_count.TabIndex = 109
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 261)
        Me.Controls.Add(Me.Txt_count)
        Me.Controls.Add(Me.pokey_Lb)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.p_btn)
        Me.Name = "Form2"
        Me.Text = "件數確認"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents p_btn As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents pokey_Lb As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Txt_count As TextBox
End Class
