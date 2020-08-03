using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bonsai.Framework.Particles
{
    public enum eParticleScalingMode { Static, Growing, Shrinking }
    public enum eParticleGroupType { Shatter }

    public class ParticleGroup
    {
        public static ParticleGroup CreateGroup(Texture2D texture, int count, eParticleGroupType type, Vector2 position, float offset_radius, Color color, float size, int degrees_lowerlimit, int degrees_upperlimit, GameTime gameTime)
        {
            ParticleGroup group = new ParticleGroup();
            group.Particle_Count = count;
            group.Position = position;
            group.Scaling_Mode = eParticleScalingMode.Shrinking;
            group.Texture = texture;
            Random randomizer = new Random();
            float max_age = 2000.0f;

            if (offset_radius < 1)
                group.origin = new Vector2(texture.Width / 2, texture.Height / 2);
            else
                group.origin = new Vector2(offset_radius);


            for (int i = 0; i < count; i++)
            {
                ParticleData particle = new ParticleData();
                particle.OriginalPosition = position;
                particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                particle.MaxAge = max_age;
                particle.ModColor = color;

                float distance = (float)randomizer.NextDouble() * size;
                Vector2 displacement = new Vector2(distance, 0);
                float angle = MathHelper.ToRadians(randomizer.Next(degrees_lowerlimit, degrees_upperlimit));
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

                particle.Direction = displacement;
                particle.Accelaration = 3.0f * particle.Direction;

                group.particles.Add(particle);
            }

            return group;
        }

        Random randomizer = new Random();
        Vector2 origin;
        List<ParticleData> particles = new List<ParticleData>();
        public eParticleScalingMode Scaling_Mode { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public int Particle_Count;
        public int Particles_Left
        {
            get { return particles.Count; }
        }

        
        public void Update(GameTime gameTime)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                ParticleData p = particles[i];
                float time_alive = now - p.BirthTime;

                if (time_alive > p.MaxAge)
                {
                    particles.RemoveAt(i);
                }
                else
                {
                    //update p
                    float rel_age = time_alive / p.MaxAge;
                    p.Position = 0.5f * p.Accelaration * rel_age * rel_age + p.Direction * rel_age + p.OriginalPosition;
                    float inv_age = 1.0f - rel_age;
                    Vector4 v = p.ModColor.ToVector4();
                    v.W = inv_age;
                    //v.Y = inv_age;
                    p.ModColor = new Color(v);

                    Vector2 pos_from_center = p.Position - p.OriginalPosition;
                    float distance = pos_from_center.Length();
                    p.Scaling = MathHelper.Clamp(inv_age - 0.4f, 0f, 1f);

                    particles[i] = p;
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                ParticleData p = particles[i];
                batch.Draw(Texture, p.Position, null, p.ModColor, i, origin, p.Scaling, SpriteEffects.None, 1);
            }
        }


    }
}
