using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public class MillisecCounter
    {
        public MillisecCounter(int millisecs)
        {
            max = millisecs;
            configuredMin = millisecs;
            configuredMax = millisecs;
            r = new Random(millisecs);
        }
        public MillisecCounter(int minMillisecs, int maxMillisecs)
        {
            max = minMillisecs;
            configuredMin = minMillisecs;
            configuredMax = maxMillisecs;
            r = new Random(minMillisecs);
        }

        int current;
        int max;
        bool completed;
        int configuredMin;
        int configuredMax;
        Random r;

        public int Current
        {
            get { return current; }
        }

        public int Max 
        { 
            get { return max; } 
        }

        public bool Completed
        {
            get { return completed; }
        }


        public void Reset()
        {
            //if premature reset (i.e. before timer has completed)
            //then reset to current
            //otherwise, take the residual milliseconds and add it on to the next time
            if (completed == false)
                current = 0;
            else
                current = (current - max);

            //reset completed flag
            completed = false;
            
            //if a min and max were supplied then randomize the next max
            //otherwise, leave it as is
            if (configuredMin != configuredMax)
                max = r.Next(configuredMin, configuredMax);

        }

        public void Reset(int millisecs)
        {
            configuredMin = millisecs;
            configuredMax = millisecs;

            this.Reset();

            //manuall reset millisec limit
            max = millisecs;
        }

        public void Reset(int minMillisecs, int maxMillisecs)
        {
            configuredMin = minMillisecs;
            configuredMax = maxMillisecs;

            this.Reset();
        }


        public void Update(int millisecs)
        {
            if (completed)
                return;

            current += millisecs;

            if (current >= max)
                completed = true;

        }

    }
}
