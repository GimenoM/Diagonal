using System.ComponentModel.Design;
using System.Reflection.PortableExecutable;

namespace Diagonal
{
    class Square
    {
        static void Main()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();    
            #region Variables 

            int iterations = 500;
            int populationlimit = 10000;
            int parentsqty = 5000;

            int Xdimension = 10;
            int Ydimension = 10;

            double mutation = 10e-3;

            double[] SOLUTION = new double[2];
            double MAXLENGTHABSOLUTE = 0;

            double[] chromosome= new double[2];
            double gen;
            Dictionary<int, double[]> dpopulation = new Dictionary<int, double[]>();

            Random rnd = new Random();

            #endregion
        

            #region Initial Solution
                for (int i=0; i<populationlimit; i++)
            {
                chromosome = new double[2];

                gen = rnd.NextDouble() * Xdimension;
                chromosome[0]= gen;

                gen = rnd.NextDouble() * Ydimension;
                chromosome[1] = gen;

                dpopulation.Add(dpopulation.Count+1, chromosome);
            }
            #endregion

            

            double length;
            Dictionary<int, double> fitnessvalues = new Dictionary<int, double>();
            double totallength;
            
            for (int i=1; i<iterations; i++)
            {
                #region Evaluation 
                totallength = 0;
                fitnessvalues = new Dictionary<int, double>();
                foreach (KeyValuePair<int, double[]> kvp in dpopulation)
                {
                    chromosome = new double[2];
                    chromosome = kvp.Value;
                    length = Math.Sqrt(Math.Pow(chromosome[0], 2) + Math.Pow(chromosome[1], 2));
                    fitnessvalues.Add(kvp.Key, length);
                    totallength+= length;

                    if (length > MAXLENGTHABSOLUTE)
                    {
                        MAXLENGTHABSOLUTE = length;
                        SOLUTION = chromosome;
                    }
                }
                #endregion

                //#region Selection Tournament
                //Dictionary<int, double[]> dparents = new Dictionary<int, double[]>();
                //int p1;
                //int p2;
                //for (int  j=0; j<parentsqty; j++)
                //{
                //SelectAspirants:
                //    p1 = rnd.Next(populationlimit);
                //    p2 = rnd.Next(populationlimit);
                //    if (p1 > p2)
                //    {
                //        dparents.Add(dparents.Count+1, dpopulation.ElementAt(p1).Value);
                //    } else if (p2> p1)
                //    {
                //        dparents.Add(dparents.Count+1, dpopulation.ElementAt(p2).Value);
                //    } else
                //    goto SelectAspirants;

                //}
                //#endregion

                #region Selection By Fitness
                Dictionary<int, double[]> dparents = new Dictionary<int, double[]>();

                Selection:
                foreach (KeyValuePair<int, double> kvp in fitnessvalues)
                {
                    double P = kvp.Value / totallength*100;
                    double threshold=rnd.NextDouble();
                    if (P > threshold){
                        dparents.Add(dparents.Count + 1, dpopulation[kvp.Key]);
                        if(dparents.Count == parentsqty)
                        {
                            goto Crossover;
                        }
                    }

                }
                if(dparents.Count != parentsqty)
                {
                    goto Selection;
                }
                #endregion

            #region Crossover
                Crossover:
                Dictionary<int, double[]> dchildren =new Dictionary<int, double[]>();
                for (int j = 0; j < populationlimit-parentsqty; j++)
                {
                    int p1 = rnd.Next(parentsqty);
                    int p2 = rnd.Next(parentsqty);

                    chromosome = new double[2];
                    chromosome[0] = dparents.ElementAt(p1).Value[0];
                    chromosome[1] = dparents.ElementAt(p2).Value[1];
                    dchildren.Add (dchildren.Count+1, chromosome);
                    j++;

                    chromosome = new double[2];
                    chromosome[0] = dparents.ElementAt(p2).Value[0];
                    
                    chromosome[1] = dparents.ElementAt(p1).Value[1];
                    dchildren.Add(dchildren.Count + 1, chromosome);
                }

                foreach(KeyValuePair<int, double[]> kvp in dparents)
                {
                    dchildren.Add(dchildren.Count+1,kvp.Value);
                }
                #endregion

                #region Mutation
                double mutationprobability;
                double mutedgen;
                foreach(KeyValuePair <int, double[]> kvp in dchildren)
                {
                    mutationprobability=rnd.NextDouble();
                    if (mutationprobability < mutation)
                    {
                        mutedgen = rnd.NextDouble();
                        if (mutedgen < 0.5)
                        {
                            kvp.Value[0] = rnd.NextDouble() * Xdimension;
                        } else
                        {
                            kvp.Value[1] = rnd.NextDouble() * Ydimension;
                        }
                    }
                }
                #endregion

                #region Replacement
                dpopulation = new Dictionary<int, double[]>();
                dpopulation = dchildren;
                #endregion

                Console.WriteLine("Iteration " + i + " - " + totallength / populationlimit);
            }

            #region FIND BEST SOLUTION
            int winner;
            double maxfitness = 0;
            chromosome = new double[2];
            foreach (KeyValuePair<int,double> fitness in fitnessvalues)
            {
                if (fitness.Value > maxfitness)
                {
                    maxfitness = fitness.Value;
                    winner = fitness.Key;
                    chromosome = dpopulation.ElementAt(winner).Value;
                }
            }
            #endregion
            watch.Stop();           
            Console.WriteLine("The longest diagonal in the square starts at (0,0) and ends at (" + chromosome[0] + "," + chromosome[1] + "), with a length of " + maxfitness);
            Console.WriteLine("The solution with the method 2 is (" + SOLUTION[0] + "," + SOLUTION[1] + "), with a length of "+ MAXLENGTHABSOLUTE);
            Console.WriteLine(watch.Elapsed);
        }
    }
}