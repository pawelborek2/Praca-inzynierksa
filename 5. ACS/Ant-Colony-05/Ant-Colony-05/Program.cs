using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;


namespace Ant_Colony_05
{
    class Program
    {

        class Graph
        {
            public int len; //ilosc wierzcholkow w grafie 
            public double[] x; //koordynaty danego wierzcholka w grafie
            public double[] y;
            public double[][] edges; //macierz odleglosci pomiedzy wierzcholkami
            public double[][] tau; // tablica feromonow
            public double[][] eta; // wartosc heretystyczna 1/dlugosc pomiedzy wierzcholkami
            public double evaporation = 0.1; // parowanie globalne
            public double lokalEvaporation = 0.1; // parowanie lokalne
            public int alpha = 1; //stale 
            public int beta = 3;
            public double q0 = 0.9;
            public double bestDistance = double.MaxValue; //najlepsza trasa i dystans
            public int[] bestPath;


            public void ExampleFile(string path2)
            {
                string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                string path = dir + "\\" + path2;
                string[] lines = File.ReadAllLines(path);
                int z = 0;
                x = new double[this.len];
                y = new double[this.len];
                foreach (var line in lines) // zapisanie wartosci do tablc na podstawie pliku
                {
                    string firstValue = line.Split(new string[] { " ", "." }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string secondtValue = line.Split(new string[] { " ", "." }, StringSplitOptions.RemoveEmptyEntries)[2];
                    x[z] = Convert.ToDouble(firstValue);
                    y[z] = Convert.ToDouble(secondtValue);
                    z++;
                }

                edges = new double[this.len][];

                for (int i = 0; i < this.len; i++) //obliczam odleglosc miedzy wierzcholkami
                {
                    edges[i] = new double[this.len];
                    for (int j = 0; j < this.len; j++)
                    {
                        double x1 = x[i];
                        double x2 = x[j];
                        double y1 = y[i];
                        double y2 = y[j];

                        double part1 = Math.Pow(x2 - x1, 2);
                        double part2 = Math.Pow(y2 - y1, 2);


                        edges[i][j] = Math.Sqrt((part1) + (part2)); //odleglosc miedzy punktami
                    }
                }
            }

            public Graph(int ex)
            {
                if (ex == 1)
                {
                    this.len = 42;
                    ExampleFile("dantzig42.txt");
                }
                if (ex == 2)
                {
                    this.len = 76;
                    ExampleFile("pr76.txt");
                }
                if (ex == 3)
                {
                    this.len = 101;
                    ExampleFile("eil101.txt");
                }
                if (ex == 4)
                {
                    this.len = 52;
                    ExampleFile("berlin52.txt");
                }
                if (ex == 5)
                {
                    this.len = 100;
                    ExampleFile("kroA100.txt");
                }
                bestPath = new int[this.len + 1]; //plus 1 gdyz powrót do poczatku
            }
            public void CreateT(double c)
            {
                tau = new double[len][];
                eta = new double[len][];
                for (int i = 0; i < len; i++)
                {
                    tau[i] = new double[len];
                    eta[i] = new double[len];
                    for (int j = 0; j < len; j++) 
                    {
                        tau[i][j] = c;
                        eta[i][j] = 1.0 / edges[i][j];
                    }
                }
            }

        }

        class Colony
        {

            public List<int> Tour { get; set; }
            public double distance = 0;

            public Colony(int nNodes) // tworzenie talibcy 
            {
                this.Tour = new List<int>(nNodes);
            }

        }

        static double NearestNeighbour (Graph g)
        {
            List<int> Tour = new List<int>(g.len);
            double distance = 0;
            Random rnd = new Random();
            int initialNode = rnd.Next(0, g.len); //losowanie poczatkowego indeksu
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
            distance += g.edges[currentNode][initialNode]; // powrot do poczatku
            return distance;
        }

