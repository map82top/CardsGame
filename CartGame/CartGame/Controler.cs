using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;

namespace CartGame
{
     public delegate void EmptyDel();//делегат для функций без аргументов
    public delegate void ContrDel(Controler sender);
    public delegate void IntDel(int number);
    public delegate void MsgWithoutData(MsgType type);//делегат для сообщения без данных
    public delegate void MsgWithData(MsgType type, byte[] data);//делегат для сообщения с данными
    public delegate void StringDel(string data);
    public delegate void Attack(int attacking, int attacked);
   

   public class Controler
    {

        private SendAndRecMsg dialogWithServ;
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

        private DataGame DataSession;

        public DataGame GetDataGame
        {
           get { return DataSession; }
        }
        public Controler()
        {

           DataSession = new DataGame();
          

        }
        public void Start(IPAddress IpAdress, int Port, string Name)
        {
            //инициализируем класс работы с сервером
            dialogWithServ = new SendAndRecMsg(IpAdress, Port);

            //подписываемя на получение сообщений
            dialogWithServ.RecMsgWithoutData += ProcMsgWithoutData;
            dialogWithServ.RecMsgWithData += ProcMsgWithData;


            dialogWithServ.Connect();
            if (dialogWithServ.Connected)
            {
                DataSession.InicializeListCards();
                SucConnect(this);
                
            }
            else FailConnect(this);


        }
        private void ProcMsgWithoutData(MsgType type)
        {
           
            switch (type)
            {
                case MsgType.StartSession:
                    dialogWithServ.Send(DataSession.UserColoda, MsgType.CarteUser);
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
                  
            }
        }
        private void ProcMsgWithData(MsgType type, byte[] data)
        {
            
            switch (type)
            {
                case MsgType.StartGame:

                    DataSession.EnName = BitConverter.ToString(data);
                    StartGame();
                    break;

                case MsgType.AddUserCarte:

                    short IdCarte = BitConverter.ToInt16(data,0);
                    IdCarte = IPAddress.NetworkToHostOrder(IdCarte);
                    Debug.WriteLine("ИД карты" + IdCarte);
                    //добавляем карты в массив карт пользователя
                    DataSession.CarteFromUser.Add(IdCarte);
                    //перерисовываем все карты у пользователя
                     PaintUserCarte(IdCarte);
                    break;

                case MsgType.UserMaxEnergy:

                    short MyMaxEnergy = BitConverter.ToInt16(data, 0);
                    MyMaxEnergy = IPAddress.NetworkToHostOrder(MyMaxEnergy);
                    DataSession.MyMaxEnergy = MyMaxEnergy;
                    Debug.WriteLine("У меня макс энергии" + MyMaxEnergy);
                    //оповещяем о измении энергии
                    PaintMyEnergy();
                    break;

                case MsgType.YourEnergy:
                    short MyEnergy = BitConverter.ToInt16(data, 0);
                    MyEnergy = IPAddress.NetworkToHostOrder(MyEnergy);
                    DataSession.MyEnergy = MyEnergy;
                    Debug.WriteLine("У меня энергии" + MyEnergy);
                    //оповещяем о измении энергии
                    PaintMyEnergy();
                    break;

                case MsgType.EnemyMaxEnergy:
                    short EnMaxEnergy = BitConverter.ToInt16(data, 0);
                    EnMaxEnergy = IPAddress.NetworkToHostOrder(EnMaxEnergy);
                    Debug.WriteLine("Макс кол энергии у противника" + EnMaxEnergy);
                    DataSession.EnMaxEnergy = EnMaxEnergy;
                    //оповещяем о измении энергии
                    PaintEnEnergy();
                    break;

                case MsgType.EnemyEnergy:
                    short EnEnergy = BitConverter.ToInt16(data, 0);
                    EnEnergy = IPAddress.NetworkToHostOrder(EnEnergy);
                    Debug.WriteLine("кол энергии у противника"+EnEnergy);
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
                    //добавляем карту в массив картна поле
                    DataSession.UsCarteOnField.Add((Robot)Carte.GetCarte(IdCards));
                    AddCardsOnField(NumberCarte);
                    break;
                case MsgType.EnemyAddCarteOnField:
                    short EnIdCarte = BitConverter.ToInt16(data, 0);
                    EnIdCarte = IPAddress.NetworkToHostOrder(EnIdCarte);
                    DataSession.EnCarteOnField.Add((Robot)Carte.GetCarte(EnIdCarte));
                    EnAddCardOnField();
                    break;
                case MsgType.MyAttackSucc:
                    short attacking = BitConverter.ToInt16(data, 0);
                    short attacked = BitConverter.ToInt16(data, 2);
                    if (attacking == -1)
                    {
                        if (attacked == -1)
                        {
                            DataSession.EnemyHQ.Armor -= DataSession.UserHQ.Attack;
                            DataSession.UserHQ.Armor -= DataSession.EnemyHQ.Attack;
                        }
                        else
                        {
                            DataSession.EnCarteOnField[attacked].Armor -= DataSession.UserHQ.Attack;
                            DataSession.UserHQ.Armor -= DataSession.EnCarteOnField[attacked].Attack;
                            if (DataSession.EnCarteOnField[attacked].Armor <= 0)
                                //удаляем карту
                                DataSession.EnCarteOnField.RemoveAt(attacked);
                     
                        }
                    }
                    else
                    {
                        if (attacked == -1)
                        {
                            DataSession.UsCarteOnField[attacking].Armor -= DataSession.EnemyHQ.Attack;
                            DataSession.EnemyHQ.Armor -= DataSession.UsCarteOnField[attacking].Attack;
                            if (DataSession.UsCarteOnField[attacking].Armor <= 0)
                                //удаляем карту
                                DataSession.UsCarteOnField.RemoveAt(attacking);
                          
                        }
                        else
                        {
                            DataSession.UsCarteOnField[attacking].Armor -= DataSession.EnCarteOnField[attacked].Attack;
                            DataSession.EnCarteOnField[attacked].Armor -= DataSession.UsCarteOnField[attacking].Attack;
                            //удаляем карты у которых очки прочности меньше 0
                            if (DataSession.UsCarteOnField[attacking].Armor <= 0)
                                DataSession.UsCarteOnField.RemoveAt(attacking);

                            if (DataSession.EnCarteOnField[attacked].Armor <= 0)
                                DataSession.EnCarteOnField.RemoveAt(attacked);

                        }
                    }
                    //оповещаем об атаке
                    MyAttack(attacking, attacked);               
                        break;
                case MsgType.EnAttackSucc:
                    short Attacking = BitConverter.ToInt16(data, 0);
                    short Attacked = BitConverter.ToInt16(data, 2);
                    if (Attacking == -1)
                    {
                        if (Attacked == -1)
                        {
                            DataSession.EnemyHQ.Armor -= DataSession.UserHQ.Attack;
                            DataSession.UserHQ.Armor -= DataSession.EnemyHQ.Attack;
                            //оповещаем об атаке
                            
                        }
                        else
                        {
                            DataSession.UsCarteOnField[Attacked].Armor -= DataSession.EnemyHQ.Attack;
                            DataSession.EnemyHQ.Armor -= DataSession.UsCarteOnField[Attacked].Attack;
                            if (DataSession.UsCarteOnField[Attacked].Armor <= 0)
                            {
                                //удаляем карту
                                DataSession.UsCarteOnField.RemoveAt(Attacked);
                            }
                        }
                    }
                    else
                    {
                        if (Attacked == -1)
                        {
                            DataSession.EnCarteOnField[Attacking].Armor -= DataSession.UserHQ.Attack;
                            DataSession.UserHQ.Armor -= DataSession.EnCarteOnField[Attacking].Attack;
                            if (DataSession.EnCarteOnField[Attacking].Armor <= 0)
                                DataSession.EnCarteOnField.RemoveAt(Attacking);
                        }
                        else
                        {
                            DataSession.EnCarteOnField[Attacking].Armor -= DataSession.UsCarteOnField[Attacked].Attack;
                            DataSession.UsCarteOnField[Attacked].Armor -= DataSession.EnCarteOnField[Attacking].Attack;
                            //удаляем уничтоженные карты
                            if (DataSession.EnCarteOnField[Attacking].Armor <= 0)
                                DataSession.EnCarteOnField.RemoveAt(Attacking);

                            if (DataSession.UsCarteOnField[Attacked].Armor <= 0)
                                DataSession.UsCarteOnField.RemoveAt(Attacked);
                        } 
                        

                   }
                    //оповещаем об атаке
                    EnAttack(Attacking, Attacked);
                    break;
            }
        }
        public SendAndRecMsg DialogWithServ
        {
            get { return dialogWithServ; }
        }
        public void Close()
        {
            dialogWithServ.Disconnect();

           

            //отвязываем все события
            SucConnect = null;
            FailConnect = null;
            StartGame = null;
                  
            
    }
    }
   public class SendAndRecMsg
    {
        TcpClient Client;
        IPEndPoint EndPoint;
        NetworkStream ClientNetwork;
        private Thread ThProcesMsg;
        
