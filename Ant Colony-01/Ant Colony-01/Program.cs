using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;


namespace Ant_Colony_01
{
    class Program
    {

        static void Main(string[] args)
        {

            // Ustawienie do testowania algorytmu

            Graph g = new Graph(1);
            int maxinter = 500;
            int nAnts = g.len;
            double iVal = ACO.NearestNeighbour(g);
            double T0;
            T0 = nAnts / (g.len * iVal);
            g.CreateT(T0);



            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int t=0;t<maxinter;t++) // Główna pętla algorytmu
            {
                Colony[] m = new Colony[nAnts]; 
                ACO.GenerateSolution(g, m, nAnts);

                for (int i=0;i<nAnts;i++)
                {
                    m[i].distance =  ACO.CalculateCostOfPath(g, m[i]);
                }
                ACO.TakeBestResult(g,m,nAnts); 
                ACO.Evaporation(g); 
                ACO.UpdatePheromone(g,m,nAnts); 

            }
            stopWatch.Stop();

            Console.WriteLine("Najlepsze rozwiazanie :");
            Console.WriteLine(g.bestDistance);
            for(int q=0;q<g.len+1;q++)
            {
                Console.Write(g.bestPath[q]+1+"->");
            }

            Console.WriteLine("\nCzas wykonania: " + stopWatch.Elapsed);

            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string result = Convert.ToString(g.bestDistance);
            result += " ";
            result += stopWatch.Elapsed;
            string tmppath = Convert.ToString(g.len);
            string path = dir + "\\" + "result" + tmppath + ".txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(result);
            }

        }
    }
}
