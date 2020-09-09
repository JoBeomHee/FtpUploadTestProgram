using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Winforsys.JKControl.MapControl
{
    public partial class GlassMap
    {
        public class RAW_INFO
        {
            public Size MapSize { get; set; }
            public Size GlassSize { get; set; }
            public Size PnlSize { get; set; }

            public Point PnlCount { get; set; }
        }

        public class GLS_INFO
        {
            public string GLS_ID { get; set; }
            public string Product_ID { get; set; }

            public RAW_INFO RawInfo { get; set; }

            public Size MapSize { get; set; }
            public Size GlassSize { get; set; }

            public Point MapOrigin { get; set; }
            public Point GlassOrigin { get; set; }

            public List<PNL_INFO> PNL_List = new List<PNL_INFO>();
            public List<DEF_INFO> DEF_List = new List<DEF_INFO>();
            public List<BBL_INFO> BBL_List = new List<BBL_INFO>();
            public MapType MapType { get; set; }

            public GLS_INFO()
            {
                RawInfo = new RAW_INFO();
                PNL_List = new List<PNL_INFO>();
                DEF_List = new List<DEF_INFO>();
                MapType = MapType.DataX_GateY;
            }
        }

        public class PNL_INFO
        {
            public string PNL_ID { get; set; }
            public Size MapSize { get; set; }
            public Point MapOrigin { get; set; }

            public Size PnlSize { get; set; }
            public Point PnlOrigin { get; set; }
            public Point Pnl_A_Origin { get; set; }

            public string Point_ID { get; set; }
        }

        public class DEF_INFO
        {
            public Point Raw_Pos { get; set; }

            public int DEF_No { get; set; }
            public string DEF_Name { get; set; }
            public Point DEF_Pos { get; set; }
            public Size DEF_Size { get; set; }
            public Color Color { get; set; }

            public bool Visible { get; set; }
            public string Point_ID { get; set; }
            public Rectangle Rect { get; set; }

            public object Data { get; set; }            
        }

        public class BBL_INFO
        {
            public string PNL_ID { get; set; }
            public Rectangle Rect { get; set; }
            public Rectangle BBL_Rect { get; set; }
            public int DEF_Count { get; set; }
        }
    }
}