        //события получения сообщения
        public event MsgWithoutData RecMsgWithoutData;
        public event MsgWithData RecMsgWithData;
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
                if (!Client.Connected)
                {
                    Client.Connect(EndPoint);
                    ClientNetwork = Client.GetStream();
                     ThProcesMsg = new Thread(ProcessMsg);
                    ThProcesMsg.Start();
                }
        }

            private void ProcessMsg()
            {
            try
            {
                while (Client.Connected)
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
                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            }
        private int RecData(byte[] data, int length, NetworkStream stream)
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

        public void Disconnect()
        {
               Client.Close();
               ThProcesMsg.Abort();
        }
        public void Send(MsgType TypeMsg)
        {
            try
            {
                short length = 0;
                length = IPAddress.HostToNetworkOrder(length);

                ClientNetwork.Write(Summ((byte)TypeMsg, BitConverter.GetBytes(length)), 0, 3);


            }
            //временно
            catch (Exception e) { MessageBox.Show(e.Message); }
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
                        temp = BitConverter.GetBytes((short)IDCarte[i]);
                        data[index] = temp[0];
                        data[index + 1] = temp[1];
                        index += 2;
                    }
                    ClientNetwork.Write(Summ((byte)TypeMsg, BitConverter.GetBytes(leng)), 0, 3);
                    ClientNetwork.Write(data, 0, length);

                }


            }
            //временно
            catch (Exception e) { MessageBox.Show(e.Message); }
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
            catch (Exception e) { MessageBox.Show(e.Message); }
        }
        public void Send(int number, MsgType TypeMsg)
        {
            try
            {
                int Length = 2;
                short leng = IPAddress.HostToNetworkOrder((short)Length);
                short Value = IPAddress.HostToNetworkOrder((short)number);
                Debug.Write("Отправлено число " + Value);
                byte[] head = Summ((byte)TypeMsg, BitConverter.GetBytes(leng));

                ClientNetwork.Write(SummNumber(head, BitConverter.GetBytes(Value)), 0, 5);

            }
            //временно
            catch (Exception e) { MessageBox.Show(e.Message); }
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


