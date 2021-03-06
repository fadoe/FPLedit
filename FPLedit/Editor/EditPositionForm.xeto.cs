﻿using Eto.Forms;
using FPLedit.Shared;
using FPLedit.Shared.UI;
using FPLedit.Shared.UI.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLedit.Editor
{
    internal sealed class EditPositionForm : FDialog<DialogResult>
    {
#pragma warning disable CS0649
        private readonly TextBox positionTextBox;
#pragma warning restore CS0649
        private readonly NumberValidator positionValidator;

        public float Position { get; private set; }

        public EditPositionForm()
        {
            Eto.Serialization.Xaml.XamlReader.Load(this);

            positionValidator = new NumberValidator(positionTextBox, false, false, errorMessage: "Bitte eine Zahl als Position eingeben!");
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            if (!positionValidator.Valid)
            {
                MessageBox.Show("Bitte erst alle Fehler beheben:" + Environment.NewLine + positionValidator.ErrorMessage);
                return;
            }

            Position = float.Parse(positionTextBox.Text);

            Close(DialogResult.Ok);
        }

        private void CancelButton_Click(object sender, EventArgs e)
            => Close(DialogResult.Cancel);
    }
}
