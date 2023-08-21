using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Raycasting_maze;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private int screenHeight = 550;

    private int  screenWidth = 1100;

    GameState gameState;
    Renderer renderer;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        gameState = new GameState();
        renderer = new Renderer(Content, gameState, this.screenHeight, this.screenWidth);
    }

    protected override void Initialize()
    {
        this.gameState.Initialize();
        this.renderer.Initialize();

        _graphics.PreferredBackBufferHeight = this.screenHeight;
        _graphics.PreferredBackBufferWidth = this.screenWidth;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        this.renderer.LoadContent(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) 
        {
            Exit();
        }
        this.gameState.Update(gameTime, this.screenWidth, this.screenHeight);
        this.renderer.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();
        this.renderer.Draw(this._spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
