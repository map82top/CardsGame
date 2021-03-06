﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CartGame
{
    public class DataGame: IDisposable
    {
        private string name;//имя игрока
        private string enName;//имя противника

        //карты которые видит игрок
        private int[] userColoda;//колода игрока
        private List<Robot> usCarteOnField;//карты на поле у игрока
        private List<Robot> enCarteOnField;//карты на поле у противника
        private List<int> carteFromUser;//ИД карты в руке у игрока
        private HeadQuarters UserHq, EnemyHq;
       
        private int countCarteEnemy;//счетчик количества карт у противника
        //и свойства доступа к ним для дальнейшей отрисовки
        public int CountCarteEnemy
         {
            get { return countCarteEnemy; }
            set { countCarteEnemy = value; }
         }
        public string UsName
        {
            get { return name; }
            set
            {
                if (name == null)
                    name = value;
            }
        }
        public string EnName
        {
            get { return enName; }
            set {
                if (enName == null)
                    enName = value;
            }
        }
        public HeadQuarters UserHQ
        {
            get { return UserHq; }
        }
        public HeadQuarters EnemyHQ
        {
            get { return EnemyHq; }
        }

        public int[] UserColoda
        {
            get { return userColoda; }
            
        }

        private int myMaxEnergy;//максимальная энергия за этот ход
        public int MyMaxEnergy
        {
            get { return myMaxEnergy; }
            set { myMaxEnergy = value; }
        }

        private int myEnergy;//энергия находящаяся в распоряжении у игрока
        public int MyEnergy
        {
            get { return myEnergy; }
            set { myEnergy = value; }
        }

        private int enMaxEnergy;//максимальная энергия за этот ход у противника
        public int EnMaxEnergy
        {
            get { return enMaxEnergy; }
            set { enMaxEnergy = value; }
        }

        private int enEnergy;//энергия находящаяся в распоряжении у противника
        public int EnEnergy
        {
            get { return enEnergy; }
            set { enEnergy = value; }
        }

        public List<Robot> UsCarteOnField
        {
            get { return usCarteOnField; }
        }
        public List<Robot> EnCarteOnField
        {
            get { return enCarteOnField; }
        }
        public List<int> CarteFromUser
        {
            get { return carteFromUser; }
        }
      
        public DataGame()
        {
            name = null;
            enName = null;
            userColoda = new int[15];//максимальное количество карт в колоде игрока 15
            usCarteOnField = null;
            enCarteOnField = null;
            carteFromUser = null;
            UserHq = null;
            EnemyHq = null;
            CountCarteEnemy = 0;
            

        }
        public void InicializeListCards()
        {
            usCarteOnField = new List<Robot>();
            enCarteOnField = new List<Robot>();
            carteFromUser = new List<int>();
            UserHq = new HeadQuarters();
            EnemyHq = new HeadQuarters();
          


        }
        //освобождает все ресурсы
        public void Dispose()
        {
            enName = null;
            usCarteOnField = null;
            carteFromUser = null;
            enCarteOnField = null;
            UserHq = null;
            EnemyHq = null;
           
        }
    }
}
