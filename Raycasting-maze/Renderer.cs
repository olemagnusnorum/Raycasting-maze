using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;

namespace Raycasting_maze;

public class Renderer
{

    private readonly int TextureWidth = 64;
    private readonly int TextureHeight = 64;
    private readonly float MinimapScale = 0.2f;

    private int Fov = 66;

    readonly ContentManager Content;
    private readonly GameState GameState;
    private int CellSize;
    private readonly int ScreenHeight;
    private readonly int ScreenWidth;

    private Texture2D WhiteRectangle;
    private Texture2D WallTexture;

    private Texture2D BarrelTexture;
    private Texture2D SkyTexture;
    private Texture2D FloorTexture;
    private Texture2D FloorAndCeilingTexture;
    private readonly Color[] FloorTextureColors;

    private readonly List<RaycastingResult> RaycastingResults = new List<RaycastingResult>();
    private readonly Color[] FloorTextureBuffer;

    private readonly List<int> IndexColored = new List<int>();

    public Renderer(ContentManager content, GameState gameState, int screenHeight, int screenWidth)
    {
        GameState = gameState;
        Content = content;
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
        FloorTextureBuffer = new Color[screenHeight * screenWidth];
        FloorTextureColors = new Color[TextureWidth * TextureHeight];
    }

    public void Initialize()
    {
        CellSize = CalculateCellSize();
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        WhiteRectangle = new Texture2D(graphicsDevice, 1, 1);
        WhiteRectangle.SetData(new[] { Color.White });
        WallTexture = Content.Load<Texture2D>("brickwall");
        BarrelTexture = Content.Load<Texture2D>("barrel");
        SkyTexture = Content.Load<Texture2D>("moonbackground");
        FloorTexture = Content.Load<Texture2D>("grassfloor");

        FloorAndCeilingTexture = new Texture2D(graphicsDevice, ScreenWidth, ScreenHeight);
        FloorTexture.GetData<Color>(FloorTextureColors);
    }

    public void Update()
    {
        if (!GameState.IsTopDownView())
        {
            Raycasting(ScreenWidth, ScreenHeight);
        }
    }

    private int CalculateCellSize()
    {
        int sizeX = ScreenWidth / GameState.MazeBitMap.GetLength(1);
        int sizeY = ScreenHeight / GameState.MazeBitMap.GetLength(0);
        return (sizeX < sizeY) ? sizeX : sizeY;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (GameState.IsTopDownView())
        {
            DrawTopDownMaze(spriteBatch, 1.0f);
            DrawPlayer(spriteBatch, 1.0f);
        }
        else
        {
            spriteBatch.Draw(WhiteRectangle, new Rectangle(0, 0, ScreenWidth, ScreenHeight / 2), Color.Green);
            spriteBatch.Draw(WhiteRectangle, new Rectangle(0, ScreenHeight / 2, ScreenWidth, ScreenHeight / 2), Color.CornflowerBlue);
            DrawSky(spriteBatch);
            DrawWalls(spriteBatch);
            DrawFloor(spriteBatch);
            DrawMinimap(spriteBatch, 0.2f);
        }
    }

    private void DrawTopDownMaze(SpriteBatch spriteBatch, float scale)
    {
        int[,] mazeBitMap = GameState.MazeBitMap;
        for (int i = 0; i < mazeBitMap.GetLength(0); i++)
        {
            for (int j = 0; j < mazeBitMap.GetLength(1); j++)
            {
                if (mazeBitMap[i, j] > 0)
                {
                    spriteBatch.Draw(WhiteRectangle, new Rectangle((int)(CellSize * scale) * j, (int)(CellSize * scale) * i, (int)(CellSize * scale), (int)(CellSize * scale)), Color.White);
                }
                else
                {
                    spriteBatch.Draw(WhiteRectangle, new Rectangle((int)(CellSize * scale) * j, (int)(CellSize * scale) * i, (int)(CellSize * scale), (int)(CellSize * scale)), Color.Black);
                }
            }
        }
    }

    private void DrawPlayer(SpriteBatch spriteBatch, float scale)
    {
        spriteBatch.Draw(
            WhiteRectangle,
            new Rectangle(((int)(CellSize * scale) * (int)GameState.Player.PosX + (int)(CellSize * MinimapScale)),
                        ((int)(CellSize * scale) * (int)GameState.Player.PosY + (int)(CellSize * MinimapScale)),
                        (int)(CellSize * scale) - 2 * (int)(CellSize * MinimapScale),
                        (int)(CellSize * scale) - 2 * (int)(CellSize * MinimapScale)),
            Color.Orange);
    }


    private void DrawMinimap(SpriteBatch spriteBatch, float scale)
    {
        DrawTopDownMaze(spriteBatch, scale);
        DrawPlayer(spriteBatch, scale);
    }

    private void DrawWalls(SpriteBatch spriteBatch)
    {
        foreach (RaycastingResult result in RaycastingResults)
        {
            int textureXIndex = (int)(TextureWidth * result.PresentageAllongTheWall);
            spriteBatch.Draw(WallTexture, new Rectangle(result.X, result.Y, result.Width, result.Height), new Rectangle(textureXIndex, 0, 1, TextureHeight), result.Shaiding);

        }
    }

    private void DrawFloor(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(FloorAndCeilingTexture, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);

    }

