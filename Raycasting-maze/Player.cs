
namespace Raycasting_maze
{
    public class Player
    {

        private float size = 0.2f;
        private float posX;
        private float posY;

        private float dirX;
        private float dirY;

        // camera vector
        private float cameraPlaneX;
        private float cameraPlaneY;

        public Player(float posX, float posY, float dirX, float dirY, float cameraPlaneX, float cameraPlaneY)
        {
            this.posX = posX;
            this.posY = posY;
            this.dirX = dirX;
            this.dirY = dirY;
            this.cameraPlaneX = cameraPlaneX;
            this.cameraPlaneY = cameraPlaneY;
        }

        public float GetPosX()
        {
            return this.posX;
        }
        public float GetPosY()
        {
            return this.posY;
        }
        public void setPosX(float posX)
        {
            this.posX = posX;
        }
        public void setPosY(float posY)
        {
            this.posY = posY;
        }
        public float GetDirX()
        {
            return this.dirX;
        }
        public void SetDirX(float dirX){
            this.dirX = dirX;
        }
        public float GetDirY()
        {
            return this.dirY;
        }
        public void SetDirY(float dirY){
            this.dirY = dirY;
        }
        public float GetCameraPlaneX()
        {
            return this.cameraPlaneX;
        }
        public void SetCameraPlaneX(float cameraPlaneX){
            this.cameraPlaneX = cameraPlaneX;
        }
        public float GetCameraPlaneY()
        {
            return this.cameraPlaneY;
        }
        public void SetCameraPlaneY(float cameraPlaneY){
            this.cameraPlaneY = cameraPlaneY;
        }

        public float GetSize()
        {
            return this.size;
        }
    }
}