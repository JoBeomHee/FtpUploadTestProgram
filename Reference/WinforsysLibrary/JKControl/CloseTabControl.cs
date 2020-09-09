using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;

namespace Winforsys.JKControl
{
    public class CloseTabControl : TabControl 
    {
        private System.ComponentModel.Container components = null;
        private List<Rectangle> cBtnRectList = new List<Rectangle>();

        public CloseTabControl()
        {
            //Initialzation
            components = new System.ComponentModel.Container();
            SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer, true);
            this.TabStop = false;
            this.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;

            this.ItemSize = new System.Drawing.Size(230, 24);
            this.SizeMode = TabSizeMode.Normal;
        }

        void CloseTabControl_ControlAdded(object sender, ControlEventArgs e)
        {

        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            //int idx = this.TabPages.Count - 1;

            //Rectangle txtRect = this.GetTabRect(idx);

            cBtnRectList.Clear();

            if (this.TabCount == 0) return;

            using (Graphics g = this.CreateGraphics())
            {
                for (int i = 0; i < this.TabCount; ++i)
                {
                    Rectangle txtRect = this.GetTabRect(i);

                    if (i != 0)
                    {
                        Rectangle cBtnRect = new Rectangle(txtRect.X + txtRect.Width - 20, 5, 15, 15);

                        cBtnRectList.Add(cBtnRect);
                        Bitmap bmp = new Bitmap(Properties.Resources.RedCross);
                        g.DrawImage(bmp, cBtnRect);
                    }

                    if (this.SelectedIndex == i)
                    {
                        g.DrawRectangle(Pens.SteelBlue, txtRect);
                    }
                    else
                    {
                        g.DrawRectangle(Pens.White, txtRect);
                    }
                    
                    string str = this.TabPages[i].Text;
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(str, this.Font, new SolidBrush(this.TabPages[i].ForeColor), txtRect, stringFormat);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            for (int i = 0; i < cBtnRectList.Count; ++i)
            {
                if (cBtnRectList[i].Contains(e.Location))
                {
                    this.Cursor = Cursors.Hand;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            for (int i = 0; i < cBtnRectList.Count; ++i)
            {
                if (cBtnRectList[i].Contains(e.Location))
                {
                    if (MessageBox.Show("Close this tabpage?", "Inform", MessageBoxButtons.OK, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        this.TabPages.RemoveAt(i + 1);
                    }
                }               
            }

            base.OnMouseClick(e);
        }
    }
}