        static int RouleteWheel(double[] tNode, int nCities, List<int> list)
        {
            double sum = 0;
            Random rnd = new Random();
            double rNumber = rnd.NextDouble();
            for (int i = 0; i < nCities; i++)
            {
                sum += tNode[i];
                if (!list.Contains(i) && rNumber < sum)
                {
                    return i;
                }

            }
            return -1;
        }
        static void GenerateSolution(Graph g, Colony[] a, int antNo, double tau0)
        {
            int nodeNo = g.len;
            for (int i = 0; i < antNo; i++) // przechodzimy po wszystkich mrowkach
            {
                Random rnd = new Random();
                int initialNode = rnd.Next(0, nodeNo); //losowanie poczatkowego indeksu
                a[i] = new Colony(nodeNo); // tworze mrowke
                a[i].Tour = new List<int>(nodeNo); // dodaje jej poczatkowe wartosci
                a[i].Tour.Add(initialNode); // wybranie dla danej mrowki miasto startowe

                int selectedNode = 0;

                for (int j = 1; j < nodeNo; j++) // kolejne wybory mrowki
                {
                    int currentNode = a[i].Tour.Last(); //aktualny wierzcholek danej mrowki
                    double q = rnd.NextDouble(); // losuje sposob wyboru
                    if (q < g.q0)
                    {
                        double argmax = double.MinValue;
                        for (int x = 0; x < nodeNo; x++)
                        {
                            if (!(a[i].Tour.Contains(x))) // Jezeli nie odwiedzilismy
                            {
                                double tmp = g.tau[currentNode][x] * Math.Pow((g.eta[currentNode][x]),g.beta);
                                if (argmax < tmp)
                                {
                                    selectedNode = x;
                                    argmax = tmp;
                                }
                            }
                        }
                    }
                    else //wybor funkcja pseudolowa w przypadku gdy q>q0
                    {
                        double sum = 0; // suma tablicy tau i eta
                        for (int x = 0; x < nodeNo; x++) // przejsc po wszysykich krawedziach i obliczenie sumy 
                        {
                            if (!(a[i].Tour.Contains(x))) // Jezeli nie odwiedzilismy
                            {
                                double tij = g.tau[currentNode][x];
                                double nij = g.eta[currentNode][x];
                                sum += Math.Pow(tij, g.alpha) * Math.Pow(nij, g.beta);

                            }
                        }
                        double[] p_allnodes = new double[nodeNo]; //lista prawdopodobienstw
                        for (int y = 0; y < nodeNo; y++)
                        {
                            double pij;
                            if (!(a[i].Tour.Contains(y))) // Jezeli nie odwiedzilismy
                            {
                                double tij = g.tau[currentNode][y];
                                double nij = g.eta[currentNode][y];
                                pij = Math.Pow(tij, g.alpha) * Math.Pow(nij, g.beta) / sum;
                            }
                            else // jezli odwiedzilismy wierzholek to p=0
                            {
                                pij = 0;
                            }
                            p_allnodes[y] = pij;
                        }
                        selectedNode = RouleteWheel(p_allnodes, nodeNo, a[i].Tour);
                    }

                    a[i].Tour.Add(selectedNode); //dodaje wybor mrowki do listy
                    g.tau[currentNode][selectedNode] = (1.0 - g.lokalEvaporation) * g.tau[currentNode][selectedNode] + tau0 * g.lokalEvaporation;// dodanie lokalne sladow feromonowych
                    g.tau[selectedNode][currentNode] = (1.0 - g.lokalEvaporation) * g.tau[selectedNode][currentNode] + tau0 * g.lokalEvaporation;
                }
                g.tau[selectedNode][initialNode] = (1.0 - g.lokalEvaporation) * g.tau[selectedNode][initialNode] + tau0 * g.lokalEvaporation;// dodanie sladu do sciezki powrotnej
                g.tau[selectedNode][initialNode] = (1.0 - g.lokalEvaporation) * g.tau[selectedNode][initialNode] + tau0 * g.lokalEvaporation;

                a[i].Tour.Add(initialNode); //powracamy do poczatkowego punktu
            }
        }

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

        static void UpdatePheromone(Graph g)
        {
            for (int j = 0; j < g.len ; j++)
            {
                int currentNode = g.bestPath[j];
                int nextNode = g.bestPath[j + 1];
                g.tau[currentNode][nextNode] = (1.0 - g.evaporation) * g.tau[currentNode][nextNode] + g.evaporation * 1.0 / g.bestDistance;
                g.tau[nextNode][currentNode] = (1.0 - g.evaporation) * g.tau[nextNode][currentNode] + g.evaporation * 1.0 / g.bestDistance;
            }
        }

        static void Main(string[] args)
        {

            //Tworzenie grafu
            Graph g = new Graph(3);


            int maxinter =5000;
            int nAnts = 10;
            double iVal = NearestNeighbour (g);
            double T0; 
            T0 = 1.0 / (g.len * iVal); // poczatkowe wartosci feromonow
            g.CreateT(T0); // tworzenie macierzy eta i tau

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            for (int t = 0; t < maxinter; t++) // Główna pętla dla algorytmu
            {
                Colony[] m = new Colony[nAnts]; //tworzymy kolonie
                GenerateSolution(g, m, nAnts, T0);

                for (int i = 0; i < nAnts; i++)
                {
                    m[i].distance = CalculateCostOfPath(g, m[i]);
                }
                TakeBestResult(g, m, nAnts); // Znalezienie mrowki z najlepszym rozwiazaniem
                UpdatePheromone(g);
            }
            stopWatch.Stop();

            Console.WriteLine("Najlepsze rozwiazanie :");
            Console.WriteLine(g.bestDistance);
            for (int q = 0; q < g.len + 1; q++)
            {
                Console.Write(g.bestPath[q] + 1 + "->");
            }

            // Write result.
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