    private void DrawSky(SpriteBatch spriteBatch)
    {
        int x = (int)GameState.PlayerAngle;
        x = (int)(x * 16.45);
        int skyTextureWidth = 5924;
        if (x + 1100 >= skyTextureWidth)
        {
            int wrapX = (x + 1100) % 5924;
            float p1 = (skyTextureWidth - x) / 1100f;
            spriteBatch.Draw(SkyTexture, new Rectangle(0, 0, (int)(ScreenWidth * p1), ScreenHeight / 2), new Rectangle(x, 0, skyTextureWidth - x, 275), Color.White);
            spriteBatch.Draw(SkyTexture, new Rectangle((int)(ScreenWidth * p1), 0, ScreenWidth - (int)(ScreenWidth * p1), ScreenHeight / 2), new Rectangle(0, 0, wrapX, 275), Color.White);
        }
        else
        {
            spriteBatch.Draw(SkyTexture, new Rectangle(0, 0, ScreenWidth, ScreenHeight / 2), new Rectangle(x, 0, 1100, 275), Color.White);
        }
    }

    private void Raycasting(int screenWidth, int screenHeight)
    {
        RaycastingResults.Clear();
        foreach (int indexColored in IndexColored)
        {
            FloorTextureBuffer[indexColored] = Color.Transparent;
        }
        IndexColored.Clear();

        for (int x = 0; x < screenWidth; x++)
        {
            // calculate the scalar for the camera plane, to find the direction of the ray to cast
            float cameraPlaneScalar = 2 * x / (float)screenWidth - 1;

            // calculating the direction of the ray (rayDir = dv + cv * a)
            float rayDirX = GameState.Player.DirX + GameState.Player.CameraPlaneX * cameraPlaneScalar;
            float rayDirY = GameState.Player.DirY + GameState.Player.CameraPlaneY * cameraPlaneScalar;

            //calculating the ratios of the deltaDists
            float deltaDistX = (rayDirX == 0) ? 1e30f : Math.Abs(1 / rayDirX);
            float deltaDistY = (rayDirY == 0) ? 1e30f : Math.Abs(1 / rayDirY);

            // the cell the player is in
            int cellX = (int)GameState.Player.PosX;
            int cellY = (int)GameState.Player.PosY;

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
                cellWallDistX = (GameState.Player.PosX - cellX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                cellWallDistX = (cellX + 1 - GameState.Player.PosX) * deltaDistX;
            }
            // finding stepY and cellWallDistY
            if (rayDirY < 0)
            {
                stepY = -1;
                cellWallDistY = (GameState.Player.PosY - cellY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                cellWallDistY = (cellY + 1 - GameState.Player.PosY) * deltaDistY;
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
                if (GameState.RayOutOfBounds(cellX, cellY)) break;
                if (GameState.MazeBitMap[cellY, cellX] > 0)
                {
                    textureId = GameState.MazeBitMap[cellY, cellX];
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
            //if (lineHeight > screenHeight)
            //{
            //    lineHeight = screenHeight;
            //}

            // calclating where the on the wall the ray hit
            float presentageAllongTheWall;
            if (hitVericalWall)
            {
                float exactWallHit = GameState.Player.PosY + rayDirY * hitWallDist;
                presentageAllongTheWall = exactWallHit - (float)Math.Floor((double)exactWallHit);
            }
            else
            {
                float exactWallHit = GameState.Player.PosX + rayDirX * hitWallDist;
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

            //int textureXIndex = (int) (presentageAllongTheWall * textureWidth);
            int y = -lineHeight / 2 + screenHeight / 2;

            // adding shaiding
            int minShadow = 30;
            int longestDistance = 10;
            float shadowScaler = Math.Min(hitWallDist / longestDistance, 1.0f);
            shadowScaler = 1 - shadowScaler;
            shadowScaler = shadowScaler * (255 - minShadow);
            shadowScaler = shadowScaler + minShadow;
            Color shadow = new Color((int)shadowScaler, (int)shadowScaler, (int)shadowScaler);
            // adding to raycaster results
            RaycastingResults.Add(new RaycastingResult(textureId, x, y, 1, lineHeight, presentageAllongTheWall, shadow));


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
                currentDistance = screenHeight / (2.0f * pixelRow - screenHeight);
                float scale = currentDistance / distanceFromWall;
                float currentFloorX = scale * firstFloorX + (1.0f - scale) * GameState.Player.PosX;
                float currentFloorY = scale * firstFloorY + (1.0f - scale) * GameState.Player.PosY;

                int textureX = (int)(currentFloorX * TextureWidth) % TextureWidth;
                int textureY = (int)(currentFloorY * TextureHeight) % TextureHeight;
                Color floorTextureColor = FloorTextureColors[TextureWidth * textureY + textureX];
                // adding shaid to the floor
                floorShadowScaler = 1 / currentDistance;
                floorShadowScaler = floorShadowScaler * (255 - minShadow);
                floorShadowScaler = floorShadowScaler + minShadow;
                floorTextureColor.R = (byte)(int)(Convert.ToInt32(floorTextureColor.R) * floorShadowScaler / 255);
                floorTextureColor.G = (byte)(int)(Convert.ToInt32(floorTextureColor.G) * floorShadowScaler / 255);
                floorTextureColor.B = (byte)(int)(Convert.ToInt32(floorTextureColor.B) * floorShadowScaler / 255);
                FloorTextureBuffer[screenWidth * pixelRow + x] = floorTextureColor;
                //ceiling is just mirrored since they share the tile
                //floorTextureBuffer[screenWidth * (screenHeight - pixelRow) + x] = floorTextureColor;

                IndexColored.Add(screenWidth * pixelRow + x);
            }
        }

        FloorAndCeilingTexture.SetData<Color>(FloorTextureBuffer);
    }
}
