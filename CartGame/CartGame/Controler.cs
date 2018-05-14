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
        public event ContrDel SucConnect;
        public event ContrDel FailConnect;
        public event EmptyDel StartGame;
        public event EmptyDel EnAddCardOnField;
        public event IntDel PaintUserCarte;
        public event IntDel PaintEnemyCarte;
        public event IntDel AddCardsOnField;
        public event EmptyDel PaintMyEnergy;
        public event EmptyDel PaintEnEnergy;
        public event StringDel UpdateTime;
        public event EmptyDel MyProgress;
        public event EmptyDel EnemyProgress;
        public event Attack MyAttack;
        public event Attack EnAttack;
        public event End EndGame;
        public event EmptyDel DeliteSeek;
        public event EmptyDel ErrorConnectToServer;
        public event DamageCardAttack MyAttackDamageCard;
        public event EnDamageCardAttack EnAttackDamageCard;
        public event DamageCardAttack MyRepairsCard;
        public event EnDamageCardAttack EnRepairsCard;
        public event AllDamage UsAllDamage;
        public event EnAllDamage EnAllDamage;
        public event StringDel ChatMsg;


        private DataGame DataSession;

        public DataGame GetDataGame
        {
           get { return DataSession; }
        }
   
        public Controler()
        {

           DataSession = new DataGame();
            dialogWithServ = null;

        }
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
        private void ErrorConnection()
        {
            try
            {
                dialogWithServ.ErrorConnectToServer -= ErrorConnection;
                if (MyProgress != null)
                {//если окно не закрыл пользователь
                    ErrorConnectToServer();//предупреждаем форму, что сервер завершил сессию
                                           //освобождаем ресурсы
                    this.Dispose();
                }
            }
            catch(Exception e) {
                MessageBox.Show(e.ToString());
            }
        }
        private void ProcMsgWithoutData(MsgType type)
        {
           
            switch (type)
            {
                case MsgType.StartSession:
                    //отправляем только те карты которые не равны 0
                    List<int> temp = new List<int>();
                    int count = DataSession.UserColoda.Length;
                    for (int i = 0; i < count; i++)
                        if (DataSession.UserColoda[i] > 0) temp.Add(DataSession.UserColoda[i]); 
                    //отправляем полученный массив
                    dialogWithServ.Send(temp.ToArray(), MsgType.CarteUser);
                    break;
                case MsgType.GetName:
                    dialogWithServ.Send(DataSession.UsName, MsgType.GetName);
                    break;
                case MsgType.AddEnemyCarte:
                    DataSession.CountCarteEnemy++;
                    PaintEnemyCarte(DataSession.CountCarteEnemy);
                    break;
                case MsgType.MyProgress:
                    MyProgress();
                    break;
                case MsgType.EnemyProgress:
                    EnemyProgress();
                    break;
                case MsgType.YouWin:
                    EndGame(MsgType.YouWin);
                    Dispose();
                    break;
                case MsgType.YouOver:
                    EndGame(MsgType.YouOver);
                    Dispose();
                    break;
                case MsgType.Draw:
                    EndGame(MsgType.Draw);
                    Dispose();
                    break;
                case MsgType.TechnicalVictory:
                    EndGame(MsgType.TechnicalVictory);
                    Dispose();
                    break;
                case MsgType.DeliteSeek:
                    DeliteSeek();
                    break;
                case MsgType.EnemyNoActiv:
                    EndGame(MsgType.EnemyNoActiv);
                    Dispose();
                    break;
                case MsgType.YouNoActiv:
                    EndGame(MsgType.YouNoActiv);
                    Dispose();
                    break;


            }
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


       }

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
                    default:

                        //если карта не типа ветеран,то считаем сколько у нас карт ветеранов и добавляем
                        //сответствующий размер бонуса
                        
                        CarteOnField[CarteOnField.Count - 1].BonusAttack += SeekVeteran(CarteOnField);
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
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
                            break;
                       
                    }
                    //удаляем карту
                    CardOnMargin.RemoveAt(index);
                    
                }
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); }
        }
        /// <summary>
        /// Выполняется после того как получены действия об атаке
        /// </summary>
        /// <param name="data"></param>
        /// <param name="UserHq"></param>
        /// <param name="EnemyHq"></param>
        /// <param name=""></param>
        public int[] AttackSucc(byte[] data, HeadQuarters UserHq, HeadQuarters EnemyHq, List<Robot> UsCardOnMargin, List<Robot> EnCardOnMargin)
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
                    EnemyHq.Armor -= damageEnemy;
                    UserHq.Armor -= damageUser;
                    //обновляем счетчики атак и оборон
                    EnemyHq.DefenseCount = defenderCount;
                    UserHq.AttackCount = attackCount;
                }
                else
                {
                    EnCardOnMargin[attacked].Armor -= damageEnemy;
                    UserHq.Armor -= damageUser;
                    EffectDeliteCard(EnCardOnMargin, attacked);
                    //обновляем счетчики атак и оборон
                    UserHq.AttackCount = attackCount;
                    EnCardOnMargin[attacked].DefenseCount = defenderCount;

                }
            }
            else
            {
                if (attacked == -1)
                {
                    UsCardOnMargin[attacking].Armor -= damageUser;
                    EnemyHq.Armor -= damageEnemy;
                    EffectDeliteCard(UsCardOnMargin, attacking);
                    //обновляем счетчики атак и оборон
                    EnemyHq.DefenseCount = defenderCount;
                    UsCardOnMargin[attacking].AttackCount = attackCount;

                }
                else
                {
                    UsCardOnMargin[attacking].Armor -= damageUser;
                    EnCardOnMargin[attacked].Armor -= damageEnemy;
                    //обновляем счетчики атак и оборон
                    UsCardOnMargin[attacking].AttackCount = attackCount;
                    EnCardOnMargin[attacked].DefenseCount = defenderCount;

                    //удаляем карты у которых очки прочности меньше 0
                    EffectDeliteCard(UsCardOnMargin, attacking);
                    EffectDeliteCard(EnCardOnMargin, attacked);


                }
            }
          
            return new int[] { attacking, attacked, damageUser, damageEnemy };
        }
        private void ProcMsgWithData(MsgType type, byte[] data)
        {
            try
            {
                switch (type)
                {
                    case MsgType.StartGame:

                        DataSession.EnName = Encoding.UTF8.GetString(data);
                        StartGame();
                        break;

                    case MsgType.AddUserCarte:

                        short IdCarte = BitConverter.ToInt16(data, 0);
                        IdCarte = IPAddress.NetworkToHostOrder(IdCarte);
                        //добавляем карты в массив карт пользователя
                        DataSession.CarteFromUser.Add(IdCarte);
                        //перерисовываем все карты у пользователя
                        PaintUserCarte(IdCarte);
                        break;

                    case MsgType.UserMaxEnergy:

                        short MyMaxEnergy = BitConverter.ToInt16(data, 0);
                        MyMaxEnergy = IPAddress.NetworkToHostOrder(MyMaxEnergy);
                        DataSession.MyMaxEnergy = MyMaxEnergy;
                        //оповещяем о измении энергии
                        PaintMyEnergy();
                        break;

                    case MsgType.YourEnergy:
                        short MyEnergy = BitConverter.ToInt16(data, 0);
                        MyEnergy = IPAddress.NetworkToHostOrder(MyEnergy);
                        DataSession.MyEnergy = MyEnergy;
                        //оповещяем о измении энергии
                        PaintMyEnergy();
                        break;

                    case MsgType.EnemyMaxEnergy:
                        short EnMaxEnergy = BitConverter.ToInt16(data, 0);
                        EnMaxEnergy = IPAddress.NetworkToHostOrder(EnMaxEnergy);
                        DataSession.EnMaxEnergy = EnMaxEnergy;
                        //оповещяем о измении энергии
                        PaintEnEnergy();
                        break;

                    case MsgType.EnemyEnergy:
                        short EnEnergy = BitConverter.ToInt16(data, 0);
                        EnEnergy = IPAddress.NetworkToHostOrder(EnEnergy);
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
                        DataSession.UsCarteOnField.Add(AddCarte);

                        //выполняем эффект появляющийся при добавлении карты
                        EffectAddCardOnField(AddCarte, DataSession.UsCarteOnField);

                        AddCardsOnField(NumberCarte);
                        break;

                    case MsgType.EnemyAddCarteOnField:
                        short EnIdCarte = BitConverter.ToInt16(data, 0);
                        EnIdCarte = IPAddress.NetworkToHostOrder(EnIdCarte);

                        //добавляем карту в массив карт на поле
                        Robot EnAddCarte = (Robot)Carte.GetCarte(EnIdCarte);
                        DataSession.EnCarteOnField.Add(EnAddCarte);
                        //выполняем эффект появлющийся при добавление новой карты на поле
                        EffectAddCardOnField(EnAddCarte, DataSession.EnCarteOnField);
                        EnAddCardOnField();
                        break;

                    case MsgType.MyAttackSucc:
                        int[] returnData = AttackSucc(data, DataSession.UserHQ, DataSession.EnemyHQ, DataSession.UsCarteOnField, DataSession.EnCarteOnField);

                        //оповещаем об атаке
                        MyAttack(returnData[0], returnData[1], returnData[2], returnData[3]);
                        break;
                    case MsgType.EnAttackSucc:
                        int[] ReturnData = AttackSucc(data, DataSession.EnemyHQ, DataSession.UserHQ, DataSession.EnCarteOnField, DataSession.UsCarteOnField);

                        //оповещаем об атаке
                        EnAttack(ReturnData[0], ReturnData[1], ReturnData[2], ReturnData[3]);
                        break;

                    case MsgType.EnemyDamageEvent:
                        //конвертируем данные в тип предсавления данных на данном компьютере
                        short EnAttackingCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short IDAttacking = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short EnAttackedDamageEvent = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));
                        short EnDamageEvent = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 6));
                        //уменьшаем очки прочности у атакуемой карты
                        if (EnAttackedDamageEvent == -1)
                        {
                            DataSession.UserHQ.Armor -= EnDamageEvent;
                        }
                        else
                        {
                            DataSession.UsCarteOnField[EnAttackedDamageEvent].Armor -= EnDamageEvent;
                            EffectDeliteCard(DataSession.UsCarteOnField, EnAttackedDamageEvent);
                        }


                        //оповещаем об атаке противника
                        EnAttackDamageCard(EnAttackingCard, IDAttacking, EnAttackedDamageEvent, EnDamageEvent);

                        break;

                    case MsgType.UserDamageEvent:
                        //конвертируем данные
                        short AttackingCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short AttackedDamageEvent = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short DamageEvent = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));

                        //уменьшаем очки прочности у атакуемой карты
                        if (AttackedDamageEvent == -1)
                        {
                            DataSession.EnemyHQ.Armor -= DamageEvent;
                        }
                        else
                        {
                            DataSession.EnCarteOnField[AttackedDamageEvent].Armor -= DamageEvent;
                            EffectDeliteCard(DataSession.EnCarteOnField, AttackedDamageEvent);
                        }

                        //удаляем карту-событие
                        DataSession.CarteFromUser.RemoveAt(AttackingCard);

                        //опопвещаем об событии атаки
                        MyAttackDamageCard(AttackingCard, AttackedDamageEvent, DamageEvent);

                        break;
                    case MsgType.UsRepairsEvent:
                        //конвертируем данные
                        short NumberCardRepairs = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short Repairable = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short Damage = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));

                        //увеличиваем очки прочности у атакуемой карты
                        if (Repairable == -1)
                            DataSession.UserHQ.Armor -= Damage;
                        else
                            DataSession.UsCarteOnField[Repairable].Armor -= Damage;
                        //удаляем карту-событие
                        DataSession.CarteFromUser.RemoveAt(NumberCardRepairs);

                        //опопвещаем о событии ремонта
                        MyRepairsCard(NumberCardRepairs, Repairable, Damage);
                        break;
                    case MsgType.EnRepairsEvent:
                        //конвертируем данные
                        short EnNumberCardRepairs = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short IDRepairs = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short EnRepairable = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));
                        short EnDamage = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 6));
                        //увеличиваем очки прочности у атакуемой карты
                        if (EnRepairable == -1)
                            DataSession.EnemyHQ.Armor -= EnDamage;
                        else
                            DataSession.EnCarteOnField[EnRepairable].Armor -= EnDamage;


                        //оповещаем об атаке противника
                        EnRepairsCard(EnNumberCardRepairs, IDRepairs, EnRepairable, EnDamage);

                        break;
          
                    case MsgType.UsAllDeliteEvent:
                        short NumberCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short damage = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        //уменьшаем очки прочности у удаляемых карт
                        int count = DataSession.EnCarteOnField.Count;
                        for (int i = 0; i < count; i++)
                            DataSession.EnCarteOnField[i].Armor -= damage;

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

                        //опопвещаем об атаке
                        UsAllDamage(NumberCard, damage);
                        break;
                    case MsgType.EnAllDeliteEvent:
                        short IDCards = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 0));
                        short NumberEnCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2));
                        short TotalDamage = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));

                        //уменьшаем очки прочности у удаляемых карт
                        int Count = DataSession.UsCarteOnField.Count;
                        for (int i = 0; i < Count; i++)
                            DataSession.UsCarteOnField[i].Armor -= TotalDamage;
                            

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
                MessageBox.Show(e.ToString());
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
        IPEndPoint EndPoint;
        NetworkStream ClientNetwork;
        private Thread ThProcesMsg;
        private bool StopThread; 
        
        //события получения сообщения
        public event MsgWithoutData RecMsgWithoutData;
        public event MsgWithData RecMsgWithData;
        public event EmptyDel ErrorConnectToServer;
        public bool Connected
        {
            get { return Client.Connected;}
        }

        public SendAndRecMsg(IPAddress IpAdress, int Port)
        {
            EndPoint = new IPEndPoint(IpAdress, Port);
            Client = new TcpClient();
  
        }

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
            catch (SocketException e) {
                Client.Close();
                EndPoint = null;
                throw e;
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
            { MessageBox.Show(e.ToString()); }
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
                    ErrorConnectToServer();
                    MessageBox.Show("Связь с сервером потеряна!");
                }
                return 0;
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); return 0; }
        }

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

    }
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
            { MessageBox.Show(e.ToString()); }
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
            //временно
            catch (IOException e)
            {
                ErrorConnectToServer();
                MessageBox.Show("Связь с сервером потеряна!");
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); }
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
            { MessageBox.Show(e.ToString()); }
        }
        public void Send(int number, MsgType TypeMsg)
        {
            try
            {
                int Length = 2;
                short leng = IPAddress.HostToNetworkOrder((short)Length);
                short Value = IPAddress.HostToNetworkOrder((short)number);
               // Debug.Write("Отправлено число " + Value);
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
            { MessageBox.Show(e.ToString()); }
        }
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


