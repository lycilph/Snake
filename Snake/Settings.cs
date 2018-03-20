using LyCilph.Utils;
using System.IO;
using System.Reflection;

namespace LyCilph
{
    public class Settings
    {
        public int Size { get; set; } // Size of the playing area in cells
        public int CellSize { get; set; } // Size of the cells in pixels
        public int EnergyPerFood { get; set; }
        public int TargetFPS { get; set; }
        public int IntervalMax { get; set; }
        public int IntervalMin { get; set; }
        public int IntervalChange { get; set; }
        public int IntervalStart { get; set; }


        public static string GetPath()
        {
            var exe_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exe_path, "settings.txt");
        }

        public static Settings Load()
        {
            var filename = GetPath();

            if (File.Exists(filename))
                return JsonUtils.ReadFromFile<Settings>(filename);
            else
            {
                var settings = new Settings
                {
                    Size = 60,
                    CellSize = 10,
                    EnergyPerFood = 250,
                    TargetFPS = 200,
                    IntervalMax = 200, // ~ 5 fps
                    IntervalMin = 5, // ~ 200 fps
                    IntervalChange = 5,
                    IntervalStart = 50,
                };
                JsonUtils.WriteToFile(filename, settings);
                return settings;
            }
        }
    }
}
