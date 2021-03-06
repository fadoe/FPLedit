﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit.Shared
{
    public interface IRouteAction
    {
        string DisplayName { get; }

        void Invoke(IPluginInterface pluginInterface, Route route);

        bool IsEnabled(IPluginInterface pluginInterface);
    }
}
