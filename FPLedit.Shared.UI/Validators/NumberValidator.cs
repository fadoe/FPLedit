﻿using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit.Shared.UI.Validators
{
    public sealed class NumberValidator : BaseValidator
    {
        public bool AllowEmpty { get; set; }

        public bool UseInt { get; set; }

        public NumberValidator(TextBox control, bool allowEmpty, bool useInt, bool enableErrorColoring = true, string errorMessage = null)
            : base(control, true, enableErrorColoring, errorMessage)
        {
            AllowEmpty = allowEmpty;
            UseInt = useInt;
        }

        protected override bool IsValid()
        {
            if (AllowEmpty && Control.Text == "")
                return true;
            if (!UseInt)
                return float.TryParse(Control.Text, out var _);
            return int.TryParse(Control.Text, out var _);
        }
    }
}
