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
using System.Threading.Tasks;

namespace Bonsai.Framework
{
    public abstract class BonsaiGame : Game
    {
        public BonsaiGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            GameObjects = new List<object>();
        }

        protected List<object> GameObjects { get; private set; }
        protected GraphicsDeviceManager Graphics { get; private set; }
        protected SpriteBatch SpriteBatch { get; private set; }
        protected IContentLoader Loader { get; set; }
        protected ICamera Camera { get; set; }
        bool isWindowSet;
        Color backgroundColor = Color.Black;


        /// <summary>
        /// Sets up a standard Loader and Camera
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// Calls .Load() on all ILoadable objects found in the GameObjects collection in parallel
        /// </summary>
        /// <param name="loader">Content loader implementation chosen in Init()</param>
        protected virtual void Load(IContentLoader loader)
        {
            var loadables = GameObjects.OfType<ILoadable>();
            var tasks = new List<Task>();

            // Load in parallel
            foreach (var obj in loadables)
                tasks.Add(Task.Factory.StartNew(() => obj.Load(loader)));

            // Block until all is loaded
            Task.WaitAll(tasks.ToArray());

        }

        /// <summary>
        /// Calls .Unload() on all ILoadables found in the GameObjects collection
        /// </summary>
        protected virtual void Unload()
        {
            // Call unload on all ILoadables
            GameObjects.OfType<ILoadable>()
                       .ToList()
                       .ForEach(l => l.Unload());
        }

        /// <summary>
        /// Calls .Draw() on all IDrawable objects found in GameObjects based on .DrawOrder/.IsHidden. 
        /// Also handles applying the Camera's matrix based on value of IDrawable.IsAttachedToCamera. 
        /// Draws objects that are not attached to camera first.
        /// </summary>
        protected override void Draw(GameTime time)
        {
            // Clear
            GraphicsDevice.Clear(backgroundColor);

            // Try get camera
            // Drawable objects
            var objects = GameObjects.OfType<IDrawable>()
                                           .Where(d => !d.IsHidden)
                                           .OrderBy(d => d.DrawOrder);

            // ------------- DRAW OBJS NOT ATTACHED TO CAMERA --------------

            // Apply camera transform
            SpriteBatch.Begin(SpriteSortMode.Deferred,
                              BlendState.AlphaBlend,
                              null, null, null, null,
                              this.Camera.Transform);

            // Draw all objs not attached to camera
            foreach (var obj in objects.Where(o => !o.IsAttachedToCamera))
                obj.Draw(time, SpriteBatch);

            SpriteBatch.End();

            // --------------- DRAW OBJS ATTACHED TO CAMERA ---------------

            SpriteBatch.Begin();

            // Draw all objs attached to camera
            foreach (var obj in objects.Where(o => o.IsAttachedToCamera))
                obj.Draw(time, SpriteBatch);

            SpriteBatch.End();

            // --------------- --------------- --------------- -----------
        }

        /// <summary>
        /// Calls .Update(time) on all IUpdateable objects found in GameObjects.
        /// </summary>
        protected override void Update(GameTime time)
        {
            var updateable = GameObjects.OfType<IUpdateable>();

            foreach (var obj in updateable.Where(u => !u.IsDisabled))
                obj.Update(time);

        }

        /// <summary>
        /// Sets up the game window
        /// </summary>
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

            // Mouse
            SetMouse(showMouse);

        }

        /// <summary>
        /// Toggle mouse visibility
        /// </summary>
        protected void SetMouse(bool isVisible)
        {
            this.IsMouseVisible = isVisible;
        }

        /// <summary>
        /// Sealed XNA framework method. Delegates to Init() override, then sets up defaults to core services if they're not provided.
        /// </summary>
        protected sealed override void Initialize()
        {
            // Create default camera
            // This is done BEFORE Init() incase it is referenced
            if (Camera == null)
                Camera = new SimpleCamera(GraphicsDevice.Viewport);

            // Init override
            Init();

            // Create default content loader
            if (Loader == null)
                Loader = new SimpleContentLoader(Content);

            // Create drawing services
            this.SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Add camera for updating
            GameObjects.Add(Camera);

            // Pass back to XNA.
            // If not, then the LoadContent() chain doesn't get called!
            base.Initialize();
        }

        /// <summary>
        /// Sealed XNA framework method. Validates Camera and window is setup, then delegates to Load(). 
        /// Ensures base.LoadContent() is the last thing called.
        /// </summary>
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

        /// <summary>
        /// Calls Unload() override, cleans up Loader, and ensures base.UnloadContent() is the last thing called.
        /// </summary>
        protected sealed override void UnloadContent()
        {
            // Call overridden unload()
            Unload();

            // Cleanup content loader
            if (Loader != null)
                Loader.Cleanup();

            // Pass back to XNA
            base.UnloadContent();
        }

    }
}
