using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CarteServer
{
    //Эта иерархия классов отличается от тех, что находятся в клиенте у пользователя
        abstract class Carte
        {
           // public abstract Panel ImageCartNormal();
            //public abstract Panel ImageCartMin();
            //public abstract Panel ImageCartMax();
            // public abstract Panel Clone { get; }
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
                    default:
                        RetCarte = null;
                        break;
                }
                return RetCarte;
            }
        }
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
        class Recruit : Robot
        {
            //уникальный идентификатор карты
            static int id = 1;
            static public int ID
            {
                get { return id; }
            }
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
        class Duelist : Robot
        {
            static int id = 2;
            static public int ID
            {
                get { return id; }
            }
           
            public Duelist()
            {
                attack = 4;
                armor = 2;
                valueEnergy = 3;
                attackCount = 1;
                nameRobot = "Дуэлянт";
           
           }
    }
        class Veteran : Robot
        {
            static int id = 3;
            static public int ID
            {
                get { return id; }
            }
         
        public Veteran()
        {
            attack = 3;
            armor = 5;
            valueEnergy = 4;
            attackCount = 1;
            nameRobot = "Ветеран";
        }
    }
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
