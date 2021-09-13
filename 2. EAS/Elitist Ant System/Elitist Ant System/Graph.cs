using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Elitist_Ant_System
{
    class Graph
    {
        // informacje o grafie
        public int len;
        public double[] x;
        public double[] y;
        public double[][] edges;
        public double[][] tau;
        public double[][] eta;

        // stałe
        public double evaporation = 0.5;
        public int alpha = 1;
        public int beta = 3;

        // Najlepsza znaleziona trasa
        public double bestDistance = double.MaxValue;
        public int[] bestPath;

        // Odczytanie danych grafu z pliku
        public void ExampleFile(string path2)
        {
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string path = dir + "\\" + path2;
            string[] lines = File.ReadAllLines(path);
            int z = 0;
            x = new double[this.len];
            y = new double[this.len];
            foreach (var line in lines)
            {
                string firstValue = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1];
                string secondtValue = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[2];
                x[z] = Convert.ToDouble(firstValue);
                y[z] = Convert.ToDouble(secondtValue);
                z++;
            }

            edges = new double[this.len][];

            for (int i = 0; i < this.len; i++)
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


                    edges[i][j] = Math.Sqrt((part1) + (part2));
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

        // utworzenie macierzy 
        public void CreateT(double c)
        {
            tau = new double[len][];
            eta = new double[len][];
            for (int i = 0; i < len; i++)
            {
                tau[i] = new double[len];
                eta[i] = new double[len];
                for (int j = 0; j < len; j++) // inicjowanie macierzy feromonow i macierzy jakości przejscia do węzła
                {
                    tau[i][j] = c;
                    eta[i][j] = 1 / edges[i][j];
                }
            }
        }

    }
}
