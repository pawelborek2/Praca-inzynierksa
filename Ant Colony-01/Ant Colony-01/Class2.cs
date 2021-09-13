using System;
using System.Collections.Generic;
using System.Text;

namespace Ant_Colony_01
{
    class Colony
    {

        public List<int> Tour { get; set; }
        public double distance = 0;

        public Colony(int nNodes) // tworzenie talibcy 
        {
            this.Tour = new List<int>(nNodes);
        }

    }
}
