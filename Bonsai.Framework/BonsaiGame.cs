using Bonsai.Framework.Actors;
using Bonsai.Framework;
using Bonsai.Framework.Content;
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
        protected IContentLoader ContentLoader { get; private set; }
        bool isWindowSet;
        Color backgroundColor = Color.Black;


        protected abstract void Init();

        protected virtual void Load(IContentLoader contentManager)
        {
            var loadables = GameObjects.OfType<ILoadable>();

            foreach (var obj in loadables)
                obj.Load(ContentLoader);

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

            foreach (var obj in updateable.Where(u => !u.IsDisabled))
                obj.Update(frame);
        }

        protected virtual void Draw(GameFrame frame)
        {
            // Try get camera
            var camera = GameObjects.OfType<ICamera>().FirstOrDefault();

            // Apply camera transform if camera present
            if (camera != null)
                SpriteBatch.Begin(SpriteSortMode.Deferred, 
                                  BlendState.AlphaBlend, 
                                  null, null, null, null, 
                                  camera.Transform);
            else
                SpriteBatch.Begin();

            // Draw all drawable game objects
            foreach (var obj in GameObjects.OfType<IDrawable>()
                                           .Where(d => !d.IsHidden)
                                           .OrderBy(d => d.DrawOrder))
                obj.Draw(frame, SpriteBatch);

            SpriteBatch.End();
        }


        protected sealed override void Initialize()
        {
            // Create content loader
            this.ContentLoader = new StandardContentLoader(Content);
            Globals.Content = ContentLoader;

            //call overridden init()
            Init();

            base.Initialize();
        }

        protected sealed override void LoadContent()
        {
            if (isWindowSet == false)
                throw new InvalidOperationException("SetWindow() was not called in Init()");

            //globals
            Globals.Device = GraphicsDevice;

            this.SpriteBatch = new SpriteBatch(GraphicsDevice);

            //call overridden load()
            Content.RootDirectory = "Content";
            Load(this.ContentLoader);

            //pass back to xna framework pipeline
            base.LoadContent();
        }

        protected sealed override void UnloadContent()
        {
            // Call overridden unload()
            Unload();

            // Content manager
            Content.Unload();

            // Pass back to xna
            base.UnloadContent();
        }

        protected sealed override void Update(GameTime gameTime)
        {
            //update frame data
            Frame.Update(gameTime);

            //call overriden update()
            Update(this.Frame);

            //pass back to xna
            base.Update(gameTime);
        }

        protected sealed override void Draw(GameTime gameTime)
        {
            //clear
            GraphicsDevice.Clear(backgroundColor);

            //call overriden draw()
            Draw(this.Frame);

            //pass back to xna
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
