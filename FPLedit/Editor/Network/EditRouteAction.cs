﻿using Eto.Forms;
using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit.Editor.Network
{
    internal sealed class EditRouteAction : IRouteAction
    {
        public string DisplayName => "Stationen dieser Strecke bearbeiten";

        public bool IsEnabled(IPluginInterface pluginInterface)
            => pluginInterface.FileState.Opened;

        public void Invoke(IPluginInterface pluginInterface, Route route)
        {
            pluginInterface.StageUndoStep();
            using (var lef = new LineEditForm(pluginInterface, route.Index))
                if (lef.ShowModal(pluginInterface.RootForm) == DialogResult.Ok)
                    pluginInterface.SetUnsaved();
        }
    }
}
