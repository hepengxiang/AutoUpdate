namespace AutoUpdate
{
    partial class frmUpdate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdate));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblUpdateMsg = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblWindowTitle = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.lblUpdateMsg);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(553, 47);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.progressBar1.Cursor = System.Windows.Forms.Cursors.Default;
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(3, 31);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(547, 13);
            this.progressBar1.TabIndex = 2;
            this.progressBar1.Value = 10;
            // 
            // lblUpdateMsg
            // 
            this.lblUpdateMsg.AutoSize = true;
            this.lblUpdateMsg.Font = new System.Drawing.Font("宋体", 9.5F);
            this.lblUpdateMsg.ForeColor = System.Drawing.Color.Green;
            this.lblUpdateMsg.Location = new System.Drawing.Point(5, 16);
            this.lblUpdateMsg.Name = "lblUpdateMsg";
            this.lblUpdateMsg.Size = new System.Drawing.Size(147, 13);
            this.lblUpdateMsg.TabIndex = 3;
            this.lblUpdateMsg.Text = "1/1 ... DownLoad.dll";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(553, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblWindowTitle
            // 
            this.lblWindowTitle.AutoSize = true;
            this.lblWindowTitle.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblWindowTitle.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblWindowTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblWindowTitle.Location = new System.Drawing.Point(121, 9);
            this.lblWindowTitle.Name = "lblWindowTitle";
            this.lblWindowTitle.Size = new System.Drawing.Size(57, 19);
            this.lblWindowTitle.TabIndex = 2;
            this.lblWindowTitle.Text = "通 用";
            // 
            // frmUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 127);
            this.ControlBox = false;
            this.Controls.Add(this.lblWindowTitle);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自动更新程序";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmUpdate_FormClosing);
            this.Load += new System.EventHandler(this.frmUpdate_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmUpdate_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblUpdateMsg;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblWindowTitle;
    }
}