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
    public class GradientButton : Button
    {
        private Brush gradientBrush;

        public GradientButton()
        {
            handlerGradientChanged = new EventHandler(GradientChanged);
            ResizeRedraw = true;
        }

        private EventHandler handlerGradientChanged;

        // encapsulated data
        private List<Color> m_clrColors = new List<Color>();
        private int m_nTransparency = 64;
        private float m_fAngle = 0.0f;

        // The properties
        // transparency value for the button gradiants
        public int Transparency
        {
            get
            {
                return m_nTransparency;
            }
            set
            {
                m_nTransparency = value;
                Invalidate();
            }
        }

        // angle for the gradiant from the x-axis
        public float Angle
        {
            get
            {
                return m_fAngle;
            }
            set
            {
                m_fAngle = value;
                Invalidate();
            }
        }
      
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        EditorAttribute("typeof(CollectionEditor)", "typeof(System.Drawing.Design.UITypeEditor)")]
        public List<Color> Colors
        {
            get
            {
                return m_clrColors;
            }

        }
              
        protected override void OnPaint(PaintEventArgs pe)
        {   
            base.OnPaint(pe);
           
            if (m_clrColors.Count > 1)
            {                
                float percentage = Convert.ToSingle(ClientRectangle.Width - 2) * (1.0f / Convert.ToSingle(m_clrColors.Count - 1));
                Brush b;
                RectangleF rectF;
                Color c1;
                Color c2;
               
                rectF = new RectangleF(new PointF(0.0f, 1.0f), new SizeF(percentage, ClientRectangle.Height - 2));
                for (int i = 0; i < m_clrColors.Count - 1; i++)
                {                  
                    c1 = Color.FromArgb(m_nTransparency, m_clrColors[i]);
                    c2 = Color.FromArgb(m_nTransparency, m_clrColors[i + 1]);

                    b = new System.Drawing.Drawing2D.LinearGradientBrush(rectF, c1, c2, m_fAngle);                   
                    pe.Graphics.FillRectangle(b, rectF);
                    rectF.Offset(percentage, 0);
                    b.Dispose();
                }
            }
        }

        private void GradientChanged(object sender, EventArgs e)
        {
            if (gradientBrush != null) gradientBrush.Dispose();
            gradientBrush = null;
            Invalidate();
        }
    }
}