using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Raycasting_maze;

public class GameState {

    private int[,] mazeBitMap;

    private Player player = new Player(1.5f,1.5f, 0, 1, -0.66f, 0);

    private bool topDownView = true;

    private bool togglePressed = false;

    private List<RaycastingResult> raycastingResults = new List<RaycastingResult>();

    private double rotSpeed = 0.05;
    private float walkSpeed = 0.03f;

    public GameState (int numRays)
    {
        
    }   

    public void Initialize()
    {
        MazeGenerator mazeGenerator = new MazeGenerator(10,15);
        mazeGenerator.GenerateMaze(0,0,1,1);
        this.mazeBitMap = mazeGenerator.GetMazeBitMap();
        this.mazeBitMap = new int[,] {{1,1,1,1},{1,0,0,1},{0,0,0,1},{1,1,1,1}};
    } 


    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {    
        /// toggle view
        this.CheckToggleView();
        this.CheckMovement();
        this.Raycasting(screenWidth, screenHeight);
        // TODO: Add your update logic here
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
        //TODO;
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

    public List<RaycastingResult> GetRaycasterResults()
    {
        return this.raycastingResults;
    }

    private void Raycasting(int screenWidth, int screenHeight)
    {
        raycastingResults.Clear();
        for (int x = 0; x < screenWidth; x++)
        {
            // calculate the scalar for the camera plane, to find the direction of the ray to cast
            float cameraPlaneScalar = 2*x / (float)screenWidth - 1;
            
            // calculating the direction of the ray (rayDir = dv + cv * a)
            float rayDirX = player.GetDirX() + player.GetCameraPlaneX() * cameraPlaneScalar;
            float rayDirY = player.GetDirY() + player.GetCameraPlaneY() * cameraPlaneScalar;

            //calculating the ratios of the deltaDists
            float deltaDistX = (rayDirX == 0)? 1e30f : Math.Abs(1/rayDirX);
            float deltaDistY = (rayDirY == 0)? 1e30f : Math.Abs(1/rayDirY);

            // the cell the player is in
            int cellX = (int) player.GetPosX();
            int cellY = (int) player.GetPosY();

            // the length to the wall in the current cell
            float cellWallDistX;
            float cellWallDistY; 

            // what direction do we take a step in
            int stepX;
            int stepY;

            // finding stepX and cellWallDistX
            if (rayDirX < 0)
            {
                stepX = -1;
                cellWallDistX = (player.GetPosX() - cellX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                cellWallDistX = (cellX + 1 - player.GetPosX()) * deltaDistX;
            }
            // finding stepY and cellWallDistY
            if (rayDirY < 0)
            {
                stepY = -1;
                cellWallDistY = (player.GetPosY() - cellY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                cellWallDistY = (cellY + 1 - player.GetPosY()) * deltaDistY;
            }

            // The DDA loop for finding walls and distance
            bool hit = false;
            bool hitVericalWall = true;
            int textureId = 0;
            while (!hit)
            {
                if (cellWallDistX < cellWallDistY)
                {
                    cellWallDistX += deltaDistX;
                    cellX += stepX;
                    hitVericalWall = true;
                }
                else
                {
                    cellWallDistY += deltaDistY;
                    cellY += stepY;
                    hitVericalWall = false;
                }
                // checking if ray hit wall
                if (RayOutOfBounds(cellX, cellY)) break;
                if (mazeBitMap[cellY, cellX] > 0)
                {
                    textureId = mazeBitMap[cellY, cellX];
                    hit = true;
                }
            }
            // check if ray hit a wall
            if (hit == false) continue;

            // calculating length of the ray that hit the wall
            float hitWallDist;
            if (hitVericalWall)
            {
                hitWallDist = cellWallDistX - deltaDistX;
            }
            else 
            {
                hitWallDist = cellWallDistY - deltaDistY;
            }

            // calculate the height of the wall on screen
            int lineHeight = (int)(screenHeight / (hitWallDist));
            //if (lineHeight > this.screenHeight)
            //{
            //    lineHeight = this.screenHeight;
            //}

            int y = -lineHeight / 2 + screenHeight / 2;
            //TODO: Test with the wall texture to make the world better use public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) for each column
            // TODO: change this with a texture mapping

            // calclating where the on the wall the ray hit
            float presentageAllongTheWall;
            if (hitVericalWall)
            {
                float exactWallHit = player.GetPosY() + rayDirY * hitWallDist;
                presentageAllongTheWall = exactWallHit - (float)Math.Floor((double)exactWallHit);
            }
            else
            {
                float exactWallHit = player.GetPosX() + rayDirX * hitWallDist;
                presentageAllongTheWall = exactWallHit - (float)Math.Floor((double)exactWallHit);
            }

            // if looking wright
            if (hitVericalWall && rayDirX > 0)
            {
                presentageAllongTheWall = 1 - presentageAllongTheWall;
            }
            // if looking down
            if (!hitVericalWall && rayDirY < 0)
            {
                presentageAllongTheWall = 1 - presentageAllongTheWall;
            }
            
            //int textureXIndex = (int) (presentageAllongTheWall * this.textureWidth);

            // adding shaiding
            int minShadow = 30;
            int longestDistance = 10;
            float shadowScaler = Math.Min(hitWallDist/longestDistance, 1.0f);
            shadowScaler = 1 - shadowScaler;
            shadowScaler = shadowScaler * (255 - minShadow);
            shadowScaler = shadowScaler + minShadow;
            Color shadow = new Color((int)shadowScaler, (int)shadowScaler, (int)shadowScaler);

            // adding to raycaster results
            this.raycastingResults.Add(new RaycastingResult(textureId, x, y, 1, lineHeight, presentageAllongTheWall, shadow));
        }
    }


    private bool RayOutOfBounds(int cellX, int cellY)
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

   private bool PlayerInsideWall(float posX, float posY)
   {
        return this.mazeBitMap[(int) posY, (int) posX] > 0;
   }
}


