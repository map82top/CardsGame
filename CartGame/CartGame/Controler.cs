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
        public event End EndGame;
        public event EmptyDel DeliteSeek;
        public event EmptyDel ErrorConnectToServer;

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
            dialogWithServ.ErrorConnectToServer -= ErrorConnection;
            ErrorConnectToServer();
            //освобождаем ресурсы
            this.Dispose();
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
        public void Dispose()
        {
            DataSession.Dispose();
            dialogWithServ.Disconnect();
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

    }
        private void ProcMsgWithData(MsgType type, byte[] data)
        {
            
            switch (type)
            {
                case MsgType.StartGame:

                    DataSession.EnName = Encoding.UTF8.GetString(data);
                    StartGame();
                    break;

                case MsgType.AddUserCarte:

                    short IdCarte = BitConverter.ToInt16(data,0);
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
                    short damageUser = BitConverter.ToInt16(data, 4);
                    short damageEnemy = BitConverter.ToInt16(data, 6);
                    if (attacking == -1)
                    {
                        if (attacked == -1)
                        {
                            DataSession.EnemyHQ.Armor -= damageEnemy;
                            DataSession.UserHQ.Armor -= damageUser;
                        }
                        else
                        {
                            DataSession.EnCarteOnField[attacked].Armor -= damageEnemy;
                            DataSession.UserHQ.Armor -= damageUser;
                            if (DataSession.EnCarteOnField[attacked].Armor <= 0)
                                //удаляем карту
                                DataSession.EnCarteOnField.RemoveAt(attacked);
                     
                        }
                    }
                    else
                    {
                        if (attacked == -1)
                        {
                            DataSession.UsCarteOnField[attacking].Armor -= damageUser;
                            DataSession.EnemyHQ.Armor -= damageEnemy;
                            if (DataSession.UsCarteOnField[attacking].Armor <= 0)
                                //удаляем карту
                                DataSession.UsCarteOnField.RemoveAt(attacking);
                          
                        }
                        else
                        {
                            DataSession.UsCarteOnField[attacking].Armor -= damageUser;
                            DataSession.EnCarteOnField[attacked].Armor -= damageEnemy;
                            //удаляем карты у которых очки прочности меньше 0
                            if (DataSession.UsCarteOnField[attacking].Armor <= 0)
                                DataSession.UsCarteOnField.RemoveAt(attacking);

                            if (DataSession.EnCarteOnField[attacked].Armor <= 0)
                                DataSession.EnCarteOnField.RemoveAt(attacked);

                        }
                    }
                    //оповещаем об атаке
                    MyAttack(attacking, attacked,damageUser, damageEnemy);               
                        break;
                case MsgType.EnAttackSucc:
                    short Attacking = BitConverter.ToInt16(data, 0);
                    short Attacked = BitConverter.ToInt16(data, 2);
                    short DamageUser = BitConverter.ToInt16(data, 4);
                    short DamageEnemy = BitConverter.ToInt16(data, 6);
                    if (Attacking == -1)
                    {
                        if (Attacked == -1)
                        {
                            DataSession.EnemyHQ.Armor -= DamageEnemy;
                            DataSession.UserHQ.Armor -= DamageUser;
                            //оповещаем об атаке
                            
                        }
                        else
                        {
                            DataSession.UsCarteOnField[Attacked].Armor -= DamageUser;
                            DataSession.EnemyHQ.Armor -= DamageEnemy;
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
                            DataSession.EnCarteOnField[Attacking].Armor -= DamageEnemy;
                            DataSession.UserHQ.Armor -= DamageUser;
                            if (DataSession.EnCarteOnField[Attacking].Armor <= 0)
                                DataSession.EnCarteOnField.RemoveAt(Attacking);
                        }
                        else
                        {
                            DataSession.EnCarteOnField[Attacking].Armor -= DamageEnemy;
                            DataSession.UsCarteOnField[Attacked].Armor -= DamageUser;
                            //удаляем уничтоженные карты
                            if (DataSession.EnCarteOnField[Attacking].Armor <= 0)
                                DataSession.EnCarteOnField.RemoveAt(Attacking);

                            if (DataSession.UsCarteOnField[Attacked].Armor <= 0)
                                DataSession.UsCarteOnField.RemoveAt(Attacked);
                        } 
                        

                   }
                    //оповещаем об атаке
                    EnAttack(Attacking, Attacked, DamageUser, DamageEnemy);
                    break;
            }
        }
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
                    Thread.Sleep(1);

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
                    int readed = stream.Read(data, 0, length - ReadBytes);
                    ReadBytes += readed;
                    if (readed == 0) return 0;
                }
                return ReadBytes;
            }
            catch (IOException e)
            {
                ErrorConnectToServer();
                MessageBox.Show("Связь с сервером потеряна!");
                return 0;
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString()); return 0; }
        }

        public void Disconnect()
        { 

               StopThread = false;
               Client.Close();
              Client = null;
              ClientNetwork = null;
               
               
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
            catch (IOException e)
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


