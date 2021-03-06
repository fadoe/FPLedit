﻿using Eto.Forms;
using FPLedit.Shared;
using FPLedit.Shared.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit.Editor
{
    //TODO: Here is more refactoring needed.
    internal sealed class EditorPlugin : IPlugin
    {
        private IPluginInterface pluginInterface;

        private int dialogOffset;
        private IEditMenuItemAction[] editMenuActions;
        private bool hasFilterables, hasDesignables;

        private ButtonMenuItem editLineItem, editTimetableItem; // Type="Network"
        private ButtonMenuItem editTrainsItem, designItem, filterItem; // Type="Both"
        private ButtonMenuItem editRoot, previewRoot, undoItem; // Type="Both"

        public void Init(IPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
            pluginInterface.FileStateChanged += PluginInterface_FileStateChanged;
            pluginInterface.ExtensionsLoaded += PluginInterface_ExtensionsLoaded;

            pluginInterface.Register<IExport>(new FPLedit.NonDefaultFiletypes.CleanedXmlExport());
            pluginInterface.Register<ITimetableCheck>(new FPLedit.TimetableChecks.TransitionsCheck());
            pluginInterface.Register<ITimetableCheck>(new FPLedit.TimetableChecks.DayOverflowCheck());
            pluginInterface.Register<ITimetableInitAction>(new FPLedit.TimetableChecks.UpdateColorsAction());
            pluginInterface.Register<ITimetableInitAction>(new FPLedit.TimetableChecks.FixNetworkAttributesAction());

            if (Environment.OSVersion.Platform != PlatformID.Win32NT || pluginInterface.Settings.Get<bool>("mp-compat.route-edit-button"))
                pluginInterface.Register<IRouteAction>(new Network.EditRouteAction());

            editRoot = (ButtonMenuItem)((MenuBar)pluginInterface.Menu).GetItem(MainForm.LocEditMenu);

            undoItem = editRoot.CreateItem("Rückgängig", enabled: false, clickHandler: (s, e) => pluginInterface.Undo());
            undoItem.Shortcut = Keys.Control | Keys.Z;

            editRoot.Items.Add(new SeparatorMenuItem());

            editLineItem = editRoot.CreateItem("Strecke bearbeiten (linear)", enabled: false,
                clickHandler: (s, e) => ShowForm(new LineEditForm(pluginInterface, Timetable.LINEAR_ROUTE_ID)));

            editTrainsItem = editRoot.CreateItem("Züge bearbeiten", enabled: false);
            editTrainsItem.Click += (s, e) =>
            {
                if (pluginInterface.Timetable.Type == TimetableType.Linear)
                    ShowForm(new Linear.LinearTrainsEditForm(pluginInterface));
                else ShowForm(new Network.NetworkTrainsEditForm(pluginInterface));
            };

            editTimetableItem = editRoot.CreateItem("Fahrplan bearbeiten", enabled: false);
            editTimetableItem.Click += (s, e) =>
            {
                if (pluginInterface.Timetable.Type == TimetableType.Linear)
                    ShowForm(new Linear.LinearTimetableEditForm(pluginInterface));
                else ShowForm(new Network.MultipleTimetableEditForm(pluginInterface));
            };

            editRoot.Items.Add(new SeparatorMenuItem());

            designItem = editRoot.CreateItem("Fahrplandarstellung", enabled: false, clickHandler: (s, e) => ShowForm(new RenderSettingsForm(pluginInterface)));
            filterItem = editRoot.CreateItem("Filterregeln", enabled: false, clickHandler: (s, e) => ShowForm(new Filters.FilterForm(pluginInterface)));

            previewRoot = (ButtonMenuItem)((MenuBar)pluginInterface.Menu).GetItem(MainForm.LocPreviewMenu);
        }

        private void ShowForm(Dialog<DialogResult> form)
        {
            pluginInterface.StageUndoStep();
            if (form.ShowModal(Program.App.MainForm) == DialogResult.Ok)
                pluginInterface.SetUnsaved();
            if (!form.IsDisposed)
                form.Dispose();
        }

        private void PluginInterface_ExtensionsLoaded(object sender, EventArgs e)
        {
            var previewables = pluginInterface.GetRegistered<IPreviewAction>();
            if (previewables.Length == 0)
                pluginInterface.Menu.Items.Remove(previewRoot); // Ausblenden in der harten Art

            foreach (var prev in previewables)
                previewRoot.CreateItem(prev.DisplayName, enabled: false, clickHandler: (s, ev) => prev.Show(pluginInterface));

            editMenuActions = pluginInterface.GetRegistered<IEditMenuItemAction>();
            if (editMenuActions.Length > 0)
                editRoot.Items.Add(new SeparatorMenuItem());

            dialogOffset = editRoot.Items.Count;
            foreach (var dialog in editMenuActions)
                editRoot.CreateItem(dialog.DisplayName, enabled: dialog.IsEnabled(pluginInterface), clickHandler: (s, ev) => dialog.Invoke(pluginInterface));

            hasFilterables = pluginInterface.GetRegistered<IFilterRuleContainer>().Length > 0;
            hasDesignables = pluginInterface.GetRegistered<IAppearanceControl>().Length > 0;
        }

        private void PluginInterface_FileStateChanged(object sender, FileStateChangedEventArgs e)
        {
            editLineItem.Enabled = e.FileState.Opened;
            editTrainsItem.Enabled = e.FileState.Opened && e.FileState.LineCreated;
            editTimetableItem.Enabled = e.FileState.Opened && e.FileState.LineCreated && e.FileState.TrainsCreated;
            designItem.Enabled = e.FileState.Opened && hasDesignables;
            filterItem.Enabled = e.FileState.Opened && hasFilterables;
            undoItem.Enabled = e.FileState.CanGoBack;

            foreach (ButtonMenuItem ddi in previewRoot.Items)
                ddi.Enabled = e.FileState.Opened && e.FileState.LineCreated;

            for (int i = 0; i < editMenuActions.Length; i++)
            {
                var elem = editRoot.Items[dialogOffset + i];
                elem.Enabled = editMenuActions[i].IsEnabled(pluginInterface);
            }

            // Im Netzwerk-Modus nicht verwendete Menü-Einträge ausblenden
            if (pluginInterface.Timetable != null)
                editLineItem.Enabled = pluginInterface.Timetable.Type != TimetableType.Network;
        }
    }
}
