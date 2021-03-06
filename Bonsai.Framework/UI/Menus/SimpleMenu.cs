﻿using Bonsai.Framework;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Input;
using Bonsai.Framework.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Bonsai.Framework.UI.Menus
{
    public class SimpleMenu : Framework.IDrawable, Framework.ILoadable, Framework.IUpdateable
    {
        public SimpleMenu(string fontPath, Dictionary<string, Action> itemsActionMap)
        {
            this.fontPath = fontPath;
            itemsMap = itemsActionMap;
            menuElements = new List<ITextElement>();
            actions = new List<Action>();
        }

        Vector2 position;
        string fontPath;
        Dictionary<string, Action> itemsMap;
        List<Action> actions;
        List<ITextElement> menuElements;
        List<KeyPressListener> keyListeners;
        SpriteFont font;
        int selectedMenuItemIndex = 0;
        Color menuItemForegroundColor = Color.FromNonPremultiplied(190, 190, 190, 255);
        Color selectedMenuItemForegroundColor = Color.FromNonPremultiplied(255, 255, 255, 255);
        Color selectedMenuItemBackgroundColor = Color.FromNonPremultiplied(33, 33, 33, 255);
        public Vector2 Position 
        {
            get => position;
            set
            {
                position = value;
            }
        }
        public bool IsHidden { get; set; }
        public DrawOrderPosition DrawOrder => DrawOrderPosition.HUD;
        public bool IsAttachedToCamera => true;
        public bool IsDisabled => false;


        public void Load(IContentLoader loader)
        {
            font = loader.Load<SpriteFont>(fontPath);

            var positionMarker = new Vector2();
            bool isFirst = true;

            foreach (var item in itemsMap)
            {
                menuElements.Add(new TextElement<string>(
                    item.Key,
                    font,
                    new TextElementSettings
                    {
                        HorizontalAlignment = TextHorizontalAlignment.Center,
                        DisplayMode = TextDisplayMode.ValueOnly,
                        ForegroundColor = isFirst ? selectedMenuItemForegroundColor : menuItemForegroundColor,
                        Padding = new Vector2(15, 10)
                    })
                {
                    Position = positionMarker
                });

                actions.Add(item.Value);

                positionMarker += new Vector2(0, 50);
                isFirst = false;
            }

            foreach (var item in menuElements)
                item.Load(loader);

            // Key listeners
            keyListeners = new List<KeyPressListener>
            {
                // [Up] Menu option up
                new KeyPressListener(Keys.Up, () => updateSelectedMenuItem(selectedMenuItemIndex - 1)),
                // [Down] Menu option down
                new KeyPressListener(Keys.Down, () => updateSelectedMenuItem(selectedMenuItemIndex + 1)),
                // [Enter] Starts game
                new KeyPressListener(Keys.Enter, () => chooseSelectedMenuItem()),
            };

            updateSelectedMenuItem(0);
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
            foreach (var item in keyListeners)
                item.Update(time);
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            parentPosition += Position;

            foreach (var item in menuElements)
                item.Draw(time, batch, parentPosition);
        }

        void updateSelectedMenuItem(int newIndex)
        {
            if (newIndex < 0 || newIndex > (menuElements.Count - 1))
                return;

            selectedMenuItemIndex = newIndex;

            menuElements.ForEach(i => {
                i.BackgroundColor = null;
                i.ForegroundColor = menuItemForegroundColor;
            });

            var item = menuElements[selectedMenuItemIndex];
            item.BackgroundColor = selectedMenuItemBackgroundColor;
            item.ForegroundColor = selectedMenuItemForegroundColor;
        }

        void chooseSelectedMenuItem()
        {
            var selectedElement = menuElements[selectedMenuItemIndex];
            var selectedAction = actions[selectedMenuItemIndex];
            selectedAction.Invoke();
        }

    }
}
