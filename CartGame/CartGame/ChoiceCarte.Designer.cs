namespace CartGame
{
    partial class ChoiceForm
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
            this.components = new System.ComponentModel.Container();
            this.panelUserCarte = new System.Windows.Forms.Panel();
            this.labelAll = new System.Windows.Forms.Label();
            this.labelUser = new System.Windows.Forms.Label();
            this.labelHelp = new System.Windows.Forms.Label();
            this.panelAllCarte = new System.Windows.Forms.Panel();
            this.buttonStartSeek = new System.Windows.Forms.Button();
            this.FewCarte = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.FewCarte)).BeginInit();
            this.SuspendLayout();
            // 
            // panelUserCarte
            // 
            this.panelUserCarte.AutoScroll = true;
            this.panelUserCarte.BackColor = System.Drawing.SystemColors.Window;
            this.panelUserCarte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUserCarte.Location = new System.Drawing.Point(603, 31);
            this.panelUserCarte.Name = "panelUserCarte";
            this.panelUserCarte.Size = new System.Drawing.Size(212, 385);
            this.panelUserCarte.TabIndex = 1;
            // 
            // labelAll
            // 
            this.labelAll.AutoSize = true;
            this.labelAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelAll.Location = new System.Drawing.Point(12, 11);
            this.labelAll.Name = "labelAll";
            this.labelAll.Size = new System.Drawing.Size(151, 16);
            this.labelAll.TabIndex = 2;
            this.labelAll.Text = "Все доступные карты:";
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelUser.Location = new System.Drawing.Point(600, 11);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(87, 16);
            this.labelUser.TabIndex = 3;
            this.labelUser.Text = "Ваши карты:";
            // 
            // labelHelp
            // 
            this.labelHelp.AutoSize = true;
            this.labelHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelHelp.Location = new System.Drawing.Point(12, 421);
            this.labelHelp.Name = "labelHelp";
            this.labelHelp.Size = new System.Drawing.Size(409, 15);
            this.labelHelp.TabIndex = 4;
            this.labelHelp.Text = "*Выберите из доступных вам карт 7 штук. Карты могут повторяться!";
            // 
            // panelAllCarte
            // 
            this.panelAllCarte.AutoScroll = true;
            this.panelAllCarte.BackColor = System.Drawing.SystemColors.Window;
            this.panelAllCarte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAllCarte.Location = new System.Drawing.Point(12, 30);
            this.panelAllCarte.Name = "panelAllCarte";
            this.panelAllCarte.Size = new System.Drawing.Size(576, 386);
            this.panelAllCarte.TabIndex = 0;
            // 
            // buttonStartSeek
            // 
            this.buttonStartSeek.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonStartSeek.Location = new System.Drawing.Point(608, 424);
            this.buttonStartSeek.Name = "buttonStartSeek";
            this.buttonStartSeek.Size = new System.Drawing.Size(207, 44);
            this.buttonStartSeek.TabIndex = 5;
            this.buttonStartSeek.Text = "Начать поиск противника";
            this.buttonStartSeek.UseVisualStyleBackColor = true;
            this.buttonStartSeek.Click += new System.EventHandler(this.buttonStartSeek_Click);
            // 
            // FewCarte
            // 
            this.FewCarte.ContainerControl = this;
            this.FewCarte.RightToLeft = true;
            // 
            // ChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 473);
            this.Controls.Add(this.buttonStartSeek);
            this.Controls.Add(this.labelHelp);
            this.Controls.Add(this.labelUser);
            this.Controls.Add(this.labelAll);
            this.Controls.Add(this.panelUserCarte);
            this.Controls.Add(this.panelAllCarte);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ChoiceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выберите карты";
           
            this.Load += new System.EventHandler(this.ChoiceForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FewCarte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelUserCarte;
        private System.Windows.Forms.Label labelAll;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.Label labelHelp;
        private System.Windows.Forms.Panel panelAllCarte;
        private System.Windows.Forms.Button buttonStartSeek;
        private System.Windows.Forms.ErrorProvider FewCarte;
    }
}