using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maze
{
    public partial class Tmaze : UserControl
    {
        public static int n = 30;
        public TCell[,] Cells = new TCell[n,n];
        public float HallSize { get; set; } = 0.8f;
        public TCell StartCell;
        public TCell StopCell;
        public static Random rnd = new Random();
        public List<TCell> path;
        public int PathCount;
        public List<TCell> CreationPath { get; set; }
        public List<TCell> EscapePath { get; set; }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var cellW = (float)Width / n;
            var cellH = (float)Height / n;
            e.Graphics.ScaleTransform(cellW, cellH);
            var brush = new SolidBrush(ForeColor);

            for (var i = 1; i < PathCount; i++)
            {
                var prevCell = path[i - 1];
                var currCell = path[i];
                RectangleF rc;

                if(prevCell.y == currCell.y)
                {
                    var x = Math.Min(prevCell.x, currCell.x);
                    var y = currCell.y;
                    rc = new RectangleF(x, y, 1 + HallSize, HallSize);
                }
                else
                {
                    var y = Math.Min(prevCell.y, currCell.y);
                    var x = currCell.x;
                    rc = new RectangleF(x, y, HallSize, 1 + HallSize);
                }
                if(i == path.IndexOf(currCell))
                {
                    brush.Color = Color.Magenta;
                }
                else
                {
                    brush.Color = ForeColor;
                }
                e.Graphics.FillRectangle(brush, rc);
            }
            brush.Color = Color.Red;
            e.Graphics.FillRectangle(brush, StartCell.x, StartCell.y, HallSize, HallSize);

            brush.Color = Color.Green;
            e.Graphics.FillRectangle(brush, StopCell.x, StopCell.y, HallSize, HallSize);
            //for (var x = 0; x < n; x++)
            //{
            //    for (var y = 0; y < n; y++)
            //    {
            //        var rc = new RectangleF(x, y, HallSize, HallSize);
            //        e.Graphics.FillRectangle(brush, rc);
            //    }
            //}
        }
        public Tmaze()
        {
            InitializeComponent();
            DoubleBuffered = true;
            ResizeRedraw = true;
            for (var x = 0; x < n; x++)
            {
                for (var y = 0; y < n; y++)
                {
                    var cell = new TCell();
                    cell.x = x;
                    cell.y = y;
                    Cells[x, y] = cell;
                }
            }
            Build();
        }
        public void Build()
        {
            path = new List<TCell>();
            StopCell = Cells[0,0];
            StartCell = Cells[rnd.Next(n), rnd.Next(n)];
            var depthCells = new List<TCell>();
            var cell = StartCell;
            do
            {
                path.Add(cell);
                var freeCells = new List<TCell>();
                if(cell.x > 0)
                {
                    var neigh = Cells[cell.x - 1, cell.y];
                    if(neigh.Connected.Count == 0)
                    {
                        freeCells.Add(neigh);
                    }
                }
                if (cell.x + 1 < n)
                {
                    var neigh = Cells[cell.x + 1, cell.y];
                    if (neigh.Connected.Count == 0)
                    {
                        freeCells.Add(neigh);
                    }
                }
                if (cell.y > 0)
                {
                    var neigh = Cells[cell.x, cell.y - 1];
                    if (neigh.Connected.Count == 0)
                    {
                        freeCells.Add(neigh);
                    }
                }
                if (cell.y + 1 < n)
                {
                    var neigh = Cells[cell.x, cell.y + 1];
                    if (neigh.Connected.Count == 0)
                    {
                        freeCells.Add(neigh);
                    }
                }
                if(freeCells.Count == 0)
                {
                    cell = depthCells[depthCells.Count - 1];
                    depthCells.RemoveAt(depthCells.Count-1);

                }
                else
                {
                    var neigh = freeCells[rnd.Next(freeCells.Count)];
                    cell.ConnectTo(neigh);
                    depthCells.Add(cell);
                    cell = neigh;
                }
            } while (depthCells.Count > 0);
            path.Add(cell);
            PathCount = path.Count;
            CreationPath = new List<TCell>(path);
        }

        public void FindPath()
        {
            path = new List<TCell>();
            FindPathFrom(StartCell);
        }

        private bool FindPathFrom(TCell cell)
        {
            path.Add(cell);
            if (cell == StopCell)
            {
                return true;
            }
            foreach (var neigh in cell.Connected)
            {
                if (!path.Contains(neigh) && FindPathFrom(neigh))
                {
                    return true;
                }
            }
            path.RemoveAt(path.Count - 1);
            return false;
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            var x = StartCell.x;
            var y = StartCell.y;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    y--;
                    break;
                case Keys.Down:
                    y++;
                    break;
                case Keys.Left:
                    x--;
                    break;
                case Keys.Right:
                    x++;
                    break;
            }
            if (x >= 0 && x < n && y >= 0 && y < n)
            {
                var neigh = Cells[x, y];
                if (neigh.Connected.Contains(StartCell))
                {
                    StartCell = neigh;
                    Invalidate();
                    if (StartCell == StopCell)
                    {
                        MessageBox.Show("Congratulation, Maze solved !");
                    }
                }
            }
        }
    }
}
