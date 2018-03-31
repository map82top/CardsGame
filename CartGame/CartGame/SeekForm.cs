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
        Form backForm;
        
        public SeekForm(ChoiceForm BackForm, Controler controler)
        {
            InitializeComponent();
            ClientContr = controler;
            backForm = BackForm;
            ClientContr.StartGame += LoadGameInterface;

        }

        private void LoadGameInterface()
        {           
            //отвязываем этот обработчик
                  ClientContr.StartGame -= LoadGameInterface;
                  this.Invoke((MethodInvoker)delegate {
                            GameMargin NewForm = new GameMargin(ClientContr);
                          NewForm.Show();
                      this.Close();
                      //backForm.Close();
                         });
                                      
                              
        }
        private void buttonBack_Click(object sender, EventArgs e)
        {
            //посылаем сообщение отмены
            backForm.Show();
            this.Close();
        }
        /// <summary>
        /// Отправляет сообщение без данных
        /// </summary>
        /// <param name="TypeMsg"></param>
    }
}
