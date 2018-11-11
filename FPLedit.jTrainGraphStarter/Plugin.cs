﻿using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FPLedit.jTrainGraphStarter
{
    [Plugin("Starter für jTrainGraph", "1.5.0", Author = "Manuel Huber")]
    public class Plugin : IPlugin
    {
        IInfo info;
        ToolStripItem startItem, settingsItem;

        public void Init(IInfo info)
        {
            this.info = info;
            info.FileStateChanged += Info_FileStateChanged;

            var item = new ToolStripMenuItem("jTrainGraph");
            info.Menu.Items.Add(item);

            startItem = item.DropDownItems.Add("jTrainGraph Starten");
            startItem.Enabled = false;
            startItem.Click += (s, e) => Start();

            settingsItem = item.DropDownItems.Add("Einstellungen");
            settingsItem.Click += (s, e) => (new SettingsForm(info.Settings)).ShowDialog();
        }

        private void Info_FileStateChanged(object sender, FileStateChangedEventArgs e)
        {
            startItem.Enabled = e.FileState.Opened;
        }

        public void Start()
        {
            bool showMessage = info.Settings.Get("jTGStarter.show-message", true);

            if (showMessage)
            {
                DialogResult res = MessageBox.Show("Dies speichert die Fahrplandatei am letzten Speicherort und öffnet dann jTrainGraph (>= 2.02). Nachdem Sie die Arbeit in jTrainGraph beendet haben, speichern Sie damit die Datei und schließen das jTrainGraph-Hauptfenster, damit werden die Änderungen übernommen. Aktion fortsetzen?"+Environment.NewLine+Environment.NewLine+"Diese Meldung kann unter jTrainGraph > Einstellungen deaktiviert werden.",
                    "jTrainGraph starten", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (res != DialogResult.Yes)
                    return;
            }

            info.Save(false);

            string javapath = info.Settings.Get("jTGStarter.javapath", "java");
            string jtgPath = info.Settings.Get("jTGStarter.jtgpath", "jTrainGraph_203.jar");

            string jtgFolder = Path.GetDirectoryName(jtgPath);

            Process p = new Process();
            p.StartInfo.WorkingDirectory = jtgFolder;
            p.StartInfo.FileName = javapath;
            p.StartInfo.Arguments = "-jar \"" + jtgPath + "\" \"" + info.FileState.FileName + "\"";

            try
            {
                p.Start();
                info.Logger.Info("Wartet darauf, dass jTrainGraph beendet wird...");
                p.WaitForExit();
                info.Logger.Info("jTrainGraph beendet! Lade Datei neu...");

                info.Reload();
            }
            catch (Exception e)
            {
                info.Logger.Error("jTrainGraphStarter: " + e.Message);
                info.Logger.Error("Möglicherweise ist das jTrainGraphStarter Plugin falsch konfiguriert! Zur Konfiguration siehe jTrainGraph > Einstellungen");
            }
        }
    }
}
