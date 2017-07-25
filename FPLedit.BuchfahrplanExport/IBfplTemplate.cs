﻿using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit.Buchfahrplan
{
    public interface IBfplTemplate
    {
        string Name { get; }
        string GetResult(Timetable tt);
    }
}
