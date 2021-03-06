﻿using Bonsai.Framework;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Input;
using Bonsai.Framework.Text;
using Bonsai.Framework.UI;
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

        public Vector2 Position { get; set; } = new Vector2(0, 0);
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
                font: fontGeneral,
                settings: new TextElementSettings()
                {
                    Label = "FPS: ",
                    DisplayMode = TextDisplayMode.LabelAndValue
                }
            )
            {
                Position = new Vector2(10, 10)
            });

            // Jumps variable
            fields.Add(new TextElement<int>(
                variable: level.Jumps,
                font: fontGeneral,
                settings: new TextElementSettings()
                {
                    Label = "Jumps: ",
                    DisplayMode = TextDisplayMode.LabelAndValue,
                    HorizontalAlignment = TextHorizontalAlignment.Center,
                    ForegroundColor = Color.Green,
                }
            )
            {
                Position = new Vector2(ScreenBounds.Width / 2, screenMargin),
            });

            // Coins variable
            fields.Add(new TextElement<int>(
                variable: level.CoinsCount,
                font: fontGeneral,
                settings: new TextElementSettings
                {
                    Label = "Coins: ",
                    DisplayMode = TextDisplayMode.LabelAndValue,
                    HorizontalAlignment = TextHorizontalAlignment.Right,
                    ForegroundColor = Color.Yellow,
                }
            )
            {
                Position = new Vector2(ScreenBounds.Width - screenMargin, screenMargin),
            });

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

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            for (var x = 0; x < fields.Count; x++) 
                fields[x].Draw(time, batch, parentPosition);
        }


        void setupKeyListeners()
        {
            // [ESC] key
            keyListeners.Add(new KeyPressListener(Keys.Escape, () => 
            {
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
