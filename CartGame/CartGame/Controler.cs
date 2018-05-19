using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace CartGame
{
     public delegate void EmptyDel();//делегат для функций без аргументов
    public delegate void ContrDel(Controler sender);
    public delegate void IntDel(int number);
    public delegate void MsgWithoutData(MsgType type);//делегат для сообщения без данных
    public delegate void MsgWithData(MsgType type, byte[] data);//делегат для сообщения с данными
    public delegate void StringDel(string data);
    public delegate void Attack(int attacking, int attacked, int damageUser,int damageEnemy);
    public delegate void End(MsgType e);
    public delegate void DamageCardAttack(int attacking, int attacked, int damage);
    public delegate void EnDamageCardAttack(int attacking,int IDAttacking, int attacked, int damage);
    public delegate void AllDamage(int number, int damage);
    public delegate void EnAllDamage(int number,int idCard, int damage);
   


    public class Controler: IDisposable
    {

        private SendAndRecMsg dialogWithServ;

        //события клиента
        public event ContrDel SucConnect;//уведомляет о удачном соединении с сервером
        public event ContrDel FailConnect;//уведомляем о неудачном соединении с сервером
        public event EmptyDel StartGame;//начало игры, на сервер запустилась новая сессия
        public event IntDel EnAddCardOnField;//уведомляем, что враг добавил карту на игровое поле
        public event IntDel PaintUserCarte;//в руки игрока добавлена новая карта
        public event IntDel PaintEnemyCarte;//в руки противника добавлена новая карта
        public event AllDamage AddCardsOnField;//собщает о том, что добаление пользователем карты на игровое поле прошло удачно
        public event EmptyDel PaintMyEnergy;//сообщает, что изменилось количество энергии у игрока
        public event EmptyDel PaintEnEnergy;//сообщает, что изменилось количество энергии у противника
        public event StringDel UpdateTime;//уведомляет, что получено новое время обратного отчета времени хода
        public event EmptyDel MyProgress;//уведомляем о том, что начался ход игрока
        public event EmptyDel EnemyProgress;//уведомляем о  том, что начался ход противника
        public event Attack MyAttack;//уведомляем о удачной атаке игрока
        public event Attack EnAttack;//уведомляем о удачной атаке проивника
        public event End EndGame;//сообщаем о конце игры(тип завершения игры)
        public event EmptyDel DeliteSeek;//игрок был удален из из очереди поиска противника
        public event EmptyDel ErrorConnectToServer;//уведомляем о нарушении соединеия с сервером
        public event DamageCardAttack MyAttackDamageCard;//уведомляем о удачной атаке игрока событием одиночного нанесения урона
        public event EnDamageCardAttack EnAttackDamageCard;//уведомляем о удачной атаке противником событием одиночного нанесения урона
        public event DamageCardAttack MyRepairsCard;//уведомляем о удачном использовании игроком события восстановления очков прочности
        public event EnDamageCardAttack EnRepairsCard;//уведомляем о удачном использовании противником события восстановления очков прочности
        public event AllDamage UsAllDamage;//уведомляем о удачном использовании игроком карты с массовым уроном
        public event EnAllDamage EnAllDamage;//уведомляем о удачном использовании противником карты с массовым уроном
        public event StringDel ChatMsg;//уведомляем о получении сообщеия от противника в чате


        private DataGame DataSession;//данные этой игры

        public DataGame GetDataGame
        {
           get { return DataSession; }
        }
   
        public Controler()
        {

           DataSession = new DataGame();
            dialogWithServ = null;

        }
        /// <summary>
        /// Начинаем соединение с сервером
        /// </summary>
        /// <param name="IpAdress"></param>
        /// <param name="Port"></param>
        /// <param name="Name"></param>
        public void Start(IPAddress IpAdress, int Port, string Name)
        {
            //инициализируем класс работы с сервером
            dialogWithServ = new SendAndRecMsg(IpAdress, Port);
            dialogWithServ.RecMsgWithoutData += ProcMsgWithoutData;
            dialogWithServ.RecMsgWithData += ProcMsgWithData;
            DataSession.UsName = Name;
            dialogWithServ.Connect();
            if (dialogWithServ.Connected)
            {
                DataSession.InicializeListCards();
                SucConnect(this);
              
                //подписываемя на получение сообщений
              
                dialogWithServ.ErrorConnectToServer += ErrorConnection;
            }
            else
            {
                dialogWithServ.RecMsgWithoutData -= ProcMsgWithoutData;
                dialogWithServ.RecMsgWithData -= ProcMsgWithData;
                FailConnect(this);
            }


        }
        /// <summary>
        /// Обрабатывает собщения о неудачной отправки собщения серверу
        /// </summary>
        private void ErrorConnection()
        {
            try
            {
                WriteLog.WriteGameLog("Произошла ошибка с со связью с сервером");
                dialogWithServ.ErrorConnectToServer -= ErrorConnection;
                if (MyProgress != null)
                {//если окно не закрыл пользователь
                    ErrorConnectToServer();//предупреждаем форму, что сервер завершил сессию
                                           //освобождаем ресурсы
                    this.Dispose();
                }
            }
            catch(Exception e) {
                WriteLog.Write(e.ToString());
            }
        }
        /// <summary>
        /// Обрабавтываем сообщения без данных
        /// </summary>
        /// <param name="type"></param>
        private void ProcMsgWithoutData(MsgType type)
        {
            try
            {
                switch (type)
                {
                    case MsgType.StartSession:
                        WriteLog.WriteGameLog("Начало игры. Мой ник " + DataSession.UsName);
                        //отправляем только те карты которые не равны 0
                        List<int> temp = new List<int>();
                        int count = DataSession.UserColoda.Length;
                        for (int i = 0; i < count; i++)
                            if (DataSession.UserColoda[i] > 0) temp.Add(DataSession.UserColoda[i]);
                        //отправляем полученный массив
                        dialogWithServ.Send(temp.ToArray(), MsgType.CarteUser);
                        WriteLog.WriteGameLog("Карты игрока отправлены серверу");
                        break;
                    case MsgType.GetName:
                        dialogWithServ.Send(DataSession.UsName, MsgType.GetName);
                        WriteLog.WriteGameLog("Серверу отправлено ник игрока");
                        break;
                    case MsgType.AddEnemyCarte:
                        DataSession.CountCarteEnemy++;
                        WriteLog.WriteGameLog("Противнику добавлена новая карта в руки");
                        PaintEnemyCarte(DataSession.CountCarteEnemy);
                        break;
                    case MsgType.MyProgress:
                        WriteLog.WriteGameLog("Начался ход игрока");
                        MyProgress();
                        break;
                    case MsgType.EnemyProgress:
                        WriteLog.WriteGameLog("Начался ход противника");
                        EnemyProgress();
                        break;
                    case MsgType.YouWin:
                        WriteLog.WriteGameLog("Игра окончилась моей победой");
                        EndGame(MsgType.YouWin);
                        Dispose();
                        break;
                    case MsgType.YouOver:
                        WriteLog.WriteGameLog("Игра окончилась поражение игрока");
                        EndGame(MsgType.YouOver);
                        Dispose();
                        break;
                    case MsgType.Draw:
                        WriteLog.WriteGameLog("Игра закончилась ничьёй");
                        EndGame(MsgType.Draw);
                        Dispose();
                        break;
                    case MsgType.TechnicalVictory:
                        WriteLog.WriteGameLog("Игрок вышел из игры");
                        EndGame(MsgType.TechnicalVictory);
                        Dispose();
                        break;
                    case MsgType.DeliteSeek:
                        WriteLog.WriteGameLog("Игрок удален из очереди ожидания противника");
                        DeliteSeek();
                        break;
                    case MsgType.EnemyNoActiv:
                        WriteLog.WriteGameLog("Противник был долго не активен");
                        EndGame(MsgType.EnemyNoActiv);
                        Dispose();
                        break;
                    case MsgType.YouNoActiv:
                        WriteLog.WriteGameLog("Игрок был долго не активен");
                        EndGame(MsgType.YouNoActiv);
                        Dispose();
                        break;


                }
            }
            catch (Exception e) { WriteLog.Write(e.ToString()); }
        }

        /// <summary>
        /// Особождает все используемые ресурсы
        /// </summary>
        public void Dispose()
        {
             DataSession.Dispose();
            if(dialogWithServ!=null) dialogWithServ.Disconnect();
            SucConnect = null;
            FailConnect =null;
            StartGame = null;
            DeliteSeek = null;
            EnAddCardOnField = null;
            PaintUserCarte = null;
            PaintEnemyCarte = null;
            AddCardsOnField = null;
            PaintMyEnergy = null;
            PaintEnEnergy = null;
            ErrorConnectToServer = null;
            UpdateTime = null;
            MyProgress = null;
            EnemyProgress = null;
            MyAttack = null;
            EnAttack = null;
            EndGame = null;
            EnAttackDamageCard = null;
            MyAttackDamageCard = null;
            MyRepairsCard = null;
            EnRepairsCard = null;
            UsAllDamage = null;
            EnAllDamage = null;

            WriteLog.WriteGameLog("Освобождены все ресурсы Controler");
        }
        /// <summary>
        /// Осуществлеят поиск в колоде карт - ветеран
        /// </summary>
        /// <param name="CardsOnField"></param>
        /// <returns></returns>
        private int SeekVeteran(List<Robot>CardsOnField)
        {
            int ValueVeteran = 0;
            foreach (Robot Card in CardsOnField)
                if (Card is Veteran)
                    ValueVeteran += 1;
            return ValueVeteran;
        }
        /// <summary>
        /// Управляет эффектами, возникающими при добавлении карточки
        /// </summary>
        /// <param name="AddCarte"></param>
        private void EffectAddCardOnField(Robot AddCarte, List<Robot> CarteOnField)
        {     try
            {
                switch (AddCarte.ID)
                {
                    //если эта карта ветеран
                    case 3:
                        //добавляем бонус всем картам кроме карт типа ветеран
                        foreach (Robot Card in CarteOnField)
                            if (!(Card is Veteran))
                                Card.BonusAttack += 1;
                        break;
                    case 9:
                        //добавляем карту в массив карт на поле                       
                       CarteOnField.Add((Robot)Carte.GetCarte(9));
                        //добавляем бонус к карте
                        int count = CarteOnField.Count;
                        int bonus = SeekVeteran(CarteOnField);
                        CarteOnField[count - 1].BonusAttack += bonus;
                        CarteOnField[count - 2].BonusAttack += bonus;
                        break;
                    case 16://если эта карта медик
                        //добавляем бонус всем картам кроме самого медика
                        int CountCards = CarteOnField.Count-1;
                        for (int i = 0; i < CountCards; i++)
                            CarteOnField[i].Armor += 2;
                        //добавляем бонус к карте
                        CarteOnField[CountCards].BonusAttack += SeekVeteran(CarteOnField);
                        break;
                    case 18:
                        //добавляем карту в массив карт на поле                       
                        CarteOnField.Add((Robot)Carte.GetCarte(19));
                        //добавляем бонус к карте
                        int countCards = CarteOnField.Count;
                        int bonusCard = SeekVeteran(CarteOnField);
                        CarteOnField[countCards - 1].BonusAttack += bonusCard;
                        CarteOnField[countCards - 2].BonusAttack += bonusCard;
                        break;
                    default:

                        //если карта не типа ветеран,то считаем сколько у нас карт ветеранов и добавляем
                        //сответствующий размер бонуса
                        
                        CarteOnField[CarteOnField.Count - 1].BonusAttack += SeekVeteran(CarteOnField);
                        break;
                }
            }
            catch (Exception e)
            {
                WriteLog.Write(e.ToString());
            }
        }

        /// <summary>
        /// Выполняет действия при уходе карты с поля
        /// </summary>
        /// <param name="CardOnMargin"></param>
        /// <param name="index"></param>
        private void EffectDeliteCard(List<Robot> CardOnMargin, int index)
        {
            try
            {
                if (CardOnMargin[index].Armor <= 0)
                {
                    switch (CardOnMargin[index].ID)
                    {
                        case 3:
                            //уменьшаем бонус к атаке у всех карт
                            foreach (Robot Card in CardOnMargin)
                                if (!(Card is Veteran))
                                   Card.BonusAttack -= 1;
                            WriteLog.WriteGameLog("Уменьшен бонус для всех карт после удаленя карты Ветеран");
                            break;
                       
                    }
                    //удаляем карту
                    WriteLog.WriteGameLog($"Карта {CardOnMargin[index].NameRobot} удалена");
                    CardOnMargin.RemoveAt(index);
                    
                }
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
        /// <summary>
        /// Обрабатывает удачные атаки
        /// </summary>
        /// <param name="data"></param>
        /// <param name="UserHq"></param>
        /// <param name="EnemyHq"></param>
        /// <param name=""></param>
        public int[] AttackSucc(byte[] data, HeadQuarters UserHq, HeadQuarters EnemyHq, List<Robot> UsCardOnMargin, List<Robot> EnCardOnMargin)
        {
            try
            {
                //преобразуем данные из вида передачи по сети в тип данного компьютера
                short attacking = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                short attacked = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                short damageUser = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));//если это враг, то это урон по врагу
                short damageEnemy = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 6));//если это враг, то это урон по игроку
                short attackCount = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 8));//количество атак у атакующей карты
                short defenderCount = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 10));//количество возможностей ответить на атаку


                if (attacking == -1)
                {
                    if (attacked == -1)
                    {
                        WriteLog.WriteGameLog($"Штаб атаковал штаб. Урон по врагу {damageEnemy} Урон от врага {damageUser}");
                        WriteLog.WriteGameLog($"Прочность атакуещего { UserHq.Armor} и обороняющегося {EnemyHq.Armor} до атаки");
                        EnemyHq.Armor -= damageEnemy;
                        UserHq.Armor -= damageUser;
                        WriteLog.WriteGameLog($"Прочность атакуещего { UserHq.Armor} и обороняющегося {EnemyHq.Armor} после атаки");
                        //обновляем счетчики атак и оборон
                        EnemyHq.DefenseCount = defenderCount;
                        UserHq.AttackCount = attackCount;
                    }
                    else
                    {
                        WriteLog.WriteGameLog($"Штаб атаковал карту противника. Урон по карте врага {EnCardOnMargin[attacked].NameRobot} с номером {attacked} {damageEnemy} Урон от врага {damageUser}");
                        WriteLog.WriteGameLog($"Прочность атакуещего { UserHq.Armor} и обороняющегося { EnCardOnMargin[attacked].Armor} до атаки");
                        EnCardOnMargin[attacked].Armor -= damageEnemy;
                        UserHq.Armor -= damageUser;
                        WriteLog.WriteGameLog($"Прочность атакуещего { UserHq.Armor} и обороняющегося { EnCardOnMargin[attacked].Armor} после атаки");
                        //обновляем счетчики атак и оборон
                        UserHq.AttackCount = attackCount;
                        EnCardOnMargin[attacked].DefenseCount = defenderCount;
                        EffectDeliteCard(EnCardOnMargin, attacked);
                    }
                }
                else
                {
                    if (attacked == -1)
                    {
                        WriteLog.WriteGameLog($"Карта игрока атакует штаб. Урон по карте врага {damageEnemy} Урон от врага по карте игрока {UsCardOnMargin[attacking].NameRobot} с номером {attacking}  {damageUser}");
                        WriteLog.WriteGameLog($"Прочность атакуещего { UsCardOnMargin[attacking].Armor} и обороняющегося { EnemyHq.Armor} до атаки");
                        UsCardOnMargin[attacking].Armor -= damageUser;
                        EnemyHq.Armor -= damageEnemy;
                        WriteLog.WriteGameLog($"Прочность атакуещего { UsCardOnMargin[attacking].Armor} и обороняющегося { EnemyHq.Armor} после атаки");
                        //обновляем счетчики атак и оборон
                        EnemyHq.DefenseCount = defenderCount;
                        UsCardOnMargin[attacking].AttackCount = attackCount;

                        EffectDeliteCard(UsCardOnMargin, attacking);
                        
                    }
                    else
                    {
                        WriteLog.WriteGameLog($"Карта игрока атакует карту противника. Урон по карте врага {EnCardOnMargin[attacked].NameRobot} с номером {attacked} {damageEnemy} Урон от врага по карте игрока {UsCardOnMargin[attacking].NameRobot} с номером {attacking}  {damageUser}");
                        WriteLog.WriteGameLog($"Прочность атакуещего { UsCardOnMargin[attacking].Armor} и обороняющегося { EnCardOnMargin[attacked].Armor } до атаки");
                        UsCardOnMargin[attacking].Armor -= damageUser;
                        EnCardOnMargin[attacked].Armor -= damageEnemy;
                        WriteLog.WriteGameLog($"Прочность атакуещего { UsCardOnMargin[attacking].Armor} и обороняющегося { EnCardOnMargin[attacked].Armor } после атаки");
                        //обновляем счетчики атак и оборон
                        UsCardOnMargin[attacking].AttackCount = attackCount;
                        EnCardOnMargin[attacked].DefenseCount = defenderCount;

                        //удаляем карты у которых очки прочности меньше 0
                        EffectDeliteCard(UsCardOnMargin, attacking);
                        EffectDeliteCard(EnCardOnMargin, attacked);


                    }
                }
                WriteLog.WriteGameLog($"Карт на игровом атакующего {UsCardOnMargin.Count} обороняющегося {EnCardOnMargin.Count}");
                return new int[] { attacking, attacked, damageUser, damageEnemy };
            }
            catch (Exception ex) { WriteLog.Write(ex.ToString()); return new int[4]; }
        }

        /// <summary>
        /// Обрабатывает сообщения с данными
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        private void ProcMsgWithData(MsgType type, byte[] data)
        {
            try
            {
                switch (type)
                {
                    case MsgType.StartGame:
                       
                        DataSession.EnName = Encoding.UTF8.GetString(data);
                        WriteLog.WriteGameLog("Началась подготовка к началу игры. Ник противника " + DataSession.EnName);
                        StartGame();
                        break;

                    case MsgType.AddUserCarte:

                        short IdCarte = BitConverter.ToInt16(data, 0);
                        IdCarte = IPAddress.NetworkToHostOrder(IdCarte);
                        WriteLog.WriteGameLog("Игроку в руки добавлена карта с id: " + IdCarte);
                        //добавляем карты в массив карт пользователя
                        DataSession.CarteFromUser.Add(IdCarte);
                        //перерисовываем все карты у пользователя
                        PaintUserCarte(IdCarte);
                        break;

                    case MsgType.UserMaxEnergy:

                        short MyMaxEnergy = BitConverter.ToInt16(data, 0);
                        MyMaxEnergy = IPAddress.NetworkToHostOrder(MyMaxEnergy);
                        WriteLog.WriteGameLog("У игрока максимальное количество энегрии на этом ходу равно " + MyMaxEnergy);
                        DataSession.MyMaxEnergy = MyMaxEnergy;
                        //оповещяем о измении энергии
                        PaintMyEnergy();
                        break;

                    case MsgType.YourEnergy:
                        short MyEnergy = BitConverter.ToInt16(data, 0);
                        MyEnergy = IPAddress.NetworkToHostOrder(MyEnergy);
                        WriteLog.WriteGameLog("Текущее количство энергии у игрока равно" + MyEnergy);
                        DataSession.MyEnergy = MyEnergy;
                        //оповещяем о измении энергии
                        PaintMyEnergy();
                        break;

                    case MsgType.EnemyMaxEnergy:
                        short EnMaxEnergy = BitConverter.ToInt16(data, 0);
                        EnMaxEnergy = IPAddress.NetworkToHostOrder(EnMaxEnergy);
                        WriteLog.WriteGameLog("У противника максимальное количество энергии установлено до: " + EnMaxEnergy);
                        DataSession.EnMaxEnergy = EnMaxEnergy;
                        //оповещяем о измении энергии
                        PaintEnEnergy();
                        break;

                    case MsgType.EnemyEnergy:
                        short EnEnergy = BitConverter.ToInt16(data, 0);
                        EnEnergy = IPAddress.NetworkToHostOrder(EnEnergy);
                        WriteLog.WriteGameLog("У противника текущее количество энергии равно: " + EnEnergy);
                        DataSession.EnEnergy = EnEnergy;
                        //оповещяем о измении энергии
                        PaintEnEnergy();
                        break;
                    case MsgType.ProgressTime:
                        UpdateTime(Encoding.UTF8.GetString(data));
                        break;

                    case MsgType.AddCarteOnField:
                        short NumberCarte = BitConverter.ToInt16(data, 0);
                        NumberCarte = IPAddress.NetworkToHostOrder(NumberCarte);

                        //удаляем карту из массива id карт на руках у игрока
                        int IdCards = DataSession.CarteFromUser[NumberCarte];

                        DataSession.CarteFromUser.RemoveAt(NumberCarte);
                        
                        //добавляем карту в массив карт на поле
                        Robot AddCarte = (Robot)Carte.GetCarte(IdCards);
                        WriteLog.WriteGameLog($"Игроку добавлена карта { AddCarte.NameRobot} находящаяся у него в руке с номером {NumberCarte} и id {IdCards} ");
                        int indexCardsMargin = DataSession.UsCarteOnField.Count;//позиция добавляемой карты в списке карт на поле
                        DataSession.UsCarteOnField.Add(AddCarte);

                        //выполняем эффект появляющийся при добавлении карты
                        EffectAddCardOnField(AddCarte, DataSession.UsCarteOnField);
                        WriteLog.WriteGameLog("Карт на игровом поле игрока после добавления карты " + DataSession.UsCarteOnField.Count);
                        AddCardsOnField(NumberCarte, indexCardsMargin);
                        break;

                    case MsgType.EnemyAddCarteOnField:
                        short EnIdCarte = BitConverter.ToInt16(data, 0);
                        EnIdCarte = IPAddress.NetworkToHostOrder(EnIdCarte);

                        //добавляем карту в массив карт на поле
                        Robot EnAddCarte = (Robot)Carte.GetCarte(EnIdCarte);
                        WriteLog.WriteGameLog($"Противнику добавлена карта { EnAddCarte.NameRobot} находящаяся у него в руке с id {EnIdCarte} ");
                        int enIndexCardsMargin = DataSession.EnCarteOnField.Count;
                        DataSession.EnCarteOnField.Add(EnAddCarte);
                        //выполняем эффект появлющийся при добавление новой карты на поле
                        EffectAddCardOnField(EnAddCarte, DataSession.EnCarteOnField);
                        WriteLog.WriteGameLog("Карт на игровом поле противника после добавления карты " + DataSession.EnCarteOnField.Count);
                        EnAddCardOnField(enIndexCardsMargin);
                        break;

                    case MsgType.MyAttackSucc:
                        WriteLog.WriteGameLog("Сервер ответил, что пользователь атаковал");
                        int[] returnData = AttackSucc(data, DataSession.UserHQ, DataSession.EnemyHQ, DataSession.UsCarteOnField, DataSession.EnCarteOnField);

                        //оповещаем об атаке
                        MyAttack(returnData[0], returnData[1], returnData[2], returnData[3]);
                        break;
                    case MsgType.EnAttackSucc:
                        WriteLog.WriteGameLog("Сервер ответил, что противник атаковал");
                        int[] ReturnData = AttackSucc(data, DataSession.EnemyHQ, DataSession.UserHQ, DataSession.EnCarteOnField, DataSession.UsCarteOnField);

                        //оповещаем об атаке
                        EnAttack(ReturnData[0], ReturnData[1], ReturnData[2], ReturnData[3]);
                        break;

                    case MsgType.EnemyDamageEvent:
                        WriteLog.WriteGameLog("Сервер ответил, что противник использовал карту одиночного нанесения урона");
                        //конвертируем данные в тип предсавления данных на данном компьютере
                        short EnAttackingCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short IDAttacking = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short EnAttackedDamageEvent = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));
                        short EnDamageEvent = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 6));
                        //уменьшаем очки прочности у атакуемой карты
                        if (EnAttackedDamageEvent == -1)
                        {
                            WriteLog.WriteGameLog($"Противник атаковал штаб Урон картой {EnDamageEvent}");
                            WriteLog.WriteGameLog($"Прочность до атаки {DataSession.UserHQ.Armor}");
                            DataSession.UserHQ.Armor -= EnDamageEvent;
                            WriteLog.WriteGameLog($"Прочность после атаки {DataSession.UserHQ.Armor}");
                        }
                        else
                        {
                            WriteLog.WriteGameLog($"Противник атаковал карту игрока на игровом поле Урон картой {EnDamageEvent} событием Номер карты {DataSession.UsCarteOnField[EnAttackedDamageEvent].NameRobot} {EnAttackedDamageEvent}");
                            WriteLog.WriteGameLog($"Прочность до атаки {DataSession.UsCarteOnField[EnAttackedDamageEvent].Armor}");
                            DataSession.UsCarteOnField[EnAttackedDamageEvent].Armor -= EnDamageEvent;
                            WriteLog.WriteGameLog($"Прочность после атаки {DataSession.UsCarteOnField[EnAttackedDamageEvent].Armor}");
                            EffectDeliteCard(DataSession.UsCarteOnField, EnAttackedDamageEvent);

                        }
                        WriteLog.WriteGameLog("Карт на игровом поле игрока "+ DataSession.UsCarteOnField.Count);

                        //оповещаем об атаке противника
                        EnAttackDamageCard(EnAttackingCard, IDAttacking, EnAttackedDamageEvent, EnDamageEvent);

                        break;

                    case MsgType.UserDamageEvent:
                        
                        //конвертируем данные
                        short AttackingCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short AttackedDamageEvent = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short DamageEvent = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));
                        WriteLog.WriteGameLog($"Сервер ответил, что пользователь использовал карту одиночного нанесения урона c номером в руке {AttackingCard}");
                        //уменьшаем очки прочности у атакуемой карты
                        if (AttackedDamageEvent == -1)
                        {
                            WriteLog.WriteGameLog($"Игрок атаковал штаб Урон картой {DamageEvent}");
                            WriteLog.WriteGameLog($"Прочность до атаки { DataSession.EnemyHQ.Armor}");
                            DataSession.EnemyHQ.Armor -= DamageEvent;
                            WriteLog.WriteGameLog($"Прочность после атаки {DataSession.EnemyHQ.Armor}");
                        }
                        else
                        {
                            WriteLog.WriteGameLog($"Игрок атаковал карту противника на игровом поле Урон картой {DamageEvent} событием Номер карты {DataSession.EnCarteOnField[AttackedDamageEvent].NameRobot}  {AttackedDamageEvent}");
                            WriteLog.WriteGameLog($"Прочность до атаки { DataSession.EnCarteOnField[AttackedDamageEvent].Armor}");
                            DataSession.EnCarteOnField[AttackedDamageEvent].Armor -= DamageEvent;
                            WriteLog.WriteGameLog($"Прочность после атаки { DataSession.EnCarteOnField[AttackedDamageEvent].Armor}");
                            EffectDeliteCard(DataSession.EnCarteOnField, AttackedDamageEvent);
                        }

                        //удаляем карту-событие
                        DataSession.CarteFromUser.RemoveAt(AttackingCard);
                        WriteLog.WriteGameLog("Карт на игровом поле противника " + DataSession.EnCarteOnField.Count);
                        //опопвещаем об событии атаки
                        MyAttackDamageCard(AttackingCard, AttackedDamageEvent, DamageEvent);

                        break;
                    case MsgType.UsRepairsEvent:
                        //конвертируем данные
                        
                        short NumberCardRepairs = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short Repairable = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short Damage = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));
                        WriteLog.WriteGameLog($"Игрок использовал восстанавливающие событие с номером в руке { NumberCardRepairs}");
                        //увеличиваем очки прочности у атакуемой карты
                        if (Repairable == -1)
                        {
                            WriteLog.WriteGameLog($"Игрок добавил прочности штабу Урон картой {Damage}");
                            WriteLog.WriteGameLog($"Прочность до восстановления { DataSession.UserHQ.Armor}");
                            DataSession.UserHQ.Armor -= Damage;
                            WriteLog.WriteGameLog($"Прочность после восстановления { DataSession.UserHQ.Armor}");
                        }
                        else
                        {
                            WriteLog.WriteGameLog($"Игрок добавил прочности своей карте на игровом поле Урон картой {Damage} событием Номер карты {DataSession.UsCarteOnField[Repairable].NameRobot}  {Repairable}");
                            WriteLog.WriteGameLog($"Прочность до восстановления { DataSession.UsCarteOnField[Repairable].Armor}");
                            DataSession.UsCarteOnField[Repairable].Armor -= Damage;
                            WriteLog.WriteGameLog($"Прочность после восстановления { DataSession.UsCarteOnField[Repairable].Armor}");
                        }
                        //удаляем карту-событие
                        DataSession.CarteFromUser.RemoveAt(NumberCardRepairs);

                        //опопвещаем о событии ремонта
                        MyRepairsCard(NumberCardRepairs, Repairable, Damage);
                        break;
                    case MsgType.EnRepairsEvent:
                        //конвертируем данные
                        WriteLog.WriteGameLog("Противник использовал восстанавливающие событие");
                        short EnNumberCardRepairs = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short IDRepairs = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short EnRepairable = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));
                        short EnDamage = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 6));
                        //увеличиваем очки прочности у атакуемой карты
                        if (EnRepairable == -1)
                        {
                            WriteLog.WriteGameLog($"Противник добавил прочности штабу Урон картой {EnDamage}");
                            WriteLog.WriteGameLog($"Прочность до восстановления { DataSession.EnemyHQ.Armor}");
                            DataSession.EnemyHQ.Armor -= EnDamage;
                            WriteLog.WriteGameLog($"Прочность после восстановления {DataSession.EnemyHQ.Armor}");
                        }
                        else
                        {
                            WriteLog.WriteGameLog($"Игрок добавил прочности своей карте на игровом поле Урон картой {EnDamage} событием Номер карты {DataSession.EnCarteOnField[EnRepairable].NameRobot}  {EnRepairable}");
                            WriteLog.WriteGameLog($"Прочность до восстановления { DataSession.EnCarteOnField[EnRepairable].Armor}");
                            DataSession.EnCarteOnField[EnRepairable].Armor -= EnDamage;
                            WriteLog.WriteGameLog($"Прочность после восстановления { DataSession.EnCarteOnField[EnRepairable].Armor}");
                        }

                      
                        //оповещаем об атаке противника
                        EnRepairsCard(EnNumberCardRepairs, IDRepairs, EnRepairable, EnDamage);

                        break;
          
                    case MsgType.UsAllDeliteEvent:
                        
                        short NumberCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short damage = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        WriteLog.WriteGameLog($"Сервер ответил, что игрок использовал событие массвого урона c номером в руке {NumberCard} и уроном {damage}");
                        //уменьшаем очки прочности у удаляемых карт
                        int count = DataSession.EnCarteOnField.Count;
                        for (int i = 0; i < count; i++)
                        {
                            WriteLog.WriteGameLog($"Очки прочности у атакуемой карты {i} { DataSession.EnCarteOnField[i].NameRobot} до атаки {DataSession.EnCarteOnField[i].Armor}");
                            DataSession.EnCarteOnField[i].Armor -= damage;
                            WriteLog.WriteGameLog($"Очки прочности у атакуемой карты {i} после атаки {DataSession.EnCarteOnField[i].Armor}");
                        }

                        //удаляем карты с HP меньше 0
                        for (int i = 0; i < count; i++)
                        {
                            EffectDeliteCard(DataSession.EnCarteOnField, i);
                            if (count > DataSession.EnCarteOnField.Count)
                            {
                                count--;
                                i--;
                            }
                        }

                        //удаляем карту-событие
                        DataSession.CarteFromUser.RemoveAt(NumberCard);
                        WriteLog.WriteGameLog("Карт на игровом поле противника " + DataSession.EnCarteOnField.Count);
                        //опопвещаем об атаке
                        UsAllDamage(NumberCard, damage);
                        break;
                    case MsgType.EnAllDeliteEvent:
                        short IDCards = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short NumberEnCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short TotalDamage = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));
                        WriteLog.WriteGameLog($"Сервер ответил, что противник использовал событие массвого урона c уроном {TotalDamage}");
                        //уменьшаем очки прочности у удаляемых карт
                        int Count = DataSession.UsCarteOnField.Count;
                        for (int i = 0; i < Count; i++)
                        {
                            WriteLog.WriteGameLog($"Очки прочности у атакуемой карты {i} { DataSession.UsCarteOnField[i].NameRobot} до атаки {DataSession.UsCarteOnField[i].Armor}");
                            DataSession.UsCarteOnField[i].Armor -= TotalDamage;
                            WriteLog.WriteGameLog($"Очки прочности у атакуемой карты {i} после атаки {DataSession.UsCarteOnField[i].Armor}");
                        }

                        //удаляем карты с HP меньше 0
                        for (int i = 0; i < Count; i++)
                        {
                            EffectDeliteCard(DataSession.UsCarteOnField, i);
                            if (Count > DataSession.UsCarteOnField.Count)
                            {
                                Count--;
                                i--;
                            }
                        }
                        WriteLog.WriteGameLog("Карт на игровом поле игрока " + DataSession.UsCarteOnField.Count);
                        //опопвещаем об атаке
                        EnAllDamage(NumberEnCard, IDCards, TotalDamage);
                        break;
                    case MsgType.ChatMsg:
                        //получения сообщения для чата
                        ChatMsg(Encoding.UTF8.GetString(data));
                        break;
                }
            }
            catch (Exception e)
            {
                WriteLog.Write(e.ToString());
            }
            
        }
        //свойство получения экземпляра класса ответчающего за отправку и первичную обработку полученных данных
        public SendAndRecMsg DialogWithServ
        {
            get { return dialogWithServ; }
        }
       
    }
   public class SendAndRecMsg
    {
        TcpClient Client;
        IPEndPoint EndPoint;//конечная точка, указывающая на сервер
        NetworkStream ClientNetwork;//поток собщения клиента и пользователя
        private Thread ThProcesMsg;//поток обрабатыающий приходящие от сервера данные
        private bool StopThread;//если false поток останавливается
        
        //события получения сообщения
        public event MsgWithoutData RecMsgWithoutData;//уведомляем контролер о том, что пришло сообщение без данных
        public event MsgWithData RecMsgWithData;//уведомляем контроллер  о том, что пришло сообщение с данными
        public event EmptyDel ErrorConnectToServer;//сообщает контроллер об ошибки отправки сообщения серверу
        public bool Connected
        {
            get { return Client.Connected;}
        }

        public SendAndRecMsg(IPAddress IpAdress, int Port)
        {
            EndPoint = new IPEndPoint(IpAdress, Port);
            Client = new TcpClient(); 
        }

        /// <summary>
        /// Соединяемся с сервером
        /// </summary>
        public void Connect()
        {
            try
            {
                if (!Client.Connected)
                {
                    Client.Connect(EndPoint);
                    ClientNetwork = Client.GetStream();
                    StopThread = true;
                    ThProcesMsg = new Thread(ProcessMsg);
                    ThProcesMsg.Start();
                }
            }
            catch (SocketException e)
            {
                Client.Close();
                EndPoint = null;
                throw e;
            }
            catch (Exception e)
            {
                WriteLog.Write(e.ToString());
            }
        }

            private void ProcessMsg()
            {
            try
            {
                while (StopThread && Client.Connected)
                {
                    byte[] caption = new byte[3];
                    int CaptionBytes = RecData(caption, caption.Length, ClientNetwork);
                    MsgType msgType;
                    if (CaptionBytes > 0)
                    {
                        msgType = (MsgType)caption[0];
                        short MsgLength = BitConverter.ToInt16(caption, 1);
                        MsgLength = IPAddress.NetworkToHostOrder(MsgLength);
                        if (MsgLength == 0)
                        {
                            RecMsgWithoutData(msgType);
                            Debug.WriteLine(msgType.ToString());
                        }
                        else
                        {
                            byte[] Data = new byte[MsgLength];
                            int ReadData = RecData(Data, MsgLength, ClientNetwork);
                            if (ReadData > 0)
                            {
                                RecMsgWithData(msgType, Data);
                            }
                        }
                    }
                    
                }
            }
            
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
        private int RecData(byte[] data, int length, NetworkStream stream)
        {
            try
            {
                int ReadBytes = 0;
                while (ReadBytes != length)
                {
                    if (stream != null)
                    {
                        int readed = stream.Read(data, 0, length - ReadBytes);
                        ReadBytes += readed;
                        if (readed == 0) return 0;
                    }
                }
                return ReadBytes;
            }
            catch (IOException e)
            {
                if (Client != null)
                {
                    ErrorConnectToServer();//уведомляем контроллер о потери связи с сервером
                    MessageBox.Show("Связь с сервером потеряна!");
                }
                return 0;
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); return 0; }
        }

        /// <summary>
        /// Обрываем связь с сервером и осовбожаем все ресурсы
        /// </summary>
        public void Disconnect()
        { 

               StopThread = false;
            RecMsgWithoutData = null;
            RecMsgWithData = null;
            ErrorConnectToServer = null;
            if (Client != null)
            {
                Client.Close();
                Client = null;
            }
              ClientNetwork = null;
            WriteLog.WriteGameLog("Освобожены все ресурсы SendAndRecMsg");
        }
        //методы отправки данных
        public void Send(MsgType TypeMsg)
        {
            try
            {
                short length = 0;
                length = IPAddress.HostToNetworkOrder(length);
                if(ClientNetwork!=null)
                ClientNetwork.Write(Summ((byte)TypeMsg, BitConverter.GetBytes(length)), 0, 3);


            }
            //временно
            catch (IOException)
            {
                ErrorConnectToServer();
                MessageBox.Show("Связь с сервером потеряна!");
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }

        public void Send(int[] IDCarte, MsgType TypeMsg)
        {
            try
            {
             
                int Length = IDCarte.Length;
                if (Length < 500)
                {
                    int length = Length * 2;
                    short leng = IPAddress.HostToNetworkOrder((short)length);

                    byte[] data = new byte[length];
                    int index = 0;
                    for (int i = 0; i < Length; i++)
                    {
                        byte[] temp;
                        temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)IDCarte[i]));
                        data[index] = temp[0];
                        data[index + 1] = temp[1];
                        index += 2;
                    }
                    ClientNetwork.Write(Summ((byte)TypeMsg, BitConverter.GetBytes(leng)), 0, 3);
                    ClientNetwork.Write(data, 0, length);

                }

             }
           
            catch (IOException e)
            {
                ErrorConnectToServer();
                MessageBox.Show("Связь с сервером потеряна!");
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
        public void Send(string message, MsgType TypeMsg)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                int length = data.Length;
                short leng = IPAddress.HostToNetworkOrder((short)length);


                //Отправляем
                ClientNetwork.Write(Summ((byte)TypeMsg, BitConverter.GetBytes(leng)), 0, 3);
                ClientNetwork.Write(data, 0, data.Length);
            }
            //временно
            catch (IOException e)
            {
                ErrorConnectToServer();
                MessageBox.Show("Связь с сервером потеряна!");
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
        public void Send(int number, MsgType TypeMsg)
        {
            try
            {
                int Length = 2;
                short leng = IPAddress.HostToNetworkOrder((short)Length);
                short Value = IPAddress.HostToNetworkOrder((short)number);
                byte[] head = Summ((byte)TypeMsg, BitConverter.GetBytes(leng));

                ClientNetwork.Write(SummNumber(head, BitConverter.GetBytes(Value)), 0, 5);

            }
            //временно
            catch (IOException e)
            {
                ErrorConnectToServer();
                MessageBox.Show("Связь с сервером потеряна!");
            }
            catch (Exception e)
            { WriteLog.Write(e.ToString()); }
        }
        /// <summary>
        /// Объединяем два массива байтов в один
        /// </summary>
        /// <param name="Head"></param>
        /// <param name="Number"></param>
        /// <returns></returns>
        private byte[] SummNumber(byte[] Head, byte[] Number)
        {
            byte[] temp = new byte[Head.Length + Number.Length];
            int i = 0;
            for (; i < Head.Length; i++)
                temp[i] = Head[i];
            for (int j = 0; j < Number.Length; j++)
                temp[i + j] = Number[j];
            return temp;
        }
        //соединям тип сообщения и длинну сообщения в один массив
        private byte[] Summ(byte type, byte[] len)
        {
            byte[] ret = new byte[len.Length + 1];
            ret[0] = type;
            ret[1] = len[0];
            ret[2] = len[1];
            return ret;
        }
    }
}


