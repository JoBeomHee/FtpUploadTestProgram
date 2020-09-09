using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Winforsys.JKControl.MapControl
{
    public enum BubbleType { Point, Panel }
    public enum MapType { DataX_GateY, GateX_DataY }

    public class DefectPointEventArgs : EventArgs
    {
        public List<int> PointList { get; set; }
    }    

    public partial class GlassMap : UserControl
    {
        #region " Properties & Variables "

        public GLS_INFO Info
        {
            get
            {
                return glsInfo;
            }

            set
            {
                glsInfo = value;
                this.ErrorMsg = string.Empty;

                InitDrawData();
                this.Refresh();
            }
        }

        public event EventHandler<DefectPointEventArgs> DefectClickEvent;

        protected virtual void OnDefectClickEvent(DefectPointEventArgs e)
        {
            EventHandler<DefectPointEventArgs> handler = DefectClickEvent;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public bool IsGroup { get; set; }
        public bool IsBubbleMap { get; set; }
        public bool IsWhiteType { get; set; }
        public bool PanelOrigin { get; set; }

        public BubbleType Bubble_Type { get; set; }
        public int Bubble_Total { get; set; }
        public int Bubble_Max { get; set; }

        public static Color DefaultDefectColor = Color.FromArgb(150, 255, 0, 0);
        public static Color GlassBackColor = Color.Black;
        public static Color GlassLineColor = Color.Yellow;
        public static Color pnlLineColor = Color.White;
        public static Color PadColor = Color.Yellow;
        public static Color SideColor = Color.Yellow;     

        public double persent = 0;
        public string ErrorMsg = string.Empty;

        private GLS_INFO glsInfo = null;

        #endregion " Properties & Variables End"

        #region " Create & Load "

        public GlassMap()
        {
            this.Load += new System.EventHandler(this.GlassMapUi_Load);
            
            this.SizeChanged += (object sender, EventArgs e) =>
            {
                if (glsInfo != null) InitDrawData();

                this.Refresh();
            };

            this.MouseClick += GlassMap_MouseClick;
            
            this.ForeColor = Color.White;

            this.IsGroup = false;
            this.IsBubbleMap = false;
            this.IsWhiteType = false;

            this.PanelOrigin = true;

            SetControlStyle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (glsInfo == null) return;

            DoubleBuffer();
        }

        private void GlassMapUi_Load(object sender, EventArgs e)
        {
            if (glsInfo == null)
            {
                UpdateData(this.Size.Width, this.Size.Height, 0, 0, 0, 0, 0);
            }
        }

        #endregion " Create & Load End"

        #region " Public methode "

        public void UpdateData()
        {
            if (glsInfo != null) InitDrawData();

            this.Refresh();
        }

        public void UpdateData(int glassDefect_Size_X, int glassDef_Size_Y, int pnlDefect_Size_X, int pnlDef_Size_Y, int pnlCount_X, int pnlCount_Y, int defectCount)
        {
            glsInfo = new GLS_INFO();
            glsInfo.RawInfo.MapSize = new Size(glassDefect_Size_X + 200, glassDef_Size_Y + 100);
            glsInfo.RawInfo.GlassSize = new Size(glassDefect_Size_X, glassDef_Size_Y);
            glsInfo.RawInfo.PnlSize = new Size(pnlDefect_Size_X, pnlDef_Size_Y);
            glsInfo.RawInfo.PnlCount = new Point(pnlCount_X, pnlCount_Y);           

            UpdateData();

            CreateRandomDefect(defectCount);
        }

        public void UpdateData(GLS_INFO info)
        {
            glsInfo = info;
            UpdateData();
        }

        public void SelectedDefect(int defNo, Size defSize, Color defColor)
        {
            for (int i = 0; i < glsInfo.DEF_List.Count; ++i)
            {
                if (i + 1 == defNo)
                {
                    glsInfo.DEF_List[i].Color = defColor;
                    glsInfo.DEF_List[i].DEF_Size = defSize;
                }
                else
                {
                    glsInfo.DEF_List[i].Color = DefaultDefectColor;
                    glsInfo.DEF_List[i].DEF_Size = new Size(7, 7);
                }
            }

            this.Refresh();
        }

        public void SetDefect(int defNo, Size defSize, Color defColor)
        {
            glsInfo.DEF_List[defNo].Color = defColor;
            glsInfo.DEF_List[defNo].DEF_Size = defSize;

            this.Refresh();
        }

        public Image GetMapImage()
        {
            using (Graphics g = this.CreateGraphics())
            {
                Bitmap memBitMap = new Bitmap(this.Width, this.Height);

                DrawGlass(memBitMap);

                if (this.ErrorMsg == string.Empty)
                {
                    DrawPanel(memBitMap);

                    if (this.IsBubbleMap == false)
                        DrawDefect(memBitMap);
                    else
                        DrawBubbleMap(memBitMap);
                }

                g.DrawImageUnscaled(memBitMap, 0, 0);

                return memBitMap;
            }
        }

        #endregion " Public methode End"

        #region " Private methode "

        private void SetControlStyle()
        {
            this.SetStyle(ControlStyles.FixedHeight, true);
            this.SetStyle(ControlStyles.FixedWidth, true);
        }

        private void DoubleBuffer()
        {
            using (Graphics g = this.CreateGraphics())
            {
                Bitmap memBitMap = new Bitmap(this.Width, this.Height);

                SetColors();

                DrawGlass(memBitMap);

                if (this.ErrorMsg == string.Empty)
                {
                    DrawPanel(memBitMap);

                    if (this.IsBubbleMap == false)
                        DrawDefect(memBitMap);
                    else
                        DrawBubbleMap(memBitMap);
                }

                g.DrawImageUnscaled(memBitMap, 0, 0);

                memBitMap.Dispose();
            }
        }

        private void SetColors()
        {
            if (this.IsWhiteType == true)
            {
                GlassBackColor = Color.White;
                GlassLineColor = Color.Red;
                pnlLineColor = Color.Black;
                PadColor = Color.Gray;
                SideColor = Color.Brown;

                this.ForeColor = Color.Black;
                this.BackColor = Color.White;
            }
            else
            {
                GlassBackColor = Color.Black;
                GlassLineColor = Color.Red;
                pnlLineColor = Color.White;
                PadColor = Color.Yellow;
                SideColor = Color.Yellow;

                this.ForeColor = Color.White;
                this.BackColor = Color.Black;
            }
        }

        private void CreateRandomDefect(int defCnt)
        {
            Random ran = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < defCnt; ++i)
            {
                DEF_INFO def = new DEF_INFO();
                def.DEF_No = i + 1;
                def.DEF_Pos = new Point(ran.Next(0, glsInfo.GlassSize.Width), ran.Next(0, glsInfo.GlassSize.Height));
                def.Color = DefaultDefectColor;
                def.DEF_Size = new Size(7, 7);

                glsInfo.DEF_List.Add(def);
            }

            this.Refresh();
        }

        private void InitDrawData()
        {
            if (glsInfo == null) return;

            //변환 비율 계산
            double per = 0;

            double per_X = 0;
            double per_Y = 0;

            per_X = Math.Round((double)this.Width / glsInfo.RawInfo.MapSize.Width, 4);
            per_Y = Math.Round((double)this.Height / glsInfo.RawInfo.MapSize.Height, 4);

            per = per_X > per_Y ? per_Y : per_X;

            InitData_Glass(per);
            InitData_Panel(per);
            InitData_DEFECTect(per);

            this.persent = per;
        }

        private void InitData_Glass(double per)
        {
            //변환 된 테두리를 포함한 Glass 크기
            int mapDefect_Size_X = (int)(glsInfo.RawInfo.MapSize.Width * per);
            int mapDef_Size_Y = (int)(glsInfo.RawInfo.MapSize.Height * per);

            glsInfo.MapSize = new Size(mapDefect_Size_X, mapDef_Size_Y);
            glsInfo.MapOrigin = new Point((this.Width - mapDefect_Size_X) / 2, (this.Height - mapDef_Size_Y) / 2);

            //변환 된 Glass 크기
            //RAW_INFO.GlassSize = new Size(1300, 1100);

            int glassDefect_Size_X = (int)(glsInfo.RawInfo.GlassSize.Width * per);
            int glassDef_Size_Y = (int)(glsInfo.RawInfo.GlassSize.Height * per);

            glsInfo.GlassSize = new Size(glassDefect_Size_X, glassDef_Size_Y);
            glsInfo.GlassOrigin = new Point((this.Width - glassDefect_Size_X) / 2, (this.Height - glassDef_Size_Y) / 2);
        }

        private void InitData_Panel(double per)
        {
            //변환 된 pnl 크기 및 위치
            glsInfo.PNL_List.Clear();

            int cntX = 0;
            int cntY = 0;

            int pnlOrg_X = 0;
            int pnlOrg_Y = 0;

            int pnlDefect_Size_X = 0;
            int pnlDef_Size_Y = 0;

            int distance_X = 0;
            int distance_Y = 0;
            int remindX = 0;
            int remindY = 0;

            //공정 별로 X, Y 뒤집기
            cntX = glsInfo.RawInfo.PnlCount.X;
            cntY = glsInfo.RawInfo.PnlCount.Y;

            //Data Gate 찾기
            if (cntX == 0 || cntY == 0)
            {
                //this.ErrorMsg = "Glass Recipe 정보가 올바르지 않습니다.";
                return;
            }

            int xSize = glsInfo.RawInfo.GlassSize.Width / cntX;
            int ySize = glsInfo.RawInfo.GlassSize.Height / cntY;

            pnlDefect_Size_X = (int)(glsInfo.RawInfo.PnlSize.Width * per);
            pnlDef_Size_Y = (int)(glsInfo.RawInfo.PnlSize.Height * per);

            glsInfo.MapType = MapType.DataX_GateY;

            //if (xSize > ySize)
            //{
            //    pnlDefect_Size_X = (int)(glsInfo.RawInfo.PnlSize.Width * per);
            //    pnlDef_Size_Y = (int)(glsInfo.RawInfo.PnlSize.Height * per);

            //    glsInfo.MapType = MapType.DataX_GateY;
            //}
            //else
            //{
            //    pnlDefect_Size_X = (int)(glsInfo.RawInfo.PnlSize.Height * per);
            //    pnlDef_Size_Y = (int)(glsInfo.RawInfo.PnlSize.Width * per);

            //    glsInfo.MapType = MapType.GateX_DataY;
            //}

            //pnl 위치 및 간격 설정
            int nx = glsInfo.GlassSize.Width - (pnlDefect_Size_X * cntX);
            int ny = glsInfo.GlassSize.Height - (pnlDef_Size_Y * cntY);

            remindX = nx % (cntX + 1);
            remindY = ny % (cntY + 1);

            //pnl 간격
            distance_X = nx / (cntX + 1);
            distance_Y = ny / (cntY + 1);

            pnlOrg_X = glsInfo.GlassOrigin.X + distance_X + (remindX / 2);
            pnlOrg_Y = glsInfo.GlassOrigin.Y + distance_Y + (remindY / 2);

            int pnlMapDefect_Size_X = pnlDefect_Size_X + distance_X;
            int pnlMapDef_Size_Y = pnlDef_Size_Y + distance_Y;

            string[,] points = GetPoints(cntX, cntY);

            string pnlNum = string.Empty;

            for (int x = 0; x < cntX; ++x)
            {
                pnlOrg_X = glsInfo.GlassOrigin.X + distance_X + (pnlMapDefect_Size_X * x) + (remindX / 2);

                for (int y = 0; y < cntY; ++y)
                {
                    pnlOrg_Y = glsInfo.GlassOrigin.Y + distance_Y + (pnlMapDef_Size_Y * y) + (remindY / 2);

                    PNL_INFO pnl = new PNL_INFO();

                    //pnl ID 넣기
                    pnl.Point_ID = points[x, y];

                    //원점 및 크기
                    pnl.MapSize = new Size(pnlMapDefect_Size_X, pnlMapDef_Size_Y);
                    pnl.MapOrigin = new Point(pnlOrg_X - distance_X / 2, pnlOrg_Y - distance_Y / 2);

                    pnl.PnlSize = new Size(pnlDefect_Size_X, pnlDef_Size_Y);
                    pnl.PnlOrigin = new Point(pnlOrg_X, pnlOrg_Y);

                    //변환 pnl 원점

                    int a_originX = 0;
                    int a_originY = 0;

                    if (glsInfo.MapType == MapType.DataX_GateY)
                    {
                        a_originX = pnl.PnlOrigin.X + pnl.PnlSize.Width;
                        a_originY = pnl.PnlOrigin.Y + pnl.PnlSize.Height;
                    }
                    else
                    {
                        a_originX = pnl.PnlOrigin.X + pnl.PnlSize.Width;
                        a_originY = pnl.PnlOrigin.Y;
                    }

                    pnl.Pnl_A_Origin = new Point(a_originX, a_originY);

                    glsInfo.PNL_List.Add(pnl);
                }
            }
        }

        private void InitData_DEFECTect(double per)
        {
            //변환 된 pnl 크기 및 위치
            for (int i = 0; i < glsInfo.DEF_List.Count; ++i)
            {
                //Defect pnl 위치
                Point d_origin = new Point(0, 0);

                int px = 0;
                int py = 0;

                px = (int)(glsInfo.DEF_List[i].Raw_Pos.X * per);
                py = (int)(glsInfo.DEF_List[i].Raw_Pos.Y * per);

                d_origin = new Point(px, py);
                Rectangle rect = new Rectangle(d_origin, glsInfo.DEF_List[i].DEF_Size);

                glsInfo.DEF_List[i].DEF_Pos = d_origin;
                glsInfo.DEF_List[i].Rect = rect;
            }
        }

        private void DrawGlass(Bitmap map)
        {
            using (Graphics g = Graphics.FromImage(map))
            {
                //테두리를 포함한 Glass 그리기
                SolidBrush glassBr = new SolidBrush(GlassBackColor);
                g.FillRectangle(glassBr, new Rectangle(glsInfo.MapOrigin, glsInfo.MapSize));

                //Glass 그리기
                Pen glassLinePen = new Pen(GlassLineColor, 1);
                Rectangle rect = new Rectangle(glsInfo.GlassOrigin, glsInfo.GlassSize);
                g.DrawRectangle(glassLinePen, rect);

                //Error 메세지 표시
                if (this.ErrorMsg != string.Empty)
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    SolidBrush errBr = new SolidBrush(this.ForeColor);
                    g.DrawString("Glass Recipe 정보가 올바르지 않습니다.", this.Font, errBr, rect, format);
                }
            }
        }

        private void DrawRecipeInfo(Graphics g, Rectangle rect)
        {
            string strStart = "0, 0";
            SolidBrush defaultBr = new SolidBrush(this.ForeColor);
            Point strPoint = new Point((int)(rect.X - g.MeasureString(strStart, this.Font).Width / 2), (int)(rect.Y - g.MeasureString(strStart, this.Font).Height));
            g.DrawString(strStart, this.Font, defaultBr, strPoint);

            string exStr = string.Empty;
            string eyStr = string.Empty;
            string xStr = string.Empty;
            string yStr = string.Empty;

            exStr = "1300";
            eyStr = "1100";
            xStr = "→ X";
            yStr = "↓\r\n Y  ";

            Point pEnd_X = new Point((int)(rect.Width + rect.X - g.MeasureString(exStr, this.Font).Width), (int)(rect.Y - g.MeasureString(exStr, this.Font).Height));
            Point pEnd_Y = new Point((int)(rect.X - g.MeasureString(eyStr, this.Font).Width), (int)(rect.Height + rect.Y - g.MeasureString(eyStr, this.Font).Height));
            g.DrawString(exStr, this.Font, defaultBr, pEnd_X);
            g.DrawString(eyStr, this.Font, defaultBr, pEnd_Y);

            SolidBrush pointBr = new SolidBrush(PadColor);
            Point xPoint = new Point((int)(rect.X + g.MeasureString(xStr, this.Font).Width), (int)(rect.Y - g.MeasureString(xStr, this.Font).Height));
            Point yPoint = new Point((int)(rect.X - g.MeasureString(yStr, this.Font).Width), (int)(rect.Y + g.MeasureString(yStr, this.Font).Height));
            g.DrawString(xStr, this.Font, pointBr, xPoint);
            g.DrawString(yStr, this.Font, pointBr, yPoint);
        }

        private void DrawPanel(Bitmap map)
        {
            using (Graphics g = Graphics.FromImage(map))
            {
                for (int i = 0; i < glsInfo.PNL_List.Count; ++i)
                {
                    PNL_INFO pnlinfo = glsInfo.PNL_List[i];

                    //pnl 외곽 선 그리기       
                    Pen pnlLinePen = new Pen(pnlLineColor);
                    Rectangle rect = new Rectangle(pnlinfo.PnlOrigin, pnlinfo.PnlSize);
                    g.DrawRectangle(pnlLinePen, rect);

                    //글자가 짤리므로...
                    Rectangle sideRect = new Rectangle(rect.X + 2, rect.Y + 3, rect.Width - 2, rect.Height - 3);

                    //가운데 글자 넣기
                    string pointStr = pnlinfo.Point_ID;

                    SolidBrush sideBrush = new SolidBrush(SideColor);
                    SolidBrush cenderBrush = new SolidBrush(SideColor);
                    Font ft = new Font(this.Font.FontFamily, 8F);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(pointStr, ft, cenderBrush, sideRect, format);

                    //pnl 원점 표시
                    if (PanelOrigin == true)
                    {
                        int lineLength = (int)(7 * this.persent);
                        Pen p = new Pen(Color.Lime, 2f);

                        if (glsInfo.MapType == MapType.DataX_GateY)
                        {
                            Point pLine01 = new Point(pnlinfo.Pnl_A_Origin.X - lineLength, pnlinfo.Pnl_A_Origin.Y + lineLength);
                            Point pLine02 = new Point(pnlinfo.Pnl_A_Origin.X + lineLength, pnlinfo.Pnl_A_Origin.Y - lineLength);
                            g.DrawLine(p, pLine01, pLine02);
                        }
                        else
                        {
                            Point pLine01 = new Point(pnlinfo.Pnl_A_Origin.X - lineLength, pnlinfo.Pnl_A_Origin.Y - lineLength);
                            Point pLine02 = new Point(pnlinfo.Pnl_A_Origin.X + lineLength, pnlinfo.Pnl_A_Origin.Y + lineLength);
                            g.DrawLine(p, pLine01, pLine02);
                        }
                    }
                }
            }
        }

        private void DrawBubbleMap(Bitmap map)
        {
            //Bubble Map 그리기
            using (Graphics g = Graphics.FromImage(map))
            {
                glsInfo.BBL_List = new List<BBL_INFO>();

                Image bubbleImg = null;
                SolidBrush valueBrush = null;

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                for (int nPnl = 0; nPnl < glsInfo.PNL_List.Count; ++nPnl)
                {
                    PNL_INFO pnlInfo = glsInfo.PNL_List[nPnl];

                    if (this.Bubble_Type == BubbleType.Panel)
                    {
                        if (this.IsWhiteType == true)
                        {
                            valueBrush = new SolidBrush(Color.Red);
                            bubbleImg = Properties.Resources.BG_Bubble02;
                        }
                        else
                        {
                            valueBrush = new SolidBrush(Color.Blue);
                            bubbleImg = Properties.Resources.BG_Bubble;
                        }

                        //각 Point 별 Bubble Map
                        int width = pnlInfo.MapSize.Width;
                        int height = pnlInfo.MapSize.Height;

                        int orgX = pnlInfo.MapOrigin.X;
                        int orgY = pnlInfo.MapOrigin.Y;

                        Pen linePen = new Pen(Color.FromArgb(100, 0, 0, 255), 1);

                        //기본 Rect 그리기
                        Rectangle rect = new Rectangle(orgX, orgY, width, height);
                        g.DrawRectangle(linePen, rect);
                        BBL_INFO bubble = new BBL_INFO();
                        bubble.Rect = rect;
                        bubble.PNL_ID = pnlInfo.Point_ID;

                        glsInfo.BBL_List.Add(bubble);
                    }
                    else
                    {
                        if (this.IsWhiteType == true)
                        {
                            valueBrush = new SolidBrush(Color.Red);
                            bubbleImg = Properties.Resources.BG_Bubble02_S;
                        }
                        else
                        {
                            valueBrush = new SolidBrush(Color.Blue);
                            bubbleImg = Properties.Resources.BG_Bubble_S;
                        }

                        //각 Point 별 Bubble Map
                        int width = pnlInfo.MapSize.Width / 3;
                        int height = pnlInfo.MapSize.Height / 3;

                        int orgX = pnlInfo.MapOrigin.X;
                        int orgY = pnlInfo.MapOrigin.Y;

                        Pen linePen = new Pen(Color.FromArgb(100, 0, 0, 255), 1);

                        //기본 Rect 그리기
                        for (int x = 0; x < 3; ++x)
                        {
                            int tmpX = orgX + (x * width);

                            for (int y = 0; y < 3; ++y)
                            {
                                int tmpY = orgY + (y * height);

                                if (x == 1 && y == 1) continue;

                                Rectangle rect = new Rectangle(tmpX, tmpY, width, height);
                                g.DrawRectangle(linePen, rect);

                                BBL_INFO bubble = new BBL_INFO();
                                bubble.Rect = rect;
                                bubble.PNL_ID = pnlInfo.Point_ID;

                                glsInfo.BBL_List.Add(bubble);
                            }
                        }
                    }
                }

                //Defect 넣기
                for (int i = 0; i < glsInfo.DEF_List.Count; ++i)
                {
                    if (glsInfo.DEF_List[i].Visible == false) continue;

                    string pointStr = glsInfo.DEF_List[i].Point_ID;

                    Rectangle defRect = glsInfo.DEF_List[i].Rect;

                    for (int k = 0; k < glsInfo.BBL_List.Count; ++k)
                    {
                        if (glsInfo.BBL_List[k].Rect.IntersectsWith(defRect))
                        {
                            if (pointStr == null)
                            {
                                glsInfo.BBL_List[k].DEF_Count++;
                                break;
                            }
                            else if (glsInfo.BBL_List[k].PNL_ID == pointStr)
                            {
                                glsInfo.BBL_List[k].DEF_Count++;
                                break;
                            }
                        }

                    }
                }

                //bubble Map 비율 가져오기
                double totalCount = 0;
                double maxCount = 0;

                for (int i = 0; i < glsInfo.BBL_List.Count; ++i)
                {
                    if (glsInfo.BBL_List[i].DEF_Count > maxCount) maxCount = glsInfo.BBL_List[i].DEF_Count;

                    totalCount += glsInfo.BBL_List[i].DEF_Count;
                }

                if (glsInfo.BBL_List.Count == 0) return;

                double maxRate = Math.Round(maxCount / totalCount, 4) * 100;
                int sizePoint = (int)Math.Ceiling((double)glsInfo.BBL_List[0].Rect.Height / maxRate);

                //Bubble Map 그리기
                for (int i = 0; i < glsInfo.BBL_List.Count; ++i)
                {
                    BBL_INFO b = glsInfo.BBL_List[i];

                    if (b.DEF_Count == 0) continue;

                    double m = Math.Round((double)b.DEF_Count / totalCount, 4) * 100;

                    Size bSize = new Size((int)(sizePoint * m), (int)(sizePoint * m));

                    if (bSize.Width > b.Rect.Width) bSize.Width = b.Rect.Width;
                    if (bSize.Height > b.Rect.Height) bSize.Height = b.Rect.Height;

                    Point center = new Point(b.Rect.X + b.Rect.Width / 2, b.Rect.Y + b.Rect.Height / 2);

                    Rectangle bubbleRect = new Rectangle(center.X - bSize.Width / 2, center.Y - bSize.Height / 2, bSize.Width, bSize.Height);
                    b.BBL_Rect = bubbleRect;
                    g.DrawImage(bubbleImg, bubbleRect);

                    //값 넣기
                    g.DrawString(b.DEF_Count.ToString(), this.Font, valueBrush, b.Rect, format);
                }

                bubbleImg.Dispose();
            }
        }

        private void DrawDefect(Bitmap map)
        {
            using (Graphics g = Graphics.FromImage(map))
            {
                Pen p = new Pen(Brushes.Black, 1F);
                SolidBrush br = new SolidBrush(Color.Black);

                for (int i = 0; i < glsInfo.DEF_List.Count; ++i)
                {
                    Rectangle rect = new Rectangle(glsInfo.DEF_List[i].DEF_Pos, glsInfo.DEF_List[i].DEF_Size);
                    br.Color = glsInfo.DEF_List[i].Color;
                    g.FillEllipse(br, rect);
                }
            }
        }

        private string[,] GetPoints(int xCnt, int yCnt)
        {
            string[,] tmps = new string[xCnt, yCnt];

            int total = xCnt * yCnt;

            char c01 = 'A';
            char c02 = '0';

            for (int x = 0; x < xCnt; ++x)
            {
                for (int y = 0; y < yCnt; ++y)
                {
                    int pX = xCnt - x - 1;
                    int pY = yCnt - y - 1;

                    if (total < 36)
                    {
                        tmps[pX, pY] = (c01++).ToString();

                        if (c01 > 'Z') c01 = '0';
                    }
                    else
                    {
                        tmps[pX, pY] = (c02).ToString() + (c01++).ToString();

                        if (c01 > 'Z') c01 = '0';
                        else if (c01 == '9' + 1)
                        {
                            c01 = 'A';
                            c02++;
                        }
                    }
                }
            }

            return tmps;
        }

        #endregion " Private methode End"

        #region "  Event handler "

        void GlassMap_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            List<int> rowNoList = new List<int>();

            if (this.IsBubbleMap == false)
            {
                foreach (DEF_INFO def in glsInfo.DEF_List)
                {
                    if (def.Visible && def.Rect.Contains(p))
                    {
                        rowNoList.Add(def.DEF_No);
                    }
                }
            }
            else
            {
                foreach (BBL_INFO bbl in glsInfo.BBL_List)
                {
                    if (bbl.Rect != null && bbl.Rect.Contains(p))
                    {
                        foreach (DEF_INFO def in glsInfo.DEF_List)
                        {
                            if (def.Visible && bbl.Rect.Contains(def.DEF_Pos))
                            {
                                rowNoList.Add(def.DEF_No);
                            }
                        }

                        break;
                    }
                }
            }

            if (rowNoList.Count > 0)
            {
                DefectPointEventArgs args = new DefectPointEventArgs();
                args.PointList = rowNoList;
                OnDefectClickEvent(args);
            }
        }

        #endregion Event handler...................................................................................................
    }
}
