﻿using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLedit.JTrainGraphImport
{
    public class Plugin : IPlugin
    {
        public string Name
        {
            get { return "Importer für jTrainGraph"; }
        }

        public void Init(IInfo info)
        {
            info.RegisterImport(new JTrainGraphImport());
        }
    }
}