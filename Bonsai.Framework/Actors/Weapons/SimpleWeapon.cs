using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bonsai.Framework;
using Bonsai.Framework.Actors.Projectiles;

namespace Bonsai.Framework.Actors.Weapons
{
    //public class SimpleWeapon : Sprite, IWeapon
    //{
    //    public SimpleWeapon(delWeaponFired weaponFiredSignal)
    //    {
    //        WeaponFired = weaponFiredSignal;
    //        Ammo = 30;
    //        AmmoCapacity = 999;
    //        Damage = 80;    
    //        Label_Ammo = Ammo.ToString() + " / " + AmmoCapacity.ToString();
    //        FireCooldown = new MillisecCounter(0050);
    //        FiringOffset = new Vector2(0, 0);
    //    }

    //    public bool IsThrowable { get; set; }
    //    public int Ammo { get; set; }
    //    public int AmmoCapacity { get; set; }
    //    public string Label_Ammo { get; set; }
    //    public int Damage { get; set; }
    //    public Vector2 FiringOffset { get; private set; }
    //    public delWeaponFired WeaponFired { get; private set; }
    //    public MillisecCounter FireCooldown { get; private set; }
    //    public AnimationOverlay IconAnimation { get; set; }
    //    public AnimationOverlay FiredAnimation { get; set; }


    //    public void Fire()
    //    {
    //        if (FireCooldown.Completed == false)
    //            return;

    //        //ensure ammo exists
    //        if (Ammo < 1)
    //            return;

    //        //restart timer
    //        FireCooldown.Reset();
    //        Ammo -= 1;

    //        //build projectile
    //        var penSettings = new Dictionary<Type, PenetrationSettings> 
    //        {
    //            {
    //                typeof(IGameObject), new PenetrationSettings 
    //                { 
    //                    Type = PenetrationType.Passthrough,
    //                    TouchDamage = 60,
    //                    SpeedSlowdownOnTouch = 200
    //                }
    //            }
    //        };
    //        BasicProjectile p = new BasicProjectile(speed: 600, damage: 10, penetrationSettings: penSettings, width: 5, height: 5);
    //        //p.LoadContent(Texture);

    //        //raise fired event
    //        WeaponFired(p);

    //    }

    //    public void LoadContent(ContentManager content)
    //    {
    //    }
        
    //    public void Update(GameFrame gameFrame)
    //    {
    //        FireCooldown.Update(gameFrame.GameTime.ElapsedGameTime.Milliseconds);
    //    }

    //}
}
