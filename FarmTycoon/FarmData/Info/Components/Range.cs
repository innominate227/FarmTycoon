using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public struct Range
    {
        /// <summary>
        /// Start of the range
        /// </summary>
        private int _start;

        /// <summary>
        /// Start of the range is inclusive
        /// </summary>
        private bool _startInclusive;

        /// <summary>
        /// End of the range
        /// </summary>
        private int _end;

        /// <summary>
        /// End of the range is inclusive
        /// </summary>
        private bool _endInclusive;

        /// <summary>
        /// Create a range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Range(int start, bool startInclusive, int end, bool endInclusive)
        {
            Debug.Assert(start <= end);

            _start = start;
            _startInclusive = startInclusive;
            _end = end;
            _endInclusive = endInclusive;
        }

        /// <summary>
        /// Check if the value is within the range        
        /// </summary>
        public bool IsInRange(int value)
        {
            if (_startInclusive && _endInclusive)
            {
                return (value >= _start && value <= _end);
            }
            else if (_startInclusive && _endInclusive == false)
            {
                return (value >= _start && value < _end);
            }
            else if (_startInclusive == false && _endInclusive)
            {
                return (value > _start && value <= _end);
            }
            else
            {
                return (value > _start && value < _end);
            }
        }
    }
}
