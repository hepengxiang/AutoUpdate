using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ftp下载
{
    public partial class frmUpdate : Form
    {
        public ProgressBar progressBar
        {
            get { return this.progressBar1; }
        }

        public Label lFile
        {
            get { return lblFile; }
        }
        public string Title
        {
            set { label1.Text = value; }
        }

        bool m_blnAltF4;
        public frmUpdate()
        {
            InitializeComponent();           
        }
        private void frmUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.m_blnAltF4)
            {
                e.Cancel = true;
            }
        }

        private void frmUpdate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && (e.KeyCode == Keys.F4))
            {
                this.m_blnAltF4 = true;
            }
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            base.Top -= 50;
        }
    }
}
