﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Buchfahrplan.Shared;

namespace Buchfahrplan
{
    public partial class TrainsEditForm : Form
    {
        private Timetable tt;
        private Timetable tt_undo;

        public TrainsEditForm()
        {
            InitializeComponent();
        }

        public void Init(Timetable tt)
        {
            this.tt = tt;
            this.tt_undo = tt;

            topFromToLabel.Text = "Züge " + tt.GetLineName(false);
            bottomFromToLabel.Text = "Züge " + tt.GetLineName(true);
            UpdateTrains();
        }

        private void UpdateTrains()
        {
            topTrainListView.Items.Clear();
            bottomTrainListView.Items.Clear();

            foreach (var train in tt.Trains.Where(o => o.Direction == false))
            {
                topTrainListView.Items.Add(new ListViewItem(new[] { 
                    train.Name, 
                    train.Line,
                    train.Locomotive }) 
                    { Tag = train });
            }

            foreach (var train in tt.Trains.Where(o => o.Direction == true))
            {
                bottomTrainListView.Items.Add(new ListViewItem(new[] { 
                    train.Name, 
                    train.Line,
                    train.Locomotive })
                    { Tag = train });
            }
        }

        private void NewEditForm_Load(object sender, EventArgs e)
        {
            topTrainListView.Columns.Add("Zugnummer");
            topTrainListView.Columns.Add("Strecke");
            topTrainListView.Columns.Add("Tfz");

            bottomTrainListView.Columns.Add("Zugnummer");
            bottomTrainListView.Columns.Add("Strecke");
            bottomTrainListView.Columns.Add("Tfz");
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            tt = tt_undo;

            Close();
        }

        private void EditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }        

        private void topNewTrainButton_Click(object sender, EventArgs e)
        {
            TrainEditForm tef = new TrainEditForm();
            tef.Initialize(false);
            DialogResult res = tef.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                Train tra = tef.NewTrain;
                foreach (var sta in tt.Stations.OrderBy(s => s.Kilometre))
                    tra.Arrivals.Add(sta, new DateTime());

                tt.Trains.Add(tra);

                UpdateTrains();
            }
        }

        private void topEditTrainButton_Click(object sender, EventArgs e)
        {
            if (topTrainListView.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)topTrainListView.Items[topTrainListView.SelectedIndices[0]];
                Train oldTrain = tt.Trains[tt.Trains.IndexOf((Train)item.Tag)];

                TrainEditForm tef = new TrainEditForm();
                tef.Initialize(oldTrain);
                DialogResult res = tef.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                    UpdateTrains();
            }
            else
                MessageBox.Show("Zuerst muss ein Zug ausgewählt werden!", "Zug bearbeiten");
        }

        private void topDeleteTrainButton_Click(object sender, EventArgs e)
        {
            if (topTrainListView.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)topTrainListView.Items[topTrainListView.SelectedIndices[0]];
                tt.Trains.Remove((Train)item.Tag);

                UpdateTrains();
            }
            else
                MessageBox.Show("Zuerst muss ein Zug ausgewählt werden!", "Zug löschen");
        }

        private void bottomNewTrainButton_Click(object sender, EventArgs e)
        {
            TrainEditForm tef = new TrainEditForm();
            tef.Initialize(true);
            DialogResult res = tef.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                Train tra = tef.NewTrain;
                foreach (var sta in tt.Stations.OrderByDescending(s => s.Kilometre))
                    tra.Arrivals.Add(sta, new DateTime());

                tt.Trains.Add(tra);

                UpdateTrains();
            }
        }

        private void bottomEditTrainButton_Click(object sender, EventArgs e)
        {
            if (bottomTrainListView.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)bottomTrainListView.Items[bottomTrainListView.SelectedIndices[0]];
                Train oldTrain = tt.Trains[tt.Trains.IndexOf((Train)item.Tag)];

                TrainEditForm tef = new TrainEditForm();
                tef.Initialize(oldTrain);
                DialogResult res = tef.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                    UpdateTrains();
            }
            else
                MessageBox.Show("Zuerst muss ein Zug ausgewählt werden!", "Zug bearbeiten");
        }

        private void bottomDeleteTrainButton_Click(object sender, EventArgs e)
        {
            if (bottomTrainListView.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)bottomTrainListView.Items[bottomTrainListView.SelectedIndices[0]];
                tt.Trains.Remove((Train)item.Tag);

                UpdateTrains();
            }
            else
                MessageBox.Show("Zuerst muss ein Zug ausgewählt werden!", "Zug löschen");
        }
    }
}