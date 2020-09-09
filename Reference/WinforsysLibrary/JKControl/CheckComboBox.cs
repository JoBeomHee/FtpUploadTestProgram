using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Winforsys.PopupControl;

namespace Winforsys.JKControl
{
    public class CheckComboBox : ComboBox
    {
        #region " Properties & Variables "

        public readonly string SelectedALL = "(ALL)";

        private Popup chkListPop;
        private CheckedListBox chkList;

        public bool CancelItemsCheck = false;

        public CheckedListBox List
        {
            get
            {
                if (this.chkList == null) this.chkList = new CheckedListBox();

                return this.chkList;
            }
        }

        public ItemCheckEventArgs EventArgs_Checked { get; set; }

        #endregion Properties & Variables...........................................................................................

        #region " Create & Load "

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CheckCmbUi
            // 
            this.DropDownHeight = 1;
            this.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DropDownWidth = 1;
            this.IntegralHeight = false;
            this.ResumeLayout(false);
        }

        public CheckComboBox()
        {
            InitializeComponent();

            if (chkList == null) chkList = new CheckedListBox();

            this.MouseDown += CheckCombobox_MouseDown;
            this.chkList.ItemCheck += chkList_ItemCheck;
        }

        #endregion Create & Load....................................................................................................

        #region "  Public methode "

        public void Clear()
        {
            chkList.Items.Clear();
        }

        public void Reset(object[] list)
        {
            string tmpString = string.Empty;

            if (chkList.Items.Count > 0 && this.Text != this.SelectedALL)
            {
                tmpString = this.Text;
            }

            this.Clear();

            chkList.Items.Add(this.SelectedALL);

            if (list != null)
                chkList.Items.AddRange(list);

            if (tmpString != string.Empty)
            {
                for (int i = 0; i < chkList.Items.Count; ++i)
                {
                    string tmp = chkList.Items[i].ToString();

                    if (tmpString.Contains(tmp))
                        chkList.SetItemChecked(i, true);
                }
            }
            else
            {
                chkList.SetItemChecked(0, true);
            }

            if (chkList.CheckedItems.Count == 0)
                chkList.SetItemChecked(0, true);
        }

        public void SetItemChecked(int index, bool value)
        {
            chkList.SetItemChecked(index, value);
        }

        //체크 이후에나 나타나서 못 씀...
        //public bool GetItemChecked(int index)
        //{
        //    if (chkList.Items.Count <= index) return true;

        //    return chkList.GetItemChecked(index);
        //}

        public string[] GetArray()
        {
            List<string> arr = new List<string>();

            for (int i = 1; i < chkList.Items.Count; ++i)
            {
                if (chkList.Items[i].ToString() == string.Empty) continue;

                arr.Add(chkList.Items[i].ToString());
            }

            if (arr.Count == 0) return new string[] { "" };

            return arr.ToArray();
        }

        #endregion Public methode...................................................................................................

        #region "  Private methode "

        #endregion Private methode..................................................................................................

        #region "  Event handler "

        //사용 안함
        //void CheckCmbUi_MouseClick(object sender, MouseEventArgs e)
        //{
        //    chkListPop.ShowOrClose(sender as CheckCombobox);
        //}

        void CheckCombobox_MouseDown(object sender, MouseEventArgs e)
        {
            if (chkListPop == null)
            {
                chkList.CheckOnClick = true;
                chkList.BackColor = Color.WhiteSmoke;
                chkListPop = new Popup(chkList);
            }

            chkListPop.BackColor = Color.Transparent;
            chkListPop.Width = this.Width;

            int h = 50 + (chkList.Items.Count * 15);

            chkListPop.Height = h > 500 ? 500 : h;

            chkListPop.ShowOrClose(sender as CheckComboBox);
        }

        void chkList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.CancelItemsCheck == true)
            {
                e.NewValue = CheckState.Unchecked;
                this.CancelItemsCheck = false;
                return;
            }

            this.chkList.ItemCheck -= new ItemCheckEventHandler(chkList_ItemCheck);

            this.EventArgs_Checked = e;

            string tmp = string.Empty;
            bool state = e.NewValue == CheckState.Checked ? true : false;

            if (chkList.Items[e.Index].ToString() == this.SelectedALL)
            {
                for (int i = 0; i < chkList.Items.Count; ++i)
                {
                    chkList.SetItemChecked(i, state);
                }

                if (state == true)
                    tmp = this.SelectedALL;
                else
                    tmp = string.Empty;
            }
            else
            {
                int startNo = 1;

                if (state == false && chkList.Items[0].ToString() == this.SelectedALL)
                    chkList.SetItemChecked(0, state);
                else
                    startNo = 0;

                for (int i = startNo; i < chkList.Items.Count; ++i)
                {
                    if ((i != e.Index && chkList.GetItemChecked(i) == true) ||
                        (i == e.Index && e.NewValue == CheckState.Checked))
                    {
                        tmp += string.Format("'{0}',", chkList.Items[i].ToString());
                    }
                }

                tmp = tmp.Length > 2 ? tmp.Remove(tmp.Length - 1, 1) : tmp;
            }

            this.Items.Clear();
            this.Items.Add(tmp);
            this.SelectedIndex = 0;

            this.chkList.ItemCheck += new ItemCheckEventHandler(chkList_ItemCheck);
        }

        #endregion Event handler....................................................................................................
    }
}
