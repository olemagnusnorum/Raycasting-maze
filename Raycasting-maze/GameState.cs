using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Raycasting_maze;

public class GameState
{

    public int[,] MazeBitMap { get; private set; }

    public Player Player { get; private set; } = new Player(1.5f, 1.5f, 0, 1, -0.66f, 0);

    private bool TopDownView = true;

    private bool TogglePressed = false;

    private readonly double RotSpeed = 0.05;
    private readonly float WalkSpeed = 0.03f;

    public float PlayerAngle { get; private set; } = 90.0f;
    public GameState()
    {

    }

    public void Initialize()
    {
        MazeGenerator mazeGenerator = new MazeGenerator(10, 15);
        mazeGenerator.GenerateMaze(0, 0, 1, 1);
        MazeBitMap = mazeGenerator.GetMazeBitMap();
        //mazeBitMap = new int[,] {{1,1,1,1,1,1,1,1},{1,0,0,0,0,0,0,1},{1,0,0,0,0,0,0,1},{1,0,0,0,0,0,0,1},{1,1,1,1,1,1,1,1}};
    }

    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        CheckToggleView();
        CheckMovement();
    }

    public void CheckToggleView()
    {
        // toggling pov
        if (!TogglePressed && Keyboard.GetState().IsKeyDown(Keys.T) && TopDownView)
        {
            TogglePressed = true;
            TopDownView = false;

        }
        // toggling top-down view
        if (!TogglePressed && Keyboard.GetState().IsKeyDown(Keys.T) && !TopDownView)
        {
            TogglePressed = true;
            TopDownView = true;
        }
        if (TogglePressed && Keyboard.GetState().IsKeyUp(Keys.T))
        {
            TogglePressed = false;
        }
    }

    public void CheckMovement()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
        {
            RotateLeft(RotSpeed);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            RotateRight(RotSpeed);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            MoveForward(WalkSpeed);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            MoveBackwards(WalkSpeed);
        }
    }

    public bool IsTopDownView()
    {
        return TopDownView;
    }

    public bool RayOutOfBounds(int cellX, int cellY)
    {
        if (cellX < 0) return true;
        if (cellX >= MazeBitMap.GetLength(1)) return true;
        if (cellY < 0) return true;
        if (cellY >= MazeBitMap.GetLength(0)) return true;
        return false;
    }

    private void RotateLeft(double rotSpeed)
    {
        float oldDirX = Player.DirX;
        float dirX = Player.DirX * (float)Math.Cos(-rotSpeed) - Player.DirY * (float)Math.Sin(-rotSpeed);
        if (dirX > 1) dirX = 1;
        if (dirX < -1) dirX = -1;
        Player.SetDirX(dirX);
        float dirY = oldDirX * (float)Math.Sin(-rotSpeed) + Player.DirY * (float)Math.Cos(-rotSpeed);
        if (dirY > 1) dirY = 1;
        if (dirY < -1) dirY = -1;
        Player.SetDirY(dirY);
        float oldPlaneX = Player.CameraPlaneX;
        Player.SetCameraPlaneX(Player.CameraPlaneX * (float)Math.Cos(-rotSpeed) - Player.CameraPlaneY * (float)Math.Sin(-rotSpeed));
        Player.SetCameraPlaneY(oldPlaneX * (float)Math.Sin(-rotSpeed) + Player.CameraPlaneY * (float)Math.Cos(-rotSpeed));

        // finding player angle
        PlayerAngle = (float)(180 / Math.PI) * (float)Math.Acos((double)Player.DirX);
        if (Player.DirY < 0) PlayerAngle = 360 - PlayerAngle;
    }

    private void RotateRight(double rotSpeed)
    {
        float oldDirX = Player.DirX;
        float dirX = Player.DirX * (float)Math.Cos(rotSpeed) - Player.DirY * (float)Math.Sin(rotSpeed);
        if (dirX > 1) dirX = 1;
        if (dirX < -1) dirX = -1;
        Player.SetDirX(dirX);
        float dirY = oldDirX * (float)Math.Sin(rotSpeed) + Player.DirY * (float)Math.Cos(-rotSpeed);
        if (dirY > 1) dirY = 1;
        if (dirY < -1) dirY = -1;
        Player.SetDirY(dirY);
        float oldPlaneX = Player.CameraPlaneX;
        Player.SetCameraPlaneX(Player.CameraPlaneX * (float)Math.Cos(-rotSpeed) - Player.CameraPlaneY * (float)Math.Sin(rotSpeed));
        Player.SetCameraPlaneY(oldPlaneX * (float)Math.Sin(rotSpeed) + Player.CameraPlaneY * (float)Math.Cos(rotSpeed));

        //finding player angle fix so it does not just go from 0 to 180, but 0 to 360
        PlayerAngle = (float)(180 / Math.PI) * (float)Math.Acos((double)Player.DirX);
        if (Player.DirY < 0) PlayerAngle = 360 - PlayerAngle;
    }

    private void MoveForward(float walkSpeed)
    {
        float prevPosX = Player.PosX;
        float nextPosX = prevPosX + Player.DirX * walkSpeed;
        if (!PlayerInsideWall(nextPosX + Player.DirX * Player.Size, Player.PosY))
        {
            Player.SetPosX(nextPosX);
        }
        float prevPosY = Player.PosY;
        float nextPosY = prevPosY + Player.DirY * walkSpeed;
        if (!PlayerInsideWall(Player.PosX, nextPosY + Player.DirY * Player.Size))
        {
            Player.SetPosY(nextPosY);
        }
    }

    private void MoveBackwards(float walkSpeed)
    {
        float prevPosX = Player.PosX;
        float nextPosX = prevPosX - Player.DirX * walkSpeed;
        if (!PlayerInsideWall(nextPosX - Player.DirX * Player.Size, Player.PosY))
        {
            Player.SetPosX(nextPosX);
        }
        float prevPosY = Player.PosY;
        float nextPosY = prevPosY - Player.DirY * walkSpeed;
        if (!PlayerInsideWall(Player.PosX, nextPosY - Player.DirY * Player.Size))
        {
            Player.SetPosY(nextPosY);
        }
    }

    public bool PlayerInsideWall(float posX, float posY)
    {
        // TODO: make sure all sides of player is checked
        return MazeBitMap[(int)posY, (int)posX] > 0;
    }
}


