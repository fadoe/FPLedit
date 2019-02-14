﻿using FPLedit.Shared;
using FPLedit.Shared.Filetypes;
using FPLedit.Shared.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLedit.NonDefaultFiletypes
{
    internal class LinearExport : BaseConverterFileType, IExport
    {
        public string Filter => "Fahrplan Dateien (*.fpl)|*.fpl";

        public bool Export(Timetable tt, string filename, IInfo info)
        {
            if (tt.Type == TimetableType.Linear)
                throw new Exception("Der Fahrplan ist bereits ein Linear-Fahrplan");
            if (tt.GetRoutes().Count() != 1)
                throw new Exception("Der Fahrplan hat mehr als eine oder keine Strecke");

            var clone = tt.Clone();

            Dictionary<Train, PathData> trainPaths = new Dictionary<Train, PathData>();
            foreach (var orig in clone.Trains)
                trainPaths[orig] = new PathData(clone, orig);

            var route = clone.GetRoutes().FirstOrDefault().Index;

            foreach (var sta in clone.Stations)
            {
                ConvertStationNetToLin(sta, route, Timetable.DefaultLinearVersion);

                sta.RemoveAttribute("fpl-rt");
                sta.RemoveAttribute("fpl-pos");
                sta.RemoveAttribute("fpl-id");
            }

            var actions = info.GetRegistered<ITimetableTypeChangeAction>();
            foreach (var action in actions)
                action.ToLinear(clone);

            clone.SetAttribute("version", Timetable.DefaultLinearVersion.ToNumberString());

            var sortedStations = clone.GetRoutes()[Timetable.LINEAR_ROUTE_ID].GetOrderedStations();

            foreach (var t in clone.Trains)
            {
                var data = trainPaths[t];

                var sta1 = data.PathEntries.FirstOrDefault()?.Station;
                var sta2 = data.PathEntries.LastOrDefault()?.Station;

                var dir = TrainDirection.ti;
                if (sta1 != sta2)
                {
                    if (sortedStations.IndexOf(sta1) > sortedStations.IndexOf(sta2))
                        dir = TrainDirection.ta;
                }
                else if (sortedStations.IndexOf(sta1) == sortedStations.Count - 1)
                    dir = TrainDirection.ta;

                t.XMLEntity.XName = dir.ToString();

                t.Children.Clear();
                t.AddLinearArrDeps();

                foreach (var sta in data.PathEntries)
                {
                    if (sta.HasArrDep)
                        t.SetArrDep(sta.Station, sta.ArrDep.Value);
                }
            }

            ColorTimetableConverter.ConvertAll(clone);

            return new XMLExport().Export(clone, filename, info);
        }
    }
}
