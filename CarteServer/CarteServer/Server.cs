using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace CarteServer
{
    class Server
    {
         User ExpectUser;//клиент ожидающий соединения
         int CountSession;

        public  List<Session> AllSession = new List<Session>();
        public  Server()
        {
            
            ExpectUser = null;
            CountSession = 0;
            Console.WriteLine("Сервер запущен...");

        }

        public  void AddUser(User NewUser)
        {
            try
            {
                if (ExpectUser != null)
                {
                    ExpectUser.GetOutOfQueue-= DeliteOfQueue;
                    AllSession.Add(new Session(ExpectUser, NewUser));
                    ExpectUser = null;
                    AllSession[CountSession].SessionEnd += DeliteSession;
                    //запускаем партию в отдельном потоке

                    CountSession++;

                }

                else
                {
                    ExpectUser = NewUser;
                    ExpectUser.GetOutOfQueue += DeliteOfQueue;
                }
                   
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString());  }


        }
        private void DeliteOfQueue()
        {
            if (ExpectUser != null)
            {
                ExpectUser.Send(MsgType.DeliteSeek);
                ExpectUser.DisposeUserToSeek();
                ExpectUser = null;
                Console.WriteLine("Пользователь находящийся в очереди удален!");
            }
        }
        private  void DeliteSession(Session sender)
        {
          if (AllSession.Count != 0)
            {
                AllSession.Remove(sender);
                Console.WriteLine("Сессия удалена...");
                CountSession--;
            }

        }







    }
}


