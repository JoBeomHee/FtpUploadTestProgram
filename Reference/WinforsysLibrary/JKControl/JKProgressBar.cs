using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Winforsys.JKControl
{
    public class JKProgressBar : ProgressBar
    {
        public Brush brush;

        private LinearGradientBrush graBrushBack;
        private LinearGradientBrush graBrushBar;
        
        public JKProgressBar()
        {  
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            brush = Brushes.LimeGreen;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap memBitMap = new Bitmap(this.Width, this.Height);

            Rectangle barRect = e.ClipRectangle;
            Rectangle controlRect = e.ClipRectangle;
            barRect.Width = (int)(barRect.Width * ((double)Value / Maximum));
            barRect.Height = Height;
          
            graBrushBack = new LinearGradientBrush(controlRect, Color.Silver, Color.White, LinearGradientMode.Horizontal);
            graBrushBar = new LinearGradientBrush(controlRect, Color.LightSkyBlue, Color.SteelBlue, LinearGradientMode.Horizontal);

            e.Graphics.FillRectangle(graBrushBack, controlRect);
            e.Graphics.FillRectangle(graBrushBar, 0, 0, barRect.Width, barRect.Height);

            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            strFormat.LineAlignment = StringAlignment.Center;
            string valueStr = string.Format("{0} / {1}", Value, Maximum);
            e.Graphics.DrawString(valueStr, this.Parent.Font, Brushes.Red, controlRect, strFormat);

            ////Double Buffering
            //using (Graphics g = this.CreateGraphics())
            //{
            //    Bitmap memBitMap = new Bitmap(this.Width, this.Height);

            //    Rectangle barRect = e.ClipRectangle;
            //    Rectangle controlRect = e.ClipRectangle;
            //    controlRect.Width = controlRect.Width + 1;
            //    controlRect.Height = controlRect.Height + 1;

            //    barRect.Width = (int)(barRect.Width * ((double)Value / Maximum)) - 4;
            //    //ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            //    ProgressBarRenderer.DrawHorizontalBar(e.Graphics, controlRect);
            //    barRect.Height = Height - 4;
            //    e.Graphics.FillRectangle(brush, 2, 2, barRect.Width, barRect.Height);

            //    StringFormat strFormat = new StringFormat();
            //    strFormat.Alignment = StringAlignment.Center;
            //    strFormat.LineAlignment = StringAlignment.Center;
            //    string valueStr = string.Format("{0} / {1}", Value, Maximum);
            //    e.Graphics.DrawString(valueStr, this.Parent.Font, Brushes.Red, controlRect, strFormat);

            //    g.DrawImageUnscaled(memBitMap, 0, 0);
            //    memBitMap.Dispose();
            //}
        }
    }
}
