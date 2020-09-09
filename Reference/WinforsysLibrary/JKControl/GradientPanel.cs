using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace Winforsys.JKControl
{
    public class GradientPanel : Panel
    {
        [Browsable(true)]
        public Color GraColorA { get; set; }

        [Browsable(true)]
        public Color GraColorB { get; set; }

        [Browsable(true)]
        public LinearGradientMode GradientFillStyle { get; set; }

        private Brush gradientBrush;

        public GradientPanel()
        {
            handlerGradientChanged = new EventHandler(GradientChanged);
            ResizeRedraw = true;
        }

        private EventHandler handlerGradientChanged;

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            gradientBrush = new LinearGradientBrush(ClientRectangle, GraColorA, GraColorB, GradientFillStyle);

            e.Graphics.FillRectangle(gradientBrush, ClientRectangle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (gradientBrush != null) gradientBrush.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            Invalidate();
        }

        private void GradientChanged(object sender, EventArgs e)
        {
            if (gradientBrush != null) gradientBrush.Dispose();
            gradientBrush = null;
            Invalidate();
        }
    }

}
