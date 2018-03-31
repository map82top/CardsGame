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
       
       // int[] IDUserCarte = new int[7];//всего можно выбрать 7 карт
        //bool[] BusySlot = new bool[7];
        List<Panel> CarteSlot = new List<Panel>(); //помогает узнать какие слоты заняты
        List<Panel> AllCarte = new List<Panel>();//все доступные карты
        bool MouseState = false;//показывает, что в данное время элемент перетаскивается
        Point MousePoint; //координаты мыши при перетаскивании

        Controler controler;
        DataGame ChoiceCards;
        public ChoiceForm( Controler controler)
        {
            InitializeComponent();
            this.controler = controler;
            ChoiceCards = controler.GetDataGame;
        }

        
        private void BackCarte_MouseDown(object sender, MouseEventArgs e)
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
            if (e.Button == MouseButtons.Right)
            {
                Panel temp = (Panel)sender;

                //если данная карта находится в panelUserCarte
                if (panelAllCarte.Location.X <= temp.Location.X && temp.Location.X <= panelAllCarte.Location.X + panelAllCarte.Width && panelAllCarte.Location.Y <= temp.Location.Y && temp.Location.Y <= panelAllCarte.Location.Y + panelAllCarte.Height)
                {
                    DeliteUserCarte(sender);
                }

                this.Controls.Remove(temp);
                PaintUserCarte();
                MouseState = false;
            }
        }
        /// <summary>
        /// Добавляет карту в карты игрока
        /// </summary>
        /// <param name="sender"></param>
        private void AddUserCarte(object sender)
        {
            int id = SeekIDAllCarte(sender);
            bool AllBusy = true;//показывает, что все слоты заняты
            if (id != -1)
            {
                //если не все слоты заняты
                int count = ChoiceCards.UserColoda.Length;
                for (int i = 0; i < count; i++)
                {
                    if (ChoiceCards.CarteFromUser[i] == 0)
                    { 
                        CarteSlot[i] = Carte.GetCarte(id).ImageCartNormal();//необходимо получить id для получения копии карты
                        AllBusy = false;
                        ChoiceCards.UserColoda[i] = id;
                        CarteSlot[i].MouseClick += new MouseEventHandler(Back_MouseClick);
                        CarteSlot[i].MouseEnter += new EventHandler(Panel_MouseEnter);
                        CarteSlot[i].MouseLeave += new EventHandler(Panel_MouseLeave);
                        CarteSlot[i].MouseDown += new MouseEventHandler(BackCarte_MouseDown);
                        CarteSlot[i].MouseMove += new MouseEventHandler(BackCarte_MouseMove);
                        CarteSlot[i].MouseUp += new MouseEventHandler(BackCarte_MouseUp);
                        break;

                    }
                }

                if (AllBusy)//добавляем в конец
                {
                    int Last = ChoiceCards.UserColoda.Length - 1;
                    CarteSlot[Last] = Carte.GetCarte(id).ImageCartNormal();
                    CarteSlot[Last].MouseClick += new MouseEventHandler(Back_MouseClick);
                    CarteSlot[Last].MouseEnter += new EventHandler(Panel_MouseEnter);
                    CarteSlot[Last].MouseLeave += new EventHandler(Panel_MouseLeave);
                    CarteSlot[Last].MouseDown += new MouseEventHandler(BackCarte_MouseDown);
                    CarteSlot[Last].MouseMove += new MouseEventHandler(BackCarte_MouseMove);
                    CarteSlot[Last].MouseUp += new MouseEventHandler(BackCarte_MouseUp);
                    ChoiceCards.UserColoda[Last] = id;
                }

                PaintUserCarte();//перерисовываем
            }
        }
        private void Carte_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) AddUserCarte(sender);
            else//показываем пользователя полную версия карты
            {
                if (!MouseState)
                {
                    //получаем координаты карты
                    Panel temp = (Panel)sender;
                    Point Location = new Point(panelAllCarte.Location.X + temp.Location.X - 40, panelAllCarte.Location.Y + temp.Location.Y - 40);
                    if (Location.X < panelAllCarte.Location.X) Location.X = 0;
                    if (Location.Y + temp.Height > panelAllCarte.Location.Y + panelAllCarte.Height - Location.Y) Location.Y -= 40;
                    int Length = AllCarte.Count;
                    int id = SeekIDAllCarte(sender);

                    Panel CarteMax = Carte.GetCarte(id).ImageCartMax();
                    CarteMax.Location = Location;
                    CarteMax.BackColor = SystemColors.Window;
                    CarteMax.MouseLeave += new EventHandler(MaxCarte_MouseLeave);

                    //добавяем в коллекцию формы

                    this.Controls.Add(CarteMax);
                    CarteMax.BringToFront();
                }
                
            }

        }
        private void DeliteUserCarte(object sender)
        {

            //если не все слоты заняты
            int count = ChoiceCards.UserColoda.Length;
            for (int i = 0; i <count; i++)
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
        private void Back_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) DeliteUserCarte(sender);
            else//показываем пользователю полную версию карты
            {
                if (!MouseState)
                {
                    //получаем координаты карты
                    Panel temp = (Panel)sender;
                    Point Location = new Point(panelAllCarte.Location.X + temp.Location.X - 40, panelAllCarte.Location.Y + temp.Location.Y - 40);

                    if (Location.Y + temp.Height > panelUserCarte.Location.Y + panelUserCarte.Height - Location.Y) Location.Y -= 40;
                    int Length = AllCarte.Count;
                    int id = SeekIDUserCarte(sender);

                    Panel CarteMax = Carte.GetCarte(id).ImageCartMax();
                    CarteMax.Location = Location;
                    CarteMax.BackColor = SystemColors.Window;
                    CarteMax.MouseLeave += new EventHandler(MaxCarte_MouseLeave);


                    //добавяем в коллекцию формы

                    this.Controls.Add(CarteMax);
                    CarteMax.BringToFront();
                }
            }

        }
        private void Carte_MouseDown(object sender, MouseEventArgs e)
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
            if (e.Button == MouseButtons.Right)
            {
                Panel temp = (Panel)sender;

                //если данная карта находится в panelUserCarte
                if (panelUserCarte.Location.X <= temp.Location.X && temp.Location.X <= panelUserCarte.Location.X + panelUserCarte.Width && panelUserCarte.Location.Y <= temp.Location.Y && temp.Location.Y <= panelUserCarte.Location.Y + panelUserCarte.Height)
                {
                    AddUserCarte(sender);
                }

                this.Controls.Remove(temp);
                PaintAllCarte();
                MouseState = false;
            }
        }
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
        
        private void MaxCarte_MouseLeave(object sender, EventArgs e)
        {
            this.Controls.Remove((Control)sender);
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
                    NewPanel.MouseDown += new MouseEventHandler(Carte_MouseDown);
                    NewPanel.MouseMove += new MouseEventHandler(Carte_MouseMove);
                    NewPanel.MouseUp += new MouseEventHandler(Carte_MouseUp);
                    AllCarte.Add(NewPanel);
                    if (i % 2 != 0) Y += 190;
                    else { X += 148; Y = 3;}
                    panelAllCarte.Controls.Add(NewPanel);
                    
                }
            }
            
            //выводим на экран пустые карты для наглядности
            X = 15; Y = 3;
            int count = ChoiceCards.UserColoda.Length;
            for (int i = 0; i < count; i++)
            {
                CarteSlot.Add(CreateEmptyCarte());
                
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


        private int SeekIDAllCarte(object sender)
        {

            Panel temp = (Panel)sender;
            int Max = AllCarte.Count;
            for (int i = 0; i < Max; i++)
            {
                 
                if (AllCarte[i] == temp)
                    return i + 1;
            }
           
            
            return -1;
        }

        private int SeekIDUserCarte(object sender)
        {

            Panel temp = (Panel)sender;
            int Max = CarteSlot.Count;
           
            for (int i = 0; i < Max; i++)
            {

                if (CarteSlot[i] == temp)
                    return ChoiceCards.UserColoda[i];
            }


            return -1;
        }

       
        private void buttonStartSeek_Click(object sender, EventArgs e)
        {
            

            
            int count = ChoiceCards.UserColoda.Length;
            for (int i = 0; i < count; i++)
            {
                if(ChoiceCards.UserColoda[i]==0)
                {
                    FewCarte.SetError(buttonStartSeek, ".Вы выбрали недостаточно карт для начал игры \n!");
                    return;
                }
            }

            //добавить в ручную выбирать адрес и порт

            try
            {
                var StreamInfo = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var StreamRead = new StreamReader(StreamInfo);
                string[] Data = StreamRead.ReadLine().Split(':');
                //закрываем поток обработки файла
                StreamRead.Close();
                StreamInfo.Close();
               
                //подключаемся к серверу
                buttonStartSeek.Enabled = false;
                controler.SucConnect += SuccessConnect;
                controler.Start(IPAddress.Parse(Data[0]), int.Parse(Data[1]), Data[2]);
                
               
                   
              
            }

            catch (FormatException)
            {
                buttonStartSeek.Enabled = false;
                Settings NewSettings = new Settings(this);
                MessageBox.Show("Неверный формат данных");  
            }

            catch (DirectoryNotFoundException)
            {
                buttonStartSeek.Enabled = false;
                Settings NewSettings = new Settings(this);
                MessageBox.Show("Ошибка к доступу к фалу с настройками!");

            }
            catch (SocketException)
            {
                MessageBox.Show("Неудалось подключиться к серверу!");
                buttonStartSeek.Enabled = true;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }
        public void buttonEnabled()
        {
            buttonStartSeek.Enabled = true;
        }

        private void SuccessConnect(Controler sender)
        {
            
            //отвязываем этот обработчик
            sender.SucConnect -= SuccessConnect;
            SeekForm NewForm = new SeekForm(this, sender);
            this.Hide();
            NewForm.Show();
        }

    }
}
