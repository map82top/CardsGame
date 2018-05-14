using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace CartGame
{
    /// <summary>
    /// Занимается обработкой и отображение чата в игре
    /// </summary>
   public class ChatControler: IDisposable
    {
       
        private int countMissedMsg; //счетчик пропущенных сообщений
        public int CountMissedMsg
        {
            get { return countMissedMsg; }
        }
        private bool MaxOrMin; //если true чат развернут
        private TextBox ChatBox;
        private SendAndRecMsg DialogWithServer;
        Button SendMsg; //кнопка отправки сообещения
        TextBox BoxWrite; //поле ввода сообщения
        private string Name;

        public event IntDel NewMessage;

        public ChatControler(SendAndRecMsg DialogWithServer, string NameUser)
        {
            //задаем все физические данные ChatBox
            ChatBox = new TextBox();
            ChatBox.Multiline = true;
            ChatBox.Size = new Size(340,185);
            ChatBox.Font = new Font("Microsoft Sans Serif", 10);
            ChatBox.ReadOnly = true;
            ChatBox.WordWrap = true;
            ChatBox.BackColor = Color.White;

            //создаем кнопку отправки нового сообшения
            SendMsg = new Button();
            SendMsg.Text = "Отправить";
            SendMsg.Font = new Font("Microsoft Sans Serif", 9);
            SendMsg.Size = new Size(83, 30);
            SendMsg.Click += SendMsg_Click;
           

            //создаем поле ввода нового сообщения
            BoxWrite = new TextBox();
            BoxWrite.Size = new Size(254, 30);
            BoxWrite.Font = new Font("Microsoft Sans Serif", 10);
            BoxWrite.KeyUp += BoxWrite_KeyUp;

            

            //инициализуруем другие поля
            MaxOrMin = false;
            countMissedMsg = 0;
            this.DialogWithServer = DialogWithServer;
            Name = NameUser;
        }
        /// <summary>
        /// альтернативный способ отпраки сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoxWrite_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMessage();
            }
        }
        /// <summary>
        /// Возвращает панель чата для отображения
        /// </summary>
        /// <returns></returns>
        public Panel ShowChatBox()
        {
            Panel ChatPanel = new Panel();
            ChatPanel.Size = new Size(340,228);

            //добавляем на Чат-панель элементы чата
            ChatPanel.Controls.Add(ChatBox);
            ChatPanel.Controls.Add(SendMsg);
            ChatPanel.Controls.Add(BoxWrite);

            //задаем координаты 
            ChatBox.Location = new Point(0, 0);
            SendMsg.Location = new Point(257, 190);
            BoxWrite.Location = new Point(0, 190);

            MaxOrMin = true; //окно находится в развернутом состояниии
            countMissedMsg = 0;//обнуляем счетчик пропущенных сообщений

            return ChatPanel;
        }

        /// <summary>
        /// 
        /// </summary>
        public void MinChatBox()
        {
            //убираем родителя 
            ChatBox.Parent = null;
            SendMsg.Parent = null;
            BoxWrite.Parent = null;

            MaxOrMin = false; //окно находится в развернутом состояниии
        }

        private void SendMsg_Click(object sendr, EventArgs e)
        {
            SendMessage();
        }
        private void SendMessage()
        {
            string Msg = BoxWrite.Text;
            if (Msg != "")//если строка не пустая 
            {
                BoxWrite.Text = "";
                Msg = $"[{Name}]: {Msg}";
                DialogWithServer.Send(Msg, MsgType.ChatMsg);//отправляем сообщение
                //добавляем сообщение в чат пользователя
                if (ChatBox.Text != "") ChatBox.Text += Environment.NewLine + Msg;
                else ChatBox.Text = Msg;

            }
        }
        public void AddMessage(string message)
        {
            //если чат развернут
            if (MaxOrMin)
            { 
                    if (ChatBox.Text != "") ChatBox.Text += Environment.NewLine + message;
                    else ChatBox.Text = message;
                }
            else
            {
                countMissedMsg++; //увеличиваем счетчик непрочитанных сообщений
                if (ChatBox.Text != "") ChatBox.Text += Environment.NewLine + message;
                else ChatBox.Text = message;
                //уведомляем об новом сообещением клиент
                NewMessage(countMissedMsg);
            }
        }
        public void Dispose()
        {
            NewMessage = null;
            Name = null;
            if(ChatBox!=null)ChatBox.Dispose();
            ChatBox = null;
            if(SendMsg !=null)SendMsg.Dispose();
            SendMsg = null;
            if(BoxWrite!=null)BoxWrite.Dispose();
            BoxWrite = null;
        }

    }
}
