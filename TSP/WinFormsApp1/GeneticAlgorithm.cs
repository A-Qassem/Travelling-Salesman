using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    public class GeneticAlgorithm
    {
        private Random _rand = new Random();
        private List<City> _cities = new List<City>();
        private List<TravelPath> _population = new List<TravelPath>();
        private TravelPath _bestPath;
        private TravelPath _currentBestPath;

        public int GenerationCount { get; private set; } = 0;
        public int BestCost { get; private set; } = int.MaxValue;
        public int InitialBestCost { get; private set; } = int.MaxValue;
        public int TotalImprovements { get; private set; } = 0;
        public int GensSinceImprovement { get; private set; } = 0;
        public bool Improved { get; private set; } = false;

        // Parameters
        public int PopulationSize { get; set; } = 50;
        public double MutationRate { get; set; } = 0.1;
        public double CrossoverRate { get; set; } = 0.8;

        public List<int> BestPathIndices => _bestPath?.Path ?? new List<int>();
        public List<int> CurrentBestPathIndices => _currentBestPath?.Path ?? new List<int>();
        public List<City> Cities => _cities;

        public void SetupCities(int count, int width, int height, int padding, bool randomPositions = true)
        {
            _cities.Clear();

            if (randomPositions)
            {
                for (int i = 0; i < count; i++)
                {
                    _cities.Add(new City(
                        _rand.Next(padding, padding + width),
                        _rand.Next(padding, padding + height),
                        i
                    ));
                }
            }
            else
            {
                // Circular arrangement
                double radius = Math.Min(width, height) / 2.5;
                double centerX = padding + width / 2;
                double centerY = padding + height / 2;

                for (int i = 0; i < count; i++)
                {
                    double angle = 2 * Math.PI * i / count;
                    int x = (int)(centerX + radius * Math.Cos(angle));
                    int y = (int)(centerY + radius * Math.Sin(angle));
                    _cities.Add(new City(x, y, i));
                }
            }
        }

        public void InitializePopulation()
        {
            _population.Clear();
            List<int> indices = new List<int>();
            for (int i = 1; i < _cities.Count; i++)
                indices.Add(i);

            for (int i = 0; i < PopulationSize; i++)
            {
                var shuffled = indices.OrderBy(x => _rand.Next()).ToList();
                _population.Add(new TravelPath(shuffled));
            }

            BestCost = int.MaxValue;
            InitialBestCost = int.MaxValue;
            _bestPath = null;
            _currentBestPath = null;
            GenerationCount = 0;
            GensSinceImprovement = 0;
            TotalImprovements = 0;
        }

        public bool NextGeneration()
        {
            Improved = false;
            GenerationCount++;
            GensSinceImprovement++;

            // Sort population by fitness
            _population = _population
                .OrderBy(p => p.CalculateFitness(_cities))
                .ToList();

            // Track current best
            _currentBestPath = _population[0];
            int currentBestCost = _currentBestPath.CalculateFitness(_cities);

            // Update overall best if improved
            if (currentBestCost < BestCost)
            {
                if (InitialBestCost == int.MaxValue)
                    InitialBestCost = currentBestCost;

                BestCost = currentBestCost;
                _bestPath = new TravelPath(_currentBestPath.Path);
                Improved = true;
                GensSinceImprovement = 0;
                TotalImprovements++;
            }

            // Create new population
            List<TravelPath> newPopulation = new List<TravelPath>();

            // Elitism - keep top performers
            int eliteCount = Math.Max(1, PopulationSize * 20 / 100);
            for (int i = 0; i < eliteCount; i++)
                newPopulation.Add(new TravelPath(_population[i].Path));

            // Fill the rest with crossover + mutation
            while (newPopulation.Count < PopulationSize)
            {
                var parent1 = TournamentSelection();
                var parent2 = TournamentSelection();

                TravelPath child;

                // Apply crossover based on crossover rate
                if (_rand.NextDouble() < CrossoverRate)
                    child = Crossover(parent1, parent2);
                else
                    child = new TravelPath(parent1.Path); // Just copy parent1

                child.Mutate(MutationRate, _rand);
                newPopulation.Add(child);
            }

            _population = newPopulation;
            return Improved;
        }

        private TravelPath TournamentSelection()
        {
            int k = Math.Max(3, _population.Count / 50); // Dynamic tournament size
            var selected = new List<TravelPath>();
            for (int i = 0; i < k; i++)
                selected.Add(_population[_rand.Next(_population.Count)]);
            return selected.OrderBy(p => p.CalculateFitness(_cities)).First();
        }

        private TravelPath Crossover(TravelPath parent1, TravelPath parent2)
        {
            // Ordered Crossover (OX)
            var p1 = parent1.Path;
            var p2 = parent2.Path;

            int start = _rand.Next(p1.Count);
            int end = _rand.Next(start, p1.Count);

            var childPath = Enumerable.Repeat(0, p1.Count).ToList();
            for (int i = start; i <= end; i++)
                childPath[i] = p1[i];

            int idx = 0;
            for (int i = 0; i < p2.Count; i++)
            {
                if (!childPath.Contains(p2[i]))
                {
                    while (idx < childPath.Count && childPath[idx] != 0) idx++;
                    if (idx < childPath.Count)
                        childPath[idx] = p2[i];
                }
            }

            return new TravelPath(childPath);
        }
    }
}