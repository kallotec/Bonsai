using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Bonsai.Framework.Utility;

namespace Bonsai.Framework.Input
{
    public class KeyPressListener : IUpdateable
    {
        public KeyPressListener(Keys key, delKeyPressed target)
        {
            this.Key = key;
            this.signal = target;
            this.singlePress = true;
        }

        public KeyPressListener(Keys key, delKeyPressed target, int coolDownMillisecs)
        {
            this.Key = key;
            if (coolDownMillisecs > 0)
                this.coolDownTimer = new MillisecCounter(coolDownMillisecs);
            else
                this.rapidFire = true;
            this.signal = target;
            
        }

        public Keys Key { get; private set; }

        public bool IsDisabled => false;

        delKeyPressed signal;
        public delegate void delKeyPressed();
        bool rapidFire;
        bool singlePress;
        bool singlePress_keyBeingPressed;
        MillisecCounter coolDownTimer;
        MillisecCounter initialCooldown = new MillisecCounter(0500);

        KeyboardState kbstate_current;

        public void Update(GameTime time)
        {
            //capture keyboard state
            kbstate_current = Keyboard.GetState();

            //this stalls the listener slightly at the beginning
            //to combat picking up keydowns that are still being 
            //pressed from previous game screens
            if (initialCooldown.Completed == false)
            {
                initialCooldown.Update(time.ElapsedGameTime.Milliseconds);
                return;
            }

            //repeatedly fire while key down
            //(zero cooldown specified)
            if (rapidFire)
            {
                //fire away
                if (kbstate_current.IsKeyDown(Key))
                    signal();

                //no need to continue method
                return;
            }

            //no cooldown and no rapid file
            //signal does not re-fire until keyup
            if (singlePress)
            {
                if (kbstate_current.IsKeyDown(Key) && singlePress_keyBeingPressed == false)
                {
                    singlePress_keyBeingPressed = true;
                    signal();
                }
                else if (kbstate_current.IsKeyUp(Key) && singlePress_keyBeingPressed == true)
                {
                    singlePress_keyBeingPressed = false;
                }

                //no need to continue method
                return;

            }
            
            //if cooled down check keydown
            if (coolDownTimer.Completed)
            {
                if (kbstate_current.IsKeyDown(Key))
                {
                    coolDownTimer.Reset();
                    signal();

                    //no need to continue method
                    return;
                }
            }
            else
            {
                coolDownTimer.Update(time.ElapsedGameTime.Milliseconds);
            }

        }

    }
}
