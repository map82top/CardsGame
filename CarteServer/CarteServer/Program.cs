using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace CarteServer
{
   
    class Program
    {
        static Object ObjectLockSyncAddUser = new Object();
        static void Main(string[] args)
        {
            try
            {
                TcpListener List;
                //удаляем старые логи
                File.Delete("logServer.log");
                File.Delete("GameLogServer.log");

                Console.Write("Введиет IP-адрес хоста: ");
                IPAddress ipServer = IPAddress.Parse(Console.ReadLine());
                Console.Write("Введиет порт сервера: ");
                int portServer = int.Parse(Console.ReadLine());
                List = new TcpListener(ipServer, portServer);
                List.Start();
                //загрузка сервера
                Server NewServer = new Server();
                while (true)
                {
                    lock (ObjectLockSyncAddUser)
                    {
                        //добавляем пользователя в сервер
                        NewServer.AddUser(new User(List.AcceptTcpClient()));
                    }
                }
                
            }
            catch (FormatException)
            {
                Console.WriteLine("Неправильный формат ввода IP-адреса или порта!");
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Неправильный формат ввода IP-адреса или порта!");
            }
            catch (SocketException)
            {
                Console.WriteLine("Ошибка инициализации сокета!");
            }
            catch (Exception e)
            {
                WriteLog.Write("Ошибка инициализации сервера " + e.ToString());
            }
        }
    }
}
