using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class TraitQualityInfo
    {
        /// <summary>
        /// The optimal value for the trait
        /// </summary>
        private int _optimal;

        /// <summary>
        /// The wieght this has on the overall running quality of the trait
        /// </summary>
        private int _weight;

        /// <summary>
        /// The max amount below the optimal value at which the traits quality is 0
        /// </summary>
        private int _maxBelow;

        /// <summary>
        /// The max amount above the optimal value at which the traits quality is 0
        /// </summary>
        private int _maxAbove;

        /// <summary>
        /// Determines the leiency when the trait value is below the optimal value
        /// </summary>
        private double _leniencyBelow;

        /// <summary>
        /// Determines the leiency when the trait value is above the optimal value
        /// </summary>
        private double _leniencyAbove;


        /// <summary>
        /// Cache the tells the quality of the trait at different values
        /// </summary>
        private Dictionary<int, int> _qualityCache = new Dictionary<int, int>();


        /// <summary>
        /// Create a TraitQualityInfo
        /// </summary>
        public TraitQualityInfo(int optimal, int weight, int maxBelow, int maxAbove, double leniencyBelow, double leniencyAbove)
        {
            _optimal = optimal;
            _weight = weight;
            _maxBelow = maxBelow;
            _maxAbove = maxAbove;
            _leniencyBelow = leniencyBelow;
            _leniencyAbove = leniencyAbove;
        }


        /// <summary>
        /// Get the traits quality given it current value
        /// </summary>
        public int GetQuality(int currentValue)
        {
            //if the quality at this value is not cached cache the value
            if (_qualityCache.ContainsKey(currentValue) == false)
            {
                int quality = 100;
                if (currentValue < _optimal)
                {
                    quality = (int)(-100 * Math.Pow(((double)(_optimal - currentValue) / (double)_maxBelow), _leniencyBelow) + 100);                    
                }
                else if (currentValue > _optimal)
                {
                    quality = (int)(-100 * Math.Pow(((double)(currentValue - _optimal) / (double)_maxAbove), _leniencyAbove) + 100);
                }
                if (quality < 0) { quality = 0; }
                if (quality > 100) { quality = 100; }
                _qualityCache.Add(currentValue, quality);
            }

            //return the quality at that value
            return _qualityCache[currentValue];
        }


        /// <summary>
        /// The optimal value for the trait
        /// </summary>
        public int Optimal
        {
            get { return _optimal; }
        }

        /// <summary>
        /// The wieght this has on the overall running quality of the trait
        /// (Note: this is not used for Instantaneous quality)
        /// </summary>
        public int Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// The max amount below the optimal value at which the traits quality is 0
        /// </summary>
        public int MaxBelow
        {
            get { return _maxBelow; }
        }

        /// <summary>
        /// The max amount above the optimal value at which the traits quality is 0
        /// </summary>
        public int MaxAbove
        {
            get { return _maxAbove; }
        }

        /// <summary>
        /// Determines the leiency when the trait value is below the optimal value
        /// </summary>
        public double LeniencyBelow
        {
            get { return _leniencyBelow; }
        }

        /// <summary>
        /// Determines the leiency when the trait value is above the optimal value
        /// </summary>
        public double LeniencyAbove
        {
            get { return _leniencyAbove; }
        }

    }
}
