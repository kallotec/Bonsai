using Bonsai.Framework;
using Bonsai.Framework.Content;
using Bonsai.Framework.Input;
using Bonsai.Framework.UI;
using Bonsai.Framework.UI.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Bonsai.Framework.UI.Menus
{
    public class SimpleMenu : Framework.IDrawable, Framework.ILoadable, Framework.IUpdateable
    {
        public SimpleMenu(string fontPath, Dictionary<string, Action> actionsMap)
        {
            this.fontPath = fontPath;
            this.actionsMap = actionsMap;
            menuElements = new List<TextElement<string>>();
        }

        string fontPath;
        Dictionary<string, Action> actionsMap;
        List<TextElement<string>> menuElements;
        List<KeyPressListener> keyListeners;
        SpriteFont font;
        int selectedMenuItemIndex = 0;
        Color menuItemForegroundColor = Color.FromNonPremultiplied(190, 190, 190, 255);
        Color selectedMenuItemForegroundColor = Color.FromNonPremultiplied(255, 255, 255, 255);
        Color selectedMenuItemBackgroundColor = Color.FromNonPremultiplied(33, 33, 33, 255);

        public bool IsHidden { get; set; }
        public DrawOrderPosition DrawOrder => DrawOrderPosition.HUD;
        public bool IsAttachedToCamera => true;

        public bool IsDisabled => throw new NotImplementedException();

        public void Load(IContentLoader loader)
        {
            font = loader.Load<SpriteFont>(fontPath);

            var screenCenter = BonsaiGame.Current.ScreenCenter + new Vector2(0, -100);
            bool isFirst = true;

            foreach (var item in actionsMap)
            {
                menuElements.Add(new TextElement<string>(item.Key, new TextElementSettings(font)
                {
                    Alignment = FieldAlignmentMode.Center,
                    DisplayMode = FieldDisplayMode.ValueOnly,
                    ForegroundColor = isFirst ? selectedMenuItemForegroundColor : menuItemForegroundColor,
                    Padding = new Vector2(15, 10),
                    Position = screenCenter,
                }));

                screenCenter += new Vector2(0, 50);
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

        public void Draw(GameTime time, SpriteBatch batch)
        {
            foreach (var item in menuElements)
                item.Draw(time, batch);
        }

        void updateSelectedMenuItem(int newIndex)
        {
            if (newIndex < 0 || newIndex > (menuElements.Count - 1))
                return;

            selectedMenuItemIndex = newIndex;

            menuElements.ForEach(i => {
                i.Settings.BackgroundColor = null;
                i.Settings.ForegroundColor = menuItemForegroundColor;
            });

            var item = menuElements[selectedMenuItemIndex];
            item.Settings.BackgroundColor = selectedMenuItemBackgroundColor;
            item.Settings.ForegroundColor = selectedMenuItemForegroundColor;
        }

        void chooseSelectedMenuItem()
        {
            var selectedElement = menuElements[selectedMenuItemIndex];
            var selectedAction = actionsMap[selectedElement.Value];
            selectedAction.Invoke();
        }

    }
}
