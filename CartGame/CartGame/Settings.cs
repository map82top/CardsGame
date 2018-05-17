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
        const string wayInfo = "system_info/info_server.txt";//путь до файла с настройками
        Form Temp = null;
        public Settings(Form StartForm)
        {
            InitializeComponent();
            Temp = StartForm;
        }
        
        /// <summary>
        /// Выполняет загрузку и отображение данных, если такие есть
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Load(object sender, EventArgs e)
        {
            try
            {
                //меняем вид формы
                Save.Text = "Изменить";
                ip.ReadOnly = true;
                port.ReadOnly = true;
                nickname.ReadOnly = true;
                label1.Text = "Последний сохраненный IP-адрес сервера:";
                label2.Text = "Последний сохраненный порт сервера:";
                label3.Text = "Ваш никнейм:";

                using (var papk = new FileStream(wayInfo, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var StreamRead = new StreamReader(papk))
                    {
                        string s = StreamRead.ReadLine();
                        string[] Data = s.Split(new Char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        ip.Text = Data[0]; port.Text = Data[1]; nickname.Text = Data[2];
                    }
                }
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
                        //создаем директорию если она до этого не была создана
                        var Direct = new DirectoryInfo("system_info");
                        Direct.Create();
    
                        using (var File = new FileStream(wayInfo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                        {
                            //проверяем правильность ввода данных
                           
                            using (var FileWrite = new StreamWriter(File))
                            {
                                IPAddress TempIp;
                                if (IPAddress.TryParse(ip.Text, out TempIp))
                                {
                                    int Port;
                                    if (int.TryParse(port.Text, out Port))
                                    {
                                        if (0 < Port && Port < 65536)
                                        {
                                            //сохраняем данные и закрываем форму
                                            FileWrite.WriteLine(ip.Text + ":" + port.Text + ":" + nickname.Text);
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
                    }
                }
                else//есл кнопка имеет значение сохранить
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
                MessageBox.Show("Неудалось сохранить данные");
                WriteLog.Write(E.ToString());
            }
        }

        
    }
}
