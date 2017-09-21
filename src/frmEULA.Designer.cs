namespace LOIC
{
    partial class frmEULA
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
            this.txtEULA = new System.Windows.Forms.RichTextBox();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnDecline = new System.Windows.Forms.Button();
            this.chkEULA = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // txtEULA
            //
            this.txtEULA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEULA.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEULA.Location = new System.Drawing.Point(1, 0);
            this.txtEULA.Name = "txtEULA";
            this.txtEULA.Size = new System.Drawing.Size(563, 510);
            this.txtEULA.TabIndex = 0;
            this.txtEULA.Text = "";
            //
            // btnAccept
            //
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Enabled = false;
            this.btnAccept.Location = new System.Drawing.Point(400, 515);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 1;
            this.btnAccept.Text = "&Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            //
            // btnDecline
            //
            this.btnDecline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDecline.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDecline.Location = new System.Drawing.Point(481, 515);
            this.btnDecline.Name = "btnDecline";
            this.btnDecline.Size = new System.Drawing.Size(75, 23);
            this.btnDecline.TabIndex = 2;
            this.btnDecline.Text = "&Decline";
            this.btnDecline.UseVisualStyleBackColor = true;
            //
            // chkEULA
            //
            this.chkEULA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkEULA.AutoSize = true;
            this.chkEULA.Location = new System.Drawing.Point(12, 519);
            this.chkEULA.Name = "chkEULA";
            this.chkEULA.Size = new System.Drawing.Size(287, 17);
            this.chkEULA.TabIndex = 3;
            this.chkEULA.Text = "I have read and &understood the terms of this agreement";
            this.chkEULA.UseVisualStyleBackColor = true;
            this.chkEULA.CheckedChanged += new System.EventHandler(this.chkEULA_CheckedChanged);
            //
            // frmEULA
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 542);
            this.Controls.Add(this.chkEULA);
            this.Controls.Add(this.btnDecline);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.txtEULA);
            this.Icon = global::LOIC.Properties.Resources.LOIC_ICO;
            this.MinimumSize = new System.Drawing.Size(500, 480);
            this.Name = "frmEULA";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Newfag\'s Low Orbit Ion Cannon End User License Agreement (EULA)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtEULA;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnDecline;
        private System.Windows.Forms.CheckBox chkEULA;
    }
}