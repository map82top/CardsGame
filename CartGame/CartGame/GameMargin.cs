using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace CartGame
{
    public partial class GameMargin : Form
    {

        private Point MousePoint;//координата карты в момент ее перетаскивания
        private bool AllowProgress;//если true то мой ход
        private int NumberCarte;//номер карты в списке карт

        private SendAndRecMsg СomWithServer;
        private Controler ClientContr;
        private DataGame DataSession;

        private bool MouseState;//если true мышь перетаскивает карту
        private List<Panel> UserCards;//карты в руке и игрока
        private List<Panel> UserCardsOfMargin;//изображения карт на поле
        private List<Panel> EnemyCardsOfMargin;//изображения вражеских карт на поле
        private Mutex AttackMtx = new Mutex();//обсеспечивает безопасную работу с списком пользовательских карт

        //сообщает о ходе начале хода игрока
        private Label NotificLabel;
        private System.Windows.Forms.Timer NotificTimer;

        public GameMargin(Controler controler)
        {
            InitializeComponent();

            ClientContr = controler;
            DataSession = controler.GetDataGame;//получаем доступ к игровым данным
            
            ClientContr.PaintUserCarte += MyCarte_Add;
            ClientContr.PaintEnemyCarte += EnemyCarte_Add;



        }
        private void EnemyAddCardsOnMyCarte()
        {
            this.Invoke((MethodInvoker)delegate
            {
                lock (EnemyCarte)
                {
                    if (EnemyCarte.Controls.Count > 0)
                    {
                        EnemyCarte.Controls.RemoveAt(0);
                        UpdateLocation_Cards(EnemyCarte, 70, 3);
                    }

                }

                //добавляем карту на панель


                int i = DataSession.EnCarteOnField.Count - 1;
                Panel temp = DataSession.EnCarteOnField[i].ImageCartMin();
                temp.MouseEnter += Carte_MouseEnter;
                temp.MouseLeave += Carte_MouseLeave;


                EnemyCardsOfMargin.Add(temp);

                EnemyMargin.Controls.Add(temp);
                UpdateLocation_Cards(EnemyMargin, EnemyMargin.Controls[0].Width, 3);

            });
        }
        private void Carte_MouseEnter(object sender, EventArgs e)
        {
            Panel temp = (Panel)sender;
            temp.BorderStyle = BorderStyle.FixedSingle;
        }
        private void Carte_MouseLeave(object sender, EventArgs e)
        {
            Panel temp = (Panel)sender;
            temp.BorderStyle = BorderStyle.None;
        }
        private void MyCarte_MouseDown(object sender, MouseEventArgs e)
        {
            lock (MyCarte)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Panel temp = (Panel)sender;
                    NumberCarte = 0;
                    int count = MyCarte.Controls.Count;
                    for (; NumberCarte < count; NumberCarte++)
                    {
                        if (MyCarte.Controls[NumberCarte] == temp) break;
                    }
                    //удаляем карту
                    MyCarte.Controls.RemoveAt(NumberCarte);

                    this.Controls.Add(temp);
                    temp.BringToFront();
                    MousePoint = new Point(e.X, e.Y);
                    MouseState = true;

                }
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
        private void MyCarte_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Panel temp = (Panel)sender;
                if (AllowProgress)
                {
                    if (temp.Location.X + temp.Width / 2 >= temp.Location.X + temp.Width / 2 &&
                        temp.Location.Y + temp.Height / 4 <= UserMargin.Location.X + UserMargin.Width && temp.Location.Y + temp.Height / 4 >= UserMargin.Location.Y && temp.Location.Y + temp.Height / 4 <= UserMargin.Location.Y + UserMargin.Height)
                    {

                        //отправляем сообщение о том что необходимо отправить добавить карты на поле, если достаточно ресурсов
                        СomWithServer.Send(NumberCarte, MsgType.AddCarteOnField);
                    }
                }

                //удаляем перетаскиваемую карточку
                this.Controls.Remove(temp);
                NumberCarte = 0;
                MouseState = false;
                //очищаем панель

                MyCarte.Controls.Clear();
                int count = UserCards.Count;
                //заполняем массив заново
                for (int i = 0; i < count; i++)
                {
                    MyCarte.Controls.Add(UserCards[i]);
                }
                //устанваливаем позицию карт
                UpdateLocation_Cards(MyCarte, MyCarte.Controls[0].Width, 3);


            }
        }
        /// <summary>
        /// Добавляем карту на игровое поле игрока
        /// </summary>
        /// <param name="number"></param>
        private void AddCardsOnMyCarte(int number)
        {

            //удаляем карты из MyCarte
            this.Invoke((MethodInvoker)delegate
            {
                lock (MyCarte)
                {
                    MyCarte.Controls.RemoveAt(number);
                    UserCards.RemoveAt(number);
                    UpdateLocation_Cards(MyCarte, MyCarte.Controls[0].Width, 3);
                }

                //добавляем карту на панель

                int i = DataSession.UsCarteOnField.Count - 1;
                Panel temp = DataSession.UsCarteOnField[i].ImageCartMin();
                temp.MouseEnter += Carte_MouseEnter;
                temp.MouseLeave += Carte_MouseLeave;
                temp.MouseDown += MarginCarte_MouseDown;
                temp.MouseMove += Carte_MouseMove;
                temp.MouseUp += MarginCarte_MouseUp;

                UserCardsOfMargin.Add(temp);


                UserMargin.Controls.Add(temp);
                UpdateLocation_Cards(UserMargin, UserMargin.Controls[0].Width, 3);


            });


        }

        private void MarginCarte_MouseDown(object sender, MouseEventArgs e)
        {
            lock (UserMargin)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Panel temp = (Panel)sender;
                    NumberCarte = 0;
                    int count = UserMargin.Controls.Count;
                    //ищем карту в списке карт
                    for (; NumberCarte < count; NumberCarte++)
                    {
                        if (UserMargin.Controls[NumberCarte] == temp) break;
                    }
                    //удаляем карту
                    UserMargin.Controls.RemoveAt(NumberCarte);

                    this.Controls.Add(temp);
                    temp.BringToFront();
                    MousePoint = new Point(e.X, e.Y);
                    MouseState = true;

                }
            }

        }
        private void MarginCarte_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    Panel temp = (Panel)sender;

                    if (AllowProgress)//если разрешена отпрака сообщений 
                    {
                        //если мышка находится на поле вражеского штаба
                        if (temp.Location.X + temp.Width / 2 >= EnemyHQPanel.Location.X && temp.Location.X + temp.Width / 2 <= EnemyHQPanel.Location.X + EnemyHQPanel.Width &&
                            temp.Location.Y + temp.Height / 4 >= EnemyHQPanel.Location.Y && temp.Location.Y + temp.Height / 4 <= EnemyHQPanel.Location.Y + EnemyHQPanel.Height)
                        {

                            //атака на штаб
                            СomWithServer.Send(new int[] { NumberCarte, -1 }, MsgType.Attack);
                        }
                        //если мышка находится на поле карт
                        if ((temp.Location.X + temp.Width / 2 >= EnemyMargin.Location.X && temp.Location.X + temp.Width / 2 <= EnemyMargin.Location.X + EnemyMargin.Width &&
                            temp.Location.Y + temp.Height / 4 >= EnemyMargin.Location.Y && temp.Location.Y + temp.Height / 4 <= EnemyMargin.Location.Y + EnemyMargin.Height))
                        {
                            int Count = EnemyMargin.Controls.Count;
                            for (int i = 0; i < Count; i++)
                            {
                                if (temp.Location.X + temp.Width / 2 >= EnemyMargin.Location.X + EnemyMargin.Controls[i].Location.X &&
                                    temp.Location.X + temp.Width / 2 <= EnemyMargin.Location.X + EnemyMargin.Controls[i].Location.X + EnemyMargin.Controls[i].Width &&
                                    temp.Location.Y + temp.Height / 4 >= EnemyMargin.Location.Y + EnemyMargin.Controls[i].Location.Y &&
                                    temp.Location.Y + temp.Height / 4 <= EnemyMargin.Location.Y + EnemyMargin.Controls[i].Location.Y + EnemyMargin.Controls[i].Height)
                                {
                                    //атака на карточку
                                    СomWithServer.Send(new int[] { NumberCarte, i }, MsgType.Attack);
                                    break;
                                }
                            }
                        }
                    }

                    //удаляем перетаскиваемую карточку
                    this.Controls.Remove(temp);
                    NumberCarte = 0;
                    MouseState = false;
                    //очищаем панель

                    AttackMtx.WaitOne();
                    UserMargin.Controls.Clear();
                    int count = UserCardsOfMargin.Count;
                    //заполняем массив заново
                    for (int i = 0; i < count; i++)
                    {
                        UserMargin.Controls.Add(UserCardsOfMargin[i]);
                    }
                    //устанваливаем позицию карт
                    UpdateLocation_Cards(UserMargin, 90, 3);
                    AttackMtx.ReleaseMutex();


                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }



        }
        private void UpdateLocation_Cards(Panel Margin, int WidthCart, int DistCards)
        {
            int count = Margin.Controls.Count;
            Debug.WriteLine("Count равен:" + count);
            try
            {
                if (count > 0)
                {
                    if (count % 2 == 0)
                    {
                        Point Location = new Point(Margin.Width / 2, 0);
                        int midle = count / 2;

                        for (int i = midle - 1; i >= 0; i--)
                        {
                            Location.X -= WidthCart + DistCards;
                            this.Invoke((MethodInvoker)delegate { Margin.Controls[i].Location = Location; });
                        }
                        Location.X = Margin.Width / 2;
                        this.Invoke((MethodInvoker)delegate { Margin.Controls[midle].Location = Location; }); midle++;
                        for (; midle < count; midle++)
                        {

                            Location.X += WidthCart + DistCards;
                            this.Invoke((MethodInvoker)delegate { Margin.Controls[midle].Location = Location; });
                        }
                    }
                    else
                    {
                        Point Location = new Point((Margin.Width / 2) - (WidthCart / 2), 0);
                        int midle = count / 2;

                        this.Invoke((MethodInvoker)delegate { Margin.Controls[midle].Location = Location; });


                        for (int i = midle - 1; i >= 0; i--)
                        {
                            Location.X -= WidthCart + DistCards;
                            this.Invoke((MethodInvoker)delegate { Margin.Controls[i].Location = Location; });

                        }
                        Location.X = (Margin.Width / 2) + (WidthCart / 2);

                        for (int j = midle + 1; j < count; j++)
                        {
                            if (j != midle + 1)
                            {
                                Location.X += WidthCart + DistCards;

                            }
                            this.Invoke((MethodInvoker)delegate { Margin.Controls[j].Location = Location; });


                        }
                    }
                }
            }
            catch (ArgumentNullException)
            { //записываем в лог
                MessageBox.Show("Ошибка синхронизиции");
            }


        }
        /// <summary>
        /// обрабатывает добавление карты на панель карт находящихся в руке у игрока
        /// </summary>
        /// <param name="index"></param>
        private void MyCarte_Add(int index)
        {

            Debug.WriteLine("Индекс " + index);
            if (index < 4)
            {
                Panel temp = Carte.GetCarte(index).ImageCartFullMin();
                //обводят карту при наведении на нее указателя мыши
                temp.MouseEnter += new EventHandler(Carte_MouseEnter);
                temp.MouseLeave += new EventHandler(Carte_MouseLeave);
                //перетаскивани карты на игровое поле
                temp.MouseDown += new MouseEventHandler(MyCarte_MouseDown);
                temp.MouseMove += new MouseEventHandler(Carte_MouseMove);
                temp.MouseUp += new MouseEventHandler(MyCarte_MouseUp);

                UserCards.Add(temp);


                this.Invoke((MethodInvoker)delegate { MyCarte.Controls.Add(temp); });
                UpdateLocation_Cards(MyCarte, MyCarte.Controls[0].Width, 3);


            }


        }

        private void NewPaintEnemyCard(int index)
        {

            AttackMtx.WaitOne();
            if (EnemyCardsOfMargin.Count > DataSession.EnCarteOnField.Count)
            {
                EnemyCardsOfMargin.RemoveAt(index);
            }
            else
            {
                //обновляем карту
                EnemyCardsOfMargin[index] = DataSession.EnCarteOnField[index].ImageCartMin();
                EnemyCardsOfMargin[index].MouseEnter += Carte_MouseEnter;
                EnemyCardsOfMargin[index].MouseLeave += Carte_MouseLeave;
            }

            EnemyMargin.Controls.Clear();
            for (int i = 0; i < EnemyCardsOfMargin.Count; i++)
            {
                EnemyMargin.Controls.Add(EnemyCardsOfMargin[i]);
            }
            //обновляем позиции карт 
            UpdateLocation_Cards(EnemyMargin, 90, 3);
            AttackMtx.ReleaseMutex();


        }

        private void NewPaintUserCard(int index)
        {


            if (UserCardsOfMargin.Count > DataSession.UsCarteOnField.Count)
            {
                this.Invoke((MethodInvoker)delegate { UserCardsOfMargin.RemoveAt(index); });
            }
            else
            {
                UserCardsOfMargin[index] = DataSession.UsCarteOnField[index].ImageCartMin();
                UserCardsOfMargin[index].MouseEnter += Carte_MouseEnter;
                UserCardsOfMargin[index].MouseLeave += Carte_MouseLeave;
                UserCardsOfMargin[index].MouseDown += MarginCarte_MouseDown;
                UserCardsOfMargin[index].MouseMove += Carte_MouseMove;
                UserCardsOfMargin[index].MouseUp += MarginCarte_MouseUp;

            }

            //обновляем карты
            UserMargin.Controls.Clear();
            for (int i = 0; i < UserCardsOfMargin.Count; i++)
            {
                UserMargin.Controls.Add(UserCardsOfMargin[i]);
            }
            UpdateLocation_Cards(UserMargin, 90, 3);


        }
        private void NewPaintHQUser()
        {

            UserHQPanel.Controls.Clear();
            UserHQPanel.Controls.Add(DataSession.UserHQ.ImageCartMin());
            UserHQPanel.Controls[0].MouseEnter += Carte_MouseEnter;
            UserHQPanel.Controls[0].MouseLeave += Carte_MouseLeave;
            UserHQPanel.Controls[0].MouseDown += HQCards_MouseDown;
            UserHQPanel.Controls[0].MouseMove += Carte_MouseMove;
            UserHQPanel.Controls[0].MouseUp += HQCars_MouseUp;


        }
        private void NewPaintHQEnemy()
        {

            EnemyHQPanel.Controls.Clear();
            EnemyHQPanel.Controls.Add(DataSession.EnemyHQ.ImageCartMin());
            EnemyHQPanel.Controls[0].MouseEnter += Carte_MouseEnter;
            EnemyHQPanel.Controls[0].MouseLeave += Carte_MouseLeave;


        }

        private void MyAttackVisual(int attacking, int attacked)
        {
            try
            {
                if (attacking == -1)
                {
                    //добавляем изображение штаба после атаки
                    this.Invoke((MethodInvoker)delegate { NewPaintHQUser(); });
                  
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate { NewPaintUserCard(attacking); });

                }

                if (attacked == -1)
                {
                    // добавляем изображение вражеского штаба после атаки
                    this.Invoke((MethodInvoker)delegate { NewPaintHQEnemy(); });

                }
                else
                {
                    this.Invoke((MethodInvoker)delegate { NewPaintEnemyCard(attacked); });
                }
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); }
        }
        private void EnAttackVisual(int attacking, int attacked)
        {
            try
            {
                 AnimationEnemyAttack(attacking, attacked);
                if (attacking == -1)
                {
                    //добавляем изображение штаба после атаки
                    this.Invoke((MethodInvoker)delegate { NewPaintHQEnemy(); });
                    
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate { NewPaintEnemyCard(attacking); });
                    
                }
                if (attacked == -1)
                {
                    //добавляем изображение  вражеского штаба после атаки
                    this.Invoke((MethodInvoker)delegate {
                        NewPaintHQUser();
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate {

                        NewPaintUserCard(attacked);
                    });

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void MyEnergy_Update()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate { MyEnergy.Controls.Clear(); });
                int X = 0;

                for (int i = 0; i < DataSession.MyEnergy; i++)
                {
                    PictureBox temp = new PictureBox();
                    temp.Image = Properties.Resources.StockEnergy;
                    temp.SizeMode = PictureBoxSizeMode.Zoom;
                    temp.Location = new Point(X, 0);
                    temp.Size = new Size(30, 25);

                    X += temp.Width + 2;
                    Debug.WriteLine("X равен " + X);

                    this.Invoke((MethodInvoker)delegate { MyEnergy.Controls.Add(temp); });


                }

                for (int i = 0; i < DataSession.MyMaxEnergy - DataSession.MyEnergy; i++)
                {
                    PictureBox temp = new PictureBox();
                    temp.Image = Properties.Resources.SpentEnergy;
                    temp.SizeMode = PictureBoxSizeMode.Zoom;
                    temp.Location = new Point(X, 0);
                    temp.Size = new Size(30, 25);
                    X += temp.Width + 2;

                    this.Invoke((MethodInvoker)delegate { MyEnergy.Controls.Add(temp); });

                }

            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); }
        }
        private void EnemyEnergy_Update()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate { EnemyEnergy.Controls.Clear(); });
                int X = 0;
                for (int i = 0; i < DataSession.EnEnergy; i++)
                {
                    PictureBox temp = new PictureBox();
                    temp.Image = Properties.Resources.StockEnergy;
                    temp.SizeMode = PictureBoxSizeMode.Zoom;
                    temp.Location = new Point(X, 0);
                    temp.Size = new Size(30, 25);
                    X += temp.Width + 2;
                    this.Invoke((MethodInvoker)delegate { EnemyEnergy.Controls.Add(temp); });
                }

                for (int i = 0; i < DataSession.EnMaxEnergy - DataSession.EnEnergy; i++)
                {
                    PictureBox temp = new PictureBox();
                    temp.Image = Properties.Resources.SpentEnergy;
                    temp.SizeMode = PictureBoxSizeMode.Zoom;
                    temp.Size = new Size(30, 25);
                    temp.Location = new Point(X, 0);
                    X += temp.Width + 2;
                    this.Invoke((MethodInvoker)delegate { EnemyEnergy.Controls.Add(temp); });
                }

            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); }
        }
        private void MyProgress_Func()
        {
            this.Invoke((MethodInvoker)delegate
            {
                StepEnd.Enabled = true;
                StepEnd.Text = "Ваш ход";
                EnemyTime.Visible = false;
                MyTime.Visible = true;
                AllowProgress = true;
                NotificLabel.Visible = true;
                NotificTimer.Start();

            });
        }
        private void EnemyProgress_Func()
        {
            this.Invoke((MethodInvoker)delegate
            {
                StepEnd.Enabled = false;
                StepEnd.Text = "Ход противника";
                EnemyTime.Visible = true;
                MyTime.Visible = false;
                AllowProgress = false;

            });
        }
        private void TimeProgress_Update(string data)
        {
            if (AllowProgress) this.Invoke((MethodInvoker)delegate { MyTime.Text = data; });
            else this.Invoke((MethodInvoker)delegate { EnemyTime.Text = data; });
        }
        private void EnemyCarte_Add(int count)
        {
            try
            {


                if (EnemyCarte.Controls.Count < count)
                {
                    this.Invoke((MethodInvoker)delegate { EnemyCarte.Controls.Add(ImageCarteInverted()); });

                }
                else this.Invoke((MethodInvoker)delegate { EnemyCarte.Controls.RemoveAt(0); });
                //перемещаем
                UpdateLocation_Cards(EnemyCarte, 70, 3);

            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); }
        }

        private void NotificTimerFunc(object sender, EventArgs e)
        {
            NotificLabel.Visible = false;
            NotificTimer.Stop();
        }

        private Panel ImageCarteInverted()
        {
            Panel NewPanel = new Panel();
            NewPanel.Size = new Size(70, 68);
            NewPanel.BackgroundImage = Properties.Resources.InvertedCarte;
            NewPanel.BackgroundImageLayout = ImageLayout.Zoom;
            return NewPanel;
        }
        private void GameMargin_Load(object sender, EventArgs e)
        {
            ClientContr.PaintEnEnergy += EnemyEnergy_Update;
            ClientContr.PaintMyEnergy += MyEnergy_Update;
            ClientContr.MyProgress += MyProgress_Func;
            ClientContr.EnemyProgress += EnemyProgress_Func;
            ClientContr.UpdateTime += TimeProgress_Update;
            ClientContr.AddCardsOnField += AddCardsOnMyCarte;
            ClientContr.EnAddCardOnField += EnemyAddCardsOnMyCarte;
            ClientContr.MyAttack += MyAttackVisual;
            ClientContr.EnAttack += EnAttackVisual;
            СomWithServer = ClientContr.DialogWithServ;//получаем класс для общения с сервером
            AllowProgress = false;

            UserHQPanel.Controls.Add(DataSession.UserHQ.ImageCartMin());
            UserHQPanel.Controls[0].MouseEnter += Carte_MouseEnter;
            UserHQPanel.Controls[0].MouseLeave += Carte_MouseLeave;
            UserHQPanel.Controls[0].MouseDown += HQCards_MouseDown;
            UserHQPanel.Controls[0].MouseMove += Carte_MouseMove;
            UserHQPanel.Controls[0].MouseUp += HQCars_MouseUp;
            UserHQPanel.Controls[0].Location = new Point(0, 0);

            EnemyHQPanel.Controls.Add(DataSession.EnemyHQ.ImageCartMin());
            EnemyHQPanel.Controls[0].Location = new Point(0, 0);
            EnemyHQPanel.Controls[0].MouseEnter += Carte_MouseEnter;
            EnemyHQPanel.Controls[0].MouseLeave += Carte_MouseLeave;
            UserCards = new List<Panel>();
            UserCardsOfMargin = new List<Panel>();
            EnemyCardsOfMargin = new List<Panel>();
            //сообщает о том что началя мой ход
            NotificLabel = new Label();
            NotificLabel.Location = new Point(405, 290);
            NotificLabel.Visible = false;
            NotificLabel.Text = "Твой ход";
            NotificLabel.Font = new Font("Arial", 32);
            NotificLabel.Size = new Size(210, 45);
            NotificLabel.BorderStyle = BorderStyle.FixedSingle;
            NotificLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(NotificLabel);
            NotificLabel.BringToFront();

            //и таймер для работы с ним
            NotificTimer = new System.Windows.Forms.Timer();
            NotificTimer.Interval = 1200;
            NotificTimer.Tick += NotificTimerFunc;


        }

        private void AnimationEnemyAttack(int attacking, int attacked)
        {

          

            //стартовое положение карты
            Point StartPoint = new Point();
            Panel ImageCarte = null;
         
            if (attacking == -1)
            {
                
                    ImageCarte = (Panel)EnemyHQPanel.Controls[0];
                    StartPoint = new Point(EnemyHQPanel.Location.X, EnemyHQPanel.Location.Y);
                this.Invoke((MethodInvoker)delegate { EnemyHQPanel.Controls.RemoveAt(0); });
               
            }
            else {
                
                    ImageCarte = (Panel)EnemyMargin.Controls[attacking];
                    StartPoint = new Point(EnemyMargin.Location.X + ImageCarte.Location.X, EnemyMargin.Location.Y + ImageCarte.Location.Y);
                this.Invoke((MethodInvoker)delegate { EnemyMargin.Controls.RemoveAt(attacking); });

            }

            //добавляем на форму
            this.Invoke((MethodInvoker)delegate { this.Controls.Add(ImageCarte); 
            ImageCarte.Location = StartPoint;
                ImageCarte.BringToFront();
            });


            //конечное положение карты
            Point EndPoint;
            if (attacked == -1) EndPoint = new Point(UserHQPanel.Location.X, UserHQPanel.Location.Y);
            else EndPoint = new Point(UserMargin.Location.X + UserMargin.Controls[attacked].Location.X, UserMargin.Location.Y + UserMargin.Controls[attacked].Location.Y);

            //общее смещение карты

            int DeltaX = EndPoint.X - StartPoint.X-120;
            if (DeltaX == 0) DeltaX = 1;
            int DeltaY = EndPoint.Y - StartPoint.Y;
            if (DeltaY == 0) DeltaY = 1;
            //смещение карты за один цикл
            int dx = DeltaX / Math.Abs(DeltaX);
            int dy = DeltaY / Math.Abs(DeltaY);
            int TimeAnimaton;//время анимации
            int SmallTime;
            double time = 0;
            int BigTime =0;
            bool Logic;
            if (Math.Abs(DeltaX) > Math.Abs(DeltaY))
            {
                TimeAnimaton = DeltaX;
                SmallTime = Math.Abs(DeltaX / DeltaY);
                time = Math.Abs((double)((DeltaX % DeltaY)/DeltaX));
                Logic = true;
            }
            else
            {
                TimeAnimaton = DeltaY;
                SmallTime = Math.Abs(DeltaY / DeltaX);
                time = Math.Abs((double)((DeltaY % DeltaX) / DeltaY));
                Logic = false;
            }

            if (SmallTime == 0) SmallTime = int.MaxValue;
            //ищем BigTime
           
            if (time != 0)
            {
                int J = 0;
                for (; J < 500; J++)
                {
                    if (J * time >= 1)
                    {
                        BigTime = J;
                        break;
                    }
                }
                if (J == 500) BigTime = int.MaxValue;
            }
          else BigTime = int.MaxValue;
            
           
            for (int i = 1; i < TimeAnimaton; i+=2)
            {
                if (Logic)
                {
                    StartPoint.X += dx;
                    if (i % SmallTime == 0) StartPoint.Y += dy;
                    if (i % BigTime == 0) StartPoint.Y += dy;
                    
   
                }
                else
                {
                    StartPoint.Y += dy;
                    if (i % SmallTime == 0) StartPoint.X += dx;
                    if (i % BigTime == 0) StartPoint.X += dx;
                    Debug.WriteLine("Координаты карты " + StartPoint.X + " " + StartPoint.Y);
                }
                this.Invoke((MethodInvoker)delegate { ImageCarte.Location = StartPoint; });
                // ImageCarte.BringToFront();

                Thread.Sleep(1);

            }
            Thread.Sleep(100);
            //возваращаем карту обратно
            for (int i = 0; i < TimeAnimaton; i++)
            {
                if (Logic)
                {
                    StartPoint.X -= dx;
                    if (i % SmallTime == 0) StartPoint.Y -= dy;
                    if (i % BigTime == 0) StartPoint.Y -= dy;


                }
                else
                {
                    StartPoint.Y -= dy;
                    if (i % SmallTime == 0) StartPoint.X -= dx;
                    if (i % BigTime == 0) StartPoint.X -= dx;
                    Debug.WriteLine("Координаты карты " + StartPoint.X + " " + StartPoint.Y);
                }
                this.Invoke((MethodInvoker)delegate { ImageCarte.Location = StartPoint; });
                Thread.Sleep(1);
            }
            //возвращаем карты обартно
            this.Invoke((MethodInvoker)delegate { this.Controls.Remove(ImageCarte); });
            if (attacking == -1)
            {


                this.Invoke((MethodInvoker)delegate { EnemyHQPanel.Controls.Add(ImageCarte); 
                    ImageCarte.Location = new Point(0, 0);
                });
            }
            else
            {

                this.Invoke((MethodInvoker)delegate
                {
                    EnemyMargin.Controls.Clear();
                    int count = EnemyCardsOfMargin.Count;
                    for (int i = 0; i < count; i++)
                        EnemyMargin.Controls.Add(EnemyCardsOfMargin[i]);
                    UpdateLocation_Cards(EnemyMargin, 90, 3);
                });
               
            }
        }
        private void HQCards_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                
                    Panel temp = (Panel)sender;
                    //удаляем его со своей панели
                    UserHQPanel.Controls.RemoveAt(0);

                    this.Controls.Add(temp);
                    temp.BringToFront();
                    MousePoint = new Point(e.X, e.Y);
                    MouseState = true;
               
       
            }
        }
        private void HQCars_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    Panel temp = (Panel)sender;

                    if (AllowProgress)//если разрешена отпрака сообщений 
                    {
                        //если мышка находится на поле вражеского штаба
                        if (temp.Location.X + temp.Width / 2 >= EnemyHQPanel.Location.X && temp.Location.X + temp.Width / 2 <= EnemyHQPanel.Location.X + EnemyHQPanel.Width &&
                            temp.Location.Y + temp.Height / 4 >= EnemyHQPanel.Location.Y && temp.Location.Y + temp.Height / 4 <= EnemyHQPanel.Location.Y + EnemyHQPanel.Height)
                        {

                            //атака на штаб
                            СomWithServer.Send(new int[] { -1, -1 }, MsgType.Attack);
                        }
                        //если мышка находится на поле карт
                        if ((temp.Location.X + temp.Width / 2 >= EnemyMargin.Location.X && temp.Location.X + temp.Width / 2 <= EnemyMargin.Location.X + EnemyMargin.Width &&
                            temp.Location.Y + temp.Height / 4 >= EnemyMargin.Location.Y && temp.Location.Y + temp.Height / 4 <= EnemyMargin.Location.Y + EnemyMargin.Height))
                        {
                            int count = EnemyMargin.Controls.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (temp.Location.X + temp.Width / 2 >= EnemyMargin.Location.X + EnemyMargin.Controls[i].Location.X &&
                                    temp.Location.X + temp.Width / 2 <= EnemyMargin.Location.X + EnemyMargin.Controls[i].Location.X + EnemyMargin.Controls[i].Width &&
                                    temp.Location.Y + temp.Height / 4 >= EnemyMargin.Location.Y + EnemyMargin.Controls[i].Location.Y &&
                                   temp.Location.Y + temp.Height / 4 <= EnemyMargin.Location.Y + EnemyMargin.Controls[i].Location.Y + EnemyMargin.Controls[i].Height)
                                {
                                    //атака на карточку
                                    СomWithServer.Send(new int[] { -1, i }, MsgType.Attack);
                                    break;
                                }
                            }
                        }
                    }

                    //удаляем перетаскиваемую карточку
                    this.Controls.Remove(temp);
                    MouseState = false;
                    temp.Location = new Point(0, 0);
                    UserHQPanel.Controls.Add(temp);
                  
                }
            }
            catch (Exception E)
            { MessageBox.Show(E.ToString()); }
        }

        private void GameMargin_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClientContr.Close();
            Thread.Sleep(100);//ожидаем завершения потока
            Application.Exit();
           
        }

        private void StepEnd_Click(object sender, EventArgs e)
        {
            СomWithServer.Send(MsgType.EndProgress);
            (sender as Button).Enabled = false;
        }
    }
}
