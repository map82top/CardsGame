namespace CartGame
{
    partial class Settings
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ip = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.TextBox();
            this.nickname = new System.Windows.Forms.TextBox();
            this.Save = new System.Windows.Forms.Button();
            this.errorIPAdress = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorPort = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorIPAdress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorPort)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(22, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Введите IP-адрес сервера:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(22, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Введите порт сервера:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(22, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Введите никнейм:";
            // 
            // ip
            // 
            this.ip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ip.Location = new System.Drawing.Point(25, 47);
            this.ip.MaxLength = 15;
            this.ip.Name = "ip";
            this.ip.Size = new System.Drawing.Size(253, 21);
            this.ip.TabIndex = 3;
            // 
            // port
            // 
            this.port.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.port.Location = new System.Drawing.Point(25, 98);
            this.port.MaxLength = 5;
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(228, 21);
            this.port.TabIndex = 4;
            // 
            // nickname
            // 
            this.nickname.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nickname.Location = new System.Drawing.Point(25, 139);
            this.nickname.MaxLength = 10;
            this.nickname.Name = "nickname";
            this.nickname.Size = new System.Drawing.Size(145, 21);
            this.nickname.TabIndex = 5;
            // 
            // Save
            // 
            this.Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Save.Location = new System.Drawing.Point(86, 176);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(121, 36);
            this.Save.TabIndex = 6;
            this.Save.Text = "Сохранить";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // errorIPAdress
            // 
            this.errorIPAdress.ContainerControl = this;
            this.errorIPAdress.RightToLeft = true;
            // 
            // errorPort
            // 
            this.errorPort.ContainerControl = this;
            this.errorPort.RightToLeft = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 224);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.nickname);
            this.Controls.Add(this.port);
            this.Controls.Add(this.ip);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки сетевой игры";
            this.Load += new System.EventHandler(this.Settings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorIPAdress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ip;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.TextBox nickname;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.ErrorProvider errorIPAdress;
        private System.Windows.Forms.ErrorProvider errorPort;
    }
}