﻿using Eto.Forms;
using FPLedit.Shared;
using FPLedit.Shared.Helpers;
using FPLedit.Shared.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLedit.Editor.Network
{
    internal class TrainsEditingForm : TrainsEditorBase
    {
        private IInfo info;
        private Timetable tt;

#pragma warning disable CS0649
        private GridView gridView;
#pragma warning restore CS0649

        public TrainsEditingForm(IInfo info) : base(info.Timetable)
        {
            Eto.Serialization.Xaml.XamlReader.Load(this);

            this.info = info;
            tt = info.Timetable;
            info.BackupTimetable();

            gridView.AddColumn<Train>(t => t.TName, "Zugnummer");
            gridView.AddColumn<Train>(t => t.Locomotive, "Tfz");
            gridView.AddColumn<Train>(t => t.Mbr, "Mbr");
            gridView.AddColumn<Train>(t => t.Last, "Last");
            gridView.AddColumn<Train>(t => DaysHelper.DaysToString(t.Days, false), "Verkehrstage");
            gridView.AddColumn<Train>(t => BuildPath(t), "Laufweg");
            gridView.AddColumn<Train>(t => t.Comment, "Kommentar");

            gridView.MouseDoubleClick += (s, e) => EditTrain(gridView, TrainDirection.tr, false);

            UpdateListView(gridView, TrainDirection.tr);

            if (Eto.Platform.Instance.IsWpf)
                KeyDown += HandleKeystroke;
            else
                gridView.KeyDown += HandleKeystroke;

            this.AddCloseHandler();
            this.AddSizeStateHandler();
        }

        private void HandleKeystroke(object sender, KeyEventArgs e)
        {
            if (e.Key == Keys.Delete)
                DeleteTrain(gridView, TrainDirection.tr, false);
            else if ((e.Key == Keys.T && e.Control))
                EditTimetable(gridView, false);
            else if ((e.Key == Keys.C && e.Control))
                CopyTrain(gridView, false);
            else if ((e.Key == Keys.P && e.Control))
                EditPath(gridView, false);
            else if ((e.Key == Keys.B && e.Control) || (e.Key == Keys.Enter))
                EditTrain(gridView, TrainDirection.tr, false);
            else if (e.Key == Keys.N && e.Control)
                NewTrain(gridView);
        }

        private string BuildPath(Train t)
        {
            var path = t.GetPath();
            return path.FirstOrDefault()?.SName + " - " + path.LastOrDefault()?.SName;
        }

        private void EditTimetable(GridView view, bool message = true)
        {
            if (view.SelectedItem != null)
            {
                var train = (Train)view.SelectedItem;

                TrainTimetableEditor tte = new TrainTimetableEditor(info, train);
                tte.ShowModal(this);
            }
            else if (message)
                MessageBox.Show("Zuerst muss ein Zug ausgewählt werden!", "Zug-Fahrplan bearbeiten");
        }

        private void EditPath(GridView view, bool message = true)
        {
            if (view.SelectedItem != null)
            {
                var train = (Train)view.SelectedItem;

                var trf = new TrainChangeRouteForm(info, train);
                if (trf.ShowModal(this) == DialogResult.Ok)
                    UpdateListView(view, TrainDirection.tr);
            }
            else if (message)
                MessageBox.Show("Zuerst muss ein Zug ausgewählt werden!", "Zug-Fahrplan bearbeiten");
        }

        private void CopyTrain(GridView view, bool message = true)
        {
            if (view.SelectedItem != null)
            {
                var train = (Train)view.SelectedItem;

                var tcf = new TrainCopyDialog(train, info.Timetable);
                tcf.ShowModal(this);

                UpdateListView(view, TrainDirection.tr);
            }
            else if (message)
                MessageBox.Show("Zuerst muss ein Zug ausgewählt werden!", "Zug kopieren");
        }

        private void NewTrain(GridView view)
        {
            var trf = new TrainChangeRouteForm(info);
            if (trf.ShowModal(this) != DialogResult.Ok)
                return;

            var tef = new TrainEditForm(info.Timetable, TrainDirection.tr);
            if (tef.ShowModal(this) == DialogResult.Ok)
            {
                tef.Train.AddAllArrDeps(trf.Path);
                tt.AddTrain(tef.Train);

                UpdateListView(view, TrainDirection.tr);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            info.ClearBackup();
            Result = DialogResult.Ok;
            this.NClose();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            info.RestoreTimetable();
            this.NClose();
        }

        private void topNewButton_Click(object sender, EventArgs e)
            => NewTrain(gridView);

        private void topEditButton_Click(object sender, EventArgs e)
            => EditTrain(gridView, TrainDirection.tr);

        private void topDeleteButton_Click(object sender, EventArgs e)
            => DeleteTrain(gridView, TrainDirection.tr);

        private void editTimetableButton_Click(object sender, EventArgs e)
            => EditTimetable(gridView);

        private void copyButton_Click(object sender, EventArgs e)
            => CopyTrain(gridView);

        private void editPathButton_Click(object sender, EventArgs e)
            => EditPath(gridView);
    }
}