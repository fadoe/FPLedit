﻿using Eto.Forms;
using FPLedit.Bildfahrplan.Model;
using FPLedit.Shared;
using FPLedit.Shared.Rendering;
using FPLedit.Shared.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLedit.Bildfahrplan.Forms
{
    internal sealed class StationStyleForm : FDialog<DialogResult>
    {
        private readonly IPluginInterface pluginInterface;
        private readonly object backupHandle;

#pragma warning disable CS0649
        private readonly GridView gridView;
#pragma warning restore CS0649

        public StationStyleForm(IPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
            var tt = pluginInterface.Timetable;
            var attrs = new TimetableStyle(tt);
            backupHandle = pluginInterface.BackupTimetable();

            Eto.Serialization.Xaml.XamlReader.Load(this);

            var cc = new ColorCollection(pluginInterface.Settings);
            var ds = new DashStyleHelper();

            var lineWidths = Enumerable.Range(1, 5).Cast<object>().ToArray();

            gridView.AddColumn<StationStyle>(t => t.Station.SName, "Name");
            gridView.AddDropDownColumn<StationStyle>(t => t.HexColor, cc.ColorHexStrings, EtoBindingExtensions.ColorBinding(cc), "Farbe", true);
            gridView.AddDropDownColumn<StationStyle>(t => t.StationWidthInt, lineWidths, Binding.Delegate<int, string>(i => i.ToString()), "Linienstärke", true);
            gridView.AddDropDownColumn<StationStyle>(t => t.LineStyle, ds.Indices.Cast<object>(), Binding.Delegate<int, string>(i => ds.GetDescription(i)), "Linientyp", true);
            gridView.AddCheckColumn<StationStyle>(t => t.Show, "Station zeichnen", true);

            gridView.DataStore = tt.Stations.Select(s => new StationStyle(s, attrs));

            this.AddCloseHandler();
        }
        private void ResetStationStyle(bool message = true)
        {
            if (gridView.SelectedItem != null)
            {
                ((StationStyle)gridView.SelectedItem).ResetDefaults();
                gridView.ReloadData(gridView.SelectedRow);
            }
            else if (message)
                MessageBox.Show("Zuerst muss eine Station ausgewählt werden!", "Stationsdarstellung zurücksetzen");
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            pluginInterface.RestoreTimetable(backupHandle);
            this.NClose();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Ok;
            pluginInterface.ClearBackup(backupHandle);
            this.NClose();
        }

        private void ResetButton_Click(object sender, EventArgs e)
            => ResetStationStyle();
    }
}
