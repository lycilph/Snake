using LyCilph.Controllers;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Reflection;

namespace LyCilph.States
{
    public class MainMenuState : State
    {
        private SpriteFont header_font;
        private SpriteFont font;

        private string last_chromosome = string.Empty;

        public MainMenuState(StateManager state_manager) : base(state_manager) { }

        public override void LoadContent(ContentManager content)
        {
            header_font = content.Load<SpriteFont>("header_font");
            font = content.Load<SpriteFont>("font");
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsPressed(Keys.D1))
                state_manager.TransitionToGameState(ControllerType.Player);

            if (input.IsPressed(Keys.D2))
                state_manager.TransitionToGameState(ControllerType.Simple);

            if (input.IsPressed(Keys.D3))
            {
                if (string.IsNullOrWhiteSpace(last_chromosome))
                    LoadChromosome();

                state_manager.TransitionToGameState(ControllerType.NeuralNetwork, last_chromosome);
            }

            if (input.IsPressed(Keys.L))
            {
                LoadChromosome();
                state_manager.TransitionToGameState(ControllerType.NeuralNetwork, last_chromosome);
            }

            if (input.IsPressed(Keys.Escape))
                state_manager.Exit();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            var text_size = font.MeasureString("(3) Neural Network");
            var x = (Settings.screen_width - text_size.X) / 2;

            sprite_batch.DrawString(header_font, "Snake", new Vector2(x, 200), Color.Black);
            sprite_batch.DrawString(font, "(1) Player", new Vector2(x, 250), Color.Black);
            sprite_batch.DrawString(font, "(2) Simple AI", new Vector2(x, 270), Color.Black);
            sprite_batch.DrawString(font, "(3) Neural Network", new Vector2(x, 290), Color.Black);
            sprite_batch.DrawString(font, "(L)oad new chromosome", new Vector2(x + 20, 310), Color.Black);
            sprite_batch.DrawString(font, "(Esc) Quit", new Vector2(x, 340), Color.Black);
        }

        private void LoadChromosome()
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ofd.DefaultExt = ".snake"; // Default file extension
            ofd.Filter = "Snake (.snake)|*.snake"; // Filter files by extension

            // Show open file dialog box
            bool? result = ofd.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                //var chromosome = JsonUtils.ReadFromFile<Chromosome>(ofd.FileName);
                //game_state.SetNeuralNetworkController(chromosome);
                last_chromosome = ofd.FileName;
            }
        }
    }
}
