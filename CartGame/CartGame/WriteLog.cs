﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace CartGame
{
   static class WriteLog
   {
        static string wayLog = "log.log";
        static string wayGameLog = "GameLog.log";
        static Mutex SinchWriteGameLog = new Mutex();
        static Mutex SinchWriteLog = new Mutex();
        static public void Write(string TextError)
        {
            SinchWriteLog.WaitOne();
            File.AppendAllText(wayLog, Environment.NewLine + DateTime.Now + "  " +  TextError, Encoding.UTF8);//записываем данные в лог
            SinchWriteLog.ReleaseMutex();
        }
        static public void WriteGameLog(string data)
        {
            SinchWriteGameLog.WaitOne();
            File.AppendAllText(wayGameLog, Environment.NewLine + DateTime.Now + "  " + data, Encoding.UTF8);//записываем данные в лог c действиями в игре
            SinchWriteGameLog.ReleaseMutex();
        }
    }
        
}
