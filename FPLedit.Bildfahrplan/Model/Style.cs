﻿using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FPLedit.Shared.Helpers;
using FPLedit.Shared.Rendering;

namespace FPLedit.Bildfahrplan.Model
{
    internal abstract class Style
    {
        internal static IPluginInterface pluginInterface;

        private readonly Timetable _parent;

        protected readonly bool overrideEntityStyle;

        public Style(Timetable tt)
        {
            _parent = tt;
            overrideEntityStyle = pluginInterface.Settings.Get<bool>("bifpl.override-entity-styles");
        }

        protected MColor ParseColor(string def, MColor defaultValue)
            => ColorFormatter.FromString(def, defaultValue);

        protected string ColorToString(MColor color)
            => ColorFormatter.ToString(color, _parent.Version == TimetableVersion.JTG2_x);
    }
}
