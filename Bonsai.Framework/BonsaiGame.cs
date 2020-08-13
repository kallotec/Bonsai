using Bonsai.Framework.Actors;
using Bonsai.Framework;
using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bonsai.Framework.Graphics;

namespace Bonsai.Framework
{
    public abstract class BonsaiGame : Game
    {
        public BonsaiGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Current = this;
        }

        public static BonsaiGame Current { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public IContentLoader Loader { get; set; }
        public ICamera Camera { get; set; }
        bool isWindowSet;

        public Rectangle ScreenBounds => new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height);
        public Vector2 ScreenCenter => new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height / 2);

        protected abstract void Init();
        protected abstract void Load(IContentLoader loader);
        protected abstract void Unload();


        protected sealed override void Initialize()
        {
            // Init override
            Init();

            // Services
            this.Loader = new SimpleContentLoader(Content);
            this.SpriteBatch = new SpriteBatch(GraphicsDevice);

            FrameworkGlobals.Pixel = TextureFormatter.CreatePixel(GraphicsDevice, 255);
            FrameworkGlobals.PixelHalfTrans = TextureFormatter.CreatePixel(GraphicsDevice, 120);

            // Pass back to XNA. If not, then the LoadContent() chain doesn't get called!
            base.Initialize();
        }

        protected sealed override void LoadContent()
        {
            // Enforce the SetWindow() call
            if (isWindowSet == false)
                throw new InvalidOperationException("SetWindow() was not called in Init()");

            // Call overridden load
            Load(this.Loader);

            // Pass back to XNA
            base.LoadContent();
        }

        protected sealed override void UnloadContent()
        {
            // Call overridden unload()
            Unload();

            // Components
            Loader?.Cleanup();
            SpriteBatch?.Dispose();

            // Pass back to XNA
            base.UnloadContent();
        }

        protected void SetWindow(string windowTitle, int width, int height, bool showMouse)
        {
            if (isWindowSet)
                return;

            isWindowSet = true;
            base.Window.Title = windowTitle;

            // Adjust window resolution
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.ApplyChanges();

            SetMouse(showMouse);

        }

        protected void SetMouse(bool isVisible)
        {
            this.IsMouseVisible = isVisible;
        }

    }
}
