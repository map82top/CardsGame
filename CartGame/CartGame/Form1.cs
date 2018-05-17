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
                    Controler controler = new Controler();
                    ChoiceForm NewForm =  new ChoiceForm(controler);
                    NewForm.Show();
                    this.Hide(); 
            }
            catch (Exception E)
            {
                WriteLog.Write(E.ToString());
            }

        }

      
    }
}
