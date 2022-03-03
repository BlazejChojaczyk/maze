using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public class TCell
    {
        public int x;
        public int y;
        public List<TCell> Connected = new List<TCell>();

        public void ConnectTo(TCell neigh)
        {
            if (!Connected.Contains(neigh))
            {
                Connected.Add(neigh);
                neigh.Connected.Add(this);
            }
        }
    }
}
