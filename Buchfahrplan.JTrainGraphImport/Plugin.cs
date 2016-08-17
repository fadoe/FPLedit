﻿using Buchfahrplan.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buchfahrplan.JTrainGraphImport
{
    public class Plugin : IPlugin
    {
        public void Init(IInfo info)
        {
            info.RegisterImport(new JTrainGraphImport());
        }
    }
}
