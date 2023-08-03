using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Raycasting_maze;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D whiteRectangle;

    private int[,] mazeBitMap;

    private Player player = new Player(1.5f,1.5f, 0, 1, -0.66f, 0);

    private bool topDownView = true;

    private bool togglePressed = false;

    private int screenHeight = 550;

    private int  screenWidth = 1100;

    private double rotSpeed = 0.05;
    private float walkSpeed = 0.03f;

    private int cellSize;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        MazeGenerator mazeGenerator = new MazeGenerator(10,15);
        mazeGenerator.GenerateMaze(0,0,1,1);
        this.mazeBitMap = mazeGenerator.GetMazeBitMap();
        this.cellSize = this.GetCellSize();
        
        _graphics.PreferredBackBufferHeight = this.screenHeight;
        _graphics.PreferredBackBufferWidth = this.screenWidth;
        _graphics.ApplyChanges();
        base.Initialize();
        Console.WriteLine(this.cellSize);
        
    }

    protected override void LoadContent()
    {
        
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
        whiteRectangle.SetData(new[] {Color.White});
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        /// toggle view
        this.CheckToggleView();
        this.CheckMovement();
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        if (topDownView)
        {
            _spriteBatch.Begin();
            this.DrawTopDownMaze(_spriteBatch, 1.0f);
            this.DrawPlayer(_spriteBatch, 1.0f);
            _spriteBatch.End();
        }
        else 
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(this.whiteRectangle, new Rectangle(0, 0, this.screenWidth, this.screenHeight/2), Color.Black);
            _spriteBatch.Draw(this.whiteRectangle, new Rectangle(0, this.screenHeight/2, this.screenWidth, this.screenHeight/2), Color.CornflowerBlue);
            this.Raycasting(_spriteBatch);
            this.DrawTopDownMaze(_spriteBatch, 0.2f);
            this.DrawPlayer(_spriteBatch, 0.2f);
            _spriteBatch.End();
        }
        
        base.Draw(gameTime);
    }

    private int GetCellSize()
    {
        int sizeX = this.screenWidth/this.mazeBitMap.GetLength(1);
        int sizeY = this.screenHeight/this.mazeBitMap.GetLength(0);
        return (sizeX < sizeY)? sizeX : sizeY;
    }

    private void DrawTopDownMaze(SpriteBatch spriteBatch, float scale)
    {
        for (int i = 0; i < mazeBitMap.GetLength(0); i++)
        {
            for (int j = 0; j < mazeBitMap.GetLength(1); j++)
            {
                if (mazeBitMap[i,j] > 0)
                {
                    // 50 should be replaced with cellScale
                    _spriteBatch.Draw(this.whiteRectangle, new Rectangle((int)(this.cellSize*scale)*j, (int)(this.cellSize*scale)*i, (int)(this.cellSize*scale), (int)(this.cellSize*scale)), Color.White);
                }
                else
                {
                    _spriteBatch.Draw(this.whiteRectangle, new Rectangle((int)(this.cellSize*scale)*j, (int)(this.cellSize*scale)*i, (int)(this.cellSize*scale), (int)(this.cellSize*scale)), Color.Black);
                }
            }
        }
    }

    private void DrawPlayer(SpriteBatch spriteBatch, float scale)
    {
        /// have to change this one
        spriteBatch.Draw(this.whiteRectangle, new Rectangle(((int)(this.cellSize*scale)*(int)this.player.GetPosX() + (int)(this.cellSize*0.2f)), ((int)(this.cellSize*scale)*(int)this.player.GetPosY() + (int)(this.cellSize*0.2)), (int)(this.cellSize*scale) - 2*(int)(this.cellSize*0.2), (int)(this.cellSize*scale) - 2*(int)(this.cellSize*0.2)), Color.Orange);
    }

    private void CheckToggleView()
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

    private void CheckMovement()
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

    /// raycaster method DDA
    private void Raycasting(SpriteBatch spriteBatch)
    {
        for (int x = 0; x < this.screenWidth; x++)
        {
            // calculate the scalar for the camera plane, to find the direction of the ray to cast
            float cameraPlaneScalar = 2*x / (float)this.screenWidth - 1;
            
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
                if (mazeBitMap[cellY, cellX] > 0)
                {
                    hit = true;
                }
            }

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
            int lineHeight = (int)(this.screenHeight / (hitWallDist));
            if (lineHeight > this.screenHeight)
            {
                lineHeight = this.screenHeight;
            }

            //calculate lowest and highest pixel to fill in current stripe
            int drawStart = -lineHeight / 2 + this.screenHeight / 2;
            if(drawStart < 0)
            {
                drawStart = 0;
            }
            if (mazeBitMap[cellY, cellX] == 2)
            {
                if (hitVericalWall)
                {
                    spriteBatch.Draw(this.whiteRectangle, new Rectangle(x, drawStart, 1, lineHeight), Color.Red);
                }
                else
                {
                    spriteBatch.Draw(this.whiteRectangle, new Rectangle(x, drawStart, 1, lineHeight), new Color(225, 0, 0));
                }
            }
            else
            {
                if (hitVericalWall)
                {
                    spriteBatch.Draw(this.whiteRectangle, new Rectangle(x, drawStart, 1, lineHeight), Color.White);
                }
                else
                {
                    spriteBatch.Draw(this.whiteRectangle, new Rectangle(x, drawStart, 1, lineHeight), new Color(225,225,225));
                }
            }
        }
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
        if (!PlayerInsideWall(nextPosX, player.GetPosY()))
        {
            player.setPosX(nextPosX);
        }
        float prevPosY = player.GetPosY();
        float nextPosY = prevPosY + player.GetDirY() * walkSpeed;
        if (!PlayerInsideWall(player.GetPosX(), nextPosY))
        {
            player.setPosY(nextPosY);
        }
   }

   private void MoveBackwards(float walkSpeed)
   {
        float prevPosX = player.GetPosX();
        float nextPosX = prevPosX - player.GetDirX() * walkSpeed;
        if (!PlayerInsideWall(nextPosX, player.GetPosY()))
        {
            player.setPosX(nextPosX);
        }
        float prevPosY = player.GetPosY();
        float nextPosY = prevPosY - player.GetDirY() * walkSpeed;
        if (!PlayerInsideWall(player.GetPosX(), nextPosY))
        {
            player.setPosY(nextPosY);
        }
   }

   private bool PlayerInsideWall(float posX, float posY)
   {
        return this.mazeBitMap[(int) posY, (int) posX] > 0;
   }
}
