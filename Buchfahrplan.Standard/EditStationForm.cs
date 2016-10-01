﻿using Buchfahrplan.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buchfahrplan.Standard
{
    public partial class NewStationForm : Form
    {
        public NewStationForm()
        {
            InitializeComponent();
        }

        public void Initialize(Station station)
        {
            Text = "Station bearbeiten";
            nameTextBox.Text = station.Name;
            velocityTextBox.Text = station.MaxVelocity.ToString();
            positionTextBox.Text = station.Kilometre.ToString();
            NewStation = station;
        }

        public Station NewStation { get; set; }

        private void closeButton_Click(object sender, EventArgs e)
        {
            string name = nameTextBox.Text;
            float pos = 0f;
            int velocity = 0;
            try
            {
                pos = Convert.ToSingle(positionTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Position (km): FEHLER Die eingegebene Zeichenfolge ist kein valider Wert für eine Kommazahl!");
                return;
            }

            try
            {
                velocity = Convert.ToInt32(velocityTextBox.Text);                
            }
            catch
            {
                MessageBox.Show("Vmax: FEHLER Die eingegebene Zeichenfolge ist kein valider Wert für eine Ganzzahl!");
                return;
            }

            if (NewStation == null)
            {
                NewStation = new Station()
                {
                    Name = name,
                    Kilometre = pos,
                    MaxVelocity = velocity
                };
            }
            else
            {
                NewStation.Name = name;
                NewStation.Kilometre = pos;
                NewStation.MaxVelocity = velocity;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}