using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Utils
{
    public static class LongIdGeneratorUtil
    {
        private static readonly object _lock = new object();
        private static long _lastTimestamp = DateTime.UtcNow.Ticks;

        public static long GenerateUniqueLongId()
        {
            lock (_lock)
            {
                long timestamp = DateTime.UtcNow.Ticks;

                if (timestamp <= _lastTimestamp)
                {
                    timestamp = _lastTimestamp + 1;
                }

                _lastTimestamp = timestamp;
                return timestamp;
            }
        }
    }
}
