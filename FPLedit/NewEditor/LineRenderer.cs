﻿using FPLedit.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FPLedit
{
    internal class LineRenderer : Control
    {
        private Timetable tt;
        private List<Panel> panels = new List<Panel>();
        private Font font;
        private Pen linePen;

        private int _selectedRoute = 0;
        public int SelectedRoute
        {
            get => _selectedRoute;
            set { _selectedRoute = value; this.Invalidate(); }
        }

        private StaPosHandler handler;
        private Dictionary<Station, Point> stapos;
        private List<Route> routes;

        public event EventHandler<MouseEventArgs> StationClicked;
        public event EventHandler<MouseEventArgs> StationDoubleClicked;

        private const int OFFSET_X = 20;
        private const int OFFSET_Y = 50;

        public LineRenderer()
        {
            this.DoubleBuffered = true;
            font = new Font(Font.FontFamily, 8);
            linePen = new Pen(Color.Black, 2.0f);
            handler = new StaPosHandler();
        }

        public void SetLine(Timetable tt)
        {
            this.tt = tt;
            if (tt == null)
            {
                routes = null;
                this.Invalidate();
                return;
            }

            routes = tt.GetRoutes();
            if (tt.Type == TimetableType.Linear)
                stapos = handler.GenerateLinearPoints(tt, ClientSize.Width);
            else
                stapos = handler.LoadNetworkPoints(tt);

            this.Invalidate();
        }

        public void UpdateLine()
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.SuspendLayout();
            e.Graphics.Clear(Color.White);
            foreach (var p in panels)
                this.Controls.Remove(p);
            panels.Clear();

            if (routes == null || routes.Count == 0)
                return;

            var xOffset = OFFSET_X;
            var yOffset = OFFSET_Y;

            foreach (var r in routes)
            {
                var pen = linePen;
                if (r.Index == SelectedRoute)
                    pen = new Pen(Color.Red, 2);
                Point? lastP = null;
                foreach (var sta in r.GetOrderedStations())
                {
                    var pos = stapos[sta];
                    var x = xOffset + pos.X;
                    var y = yOffset + pos.Y;

                    var cont = e.Graphics.BeginContainer();
                    e.Graphics.TranslateTransform(x + 6, y + 7);
                    e.Graphics.RotateTransform(60);

                    var km = sta.Positions.GetPosition(r.Index).Value;
                    var text = sta.SName + " (" + km.ToString("0.0") + ")";
                    e.Graphics.DrawString(text, font, Brushes.Black, new Point(0, 0));

                    e.Graphics.EndContainer(cont);

                    var p = new Panel()
                    {
                        Location = new Point(x - 5, y - 5),
                        Size = new Size(10, 10),
                        BackColor = Color.Gray,
                        Tag = sta,
                    };
                    p.MouseDoubleClick += (s, args) => StationDoubleClicked?.Invoke(sta, args);
                    p.MouseClick += (s, args) => StationClicked?.Invoke(sta, args);

                    // Drag'n'Drop-Events
                    p.MouseDown += (s, args) =>
                    {
                        draggedControl = (Control)s;
                        Cursor.Current = Cursors.SizeAll;
                    };
                    p.MouseMove += (s, args) => this.OnMouseMove(args);
                    p.MouseUp += (s, args) => this.OnMouseUp(args);
                    Controls.Add(p);
                    panels.Add(p);

                    if (lastP.HasValue)
                        e.Graphics.DrawLine(pen, x, y, xOffset + lastP.Value.X, yOffset + lastP.Value.Y);
                    lastP = pos;
                }
            }

            this.ResumeLayout();
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
            base.OnResize(e);
        }

        #region Drag'n'Drop
        private Control draggedControl;
        private bool hasDragged = false;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (draggedControl != null)
            {
                var p = PointToClient(MousePosition);

                //Not getting out of bounds
                if (p.X < 0)
                    p.X = 0;
                if (p.Y < 0)
                    p.Y = 0;
                if (p.X > ClientSize.Width)
                    p.X = ClientSize.Width;
                if (p.Y > ClientSize.Height)
                    p.Y = ClientSize.Height;

                draggedControl.Location = p;
                stapos[(Station)draggedControl.Tag] = new Point(p.X - OFFSET_X, p.Y - OFFSET_Y);
                hasDragged = true;
                Refresh();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (draggedControl != null)
            {
                draggedControl = null;
                Cursor.Current = Cursors.Default;
                handler.WriteStapos(tt, stapos);
                if (hasDragged)
                    Refresh();
                hasDragged = false;
            }
            base.OnMouseUp(e);
        }
        #endregion
    }
}
