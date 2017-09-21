namespace LOIC
{
    partial class frmEZGrab
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnUpdate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtHivemind = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.btnShorten = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtOverlord = new System.Windows.Forms.TextBox();
            this.rbbitly = new System.Windows.Forms.RadioButton();
            this.rbisgd = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            //
            // btnUpdate
            //
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(64)))), ((int)(((byte)(96)))));
            this.btnUpdate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.ForeColor = System.Drawing.Color.Azure;
            this.btnUpdate.Location = new System.Drawing.Point(340, 19);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(55, 22);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.txtHivemind);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.LightBlue;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(411, 52);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IRC Hivemind";
            //
            // txtHivemind
            //
            this.txtHivemind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(48)))), ((int)(((byte)(64)))));
            this.txtHivemind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHivemind.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHivemind.ForeColor = System.Drawing.Color.Azure;
            this.txtHivemind.Location = new System.Drawing.Point(15, 19);
            this.txtHivemind.Name = "txtHivemind";
            this.txtHivemind.Size = new System.Drawing.Size(380, 20);
            this.txtHivemind.TabIndex = 1;
            //
            // label2
            //
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Attacktime in UTC (leave empty if none):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDate
            //
            this.txtDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(48)))), ((int)(((byte)(64)))));
            this.txtDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDate.ForeColor = System.Drawing.Color.Azure;
            this.txtDate.Location = new System.Drawing.Point(222, 19);
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(105, 20);
            this.txtDate.TabIndex = 3;
            this.txtDate.Text = "2010/05/25 18:00";
            this.txtDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // btnShorten
            //
            this.btnShorten.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(64)))), ((int)(((byte)(96)))));
            this.btnShorten.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShorten.ForeColor = System.Drawing.Color.Azure;
            this.btnShorten.Location = new System.Drawing.Point(154, 109);
            this.btnShorten.Name = "btnShorten";
            this.btnShorten.Size = new System.Drawing.Size(127, 31);
            this.btnShorten.TabIndex = 2;
            this.btnShorten.Text = "Make tiny";
            this.btnShorten.UseVisualStyleBackColor = false;
            this.btnShorten.Click += new System.EventHandler(this.btnShorten_Click);
            //
            // groupBox2
            //
            this.groupBox2.Controls.Add(this.txtOverlord);
            this.groupBox2.Controls.Add(this.btnUpdate);
            this.groupBox2.Controls.Add(this.rbbitly);
            this.groupBox2.Controls.Add(this.btnShorten);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.rbisgd);
            this.groupBox2.Controls.Add(this.txtDate);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.LightBlue;
            this.groupBox2.Location = new System.Drawing.Point(12, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(411, 154);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "OverLord";
            //
            // txtOverlord
            //
            this.txtOverlord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(48)))), ((int)(((byte)(64)))));
            this.txtOverlord.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOverlord.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOverlord.ForeColor = System.Drawing.Color.Azure;
            this.txtOverlord.Location = new System.Drawing.Point(15, 52);
            this.txtOverlord.Name = "txtOverlord";
            this.txtOverlord.Size = new System.Drawing.Size(380, 20);
            this.txtOverlord.TabIndex = 1;
            //
            // rbbitly
            //
            this.rbbitly.AutoSize = true;
            this.rbbitly.Location = new System.Drawing.Point(261, 82);
            this.rbbitly.Name = "rbbitly";
            this.rbbitly.Size = new System.Drawing.Size(47, 18);
            this.rbbitly.TabIndex = 30;
            this.rbbitly.Text = "bit.ly";
            this.rbbitly.UseVisualStyleBackColor = true;
            //
            // rbisgd
            //
            this.rbisgd.AutoSize = true;
            this.rbisgd.Checked = true;
            this.rbisgd.Location = new System.Drawing.Point(103, 82);
            this.rbisgd.Name = "rbisgd";
            this.rbisgd.Size = new System.Drawing.Size(48, 18);
            this.rbisgd.TabIndex = 29;
            this.rbisgd.TabStop = true;
            this.rbisgd.Text = "is.gd";
            this.rbisgd.UseVisualStyleBackColor = true;
            //
            // btnCancel
            //
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(64)))), ((int)(((byte)(96)))));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.Azure;
            this.btnCancel.Location = new System.Drawing.Point(190, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(55, 22);
            this.btnCancel.TabIndex = 32;
            this.btnCancel.Text = "GTFO";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmEZGrab
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(24)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(435, 279);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.Color.LightBlue;
            this.Icon = global::LOIC.Properties.Resources.LOIC_ICO;
            this.Name = "frmEZGrab";
            this.Text = "HiveMind / Overlord eZGrab";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmEZGrab_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtHivemind;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.Button btnShorten;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtOverlord;
        private System.Windows.Forms.RadioButton rbbitly;
        private System.Windows.Forms.RadioButton rbisgd;
        private System.Windows.Forms.Button btnCancel;

    }
}