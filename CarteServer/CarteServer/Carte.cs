using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

public enum TypeEventCard
{
    DamageCard,
    HealingCard
}
namespace CarteServer
{
    //Эта иерархия классов отличается от тех, что находятся в клиенте у пользователя
        abstract class Carte
        {
        // public abstract Panel ImageCartNormal();
        //public abstract Panel ImageCartMin();
        //public abstract Panel ImageCartMax();
        // public abstract Panel Clone { get; }
        static int id = 0;
        public int ID
        {
            get { return id; }
        }
        /// <summary>
        /// Возвращает экземпляр карты по её id
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        static public Carte GetCarte(int ID)
            {
                Carte RetCarte;
                switch (ID)
                {
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
                    case 5: RetCarte = new Rocket();
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
                default:
                        RetCarte = null;
                        break;
                }
                return RetCarte;
            }
        }
        /// <summary>
        /// Класс всез карт-событий
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
        }
    /// <summary>
    /// Класс карты Засада
    /// </summary>
    class Ambush : DamageEvent
        {
        static int id = 4;
       
        public Ambush()
            {
                typeEvent = TypeEventCard.DamageCard;
                valueEnergy = 0;
                damage = 1;
            }
        }
    /// <summary>
    /// Класс карты Сверхзвуковой ракеты
    /// </summary>
        class Rocket : DamageEvent
        {
            static int id = 5;

            public Rocket()
            {
                typeEvent = TypeEventCard.DamageCard;
                valueEnergy = 3;
                damage = 3;
            }
        }
    /// <summary>
    /// Класс карты точечного удара из космоса
    /// </summary>
        class PointAttackSpace : DamageEvent
        {
            static int id = 6;

            public PointAttackSpace()
            {
                typeEvent = TypeEventCard.DamageCard;
                valueEnergy = 8;
                damage = 7;
            }
        }
    /// <summary>
    /// Класс карты Аптечка
    /// </summary>
        class RepairsBox : RepairsEvent
    {
            static int id = 7;

            public RepairsBox()
            {
                typeEvent = TypeEventCard.HealingCard;
                valueEnergy = 1;
                damage = -1;
            }
        }
     /// <summary>
     /// Класс карты Полевой ремонтной бригады
     /// </summary>
        class FieldRepairs : RepairsEvent
    {
            static int id = 8;

            public FieldRepairs()
            {
                typeEvent = TypeEventCard.HealingCard;
                valueEnergy = 4;
                damage = -3;
            }
        }
    /// <summary>
    /// Класс базовой карты Робот
    /// </summary>
    class Robot : Carte
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

            protected int valueEnergy;
            public int ValueEnergy
            {
                get { return valueEnergy; }
            }

            protected string nameRobot;
            public string NameRobot
            {
                get { return nameRobot; }
            }
            protected int attackCount;
            public int AttackCount
            {
                 get
                {
                     int count = attackCount;
                     attackCount--;
                     return count;
                }
                 
            }
            protected int defenseCount;
            public int  DefenseCount
            {
                get
                {
                    int count = defenseCount;
                    defenseCount--;
                    return count;
                }

            }

        //защитник этой карты
        protected DefenceConstr defender;
        public DefenceConstr Defender
        {
            get { return defender; }
            set { defender = value; }
        }
        protected int bonusAttack;//бонус к атаке 

            public virtual int BonusAttack
            {
                get { return bonusAttack; }
                set { bonusAttack = value; }
            }
           public virtual void NewProgress()
             {
                 attackCount = 1;
                 defenseCount = 1; 
             }
            public Robot()
            {
                attack = 0;
                armor = 0;
                valueEnergy = 0;
                nameRobot = "";
                attackCount = 0;
                bonusAttack = 0;
                defenseCount = 0;
                defender = null;

            }
    

       }
    /// <summary>
    /// Класс карты рекрут
    /// </summary>
        class Recruit : Robot
        {
            //уникальный идентификатор карты
            static int id = 1;
            
            //описание
            
         public Recruit()
             {
                 attack = 2;
                 armor = 5;
                valueEnergy = 2;
                attackCount = 1;
                nameRobot = "Рекрут";
                bonusAttack = 0;
                defenseCount = 1;
                defender = null;
        }
       }
        /// <summary>
        ///Класс карты Дуэлянт
        /// </summary>
        class Duelist : Robot
        {
            static int id = 2;
            
           
            public Duelist()
            {
                attack = 4;
                armor = 2;
                valueEnergy = 3;
                attackCount = 1;
                nameRobot = "Дуэлянт";
                bonusAttack = 0;
                defenseCount = 1;
                defender = null;

        }

        }
       /// <summary>
       /// Класс карты Ветеран
       /// </summary>
      class Veteran : Robot
      {
            static int id = 3;
            
         
        public Veteran()
        {
            attack = 2;
            armor = 5;
            valueEnergy = 4;
            attackCount = 1;
            nameRobot = "Ветеран";
            bonusAttack = 0;
            defenseCount = 1;
            defender = null;
        }
    }
    class Destroyer : Robot
    {
        static int id = 13;


        public Destroyer()
        {
            attack = 5;
            armor = 10;
            valueEnergy = 8;
            attackCount = 1;
            nameRobot = "Разрушитель";
            bonusAttack = 0;
            defenseCount = 1;
            defender = null;
        }
    }
    /// <summary>
    /// Класс дроида B1
    /// </summary>
    class B1 : Robot
    {
        static int id = 9;


        public B1()
        {
            attack = 1;
            armor = 1;
            valueEnergy = 2;
            attackCount = 1;
            nameRobot = "B1";
            bonusAttack = 0;
            defenseCount = 1;
            defender = null;
        }
    }

    //класс карты Боксер
    class Boxer: Robot
    {
        static int id = 10;


        public Boxer()
        {
            attack = 2;
            armor = 6;
            valueEnergy = 5;
            attackCount = 1;
            nameRobot = "Боксер";
            bonusAttack = 0;
            defenseCount = 2;
            defender = null;
        }
        public override void NewProgress()
        {
            attackCount = 1;
            defenseCount = 2;
        }
    }

    //класс карты Гладиатор
    class Gladiator : Robot
    {
        static int id = 11;


        public Gladiator()
        {
            attack = 1;
            armor = 8;
            valueEnergy = 6;
            attackCount = 3;
            nameRobot = "Гладиатор";
            defender = null;
            bonusAttack = 0;
            defenseCount = 1;
        }
        public override void NewProgress()
        {
            attackCount = 3;
            defenseCount = 1;
        }
    }

    /// <summary>
    /// Класс базового класс всех защитных сооружения
    /// </summary>
    class DefenceConstr : Robot
    {
        //общие характеристики каждой карты робота
       
        //защитиком этой карты может быть только более новая карта
        
      
        //бонус всегда равен 0
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
            nameRobot = "";
            attackCount = 0;
            bonusAttack = 0;
            defenseCount = 0;
            defender = null;
            

        }


    }

    //Класс Турели
    class Turret : DefenceConstr
    {
        //уникальный идентификатор карты
        static int id = 12;

        //описание

        public Turret()
        {
            attack = 1;
            armor = 10;
            valueEnergy = 5;
            attackCount = 1;
            nameRobot = "Турель";
            bonusAttack = 0;
            defenseCount = 1;
            defender = null;
        }
    }
    /// <summary>
    /// Класс карты Штаба
    /// </summary>
    class HeadQuarters
    {
        private int attack;
        private int attackCount;
        public int AttackCount
        {
            get {
                int count = attackCount;
                attackCount--;
                return count; }
        }
        protected int defenseCount;
        public int DefenseCount
        {
            get
            {
                int count = defenseCount;
                defenseCount--;
                return count;
            }

        }
        public void NewProgress()
        {
            attackCount = 1;
            defenseCount = 1;
        }
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
            attackCount = 1;
            defenseCount = 1;

        }
    }
}
