﻿using Bonsai.Framework;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Input;
using Bonsai.Framework.Text;
using Bonsai.Framework.UI;
using Bonsai.Framework.UI.Text;
using Bonsai.Framework.Variables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class HUD : DrawableBase, IHUD
    {
        public HUD(Level level) 
        {
            // Ensure HUD follows camera around
            base.IsAttachedToCamera = true;

            this.level = level;
            fields = new List<ITextElement>();
            keyListeners = new List<KeyPressListener>();
        }

        Level level;
        SpriteFont fontGeneral;
        List<ITextElement> fields;
        GameVariable<int> fps;
        List<KeyPressListener> keyListeners;
        double fps_ms = 0;
        int frame_count = 0;
        int screenMargin = 40;

        public event EventHandler Exit;
        public bool IsDisabled { get; set; }
        public Rectangle ScreenBounds { get; set; }


        public void Load(IContentLoader loader)
        {
            // Fonts
            fontGeneral = loader.Load<SpriteFont>(ContentPaths.FONT_UI_GENERAL);

            //key listeners
            setupKeyListeners();

            // Variables
            fps = new GameVariable<int>();

            // [Fields]
            // FPS variable
            fields.Add(new TextElement<int>(
                variable: this.fps,
                settings: new TextElementSettings(fontGeneral)
                {
                    Label = "FPS: ",
                    Position = new Vector2(10, 10),
                }
            ));

            // Jumps variable
            fields.Add(new TextElement<int>(
                variable: level.Jumps,
                settings: new TextElementSettings(fontGeneral)
                {
                    Label = "Jumps: ",
                    Position = new Vector2(ScreenBounds.Width / 2, screenMargin),
                    HorizontalAlignment = TextHorizontalAlignment.Center,
                    ForegroundColor = Color.Green,
                }
            ));

            // Coins variable
            fields.Add(new TextElement<int>(
                variable: level.CoinsCount,
                settings: new TextElementSettings(fontGeneral)
                {
                    Label = "Coins: ",
                    Position = new Vector2(ScreenBounds.Width - screenMargin, screenMargin),
                    HorizontalAlignment = TextHorizontalAlignment.Right,
                    ForegroundColor = Color.Yellow,
                }
            ));

            foreach (var field in fields)
                field.Load(loader);

        }

        public void Unload()
        {
            // Unload all fields
            for (int x = 0; x < fields.Count; x++)
                fields[x].Unload();
        }

        public void Update(GameTime time)
        {
            // Calculate FPS
            fps_ms += time.ElapsedGameTime.TotalMilliseconds;
            frame_count += 1;
            if (fps_ms >= 1000.0d)
            {
                fps_ms = fps_ms - 1000.0d;
                fps.Value = frame_count;
                frame_count = 0;
            }

            // Key listeners
            foreach (var listener in keyListeners)
                listener.Update(time);

        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            for (var x = 0; x < fields.Count; x++) 
                fields[x].Draw(time, batch);
        }


        void setupKeyListeners()
        {
            // [ESC] key
            keyListeners.Add(new KeyPressListener(Keys.Escape, () => 
            {
                // Send game exit request
                this.Exit?.Invoke(this, null);
            }));

            // [P] key
            keyListeners.Add(new KeyPressListener(Keys.P, () =>
            {
                // Pause/unpause
                level.IsDisabled = !level.IsDisabled;
            }));

        }

    }
}
