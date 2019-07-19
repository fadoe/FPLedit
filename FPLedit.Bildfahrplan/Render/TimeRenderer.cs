﻿using FPLedit.Bildfahrplan.Model;
using System;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLedit.Bildfahrplan.Render
{
    internal class TimeRenderer
    {
        private readonly TimetableStyle attrs;
        private readonly Renderer parent;

        public TimeRenderer(TimetableStyle attrs, Renderer parent)
        {
            this.attrs = attrs;
            this.parent = parent;
        }

        public void Render(Graphics g, Margins margin, TimeSpan startTime, TimeSpan endTime, float width)
        {
            var timeFont = (Font)attrs.TimeFont; // Reminder: Do not dispose, will be disposed with MFont instance!
            using (var timeBrush = new SolidBrush((Color)attrs.TimeColor))
            using (var minutePen = new Pen((Color)attrs.TimeColor, attrs.MinuteTimeWidth))
            using (var hourPen = new Pen((Color)attrs.TimeColor, attrs.HourTimeWidth))
            {
                foreach (var l in parent.GetTimeLines(out bool hour, startTime, endTime))
                {
                    var offset = margin.Top + l * attrs.HeightPerHour / 60f;
                    g.DrawLine(hour ? hourPen : minutePen, margin.Left - 5, offset, width - margin.Right, offset); // Linie

                    var text = new TimeSpan(0, l + startTime.GetMinutes(), 0).ToString(Renderer.TIME_FORMAT);
                    var size = g.MeasureString(timeFont, text);
                    g.DrawText(timeFont, timeBrush, margin.Left - 5 - size.Width, offset - (size.Height / 2), text); // Beschriftung
                    hour = !hour;
                }
            }
        }
    }
}
