using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace CartGame
{
    public partial class ChoiceForm : Form
    {
        const string text = "system_info/info_server.txt";
       
        List<Panel> CarteSlot = new List<Panel>(); //помогает узнать какие слоты заняты
        List<Panel> AllCarte = new List<Panel>();//все доступные карты
        //bool MouseState = false;//показывает, что в данное время элемент перетаскивается
        //Point MousePoint; //координаты мыши при перетаскивании
        bool OpenSeekForm = true;
        Controler controler;
        DataGame ChoiceCards;
        const int ValueCardsUser = 15;//максимальное количество карт в колоде равно 15
       
        public ChoiceForm( Controler controler)
        {
            InitializeComponent();
            this.controler = controler;
            ChoiceCards = controler.GetDataGame;        
        }

       /*private void BackCarte_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Panel temp = (Panel)sender;
                //удаяляем их panel и добавляем в основуню форму, чтобы можно было перетаскивать в другую panel 
                panelUserCarte.Controls.Remove(temp);
                int Index = this.Controls.Count;
                this.Controls.Add(temp);
                temp.BringToFront();
                MousePoint = new Point(e.X, e.Y);
                MouseState = true;
            }
        }
        private void BackCarte_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseState)
            {
                Panel temp = (Panel)sender;
                int dx = e.X - MousePoint.X;
                int dy = e.Y - MousePoint.Y;
                temp.Location = new Point(temp.Location.X + dx, temp.Location.Y + dy);
            }
        }
        private void BackCarte_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && MouseState)
            {
                Panel temp = (Panel)sender;

                //ничего не деламе так как карта все равно будет дабавлена, так как произошло событие click
              
                this.Controls.Remove(temp);
                PaintUserCarte();
                MouseState = false;
            }
        }*/
        /// <summary>
        /// Добавляет карту в карты игрока
        /// </summary>
        /// <param name="sender"></param>
        private void AddUserCarte(object sender)
        {
            int id = (int)(sender as Panel).Tag;
            bool AllBusy = true;//показывает, что все слоты заняты при 
            if (id != -1)
            {
                //если не все слоты заняты
                 
                for (int i = 0; i < ValueCardsUser; i++)
                {
                    if (ChoiceCards.UserColoda[i] == 0)
                    {
                        Carte TempCarte = Carte.GetCarte(id);//необходимо получить id для получения копии карты
                        CarteSlot[i] = TempCarte.ImageCartNormal();
                        AllBusy = false;
                        ChoiceCards.UserColoda[i] = id;
                        CarteSlot[i].MouseClick += new MouseEventHandler(Back_MouseClick);
                        CarteSlot[i].MouseEnter += new EventHandler(Panel_MouseEnter);
                        CarteSlot[i].MouseLeave += new EventHandler(Panel_MouseLeave);
                        /*CarteSlot[i].MouseDown += new MouseEventHandler(BackCarte_MouseDown);
                        CarteSlot[i].MouseMove += new MouseEventHandler(BackCarte_MouseMove);
                        CarteSlot[i].MouseUp += new MouseEventHandler(BackCarte_MouseUp);*/
                        //добавляем подсказку
                        AddCardHelp(CarteSlot[i], TempCarte, false);
                        break;

                    }
                }

                if (AllBusy)//добавляем в конец
                {
                    int Last = ValueCardsUser-1;
                    Carte TempCarte = Carte.GetCarte(id);
                    CarteSlot[Last] = TempCarte.ImageCartNormal();
                    CarteSlot[Last].MouseClick += new MouseEventHandler(Back_MouseClick);
                    CarteSlot[Last].MouseEnter += new EventHandler(Panel_MouseEnter);
                    CarteSlot[Last].MouseLeave += new EventHandler(Panel_MouseLeave);
                    /*CarteSlot[Last].MouseDown += new MouseEventHandler(BackCarte_MouseDown);
                    CarteSlot[Last].MouseMove += new MouseEventHandler(BackCarte_MouseMove);
                    CarteSlot[Last].MouseUp += new MouseEventHandler(BackCarte_MouseUp);*/
                    AddCardHelp(CarteSlot[Last], TempCarte, false);
                }

                PaintUserCarte();//перерисовываем
            }
        }
        private void Carte_Click(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {

                AddUserCarte(sender);
            }
            else  ShowMaxCard(sender, panelAllCarte);
            
        }
        private void DeliteUserCarte(object sender)
        {

            //если не все слоты заняты
            
            for (int i = 0; i < ValueCardsUser; i++)
            {
                if (CarteSlot[i] == (Panel)sender)
                {
                    Panel temp = (Panel)sender;
                    if (temp.BorderStyle == BorderStyle.FixedSingle)
                    CarteSlot[i] = CreateEmptyCarte();//делаем карту пустой
                    ChoiceCards.UserColoda[i] = 0;
                    break;

                }

            }
            PaintUserCarte();//перерисовываем
        }
        public void ShowMaxCard(object sender, Panel ParentPanel)
        {
                //получаем координаты карты
                Panel temp = (Panel)sender;
                Point Location = new Point(ParentPanel.Location.X + temp.Location.X - 40, ParentPanel.Location.Y + temp.Location.Y - 40);
                //cоздаем изображение карты
                int id = (int)(sender as Panel).Tag;
                Panel CarteMax = Carte.GetCarte(id).ImageCartMax();
                //проверка на выход за границы формы по оси y
                if (Location.Y + CarteMax.Height > this.Height) Location.Y -= Math.Abs(this.Height - 35 - Location.Y - CarteMax.Height);
                else if (Location.Y < 0) Location.Y = 5;

                if (Location.X + CarteMax.Width > this.Width) Location.Y -= Math.Abs(this.Width - 5 - Location.X - CarteMax.Width);
                else if (Location.X < 0) Location.X = 5;
                //получаем id карты
                
                
                CarteMax.Location = Location;
                CarteMax.BackColor = SystemColors.Window;
                CarteMax.MouseLeave += new EventHandler(MaxCarte_Delite);
                CarteMax.MouseClick += new MouseEventHandler(MaxCarte_Delite);


                //добавяем в коллекцию формы

                this.Controls.Add(CarteMax);
                CarteMax.BringToFront();
        }
        private void Back_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) DeliteUserCarte(sender);
            else//показываем пользователю полную версию карты
            {
                ShowMaxCard(sender, panelUserCarte);
            }

        }
        /*private void Carte_MouseDown(object sender, MouseEventArgs e)
        {
            
                if (e.Button == MouseButtons.Right)
                {
                 
                    Panel temp = (Panel)sender;
                    //удаяляем их panel и добавляем в основуню форму, чтобы можно было перетаскивать в другую panel 
                    panelAllCarte.Controls.Remove(temp);

                    this.Controls.Add(temp);
                    temp.BringToFront();
                    MousePoint = new Point(e.X, e.Y);
                    MouseState = true;

                }
        }
        private void Carte_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseState)
            {
                Panel temp = (Panel)sender;
                int dx = e.X - MousePoint.X;
                int dy = e.Y - MousePoint.Y;
                temp.Location = new Point(temp.Location.X + dx, temp.Location.Y + dy);
            }
        }
        private void Carte_MouseUp(object sender, MouseEventArgs e)
        {
            
                if (e.Button == MouseButtons.Right && MouseState)
                {
                    Panel temp = (Panel)sender;
                    //ничего не деламе так как карта все равно будет дабавлена, так как произошло событие click

                    this.Controls.Remove(temp);
                    PaintAllCarte();
                    MouseState = false;

                }
        }*/
        private void PaintAllCarte()
        {
            int X = 3, Y = 3;
            panelAllCarte.Controls.Clear();
            int Count = AllCarte.Count;
            for (int i = 0; i < Count; i++)
            {
                AllCarte[i].Location = new Point(X, Y);
                panelAllCarte.Controls.Add(AllCarte[i]);
                if ((i + 1) % 2 != 0) Y += 190;
                else { X += 148; Y = 3; }
            }
        }
        
        private void MaxCarte_Delite(object sender, EventArgs e)
        {
            this.Controls.Remove((Control)sender);
        }

        /// <summary>
        /// Присоединяет к изображению карты подсказку
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="card"></param>
        private void AddCardHelp(Panel panel, Carte card, bool UserOrAll)
        {
            string HelpMsg = null;

            if (card is Event) HelpMsg = Properties.Resources.HelpEvent; 
                else if (card is DefenceConstr) HelpMsg = Properties.Resources.HelpDefenderConstr; 
                else HelpMsg = Properties.Resources.HelpRobot; 
             toolTipHelp.SetToolTip(panel, HelpMsg );
           
            
        }
        private void ChoiceForm_Load(object sender, EventArgs e)
        {
            //выводим на экран все доступные карты
            Carte TempCarte;//здесь временно храним экземпляр карты, пока не отобразим её на экране
            int X = 3, Y = 3;
           
            for (int i = 1; true; i++)
            {
                if ((TempCarte = Carte.GetCarte(i)) == null) break;
                else
                {
                    Panel NewPanel = TempCarte.ImageCartNormal();
                    NewPanel.Location = new Point(X, Y);
                    NewPanel.MouseClick += new MouseEventHandler(Carte_Click);
                    NewPanel.MouseEnter += new EventHandler(Panel_MouseEnter);
                    NewPanel.MouseLeave += new EventHandler(Panel_MouseLeave);
                    /*NewPanel.MouseDown += new MouseEventHandler(Carte_MouseDown);
                    NewPanel.MouseMove += new MouseEventHandler(Carte_MouseMove);
                    NewPanel.MouseUp += new MouseEventHandler(Carte_MouseUp);*/
                   
                    AllCarte.Add(NewPanel);
                    if (i % 2 != 0) Y += 190;
                    else { X += 148; Y = 3;}
                    panelAllCarte.Controls.Add(NewPanel);
                    //добавляем подсказку
                    AddCardHelp(NewPanel, TempCarte, true);


                }
            }
            
            //выводим на экран пустые карты для наглядности
            //или карты из предудущей колоды игрока
           
            for (int i = 0; i < ValueCardsUser; i++)
            {
                if (ChoiceCards.UserColoda[i] == 0)
                    CarteSlot.Add(CreateEmptyCarte());
                else
                {
                    CarteSlot.Add(Carte.GetCarte(ChoiceCards.UserColoda[i]).ImageCartNormal());//необходимо получить id для получения копии карты
                    //привязываем обработчики к данным картам
                    CarteSlot[i].MouseClick += new MouseEventHandler(Back_MouseClick);
                    CarteSlot[i].MouseEnter += new EventHandler(Panel_MouseEnter);
                    CarteSlot[i].MouseLeave += new EventHandler(Panel_MouseLeave);
                } 
                
            }
            PaintUserCarte();//отрисовываем
        }
        /// <summary>
        /// выделяем эту карту
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseEnter(object sender, EventArgs e)
        {
            Panel temp = (Panel)sender;
            temp.BorderStyle = BorderStyle.FixedSingle;
        }
        private void Panel_MouseLeave(object sender, EventArgs e)
        {
            Panel temp = (Panel)sender;
            temp.BorderStyle = BorderStyle.None;

        }
        private void PaintUserCarte()
        {
            Point scroll = panelUserCarte.AutoScrollPosition;
               //сохраняем положение прокрутки
            int X = 15, Y = 3;
            panelUserCarte.Controls.Clear();
            int count = ChoiceCards.UserColoda.Length;
            for (int i = 0; i < count; i++)
            {
                panelUserCarte.Controls.Add(CarteSlot[i]);
                panelUserCarte.Controls[i].Location = new Point(X, Y);
                Y += 190;

            }
            scroll.Y = -scroll.Y;
            panelUserCarte.AutoScrollPosition = scroll;//востанавливаем положение прокрутки
        }
        /// <summary>
        /// Создаем Panel с пустой картой
        /// </summary>
        /// <returns></returns>
        private Panel CreateEmptyCarte()
        {
            Panel NewEmptyCarte = new Panel();
            NewEmptyCarte.Size = new Size(142, 184);
            NewEmptyCarte.BackgroundImage = Properties.Resources.EmptyCarteNormal;
            //загружаем изображение в panel
            
            return NewEmptyCarte;

        }

        private void buttonStartSeek_Click(object sender, EventArgs e)
        {

            try
            {

                int CountCardNONull = 0;//считчик количества карт в колоде игрока
                for (int i = 0; i < ValueCardsUser; i++)
                    if (ChoiceCards.UserColoda[i] != 0) CountCardNONull++;
                if (CountCardNONull < 3)//если карт меньше 3 в бой выйти нельзя
                {
                    FewCarte.SetError(buttonStartSeek, ".Вы выбрали недостаточно карт для начал игры \n!");
                    return;
                }

                //добавить в ручную выбирать адрес и порт


                using (var StreamInfo = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var StreamRead = new StreamReader(StreamInfo))
                    {
                        string[] Data = StreamRead.ReadLine().Split(':');//данные считываемые из файла

                        //если данные существуют
                        if (Data != null)
                        {
                            //подключаемся к серверу
                            buttonStartSeek.Enabled = false;
                            //привязваем обработчик  для обработчки сообщения о неудачном сообщении
                            controler.SucConnect += SuccessConnect;
                            controler.Start(IPAddress.Parse(Data[0]), int.Parse(Data[1]), Data[2]);
                        }
                        else throw new DirectoryNotFoundException();
                    }
                }
            }

            catch (FormatException)
            {
                MessageBox.Show("Неверный формат данных");
                Settings NewSettings = new Settings(this);
                NewSettings.ShowDialog();
               
            }

            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Ошибка к доступу к фалу с настройками!");
                Settings NewSettings = new Settings(this);
                NewSettings.ShowDialog();
               

            }
            catch (SocketException)
            {
                controler.SucConnect -= SuccessConnect;
                MessageBox.Show("Неудалось подключиться к серверу!");
                buttonStartSeek.Enabled = true;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
        }
       
        private void SuccessConnect(Controler sender)
        {    
            //отвязываем этот обработчик
            sender.SucConnect -= SuccessConnect;
            SeekForm NewForm = new SeekForm(sender);
            OpenSeekForm = false;
            this.Close();
            NewForm.Show();
        }

        private void ChoiceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           if(OpenSeekForm) Application.Exit();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            Settings NewForm = new Settings(this);
            NewForm.ShowDialog();
        }

        private void помощьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Help.ShowHelp(this, "HelpCardsGame.chm");
            }
            catch (Exception E)
            {
                MessageBox.Show("Ошибка доступа к справке!");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
