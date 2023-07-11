
namespace Raycasting_maze
{
    class Player
    {
        private int x;
        private int y;

        private float theta;

        public Player(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.theta = 0;
        }

        public int getX()
        {
            return this.x;
        }

        public int getY()
        {
            return this.y;
        }
    }
}