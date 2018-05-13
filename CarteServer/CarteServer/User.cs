using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace CarteServer
{
    delegate void EmptyDel();//делегат для функций без аргументов
    delegate void CardOnField(User sender,int numberCard, int idCards);
    delegate void AttackDelegate(User sender, int attacking, int attacked, int attackCount);
    delegate void ErrorSend(User user);
    delegate void DamageCard(User user,int IDAttacking, DamageEvent Card,int attacking, int attacked);
    delegate void RepairsCard(User user, int IDAttacking, RepairsEvent Card, int attacking, int attacked);
    delegate void AllDamageCard(User user, int IDAttacking, AllDamageEvent Card, int attacking);

    class User
    {
        private string name = null;
        public string Name
        {
            get { return name; }
            set { if (name == null) name = value; }
        }
        int idUser;//уникальный идентификатор пользователя
        public int IdUser
        {
            get { return idUser;}
        }
        private TcpClient UserTcpClient;
        private Thread UserThread;//поток обрабатывающий сообщиеня от клиента
        private NetworkStream TcpStream;
        private bool myProgress;
        private bool StopThread;
        public event AttackDelegate Attack;
        public event DamageCard DamageCardEvent;
        public event ErrorSend FailedSendMsg;
        public event EmptyDel GetOutOfQueue;
        public event RepairsCard RepairsCardEvent;
        public event AllDamageCard AllDamageEvent;
        public bool MyProgress
        {
            set { myProgress = value; }
        }

        private int maxEnergy;//максимальная энергия за этот ход
        public int MaxEnergy
        {
            get {return  maxEnergy; }
            set { maxEnergy = value; }
        }

        private int energy;//энергия находящаяся в распоряжении у игрока
        public int Energy
        {
            get { return energy; }
            set { energy = value; }
        }
        List<int> carteUser;//id карт игрока
        public List<int> CarteUser
        {
            get { return carteUser; }
        }

        private List<int> CardsOnHands; //карты находящиеся в руке у игрока
        public List<int> CartsHand
        {
            get { return CardsOnHands; }
           // set { CardsOnHands = value; }
        }
        private bool userNoProgress;
        public bool UserNoProgress
        {
            get { return userNoProgress; }
            set { userNoProgress = value; }
        }
        private List<Robot> CardsOnMargin;//карты на поле
        public List<Robot> CardsMargin
        {
            get { return CardsOnMargin; }
           // set { CardsOnMargin = value; }
        }
        private HeadQuarters userHq;//штаб игрока
        public HeadQuarters userHQ
        {
            get { return userHq; }
        }
       
        public event EmptyDel ColodRec;
        public event EmptyDel NameRec;
        public event EmptyDel EndProgress;
        public event CardOnField AddCardField;
       
        public User()
        {
            UserTcpClient = null;

        }

        public User(TcpClient client)
        {
            UserTcpClient = client;
            TcpStream = UserTcpClient.GetStream();

             //запускаем поток обработки сообшений
            UserThread = new Thread(RecMessage);
            UserThread.IsBackground = true;
            UserThread.Start();
            StopThread = true;
            userNoProgress = false;
        }
        //обрабатывает сообшения пользователя
        public void RecMessage()
        {
            try
            {
                while (StopThread && UserTcpClient.Connected)
                {

                    
                    byte[] caption = new byte[3];
                    int CaptionBytes = RecData(caption, caption.Length, TcpStream);
                    MsgType? msgType = null;
                    if (CaptionBytes > 0)
                    {
                        msgType = (MsgType)caption[0];
                        short MsgLength = BitConverter.ToInt16(caption, 1);
                        MsgLength = IPAddress.NetworkToHostOrder(MsgLength);//не работает
                        if (MsgLength > 0)
                        {
                            byte[] Data = new byte[MsgLength];
                            int ReadData = RecData(Data, MsgLength, TcpStream);
                            if (ReadData > 0)
                            {
                                //отделить swicth от основной функции
                                switch (msgType)
                                {
                                    case MsgType.CarteUser:
                                        if (MsgLength % 2 == 0)
                                        {
                                          
                                            int index = 0;
                                            for (int i = 0; i < MsgLength/2; i++)
                                            {
                                                carteUser.Add(IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, index)));

                                                index += 2;
                                            }

                                            ColodRec();
                                        }
                                        break;

                                    case MsgType.GetName:
                                        name = Encoding.UTF8.GetString(Data);
                                        Debug.Write(name);
                                        //случайным образом определяем id пользователя
                                        Random id = new Random();
                                        idUser = id.Next(0, int.MaxValue);
                                        NameRec();
                                        break;

                                    case MsgType.AddCarteOnField:
                                        if (myProgress)
                                        {
                                            int NumberCarte = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 0));

                                            //получаем ID карты
                                            int IDCarte = CardsOnHands[NumberCarte];
                                            //получаем экземпляр этой карты
                                            Carte NewCarte = Carte.GetCarte(IDCarte);
                                            if (NewCarte is Robot)
                                            {
                                                Robot NewCarteRobot = (Robot)NewCarte;
                                                //сравниваем цену с кол-во энергии
                                                if (NewCarteRobot.ValueEnergy <= energy)
                                                {
                                                    energy -= NewCarteRobot.ValueEnergy;
                                                    CardsOnHands.RemoveAt(NumberCarte);
                                                    CardsOnMargin.Add(NewCarteRobot);
                                                    //уведомляем об добавление карты класс сессии
                                                    AddCardField(this, NumberCarte, IDCarte);
                                                }
                                            }
                                        }
                                        break;
                                    case MsgType.Attack:
                                        if (myProgress)//если мой ход
                                        {
                                            short attacking = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 0));
                                            short attacked = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 2));
                                            if (attacking == -1)
                                            {
                                                int attackCount = userHq.AttackCount;
                                                if (attackCount > 0)
                                                    Attack(this, -1, attacked, attackCount-1);


                                            }
                                            else
                                            {
                                                int attackCount = CardsOnMargin[attacking].AttackCount;
                                                if (attackCount > 0)
                                                    Attack(this, attacking, attacked, attackCount-1);
                                            }
                                        }
                                        break;
                                    case MsgType.DamageEvent:
                                        if (myProgress)
                                        {
                                            short attacking = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 0));//номер карты  в руке у игрока
                                            short attacked = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 2));
                                            int IdAttackingCard = CardsOnHands[attacking];
                                            DamageEvent Card = Carte.GetCarte(IdAttackingCard) as DamageEvent;
                                            if (Card.ValueEnergy <= energy)
                                            {
                                                energy -= Card.ValueEnergy;
                                                CardsOnHands.RemoveAt(attacking);
                                                DamageCardEvent(this, IdAttackingCard, Card,attacking, attacked);
                                            }
                                           
                                        }
                                
                                        break;
                                    case MsgType.RepairsEvent:
                                        if (myProgress)
                                        {
                                            short NumberCardRepairs = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 0));//номер карты  в руке у игрока
                                            short Repairable = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 2));
                                            int IdRepairsCard = CardsOnHands[NumberCardRepairs];
                                            RepairsEvent Card = Carte.GetCarte(IdRepairsCard) as RepairsEvent;
                                            if (Card.ValueEnergy <= energy)
                                            {
                                                energy -= Card.ValueEnergy;
                                                CardsOnHands.RemoveAt(NumberCardRepairs);
                                                RepairsCardEvent(this, IdRepairsCard, Card, NumberCardRepairs, Repairable);
                                            }
                                        }
                                        break;
                                    case MsgType.AllDamageEvent:
                                        if (myProgress)
                                        {
                                            short NumberCard = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Data, 0));//номер карты  в руке у игрока
                                            int IdRepairsCard = CardsOnHands[NumberCard];
                                            AllDamageEvent Card = Carte.GetCarte(IdRepairsCard) as AllDamageEvent;
                                            if (Card.ValueEnergy <= energy)
                                            {
                                                energy -= Card.ValueEnergy;
                                                CardsOnHands.RemoveAt(NumberCard);
                                                AllDamageEvent(this, IdRepairsCard, Card, NumberCard);
                                            }
                                        }
                                     break;

                                }
                            }
                            //добавить иначе
                        }
                        else
                        {
                            switch (msgType)
                            {
                                case MsgType.EndProgress:
                                     EndProgress(); 
                                    break;
                                case MsgType.DeliteSeek:
                                    GetOutOfQueue();
                                    break;
                                case MsgType.ClientClosing:
                                    FailedSendMsg(this);
                                    break;
                            }
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        public void Initialize()
        {
            //устанавливаем энеригю на 0
            maxEnergy = 0;
            energy = 0;

            //инициализируем колоды карт
            carteUser = new List<int>();
            CardsOnMargin = new List<Robot>();
            CardsOnHands = new List<int>();
            userHq = new HeadQuarters();
        }
        private int RecData(byte[] data, int length, NetworkStream stream)
        {
            try
            {
                int ReadBytes = 0;
                while (ReadBytes != length)
                {
                    int readed = stream.Read(data, 0, length - ReadBytes);
                    ReadBytes += readed;
                    if (readed == 0) return 0;
                }
                return ReadBytes;
            }
            catch (IOException e)
            {
                if (GetOutOfQueue != null)
                {
                    GetOutOfQueue();
                    return 0;
                }
                if (FailedSendMsg != null)
                {
                    FailedSendMsg(this);
                    Console.WriteLine("Вызван метод окончания игровой сессии из-за недоступности одного из игроков");
                }
                return 0;
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return 0; }
            }
        public void Send(MsgType TypeMsg)
        {
            try
            {
                short length = 0;
                length = IPAddress.HostToNetworkOrder(length);

                TcpStream.Write(Summ((byte)TypeMsg, BitConverter.GetBytes(length)), 0, 3);

            }
            //временно
            catch (IOException e)
            {
                if (FailedSendMsg != null)
                {
                    FailedSendMsg(this);
                    Console.WriteLine("Вызван метод окончания игровой сессии из-за недоступности одного из игроков");
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }  

        public void Send(int number, MsgType TypeMsg)
        {
            try
            {
                int Length = 2;
                short leng = IPAddress.HostToNetworkOrder((short)Length);
                short Value = IPAddress.HostToNetworkOrder((short)number);

                byte[] head = Summ((byte)TypeMsg, BitConverter.GetBytes(leng));

                TcpStream.Write(SummNumber(head, BitConverter.GetBytes(Value)), 0, 5);


            }
            //временно
            catch (IOException e)
            {
                if (FailedSendMsg != null)
                {
                    FailedSendMsg(this);
                    Console.WriteLine("Вызван метод окончания игровой сессии из-за недоступности одного из игроков");
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
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
        public void Send(string message, MsgType TypeMsg)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                int length = data.Length;
                short leng = IPAddress.HostToNetworkOrder((short)length);

                //Отправляем
                TcpStream.Write(Summ((byte)TypeMsg, BitConverter.GetBytes(leng)), 0, 3);
                TcpStream.Write(data, 0, data.Length);
            }
            //временно
            catch (IOException e)
            {
                if (FailedSendMsg != null)
                {
                    FailedSendMsg(this);
                    Console.WriteLine("Вызван метод окончания игровой сессии из-за недоступности одного из игроков");
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
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
                        //конвертируем чилсо для передачи по сети и преобразуем в байты  
                        temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)IDCarte[i]));
                        data[index] = temp[0];
                        data[index + 1] = temp[1];
                        index += 2;
                    }
                    TcpStream.Write(Summ((byte)TypeMsg, BitConverter.GetBytes(leng)), 0, 3);
                    TcpStream.Write(data, 0, length);

                }
            }
            //временно
            catch (IOException e)
            {
                if (FailedSendMsg != null)
                {
                    FailedSendMsg(this);
                    Console.WriteLine("Вызван метод окончания игровой сессии из-за недоступности одного из игроков");
                }
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString());}
            
        }
        private byte[] Summ(byte type, byte[] len)
        {
            byte[] ret = new byte[len.Length + 1];
            ret[0] = type;
            ret[1] = len[0];
            ret[2] = len[1];
            return ret;
        }
        public void DisposeUserToSeek()
        {
            UserTcpClient.Close();
            UserTcpClient = null;
            TcpStream = null;

            StopThread = false;
            //запускаем поток обработки сообшений
           
        }
        public void Dispose()
        {
          
           
            //закрываем соединение
            UserTcpClient.Close();
            UserTcpClient = null;
            TcpStream = null;

            StopThread = false;

            name = null;
            Attack = null;
            DamageCardEvent = null;
            RepairsCardEvent = null;
            AllDamageEvent = null;
            carteUser = null;

            CardsOnHands.Clear();
            CardsOnHands = null;

            CardsOnMargin.Clear();
            CardsOnMargin = null;

            userHq = null;
            ColodRec = null;
            NameRec = null;
            EndProgress = null;
            AddCardField = null;

        }

    }
}
