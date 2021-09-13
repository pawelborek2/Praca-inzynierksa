using System;
using System.Collections.Generic;
using System.Text;

namespace Elitist_Ant_System
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
