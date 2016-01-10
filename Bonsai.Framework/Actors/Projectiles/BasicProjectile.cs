using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Framework.Common;
using Bonsai.Framework.Actors.Projectiles;
using System.Collections.Generic;

namespace Bonsai.Framework.Actors.Projectiles
{
    //public class BasicProjectile : MoveableActor, IProjectile
    //{
    //    public BasicProjectile(int speed, int damage, Dictionary<Type, PenetrationSettings> penetrationSettings, int width, int height) : base(width, height)
    //    {
    //        this.Damage = damage;
    //        this.penetrationSettings = penetrationSettings;
    //    }

    //    Dictionary<Type, PenetrationSettings> penetrationSettings;
    //    IGameObject actorLastTouched;

    //    public int Damage { get; set; }
    //    public IGameObject Owner { get; set; }
        
        
    //    public override void Update(GameFrame gameFrame)
    //    {
    //        ////always add velocity value to position
    //        //base.SetPosition(base.Position + base.Velocity * (float)gameFrame.GameTime.ElapsedGameTime.TotalSeconds, PositionMode.Center);
    //    }

    //    public override void CollidedWith(IGameObject actor)
    //    {
    //        //dont do collision with uncollidable obj, owner or last actor touched
    //        if (actor.IsCollidable == false || actor == Owner || actor == actorLastTouched)
    //            return;

    //        //record touch
    //        actorLastTouched = actor;

    //        //cant pass a gameboundary
    //        if (actor is Boundary)
    //        {
    //        //    //kill projectile
    //        //    base.MarkForDeletion();
    //            return;
    //        }

    //        //handle change in direction + slowdown, based on settings
    //        var actorType = actor.GetType();
    //        if (penetrationSettings.ContainsKey(actorType))
    //        {
    //            //get settings for touched actor type
    //            var penetrationSetting = penetrationSettings[actorType];

    //            switch (penetrationSetting.Type)
    //            {
    //                case PenetrationType.Bounce:
    //                    {
    //                        //detect the correct side and thus the correct normal to use for reflecting
    //                        var reflectionNormal = EntityMathHelper.GetReflectionNormal(this.CollisionBox, actor.CollisionBox);

    //                        ////invert velocity if side could not be found
    //                        ////otherwise reflect using the normal found
    //                        //if (reflectionNormal != Vector2.Zero)
    //                        //    base.SetVelocity(Vector2.Reflect(Velocity, reflectionNormal));
    //                        //else
    //                        //    base.SetVelocity(-Velocity);

    //                        break;
    //                    }
    //                case PenetrationType.Riccochet:
    //                    {
    //                        //randomize riccochet angle
    //                        //TODO: riccochet on angle greater than approach
    //                        Random r = new Random();
    //                        float riccochetAngle = MathHelper.ToRadians(180); //spin it around
    //                        riccochetAngle += MathHelper.ToRadians(r.Next(-60, 60)); //randomize bounce-back angle
    //                        //base.SetDirection(base.Direction + riccochetAngle);

    //                        break;
    //                    }
    //            }

    //            ////incur touch slowdown
    //            //base.SetSpeed(base.Speed - penetrationSetting.SpeedSlowdownOnTouch);

    //            ////incur touch damage
    //            //base.ApplyDamage(penetrationSetting.TouchDamage);

    //        }

    //        //signal touch call to target
    //        actor.TouchedBy(this);
    //    }

    //    public new int Speed
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override void TouchedBy(IGameObject actor)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Load(ContentManager content)
    //    {
    //    }

    //    public void Unload()
    //    {
    //    }

    //    public void Draw(GameFrame frame, SpriteBatch batch)
    //    {
    //    }

    //}
}
