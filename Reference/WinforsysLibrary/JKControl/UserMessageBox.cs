using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Winforsys.JKControl
{
    public partial class UserMessageBox : Form
    {
        public UserMessageBox()
        {
            InitializeComponent();

            this.DialogResult = System.Windows.Forms.DialogResult.None;

            this.uiBtn_Ok.Click += uiBtn_Ok_Click;
            this.uiBtn_Open.Click += uiBtn_Open_Click;
        }

        void uiBtn_Open_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        void uiBtn_Ok_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
    }
}
