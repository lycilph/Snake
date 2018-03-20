namespace LyCilph
{
    public static class Settings
    {
        public static int board_size = 60; // Board size in cells
        public static int cell_size = 10;  // Cell size in pixels

        public static int text_area_width = 400;
        public static int board_width = board_size * cell_size;
        public static int screen_width = board_width + text_area_width;
        public static int screen_height = board_width;

        public static int TargetFPS = 200;

        public static int energy_per_food = 250;

        //public int Size { get; set; } // Size of the playing area in cells
        //public int CellSize { get; set; } // Size of the cells in pixels
        //public int EnergyPerFood { get; set; }
        //public int TargetFPS { get; set; }
        //public int IntervalMax { get; set; }
        //public int IntervalMin { get; set; }
        //public int IntervalChange { get; set; }
        //public int IntervalStart { get; set; }
    }
}
