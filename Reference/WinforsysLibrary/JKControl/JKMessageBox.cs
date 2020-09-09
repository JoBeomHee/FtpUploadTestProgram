using System;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel; 

namespace Winforsys.JKControl
{
    //Top Most 메세지 박스
    public static class JKMessageBox
    {
        private static Form CreateTopMostFrm()
        {
            Form parentFrm = Form.ActiveForm;

            if (parentFrm == null) parentFrm = Application.OpenForms[0];

            Form frm = new Form();

            frm.Opacity = 0;
            //frm.ControlBox = false;
            //frm.ShowInTaskbar = false;
            //frm.FormBorderStyle = FormBorderStyle.None;
            //frm.Size = new System.Drawing.Size(0, 0);
            frm.StartPosition = FormStartPosition.Manual;

            if (parentFrm.InvokeRequired)
            {
                parentFrm.Invoke((MethodInvoker)delegate
                {
                    frm.Location = parentFrm.PointToScreen(new Point(parentFrm.Width / 2, parentFrm.Height / 2));
                });
            }
            else
            {
                frm.Location = parentFrm.PointToScreen(new Point(parentFrm.Width / 2, parentFrm.Height / 2));
            }

            frm.Show();
            frm.Focus();
            frm.BringToFront();
            frm.TopMost = true;

            return frm;
        }

        public static DialogResult Show(string text)
        {
            Form frm = CreateTopMostFrm();
            DialogResult result = MessageBox.Show(frm, text);
            frm.Dispose();

            return result;
        }

        public static DialogResult Show(string text, string caption)
        {
            Form frm = CreateTopMostFrm();
            DialogResult result = MessageBox.Show(frm, text, caption);
            frm.Dispose();

            return result;
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Form frm = CreateTopMostFrm();
            DialogResult result = MessageBox.Show(frm, text, caption, buttons, icon);
            frm.Dispose();

            return result;

        }

        public static DialogResult ShowExcel(Control ctl, string fileName)
        {
            UserMessageBox box = new UserMessageBox();
            box.StartPosition = FormStartPosition.CenterParent;

            DialogResult result = box.ShowDialog(ctl);

            if (result == DialogResult.OK)
            {
                Microsoft.Office.Interop.Excel.Workbook workbook = null;
                Microsoft.Office.Interop.Excel.Application application = null;
                application = new Microsoft.Office.Interop.Excel.Application();

                workbook = (Excel.Workbook)(application.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing));

                application.Visible = true;
            }

            return result;
        }
    }
}
