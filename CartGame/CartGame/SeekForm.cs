using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CartGame
{
    public partial class SeekForm : Form
    {
        Controler ClientContr;
        private bool BoolLoadGame = false;//если true, то игровая форма уже создана
        Mutex LoadGameMutex = new Mutex();
        
        public SeekForm(Controler controler)
        {
            InitializeComponent();
            ClientContr = controler;
            ClientContr.StartGame += LoadGameInterface;
            ClientContr.ErrorConnectToServer += ErrorConnectionServer;
        }
        private void ErrorConnectionServer()
        {
            ChoiceForm NewForm = new ChoiceForm(ClientContr);
            WriteLog.WriteGameLog("Ошибка соединения с сервером!");
            this.Invoke((MethodInvoker)delegate
            {
                NewForm.Show();
                this.Close();
            });
         }
        /// <summary>
        /// Обработичк события начала игры
        /// </summary>
        private void LoadGameInterface()
        {

            LoadGameMutex.WaitOne();//мьютекс запрещает выполнять это метод повторно
                if (!BoolLoadGame)
                {
                    BoolLoadGame = true;
                    ClientContr.ErrorConnectToServer -= ErrorConnectionServer;
                    //отвязываем этот обработчик
                    ClientContr.StartGame -= LoadGameInterface;
                    GameMargin NewForm = new GameMargin(ClientContr);

                   WriteLog.WriteGameLog("Создается форма игрового поля");

                   this.Invoke((MethodInvoker)delegate
                    {                      
                        NewForm.Show();
                        this.Close();
                    });
                }
            LoadGameMutex.ReleaseMutex();
           
        }
        
        private void buttonBack_Click(object sender, EventArgs e)
        {
            //посылаем сообщение отмены
            ClientContr.DialogWithServ.Send(MsgType.DeliteSeek);
            ClientContr.Dispose();
            WriteLog.WriteGameLog("Пользователь вышел из режима ожидания противника");
            ChoiceForm NewForm = new ChoiceForm(ClientContr);
            this.Invoke((MethodInvoker)delegate
           {          
               NewForm.Show();
                this.Close();

            });
        }

        private void SeekForm_Load(object sender, EventArgs e)
        {

        }
    }
}
