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
        static public int ID
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
             public virtual void NewProgress()
             {
                 attackCount = 1;
             }
            public Robot()
            {
                attack = 0;
                armor = 0;
                valueEnergy = 0;
                nameRobot = "";
                attackCount = 0;

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
                valueEnergy = 1;
                attackCount = 1;
                nameRobot = "Рекрут";
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
        public void NewProgress()
        {
            attackCount = 1;
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

        }
    }
}
