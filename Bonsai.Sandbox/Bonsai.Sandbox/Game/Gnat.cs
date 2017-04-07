using Bonsai.Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonsai.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Framework.Components;
using Bonsai.Framework.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Bonsai.Sandbox.Game
{
    public class Gnat : Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable
    {
        public Gnat()
        {
            props = new PhysicalProperties
            {
                Gravity = 0.2f,
                TerminalVelocity = 20f,
            };

            drawingBox = new Rectangle(0, 0, 20, 20);
            physics = new PhysicsComponent();
            keyListeners = new List<KeyPressListener>();
        }

        PhysicalProperties props;
        Rectangle drawingBox;
        PhysicsComponent physics;
        List<KeyPressListener> keyListeners;

        public int DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsHidden { get; set; }


        public void Load(IContentLoader loader)
        {
            props.Texture = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            props.DrawingTint = Color.Red;

            setupKeyListeners();
        }

        public void Unload()
        {
        }


        public void Update(GameTime time)
        {
            physics.UpdateMovement(props, time.ElapsedGameTime.Milliseconds);


            // Key listeners
            foreach (var listener in keyListeners)
                listener.Update(time);
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            // Draw tinted box
            batch.Draw(props.Texture,
                props.Position,
                this.drawingBox,
                props.DrawingTint.Value);
        }

        void setupKeyListeners()
        {
            // [W] key
            keyListeners.Add(new KeyPressListener(Keys.W, () =>
            {
                Debug.WriteLine("[W] pressed");

                physics.ApplyForceY(5, props);

            }));

        }

    }
}
