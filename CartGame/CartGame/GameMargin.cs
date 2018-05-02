﻿using System;
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
        private bool CloseForm = true;
        private SendAndRecMsg СomWithServer;
        private Controler ClientContr;
        private DataGame DataSession;


        private bool MouseState;//если true мышь перетаскивает карту
        private List<Panel> UserCards;//карты в руке игрока
        private List<Panel> UserCardsOfMargin;//изображения карт на поле
        private List<Panel> EnemyCardsOfMargin;//изображения вражеских карт на поле
     

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
        /// <summary>
        /// Обрабатывает события, когда противниик перетаскивает карты на игровое поле
        /// </summary>
        private void EnemyAddCardsOnMyCarte()
        {
            try
            {
                //удаляем карту из карт в руке у игрока
                this.Invoke((MethodInvoker)delegate
                {
                    if (EnemyCarte.Controls.Count > 0)
                    {
                        EnemyCarte.Controls.RemoveAt(0);
                        UpdateLocation_Cards(EnemyCarte, 70, 3);
                    }

                    //добавляем карту на панель

                    int i = DataSession.EnCarteOnField.Count - 1;
                    Panel temp = DataSession.EnCarteOnField[i].ImageCartMin();
                    temp.MouseEnter += Carte_MouseEnter;
                    temp.MouseLeave += Carte_MouseLeave;

                    EnemyCardsOfMargin.Add(temp);
                    EnemyMargin.Controls.Add(temp);
                    EffectAddCart(DataSession.EnCarteOnField, i, EnemyCardsOfMargin, EnemyMargin, false);
                });
            }
            catch(Exception E) { E.ToString(); }
         
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

        /// <summary>
        /// Начинает перетаскивание карты, когда игрок зажимает кнопку мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    //если карта является наследником классса Event
                    if (Carte.GetCarte(DataSession.CarteFromUser[NumberCarte]) is Event)
                    {
                        //привязываем к ней новый обработичик
                        temp.MouseUp -= MyCarte_MouseUp;
                        temp.MouseUp += EventCarte_MouseUp;
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
        /// <summary>
        /// Проверяем, какая из карт противника была атакована
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private int Which_Сard_Is_Attacked(Panel temp, int index)
        {
            //если мышка находится на поле вражеского штаба
            if (temp.Location.X + temp.Width / 2 >= EnemyHQPanel.Location.X && temp.Location.X + temp.Width / 2 <= EnemyHQPanel.Location.X + EnemyHQPanel.Width &&
                temp.Location.Y + temp.Height / 4 >= EnemyHQPanel.Location.Y && temp.Location.Y + temp.Height / 4 <= EnemyHQPanel.Location.Y + EnemyHQPanel.Height &&
                !(DataSession.UsCarteOnField[index] is DefenceConstr))
            {

                //атака на штаб
                return -1;

            }
            //если мышка находится на поле карт
            else if ((temp.Location.X + temp.Width / 2 >= EnemyMargin.Location.X && temp.Location.X + temp.Width / 2 <= EnemyMargin.Location.X + EnemyMargin.Width &&
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
                        return i;

                    }
                }
            }
            return int.MinValue;
        }
        private void EventCarte_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    Panel temp = (Panel)sender;
                    int attacked = int.MinValue; ;
                    //определяем тип события
                    switch ((Carte.GetCarte(DataSession.CarteFromUser[NumberCarte]) as Event).TypeEvent)
                    {
                        case TypeEventCard.DamageCard:
                            //узнаем какую карту атакует игрок
                            attacked = Which_Сard_Is_Attacked(temp, NumberCarte);
                            ReturnDropCard(temp, MyCarte, UserCards);
                            if (attacked != int.MinValue && AllowProgress)
                                СomWithServer.Send(new int[] { NumberCarte, attacked }, MsgType.DamageEvent);

                            //если событие было использовано
                            break;
                        case TypeEventCard.HealingCard:
                            //узнаем какую карту игрок собирается ремонитровать
                            attacked = Which_Сard_Is_Repairs(temp);
                            ReturnDropCard(temp, MyCarte, UserCards);
                            if (attacked != int.MinValue && AllowProgress)
                                СomWithServer.Send(new int[] { NumberCarte, attacked }, MsgType.RepairsEvent);
                            break;
                        case TypeEventCard.AllDamageCard:
                            //если карта была перетащена на поле врага
                            if ((temp.Location.X + temp.Width / 2 >= EnemyMargin.Location.X && temp.Location.X + temp.Width / 2 <= EnemyMargin.Location.X + EnemyMargin.Width &&
                              temp.Location.Y + temp.Height / 4 >= EnemyMargin.Location.Y && temp.Location.Y + temp.Height / 4 <= EnemyMargin.Location.Y + EnemyMargin.Height))
                              {
                                ReturnDropCard(temp, MyCarte, UserCards);
                                if (AllowProgress)
                                    СomWithServer.Send(new int[] { NumberCarte }, MsgType.AllDamageEvent);
                              }
                             else ReturnDropCard(temp, MyCarte, UserCards);
                            break;
                        default:
                            ReturnDropCard(temp, MyCarte, UserCards);
                            break;
                    }

                    NumberCarte = 0;




                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString() + "   " + MyCarte.Controls.Count);
            }
        }

        public int Which_Сard_Is_Repairs(Panel MoveCard)
        {
            //если мышка находится на поле вражеского штаба
            if (MoveCard.Location.X + MoveCard.Width / 2 >= UserHQPanel.Location.X && MoveCard.Location.X + MoveCard.Width / 2 <= UserHQPanel.Location.X + UserHQPanel.Width &&
                MoveCard.Location.Y + MoveCard.Height / 4 >= UserHQPanel.Location.Y && MoveCard.Location.Y + MoveCard.Height / 4 <= UserHQPanel.Location.Y + UserHQPanel.Height)
            {

                //атака на штаб
                return -1;

            }
            //если мышка находится на поле карт
            else if ((MoveCard.Location.X + MoveCard.Width / 2 >= UserMargin.Location.X && MoveCard.Location.X + MoveCard.Width / 2 <= UserMargin.Location.X + UserMargin.Width &&
                MoveCard.Location.Y + MoveCard.Height / 4 >= UserMargin.Location.Y && MoveCard.Location.Y + MoveCard.Height / 4 <= UserMargin.Location.Y + UserMargin.Height))
            {
                int Count = EnemyMargin.Controls.Count;
                for (int i = 0; i < Count; i++)
                {
                    if (MoveCard.Location.X + MoveCard.Width / 2 >= UserMargin.Location.X + UserMargin.Controls[i].Location.X &&
                        MoveCard.Location.X + MoveCard.Width / 2 <= UserMargin.Location.X + UserMargin.Controls[i].Location.X + UserMargin.Controls[i].Width &&
                        MoveCard.Location.Y + MoveCard.Height / 4 >= UserMargin.Location.Y + UserMargin.Controls[i].Location.Y &&
                        MoveCard.Location.Y + MoveCard.Height / 4 <= UserMargin.Location.Y + UserMargin.Controls[i].Location.Y + UserMargin.Controls[i].Height)
                    {
                        //атака на карточку
                        return i;

                    }
                }
            }
            return int.MinValue;
        }
        /// <summary>
        /// Возвращает перетаскиваемую карту на место
        /// </summary>
        /// <param name="DropCart"></param>
        /// <param name="ParentPanel"></param>
        /// <param name="ListCardsBeforeDrop"></param>
        private void ReturnDropCard(Panel DropCart, Panel ParentPanel, List<Panel> ListCardsBeforeDrop)
        {
            //удаляем перетаскиваемую карточку
            this.Controls.Remove(DropCart);

            MouseState = false;
            //очищаем панель

            ParentPanel.Controls.Clear();
            int count = ListCardsBeforeDrop.Count;
            //заполняем массив заново
            for (int i = 0; i < count; i++)
            {
                ParentPanel.Controls.Add(ListCardsBeforeDrop[i]);
            }
            //устанваливаем позицию карт
            UpdateLocation_Cards(ParentPanel, ParentPanel.Controls[0].Width, 3);

            //ParentPanel.Controls[0].Width
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
        /// <summary>
        /// Обрабатывает событие отпускания карты игроком, при перетаскивании карты на игровое поле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyCarte_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //здесь должна быть проверка на тип карты
                Panel temp = (Panel)sender;
                bool CardFound = false;
                if (temp.Location.X + temp.Width / 2 <= UserMargin.Location.X + UserMargin.Width &&
                    temp.Location.X + temp.Width / 2 >= UserMargin.Location.X && temp.Location.Y + temp.Height / 4 >= UserMargin.Location.Y && temp.Location.Y + temp.Height / 4 <= UserMargin.Location.Y + UserMargin.Height)
                {

                    CardFound = true;
                }

                ReturnDropCard(temp, MyCarte, UserCards);

                if (AllowProgress && CardFound)
                {//отправляем сообщение о том что необходимо отправить добавить карты на поле, если достаточно ресурсов
                    СomWithServer.Send(NumberCarte, MsgType.AddCarteOnField);
                }
                NumberCarte = 0;
            }
        }


        private void AddCardOnField(List<Robot> CarteOnField, int index, List<Panel> CardsOnMargin, Panel Margin)
        {
            //добавляем еще одну карту на игровое поле
            Panel temp = CarteOnField[index].ImageCartMin();
            temp.MouseEnter += Carte_MouseEnter;
            temp.MouseLeave += Carte_MouseLeave;
            temp.MouseDown += MarginCarte_MouseDown;
            temp.MouseMove += Carte_MouseMove;
            temp.MouseUp += MarginCarte_MouseUp;

            CardsOnMargin.Add(temp);
            Margin.Controls.Add(temp);
        }
        /// <summary>
        /// Выполняет эффект появляющейся при добавлении определенной карты
        /// </summary>
        /// <param name=""></param>
        private void EffectAddCart(List<Robot> CarteOnField, int index, List<Panel> CardsOnMargin, Panel Margin, bool EnemyOrUser)
        {
            //проверяем на необходимость обновить карты
            switch (CarteOnField[index].ID)
            {
                case 3:
                    AllPaintCard(CardsOnMargin, Margin, CarteOnField);
                    break;
                case 9:
                    //добавляем еще одну карту на игровое поле
                    AddCardOnField(CarteOnField, index, CardsOnMargin, Margin);

                    //обновляем положение карт
                    UpdateLocation_Cards(Margin, 90, 3);
                    break;
                case 16:
                    //отображаем урон по картам
                    int count = Margin.Controls.Count-1;
                    for (int i = 0; i < count; i++)
                    {
                        if(EnemyOrUser) AnimationDamage(i, -2, -2, 0);
                        else AnimationDamage(-2, i, 0, -2);
                    }
                      
                    //обновляем все карты
                    AllPaintCard(CardsOnMargin, Margin, CarteOnField);
                    break;
                default:
                    UpdateLocation_Cards(Margin, 90, 3);
                    break;
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
                
                    MyCarte.Controls.RemoveAt(number);
                    UserCards.RemoveAt(number);
                    UpdateLocation_Cards(MyCarte, 100, 3);


                //добавляем карту на панель
                int i = DataSession.UsCarteOnField.Count - 1;
                AddCardOnField(DataSession.UsCarteOnField, i, UserCardsOfMargin, UserMargin);
                
                //обновляем карты, если эта карта ветеран, или просто обновляем положение карт
                EffectAddCart(DataSession.UsCarteOnField, i, UserCardsOfMargin, UserMargin, true);
            });
           

        }
        /// <summary>
        /// Класс обработки отпускания карты, находящейся на поле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarginCarte_MouseDown(object sender, MouseEventArgs e)
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
        private void MarginCarte_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int attacked = int.MinValue;
                if (e.Button == MouseButtons.Left)
                {
                    Panel temp = (Panel)sender;
                                    
                            //узнаем какую карту атакует игрок
                            attacked = Which_Сard_Is_Attacked(temp, NumberCarte);

                            ReturnDropCard(temp, UserMargin, UserCardsOfMargin);
                            if (AllowProgress && attacked != int.MinValue)//если разрешена отпрака сообщений 
                            {

                                СomWithServer.Send(new int[] { NumberCarte, attacked }, MsgType.Attack);
                            }
                            NumberCarte = 0;
                                                   
                                  
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }



        }
        /// <summary>
        /// Обновляет позиции карт на игровом поле и в руке у игрока
        /// </summary>
        /// <param name="Margin"></param>
        /// <param name="WidthCart"></param>
        /// <param name="DistCards"></param>
        private void UpdateLocation_Cards(Panel Margin, int WidthCart, int DistCards)
        {
           

                int count = Margin.Controls.Count;
                Debug.WriteLine($"Count при {Margin.Name} равен:" + count);
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
            try
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
            catch (Exception e) { MessageBox.Show(e.ToString()); }

        }

        private void NewPaintEnemyCard(int index)
        {

            try
            {
                if (EnemyCardsOfMargin.Count > DataSession.EnCarteOnField.Count)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        //если обновляемая карта ветеран, то обновляем все карты
                        if (EnemyCardsOfMargin[index].GetType() == (Carte.GetCarte(3) as Robot).ImageCartMin().GetType())
                        {
                            EnemyCardsOfMargin.RemoveAt(index);
                            AllPaintCard(EnemyCardsOfMargin, EnemyMargin, DataSession.EnCarteOnField);
                        }
                        else EnemyCardsOfMargin.RemoveAt(index);

                    });
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


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
        }

        private void NewPaintUserCard(int index)
        {
            try
            {
                if (UserCardsOfMargin.Count > DataSession.UsCarteOnField.Count)
                {
                    //если обновляемая карта ветеран, то обновляем все карты
                    this.Invoke((MethodInvoker)delegate
                    {

                       
                        if (UserCardsOfMargin[index].GetType() == (Carte.GetCarte(3) as Robot).ImageCartMin().GetType())
                        {
                            UserCardsOfMargin.RemoveAt(index);
                            AllPaintCard(UserCardsOfMargin, UserMargin, DataSession.UsCarteOnField);
                        }
                        else UserCardsOfMargin.RemoveAt(index);

                    });
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
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void AllPaintCard(List<Panel> CardsOfMargin, Panel Margin, List<Robot> CarteOnField)
        {
            try
            {
                int count = CarteOnField.Count;
                CardsOfMargin.Clear();
                for (int index = 0; index < count; index++)
                {
                    CardsOfMargin.Add(CarteOnField[index].ImageCartMin());
                    CardsOfMargin[index].MouseEnter += Carte_MouseEnter;
                    CardsOfMargin[index].MouseLeave += Carte_MouseLeave;
                    CardsOfMargin[index].MouseDown += MarginCarte_MouseDown;
                    CardsOfMargin[index].MouseMove += Carte_MouseMove;
                    CardsOfMargin[index].MouseUp += MarginCarte_MouseUp;
                }


                //обновляем карты
                Margin.Controls.Clear();
                for (int i = 0; i < count; i++)
                {
                    Margin.Controls.Add(CardsOfMargin[i]);
                }
                UpdateLocation_Cards(Margin, 90, 3);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

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

        private void MyAttackVisual(int attacking, int attacked, int damageUser, int damageEnemy)
        {
            try
            {
                //отображаем урон по карточкам
                AnimationDamage(attacking, attacked, damageUser, damageEnemy); 

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
        /// <summary>
        /// Отображает иконки с полученным уроном
        /// </summary>
        /// <param name="MoveEnemyPanel"></param>
        /// <param name="attacked"></param>
        /// <param name="damageUser"></param>
        /// <param name="damageEnemy"></param>
        private Panel AnimationDamage(Panel MoveEnemyPanel, int attacked, int damageUser, int damageEnemy)
        {
            Point damageLoc;
            Panel DamageEnemy = null, DamageUser = null;//изображение урона во врагу и по игроку
            //отображаем урон на карте пользователя
            if (damageUser != 0)
            {
                if (attacked == -1)
                {
                    damageLoc = new Point();
                    damageLoc.X = UserHQPanel.Location.X + 15;
                    damageLoc.Y = UserHQPanel.Location.Y + 15;
                }
                else
                {
                    damageLoc = new Point();
                    damageLoc.X = UserMargin.Location.X + UserMargin.Controls[attacked].Location.X + 15;
                    damageLoc.Y = UserMargin.Location.Y + UserMargin.Controls[attacked].Location.Y + 15;
                }
                DamageUser = CreateDamagePanel(damageUser);

                DamageUser.Location = damageLoc;
                this.Invoke((MethodInvoker)delegate { this.Controls.Add(DamageUser); DamageUser.BringToFront(); });
            }
            if (damageEnemy != 0)
            {
                //отображаем урон на карте врага
                damageLoc = new Point();
                damageLoc.X = MoveEnemyPanel.Location.X + 15;
                damageLoc.Y = MoveEnemyPanel.Location.Y + 15;

                //создаем элемент показывающией урон и устанавливаем текущую позиции карточки врага которая будет перемещатсься
                DamageEnemy = CreateDamagePanel(damageEnemy);

                DamageEnemy.Location = damageLoc;
                this.Invoke((MethodInvoker)delegate { this.Controls.Add(DamageEnemy); DamageEnemy.BringToFront(); });
            }
            

            //выделяем дополнительны поток для удаления через некорое время картинок с уроном
            ThreadPool.QueueUserWorkItem(Elapsed_TimerPaint, new Panel[] {DamageEnemy, DamageUser });
            return DamageEnemy;
        }
        /// <summary>
        /// Отобажает иконки с полученным уроном
        /// </summary>
        /// <param name="attacking"></param>
        /// <param name="attacked"></param>
        /// <param name="damageUser"></param>
        /// <param name="damageEnemy"></param>
        private Panel AnimationDamage(int attacking, int attacked, int damageUser, int damageEnemy)
        {
            Panel DamageEnemy = null, DamageUser = null;//изображение урона во врагу и по игроку
            Point damageLoc = new Point();
            if (damageUser!=0)
            {
                
            if (attacking == -1)
            {
                damageLoc = new Point();
                damageLoc.X = UserHQPanel.Location.X + 15;
                damageLoc.Y = UserHQPanel.Location.Y + 15;
            }
            else 
            {
                damageLoc = new Point();
                damageLoc.X = UserMargin.Location.X + UserMargin.Controls[attacking].Location.X + 15;
                damageLoc.Y = UserMargin.Location.Y + UserMargin.Controls[attacking].Location.Y + 15;
            }
                DamageUser = CreateDamagePanel(damageUser);
                
                    DamageUser.Location = damageLoc;
                    this.Invoke((MethodInvoker)delegate { this.Controls.Add(DamageUser); DamageUser.BringToFront(); });
            }

            if (damageEnemy != 0)
            {
                if (attacked == -1)
                {
                    damageLoc = new Point();
                    damageLoc.X = EnemyHQPanel.Location.X + 15;
                    damageLoc.Y = EnemyHQPanel.Location.Y + 15;
                }
                else
                {
                    damageLoc = new Point();
                    damageLoc.X = EnemyMargin.Location.X + EnemyMargin.Controls[attacked].Location.X + 15;
                    damageLoc.Y = EnemyMargin.Location.Y + EnemyMargin.Controls[attacked].Location.Y + 15;
                }
            
                DamageEnemy = CreateDamagePanel(damageEnemy);
                DamageEnemy.Location = damageLoc;
                this.Invoke((MethodInvoker)delegate { this.Controls.Add(DamageEnemy); DamageEnemy.BringToFront(); });
            }
            //выделяем дополнительны поток для удаления через некорое время картинок с уроном
            ThreadPool.QueueUserWorkItem(Elapsed_TimerPaint,new Panel[] { DamageEnemy, DamageUser });
           
            return DamageEnemy;
        }

        /// <summary>
        /// Выполняется при окончании отображении иконок урона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void Elapsed_TimerPaint(object statinfo)
        {
           
            Panel[] temp = (Panel[])statinfo;

            Thread.Sleep(1200);
            this.Invoke((MethodInvoker)delegate
            {
                //удаляем с основной формы DamageEnemy
                this.Controls.Remove(temp[0]);
               temp[0] = null;
                //удаляем с основной формы DamageUser
                this.Controls.Remove(temp[1]);
              temp[1] = null;
            });
           
        }

        /// <summary>
        /// Создает элемент урона по карте 
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        private Panel CreateDamagePanel(int damage)
        {
            Panel damagePanel = new Panel();
            damagePanel.Size = new Size(60, 65);
            damagePanel.BackgroundImage = Properties.Resources.damage;
            damagePanel.BackgroundImageLayout = ImageLayout.Zoom;

            Label ValueDamage = new Label();
            if (damage > 0)
            {
                ValueDamage.Text = "-" + damage;
                ValueDamage.Size = new Size(36, 30);
                ValueDamage.Font = new Font("Arial", 17);
            }
            else if (damage < 0)
            {
                ValueDamage.Text = "+" + Math.Abs(damage);
                ValueDamage.Size = new Size(40, 30);
                ValueDamage.Font = new Font("Arial", 16);
            }
           
            ValueDamage.Location = new Point(11,18);
           
            damagePanel.Controls.Add(ValueDamage);
            return damagePanel;
        }

        /// <summary>
        /// Визуально отображает атаку противника картой робота
        /// </summary>
        /// <param name="attacking"></param>
        /// <param name="attacked"></param>
        /// <param name="damageUser"></param>
        /// <param name="damageEnemy"></param>
        private void EnAttackVisual(int attacking, int attacked, int damageUser, int damageEnemy)
        {
            try
            {
                                
                    AnimationEnemyAttack(attacking, attacked, damageUser, damageEnemy);
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
                        this.Invoke((MethodInvoker)delegate
                        {
                            NewPaintHQUser();
                        });
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)delegate
                        {

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

        /// <summary>
        /// Обрабавтывает на уровне интерфейса новый ход игрока
        /// </summary>
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

        /// <summary>
        /// Обрабатывает на уровне интерфейса новый ход противника
        /// </summary>
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

        /// <summary>
        /// Обновляет время хода
        /// </summary>
        /// <param name="data"></param>
        private void TimeProgress_Update(string data)
        {
            if (AllowProgress) this.Invoke((MethodInvoker)delegate { MyTime.Text = data; });
            else this.Invoke((MethodInvoker)delegate { EnemyTime.Text = data; });
        }

        /// <summary>
        /// Добавляет карту в руки противнику
        /// </summary>
        /// <param name="count"></param>
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

        /// <summary>
        /// Обрабатывает ошибку соединения с сервером на уровне итерфейса
        /// </summary>
        private void ErrorConnectoinServer()
        {
            
            UserCards.Clear();
            UserCards = null;
            UserCardsOfMargin.Clear();
            UserCardsOfMargin = null;
            EnemyCardsOfMargin.Clear();
            EnemyCardsOfMargin = null;
            CloseForm = false;
            this.Invoke((MethodInvoker)delegate {
                ChoiceForm NewForm = new ChoiceForm(ClientContr);
                NewForm.Show();
                this.Close();
            });
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
        private void UsAllDamage(int number, int damage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                int count = EnemyMargin.Controls.Count;
                for (int i = 0; i < count; i++)
                    //отображаем урон по картам
                    AnimationDamage(-2, i, 0, damage);
            });

            //обновляем все карты
            this.Invoke((MethodInvoker)delegate { AllPaintCard(this.EnemyCardsOfMargin, EnemyMargin, DataSession.EnCarteOnField); });

            //удаляем карту у нас в руке
            if (MyCarte.Controls.Count > number && UserCards.Count > number)
                this.Invoke((MethodInvoker)delegate
            {
                MyCarte.Controls.RemoveAt(number);
                UserCards.RemoveAt(number);
            });
            UpdateLocation_Cards(MyCarte, 100, 3);

        }
        private void EnAllDamage(int number, int id, int damage)
        {
            try
            {
                //отображаем урон картой-событием и обновляем все карты по которым был нанесен урон
                 AnimationEnemyAllAttack(number, id, damage);

                //перерисовываем все карты
                this.Invoke((MethodInvoker)delegate { AllPaintCard(this.UserCardsOfMargin, UserMargin, DataSession.UsCarteOnField); });
                //удаляем карту в руке у противника
                if (EnemyCarte.Controls.Count > number)
                    this.Invoke((MethodInvoker)delegate { EnemyCarte.Controls.RemoveAt(number); });
                UpdateLocation_Cards(EnemyCarte, 70, 3);
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); }
        }

        private void AnimationEnemyAllAttack(int number, int id, int damage)
        {
            //изображение карты
            Panel ImageCarte = Carte.GetCarte(id).ImageCartFullMin();

            //ищем координаты используемой карты
            Point StartPoint = new Point(EnemyCarte.Location.X + EnemyCarte.Controls[number].Location.X, EnemyCarte.Location.Y + EnemyCarte.Controls[number].Location.Y);
            //скрываем карту
            this.Invoke((MethodInvoker)delegate
            {
                EnemyCarte.Controls[number].Visible = false;

                //добавляем изображение карты
                this.Controls.Add(ImageCarte);
 
                ImageCarte.Location = StartPoint;
                ImageCarte.BringToFront();
               
               
            });

           
            //конечное положение карты
            Point EndPoint = new Point( UserMargin.Location.X + UserMargin.Width/2, UserMargin.Location.Y + UserMargin.Height / 2-50);
            //создаем анимацию этой карты
         
            MoveCarteStartToEnd(ImageCarte, StartPoint, EndPoint, null);
            
            //отображаем урон по картам
            int count = UserMargin.Controls.Count;
            for (int i = 0; i < count; i++)
                AnimationDamage(i, -2, damage, 0);
          
            Thread.Sleep(500);

            //удаляем изображение карты
            this.Invoke((MethodInvoker)delegate
            {
                this.Controls.Remove(ImageCarte);
                ImageCarte = null;
            });
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
            ClientContr.EnAttackDamageCard += EnAttackDamageCarte;
            ClientContr.MyAttackDamageCard += UsAttackDamageCarte;
            ClientContr.MyRepairsCard += UsRepairsCarte;
            ClientContr.EnRepairsCard += EnRepairsCarte;
            ClientContr.EndGame += EndGame;
            ClientContr.UsAllDamage += UsAllDamage;
            ClientContr.EnAllDamage += EnAllDamage;
            ClientContr.ErrorConnectToServer += ErrorConnectoinServer;

            СomWithServer = ClientContr.DialogWithServ;//получаем класс для общения с сервером
            AllowProgress = false;

            UserHQPanel.Controls.Add(DataSession.UserHQ.ImageCartMin());
            UserHQPanel.Controls[0].MouseEnter += Carte_MouseEnter;
            UserHQPanel.Controls[0].MouseLeave += Carte_MouseLeave;
            UserHQPanel.Controls[0].MouseDown += HQCards_MouseDown;
            UserHQPanel.Controls[0].MouseMove += Carte_MouseMove;
            UserHQPanel.Controls[0].MouseUp += HQCars_MouseUp;
            UserHQPanel.Controls[0].Location = new Point(0, 0);

            MyName.Text += DataSession.UsName;
            EnemyName.Text += DataSession.EnName; 

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

        /// <summary>
        /// Отображает на уровне интерфейса урон картой-событием по карте противника
        /// </summary>
        /// <param name="attacking"></param>
        /// <param name="attacked"></param>
        /// <param name="damage"></param>
        private void UsAttackDamageCarte(int attacking, int attacked, int damage)
        {  
            //отображаем урон по карте врага
            AnimationDamage(-2, attacked, 0, damage);

            //обновляем аттакуемую карту
            if (attacked == -1) this.Invoke((MethodInvoker)delegate { NewPaintHQEnemy(); });
            else this.Invoke((MethodInvoker)delegate {
                NewPaintEnemyCard(attacked);
            });

            //удаляем карту у нас в руке
            if (MyCarte.Controls.Count > attacking && UserCards.Count > attacking)
                this.Invoke((MethodInvoker)delegate
                {
                    MyCarte.Controls.RemoveAt(attacking);
                    UserCards.RemoveAt(attacking);
                });
            UpdateLocation_Cards(MyCarte, 100, 3);
            
        }
        /// <summary>
        /// Отображает клиенту его ремонт
        /// </summary>
        /// <param name="NumberCardRepairs"></param>
        /// <param name="Repairable"></param>
        /// <param name="damage"></param>
        private void UsRepairsCarte(int NumberCardRepairs, int Repairable, int damage)
        {
            //отображаем ремонт моей карты 
            AnimationDamage(Repairable, -2 , damage, 0);

            //обновляем аттакуемую карту
            if (Repairable == -1) this.Invoke((MethodInvoker)delegate { NewPaintHQUser(); });
            else this.Invoke((MethodInvoker)delegate {
                NewPaintUserCard(Repairable);
            });

            //удаляем карту у нас в руке
            if (MyCarte.Controls.Count > NumberCardRepairs && UserCards.Count > NumberCardRepairs)
                this.Invoke((MethodInvoker)delegate
                {
                    MyCarte.Controls.RemoveAt(NumberCardRepairs);
                    UserCards.RemoveAt(NumberCardRepairs);
                });
            UpdateLocation_Cards(MyCarte, 100, 3);
        }



        /// <summary>
        /// Отображае клиенту ремонт противника
        /// </summary>
        /// <param name="attacking"></param>
        /// <param name="IDAttacking"></param>
        /// <param name="attacked"></param>
        /// <param name="damage"></param>
        private void EnRepairsCarte(int NumberCardRepairs, int IDRepairs, int Repairable, int damage)
        {
            //Отображаем ремонт врага в клиенте у пользователя
            AnimationAttackDamageEvent(NumberCardRepairs, IDRepairs, Repairable, damage, false);
            //обновляем аттакуемую карту
            if (Repairable == -1) this.Invoke((MethodInvoker)delegate { NewPaintHQEnemy(); });
            else this.Invoke((MethodInvoker)delegate { NewPaintEnemyCard(Repairable); });

            //удаляем карту в руке у противника
            if (EnemyCarte.Controls.Count > NumberCardRepairs)
                this.Invoke((MethodInvoker)delegate { EnemyCarte.Controls.RemoveAt(NumberCardRepairs); });
            UpdateLocation_Cards(EnemyCarte, 70, 3);
        }

        /// <summary>
        /// Отображает на визуальном уровне атаку картой-событием наносящим урон
        /// </summary>
        /// <param name="attacking"></param>
        /// <param name="attacked"></param>
        /// <param name="damage"></param>
        private void EnAttackDamageCarte(int attacking,int IDAttacking, int attacked, int damage)
        {
            //Отображаем урон картой-событием
            AnimationAttackDamageEvent(attacking, IDAttacking, attacked, damage, true);
            //обновляем аттакуемую карту
            if (attacked == -1) this.Invoke((MethodInvoker)delegate { NewPaintHQUser(); });
            else this.Invoke((MethodInvoker)delegate { NewPaintEnemyCard(attacked); });

            //удаляем карту в руке у противника
            if (EnemyCarte.Controls.Count > attacking)
            this.Invoke((MethodInvoker)delegate { EnemyCarte.Controls.RemoveAt(attacking); });
            UpdateLocation_Cards(EnemyCarte, 70, 3);
        }

        private void AnimationAttackDamageEvent(int attacking,int IDAttacking,  int attacked, int damage, bool EnemyOrUser)
        {
            try
            {

                //изображение карты
                Panel ImageCarte = Carte.GetCarte(IDAttacking).ImageCartFullMin();
                //ищем координаты используемой карты
                Point StartPoint = new Point(EnemyCarte.Location.X + EnemyCarte.Controls[attacking].Location.X, EnemyCarte.Location.Y + EnemyCarte.Controls[attacking].Location.Y);
                //скрываем карту
                this.Invoke((MethodInvoker)delegate
                {
                    EnemyCarte.Controls[attacking].Visible = false;

                    //добавляем изображение карты
                    this.Controls.Add(ImageCarte);
                    ImageCarte.Location = StartPoint;
                    ImageCarte.BringToFront();
                });
                //конечное положение карты
                Point EndPoint;
                if (EnemyOrUser)
                {
                    if (attacked == -1) EndPoint = new Point(UserHQPanel.Location.X, UserHQPanel.Location.Y);
                    else EndPoint = new Point(UserMargin.Location.X + UserMargin.Controls[attacked].Location.X, UserMargin.Location.Y + UserMargin.Controls[attacked].Location.Y);
                }
                else
                {
                    if (attacked == -1) EndPoint = new Point(EnemyHQPanel.Location.X, EnemyHQPanel.Location.Y);
                    else EndPoint = new Point(EnemyMargin.Location.X + EnemyMargin.Controls[attacked].Location.X,EnemyMargin.Location.Y + EnemyMargin.Controls[attacked].Location.Y);
                }

                MoveCarteStartToEnd(ImageCarte, StartPoint, EndPoint, null);
                if (EnemyOrUser)
                    AnimationDamage(attacked, -2, damage, 0);
                else AnimationDamage(-2,attacked,0, damage);
                Thread.Sleep(500);

                //удаляем изображение карты
                this.Invoke((MethodInvoker)delegate
                {
                    this.Controls.Remove(ImageCarte);
                    ImageCarte = null;
                });
                

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        private void EndGame(MsgType e)
        {

            this.Invoke((MethodInvoker)delegate
            {
                if (e == MsgType.YouWin)
                    NotificLabel.Text = "Вы победили!!!";

                if (e == MsgType.YouOver)
                    NotificLabel.Text = "Вы проиграли!!!";
                if (e == MsgType.Draw)
                    NotificLabel.Text = "Ничья!!!";
                if (e == MsgType.EnemyNoActiv)
                {
                    NotificLabel.Text = "Противник неактивен!";
                }
                    NotificLabel.Size = new Size(376, 55);
                NotificLabel.Visible = true;
                NotificLabel.Location = new Point(334, 235);
                Button Close = new Button();
                Close.Text = "Выйти";
                Close.Location = new Point(422, 317);
                Close.Size = new Size(192, 42);
                Close.Font = new Font("Arial", 20);
                Close.Click += Click_ButtonClose;
                this.Controls.Add(Close);
                Close.BringToFront();

                if (e == MsgType.TechnicalVictory)
                {
                    NotificLabel.Text = "Противник вышел из игры!!!";
                    NotificLabel.Size = new Size(600, 55);
                    NotificLabel.Location = new Point(250, 235);
                    
                }
                       
                if(e == MsgType.YouNoActiv)
                {
                    NotificLabel.Text = "Вы долго были неактивны!";
                    NotificLabel.Size = new Size(600, 55);
                    NotificLabel.Location = new Point(250, 235);
                }
                if (e == MsgType.EnemyNoActiv)
                {
                    NotificLabel.Text = "Противник неактивен!";
                    NotificLabel.Size = new Size(600, 55);
                    NotificLabel.Location = new Point(250, 235);
                }

            });
            
        }

        private void Click_ButtonClose(object sender, EventArgs e)
        {
            UserCards.Clear();
            UserCards = null;
            UserCardsOfMargin.Clear();
            UserCardsOfMargin = null;
            EnemyCardsOfMargin.Clear();
            EnemyCardsOfMargin = null;
            CloseForm = false;
            this.Invoke((MethodInvoker)delegate {
                ChoiceForm NewForm = new ChoiceForm(ClientContr);
                NewForm.Show();
                this.Close();
            }); 
            
            

        }
        private void AnimationEnemyAttack(int attacking, int attacked, int damageUser, int damageEnemy)
        {

            try
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
                else
                {

                    ImageCarte = (Panel)EnemyMargin.Controls[attacking];
                    StartPoint = new Point(EnemyMargin.Location.X + ImageCarte.Location.X, EnemyMargin.Location.Y + ImageCarte.Location.Y);
                    this.Invoke((MethodInvoker)delegate { EnemyMargin.Controls.RemoveAt(attacking); });

                }

                //добавляем на форму
                this.Invoke((MethodInvoker)delegate
                {
                    this.Controls.Add(ImageCarte);
                    ImageCarte.Location = StartPoint;
                    ImageCarte.BringToFront();
                });
                //конечное положение карты
                Point EndPoint;
                if (attacked == -1) EndPoint = new Point(UserHQPanel.Location.X, UserHQPanel.Location.Y);
                else EndPoint = new Point(UserMargin.Location.X + UserMargin.Controls[attacked].Location.X, UserMargin.Location.Y + UserMargin.Controls[attacked].Location.Y);

                MoveCarteStartToEnd(ImageCarte, StartPoint, EndPoint, null);
                 Panel tempDamageEnemy = AnimationDamage(ImageCarte, attacked, damageUser, damageEnemy);

                //возваращаем карту обратно
                MoveCarteStartToEnd(ImageCarte, EndPoint, StartPoint, tempDamageEnemy);

                //возвращаем карты обартно
                this.Invoke((MethodInvoker)delegate { this.Controls.Remove(ImageCarte); });
                if (attacking == -1)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        EnemyHQPanel.Controls.Add(ImageCarte);
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
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }




        /// <summary>
        /// Перемещаем карту из одной точки в другую
        /// </summary>
        /// <param name="MovePanel"></param>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        private void MoveCarteStartToEnd(Panel MovePanel,Point StartPoint, Point EndPoint,  Panel DamageEnemy)
        {

            int DeltaX = EndPoint.X - StartPoint.X;//смещение по Х
            if (DeltaX == 0) DeltaX = 1;//оно не может быть равно 0
            int DeltaY = 0 ;
            if (EndPoint.Y > StartPoint.Y) DeltaY = EndPoint.Y - StartPoint.Y - MovePanel.Height +10;//смещение по оси У
            else DeltaY = EndPoint.Y - StartPoint.Y-10;

            if (DeltaY == 0) DeltaY = 1;
            //определяем в какую сторону будут смещаться карты
            int dx = DeltaX / Math.Abs(DeltaX);
            int dy = DeltaY / Math.Abs(DeltaY);
            int TimeAnimaton;//время анимации
            int SmallTime = 0;//наименьшее время через которое должно смещаться карта по маленькой оси
            double time = 0;//остаток он SmallTime
            int BigTime = 0;
            bool Logic;//если true перемещение по оси Х больше чем по У

            if (Math.Abs(DeltaX) > Math.Abs(DeltaY))//если смещение по оси Х больше
            {
                TimeAnimaton = Math.Abs(DeltaX);//время анимации равно длинне наибольшего расстояния смещения
                SmallTime = Math.Abs(DeltaX / DeltaY);//определяем через какое время карта должна смещаться по меньшей оси
                time = Math.Abs((double)((DeltaX % DeltaY) / DeltaX));//остаток от  SmallTime
                Logic = true;
            }
            else
            {
                TimeAnimaton = Math.Abs(DeltaY);
                SmallTime = Math.Abs(DeltaY / DeltaX);
                time = Math.Abs((double)((DeltaY % DeltaX) / DeltaY));
                Logic = false;
            }

            if (SmallTime == 0) SmallTime = int.MaxValue;//если длинны осей почти равны, то карта по меньшей из осей почти не смещается
            //ищем BigTime

            if (time != 0)//если длины все же различны
            {
                int J = 0;
                for (; J < 500; J++)
                {
                    if (J * time >= 1)//ищем кратное число
                    {
                        BigTime = J;//наибольшее время через которое карта будет смещаться меньшей из осей
                        break;
                    }
                }
                if (J == 500) BigTime = int.MaxValue;//если разница между осями минимальна, то опустим их разницу
            }
            else BigTime = int.MaxValue;//опустим разницу длинн перемещения по осям

            //начинаем перемещение
            for (int i = 1; i < TimeAnimaton; i++)
            {
                if (Logic)//если Х больше Y
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

                }
                this.Invoke((MethodInvoker)delegate { MovePanel.Location = StartPoint; });
                //если к карте привязан урон перемещаем его вместе с ней
                this.Invoke((MethodInvoker)delegate {
                    if (DamageEnemy != null)
                       DamageEnemy.Location = new Point(StartPoint.X + 15, StartPoint.Y + 15);
                  
                });
                
                Thread.Sleep(1);

            }
        }

        /// <summary>
        /// Обрабатывает нажатие игроком на карточке штаба
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Обрабатывает отпускание карточки штаба игроком
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HQCars_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    Panel temp = (Panel)sender;
                    int attacked = int.MinValue;
                    attacked = Which_Сard_Is_Attacked(temp, NumberCarte);
                   

                    //удаляем перетаскиваемую карточку
                    this.Controls.Remove(temp);
                    MouseState = false;
                    temp.Location = new Point(0, 0);
                    UserHQPanel.Controls.Add(temp);

                if (AllowProgress && attacked!= int.MinValue)//если разрешена отпрака сообщений 
                {
                        СomWithServer.Send(new int[] { -1, attacked }, MsgType.Attack);
                }
                    NumberCarte = 0;
              }
            }
            catch (Exception E)
            { MessageBox.Show(E.ToString()); }
        }

        private void GameMargin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (CloseForm)
                {
                    СomWithServer.Send(MsgType.ClientClosing);
                    Thread.Sleep(200);
                    ClientContr.Dispose();
                    this.Invoke((MethodInvoker)delegate
                    {
                        ChoiceForm NewForm = new ChoiceForm(ClientContr);
                        NewForm.Show();

                    });

                }
            }
            catch (Exception E) { MessageBox.Show(e.ToString()); }
          
        }

        private void StepEnd_Click(object sender, EventArgs e)
        {
            СomWithServer.Send(MsgType.EndProgress);
           // (sender as Button).Enabled = false;
           //убрал, так как не всегда сообшение доходило до сервера
        }
    }
}
