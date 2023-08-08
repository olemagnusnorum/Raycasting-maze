using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Raycasting_maze;

public class GameState {

    private int[,] mazeBitMap;

    private Player player = new Player(1.5f, 1.5f, 0, 1, -0.66f, 0);

    private bool topDownView = true;

    private bool togglePressed = false;

    private double rotSpeed = 0.05;
    private float walkSpeed = 0.03f;

    public GameState ()
    {
        
    }   

    public void Initialize()
    {
        MazeGenerator mazeGenerator = new MazeGenerator(10,15);
        mazeGenerator.GenerateMaze(0,0,1,1);
        this.mazeBitMap = mazeGenerator.GetMazeBitMap();
        //this.mazeBitMap = new int[,] {{1,1,1,1},{1,0,0,1},{0,0,0,1},{1,1,1,1}};
    } 

    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {    
        this.CheckToggleView();
        this.CheckMovement();
    }

    public void CheckToggleView()
    {
        // toggling pov
        if (!this.togglePressed && Keyboard.GetState().IsKeyDown(Keys.T) && this.topDownView)
        {
            this.togglePressed = true;
            this.topDownView = false;
            
        }
        // toggling top-down view
        if (!this.togglePressed && Keyboard.GetState().IsKeyDown(Keys.T) && !this.topDownView)
        {   
            this.togglePressed = true;
            this.topDownView = true;
        }
        if (this.togglePressed && Keyboard.GetState().IsKeyUp(Keys.T))
        {
            this.togglePressed = false;
        }
    }

    public void CheckMovement()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
        {
            this.RotateLeft(this.rotSpeed);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            this.RotateRight(this.rotSpeed);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            this.MoveForward(this.walkSpeed);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            this.MoveBackwards(this.walkSpeed);
        }
    }

    public bool IsTopDownView()
    {
        return this.topDownView;
    }

    public int[,] GetMazeBitMap()
    {
        return this.mazeBitMap;
    }

    public Player GetPlayer()
    {
        return this.player;
    }

    public bool RayOutOfBounds(int cellX, int cellY)
    {
        if (cellX < 0) return true;
        if (cellX >= this.mazeBitMap.GetLength(1)) return true;
        if (cellY < 0) return true;
        if (cellY >= this.mazeBitMap.GetLength(0)) return true;
        return false;
    }

    private void RotateLeft(double rotSpeed)
    {
        float oldDirX = player.GetDirX();
        player.SetDirX(player.GetDirX() * (float)Math.Cos(-rotSpeed) - player.GetDirY() * (float)Math.Sin(-rotSpeed));
        player.SetDirY(oldDirX * (float)Math.Sin(-rotSpeed) + player.GetDirY() * (float)Math.Cos(-rotSpeed));
        float oldPlaneX = player.GetCameraPlaneX();
        player.SetCameraPlaneX(player.GetCameraPlaneX() * (float)Math.Cos(-rotSpeed) - player.GetCameraPlaneY() * (float)Math.Sin(-rotSpeed));
        player.SetCameraPlaneY(oldPlaneX * (float)Math.Sin(-rotSpeed) + player.GetCameraPlaneY() * (float)Math.Cos(-rotSpeed));
    }
   
   private void RotateRight(double rotSpeed)
   {
        float oldDirX = player.GetDirX();
        player.SetDirX(player.GetDirX() * (float)Math.Cos(rotSpeed) - player.GetDirY() * (float)Math.Sin(rotSpeed));
        player.SetDirY(oldDirX * (float)Math.Sin(rotSpeed) + player.GetDirY() * (float)Math.Cos(-rotSpeed));
        float oldPlaneX = player.GetCameraPlaneX();
        player.SetCameraPlaneX(player.GetCameraPlaneX() * (float)Math.Cos(-rotSpeed) - player.GetCameraPlaneY() * (float)Math.Sin(rotSpeed));
        player.SetCameraPlaneY(oldPlaneX * (float)Math.Sin(rotSpeed) + player.GetCameraPlaneY() * (float)Math.Cos(rotSpeed));
   }

   private void MoveForward(float walkSpeed)
   {
        float prevPosX = player.GetPosX();
        float nextPosX = prevPosX + player.GetDirX() * walkSpeed;
        if (!PlayerInsideWall(nextPosX + player.GetDirX() * player.GetSize(), player.GetPosY()))
        {
            player.setPosX(nextPosX);
        }
        float prevPosY = player.GetPosY();
        float nextPosY = prevPosY + player.GetDirY() * walkSpeed;
        if (!PlayerInsideWall(player.GetPosX(), nextPosY + player.GetDirY() * player.GetSize()))
        {
            player.setPosY(nextPosY);
        }
   }

   private void MoveBackwards(float walkSpeed)
   {
        float prevPosX = player.GetPosX();
        float nextPosX = prevPosX - player.GetDirX() * walkSpeed;
        if (!PlayerInsideWall(nextPosX - player.GetDirX() * player.GetSize(), player.GetPosY()))
        {
            player.setPosX(nextPosX);
        }
        float prevPosY = player.GetPosY();
        float nextPosY = prevPosY - player.GetDirY() * walkSpeed;
        if (!PlayerInsideWall(player.GetPosX(), nextPosY - player.GetDirY() * player.GetSize()))
        {
            player.setPosY(nextPosY);
        }
   }

   public bool PlayerInsideWall(float posX, float posY)
   {
        return this.mazeBitMap[(int) posY, (int) posX] > 0;
   }
}


