using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ant_Colony_01
{
    class ACO
    {
        // Znajdowanie trasy metodą na najbliższego sąsiada
        static double NearestNeighbour(Graph g)
        {
            List<int> Tour = new List<int>(g.len);
            double distance = 0;

            Random rnd = new Random();
            int initialNode = rnd.Next(0, g.len);

            Tour.Add(initialNode);
            int currentNode = initialNode;
            int nextNode = 0;

            for (int i = 0; i < g.len; i++)
            {
                double minval = Double.MaxValue;
                for (int y = 0; y < g.len; y++)
                {
                    double distancetoCity = g.edges[currentNode][y];
                    if (!Tour.Contains(y) && distancetoCity < minval)
                    {
                        minval = distancetoCity;
                        nextNode = y;
                    }
                }
                Tour.Add(nextNode);
                distance += g.edges[currentNode][nextNode];
                currentNode = nextNode;
            }

            distance += g.edges[currentNode][initialNode];
            return distance;
        }

        // Pseudolowe losowanie liczby 
        static int RouleteWheel(double[] tNode, int Ncitys, List<int> list)
        {
            double sum = 0;
            Random rnd = new Random();
            double probalitysum = 0;
            for (int j = 0; j < Ncitys; j++)
            {
                probalitysum += tNode[j];
            }
            double rNumber = rnd.NextDouble() * probalitysum;
            for (int i = 0; i < Ncitys; i++)
            {
                sum += tNode[i];
                if (!list.Contains(i) && rNumber <= sum)
                {

                    return i;
                }

            }

            Console.WriteLine(rNumber);
            Console.WriteLine(probalitysum);
            return -1;
        }

        // Wygenerowanie rozwiązania- 1 obrót pętli
        static void GenerateSolution(Graph g, Colony[] a, int antNo)
        {
            int nodeNo = g.len;
            for (int i = 0; i < antNo; i++) // przechodzimy po wszystkich mrowkach
            {
                Random rnd = new Random();
                int initialNode = rnd.Next(0, nodeNo);
                a[i] = new Colony(nodeNo);
                a[i].Tour = new List<int>(nodeNo);
                a[i].Tour.Add(initialNode);

                for (int j = 1; j < nodeNo; j++)
                {
                    int currentNode = a[i].Tour.Last(); //
                    double sum = 0;

                    for (int x = 0; x < nodeNo; x++)
                    {
                        if (!(a[i].Tour.Contains(x)))
                        {
                            double tij = g.tau[currentNode][x];
                            double nij = g.eta[currentNode][x];
                            sum += Math.Pow(tij, g.alpha) * Math.Pow(nij, g.beta);
                        }
                    }
                    double[] pAllNodes = new double[nodeNo];
                    for (int y = 0; y < nodeNo; y++)
                    {
                        double pij;
                        if (!(a[i].Tour.Contains(y)))
                        {
                            double tij = g.tau[currentNode][y];
                            double nij = g.eta[currentNode][y];
                            pij = Math.Pow(tij, g.alpha) * Math.Pow(nij, g.beta) / sum;

                        }
                        else
                        {
                            pij = 0;
                        }
                        pAllNodes[y] = pij;
                    }
                    int selectedNode = RouleteWheel(pAllNodes, nodeNo, a[i].Tour);
                    a[i].Tour.Add(selectedNode);
                }
                a[i].Tour.Add(initialNode);
            }
        }

        //Obliczenie koszt trasy
        static double CalculateCostOfPath(Graph g, Colony ant)
        {
            double distance = 0;

            for (int i = 0; i < g.len; i++)
            {
                int currentNode = ant.Tour[i];
                int nextNode = ant.Tour[i + 1];
                distance += g.edges[currentNode][nextNode];
            }

            return distance;

        }

        // Aktualizacja najepszej trasy
        static void TakeBestResult(Graph g, Colony[] m, int nAnts)
        {
            for (int i = 0; i < nAnts; i++)
            {
                if (m[i].distance < g.bestDistance)
                {
                    g.bestDistance = m[i].distance;
                    Console.WriteLine("Najkrótsza trasa do tej pory: " + g.bestDistance);

                    for (int j = 0; j < m[i].Tour.Count - 1; j++)
                    {
                        g.bestPath[j] = m[i].Tour[j];
                    }

                    g.bestPath[g.len] = m[i].Tour.Last();
                }
            }
        }

        // Aktualizacja feromonu
        static void UpdatePheromone(Graph g, Colony[] m, int nAnts)
        {
            for (int i = 0; i < nAnts; i++)
            {
                for (int j = 0; j < g.len - 1; j++)
                {
                    int currentNode = m[i].Tour[j];
                    int nextNode = m[i].Tour[j + 1];
                    g.tau[currentNode][nextNode] += 1.0 / m[i].distance;
                    g.tau[nextNode][currentNode] += 1.0 / m[i].distance;
                }
            }
        }

        // Odparowanie feromonów
        static void Evaporation(Graph g)
        {
            for (int i = 0; i < g.len; i++)
            {
                for (int j = 0; j < g.len; j++)
                {
                    g.tau[i][j] = g.tau[i][j] * (1.0 - g.evaporation);
                }
            }
        }
    }
}
