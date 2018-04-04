﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Timers;



namespace CarteServer
{
    public enum MsgType
    {
        StartSession,
        DeliteSeek, 
        CarteUser,
        GetName,
        StartGame,
        AddUserCarte, 
        AddEnemyCarte, 
        UserMaxEnergy,
        YourEnergy,
        EnemyMaxEnergy,
        EnemyEnergy,
        ProgressTime,
        MyProgress,
        EnemyProgress,
        AddCarteOnField,
        EnemyAddCarteOnField,
        Attack,
        MyAttackSucc,
        EnAttackSucc,
        EndProgress, 
        YouWin, 
        YouOver, 
        Draw,
        TechnicalVictory,
        ClientClosing



    }
    delegate void EndSession(Session sender);
    class Session
    {
        public event EndSession SessionEnd;
        private User Us1, Us2;
        private  int ColodsRec;//колод получено
        private int NameRec;//имен получено
        private Thread SessionThread;
        private User UsProgress;//ссылка на клиента, который ходит в данный момент
        private System.Timers.Timer TempProgress;
        private int GameLoad;//партий загружено
        private int SecProgress;
        
        /// <summary>
        /// Увеличивает счетчик на 1, когда колода получена
        /// </summary>
        /// <param name="sender"></param>
        private void RecColod()
        {
            ColodsRec++;
        }
        /// <summary>
        /// Увеличивает счетчик на 1, когда имя пользователя получено
        /// </summary>
        private void RecName()
        {
            NameRec++;
        }
        public Session(User User1, User User2)
         {
            Us1 = User1;
            Us2 = User2;
            ColodsRec = 0;
            GameLoad = 0;
            //добавляем обработчики событий получения колоды
            Us1.ColodRec += RecColod;
            Us2.ColodRec += RecColod;
            Us1.AddCardField += AddCardOnField;
            Us2.AddCardField += AddCardOnField;
           

            SessionThread = new Thread(StartSession);
            SessionThread.Start();
            


         }
        private void AddCardOnField(User sender, int number, int idCards)
        {
            if (sender == Us1)
            {
                Us1.Send(Us1.Energy, MsgType.YourEnergy);
                Us2.Send(Us1.Energy, MsgType.EnemyEnergy);
                Us1.Send(number, MsgType.AddCarteOnField);
                Us2.Send(idCards, MsgType.EnemyAddCarteOnField);
            }
            else
            {
                Us2.Send(Us2.Energy, MsgType.YourEnergy);
                Us1.Send(Us2.Energy, MsgType.EnemyEnergy);
                Us2.Send(number, MsgType.AddCarteOnField);
                Us1.Send(idCards, MsgType.EnemyAddCarteOnField);
            }
           
        }
        private void DisconnectClient(User user)
        {
            //отвязваемся от игроков, чтобы этот метод не был запущен вторично
            Us1.FailedSendMsg -= DisconnectClient;
            Us2.FailedSendMsg -= DisconnectClient;

            //отправляем сообщение о победе противоположному игроку 
            if (user == Us1)
                Us2.Send(MsgType.TechnicalVictory);
            else
                Us1.Send(MsgType.TechnicalVictory);
            //завершаем сессию
            Dispose();

        }
        public void StartSession(object sender)
        {
            try
            {
                //инициализируем
                Us1.Initialize();
                Us2.Initialize();
                //привязываем обработчик ошибок связанных с отправкой сообщений
                Us1.FailedSendMsg += DisconnectClient;
                Us2.FailedSendMsg += DisconnectClient;
                //посылам сообщение о начале сессии
                Us1.Send(MsgType.StartSession);
                Us2.Send(MsgType.StartSession);

                //ожидаем получение сообщения о получении колод
                while (ColodsRec != 2)
                {
                    Thread.Sleep(1);
                }
                //привязываем обработчики события
                Us1.NameRec += RecName;
                Us2.NameRec += RecName;
                //колоды получены
                //получаем имя
                Us1.Send(MsgType.GetName);
                Us2.Send(MsgType.GetName);

                //ожидаем ответа
                while (NameRec != 2)
                {
                    Thread.Sleep(1);
                }
                //имена получены
                //отправляем пользователям сообщение о создании новой формы, и посылаем имена противников
                Us1.Send(Us2.Name, MsgType.StartGame);
                Us2.Send(Us1.Name, MsgType.StartGame);
                Debug.Write("Имена отправлены");

                //устанваливаем очередность ходов случайным образом
                Random rand = new Random();
                switch (rand.Next(0, 1))
                {
                    case 0:
                        UsProgress = Us1;
                        Us1.MyProgress = true;
                        Us2.MyProgress = false;
                        Debug.WriteLine("Первым ходится первый игрок");
                        break;
                    case 1:
                        UsProgress = Us2;
                        Us1.MyProgress = false;
                        Us2.MyProgress = true;
                        Debug.WriteLine("Первым ходится второй игрок");
                        break;
                }


                Thread.Sleep(500);
                //создаем пулы потоков для создания колоды карт у каждого игрока
                ThreadPool.QueueUserWorkItem(LoadGame, Us1);
                ThreadPool.QueueUserWorkItem(LoadGame, Us2);
                while (GameLoad != 2)
                {
                    Thread.Sleep(1);
                }
                Us1.Attack += AttackFunc;
                Us2.Attack += AttackFunc;
                Us1.EndProgress += NewProgress;
                Us2.EndProgress += NewProgress;
                //инициализируем таймер
                SecProgress = 120;
                TempProgress = new System.Timers.Timer();
                TempProgress.Interval = 1000;
                TempProgress.Elapsed += TempProgress_Elapsed;
                TempProgress.Start();
            }
            catch(Exception e)
            { Console.WriteLine(e.ToString()); }
        }

