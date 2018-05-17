using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CartGame
{
   static class WriteLog
   {
        static string wayLog = "Log/log.log";
        static public void Write(string TextError)
        {
            //создаем папку, если она еще не создана
            if (Directory.Exists("Log"))
            {
                Directory.CreateDirectory("Log");
            }

            File.AppendAllText(wayLog, TextError, Encoding.UTF8);//записываем данные в лог
        }
   }
}
