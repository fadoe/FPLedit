﻿using FPLedit.Shared;
using FPLedit.Shared.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FPLedit.BuchfahrplanExport
{
    public class Plugin : IPlugin
    {
        private IInfo info;
        private ToolStripItem showItem, velocityItem;

        public string Name
        {
            get { return "Exporter für Buchfahrpläne"; }
        }

        public void Init(IInfo info)
        {
            this.info = info;
            info.FileStateChanged += Info_FileStateChanged;

            info.RegisterExport(new HtmlExport());

            ToolStripMenuItem item = new ToolStripMenuItem("Buchfahrplan");
            info.Menu.Items.AddRange(new[] { item });
            showItem = item.DropDownItems.Add("Anzeigen");
            showItem.Enabled = false;
            showItem.Click += ShowItem_Click;

            velocityItem = item.DropDownItems.Add("Höchstgeschwindigkeiten ändern");
            velocityItem.Enabled = false;
            velocityItem.Click += VelocityItem_Click;
        }

        private void VelocityItem_Click(object sender, EventArgs e)
        {
            StationVelocityForm svf = new StationVelocityForm(info);
            if (svf.ShowDialog() == DialogResult.OK)
                info.SetUnsaved();
        }

        private void Info_FileStateChanged(object sender, FileStateChangedEventArgs e)
        {
            showItem.Enabled = velocityItem.Enabled = e.FileState.Opened;
        }

        private void ShowItem_Click(object sender, EventArgs e)
        {
            HtmlExport exp = new HtmlExport();
            string path = Path.Combine(Path.GetTempPath(), "buchfahrplan.html");
            exp.Export(info.Timetable, path, new ConsoleLogger());
            Process.Start(path);
        }
    }
}
