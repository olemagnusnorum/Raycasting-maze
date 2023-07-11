using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Raycasting_maze;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D whiteRectangle;

    private int[,] mazeBitMap;

    private int[] wallHeight = new int[]{10, 20, 30, 40, 50, 60};

    private Player player = new Player(0,0);

    private bool topDownView = true;

    private bool togglePressed = false;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        MazeGenerator mazeGenerator = new MazeGenerator(5,5);
        mazeGenerator.GenerateMaze(0,0,1,1);
        this.mazeBitMap = mazeGenerator.GetMazeBitMap();

        _graphics.PreferredBackBufferHeight = 550;
        _graphics.PreferredBackBufferWidth = 550;
        _graphics.ApplyChanges();
        base.Initialize();
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
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        if (topDownView)
        {
            _spriteBatch.Begin();
            this.DrawMaze(_spriteBatch);
            this.DrawPlayer(_spriteBatch);
            _spriteBatch.End();
        }
        else 
        {
            GraphicsDevice.Clear(Color.Black);
        }
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    private void DrawMaze(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < mazeBitMap.GetLength(0); i++)
        {
            for (int j = 0; j < mazeBitMap.GetLength(1); j++)
            {
                if (mazeBitMap[i,j] == 1)
                {
                    _spriteBatch.Draw(this.whiteRectangle, new Rectangle(50*j,50*i,50,50), Color.White);
                }
                else
                {
                    _spriteBatch.Draw(this.whiteRectangle, new Rectangle(50*j,50*i,50,50), Color.Black);
                }
            }
        }
    }

    private void DrawPlayer(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(this.whiteRectangle, new Rectangle(this.player.getX(), this.player.getY(), 10, 10), Color.Pink);
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

    /// test method

   
}
