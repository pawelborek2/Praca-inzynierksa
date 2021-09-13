using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Elitist_Ant_System
{
        class Program
        {

            static void Main(string[] args)
            {

                //Ustawienia do testowania algorytmu
                Graph g = new Graph(2);
                int maxinter = 500;
                int nAnts = g.len;
                double iVal = EAS.NearestNeighbour (g);
                double T0; 
                double e = g.len;
                T0 = (nAnts +e) / (g.evaporation * iVal); 
                g.CreateT(T0); 

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                for (int t = 0; t < maxinter; t++) // Główna pętla dla algorytmu
                {
                    Colony[] m = new Colony[nAnts]; 
                    EAS.GenerateSolution(g, m, nAnts);

                    for (int i = 0; i < nAnts; i++)
                    {
                        m[i].distance = EAS.CalculateCostOfPath(g, m[i]);
                    }
                    EAS.TakeBestResult(g, m, nAnts); 
                    EAS.Evaporation(g); 
                    EAS.UpdatePheromone(g, m, nAnts,e); 

                }

                stopWatch.Stop();

                // Wypisanie oraz zapisanie wyników do plików
                Console.WriteLine("Najlepsze rozwiazanie :");
                Console.WriteLine(g.bestDistance);
                for (int q = 0; q < g.len + 1; q++)
                {
                    Console.Write(g.bestPath[q] + 1 + "->");
                }

                Console.WriteLine("Czas wykonania: "+ stopWatch.Elapsed);

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