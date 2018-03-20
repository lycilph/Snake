﻿using LyCilph.Controllers;
using LyCilph.Elements;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LyCilph.States
{
    public class StateManager
    {
        private MainMenuState main_menu_state;
        private GameState game_state;
        private GameOverState game_over_state;

        public State CurrentState { get; private set; }

        public StateManager(GraphicsDevice graphics_device)
        {
            main_menu_state = new MainMenuState(this);
            game_state = new GameState(this, graphics_device);
            game_over_state = new GameOverState(this);
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
                    game_state.SetNeuralNetworkController();
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
    }
}
