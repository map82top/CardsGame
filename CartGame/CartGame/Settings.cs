using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;


namespace CartGame
{
    public partial class Settings : Form
    {
        const string text = "system_info/info_server.txt";
        Form Temp = null;
        public Settings(Form StartForm)
        {
            InitializeComponent();
            Temp = StartForm;
        }
        

        private void Settings_Load(object sender, EventArgs e)
        {
            try
            {
                var papk = new FileStream(text, FileMode.Open,FileAccess.Read, FileShare.ReadWrite);
                Save.Text = "Изменить";
                ip.ReadOnly = true;
                port.ReadOnly = true;
                nickname.ReadOnly = true;
                label1.Text = "Последний сохраненный IP-адрес сервера:";
                label2.Text = "Последний сохраненный порт сервера:";
                label3.Text = "Ваш никнейм:";

                var StreamRead = new StreamReader(papk);
                string s = StreamRead.ReadLine();
                string[] Data = s.Split(new Char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                ip.Text = Data[0]; port.Text = Data[1]; nickname.Text = Data[2];
                StreamRead.Dispose();
                papk.Dispose();

            }
            catch
            {
                MessageBox.Show("Нет данных о предыдущем подключении! Введите данные!");
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (Save.Text == "Сохранить")
                {

                    if (!string.IsNullOrWhiteSpace(ip.Text) & !string.IsNullOrWhiteSpace(port.Text) & !string.IsNullOrWhiteSpace(nickname.Text))
                    {
                       var File = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write,FileShare.ReadWrite);
                        //проверяем правильность ввода данных
                        IPAddress TempIp;
                        var FileWrite = new StreamWriter(File);
                        if (IPAddress.TryParse(ip.Text, out TempIp))
                        {
                            int Port;
                            if (int.TryParse(port.Text, out Port))
                            {
                                if (0 < Port && Port < 65536)
                                {
                                    FileWrite.WriteLine(ip.Text + ":" + port.Text + ":" + nickname.Text);
                                    FileWrite.Dispose();
                                    File.Dispose();
                                    this.Close();
                                }
                                else errorPort.SetError(port, "!Номер порта превышает допустимые значения");
                            }
                            else errorPort.SetError(port, "!Неверный формат порта");

                        }
                        else
                        {
                            errorIPAdress.SetError(ip, "!IPv4 адрес имеет недопустимые значения, или неверный формат ввода");
                        }
                 
                        
                    }
                }
                else
                {
                    ip.ReadOnly = false;
                    port.ReadOnly = false;
                    nickname.ReadOnly = false;

                    label1.Text = "Введите IP-адрес сервера:";
                    label2.Text = "Введите порт сервера:";
                    label3.Text = "Введите никнейм:";
                    Save.Text = "Сохранить";


                }

            }
            catch (Exception E)
            {
                MessageBox.Show("Неудалось сохранить данные./nОшибка " + E.ToString());
            }
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Temp is FormStart) (Temp as FormStart).buttonEnabled();
            else (Temp as ChoiceForm).buttonEnabled();
        }
    }
}
