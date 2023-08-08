using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;

namespace Raycasting_maze;

public class Renderer {

    private int textureWidth = 64;
    private int textureHeight = 64;
    private float minimapScale = 0.2f;

    ContentManager content;
    private GameState gs;
    private int cellSize;
    private int screenHeight;
    private int  screenWidth;

    private Texture2D whiteRectangle;
    private Texture2D wallTexture;
    private Texture2D floorTexture;
    private Color[] floorTextureColors;
    
    private List<RaycastingResult> raycastingResults = new List<RaycastingResult>();
    private Color[] floorTextureBuffer;

    public Renderer(ContentManager content, GameState gameState, int screenHeight, int screenWidth)
    {
        this.gs = gameState;
        this.content = content;
        this.screenHeight = screenHeight;
        this.screenWidth = screenWidth;
        this.floorTextureBuffer = new Color[this.screenHeight*this.screenWidth];
        this.floorTextureColors = new Color[this.textureWidth*this.textureHeight];
    }

    public void Initialize()
    {
        this.cellSize = this.CalculateCellSize();
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        whiteRectangle = new Texture2D(graphicsDevice, 1, 1);
        whiteRectangle.SetData(new[] {Color.White});
        wallTexture = this.content.Load<Texture2D>("brickwall");
        floorTexture = new Texture2D(graphicsDevice, this.screenWidth, this.screenHeight);
        wallTexture.GetData<Color>(floorTextureColors);
    }

    public void Update()
    {
        this.Raycasting(this.screenWidth, this.screenHeight);
    }

    private int CalculateCellSize()
    {
        int sizeX = this.screenWidth/gs.GetMazeBitMap().GetLength(1);
        int sizeY = this.screenHeight/gs.GetMazeBitMap().GetLength(0);
        return (sizeX < sizeY)? sizeX : sizeY;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (gs.IsTopDownView())
        {
            this.DrawTopDownMaze(spriteBatch, 1.0f);
            this.DrawPlayer(spriteBatch, 1.0f);
        }
        else 
        {
            spriteBatch.Draw(this.whiteRectangle, new Rectangle(0, 0, this.screenWidth, this.screenHeight/2), Color.Black);
            spriteBatch.Draw(this.whiteRectangle, new Rectangle(0, this.screenHeight/2, this.screenWidth, this.screenHeight/2), Color.CornflowerBlue);
            this.DrawWalls(spriteBatch);
            this.DrawFloor(spriteBatch);
            this.DrawMinimap(spriteBatch, 0.2f);
            
        }
    }

    private void DrawTopDownMaze(SpriteBatch spriteBatch, float scale)
    {
        int[,] mazeBitMap = gs.GetMazeBitMap();
        for (int i = 0; i < mazeBitMap.GetLength(0); i++)
        {
            for (int j = 0; j < mazeBitMap.GetLength(1); j++)
            {
                if (mazeBitMap[i,j] > 0)
                {
                    spriteBatch.Draw(this.whiteRectangle, new Rectangle((int)(this.cellSize*scale)*j, (int)(this.cellSize*scale)*i, (int)(this.cellSize*scale), (int)(this.cellSize*scale)), Color.White);
                }
                else
                {
                    spriteBatch.Draw(this.whiteRectangle, new Rectangle((int)(this.cellSize*scale)*j, (int)(this.cellSize*scale)*i, (int)(this.cellSize*scale), (int)(this.cellSize*scale)), Color.Black);
                }
            }
        }
    }

    private void DrawPlayer(SpriteBatch spriteBatch, float scale)
    {
        spriteBatch.Draw(
            this.whiteRectangle, 
            new Rectangle(((int)(this.cellSize*scale)*(int)gs.GetPlayer().GetPosX() + (int)(this.cellSize*this.minimapScale)), 
                        ((int)(this.cellSize*scale)*(int)gs.GetPlayer().GetPosY() + (int)(this.cellSize*this.minimapScale)), 
                        (int)(this.cellSize*scale) - 2*(int)(this.cellSize*this.minimapScale), 
                        (int)(this.cellSize*scale) - 2*(int)(this.cellSize*this.minimapScale)), 
            Color.Orange);
    }


    private void DrawMinimap(SpriteBatch spriteBatch, float scale)
    {
        this.DrawTopDownMaze(spriteBatch, scale);
        this.DrawPlayer(spriteBatch, scale);
    }

    private void DrawWalls(SpriteBatch spriteBatch)
    {
        foreach(RaycastingResult result in this.raycastingResults)
        {
            int textureXIndex = (int)(this.textureWidth * result.PresentageAllongTheWall);
            spriteBatch.Draw(this.wallTexture, new Rectangle(result.X, result.Y, result.Width, result.Height), new Rectangle(textureXIndex, 0, 1, this.textureHeight), result.Shaiding);
            
        }
    }

    private void DrawFloor(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(this.floorTexture, new Rectangle(0, 0, this.screenWidth, this.screenHeight), Color.White);
    }

