using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Raycasting_maze;

public class Renderer {

    ContentManager content;
    private int cellSize;

    private int screenHeight;

    private int  screenWidth;

    private Texture2D whiteRectangle;

    private Texture2D wallTexture;

    private int textureWidth = 64;
    private int textureHeight = 64;

    private float minimapScale = 0.2f;

    private GameState gs;

    public Renderer(ContentManager content, GameState gameState, int screenHeight, int screenWidth)
    {
        this.gs = gameState;
        this.content = content;
        this.screenHeight = screenHeight;
        this.screenWidth = screenWidth;
    }

    public void Initialize()
    {
        this.cellSize = this.GetCellSize();
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        whiteRectangle = new Texture2D(graphicsDevice, 1, 1);
        whiteRectangle.SetData(new[] {Color.White});
        wallTexture = this.content.Load<Texture2D>("brickwall");
    }

    private int GetCellSize()
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
            this.DrawRaycasting(spriteBatch);
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

    private void DrawRaycasting(SpriteBatch spriteBatch)
    {
        foreach(RaycastingResult result in gs.GetRaycasterResults())
        {
            int textureXIndex = (int)(this.textureWidth * result.PresentageAllongTheWall);
            spriteBatch.Draw(this.wallTexture, new Rectangle(result.X, result.Y, result.Width, result.Height), new Rectangle(textureXIndex, 0, 1, this.textureHeight), result.Shaiding);
        }
    }

}
