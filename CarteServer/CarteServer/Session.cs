using System;
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
        StartSession,//начало игры и сессии на сервере
        DeliteSeek,//запрос на удалении их очереди ожидания противника
        CarteUser,//запрос сервера на получении карт игрока
        GetName,//запрос сервера на получении имени игрока
        StartGame,//сервер сообщает о начале игры
        AddUserCarte,//сервер добавлет карту игроку в руки
        AddEnemyCarte,//сервер добавляет карту противнику в руки
        UserMaxEnergy,//сервер уведомляет игрока о его максимальной энергии на этот ход
        YourEnergy,//сервер сообщает о доступной энергии игрока в данный момент
        EnemyMaxEnergy,//максимальной количество энергии у противника
        EnemyEnergy,//сервер сообщает о доступной энергии противника в данный момент
        ProgressTime,//сервер сообщает об изменении времени, оставшегося на ход
        MyProgress,//сервер сообщает, что сейчас ход игрока
        EnemyProgress,//сервер сообщает, что сейчас ход противника
        AddCarteOnField,//сервер сообщает о удачном добавлении игроком карты на поле
        EnemyAddCarteOnField,//сервер сообщает о удачно добавлении противником карты на поле
        Attack,//клиент сообщает о атаке игрока на карты противника
        MyAttackSucc,//атака игрока закончилась успешно(была возможна)
        EnAttackSucc, //атака противника закончилась успешно(была возможна)
        EndProgress,//клиент сообщает о завершении игроком своего хода
        YouWin,//игрок победил
        YouOver,//игрок проиграл
        Draw,//ничья
        TechnicalVictory,//техничская победа(один из игроков недоступен)
        ClientClosing,//клиент собщает серверу о своем закрытии пользователем
        EnemyNoActiv,//сервер сообщает пользователю, что противник был не активен и поэтом сессия завершилась 
        YouNoActiv, //сервер сообщает игроку, что игрок был не активен и поэтом сессия завершилась 
        DamageEvent,//клиент уведомляет сервер о использовании события, наносящего одиночный урон
        UserDamageEvent,//сервер соощеает клиенту об удачно использовании события, наносящего одиночный урон
        EnemyDamageEvent,//враг удачно использовал событие, наносящее одиночный урон 
        RepairsEvent,//пользователь использовал событие востанволения
        UsRepairsEvent,//сервер сообщает об удачном использовании игроком событием восстановления
        EnRepairsEvent, //сервер сообщает об удачном использовании противником событием восстановления
        AllDamageEvent,//клиент сообщает серверу о исплользовании события массвого урона
        UsAllDeliteEvent,//сервер сообщает о удачном использовании игроком события наносщего массовый урон
        EnAllDeliteEvent,//сервер сообщает о удачном использовании противником события наносщего массовый урон
        ChatMsg//собщает о получении сообщения для чата

    }
    delegate void EndSession(Session sender);
    class Session: IDisposable
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
        private const int TimeProgress = 180;//длительнсть хода 
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
            try
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
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }

        }
        //считает количестов карт-ветеран на игровом поле
        private int SeekVeteran(User us)
        {
            int ValueVeteran = 0;
            foreach (Robot Card in us.CardsMargin)
                if (Card is Veteran)
                    ValueVeteran += 1;
            return ValueVeteran;
        }
        //осуществляет поиск оборонитльных сооружений на игровом поле игрока
        private DefenceConstr SeekDefenceConstr(User us)
        {
            foreach (Robot Card in us.CardsMargin)
                if (Card is DefenceConstr)//ищем самого первого защитника
                {
                    return Card as DefenceConstr;
                    
                }
            return null;
        }
        /// <summary>
        /// Создает эффекты при добавлении карты на поле
        /// </summary>
        /// <param name="us"></param>
        /// <param name="idCards"></param>
        private void EffectAddCardOfField(User us, int idCards)
        {
            
            switch (Carte.GetCarte(idCards).ID)
            {
                //если эта карта ветеран
                case 3:
                    //добавляем бонус всем картам кроме карт типа ветеран
                    foreach (Robot Card in us.CardsMargin)
                        if (!(Card is Veteran))
                            Card.BonusAttack += 1;

                    //если у нас есть защитник, то добавляем ссылку на него новой карте
                    us.CardsMargin[us.CardsMargin.Count - 1].Defender = SeekDefenceConstr(us);
                    break;
                case 9:
                    //добавляем на поле еще одну карту дроида B1
                        us.CardsMargin.Add((Robot)Carte.GetCarte(idCards));
                    //если карта не типа ветеран,то считаем сколько у нас карт ветеранов и добавляем
                    //сответствующий размер бонуса
                    int Bonus = SeekVeteran(us);
                    int countCards = us.CardsMargin.Count - 1;
                    us.CardsMargin[countCards].BonusAttack += Bonus;
                    us.CardsMargin[countCards-1].BonusAttack += Bonus;

                    //если у нас есть защитник, то добавляем ссылку на него новой карте
                    DefenceConstr temp = SeekDefenceConstr(us);
                    us.CardsMargin[countCards].Defender = temp;
                    us.CardsMargin[countCards-1].Defender = temp;
                    break;
                case 12:
                    //ищем карту, не являющуюся защитным сооружением
                  
                    foreach (Robot Card in us.CardsMargin)
                        if (!(Card is DefenceConstr))
                        {
                            //если уже есть защитник
                            if (Card.Defender != null)
                            {
                               DefenceConstr LocalCard = Card.Defender;
                                //ищем последнего защитника
                                while (LocalCard.Defender != null)
                                {
                                    LocalCard = LocalCard.Defender;
                                }
                                //добавляем первому защитнику ссылку на нового защитника
                                LocalCard.Defender = (DefenceConstr)us.CardsMargin[us.CardsMargin.Count - 1];
                            }
                            else
                            {
                                int index = us.CardsMargin.Count - 1;
                                //добавляем все картам ссылку на защитника
                                foreach (Robot CardNoDefender in us.CardsMargin)
                                    if (!(CardNoDefender is DefenceConstr)) CardNoDefender.Defender = (DefenceConstr)us.CardsMargin[index];
                                //добавляем защитника штабу
                                us.userHQ.Defender = (DefenceConstr)us.CardsMargin[index];
                            }
                            //выходим из цикла
                            break;                         
                        }
                         
                        
                    break;
                case 16:
                    //добавляем бонус ко всем картам
                    int count = us.CardsMargin.Count - 1;
                        for(int i =0; i < count; i++) us.CardsMargin[i].Armor += 2;

                    //если карта не типа ветеран,то считаем сколько у нас карт ветеранов и добавляем
                    //сответствующий размер бонуса
                    us.CardsMargin[us.CardsMargin.Count - 1].BonusAttack += SeekVeteran(us);

                    //если у нас есть защитник, то добавляем ссылку на него новой карте
                    us.CardsMargin[us.CardsMargin.Count - 1].Defender = SeekDefenceConstr(us);
                    break;
                case 18:
                    //добавляем на поле карту Гончей
                    us.CardsMargin.Add((Robot)Carte.GetCarte(19));
                    //если карта не типа ветеран,то считаем сколько у нас карт ветеранов и добавляем
                    //сответствующий размер бонуса
                    int bonus = SeekVeteran(us);
                    int countCardsMargin = us.CardsMargin.Count - 1;
                    us.CardsMargin[countCardsMargin].BonusAttack += bonus;
                    us.CardsMargin[countCardsMargin-1].BonusAttack += bonus;

                    //если у нас есть защитник, то добавляем ссылку на него новой карте
                    DefenceConstr Temp = SeekDefenceConstr(us);
                    us.CardsMargin[countCardsMargin].Defender = Temp;
                    us.CardsMargin[countCardsMargin-1].Defender = Temp;
                    break;
                default:
                    //если карта не типа ветеран,то считаем сколько у нас карт ветеранов и добавляем
                    //сответствующий размер бонуса
                    us.CardsMargin[us.CardsMargin.Count - 1].BonusAttack += SeekVeteran(us);

                    //если у нас есть защитник, то добавляем ссылку на него новой карте
                    us.CardsMargin[us.CardsMargin.Count - 1].Defender = SeekDefenceConstr(us);
                  break;
            }
        }
        private void ForAddCardOnField(User us1, User us2, int number, int idCards)
        {
            //выполеняем необходимы действия при добавлении карточки
            EffectAddCardOfField(us1, idCards);
            WriteLog.WriteGameLog($"Карт на игровом поле игрока {us1.Name} {us1.CardsMargin.Count}");
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
            try
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
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }

        //по недоступности одного из игроков
        private void DisconnectClient(User user)
        {
            try
            {
                //отвязваемся от игроков, чтобы этот метод не был запущен вторично
                Us1.FailedSendMsg -= DisconnectClient;
                Us2.FailedSendMsg -= DisconnectClient;
                WriteLog.WriteGameLog($"Произошел разрыв связи с игроком {user.Name}");
                //отправляем сообщение о победе противоположному игроку 
                if (user == Us1)
                    Us2.Send(MsgType.TechnicalVictory);
                else
                    Us1.Send(MsgType.TechnicalVictory);
                //завершаем сессию
                Dispose();
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }

        }
        //по неактивности
        private void DisconnectClient()
        {
            try
            { //отвязваемся от игроков, чтобы этот метод не был запущен вторично
                Us1.FailedSendMsg -= DisconnectClient;
                Us2.FailedSendMsg -= DisconnectClient;
                WriteLog.WriteGameLog("Произошел разрыв связи");
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
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
        //начинаем игру
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
                WriteLog.WriteGameLog("Отправлено сообщение о начале игровй сессии");
                Us1.Send(MsgType.StartSession);
                Us2.Send(MsgType.StartSession);

                //ожидаем получение сообщения о получении колод
                while (ColodsRec != 2)
                {
                    Thread.Sleep(1);
                }
                WriteLog.WriteGameLog("Получены колоды от обоих игроков");
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
                WriteLog.WriteGameLog("Получены ники игроков");
                WriteLog.WriteGameLog($"Игра {Us1.Name} против {Us2.Name}");
                Console.WriteLine($" Игра {Us1.Name} против {Us2.Name}");
                //имена получены
                //отправляем пользователям сообщение о создании новой формы, и посылаем имена противников
                Us1.Send(Us2.Name, MsgType.StartGame);
                Us2.Send(Us1.Name, MsgType.StartGame);

                //устанваливаем очередность ходов случайным образом
                Random rand = new Random();
                switch (rand.Next(0, 2))
                {
                    case 0:
                        UsProgress = Us1;
                        Us1.MyProgress = true;
                        Us2.MyProgress = false;
                        WriteLog.WriteGameLog($"Первым ходит {Us1.Name}");
                        break;
                    case 1:
                        UsProgress = Us2;
                        Us1.MyProgress = false;
                        Us2.MyProgress = true;
                        WriteLog.WriteGameLog($"Первым ходит {Us2.Name}");
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
                WriteLog.WriteGameLog($"Игроки получили всю информацию для начал игры");
                //привязываем обработчик обработки атаки картой робота
                Us1.Attack += AttackFunc;
                Us2.Attack += AttackFunc;
                //привязываем обработчик атаки 
                Us1.DamageCardEvent += DamageEventFunc;
                Us2.DamageCardEvent += DamageEventFunc;
                //привязываем обработчик ремонта 
                Us1.RepairsCardEvent += RepairsEventFunc;
                Us2.RepairsCardEvent += RepairsEventFunc;
                //привязываем обработчик массовой атаки
                Us1.AllDamageEvent += AllDamageEvent;
                Us2.AllDamageEvent += AllDamageEvent;
                //привязываем обработчик окночания хода игрока
                Us1.EndProgress += NewProgress;
                Us2.EndProgress += NewProgress;
                //добавляем обработчик пересылки собщения из чата
                Us1.NewChatMsg += ForwardMsg;
                Us2.NewChatMsg += ForwardMsg;
                //инициализируем таймер
                SecProgress = TimeProgress;
                TempProgress = new System.Timers.Timer();
                TempProgress.Interval = 1000;
                TempProgress.Elapsed += TempProgress_Elapsed;
                TempProgress.Start();
            }
            catch(Exception e)
            { WriteLog.Write(e.ToString()); }
        }

        //отправляем сообщение чата противоположному игроку
        private void ForwardMsg(User user, string message)
        {
            if (user == Us1) Us2.Send(message, MsgType.ChatMsg);
            else Us1.Send(message, MsgType.ChatMsg);

        }
        private void ForAllDamageEvent(User us1,User us2,  int IdCard, AllDamageEvent Card, int NumberCard)
        {
            //наносим всем картам урон
            WriteLog.WriteGameLog($"Игрок {us1.Name} использовал событие массового урона по игроку {Us2.Name}");
            int count = us2.CardsMargin.Count;
            for (int i = 0; i < count; i++)
            {       
                us2.CardsMargin[i].Armor -= Card.AllDamage;
                WriteLog.WriteGameLog($"Прочность карты { us2.CardsMargin[i].NameRobot} c номером на поле {i+1} равна {us2.CardsMargin[i].Armor}");

            }
            //удаляем карты если их hp меньше 0
            for (int i = 0; i < count; i++)
            {
                EffectDeliteCard(us2, i);
                if (us2.CardsMargin.Count < count)
                {
                    i--;
                    count--;
                }

            }
            WriteLog.WriteGameLog($"Карт в колоде игрока {us2.Name} равно {us2.CardsMargin.Count}");
            //отправляем о сообщение об удачной атаке
            us1.Send(us1.Energy, MsgType.YourEnergy);
            us2.Send(us2.Energy, MsgType.EnemyEnergy);
            us1.Send(new int[] { NumberCard, Card.AllDamage }, MsgType.UsAllDeliteEvent);
            us2.Send(new int[] { IdCard, NumberCard, Card.AllDamage }, MsgType.EnAllDeliteEvent);
        }
        /// <summary>
        /// Обрабатывает события связанные с использованием карт массового урона
        /// </summary>
        /// <param name="us"></param>
        /// <param name="IdCard"></param>
        /// <param name="Card"></param>
        /// <param name="NumberCard"></param>
        private void AllDamageEvent(User user, int IdCard, AllDamageEvent Card,int NumberCard)
        {
            try
            {
                if (user == Us1)
                {
                    ForAllDamageEvent(Us1, Us2, IdCard, Card, NumberCard);

                }
                else
                {
                    ForAllDamageEvent(Us2, Us1, IdCard, Card, NumberCard);
                }
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
        //таймер обратного отсчета времени хода
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
                        WriteLog.WriteGameLog($"Игрок {UsProgress.Name} весь ход был неактивен");
                        NewProgress();
                       
                    }
                    else
                    {
                        WriteLog.WriteGameLog($"Игрок {UsProgress.Name} удален по неактивности");
                        DisconnectClient();
                       
                    }
                   
                }
            }
            catch (Exception E)
            { WriteLog.Write(E.ToString()); }
        }
        //осуществляет обработки события использовании карты нанесния одиночног урона
        private void DamageEventFunc(User user,int IDAttacking, DamageEvent Card,int attacking, int attacked)
        {
            try
            {
                if (user == Us1)
                {
                    if (attacked == -1)
                    {
                        WriteLog.WriteGameLog($"Игрок {user.Name} использовал событие одиночного урона по штабу игрока {Us2.Name}");
                        Us2.userHQ.Armor -= Card.Damage;
                        WriteLog.WriteGameLog($"Урон по штабу {Us2.Name} равен {Card.Damage} Прочность штаба {Us2.userHQ.Armor}");
                    }
                    else
                    {
                        WriteLog.WriteGameLog($"Игрок {user.Name} использовал событие одиночного урона по карте игрока {Us2.Name}");
                        Us2.CardsMargin[attacked].Armor -= Card.Damage;
                        WriteLog.WriteGameLog($"Урон по карте {Us2.Name} равен {Card.Damage} Прочность карты { Us2.CardsMargin[attacked].NameRobot} c номером {attacked+1} { Us2.CardsMargin[attacked].Armor}");
                        //удаляем карту
                        EffectDeliteCard(user, attacked);

                    }
                    WriteLog.WriteGameLog($"Количестов карт на поле игрока {Us2.Name} равно {Us2.CardsMargin.Count}");
                    //отправляем о сообщение об удачной атаке
                    Us1.Send(Us1.Energy, MsgType.YourEnergy);
                    Us2.Send(Us1.Energy, MsgType.EnemyEnergy);
                    Us1.Send(new int[] { attacking, attacked, Card.Damage }, MsgType.UserDamageEvent);
                    Us2.Send(new int[] { attacking, IDAttacking, attacked, Card.Damage }, MsgType.EnemyDamageEvent);
                    //проверяем на уничтожение одного из штабов
                    TestEndGame();

                }
                else if (user == Us2)
                {
                    if (attacked == -1)
                    {
                        WriteLog.WriteGameLog($"Игрок {user.Name} использовал событие одиночного урона по карте игрока {Us1.Name}");
                        Us1.userHQ.Armor -= Card.Damage;
                        WriteLog.WriteGameLog($"Урон по штабу {Us1.Name} равен {Card.Damage} Прочность штаба {Us1.userHQ.Armor}");
                    }
                    else
                    {
                        WriteLog.WriteGameLog($"Игрок {user.Name} использовал событие одиночного урона по карте игрока {Us1.Name}");
                        Us1.CardsMargin[attacked].Armor -= Card.Damage;
                        WriteLog.WriteGameLog($"Урон по карте {Us1.Name} равен {Card.Damage} Прочность карты { Us1.CardsMargin[attacked].NameRobot} c номером {attacked+1} { Us1.CardsMargin[attacked].Armor}");
                        if (Us1.CardsMargin[attacked].Armor <= 0) Us1.CardsMargin.RemoveAt(attacked);
                    }
                    WriteLog.WriteGameLog($"Количестов карт на поле игрока {Us1.Name} равно {Us1.CardsMargin.Count}");
                    //отправляем о сообщение об удачной атаке
                    Us2.Send(Us2.Energy, MsgType.YourEnergy);
                    Us1.Send(Us2.Energy, MsgType.EnemyEnergy);
                    Us2.Send(new int[] { attacking, attacked, Card.Damage }, MsgType.UserDamageEvent);
                    Us1.Send(new int[] { attacking, IDAttacking, attacked, Card.Damage }, MsgType.EnemyDamageEvent);
                    //проверяем на конец игры
                    TestEndGame();

                }
            }
            catch (Exception e)
            {
                WriteLog.Write(e.ToString());
            }
        }
        //обработка события использовании события восстановления
        private void RepairsEventFunc(User user, int IDRepairs, RepairsEvent Card, int NumberCardRepairs, int Repairable)
        {
            try
            {

              
                if (Repairable == -1)
                {
                    user.userHQ.Armor -= Card.Damage;
                    WriteLog.WriteGameLog($"Игрок {user.Name} использоваел карту восстановления штаба на {Card.Damage} очков. Прочность штаба {user.userHQ.Armor}");
                }
                else
                {
                    user.CardsMargin[Repairable].Armor -= Card.Damage;
                    WriteLog.WriteGameLog($"Игрок {user.Name} использоваел карту восстановления карты на {Card.Damage} очков. Прочность карты {user.CardsMargin[Repairable].Armor}");
                }

                if (user == Us1)
                {
                    //отправляем о сообщение об удачной атаке
                    Us1.Send(Us1.Energy, MsgType.YourEnergy);
                    Us2.Send(Us1.Energy, MsgType.EnemyEnergy);
                    Us1.Send(new int[] { NumberCardRepairs, Repairable, Card.Damage }, MsgType.UsRepairsEvent);
                    Us2.Send(new int[] { NumberCardRepairs, IDRepairs, Repairable, Card.Damage }, MsgType.EnRepairsEvent);
                }
                else if (user == Us2)
                {
                    //отправляем о сообщение об удачной атаке
                    Us2.Send(Us2.Energy, MsgType.YourEnergy);
                    Us1.Send(Us2.Energy, MsgType.EnemyEnergy);
                    Us2.Send(new int[] { NumberCardRepairs, Repairable, Card.Damage }, MsgType.UsRepairsEvent);
                    Us1.Send(new int[] { NumberCardRepairs, IDRepairs, Repairable, Card.Damage }, MsgType.EnRepairsEvent);
                }
            }
            catch (Exception e)
            {
                WriteLog.Write(e.ToString());
            }
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
                switch (Us.CardsMargin[index].ID)
                {//если эта карта ветеран
                    case 3:
                        //уменьшаем бонус к атаке у всех карт
                        foreach (Robot Card in Us.CardsMargin)
                            if (!(Card is Veteran))
                                Card.BonusAttack -= 1;
                        break;
                    case 12:
                       
                        foreach(Robot CardNoDefence in Us.CardsMargin)
                        if (!(CardNoDefence is DefenceConstr))
                        {
                                //если это последний защитник
                                if (CardNoDefence.Defender == Us.CardsMargin[index])
                                {
                                    // обнуляем все ссылки
                                    foreach (Robot Card in Us.CardsMargin)
                                        if (!(Card is DefenceConstr)) Card.Defender = null;

                                    //обнуляем ссылку на защитника штаба
                                    Us.userHQ.Defender = null;
                                }
                                else
                                {
                                    int i = index-1;//берем предыдущую карту
                                    for (; i >= 0; i--)
                                    {
                                        //идем пока не встретили еще одного защитника
                                        if(Us.CardsMargin[i] is DefenceConstr)
                                        {
                                            Us.CardsMargin[i].Defender = null;
                                            break;
                                        }
                                    }
                                }
                                //выходим из цикла
                                break;
                        }
                       
                        break;

                }
                WriteLog.WriteGameLog($"Карта {Us.CardsMargin[index].NameRobot} удалена");
                Us.CardsMargin.RemoveAt(index);
            }
        }
        /// <summary>
        /// Определяет кого будут атаковать
        /// </summary>
        /// <returns></returns>
        private int BeforeDamage(List<Robot> CardsMargin,int attacked)
        {
            int RealAttacked = 0;
                //если на поле есть защитник
                if (CardsMargin[attacked].Defender != null)
                {
                    //ищем последнего защитника
                    DefenceConstr LocalCard = CardsMargin[attacked].Defender;
                    while (LocalCard.Defender != null)
                    {
                        LocalCard = LocalCard.Defender;
                    }

                    //возвращаем индекс защитника
                    RealAttacked = CardsMargin.IndexOf(LocalCard);
                }
                else//если его нет
                {
                    switch (CardsMargin[attacked].ID)
                    {
                        case 2:
                            if (attacked != 0)
                            {
                                //если карта дуэлиста не первая в списке
                                //то атакуем карту слева
                                RealAttacked = attacked - 1;
                            }
                            else if (attacked != CardsMargin.Count - 1)
                            {
                                //если карта не последняя в списке карт находящихся на поле
                                //то атакуем карту находящуюся справа
                                RealAttacked = attacked + 1;
                            }
                            else
                            {
                                //если кроме дуэлиста других карт на поле нет, то атакуем его
                                RealAttacked = attacked;
                            }

                            break;
                        default:
                            RealAttacked = attacked;
                            break;
                    }
                }
                   
            return RealAttacked;
        }
        //для штаба
        private int BeforeDamage(HeadQuarters UserHq, List<Robot> CardsMargin)
        {
            int RealAttacked = -1;
            if (UserHq.Defender != null)
            {
                if (CardsMargin.Count > 0)
                {
                    //ищем последнего защитника
                    DefenceConstr LocalCard = UserHq.Defender;
                    while (LocalCard.Defender != null)
                    {
                        LocalCard = LocalCard.Defender;
                    }
                    //возвращаем индекс защитника
                    RealAttacked = CardsMargin.IndexOf(LocalCard);
                }
                  
            }

            return RealAttacked;
        }
        /// <summary>
        /// Дробит функции обработки атаки на уровне сессии на части
        /// </summary>
        /// <param name="Us1"></param>
        /// <param name="Us2"></param>
        /// <param name="attacking"></param>
        /// <param name="attacked"></param>
        private void ForAttackFunc(User us1,User us2, int attacking, int attacked, int attackCount)
        {
             int damageEnemy = 0, damageUser = 0;
            int defenceCount = 0;
                    if (attacking == -1)
                    {
                        if (attacked == -1)
                        {
                   
                            //урон по противнику
                            damageEnemy = us1.userHQ.Attack;
                           
                             attacked = BeforeDamage(us2.userHQ, us2.CardsMargin);
                        //если штаб находится без защиты
                        if (attacked == -1) us2.userHQ.Armor -= damageEnemy;
                        else
                        {
                            us2.CardsMargin[attacked].Armor -= damageEnemy;
                            EffectDeliteCard(us2, attacked);
                        }//если под защитой защитного сооружения                           
                         WriteLog.WriteGameLog($"{us1.Name} атакует штаб {us2.Name} нанося {damageEnemy} урона. штаб атакуемого имеет прочность {us2.userHQ.Armor}");
                             defenceCount = us2.userHQ.DefenseCount;

                            //урон от противника
                            if (defenceCount > 0)
                            {
                                damageUser = us2.userHQ.Attack;
                                us1.userHQ.Armor -= damageUser;
                        WriteLog.WriteGameLog($"{us2.Name} ответчает на атаку штабом {us1.Name} нанося {damageUser} урона. штаб атакующего имеет прочность {us1.userHQ.Armor}");
                            }
                            
                         }
                        else
                        {
                             defenceCount = us2.CardsMargin[attacked].DefenseCount;
                            if (defenceCount > 0)
                            {
                                //атака по карте пользователя
                                damageUser = us2.CardsMargin[attacked].Attack + us2.CardsMargin[attacked].BonusAttack;
                                us1.userHQ.Armor -= damageUser;
                        WriteLog.WriteGameLog($"{us2.Name} ответчает на атаку штабом{us1.Name} нанося {damageUser} урона. штаб атакующего имеет прочность {us1.userHQ.Armor}");
                            }
                             //урон по карте врага
                            damageEnemy = us1.userHQ.Attack;
                            //определяем кто на самом деле дожен быть атакован
                            attacked = BeforeDamage(us2.CardsMargin, attacked);
                             us2.CardsMargin[attacked].Armor -= damageEnemy;
                       WriteLog.WriteGameLog($"{us1.Name} атакует карту {us2.Name} нанося {damageEnemy} урона. карта {us2.CardsMargin[attacked].NameRobot} на поле атакуемого имеет прочность {us2.CardsMargin[attacked].Armor} и номер {attacked+1}");

                    //если очков прочности меньше 0 удаляем карту
                    EffectDeliteCard(us2, attacked);
                 
                        }
                         
                    }
                    else //если атакует не штаб
                    {
                        if (attacked == -1)
                        {
                        //урон от игрока по врагу
                        damageEnemy = us1.CardsMargin[attacking].Attack + us1.CardsMargin[attacking].BonusAttack;
                        attacked = BeforeDamage(us2.userHQ, us2.CardsMargin);
                        //если штаб находится без защиты
                        if (attacked == -1) us2.userHQ.Armor -= damageEnemy;
                        else
                        {
                            us2.CardsMargin[attacked].Armor -= damageEnemy;
                            EffectDeliteCard(us2, attacked);
                        }//если под защитой защитного сооружения   

                    WriteLog.WriteGameLog($"{us1.Name} атакует штаб {us2.Name} нанося {damageEnemy} урона. штаб атакуемого имеет прочность {us2.userHQ.Armor}");
                             defenceCount = us2.userHQ.DefenseCount;

                            if (defenceCount > 0)
                            {
                                //урон от врага по игроку
                                damageUser = us2.userHQ.Attack;
                                us1.CardsMargin[attacking].Armor -= damageUser;

                        //если очков прочности меньше 0 удаляем карту
                        EffectDeliteCard(us1, attacking);
                        WriteLog.WriteGameLog($"{us2.Name} ответчает на атаку карты {us1.Name} нанося {damageUser} урона. карта {us1.CardsMargin[attacking].NameRobot} на поле атакуещего имеет прочность {us1.CardsMargin[attacking].Armor} и номер {attacking+1}");
                            }
                        }
                        else
                        {
                                //карты атакуют друг друга
                                //урон по карте игрока
                                defenceCount = us2.CardsMargin[attacked].DefenseCount;
                                if (defenceCount > 0)
                                 {
                                   damageUser = us2.CardsMargin[attacked].Attack + us2.CardsMargin[attacked].BonusAttack;
                                   us1.CardsMargin[attacking].Armor -= damageUser;
                                    EffectDeliteCard(us1, attacking);
                        WriteLog.WriteGameLog($"{us2.Name} ответчает на атаку карты {us1.Name} нанося {damageUser} урона. карта {us1.CardsMargin[attacking].NameRobot} на поле атакуещего имеет прочность {us1.CardsMargin[attacking].Armor} и номер {attacking+1}");
                                 }
                                    //урон по карте противника  
                                   damageEnemy = us1.CardsMargin[attacking].Attack + us1.CardsMargin[attacking].BonusAttack;
                                   attacked = BeforeDamage(us2.CardsMargin, attacked);
                                   us2.CardsMargin[attacked].Armor -= damageEnemy;
                    WriteLog.WriteGameLog($"{us1.Name} атакует карту {us2.Name} нанося {damageEnemy} урона. карта {us2.CardsMargin[attacked].NameRobot} на поле атакуемого имеет прочность {us2.CardsMargin[attacked].Armor} и номер {attacked+1}");
                                //если их очки прочности меньше 0, то удаляем их
                               
                                 EffectDeliteCard(us2, attacked);
                           
                                }
                            }

            WriteLog.WriteGameLog($"Количества карт после атаки на поле {us1.Name} равно {us1.CardsMargin.Count}");
            WriteLog.WriteGameLog($"Количества карт после атаки на поле {us2.Name} равно {us2.CardsMargin.Count}");
                     //отправляем о сообщение об удачной атаке
                   us1.Send(new int[] { attacking, attacked, damageUser, damageEnemy, attackCount, defenceCount-1 }, MsgType.MyAttackSucc);
                    us2.Send(new int[] { attacking, attacked, damageUser, damageEnemy, attackCount, defenceCount-1 }, MsgType.EnAttackSucc);//поменял уроны местами из-за ввода универсальной функции AttackSucc
                                                                                                               //проверяем на конец игры
                    TestEndGame();
        }

        /// <summary>
        /// Обрабатывает атаки пользователей на уровне пользователей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="attacking"></param>
        /// <param name="attacked"></param>
        private void AttackFunc(User sender, int attacking, int attacked, int attackCount)
        {
            try
            {
                
                if (sender == Us1)
                {
                    ForAttackFunc(Us1, Us2, attacking, attacked, attackCount);
                }
                else//если пользователь Us2
                {
                    ForAttackFunc(Us2, Us1, attacking, attacked, attackCount);
                }
                //проверяем на уничтожение одного из штабов
                TestEndGame();
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
        /// <summary>
        /// Проверят на признак завершения игры
        /// </summary>
        private void TestEndGame()
        {
            //проверяем на конец игры
            if ((Us1.userHQ.Armor <= 0 && Us2.userHQ.Armor <= 0))
            {
                Us1.Send(MsgType.Draw);
                Us2.Send(MsgType.Draw);
                WriteLog.WriteGameLog("Игра закончлиась ничьей");
                return;
            }
            if (Us1.userHQ.Armor <= 0)
            {
                Us1.Send(MsgType.YouOver);
                Us2.Send(MsgType.YouWin);
                WriteLog.WriteGameLog($"Игрок {Us2.Name} победил");
                return;
            }
            if (Us2.userHQ.Armor <= 0)
            {
                Us2.Send(MsgType.YouOver);
                Us1.Send(MsgType.YouWin);
                WriteLog.WriteGameLog($"Игрок {Us1.Name} победил");
                return;
            }
        }

        /// <summary>
        /// Освобождает все ресурсы сессии
        /// </summary>
        public void Dispose()
        {
            try
            {
                TempProgress.Stop();//останавливаем таймер
                Us1.Dispose();
                Us2.Dispose();

                //очищаем ресурсы сессии
                Us1 = null;
                Us2 = null;
                
                TempProgress.Dispose();
                TempProgress = null;
                SessionEnd(this);
                SessionEnd = null;
                WriteLog.WriteGameLog("Класс игровой сессии освободил все ресурсы");
            }
            catch (Exception e)
            {
                WriteLog.Write(e.ToString());
            }

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
                if (UsProgress.MaxEnergy < 10) UsProgress.MaxEnergy++;
                UsProgress.Energy = UsProgress.MaxEnergy;


                if (UsProgress == Us1)
                {
                    ChangeUser(Us1, Us2, rand);
                }
                else
                {
                    ChangeUser(Us2, Us1, rand);
                }


                //отправляем время начального отсчета
                Us1.Send("3:00", MsgType.ProgressTime);
                Us2.Send("3:00", MsgType.ProgressTime);


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
                SecProgress = TimeProgress;
                TempProgress.Start();
            }
            catch (Exception e) { WriteLog.Write(e.ToString()); }
        }

        private void ChangeUser(User us1,User us2, Random rand)
        {

            WriteLog.WriteGameLog($"Начался ход игрока {us2.Name}");
            //отправляем новые показатели энергии
            us1.Send(us1.MaxEnergy, MsgType.UserMaxEnergy);
            us2.Send(us1.MaxEnergy, MsgType.EnemyMaxEnergy);
            us1.Send(us1.Energy, MsgType.YourEnergy);
            us2.Send(us1.Energy, MsgType.EnemyEnergy);
            //меняем игрока
            UsProgress = us2;
            us1.MyProgress = false;
            us2.MyProgress = true;

            //добавляем  игроку карту
            int Length = us2.CarteUser.Count;//всего карт у игрока
            int IdCarte = us2.CarteUser[rand.Next(0, Length)];
            WriteLog.WriteGameLog($"Игроку {us2.Name} добавлена карта с id {IdCarte}");
            us2.CartsHand.Add(IdCarte);
            us1.Send(MsgType.AddEnemyCarte);
            us2.Send(IdCarte, MsgType.AddUserCarte);
        }
        /// <summary>
        /// Отправляет и получает необходимые данные перед началом игры
        /// </summary>
        /// <param name="stateInfo"></param>
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

                int Length = user.CarteUser.Count;//всего выбрано карт у игрока
                for (int i = 0; i < count; i++)
                {
                    int IdCarte = user.CarteUser[rand.Next(0, Length)];

                    //добавляем id в массив карт, находящихся в руках у игрока
                    user.CartsHand.Add(IdCarte);
                    WriteLog.WriteGameLog($"Игроку {user.Name} добавлена карта с id {IdCarte}");
                    //отправляем сообщения клиентам, о том что надо добавить карту 
                    user.Send(IdCarte, MsgType.AddUserCarte);
                    if (user == Us1) Us2.Send(MsgType.AddEnemyCarte);
                    else Us1.Send(MsgType.AddEnemyCarte);
                }

                //отправляем энергию игрока
                user.Energy = ++user.MaxEnergy;
                user.Send(user.MaxEnergy, MsgType.UserMaxEnergy);
                user.Send(user.Energy, MsgType.YourEnergy);
                WriteLog.WriteGameLog($"Игроку {user.Name} обновлена энргия до { user.Energy}");
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

                //устанавливаем время на 3 минуты
                user.Send("3:00", MsgType.ProgressTime);
                GameLoad++;
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
    }
}
