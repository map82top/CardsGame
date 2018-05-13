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
         int CountSession;//счетчик количества сессий
        private Object ObjectLockDeliteSession = new Object();
        private Object ObjectLockDeliteOfQueue = new Object();
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
            lock(ObjectLockDeliteOfQueue)
                {
                if (ExpectUser != null)
                {
                    ExpectUser.DisposeUserToSeek();
                    ExpectUser = null;
                    Console.WriteLine("Пользователь находящийся в очереди удален!");
                }
            }
        }
        private  void DeliteSession(Session sender)
        {
            lock(ObjectLockDeliteSession)
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
}


