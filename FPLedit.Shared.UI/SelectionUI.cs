﻿using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLedit.Shared.UI
{
    public sealed class SelectionUI<T> : IDisposable where T : Enum
    {
        private readonly Action<T> selectedEvent;

        private readonly ActionInfo[] actions;

        private ActionInfo selectedState;

        public T SelectedState => selectedState.Value;

        public bool EnabledOptionSelected => selectedState.Enabled;

        public SelectionUI(Action<T> selectedEvent, StackLayout st)
        {
            this.selectedEvent = selectedEvent;

            actions = GetActions(typeof(T)).ToArray();

            if (actions.Length == 0)
                return;

            RadioButton master = null;

            for (int i = 0; i < actions.Length; i++)
            {
                var ac = actions[i];
                var rb = new RadioButton(master)
                {
                    Text = ac.Name,
                    Checked = i == 0
                };
                if (master == null)
                    master = rb;

                rb.CheckedChanged += (s, e) =>
                {
                    if (rb.Checked)
                        InternalSelect(ac.Value);
                };

                st.Items.Add(rb);
                ac.RadioButton = rb;
            }
        }

        private IEnumerable<ActionInfo> GetActions(Type type)
        {
            var values = type.GetEnumValues();

            foreach (T val in values)
            {
                var memInfo = type.GetMember(val.ToString());
                var mem = memInfo.FirstOrDefault(m => m.DeclaringType == type);
                var attributes = mem.GetCustomAttributes(typeof(SelectionNameAttribute), false);
                var name = ((SelectionNameAttribute) attributes.FirstOrDefault())?.Name;
                yield return new ActionInfo(val, name);
            }
        }

        public void DisableOption(T option)
        {
            var state = GetState(option);
            state.RadioButton.Enabled = false;
        }

        public void ChangeSelection(T option)
        {
            var state = GetState(option);
            state.RadioButton.Checked = true;
        }

        private void InternalSelect(T option)
        {
            selectedState = GetState(option);
            selectedEvent?.Invoke(option);
        }

        public void Dispose()
        {
            foreach (var rb in actions)
                if (rb.RadioButton != null && !rb.RadioButton.IsDisposed)
                    rb.RadioButton.Dispose();
        }

        private ActionInfo GetState(T value) => actions.First(v => v.Value.Equals(value));

        private class ActionInfo
        {
            public readonly string Name;
            public T Value;
            public readonly bool Enabled;
            public RadioButton RadioButton;

            public ActionInfo(T value, string name)
            {
                Name = name;
                Value = value;
                Enabled = true;
                RadioButton = null;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SelectionNameAttribute : Attribute
    {
        private readonly string name;

        public string Name => name;

        public SelectionNameAttribute(string name)
        {
            this.name = name;
        }
    }
}