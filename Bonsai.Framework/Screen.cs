using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Bonsai.Framework;

namespace Bonsai.Framework
{
    public abstract class Screen
    {
        public Screen(Framework.BonsaiGame game)
        {
            Game = game;

            Camera = new SimpleCamera(Game.GraphicsDevice.Viewport);
            Camera.SetFocus(game.ScreenCenter, immediateFocus: true);

            GameObjects = new List<object>();
        }

        public Framework.ICamera Camera { get; set; }
        public Framework.BonsaiGame Game { get; private set; }
        protected List<object> GameObjects { get; private set; }

        /// <summary>
        /// Calls .Load() on all ILoadable objects found in the GameObjects collection in parallel
        /// </summary>
        /// <param name="loader">Content loader implementation chosen in Init()</param>
        public virtual void Load(IContentLoader loader)
        {
            var loadables = GameObjects.OfType<Framework.ILoadable>();
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
        public virtual void Unload()
        {
            // Call unload on all ILoadables
            GameObjects.OfType<Framework.ILoadable>()
                       .ToList()
                       .ForEach(l => l.Unload());
        }

        /// <summary>
        /// Calls .Update(time) on all IUpdateable objects found in GameObjects.
        /// </summary>
        public virtual void Update(GameTime time)
        {
            var updateable = GameObjects.OfType<Framework.IUpdateable>();

            foreach (var obj in updateable)
                obj.Update(time);
        }

        /// <summary>
        /// Calls .Draw() on all IDrawable objects found in GameObjects based on .DrawOrder/.IsHidden. 
        /// Also handles applying the Camera's matrix based on value of IDrawable.IsAttachedToCamera. 
        /// Draws objects that are not attached to camera first.
        /// </summary>
        public virtual void Draw(GameTime time)
        {
            // Clear
            Game.GraphicsDevice.Clear(Color.Black);

            var parentPosition = new Vector2();

            // Try get camera
            // Drawable objects
            var objects = GameObjects.OfType<Framework.IDrawable>()
                                           .Where(d => !d.IsHidden)
                                           .OrderBy(d => d.DrawOrder);

            // < 0
            var lessThanZero = objects.Where(o => o.DrawOrder < 0).ToList();
            drawObjectsAttachedToCamera(lessThanZero, time, parentPosition);
            drawObjectsNotAttachedToCamera(lessThanZero, time, parentPosition);

            // >= 0
            var zeroOrGreater = objects.Where(o => o.DrawOrder >= 0).ToList();
            drawObjectsNotAttachedToCamera(zeroOrGreater, time, parentPosition);
            drawObjectsAttachedToCamera(zeroOrGreater, time, parentPosition);

        }

        protected Ephemeral<SpriteBatch> StartDrawing()
        {
            return new Ephemeral<SpriteBatch>(() => 
            {
                Game.SpriteBatch.Begin();
                return Game.SpriteBatch;
            }, 
            (b) => b.End());
        }

        void drawObjectsAttachedToCamera(List<Framework.IDrawable> drawables, GameTime time, Vector2 parentPosition)
        {
            using (var drawer = StartDrawing())
            {
                // Draw all objs attached to camera
                foreach (var obj in drawables.Where(o => o.IsAttachedToCamera))
                    obj.Draw(time, drawer.Value, parentPosition);
            }
        }

        void drawObjectsNotAttachedToCamera(List<Framework.IDrawable> drawables, GameTime time, Vector2 parentPosition)
        {
            // Apply camera transform
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,
                              BlendState.AlphaBlend,
                              null, null, null, null,
                              Camera.Transform);

            // Draw all objs not attached to camera
            foreach (var obj in drawables.Where(o => !o.IsAttachedToCamera))
                obj.Draw(time, Game.SpriteBatch, parentPosition);

            Game.SpriteBatch.End();
        }

    }

    public class Ephemeral<T> : IDisposable
    {
        public Ephemeral(Func<T> init, Action<T> dispose)
        {
            value = init();

            this.dispose = dispose;
        }

        T value;
        Action<T> dispose;

        public T Value => value;

        public void Dispose()
        {
            dispose(value);
        }
    }
}
