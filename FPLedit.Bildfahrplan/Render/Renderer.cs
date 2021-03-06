﻿using FPLedit.Bildfahrplan.Model;
using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLedit.Bildfahrplan.Render
{
    internal sealed class Renderer
    {
        private readonly Timetable tt;
        private readonly int route;

        private Margins margin = new Margins(10, 20, 20, 20);
        internal float width = 0, height = 0;

        private readonly TimetableStyle attrs;

        public Renderer(Timetable timetable, int route)
        {
            tt = timetable;
            this.route = route;
            attrs = new TimetableStyle(tt);
        }

        public void Draw(Graphics g, bool drawHeader)
            => Draw(g, attrs.StartTime, attrs.EndTime, drawHeader);

        public void Draw(Graphics g, TimeEntry startTime, TimeEntry endTime, bool drawHeader)
        {
            g.Clear((Color)attrs.BgColor);

            var stations = tt.GetRoute(route).Stations;

            if (!margin.Calced)
                margin = CalcMargins(g, margin, stations, startTime, endTime, drawHeader);
            margin.Calced = true;

            if (width == 0) width = g.ClipBounds.Width;
            if (height == 0) height = GetHeight(startTime, endTime, drawHeader);

            // Zeitaufteilung
            var timeRenderer = new TimeRenderer(attrs, this);
            timeRenderer.Render(g, margin, startTime, endTime, width);

            // Stationenaufteilung
            var headerRenderer = new HeaderRenderer(stations, attrs, route);
            var stationOffsets = headerRenderer.Render(g, margin, width, height, drawHeader);

            // Züge
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var clip = g.ClipBounds;
            clip.Y += margin.Top;
            clip.Height -= (margin.Top + margin.Bottom + 1);
            g.SetClip(clip);

            var trains = tt.Trains.Where(t =>
            {
                for (int i = 0; i < 6; i++)
                    if (attrs.RenderDays[i] && t.Days[i]) return true;
                return false;
            });
            var trainRenderer = new TrainRenderer(stations, tt, margin, startTime, stationOffsets);
            foreach (var train in trains)
                trainRenderer.Render(g, train);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        }

        public void DrawHeader(Graphics g, int width)
        {
            g.Clear((Color)attrs.BgColor);

            var stations = tt.GetRoute(route).Stations;

            var margin = CalcMargins(g, new Margins(11, 20, 20, 0), stations, attrs.StartTime, attrs.EndTime, true);

            var height = GetHeight(default, default, true);

            // Stationenaufteilung
            var headerRenderer = new HeaderRenderer(stations, attrs, route);
            headerRenderer.Render(g, margin, width, height, true);
        }

        internal Margins CalcMargins(Graphics g, Margins orig, IEnumerable<Station> stations, TimeEntry startTime, TimeEntry endTime, bool drawHeader)
        {
            if (orig.Calced)
                return orig;

            var stationFont = (Font)attrs.StationFont;
            var timeFont = (Font)attrs.TimeFont;

            var result = orig;
            // MarginTop berechnen
            float sMax = 0f;
            var emSize = g.MeasureString(stationFont, "M").Height;
            if (attrs.StationVertical)
                sMax = stations.Max(sta => g.MeasureString(sta.ToString(attrs.DisplayKilometre, route), stationFont).Width);
            else
                sMax = emSize;

            if (attrs.MultiTrack)
                sMax += emSize;

            result.Top = attrs.DrawHeader ? sMax + result.Top : result.Top;
            result.Top = drawHeader ? result.Top : 5;

            // MarginLeft berechnen
            float tMax = 0f;
            foreach (var l in GetTimeLines(out bool _, startTime, endTime))
                tMax = Math.Max(tMax, g.MeasureString(new TimeEntry(0, l + startTime.GetTotalMinutes()).ToShortTimeString(), timeFont).Width);
            result.Left += tMax;
            return result;
        }

        internal List<int> GetTimeLines(out bool hourStart, TimeEntry start, TimeEntry end)
        {
            List<int> lines = new List<int>();
            int minutesToNextLine = 60 - start.Minutes;
            if (minutesToNextLine == 60)
                lines.Add(0);
            if (minutesToNextLine >= 30)
                lines.Add(minutesToNextLine - 30);
            hourStart = lines.Count != 1;
            int min = 0;
            while (true)
            {
                min += minutesToNextLine;
                if (min > end.GetTotalMinutes() - start.GetTotalMinutes())
                    break;
                lines.Add(min);
                minutesToNextLine = 30;
            }
            return lines;
        }

        public int GetHeight(TimeEntry start, TimeEntry end, bool drawHeader)
        {
            var stations = tt.GetRoute(route).Stations;
            using (var image = new Bitmap(1, 1, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(image))
            {
                var m = CalcMargins(g, margin, stations, start, end, drawHeader);
                return (int)(m.Top + m.Bottom + (end - start).GetTotalMinutes() * attrs.HeightPerHour / 60f);
            }
        }

        public int GetHeight(bool drawHeader)
            => GetHeight(attrs.StartTime, attrs.EndTime, drawHeader);
    }
}
