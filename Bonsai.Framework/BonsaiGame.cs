using Bonsai.Framework.Actors;
using Bonsai.Framework.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public abstract class BonsaiGame : Game
    {
        public BonsaiGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Frame = new GameFrame();

            GameObjects = new List<BonsaiGameObject>();
        }

        protected GraphicsDeviceManager Graphics { get; private set; }
        protected GameFrame Frame { get; private set; }
        protected SpriteBatch SpriteBatch { get; private set; }
        protected List<BonsaiGameObject> GameObjects { get; private set; }
        bool isWindowSet;
        Color backgroundColor = Color.Black;


        protected abstract void Init();

        protected virtual void Load(ContentManager contentManager)
        {
            var loadables = GameObjects.OfType<ILoadable>();

            foreach (var obj in loadables)
                obj.Load(contentManager);

        }

        protected virtual void Unload()
        {
            var loadables = GameObjects.OfType<ILoadable>();

            foreach (var obj in loadables)
                obj.Unload();
        }

        protected virtual void Update(GameFrame frame)
        {
            var updateable = GameObjects.OfType<IUpdateable>();

            foreach (var obj in updateable)
                obj.Update(frame);
        }

        protected virtual void Draw(GameFrame frame)
        {
            SpriteBatch.Begin();

            var drawable = GameObjects.OfType<IDrawable>();

            foreach (var obj in drawable.OrderBy(d => d.DrawOrder))
                obj.Draw(frame, SpriteBatch);

            SpriteBatch.End();
        }



        protected sealed override void Initialize()
        {
            //call overridden init()
            Init();

            base.Initialize();
        }

        protected override sealed void LoadContent()
        {
            if (isWindowSet == false)
                throw new InvalidOperationException("SetWindow() was not called in Init()");

            //globals
            Globals.Device = GraphicsDevice;
            Globals.Content = Content;

            this.SpriteBatch = new SpriteBatch(GraphicsDevice);

            //call overridden load()
            Content.RootDirectory = "Content";
            Load(this.Content);

            //pass back to xna framework pipeline
            base.LoadContent();
        }

        protected override sealed void UnloadContent()
        {
            Content.Unload();

            //call overridden unload()
            Unload();

            //pass back to xna framework pipeline
            base.UnloadContent();
        }

        protected override sealed void Update(GameTime gameTime)
        {
            //update frame data
            Frame.Update(gameTime);

            //call overriden update()
            Update(this.Frame);

            //pass back to xna framework pipeline
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //clear
            GraphicsDevice.Clear(backgroundColor);

            //call overriden draw()
            Draw(this.Frame);

            //pass back to xna framework pipeline
            base.Update(gameTime);
        }

        protected void SetWindow(int width, int height, bool showMouse)
        {
            //adjust window resolution
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.ApplyChanges();

            //track resolution changes globally
            Globals.Viewport = Graphics.GraphicsDevice.Viewport.Bounds;
            Globals.Viewport_Centerpoint = new Vector2(Globals.Viewport.Width * 0.5f, Globals.Viewport.Height * 0.5f);
            Globals.Window_Position = new Vector2(this.Window.ClientBounds.X, this.Window.ClientBounds.Y);

            // Mouse
            SetMouse(showMouse);

            isWindowSet = true;
        }

        protected void SetMouse(bool isVisible)
        {
            this.IsMouseVisible = isVisible;
        }

    }
}
