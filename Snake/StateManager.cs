using LyCilph.Controllers;
using LyCilph.Elements;
using LyCilph.States;
using LyCilph.Utils;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;

namespace LyCilph
{
    public class StateManager
    {
        private MainMenuState main_menu_state;
        private GameState game_state;
        private GameOverState game_over_state;

        private Action exit_action;

        public State CurrentState { get; private set; }

        public StateManager(GraphicsDevice graphics_device, Action exit_action)
        {
            main_menu_state = new MainMenuState(this);
            game_state = new GameState(this, graphics_device);
            game_over_state = new GameOverState(this);

            this.exit_action = exit_action;
        }

        public void LoadContent(ContentManager content)
        {
            main_menu_state.LoadContent(content);
            game_state.LoadContent(content);
            game_over_state.LoadContent(content);
        }

        public void TransitionToGameState(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.Player:
                    game_state.SetPlayerController();
                    break;
                case ControllerType.Simple:
                    game_state.SetSimpleController();
                    break;
                case ControllerType.NeuralNetwork:
                    var ofd = new OpenFileDialog();
                    ofd.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    ofd.DefaultExt = ".snake"; // Default file extension
                    ofd.Filter = "Snake (.snake)|*.snake"; // Filter files by extension

                    // Show open file dialog box
                    bool? result = ofd.ShowDialog();

                    // Process open file dialog box results
                    if (result == true)
                    {
                        var chromosome = JsonUtils.ReadFromFile<Chromosome>(ofd.FileName);
                        game_state.SetNeuralNetworkController(chromosome);
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown controller type - {controller}");
            }

            CurrentState = game_state;
            CurrentState.Reset();
        }

        public void TransitionToGameOverState(Snake snake)
        {
            game_over_state.Set(snake);
            CurrentState = game_over_state;
        }

        public void TransitionToMainMenuState()
        {
            CurrentState = main_menu_state;
        }

        public void Exit()
        {
            exit_action();
        }
    }
}
