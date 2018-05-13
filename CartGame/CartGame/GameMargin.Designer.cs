namespace CartGame
{
    partial class GameMargin
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
            this.UserHQPanel = new System.Windows.Forms.Panel();
            this.EnemyHQPanel = new System.Windows.Forms.Panel();
            this.MyCarte = new System.Windows.Forms.Panel();
            this.EnemyCarte = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.StepEnd = new System.Windows.Forms.Button();
            this.EnemyTime = new System.Windows.Forms.Label();
            this.MyTime = new System.Windows.Forms.Label();
            this.MyEnergy = new System.Windows.Forms.Panel();
            this.EnemyEnergy = new System.Windows.Forms.Panel();
            this.UserMargin = new System.Windows.Forms.Panel();
            this.EnemyMargin = new System.Windows.Forms.Panel();
            this.MyName = new System.Windows.Forms.Label();
            this.EnemyName = new System.Windows.Forms.Label();
            this.toolTipHelp = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // UserHQPanel
            // 
            this.UserHQPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UserHQPanel.Location = new System.Drawing.Point(467, 438);
            this.UserHQPanel.Name = "UserHQPanel";
            this.UserHQPanel.Size = new System.Drawing.Size(90, 120);
            this.UserHQPanel.TabIndex = 2;
            // 
            // EnemyHQPanel
            // 
            this.EnemyHQPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EnemyHQPanel.Location = new System.Drawing.Point(467, 74);
            this.EnemyHQPanel.Name = "EnemyHQPanel";
            this.EnemyHQPanel.Size = new System.Drawing.Size(90, 120);
            this.EnemyHQPanel.TabIndex = 3;
            // 
            // MyCarte
            // 
            this.MyCarte.AutoScroll = true;
            this.MyCarte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MyCarte.Location = new System.Drawing.Point(90, 560);
            this.MyCarte.Name = "MyCarte";
            this.MyCarte.Size = new System.Drawing.Size(827, 125);
            this.MyCarte.TabIndex = 4;
            this.toolTipHelp.SetToolTip(this.MyCarte, "Это карты, которые находятся у вас в руке\r\nВы можете использовать любое количеств" +
        "о этих карт, \r\nесли у вас достаточно энергии");
            // 
            // EnemyCarte
            // 
            this.EnemyCarte.AutoScroll = true;
            this.EnemyCarte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EnemyCarte.Location = new System.Drawing.Point(82, 2);
            this.EnemyCarte.Name = "EnemyCarte";
            this.EnemyCarte.Size = new System.Drawing.Size(827, 70);
            this.EnemyCarte.TabIndex = 5;
            this.toolTipHelp.SetToolTip(this.EnemyCarte, "Карты находящиеся в руке противника.\r\nВы не можете видеть их, как и противник не " +
        "может \r\nвидеть ваши карты");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(582, 459);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Моя энергия:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(582, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Энергия противника:";
            // 
            // StepEnd
            // 
            this.StepEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StepEnd.Location = new System.Drawing.Point(953, 285);
            this.StepEnd.Name = "StepEnd";
            this.StepEnd.Size = new System.Drawing.Size(115, 60);
            this.StepEnd.TabIndex = 9;
            this.StepEnd.Text = "Ваш ход";
            this.StepEnd.UseVisualStyleBackColor = true;
            this.StepEnd.Click += new System.EventHandler(this.StepEnd_Click);
            // 
            // EnemyTime
            // 
            this.EnemyTime.AutoSize = true;
            this.EnemyTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EnemyTime.Location = new System.Drawing.Point(949, 116);
            this.EnemyTime.Name = "EnemyTime";
            this.EnemyTime.Size = new System.Drawing.Size(60, 24);
            this.EnemyTime.TabIndex = 10;
            this.EnemyTime.Text = "00:00";
            this.toolTipHelp.SetToolTip(this.EnemyTime, "Время, оставшееся до конца хода противника");
            // 
            // MyTime
            // 
            this.MyTime.AutoSize = true;
            this.MyTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MyTime.Location = new System.Drawing.Point(949, 552);
            this.MyTime.Name = "MyTime";
            this.MyTime.Size = new System.Drawing.Size(60, 24);
            this.MyTime.TabIndex = 11;
            this.MyTime.Text = "00:00";
            this.toolTipHelp.SetToolTip(this.MyTime, "Время оставшееся до конца вашего хода");
            // 
            // MyEnergy
            // 
            this.MyEnergy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MyEnergy.Location = new System.Drawing.Point(585, 478);
            this.MyEnergy.Name = "MyEnergy";
            this.MyEnergy.Size = new System.Drawing.Size(332, 28);
            this.MyEnergy.TabIndex = 12;
            this.toolTipHelp.SetToolTip(this.MyEnergy, "Без денег не обходится ни одна война");
            // 
            // EnemyEnergy
            // 
            this.EnemyEnergy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EnemyEnergy.Location = new System.Drawing.Point(585, 141);
            this.EnemyEnergy.Name = "EnemyEnergy";
            this.EnemyEnergy.Size = new System.Drawing.Size(332, 28);
            this.EnemyEnergy.TabIndex = 13;
            this.toolTipHelp.SetToolTip(this.EnemyEnergy, "Без денег не обходится ни одна война");
            // 
            // UserMargin
            // 
            this.UserMargin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UserMargin.Location = new System.Drawing.Point(90, 316);
            this.UserMargin.Name = "UserMargin";
            this.UserMargin.Size = new System.Drawing.Size(827, 120);
            this.UserMargin.TabIndex = 1;
            this.toolTipHelp.SetToolTip(this.UserMargin, "Поле для высадки ваших роботов");
            // 
            // EnemyMargin
            // 
            this.EnemyMargin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EnemyMargin.Location = new System.Drawing.Point(90, 196);
            this.EnemyMargin.Name = "EnemyMargin";
            this.EnemyMargin.Size = new System.Drawing.Size(827, 120);
            this.EnemyMargin.TabIndex = 0;
            this.toolTipHelp.SetToolTip(this.EnemyMargin, "Поле для высадки роботов противника");
            // 
            // MyName
            // 
            this.MyName.AutoSize = true;
            this.MyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MyName.Location = new System.Drawing.Point(54, 478);
            this.MyName.Name = "MyName";
            this.MyName.Size = new System.Drawing.Size(107, 25);
            this.MyName.TabIndex = 14;
            this.MyName.Text = "Мой ник: ";
            // 
            // EnemyName
            // 
            this.EnemyName.AutoSize = true;
            this.EnemyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EnemyName.Location = new System.Drawing.Point(54, 141);
            this.EnemyName.Name = "EnemyName";
            this.EnemyName.Size = new System.Drawing.Size(184, 25);
            this.EnemyName.TabIndex = 15;
            this.EnemyName.Text = "Ник противника: ";
            // 
            // toolTipHelp
            // 
            this.toolTipHelp.AutoPopDelay = 7000;
            this.toolTipHelp.InitialDelay = 500;
            this.toolTipHelp.ReshowDelay = 100;
            this.toolTipHelp.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipHelp.ToolTipTitle = "Подсказка";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CartGame.Properties.Resources.helpCardsGame1;
            this.pictureBox1.Location = new System.Drawing.Point(885, 522);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            this.toolTipHelp.SetToolTip(this.pictureBox1, "Чтобы увеличить карту, кликните по ней правой кнопкой мыши.\t\r\nЧтобы перетащить ка" +
        "рту, нажмите и удерживайте левую кнопку мыши.\r\nКак только вы отпустите кнопку, к" +
        "арта будет считаться отпущенной.");
            // 
            // GameMargin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1074, 696);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.EnemyName);
            this.Controls.Add(this.MyName);
            this.Controls.Add(this.EnemyEnergy);
            this.Controls.Add(this.MyEnergy);
            this.Controls.Add(this.EnemyHQPanel);
            this.Controls.Add(this.UserHQPanel);
            this.Controls.Add(this.MyTime);
            this.Controls.Add(this.EnemyTime);
            this.Controls.Add(this.StepEnd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EnemyCarte);
            this.Controls.Add(this.MyCarte);
            this.Controls.Add(this.UserMargin);
            this.Controls.Add(this.EnemyMargin);
            this.MaximizeBox = false;
            this.Name = "GameMargin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameMargin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameMargin_FormClosing);
            this.Load += new System.EventHandler(this.GameMargin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel UserHQPanel;
        private System.Windows.Forms.Panel EnemyHQPanel;
        private System.Windows.Forms.Panel MyCarte;
        private System.Windows.Forms.Panel EnemyCarte;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button StepEnd;
        private System.Windows.Forms.Label EnemyTime;
        private System.Windows.Forms.Label MyTime;
        private System.Windows.Forms.Panel MyEnergy;
        private System.Windows.Forms.Panel EnemyEnergy;
        private System.Windows.Forms.Panel UserMargin;
        private System.Windows.Forms.Panel EnemyMargin;
        private System.Windows.Forms.Label MyName;
        private System.Windows.Forms.Label EnemyName;
        private System.Windows.Forms.ToolTip toolTipHelp;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}