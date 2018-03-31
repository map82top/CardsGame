﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CartGame
{
    public enum MsgType
    {
        StartSession,
        DeliteSeek,
        CarteUser,
        GetName,
        StartGame,
        AddUserCarte,
        AddEnemyCarte,
        UserMaxEnergy,
        YourEnergy,
        EnemyMaxEnergy,
        EnemyEnergy,
        ProgressTime,
        MyProgress,
        EnemyProgress,
        AddCarteOnField,
        EnemyAddCarteOnField,
        Attack,
        MyAttackSucc,
        EnAttackSucc, 
        EndProgress



    }
    public abstract class Carte
    {
        public abstract Panel ImageCartNormal();
        public abstract Panel ImageCartMin();
        public abstract Panel ImageCartMax();
        public abstract Panel ImageCartFullMin();

        // public abstract Panel Clone { get; }
        static public Carte GetCarte(int ID)
        {
            Carte RetCarte;
            switch (ID)
            {
                case 1:
                    RetCarte = new Recruit(Properties.Resources.DescrRecruit);
                    break;
                case 2:
                    RetCarte = new Duelist(Properties.Resources.DescrDuelist);
                    break;
                case 3:
                    RetCarte = new Veteran(Properties.Resources.DescrVeteran);
                    break;
                default:
                    RetCarte = null;
                    break;
            }
            return RetCarte;
        }

    }
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

        protected int valueEnergy;
        public int ValueEnergy
        {
            get { return valueEnergy; }
        }

        protected string description;
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
            description = "";
            nameRobot = "";

        }
        public string Description
        {
            get { return description; }
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
            Descr.Text = description;
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

            return CarteImage;

        }
        public override Panel ImageCartMin()
        {
            Panel CarteImage = new Panel();
            CarteImage.Size = new Size(85, 120);

            //отображаем атаку
            Label attackLabel = new Label();
            attackLabel.Text = Convert.ToString(attack);
            attackLabel.Font = new Font("Arial", 9);
            attackLabel.Location = new Point(10, 87);
            attackLabel.Size = new Size(12, 12);
            CarteImage.Controls.Add(attackLabel);


            //отображаем очки здоровья
            Label armorLabel = new Label();
            armorLabel.Text = Convert.ToString(armor);
            armorLabel.Font = new Font("Arial", 9);
            armorLabel.Location = new Point(59, 87);
            armorLabel.Size = new Size(12, 12);
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

            return CarteImage;
        }


    }
   public class Recruit : Robot
    {
        //уникальный идентификатор карты
        static int id = 1;
        static public int ID
        {
            get { return id; }
        }
        //описание
        public Recruit(string Description)
        {
            attack = 2;
            armor = 5;
            valueEnergy = 2;
            description = Description;
            nameRobot = "Рекрут";
        }
        public Recruit()
        {
            attack = 2;
            armor = 5;
            valueEnergy = 2;

            nameRobot = "Рекрут";
        }


    }
   public class Duelist : Robot
    {
        static int id = 2;
        static public int ID
        {
            get { return id; }
        }
        public Duelist(string Description)
        {
            attack = 4;
            armor = 2;
            valueEnergy = 3;
            description = Description;
            nameRobot = "Дуэлянт";

        }
        public Duelist()
        {
            attack = 4;
            armor = 2;
            valueEnergy = 3;
            nameRobot = "Дуэлянт";

        }
    }
   public class Veteran : Robot
    {
        static int id = 3;
        static public int ID
        {
            get { return id; }
        }
        public Veteran(string Description)
        {
            attack = 3;
            armor = 5;
            valueEnergy = 4;
            description = Description;
            nameRobot = "Ветеран";
        }
        public Veteran()
        {
            attack = 3;
            armor = 5;
            valueEnergy = 4;

            nameRobot = "Ветеран";
        }

    }
   public class HeadQuarters
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

        }
        public Panel ImageCartMin()
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

            return CarteImage;
        }
    }
}
