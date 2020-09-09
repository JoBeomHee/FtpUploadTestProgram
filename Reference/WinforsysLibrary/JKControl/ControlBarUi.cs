using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Winforsys.JKControl
{
    public partial class ControlBarUi : UserControl
    {
        [Browsable(true)]
        public String TitleText
        {
            get
            {                
                return this.uiLab_Title.Text;
            }
            
            set
            {
                uiLab_Title.Text = value;
            }
        }        

        [Browsable(true)]
        public Icon Icon
        {
            get
            {
                if (this.ParentForm != null)
                    return this.ParentForm.Icon;
                else
                    return null;
            }

            set
            {
                if (this.ParentForm != null)
                {
                    this.ParentForm.Icon = value;
                    this.uiPic_Icon.Image = this.ParentForm.Icon.ToBitmap();
                }
            }
        }

        public ControlBarUi()
        {
            InitializeComponent();
            
            SetFormMove();

            this.Load += ControlBarUi_Load;

            uiBtn_Min.Click += uiBtn_Min_Click;
            uiBtn_Close.Click += uiBtn_Close_Click;
        }

        void uiBtn_Min_Click(object sender, EventArgs e)
        {
            this.ParentForm.WindowState = FormWindowState.Minimized;
        }

        void ControlBarUi_Load(object sender, EventArgs e)
        {
            if (this.Parent != null)
                uiPic_Icon.Image = this.ParentForm.Icon.ToBitmap();
        }

        void uiBtn_Close_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }

        #region " Form Move "        
        private Point mCurrentPosition = new Point(0, 0);

        private void SetFormMove()
        {
            this.Cursor = Cursors.SizeAll;
            
            this.tableLayoutPanel1.MouseDown += ControlBarUi_MouseDown;
            this.tableLayoutPanel1.MouseMove += ControlBarUi_MouseMove;
            this.uiLab_Title.MouseDown += ControlBarUi_MouseDown;
            this.uiLab_Title.MouseMove += ControlBarUi_MouseMove;
        }

        void ControlBarUi_MouseMove(object sender, MouseEventArgs e)
        {          
            if (e.Button == MouseButtons.Left)
            {
                this.ParentForm.Location = new Point(
                    this.ParentForm.Location.X + (mCurrentPosition.X + e.X),
                    this.ParentForm.Location.Y + (mCurrentPosition.Y + e.Y));
            }
        }

        void ControlBarUi_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mCurrentPosition = new Point(-e.X, -e.Y);
        }

        #endregion " Form Move "

    }
}
