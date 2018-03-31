using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace CartGame
{
    public partial class FormStart : Form
    {
        const string text = "system_info/info_server.txt";
        public FormStart()
        {
            InitializeComponent();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var StreamInfo = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var StreamRead = new StreamReader(StreamInfo);
                string s = StreamRead.ReadLine();
                StreamInfo.Close();
                StreamInfo.Dispose();
                if (s != null)
                {
                    Controler controler = new Controler();
                    Application.Run(new ChoiceForm(controler));
                    this.Close();
                }
                else throw new DirectoryNotFoundException();
                
            }
            
            catch (DirectoryNotFoundException)
            { 
                button1.Enabled = false;
                Settings NewForm = new Settings(this);
                NewForm.Show();
                

            }
           
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }

        }

        public void buttonEnabled()
        {
            button1.Enabled = true;
        }

        
        private void настройкиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Settings NewForm = new Settings(this);
            NewForm.Show();
        }
    }
}
