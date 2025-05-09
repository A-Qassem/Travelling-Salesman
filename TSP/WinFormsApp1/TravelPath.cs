using System;
using System.Collections.Generic;
using System.Linq;
namespace TSP
{
    public class TravelPath
    {
        private List<int> _path;
        private int _fitness = -1;
        public TravelPath(List<int> path)
        {
            _path = new List<int>(path);
        }
        public List<int> Path => new List<int>(_path);
        public int CalculateFitness(List<City> cities)
        {
            if (_fitness >= 0) return _fitness;
            int cost = 0;
            int k = 0;
            foreach (var cityIndex in _path)
            {
                if (k < cities.Count && cityIndex < cities.Count)
                    cost += City.Distance(cities[k], cities[cityIndex]);
                k = cityIndex;
            }
            if (k < cities.Count)
                cost += City.Distance(cities[k], cities[0]);
            _fitness = cost;
            return cost;
        }
        public void Mutate(double mutationRate, Random rand)
        {
            // Swap mutation
            if (rand.NextDouble() < mutationRate)
            {
                int i = rand.Next(_path.Count);
                int j = rand.Next(_path.Count);
                int temp = _path[i];
                _path[i] = _path[j]; // Fixed: was *path[i] = *path[j]
                _path[j] = temp;
                _fitness = -1; // Reset fitness since path changed
            }
            // Additional mutation type: 2-opt local improvement
            if (rand.NextDouble() < mutationRate * 0.5)
            {
                int i = rand.Next(_path.Count);
                int j = rand.Next(_path.Count);
                if (i > j) { int temp = i; i = j; j = temp; }
                if (j - i >= 2) // Only reverse if segment is at least 2 elements
                {
                    // Reverse the segment between i and j
                    while (i < j)
                    {
                        int temp = _path[i];
                        _path[i] = _path[j]; // Fixed: was *path[i] = *path[j]
                        _path[j] = temp;
                        i++; j--;
                    }
                    _fitness = -1; // Reset fitness since path changed
                }
            }
        }
    }
}