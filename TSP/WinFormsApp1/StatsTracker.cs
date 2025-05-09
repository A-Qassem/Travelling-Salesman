using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TSP
{
    public class StatsTracker
    {
        private Stopwatch _generationTimer = new Stopwatch();
        private Queue<int> _recentCosts = new Queue<int>();
        private const int MaxCostsToTrack = 20;

        public long LastGenerationTime { get; private set; }
        public double GenerationsPerSecond => LastGenerationTime > 0 ? 1000.0 / LastGenerationTime : 0;

        public void StartGenerationTimer()
        {
            _generationTimer.Restart();
        }

        public void StopGenerationTimer()
        {
            _generationTimer.Stop();
            LastGenerationTime = _generationTimer.ElapsedMilliseconds;
        }

        public void TrackCost(int cost)
        {
            if (_recentCosts.Count >= MaxCostsToTrack)
                _recentCosts.Dequeue();
            _recentCosts.Enqueue(cost);
        }

        public double CalculateImprovementPercentage(int initialCost, int currentCost)
        {
            if (initialCost != int.MaxValue && initialCost != 0)
                return 100.0 * (initialCost - currentCost) / initialCost;
            return 0;
        }
    }
}
