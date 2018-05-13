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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoiceForm));
            this.panelUserCarte = new System.Windows.Forms.Panel();
            this.labelAll = new System.Windows.Forms.Label();
            this.labelUser = new System.Windows.Forms.Label();
            this.panelAllCarte = new System.Windows.Forms.Panel();
            this.buttonStartSeek = new System.Windows.Forms.Button();
            this.FewCarte = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.менюToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.помощьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.обАвтореToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTipHelp = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.FewCarte)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelUserCarte
            // 
            this.panelUserCarte.AutoScroll = true;
            this.panelUserCarte.BackColor = System.Drawing.SystemColors.Window;
            this.panelUserCarte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUserCarte.Location = new System.Drawing.Point(603, 43);
            this.panelUserCarte.Name = "panelUserCarte";
            this.panelUserCarte.Size = new System.Drawing.Size(212, 412);
            this.panelUserCarte.TabIndex = 1;
            this.toolTipHelp.SetToolTip(this.panelUserCarte, "Карты в вашей колоде могут повторяться\r\nЧтобы удалить карту из своей колоды, клик" +
        "ните по ней левой кнопкой мыши.\r\nЧтобы увеличить карту, кликните по ней правой к" +
        "нопкой мыши.\r\n");
            // 
            // labelAll
            // 
            this.labelAll.AutoSize = true;
            this.labelAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelAll.Location = new System.Drawing.Point(12, 23);
            this.labelAll.Name = "labelAll";
            this.labelAll.Size = new System.Drawing.Size(151, 16);
            this.labelAll.TabIndex = 2;
            this.labelAll.Text = "Все доступные карты:";
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelUser.Location = new System.Drawing.Point(600, 23);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(87, 16);
            this.labelUser.TabIndex = 3;
            this.labelUser.Text = "Ваши карты:";
            // 
            // panelAllCarte
            // 
            this.panelAllCarte.AutoScroll = true;
            this.panelAllCarte.BackColor = System.Drawing.SystemColors.Window;
            this.panelAllCarte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAllCarte.Location = new System.Drawing.Point(12, 42);
            this.panelAllCarte.Name = "panelAllCarte";
            this.panelAllCarte.Size = new System.Drawing.Size(576, 413);
            this.panelAllCarte.TabIndex = 0;
            this.toolTipHelp.SetToolTip(this.panelAllCarte, "Чтобы добавить карту в свою колоду, кликните по ней левой кнопкой мыши. \r\nЧтобы у" +
        "величить карту кликните по ней правой кнопкой мыши.");
            // 
            // buttonStartSeek
            // 
            this.buttonStartSeek.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonStartSeek.Location = new System.Drawing.Point(608, 461);
            this.buttonStartSeek.Name = "buttonStartSeek";
            this.buttonStartSeek.Size = new System.Drawing.Size(207, 44);
            this.buttonStartSeek.TabIndex = 5;
            this.buttonStartSeek.Text = "Начать поиск противника";
            this.toolTipHelp.SetToolTip(this.buttonStartSeek, "Не забудьте перед 1 боем зайти в настройки зайти в настройки");
            this.buttonStartSeek.UseVisualStyleBackColor = true;
            this.buttonStartSeek.Click += new System.EventHandler(this.buttonStartSeek_Click);
            // 
            // FewCarte
            // 
            this.FewCarte.ContainerControl = this;
            this.FewCarte.RightToLeft = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.менюToolStripMenuItem,
            this.обАвтореToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(827, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "Меню";
            // 
            // менюToolStripMenuItem
            // 
            this.менюToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкиToolStripMenuItem,
            this.помощьToolStripMenuItem});
            this.менюToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.менюToolStripMenuItem.Name = "менюToolStripMenuItem";
            this.менюToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.менюToolStripMenuItem.Text = "Меню";
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            this.настройкиToolStripMenuItem.Click += new System.EventHandler(this.настройкиToolStripMenuItem_Click);
            // 
            // помощьToolStripMenuItem
            // 
            this.помощьToolStripMenuItem.Name = "помощьToolStripMenuItem";
            this.помощьToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.помощьToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.помощьToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.помощьToolStripMenuItem.Text = "Помощь";
            this.помощьToolStripMenuItem.Click += new System.EventHandler(this.помощьToolStripMenuItem_Click);
            // 
            // обАвтореToolStripMenuItem
            // 
            this.обАвтореToolStripMenuItem.Name = "обАвтореToolStripMenuItem";
            this.обАвтореToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.обАвтореToolStripMenuItem.Text = "Об авторе";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 475);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Создайте свою колоду размером от 3 до 15 карт.\r\n";
            this.label1.Click += new System.EventHandler(this.label1_Click);
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
            this.pictureBox1.Location = new System.Drawing.Point(345, 461);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            this.toolTipHelp.SetToolTip(this.pictureBox1, resources.GetString("pictureBox1.ToolTip"));
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // ChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 517);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonStartSeek);
            this.Controls.Add(this.labelUser);
            this.Controls.Add(this.labelAll);
            this.Controls.Add(this.panelUserCarte);
            this.Controls.Add(this.panelAllCarte);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ChoiceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выберите карты";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChoiceForm_FormClosing);
            this.Load += new System.EventHandler(this.ChoiceForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FewCarte)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelUserCarte;
        private System.Windows.Forms.Label labelAll;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.Panel panelAllCarte;
        private System.Windows.Forms.Button buttonStartSeek;
        private System.Windows.Forms.ErrorProvider FewCarte;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem менюToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem обАвтореToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem помощьToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTipHelp;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}