using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bonsai.Framework.Common
{
    public class GameFrame
    {
        public GameTime GameTime { get; set; }

        public KeyboardState KeyboardState { get; set; }
        public MouseState MouseState { get; set; }
        public GamePadState PadState { get; set; }


        public void Update(GameTime gameTime)
        {
            this.GameTime = gameTime;
            this.KeyboardState = Keyboard.GetState(PlayerIndex.One);
            this.MouseState = Mouse.GetState();
            this.PadState = GamePad.GetState(PlayerIndex.One);
        }

    }
}
