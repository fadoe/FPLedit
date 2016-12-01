﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FPLedit.Shared
{
    [Serializable]
    public sealed class Timetable : Meta
    {
        public const string MAGIC = "BFPL/1.1";

        public string Name { get; set; }

        public List<Station> Stations { get; set; }

        public List<Train> Trains { get; set; }        

        public Timetable() : base()
        {
            Stations = new List<Station>();
            Trains = new List<Train>();

            Name = "";
        }

        public static Timetable GenerateTestTimetable()
        {
            Timetable t = new Timetable();
            t.Name = "ATal - BTal";
            Station a = new Station() { Name = "ATal", Kilometre = 0.0f };
            Station b = new Station() { Name = "BTal", Kilometre = 1.0f };
            Train tr = new Train() { Name = "P 01", Locomotive = "211" };
            tr.Line = "ATal - BTal";
            tr.Direction = true;
            tr.Arrivals = new Dictionary<Station, TimeSpan>();
            tr.Departures = new Dictionary<Station, TimeSpan>();
            tr.Arrivals.Add(b, DateTime.Now.TimeOfDay);
            tr.Departures.Add(a, DateTime.Now.TimeOfDay);

            Train t2 = new Train() { Name = "P 01", Locomotive = "211" };
            t2.Arrivals = new Dictionary<Station, TimeSpan>();
            t2.Departures = new Dictionary<Station, TimeSpan>();
            t2.Arrivals.Add(b, DateTime.Now.TimeOfDay);
            t2.Departures.Add(a, DateTime.Now.TimeOfDay);

            t.Stations = new List<Station>();
            t.Trains = new List<Train>();
            t.Stations.Add(b);
            t.Stations.Add(a);
            t.Trains.Add(tr);
            t.Trains.Add(t2);

            return t;
        }

        public static Timetable Deserialize(BinaryReader reader)
        {
            Timetable res = new Timetable();

            string magic = reader.ReadString();
            if (magic != MAGIC)
                throw new Exception("Ein Fehler ist beim Öffnen der Datei aufgetreten: Falsche Dateiversion");

            res.Name = reader.ReadString();
            res.Metadata = DeserializeMeta(reader);

            var stations = new Dictionary<int, Station>();
            int sta_count = reader.ReadInt32();
            for (int i = 0; i < sta_count; i++)
            {
                int id = reader.ReadInt32();
                var sta = Station.Deserialize(reader);
                stations.Add(id, sta);
                res.Stations.Add(sta);
            }

            int tra_count = reader.ReadInt32();
            for (int i = 0; i < tra_count; i++)
            {
                var tr = Train.Deserialize(reader, stations);
                res.Trains.Add(tr);
            }

            return res;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(MAGIC);
            Name = "";
            writer.Write(Name);
            SerializeMeta(writer);

            var stations = new Dictionary<Station, int>();
            writer.Write(Stations.Count);
            int i = 0;
            foreach (var sta in Stations)
            {
                writer.Write(i); // STA_UNIQ_ID
                sta.Serialize(writer);
                stations.Add(sta, i);
                i++;
            }

            writer.Write(Trains.Count);
            foreach (var tra in Trains)
                tra.Serialize(writer, stations);
        }

        public void SaveToStream(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
                Serialize(writer);
        }

        public static Timetable OpenFromStream(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
                return Deserialize(reader);
        }

        public List<Station> GetStationsOrderedByDirection(bool direction)
        {
            return (direction ?
                Stations.OrderByDescending(s => s.Kilometre)
                : Stations.OrderBy(s => s.Kilometre)).ToList();
        }

        public string GetLineName(bool direction)
        {
            string first = GetStationsOrderedByDirection(direction).First().Name;
            string last = GetStationsOrderedByDirection(direction).Last().Name;

            return first + " - " + last;
        }

        public override string ToString()
        {
            return GetLineName(true);
        }

        public Timetable Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return (Timetable)formatter.Deserialize(stream);
            }
        }
    }
}