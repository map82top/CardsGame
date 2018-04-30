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
        ClientClosing,
        EnemyNoActiv,
        YouNoActiv,
        DamageEvent,
        UserDamageEvent, 
        EnemyDamageEvent,
        RepairsEvent,
        UsRepairsEvent,
        EnRepairsEvent



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
        /// <summary>
        /// Создает эффекты при добавлении карты на поле
        /// </summary>
        /// <param name="us"></param>
        /// <param name="idCards"></param>
        private void EffectAddCardOfField(User us, int idCards)
        {
            
            switch (Carte.GetCarte(idCards).GetType().Name)
            {
                //если эта карта ветеран
                case "Veteran":
                    //добавляем бонус всем картам кроме карт типа ветеран
                    foreach (Robot Card in us.CardsMargin)
                        if (!(Card is Veteran))
                            Card.BonusAttack += 1;
                    break;
                default:
                    //если карта не типа ветеран,то считаем сколько у нас карт ветеранов и добавляем
                    //сответствующий размер бонуса
                    int ValueVeteran = 0;
                    foreach (Robot Card in us.CardsMargin)
                        if (Card is Veteran)
                            ValueVeteran += 1;

                    us.CardsMargin[us.CardsMargin.Count - 1].BonusAttack += ValueVeteran;
                    break;
            }
        }
        private void ForAddCardOnField(User us1, User us2, int number, int idCards)
        {
            //выполеняем необходимы действия при добавлении карточки
            EffectAddCardOfField(us1, idCards);
            us1.Send(us1.Energy, MsgType.YourEnergy);
            us2.Send(us1.Energy, MsgType.EnemyEnergy);
            us1.Send(number, MsgType.AddCarteOnField);
            us2.Send(idCards, MsgType.EnemyAddCarteOnField);
        }
        /// <summary>
        /// Обрабатывает добавление новой карты на игровое поле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="number"></param>
        /// <param name="idCards"></param>
        private void AddCardOnField(User sender, int number, int idCards)
        {
            if (sender == Us1)
            {
                ForAddCardOnField(Us1, Us2, number, idCards);
            }
            else
            {
                ForAddCardOnField(Us2, Us1, number, idCards);
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

        private void DisconnectClient()
        {
            //отвязваемся от игроков, чтобы этот метод не был запущен вторично
            Us1.FailedSendMsg -= DisconnectClient;
            Us2.FailedSendMsg -= DisconnectClient;

            //отправляем сообщение о победе противоположному игроку 
            if (UsProgress == Us1)
            {
                Us1.Send(MsgType.YouNoActiv);
                Us2.Send(MsgType.EnemyNoActiv);
            }
            else
            {
                Us2.Send(MsgType.YouNoActiv);
                Us1.Send(MsgType.EnemyNoActiv);
            }
              
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
                //привязываем обработчик обработки атаки картой робота
                Us1.Attack += AttackFunc;
                Us2.Attack += AttackFunc;
                //привязываем обработчик атаки 
                Us1.DamageCardEvent += DamageEventFunc;
                Us2.DamageCardEvent += DamageEventFunc;
                //привязываем обработчик ремонта 
                Us1.RepairsCardEvent += RepairsEventFunc;
                Us2.RepairsCardEvent += RepairsEventFunc;
                //привязываем обработчик окночания хода игрока
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
                    if (!UsProgress.UserNoProgress)
                    {
                      
                        TempProgress.Stop();
                        UsProgress.UserNoProgress = true;
                        NewProgress();
                       
                    }
                    else
                    {
                        DisconnectClient();
                       
                    }
                   
                }
            }
            catch (Exception E)
            { Console.WriteLine(E.ToString()); }
        }
        private void DamageEventFunc(User user,int IDAttacking, DamageEvent Card,int attacking, int attacked)
        {
            if (user == Us1)
            {
                if (attacked == -1)
                {
                    Us2.userHQ.Armor -= Card.Damage;
                }
                else
                {
                    Us2.CardsMargin[attacked].Armor -= Card.Damage;
                    if(Us2.CardsMargin[attacked].Armor <= 0) Us2.CardsMargin.RemoveAt(attacked);
                }
                //отправляем о сообщение об удачной атаке
                Us1.Send(Us1.Energy, MsgType.YourEnergy);
                Us2.Send(Us1.Energy, MsgType.EnemyEnergy);
                Us1.Send(new int[] { attacking, attacked, Card.Damage }, MsgType.UserDamageEvent);
                Us2.Send(new int[] { attacking, IDAttacking, attacked, Card.Damage }, MsgType.EnemyDamageEvent);
            }
            else if (user == Us2)
            {
                if (attacked == -1)
                {
                    Us1.userHQ.Armor -= Card.Damage;

                }
                else
                {
                    Us1.CardsMargin[attacked].Armor -= Card.Damage;
                    if (Us1.CardsMargin[attacked].Armor <= 0) Us1.CardsMargin.RemoveAt(attacked);
                }
                //отправляем о сообщение об удачной атаке
                Us2.Send(Us2.Energy, MsgType.YourEnergy);
                Us1.Send(Us2.Energy, MsgType.EnemyEnergy);
                Us2.Send(new int[] {attacking, attacked, Card.Damage }, MsgType.UserDamageEvent);
                Us1.Send(new int[] {attacking, IDAttacking, attacked, Card.Damage }, MsgType.EnemyDamageEvent);
            }
            //проверяем на уничтожение одного из штабов
            TestEndGame();
        }
        private void RepairsEventFunc(User user, int IDRepairs, RepairsEvent Card, int NumberCardRepairs, int Repairable)
        {
            if (user == Us1)
            {
                if( Repairable == -1)
                    Us1.userHQ.Armor -= Card.Damage;
                else
                    Us1.CardsMargin[Repairable].Armor -= Card.Damage;
                   
                //отправляем о сообщение об удачной атаке
                Us1.Send(Us1.Energy, MsgType.YourEnergy);
                Us2.Send(Us1.Energy, MsgType.EnemyEnergy);
                Us1.Send(new int[] { NumberCardRepairs, Repairable, Card.Damage }, MsgType.UsRepairsEvent);
                Us2.Send(new int[] { NumberCardRepairs, IDRepairs, Repairable, Card.Damage }, MsgType.EnRepairsEvent);
            }
            else if (user == Us2)
            {
                if (Repairable == -1)
                    Us2.userHQ.Armor -= Card.Damage;

                else
                    Us1.CardsMargin[Repairable].Armor -= Card.Damage;
                   
                //отправляем о сообщение об удачной атаке
                Us2.Send(Us2.Energy, MsgType.YourEnergy);
                Us1.Send(Us2.Energy, MsgType.EnemyEnergy);
                Us2.Send(new int[] { NumberCardRepairs, Repairable, Card.Damage }, MsgType.UsRepairsEvent);
                Us1.Send(new int[] { NumberCardRepairs, IDRepairs, Repairable, Card.Damage }, MsgType.EnRepairsEvent);
            }
            //проверяем на уничтожение одного из штабов
            TestEndGame();
        }
        /// <summary>
        /// Выполняет нектороый действия происходящие при удалении некоторой карты
        /// </summary>
        /// <param name="Us"></param>
        /// <param name="index"></param>
        private void EffectDeliteCard(User Us, int index)
        {

            if (Us.CardsMargin[index].Armor <= 0)
            {
                switch (Us.CardsMargin[index].GetType().Name)
                {//если эта карта ветеран
                    case "Veteran":
                        //уменьшаем бонус к атаке у всех карт
                        foreach (Robot Card in Us.CardsMargin)
                            if (!(Card is Veteran))
                               Card.BonusAttack -= 1;
                        break;

                }
                Us.CardsMargin.RemoveAt(index);
            }
        }

        /// <summary>
        /// Дробит функции обработки атаки на уровне сессии на части
        /// </summary>
        /// <param name="Us1"></param>
        /// <param name="Us2"></param>
        /// <param name="attacking"></param>
        /// <param name="attacked"></param>
        private void ForAttackFunc(User us1,User us2, int attacking, int attacked)
        {
             int damageEnemy = 0, damageUser = 0;
           
                    if (attacking == -1)
                    {
                        if (attacked == -1)
                        {
                            //урон по противнику
                            damageEnemy = us1.userHQ.Attack;
                            us2.userHQ.Armor -= damageUser;
                            //урон от противника
                             damageUser = us2.userHQ.Attack;
                            us1.userHQ.Armor -= damageUser;
                        }
                        else
                        {

                             damageEnemy = us1.userHQ.Attack;
                            us2.CardsMargin[attacked].Armor -= damageEnemy;
                            damageUser = us2.CardsMargin[attacked].Attack + us2.CardsMargin[attacked].BonusAttack;
                            us1.userHQ.Armor -= damageUser;
                            //если очков прочности меньше 0 удаляем карту
                            EffectDeliteCard(us2, attacked);
                 
                        }
                    }
                    else
                    {
                        if (attacked == -1)
                        {
                            damageEnemy = us1.CardsMargin[attacking].Attack + us1.CardsMargin[attacking].BonusAttack;
                            us2.userHQ.Armor -= damageEnemy;
                            damageUser = us2.userHQ.Attack;
                            us1.CardsMargin[attacking].Armor -= damageUser;
                            //если очков прочности меньше 0 удаляем карту
                            EffectDeliteCard(us1, attacking);
                            
                        }
                        else
                        {
                            //карты атакуют друг друга
                            damageEnemy = us1.CardsMargin[attacking].Attack + us1.CardsMargin[attacking].BonusAttack;
                            us2.CardsMargin[attacked].Armor -= damageEnemy;
                            damageUser = us2.CardsMargin[attacked].Attack + us2.CardsMargin[attacked].BonusAttack;
                            us1.CardsMargin[attacking].Armor -= damageUser;
                            //если их очки прочности меньше 0, то удаляем их
                            EffectDeliteCard(us1, attacking);
                            EffectDeliteCard(us2, attacked);
                           
                        }
                    }

            Debug.WriteLine("Урон по карте пользователя " + damageUser + "  Урон по карте врага " + damageEnemy);
                    //отправляем о сообщение об удачной атаке
                    us1.Send(new int[] { attacking, attacked, damageUser, damageEnemy }, MsgType.MyAttackSucc);
                    us2.Send(new int[] { attacking, attacked, damageUser, damageEnemy }, MsgType.EnAttackSucc);//поменял уроны местами из-за ввода универсальной функции AttackSucc
        }

        /// <summary>
        /// Обрабатывает атаки пользователей на уровне пользователей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="attacking"></param>
        /// <param name="attacked"></param>
        private void AttackFunc(User sender, int attacking, int attacked)
        {
            try
            {
                
                if (sender == Us1)
                {
                    ForAttackFunc(Us1, Us2, attacking, attacked);
                }
                else//если пользователь Us2
                {
                    ForAttackFunc(Us2, Us1, attacking, attacked);
                }
                //проверяем на уничтожение одного из штабов
                TestEndGame();
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString()); }
        }
        private void TestEndGame()
        {
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

        /// <summary>
        /// Освобождает все ресурсы сессии
        /// </summary>
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

        /// <summary>
        /// Управляет созданиием нового хода
        /// </summary>
        private void NewProgress()
        {
            try
            {
                if (SecProgress > 0)
                {

                    UsProgress.UserNoProgress = false;
                }
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
