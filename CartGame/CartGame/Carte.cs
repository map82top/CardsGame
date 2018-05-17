using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CartGame
{
    public enum MsgType
    {
        StartSession,//начало игры и сессии на сервере
        DeliteSeek,//запрос на удалении их очереди ожидания противника
        CarteUser,//запрос сервера на получении карт игрока
        GetName,//запрос сервера на получении имени игрока
        StartGame,//сервер сообщает о начале игры
        AddUserCarte,//сервер добавлет карту игроку в руки
        AddEnemyCarte,//сервер добавляет карту противнику в руки
        UserMaxEnergy,//сервер уведомляет игрока о его максимальной энергии на этот ход
        YourEnergy,//сервер сообщает о доступной энергии игрока в данный момент
        EnemyMaxEnergy,//максимальной количество энергии у противника
        EnemyEnergy,//сервер сообщает о доступной энергии противника в данный момент
        ProgressTime,//сервер сообщает об изменении времени, оставшегося на ход
        MyProgress,//сервер сообщает, что сейчас ход игрока
        EnemyProgress,//сервер сообщает, что сейчас ход противника
        AddCarteOnField,//сервер сообщает о удачном добавлении игроком карты на поле
        EnemyAddCarteOnField,//сервер сообщает о удачно добавлении противником карты на поле
        Attack,//клиент сообщает о атаке игрока на карты противника
        MyAttackSucc,//атака игрока закончилась успешно(была возможна)
        EnAttackSucc, //атака противника закончилась успешно(была возможна)
        EndProgress,//клиент сообщает о завершении игроком своего хода
        YouWin,//игрок победил
        YouOver,//игрок проиграл
        Draw,//ничья
        TechnicalVictory,//техничская победа(один из игроков недоступен)
        ClientClosing,//клиент собщает серверу о своем закрытии пользователем
        EnemyNoActiv,//сервер сообщает пользователю, что противник был не активен и поэтом сессия завершилась 
        YouNoActiv, //сервер сообщает игроку, что игрок был не активен и поэтом сессия завершилась 
        DamageEvent,//клиент уведомляет сервер о использовании события, наносящего одиночный урон
        UserDamageEvent,//сервер соощеает клиенту об удачно использовании события, наносящего одиночный урон
        EnemyDamageEvent,//враг удачно использовал событие, наносящее одиночный урон 
        RepairsEvent,//пользователь использовал событие востанволения
        UsRepairsEvent,//сервер сообщает об удачном использовании игроком событием восстановления
        EnRepairsEvent, //сервер сообщает об удачном использовании противником событием восстановления
        AllDamageEvent,//клиент сообщает серверу о исплользовании события массвого урона
        UsAllDeliteEvent,//сервер сообщает о удачном использовании игроком события наносщего массовый урон
        EnAllDeliteEvent,//сервер сообщает о удачном использовании противником события наносщего массовый урон
        ChatMsg//собщает о получении сообщения для чата



    }
    //тип карт-событий
    public enum TypeEventCard
    {
        DamageCard,
        HealingCard,
        AllDamageCard
    }
    public abstract class Carte
    {
        public abstract Panel ImageCartNormal();
        public abstract Panel ImageCartMax();
        public abstract Panel ImageCartFullMin();
        protected int id = 0;
        public int ID
        {
            get { return id; }
        }
        // public abstract Panel Clone { get; }
        static public Carte GetCarte(int ID)
        {
            Carte RetCarte;
            switch (ID)
            {
                case 0:
                    RetCarte = new HeadQuarters();
                    break;
                case 1:
                    RetCarte = new Recruit();
                    break;
                case 2:
                    RetCarte = new Duelist();
                    break;
                case 3:
                    RetCarte = new Veteran();
                    break;
                case 4: RetCarte = new Ambush();
                    break;
                case 5:
                    RetCarte = new Rocket();
                    break;
                case 6:
                    RetCarte = new PointAttackSpace();
                    break;
                case 7:
                    RetCarte = new RepairsBox();
                    break;
                case 8:
                    RetCarte = new FieldRepairs();
                    break;
                case 9:
                    RetCarte = new B1();
                    break;
                case 10:
                    RetCarte = new Boxer();
                    break;
                case 11:
                    RetCarte = new Gladiator();
                    break;
                case 12:
                    RetCarte = new Turret();
                    break;
                case 13:
                    RetCarte = new Destroyer();
                    break;
                case 14:
                    RetCarte = new BombAttack();
                    break;
                case 15:
                    RetCarte = new BigAttack();
                    break;
                case 16:
                    RetCarte = new Medic();
                    break;

                default:
                    RetCarte = null;
                    break;
            }
            return RetCarte;
        }

    }
    /// <summary>
    /// Базовый класс для всез роботов
    /// </summary>
   public class Robot : Carte
    {
        //общие характеристики каждой карты робота
        protected int attack;
        public int Attack
        {
            get { return attack; }
        }

        protected int armor;
        public int Armor
        {
            get { return armor; }
            set { armor = value; }
        }
        public virtual void NewProgress()
        {
            attackCount = 1;
            defenseCount = 1;
        }
        protected int attackCount;
        public int AttackCount
        {
            get
            {
                return attackCount;
            }
            set
            {
                if (value > 0)
                    attackCount = value;
                else attackCount = 0;
            }

        }
        protected int defenseCount;
        public int DefenseCount
        {
            get
            {
                return defenseCount;
            }
            set
            {
                if (value > 0)
                    defenseCount = value;
                else defenseCount = 0;
            }

        }

        protected int valueEnergy;
        public int ValueEnergy
        {
            get { return valueEnergy; }
        }
        protected int bonusAttack;//бонус к атаке 

        public virtual int BonusAttack
        {
            get { return bonusAttack;  }
            set { bonusAttack = value; }
        }
            
        protected string nameRobot;
        public string NameRobot
        {
            get { return nameRobot; }
        }
        public Robot()
        {
            attack = 0;
            armor = 0;
            valueEnergy = 0;
            bonusAttack = 0;
            nameRobot = "";
            attackCount = 1;
            defenseCount = 1;

        }
        
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);


            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack);
            attackLabel.Font = new Font("Arial", 9);
            attackLabel.Location = new Point(12, 16);
            attackLabel.Size = new Size(13, 13);
            CarteImage.Controls.Add(attackLabel);


            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 9);
            armorLabel.Location = new Point(11, 149);
            armorLabel.Size = new Size(13, 13);
            CarteImage.Controls.Add(armorLabel);

            //отображаем цену
            Label valueEnergyLabel = new Label();
            valueEnergyLabel.Text = Convert.ToString(valueEnergy);
            valueEnergyLabel.Font = new Font("Arial", 9);
            valueEnergyLabel.Location = new Point(117, 16);
            valueEnergyLabel.Size = new Size(13, 13);
            CarteImage.Controls.Add(valueEnergyLabel);

            //отображаем имя робота
            Label Name = new Label();
            Name.Text = nameRobot;
            Name.Font = new Font("Arial", 9);
            Name.Location = new Point(38, 20);
            Name.Size = new Size(65, 15);
            Name.TextAlign = ContentAlignment.MiddleCenter;
            CarteImage.Controls.Add(Name);

            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.TestCart;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);

            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack);
            attackLabel.Font = new Font("Arial", 14);
            attackLabel.Location = new Point(19, 25);
            attackLabel.Size = new Size(20, 20);
            CarteImage.Controls.Add(attackLabel);


            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 14);
            armorLabel.Location = new Point(18, 224);
            armorLabel.Size = new Size(20, 20);
            CarteImage.Controls.Add(armorLabel);

            //отображаем цену
            Label valueEnergyLabel = new Label();
            valueEnergyLabel.Text = Convert.ToString(valueEnergy);
            valueEnergyLabel.Font = new Font("Arial", 14);
            valueEnergyLabel.Location = new Point(176, 24);
            valueEnergyLabel.Size = new Size(20, 20);
            CarteImage.Controls.Add(valueEnergyLabel);

            //отображаем имя робота
            Label Name = new Label();
            Name.Text = nameRobot;
            Name.Font = new Font("Arial", 15);
            Name.Location = new Point(60, 40);
            Name.Size = new Size(100, 30);
            Name.TextAlign = ContentAlignment.MiddleCenter;
            CarteImage.Controls.Add(Name);

            //описание карты
            Label Descr = new Label();
            Descr.Font = new Font("Arial", 9);
            //Descr.BorderStyle = BorderStyle.Fixed3D;
            Descr.Location = new Point(50, 164);
            Descr.Size = new Size(100, 30);
            Descr.AutoSize = true;
            Descr.Margin = new Padding(3);
            Descr.MaximumSize = new Size(140, 60);
            Descr.Size = new Size(120, 30);
            Descr.TextAlign = ContentAlignment.MiddleCenter;
            CarteImage.Controls.Add(Descr);



            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.TestCart;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;

        }
        public virtual Panel ImageCartMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(85, 120);

            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack + bonusAttack);
            attackLabel.Font = new Font("Arial", 9);
            attackLabel.Location = new Point(10, 87);
            attackLabel.Size = new Size(12, 12);
            CarteImage.Controls.Add(attackLabel);


            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 9);
            armorLabel.Location = new Point(55, 87);
            armorLabel.Size = new Size(21, 12);
            CarteImage.Controls.Add(armorLabel);


            //отображаем имя
            Label Name = new Label();
            Name.Text = nameRobot;
            Name.Font = new Font("Arial", 9);
            Name.Location = new Point(10, 45);
            Name.Size = new Size(70, 12);
            Name.TextAlign = ContentAlignment.MiddleCenter;
            CarteImage.Controls.Add(Name);

            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.CarteMin;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);


            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack);
            attackLabel.Font = new Font("Arial", 9);
            attackLabel.Location = new Point(11, 9);
            attackLabel.Size = new Size(12, 12);
            CarteImage.Controls.Add(attackLabel);


            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 9);
            armorLabel.Location = new Point(11, 91);
            armorLabel.Size = new Size(12, 12);
            CarteImage.Controls.Add(armorLabel);

            //отображаем цену
            Label valueEnergyLabel = new Label();
            valueEnergyLabel.Text = Convert.ToString(valueEnergy);
            valueEnergyLabel.Font = new Font("Arial", 9);
            valueEnergyLabel.Location = new Point(76, 6);
            valueEnergyLabel.Size = new Size(12, 12);
            CarteImage.Controls.Add(valueEnergyLabel);

            //отображаем имя робота
            Label Name = new Label();
            Name.Text = nameRobot;
            Name.Font = new Font("Arial", 9);
            Name.Location = new Point(18,52);
            Name.Size = new Size(60, 13);
            Name.TextAlign = ContentAlignment.MiddleCenter;
            CarteImage.Controls.Add(Name);

            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.FullMinCarte;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }


    }
    /// <summary>
    /// Базовый класс для всез карт-событий
    /// </summary>
    abstract class Event : Carte
    {
        //стоимость есть у всех карт-событий
        protected int valueEnergy;
        public int ValueEnergy
        {
            get { return valueEnergy; }
        }
        //тип события
        protected TypeEventCard typeEvent;
        public TypeEventCard TypeEvent
        {
            get { return typeEvent; }
        }
        
    }
    /// <summary>
    /// Базовый класс для карт, наносящих урон
    /// </summary>
    class DamageEvent : Event
    {
        //урон по противнику
        protected int damage;
        public int Damage
        {
            get { return damage; }
        }
        public DamageEvent()
        {
            typeEvent = TypeEventCard.DamageCard;
            valueEnergy = 0;
            damage = 0;
        }

        public override Panel ImageCartNormal()
        {
              Panel CarteImage = new Panel();
              CarteImage.Size = new Size(145, 187);
             //изображение карты
             CarteImage.BackgroundImage = Properties.Resources.EventCard;
             CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.EventCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.EventCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }
    }

    /// <summary>
    /// Общий класс для всех карт наносящих урон по всем картам находящися на игровом поле
    /// </summary>
    class AllDamageEvent : Event
    {
        //урон по противнику
        protected int allDamage;
        public int AllDamage
        {
            get { return allDamage; }
        }
        public AllDamageEvent()
        {
            typeEvent = TypeEventCard.AllDamageCard;
            valueEnergy = 0;
            allDamage = 0;
        }

        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.EventCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.EventCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.EventCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }
    }

    /// <summary>
    /// Класс карты ковровая бомбандировка
    /// </summary>
    /// 
    class BombAttack : AllDamageEvent
    {
     

        public BombAttack()
        {
            typeEvent = TypeEventCard.AllDamageCard;
            valueEnergy = 3;
            allDamage = 1;
            id = 14;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BombAttackNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BombAttackNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BombAttackFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

    }

    class BigAttack : AllDamageEvent
    {

        public BigAttack()
        {
            typeEvent = TypeEventCard.AllDamageCard;
            valueEnergy = 6;
            allDamage = 3;
            id = 15;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BigAttackNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BigAttackNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BigAttackFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

    }

    class RepairsEvent : Event
    {
        //урон по противнику
        protected int damage;
        public int Damage
        {
            get { return damage; }
        }
        public RepairsEvent()
        {
            typeEvent = TypeEventCard.HealingCard;
            valueEnergy = 0;
            damage = 0;
        }

        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.EventCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.EventCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.EventCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }
    }
    /// <summary>
    /// Класс карты Засада
    /// </summary>
    class Ambush : DamageEvent
    {
        
        
        public Ambush()
        {
            typeEvent = TypeEventCard.DamageCard;
            valueEnergy = 0;
            damage = 1;
            id = 4;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.AmbushNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.AmbushNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.AmbushFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

    }

    /// <summary>
    /// Класс сверхзвуковой ракеты
    /// </summary>
    class Rocket : DamageEvent
    {

        public Rocket()
        {
            typeEvent = TypeEventCard.DamageCard;
            valueEnergy = 3;
            damage = 3;
            id = 5;

        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RocketNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RocketNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RocketFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

    }

    /// <summary>
    /// Класс космической точечной атаки
    /// </summary>
    class PointAttackSpace : DamageEvent
    {

        public PointAttackSpace()
        {
            typeEvent = TypeEventCard.DamageCard;
            valueEnergy = 8;
            damage = 7;
            id = 6;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.PointAttackSpaceCarteNormal;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.PointAttackSpaceCarteNormal;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.PointAttackSpaceFullMin;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

    }

    /// <summary>
    /// Класс ремкомплекта
    /// </summary>
    class RepairsBox : RepairsEvent
    {

        public RepairsBox()
        {
            typeEvent = TypeEventCard.HealingCard;
            valueEnergy = 1;
            damage = -1;
            id = 7;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RepairsBoxNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RepairsBoxNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RepairsBoxFullMin;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

    }

    /// <summary>
    /// Класс карты полевой ремонтной бригады
    /// </summary>
    class FieldRepairs : RepairsEvent
    {

        public FieldRepairs()
        {
            typeEvent = TypeEventCard.HealingCard;
            valueEnergy = 4;
            damage = -3;
            id = 8;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.FieldRepairsNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.FieldRepairsNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.FieldRepairsFullMin;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }

    }
    /// <summary>
    /// Класс карты Рекрут
    /// </summary>
    public class Recruit : Robot
    {
        //уникальный идентификатор карты
        //описание
       
        public Recruit()
        {
            attack = 2;
            armor = 5;
            valueEnergy = 2;
            id = 1;
            nameRobot = "Рекрут";
            bonusAttack = 0;
            attackCount = 1;
            defenseCount = 1;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RecruitNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RecruitFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.RecruitNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
     }


    public class Medic : Robot
    {
        //уникальный идентификатор карты
        //описание
        public Medic()
        {
            attack = 1;
            armor = 4;
            valueEnergy = 5;
            id = 16;
            nameRobot = "Медик";
            bonusAttack = 0;
            attackCount = 1;
            defenseCount = 1;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.MedicNormalCart;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.MedicFullMinlCart;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.MedicNormalCart;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
    }
    /// <summary>
    /// Класс карты Дуэлент
    /// </summary>
    public class Duelist : Robot
    {
      
        public Duelist()
        {
            attack = 4;
            armor = 2;
            valueEnergy = 3;
            nameRobot = "Дуэлянт";
            bonusAttack = 0;
            id = 2;
            attackCount = 1;
            defenseCount = 1;

        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.DuelistNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.DuelistFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.DuelistNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
    }

    public class Destroyer : Robot
    {
       
        public Destroyer()
        {
            attack = 5;
            armor = 10;
            valueEnergy = 8;
            nameRobot = "Разрушитель";
            bonusAttack = 0;
            id = 13;
            attackCount = 1;
            defenseCount = 1;

        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.DestroyerNormalCarte;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.DestroyerFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.DestroyerNormalCarte;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
    }

    /// <summary>
    /// Класс карты Ветеран
    /// </summary>
    public class Veteran : Robot
    {
      
        public Veteran()
        {
            attack = 2;
            armor = 5;
            valueEnergy = 4;
            bonusAttack = 0;
            nameRobot = "Ветеран";
            id = 3;
            attackCount = 1;
            defenseCount = 1;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.VeteranNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.VeteranNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.VeteranFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
    }

    /// <summary>
    /// Класс робота Боксер
    /// </summary>
    public class Boxer : Robot
    {

        public Boxer()
        {
            attack = 2;
            armor = 6;
            valueEnergy = 5;
            bonusAttack = 0;
            nameRobot = "Боксер";
            id = 10;
            attackCount = 1;
            defenseCount = 2;
        }
        public override void NewProgress()
        {
            attackCount = 1;
            defenseCount = 2;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BoxerNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BoxerNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.BoxerFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
    }

    /// <summary>
    /// Класс карты Гладиатор
    /// </summary>
    public class Gladiator : Robot
    {
  
        public Gladiator()
        {
            attack = 1;
            armor = 8;
            valueEnergy = 6;
            bonusAttack = 0;
            nameRobot = "Гладиатор";
            id = 11;
            attackCount = 3;
            defenseCount = 1;
        }
        public override void NewProgress()
        {
            attackCount = 3;
            defenseCount = 1;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.GladiatorNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.GladiatorNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.GladiatorFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
    }


    public class DefenceConstr : Robot
    {
        //общие характеристики каждой карты робота
        /// <summary>
        /// У данного сооружения нет бонуса к атаке
        /// </summary>
        public override int BonusAttack
        {
            get { return bonusAttack; }
            set { bonusAttack = 0; }
        }
       
        public DefenceConstr()
        {
            attack = 0;
            armor = 0;
            valueEnergy = 0;
            bonusAttack = 0;
            nameRobot = "";
           
        }

        
        public override Panel ImageCartMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(85, 120);

            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack + bonusAttack);
            attackLabel.Font = new Font("Arial", 9);
            attackLabel.Location = new Point(10, 87);
            attackLabel.Size = new Size(12, 12);
            CarteImage.Controls.Add(attackLabel);


            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 9);
            armorLabel.Location = new Point(55, 87);
            armorLabel.Size = new Size(21, 12);
            CarteImage.Controls.Add(armorLabel);


            //отображаем имя
            Label Name = new Label();
            Name.Text = nameRobot;
            Name.Font = new Font("Arial", 9);
            Name.Location = new Point(10, 45);
            Name.Size = new Size(70, 12);
            Name.TextAlign = ContentAlignment.MiddleCenter;
            CarteImage.Controls.Add(Name);

            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.CarteMin;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;

            return CarteImage;
        }
       


    }
    
    /// <summary>
    /// Класс защитного сооружения Турель
    /// </summary>
    public class Turret : DefenceConstr
    {
    
        public Turret()
        {
            attack = 1;
            armor = 10;
            valueEnergy = 5;
            bonusAttack = 0;
            nameRobot = "Турель";
            id = 12;
            attackCount = 1;
            defenseCount = 1;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.TurretNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.TurretNormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.TurretFullMinCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
    }
    /// <summary>
    /// Класс дроид B1
    /// </summary>
    public class B1 : Robot
    {

        public B1()
        {
            attack = 1;
            armor = 1;
            valueEnergy = 2;
            bonusAttack = 0;
            nameRobot = "B1";
            id = 9;
            attackCount = 1;
            defenseCount = 1;
        }
        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.B1NormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.B1NormalCard;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(100, 120);
            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.B1FullMin;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;//id карты
            return CarteImage;
        }
    }
    /// <summary>
    /// Класс карты Штаба
    /// </summary>
    public class HeadQuarters: Carte
    {
        private int attack;
        public int Attack
        {
            get { return attack; }
        }
        private int armor;
        public int Armor
        {
            get { return armor; }
            set { armor = value; }
        }
        private string name;
        public string Name
        {
            get { return name; }
        }
        public HeadQuarters()
        {
            attack = 2;
            armor = 28;
            name = "Штаб";
            id = 0;
            attackCount = 1;
            defenseCount = 1;

        }
        public void NewProgress()
        {
            attackCount = 1;
            defenseCount = 1;
        }
        protected int attackCount;
        public int AttackCount
        {
            get
            {
                return attackCount;
            }
            set
            {
                if (value > 0)
                    attackCount = value;
                else attackCount = 0;
            }

        }

        protected int defenseCount;
        public int DefenseCount
        {
            get
            {
                return defenseCount;
            }
            set
            {
                if (value > 0)
                    defenseCount = value;
                else defenseCount = 0;
            }

        }

        public override Panel ImageCartNormal()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(145, 187);


            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack);
            attackLabel.Font = new Font("Arial", 9);
            attackLabel.Location = new Point(12, 16);
            attackLabel.Size = new Size(13, 13);
            CarteImage.Controls.Add(attackLabel);


            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 9);
            armorLabel.Location = new Point(11, 149);
            armorLabel.Size = new Size(13, 13);
            CarteImage.Controls.Add(armorLabel);

            //отображаем имя робота
            Label Name = new Label();
            Name.Text = name;
            Name.Font = new Font("Arial", 9);
            Name.Location = new Point(38, 20);
            Name.Size = new Size(65, 15);
            Name.TextAlign = ContentAlignment.MiddleCenter;
            CarteImage.Controls.Add(Name);

            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.TestCart;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;
            return CarteImage;
        }
        public override Panel ImageCartMax()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(218, 280);

            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack);
            attackLabel.Font = new Font("Arial", 14);
            attackLabel.Location = new Point(19, 25);
            attackLabel.Size = new Size(20, 20);
            CarteImage.Controls.Add(attackLabel);

            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 14);
            armorLabel.Location = new Point(12, 224);
            armorLabel.Size = new Size(32, 20);
            CarteImage.Controls.Add(armorLabel);

            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.HQCardMax;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;
            return CarteImage;
        }
        public override Panel ImageCartFullMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(90, 120);

            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack);
            attackLabel.Font = new Font("Arial", 11);
            attackLabel.Location = new Point(12, 88);
            attackLabel.Size = new Size(14, 14);
            CarteImage.Controls.Add(attackLabel);


            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 11);
            armorLabel.Location = new Point(58,87);
            armorLabel.Size = new Size(26, 14);
            CarteImage.Controls.Add(armorLabel);


            //отображаем имя
            Label Name = new Label();
            Name.Text = name;
            Name.Font = new Font("Arial", 10);
            Name.Location = new Point(8, 28);
            Name.Size = new Size(70, 13);
            Name.TextAlign = ContentAlignment.MiddleCenter;
            CarteImage.Controls.Add(Name);

            //изображение карты
            CarteImage.BackgroundImage = Properties.Resources.HQMinCarte;
            CarteImage.BackgroundImageLayout = ImageLayout.Zoom;
            CarteImage.Tag = id;
            return CarteImage;
        }
    }
}
