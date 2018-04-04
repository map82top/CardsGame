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
       
        
        public SeekForm(Controler controler)
        {
            InitializeComponent();
            ClientContr = controler;
            ClientContr.StartGame += LoadGameInterface;
            ClientContr.DeliteSeek += FindAnswerDeliteSeek;
            ClientContr.ErrorConnectToServer += ErrorConnectionServer;
        }
        private void ErrorConnectionServer()
        {
            this.Invoke((MethodInvoker)delegate
            {
               
                ChoiceForm NewForm = new ChoiceForm(ClientContr);
                NewForm.Show();
                this.Close();
            });
         }
        private void LoadGameInterface()
        {
            ClientContr.ErrorConnectToServer -= ErrorConnectionServer;
            //отвязываем этот обработчик
            ClientContr.StartGame -= LoadGameInterface;
            GameMargin NewForm = new GameMargin(ClientContr);
            this.Invoke((MethodInvoker)delegate {
              
                NewForm.Show();
                this.Close();
               
            });
            
        }
        private void FindAnswerDeliteSeek()
        {
            ClientContr.DeliteSeek -= FindAnswerDeliteSeek;
            ClientContr.DialogWithServ.Disconnect();
            this.Invoke((MethodInvoker)delegate {
                ChoiceForm NewForm = new ChoiceForm(ClientContr);
                NewForm.Show();
                this.Close();
            });
        }
        private void buttonBack_Click(object sender, EventArgs e)
        {
            //посылаем сообщение отмены
            ClientContr.DialogWithServ.Send(MsgType.DeliteSeek);
        }
        /// <summary>
        /// Отправляет сообщение без данных
        /// </summary>
        /// <param name="TypeMsg"></param>
    }
}
