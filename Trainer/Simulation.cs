using LyCilph;
using LyCilph.Controllers;
using LyCilph.Elements;

namespace Trainer
{
    public class Simulation
    {
        public Cell food;
        public Snake snake;
        public bool alive;

        public Simulation()
        {
            food = new Cell();
            snake = new Snake();
        }

        public void Run(NeuralNetworkController controller)
        {
            snake.Reset();

            alive = true;
            while (alive)
            {
                Update(controller);
            }
        }

        private void Update(NeuralNetworkController controller)
        {
            controller.HandleInput(null);

            snake.Move();

            // Did the snake eat the food?
            if (food.Hit(snake.Head))
            {
                snake.HasEaten = true;
                CreateFood();
            }

            // Was the snake out of bounds?
            if (snake.Head.X < 0 || snake.Head.X >= Settings.board_size || snake.Head.Y < 0 || snake.Head.Y >= Settings.board_size)
            {
                snake.CauseOfDeath = "Hit border";
                alive = false;
            }

            // Did the snake hit itself?
            if (snake.HitSelf())
            {
                snake.CauseOfDeath = "Hit self";
                alive = false;
            }

            // Did the snake die of hunger?
            if (snake.Energy <= 0)
            {
                snake.CauseOfDeath = "Died of hunger";
                alive = false;
            }
        }

        private void CreateFood()
        {
            var retry = true;
            while (retry)
            {
                food.Random();
                retry = snake.Hit(food);
            }
        }
    }
}