        private void TempProgress_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (SecProgress > 0)
                {
                    SecProgress--;
                    string Sec = null;
                    if (SecProgress % 60 < 10) Sec = "0" + SecProgress % 60;
                    else Sec = Convert.ToString(SecProgress % 60);
                    string Min = "0" + SecProgress / 60;

                    Us1.Send($"{Min}:{Sec}", MsgType.ProgressTime);
                    Us2.Send($"{Min}:{Sec}", MsgType.ProgressTime);

                }
                else
                {
                    TempProgress.Stop();
                    NewProgress();
                }
            }
            catch (Exception E)
            { Console.WriteLine(E.ToString()); }
        }

        
        private void AttackFunc(User sender, int attacking, int attacked)
        {
            try
            {
                if (sender == Us1)
                {
                    if (attacking == -1)
                    {
                        if (attacked == -1)
                        {
                            //урон по противнику
                            Us2.userHQ.Armor -= Us1.userHQ.Attack;
                            //урон от противника
                            Us1.userHQ.Armor -= Us2.userHQ.Attack;

                        }
                        else
                        {
                            Us2.CardsMargin[attacked].Armor -= Us1.userHQ.Attack;
                            Us1.userHQ.Armor -= Us2.CardsMargin[attacked].Attack;
                            //если очков прочности меньше 0 удаляем карту
                            if (Us2.CardsMargin[attacked].Armor <= 0)
                            {
                                Us2.CardsMargin.RemoveAt(attacked);
                            }
                        }
                    }
                    else
                    {
                        if (attacked == -1)
                        {
                            Us1.CardsMargin[attacking].Armor -= Us2.userHQ.Attack;
                            Us2.userHQ.Armor -= Us1.CardsMargin[attacking].Attack;
                            //если очков прочности меньше 0 удаляем карту
                            if (Us1.CardsMargin[attacking].Armor <= 0)
                            {
                                Us1.CardsMargin.RemoveAt(attacking);
                            }
                        }
                        else
                        {
                            //карты атакуют друг друга
                            Us1.CardsMargin[attacking].Armor -= Us2.CardsMargin[attacked].Attack;
                            Us2.CardsMargin[attacked].Armor -= Us1.CardsMargin[attacking].Attack;
                            //если их очки прочности меньше 0, то удаляем их
                            if (Us1.CardsMargin[attacking].Armor <= 0)
                                Us1.CardsMargin.RemoveAt(attacking);
                            if (Us2.CardsMargin[attacked].Armor <= 0)
                                Us2.CardsMargin.RemoveAt(attacked);

                        }
                    }

                    //отправляем о сообщение об удачной атаке
                    Us1.Send(new int[] { attacking, attacked }, MsgType.MyAttackSucc);
                    Us2.Send(new int[] { attacking, attacked }, MsgType.EnAttackSucc);
                }
                else//если пользователь Us2
                {
                    if (attacking == -1)
                    {
                        if (attacked == -1)
                        {
                            //урон по противнику
                            Us1.userHQ.Armor -= Us2.userHQ.Attack;
                            //урон от противника
                            Us2.userHQ.Armor -= Us1.userHQ.Attack;
                            //отправляем о сообщение об удачной атаке

                        }
                        else
                        {

                            Us1.CardsMargin[attacked].Armor -= Us2.userHQ.Attack;
                            Us2.userHQ.Armor -= Us1.CardsMargin[attacked].Attack;
                            //если очков прочности меньше 0 удаляем карту
                            if (Us1.CardsMargin[attacked].Armor <= 0)
                                Us1.CardsMargin.RemoveAt(attacked);

                        }
                    }
                    else
                    {
                        if (attacked == -1)
                        {
                            Us2.CardsMargin[attacking].Armor -= Us1.userHQ.Attack;
                            Us1.userHQ.Armor -= Us2.CardsMargin[attacking].Attack;

                            //если очков прочности меньше 0 удаляем карту
                            if (Us2.CardsMargin[attacking].Armor <= 0)
                                Us2.CardsMargin.RemoveAt(attacking);


                        }
                        else
                        {
                            //карты атакуют друг друга
                            Us2.CardsMargin[attacking].Armor -= Us1.CardsMargin[attacked].Attack;
                            Us1.CardsMargin[attacked].Armor -= Us2.CardsMargin[attacking].Attack;
                            //если очки прочности карты меньше 0, удаляем ее
                            if (Us2.CardsMargin[attacking].Armor <= 0)
                                Us2.CardsMargin.RemoveAt(attacking);
                            if (Us1.CardsMargin[attacked].Armor <= 0)
                                Us1.CardsMargin.RemoveAt(attacked);
                        }
                    }
                    Us2.Send(new int[] { attacking, attacked }, MsgType.MyAttackSucc);
                    Us1.Send(new int[] { attacking, attacked }, MsgType.EnAttackSucc);
                }
                //проверяем на конец игры
                if ((Us1.userHQ.Armor <= 0 && Us2.userHQ.Armor <= 0))
                {
                    Us1.Send(MsgType.Draw);
                    Us2.Send(MsgType.Draw);
                    return;
                }
                if (Us1.userHQ.Armor <= 0)
                {
                    Us1.Send(MsgType.YouOver);
                    Us2.Send(MsgType.YouWin);
                    return;
                }
                if (Us2.userHQ.Armor <= 0)
                {
                    Us2.Send(MsgType.YouOver);
                    Us1.Send(MsgType.YouWin);
                    return;
                }
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString()); }
        }
        private void Dispose()
        {
            Us1.Dispose();
            Us2.Dispose();

            //очищаем ресурсы сессии
            Us1 = null;
            Us2 = null;
            TempProgress.Dispose();
            TempProgress = null;
            SessionEnd(this);
            SessionEnd = null;

        }
        private void NewProgress()
        {
            try
            {
                Random rand = new Random();
                //обвновляем атаки карт
                UsProgress.userHQ.NewProgress();
                int Count = UsProgress.CardsMargin.Count;
                for (int i = 0; i < Count; i++)
                {
                    UsProgress.CardsMargin[i].NewProgress();
                }

                //обновляем показатели энергии
                if (UsProgress.MaxEnergy < 11) UsProgress.MaxEnergy++;
                UsProgress.Energy = UsProgress.MaxEnergy;


                if (UsProgress == Us1)
                {
                    //отправляем новые показатели энергии
                    Us1.Send(Us1.MaxEnergy, MsgType.UserMaxEnergy);
                    Us2.Send(Us1.MaxEnergy, MsgType.EnemyMaxEnergy);
                    Us1.Send(Us1.Energy, MsgType.YourEnergy);
                    Us2.Send(Us1.Energy, MsgType.EnemyEnergy);
                    //меняем игрока
                    UsProgress = Us2;
                    Us1.MyProgress = false;
                    Us2.MyProgress = true;
                    //добавляем  игроку карту
                    int IdCarte = Us2.CarteUser[rand.Next(0, 6)];
                    Us2.CartsHand.Add(IdCarte);
                    Us1.Send(MsgType.AddEnemyCarte);
                    Us2.Send(IdCarte, MsgType.AddUserCarte);

                }
                else
                {
                    //отправляем новые показатели энергии
                    Us2.Send(Us2.MaxEnergy, MsgType.UserMaxEnergy);
                    Us1.Send(Us2.MaxEnergy, MsgType.EnemyMaxEnergy);
                    Us2.Send(Us2.Energy, MsgType.YourEnergy);
                    Us1.Send(Us2.Energy, MsgType.EnemyEnergy);

                    //меняем игрока
                    UsProgress = Us1;
                    Us1.MyProgress = true;
                    Us2.MyProgress = false;

                    //добавляем  игроку карту
                    int IdCarte = Us1.CarteUser[rand.Next(0, 6)];
                    Us1.CartsHand.Add(IdCarte);
                    Us1.Send(IdCarte, MsgType.AddUserCarte);
                    Us2.Send(MsgType.AddEnemyCarte);

                }


                //отправляем карты, которые находятся у него в руках
                Us1.Send("2:00", MsgType.ProgressTime);
                Us2.Send("2:00", MsgType.ProgressTime);


                //отправляем сообщения о том, что сейчас ход этого клиента
                if (Us1 == UsProgress)
                {
                    Us1.Send(MsgType.MyProgress);
                    Us2.Send(MsgType.EnemyProgress);
                }
                else
                {
                    Us2.Send(MsgType.MyProgress);
                    Us1.Send(MsgType.EnemyProgress);
                }
                SecProgress = 120;
                TempProgress.Start();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        private void LoadGame(object stateInfo)
        {
            try
            {
                User user = (User)stateInfo;

                //отправляем карты, которые находятся у него в руках
                Random rand = new Random();
                int count = 0;
                if (user == UsProgress) count = 7;
                else count = 6;
                for (int i = 0; i < count; i++)
                {
                    int IdCarte = user.CarteUser[rand.Next(0, 6)];
                    //добавляем id в массив карт, находящихся в руках у игрока
                    user.CartsHand.Add(IdCarte);

                    //отправляем сообщения клиентам, о том что надо добавить карту 
                    user.Send(IdCarte, MsgType.AddUserCarte);
                    if (user == Us1) Us2.Send(MsgType.AddEnemyCarte);
                    else Us1.Send(MsgType.AddEnemyCarte);
                }




                //отправляем энергию игрока энергию
                user.Energy = ++user.MaxEnergy;
                user.Send(user.MaxEnergy, MsgType.UserMaxEnergy);
                user.Send(user.Energy, MsgType.YourEnergy);
                if (user == Us1)
                {
                    Us2.Send(user.MaxEnergy, MsgType.EnemyMaxEnergy);
                    Us2.Send(user.Energy, MsgType.EnemyEnergy);
                }
                else
                {
                    Us1.Send(user.MaxEnergy, MsgType.EnemyMaxEnergy);
                    Us1.Send(user.Energy, MsgType.EnemyEnergy);
                }

                //отправляем сообщения о том, что сейчас ход этого клиента
                if (user == UsProgress) user.Send(MsgType.MyProgress);
                else user.Send(MsgType.EnemyProgress);

                //устанавливаем время на 1 минуту
                user.Send("2:00", MsgType.ProgressTime);
                GameLoad++;
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString()); }
        }
    }
}
