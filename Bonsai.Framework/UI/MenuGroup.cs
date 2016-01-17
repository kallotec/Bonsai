using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Framework;
using Bonsai.Framework.Input;

namespace Bonsai.Framework.UI
{
    public class MenuGroup
    {
        public MenuGroup()
        { 
            items = new List<IMenuItem>();
            keyListener_Up = new KeyPressListener(Microsoft.Xna.Framework.Input.Keys.Up, new KeyPressListener.delKeyPressed(onKeyPress_Up));
            keyListener_Down = new KeyPressListener(Microsoft.Xna.Framework.Input.Keys.Down, new KeyPressListener.delKeyPressed(onKeyPress_Down));
            bounds = new Rectangle(0, 0, 0, 0);
            itemSpacing = new Vector2(pad, pad);
            boundsBorder = new Vector2(border, border);
        }

        void onKeyPress_Up()
        {
            selectedIndex = (int)MathHelper.Clamp((float)selectedIndex - 1, 0f, (float)items.Count - 1);

            foreach (IMenuItem menu in items)
                menu.IsHighlighted = false;

            items[selectedIndex].IsHighlighted = true;
        }
        void onKeyPress_Down()
        {
            selectedIndex = (int)MathHelper.Clamp((float)selectedIndex + 1, 0f, (float)items.Count - 1);

            foreach (IMenuItem menu in items)
                menu.IsHighlighted = false;

            items[selectedIndex].IsHighlighted = true;
        }

        int pad = 2;
        int border = 5;
        List<IMenuItem> items;
        Vector2 itemSpacing;
        Vector2 runningSpacer;
        Vector2 boundsBorder;
        SpriteFont menuFont;
        int selectedIndex;
        KeyPressListener keyListener_Up;
        KeyPressListener keyListener_Down;
        Rectangle bounds;

        public Rectangle Bounds 
        {
             get { return bounds; }
        }
        public Vector2 Position
        {
            get;
            set;
        }
        public List<IMenuItem> Items
        {
            get { return items; }
            private set { items = value; }
        }

        public void LoadContent(ContentManager content)
        {
            menuFont = content.Load<SpriteFont>(Globals.FONT_MENUREGULAR);
            runningSpacer = Position + boundsBorder;
            bounds.X = (int)Position.X;
            bounds.Y = (int)Position.Y;
            bounds.Width = (int)boundsBorder.X * 2;
            bounds.Height = (int)boundsBorder.Y * 2;
        }

        public void AddItem(StringBuilder label, delMenuItemClick callbackMenuItemClick)
        {
            //create item
            MenuItem mi = new MenuItem(label, runningSpacer, menuFont);
            mi.Clicked = callbackMenuItemClick;
            if (Items.Count == 0)
                mi.IsHighlighted = true;
            Items.Add(mi);

            //update spacer and box
            updateRunningSpacerAndBoundingBox(mi);
        }

        public void AddItem(StringBuilder label, object attachedValue, List<object> possibleValues)
        {
            //create item
            MenuItem mi = new MenuItem(label, runningSpacer, menuFont);
            mi.Value = attachedValue;
            mi.ValueList = possibleValues;
            if (Items.Count == 0)
                mi.IsHighlighted = true;
            Items.Add(mi);

            //update spacer and box
            updateRunningSpacerAndBoundingBox(mi);
        }

        private void updateRunningSpacerAndBoundingBox(MenuItem mi)
        {
            //update spacer
            runningSpacer.Y = mi.Bounds.Y + mi.Bounds.Height;

            //only add item spacer if more than 11 item
            if (items.Count > 1)
                runningSpacer.Y += itemSpacing.Y;

            //stretch height down
            bounds.Height = (int)(bounds.Height + mi.Bounds.Height + itemSpacing.Y);

            if (bounds.Width < (mi.Bounds.Width + (boundsBorder.X * 2)))
                bounds.Width = (int)(mi.Bounds.Width + (boundsBorder.X * 2));
        }


        public void Update(GameFrame frame)
        {
            //key press listeners
            keyListener_Up.Update(frame.GameTime);
            keyListener_Down.Update(frame.GameTime);

            //menu items update
            foreach (IMenuItem mi in items)
                mi.Update(frame.GameTime);
        }

        public void Draw(SpriteBatch batch)
        {
            //box
            batch.Draw(Globals.Pixel_HalfTransparent, bounds, Color.Black);

            //draw menu options
            foreach (IMenuItem mi in items)
                mi.Draw(batch);
        }

    }
}
