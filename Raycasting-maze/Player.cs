
namespace Raycasting_maze
{
    public class Player
    {

        public readonly float Size = 0.2f;
        public float PosX { get; private set; }
        public float PosY { get; private set; }

        public float DirX { get; private set; }
        public float DirY { get; private set; }

        // camera vector
        public float CameraPlaneX { get; private set; }
        public float CameraPlaneY { get; private set; }

        public Player(float posX, float posY, float dirX, float dirY, float cameraPlaneX, float cameraPlaneY)
        {
            PosX = posX;
            PosY = posY;
            DirX = dirX;
            DirY = dirY;
            CameraPlaneX = cameraPlaneX;
            CameraPlaneY = cameraPlaneY;
        }

        public void SetPosX(float posX)
        {
            PosX = posX;
        }
        public void SetPosY(float posY)
        {
            PosY = posY;
        }
        public void SetDirX(float dirX)
        {
            DirX = dirX;
        }
        public void SetDirY(float dirY)
        {
            DirY = dirY;
        }
        public void SetCameraPlaneX(float cameraPlaneX)
        {
            CameraPlaneX = cameraPlaneX;
        }
        public void SetCameraPlaneY(float cameraPlaneY)
        {
            CameraPlaneY = cameraPlaneY;
        }
    }
}