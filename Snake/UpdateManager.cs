using System;
using Microsoft.Xna.Framework;

namespace LyCilph
{
    public class UpdateManager
    {
        private double delta = 0;
        private double interval_step_size = 5;
        private bool running = true;
        private bool step;

        public double Interval { get; private set; }

        public UpdateManager()
        {
            Reset();
        }

        public void Reset()
        {
            Interval = Settings.update_interval;
        }

        public void Inc()
        {
            Interval += interval_step_size;
        }

        public void Dec()
        {
            var min_interval = 1000.0 / Settings.TargetFPS;
            Interval = Math.Max(min_interval, Interval - interval_step_size);
        }

        public void Max()
        {
            Interval = 1000.0 / Settings.TargetFPS;
        }

        public void Toggle()
        {
            running = !running;
        }

        public void Step()
        {
            step = true;
            delta += Interval; // This will force an update
        }

        public bool Update(GameTime game_time)
        {
            if (!running && !step)
                return false;

            delta += game_time.ElapsedGameTime.TotalMilliseconds;
            if (delta < Interval)
                return false;

            step = false;
            delta = 0;

            return true;
        }
    }
}
