namespace UC_test
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.sM200B1 = new SM200Bx64.SM200B();
            this.SuspendLayout();
            // 
            // sM200B1
            // 
            this.sM200B1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sM200B1.Location = new System.Drawing.Point(0, 0);
            this.sM200B1.Name = "sM200B1";
            this.sM200B1.Size = new System.Drawing.Size(445, 279);
            this.sM200B1.TabIndex = 0;
            this.sM200B1.是否显示菜单 = true;
            this.sM200B1.自动连接启用 = true;
            this.sM200B1.调用类库位数 = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 279);
            this.Controls.Add(this.sM200B1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SM200Bx64.SM200B sM200B1;
    }
}

