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

        public static int update_interval = 50;

        public static int energy_per_food = 250;
    }
}
