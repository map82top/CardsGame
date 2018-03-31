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

                    AllSession.Add(new Session(ExpectUser, NewUser));
                    ExpectUser = null;
                    AllSession[CountSession].SessionEnd += new EventHandler(DeliteSession);
                    //запускаем партию в отдельном потоке
                    
                    CountSession++;
                   
                }

                else ExpectUser = NewUser;
                   
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString());  }


        }
        public void DeliteSession(object sender, EventArgs e)
        {
            /*if (number + 1 < Sesion.Count)
            {
                for (int i = number + 1; i < Sesion.Count; i++)
                {
                    Sesion[i].DownUser();
                    Sesion[i].Number--;
                }
            }
            else
            {
                Sesion.RemoveAt(number);
            }
            OnlyUser--;*/

        }







    }
}