    private void Raycasting(int screenWidth, int screenHeight)
    {
        raycastingResults.Clear();
        for (int i = 0; i < this.floorTextureBuffer.Length; i++) 
        {
            this.floorTextureBuffer[i] = Color.Transparent;
        }
    
        for (int x = 0; x < screenWidth; x++)
        {
            // calculate the scalar for the camera plane, to find the direction of the ray to cast
            float cameraPlaneScalar = 2*x / (float)screenWidth - 1;
            
            // calculating the direction of the ray (rayDir = dv + cv * a)
            float rayDirX = gs.GetPlayer().GetDirX() + gs.GetPlayer().GetCameraPlaneX() * cameraPlaneScalar;
            float rayDirY = gs.GetPlayer().GetDirY() + gs.GetPlayer().GetCameraPlaneY() * cameraPlaneScalar;

            //calculating the ratios of the deltaDists
            float deltaDistX = (rayDirX == 0)? 1e30f : Math.Abs(1/rayDirX);
            float deltaDistY = (rayDirY == 0)? 1e30f : Math.Abs(1/rayDirY);

            // the cell the player is in
            int cellX = (int) gs.GetPlayer().GetPosX();
            int cellY = (int) gs.GetPlayer().GetPosY();

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
                cellWallDistX = (gs.GetPlayer().GetPosX() - cellX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                cellWallDistX = (cellX + 1 - gs.GetPlayer().GetPosX()) * deltaDistX;
            }
            // finding stepY and cellWallDistY
            if (rayDirY < 0)
            {
                stepY = -1;
                cellWallDistY = (gs.GetPlayer().GetPosY() - cellY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                cellWallDistY = (cellY + 1 - gs.GetPlayer().GetPosY()) * deltaDistY;
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
                if (gs.RayOutOfBounds(cellX, cellY)) break;
                if (gs.GetMazeBitMap()[cellY, cellX] > 0)
                {
                    textureId = gs.GetMazeBitMap()[cellY, cellX];
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

            // calclating where the on the wall the ray hit
            float presentageAllongTheWall;
            if (hitVericalWall)
            {
                float exactWallHit = gs.GetPlayer().GetPosY() + rayDirY * hitWallDist;
                presentageAllongTheWall = exactWallHit - (float)Math.Floor((double)exactWallHit);
            }
            else
            {
                float exactWallHit = gs.GetPlayer().GetPosX() + rayDirX * hitWallDist;
                presentageAllongTheWall = exactWallHit - (float)Math.Floor((double)exactWallHit);
            }

            float presentageAllongTheFloor = presentageAllongTheWall;

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
            int y = -lineHeight / 2 + screenHeight / 2;

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

            
            // Floorcasting and Ceilingcasting
            float firstFloorX;
            float firstFloorY;

            // if checking what side we hit the wall
            if (hitVericalWall && rayDirX > 0)
            {
                firstFloorX = cellX;
                firstFloorY = cellY + presentageAllongTheFloor;
            }
            else if (hitVericalWall && rayDirX < 0)
            {
                firstFloorX = cellX + 1.0f;
                firstFloorY = cellY + presentageAllongTheFloor;
            }
            else if (!hitVericalWall && rayDirY > 0)
            {
                firstFloorX = cellX + presentageAllongTheFloor;
                firstFloorY = cellY;
            }
            else
            {
                firstFloorX = cellX + presentageAllongTheFloor;
                firstFloorY = cellY + 1.0f;
            }

            float distanceFromWall = hitWallDist;
            float currentDistance;

            int wallEnd = y + lineHeight;
            if (wallEnd < 0) wallEnd = screenHeight;
            float floorShadowScaler = 0;
            for (int pixelRow = wallEnd; pixelRow < screenHeight; pixelRow++)
            {
                currentDistance = screenHeight / (2.0f*pixelRow - screenHeight);
                float scale = currentDistance/distanceFromWall;
                float currentFloorX = scale * firstFloorX + (1.0f - scale) * gs.GetPlayer().GetPosX();
                float currentFloorY = scale * firstFloorY + (1.0f - scale) * gs.GetPlayer().GetPosY();
               
                int textureX = (int) (currentFloorX * this.textureWidth) % this.textureWidth;
                int textureY = (int) (currentFloorY * this.textureHeight) % this.textureHeight;
                Color floorTextureColor = this.floorTextureColors[this.textureWidth * textureY + textureX];
                // adding shaid to the floor
                floorShadowScaler = 1/currentDistance;
                floorShadowScaler = floorShadowScaler * (255 - minShadow);
                floorShadowScaler = floorShadowScaler + minShadow;
                floorTextureColor.R = (byte) (int)(Convert.ToInt32(floorTextureColor.R) * floorShadowScaler/255);
                floorTextureColor.G = (byte) (int)(Convert.ToInt32(floorTextureColor.G) * floorShadowScaler/255);
                floorTextureColor.B = (byte) (int)(Convert.ToInt32(floorTextureColor.B) * floorShadowScaler/255);
                this.floorTextureBuffer[this.screenWidth * pixelRow + x] = floorTextureColor;
                //ceiling is just mirrored since they share the tile
                this.floorTextureBuffer[this.screenWidth * (this.screenHeight - pixelRow) + x] = floorTextureColor;
                
            }
        }
        
        this.floorTexture.SetData<Color>(this.floorTextureBuffer);
    }

}
