namespace Winforsys.JKControl
{
    partial class ControlBarUi
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlBarUi));
            this.uiLab_Title = new System.Windows.Forms.Label();
            this.uiBtn_Min = new System.Windows.Forms.Button();
            this.uiBtn_Max = new System.Windows.Forms.Button();
            this.uiBtn_Close = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.uiPic_Icon = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiPic_Icon)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiLab_Title
            // 
            this.uiLab_Title.AutoSize = true;
            this.uiLab_Title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLab_Title.ForeColor = System.Drawing.Color.White;
            this.uiLab_Title.Location = new System.Drawing.Point(53, 0);
            this.uiLab_Title.Name = "uiLab_Title";
            this.uiLab_Title.Size = new System.Drawing.Size(197, 34);
            this.uiLab_Title.TabIndex = 12;
            this.uiLab_Title.Text = "Title";
            this.uiLab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiBtn_Min
            // 
            this.uiBtn_Min.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uiBtn_Min.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiBtn_Min.Image = ((System.Drawing.Image)(resources.GetObject("uiBtn_Min.Image")));
            this.uiBtn_Min.Location = new System.Drawing.Point(4, 3);
            this.uiBtn_Min.Name = "uiBtn_Min";
            this.uiBtn_Min.Size = new System.Drawing.Size(32, 26);
            this.uiBtn_Min.TabIndex = 9;
            this.uiBtn_Min.UseVisualStyleBackColor = true;
            // 
            // uiBtn_Max
            // 
            this.uiBtn_Max.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uiBtn_Max.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiBtn_Max.Image = ((System.Drawing.Image)(resources.GetObject("uiBtn_Max.Image")));
            this.uiBtn_Max.Location = new System.Drawing.Point(39, 3);
            this.uiBtn_Max.Name = "uiBtn_Max";
            this.uiBtn_Max.Size = new System.Drawing.Size(32, 26);
            this.uiBtn_Max.TabIndex = 10;
            this.uiBtn_Max.UseVisualStyleBackColor = true;
            // 
            // uiBtn_Close
            // 
            this.uiBtn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uiBtn_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiBtn_Close.Image = ((System.Drawing.Image)(resources.GetObject("uiBtn_Close.Image")));
            this.uiBtn_Close.Location = new System.Drawing.Point(72, 3);
            this.uiBtn_Close.Name = "uiBtn_Close";
            this.uiBtn_Close.Size = new System.Drawing.Size(32, 26);
            this.uiBtn_Close.TabIndex = 11;
            this.uiBtn_Close.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 107F));
            this.tableLayoutPanel1.Controls.Add(this.uiPic_Icon, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.uiLab_Title, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(360, 34);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // uiPic_Icon
            // 
            this.uiPic_Icon.BackColor = System.Drawing.Color.Transparent;
            this.uiPic_Icon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPic_Icon.Location = new System.Drawing.Point(2, 2);
            this.uiPic_Icon.Margin = new System.Windows.Forms.Padding(2);
            this.uiPic_Icon.Name = "uiPic_Icon";
            this.uiPic_Icon.Size = new System.Drawing.Size(46, 30);
            this.uiPic_Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.uiPic_Icon.TabIndex = 9;
            this.uiPic_Icon.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.uiBtn_Close);
            this.panel1.Controls.Add(this.uiBtn_Min);
            this.panel1.Controls.Add(this.uiBtn_Max);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(253, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(107, 34);
            this.panel1.TabIndex = 13;
            // 
            // ControlBarUi
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ControlBarUi";
            this.Size = new System.Drawing.Size(360, 34);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiPic_Icon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label uiLab_Title;
        public System.Windows.Forms.Button uiBtn_Min;
        public System.Windows.Forms.Button uiBtn_Max;
        public System.Windows.Forms.Button uiBtn_Close;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox uiPic_Icon;
        private System.Windows.Forms.Panel panel1;

    }
}
