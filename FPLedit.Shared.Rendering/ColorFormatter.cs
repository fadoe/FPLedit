﻿using Eto.Drawing;
using System;
using System.Collections.Generic;

namespace FPLedit.Shared.Rendering
{
    /// <summary>
    /// Konvertiert Farbangeben in das im Dateiformat übliche string-basierte Format.
    /// </summary>
    public static class ColorFormatter
    {
        #region Convert to string
        private static string ToHexString(MColor c)
            => string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);

        private static string ToJtg2CustomColor(MColor c)
            => "c(" + c.R + "," + c.G + "," + c.B + ")";

        public static string ToString(MColor c, bool useJtg2Format = false)
            => useJtg2Format ? ToJtg2CustomColor(c) : ToHexString(c);
        #endregion

        #region Convert from string
        public static MColor FromHexString(string hex)
        {
            if (hex.Length != 7 || hex[0] != '#')
                return null;

            return (MColor)Color.FromArgb(int.Parse(hex.Substring(1), System.Globalization.NumberStyles.HexNumber));
        }

        private static MColor FromJtg2CustomColor(string jtg2)
        {
            var parts = jtg2.Substring(2, jtg2.Length - 3).Split(',');
            return new MColor(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]));
        }

        private static readonly Dictionary<string, MColor> jtraingraphColors = new Dictionary<string, MColor>()
        {
            ["schwarz"] = (MColor)Colors.Black,
            ["grau"] = (MColor)Colors.Gray,
            ["weiß"] = (MColor)Colors.White,
            ["rot"] = (MColor)Colors.Red,
            ["orange"] = (MColor)Colors.Orange,
            ["gelb"] = (MColor)Colors.Yellow,
            ["blau"] = (MColor)Colors.Blue,
            ["hellblau"] = (MColor)Colors.LightBlue,
            ["grün"] = (MColor)Colors.Green,
            ["dunkelgrün"] = (MColor)Colors.DarkGreen,
            ["braun"] = (MColor)Colors.Brown,
            ["magenta"] = (MColor)Colors.Magenta,
        };

        public static MColor FromString(string def, MColor defaultValue)
        {
            if (def == null)
                return defaultValue;

            if (def.StartsWith("#"))
                return FromHexString(def);

            if (def.StartsWith("c(") && def.EndsWith(")"))
                return FromJtg2CustomColor(def);

            if (jtraingraphColors.ContainsKey(def))
                return jtraingraphColors[def];

            return defaultValue;
        }
        #endregion
    }
}
