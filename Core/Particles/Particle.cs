using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Stellamod.Core.Particles
{
    internal abstract class Particle
    {
        public Particle()
        {
            StartSize = 1f;
            EndSize = 0.8f;
            LifeTime = 120;
            RandSpawnRadius = 1;
            RandLifeTime = 30;
            SizeInLerp = 0.01f;
            StartColor = Color.White;
            EndColor = Color.White;
        }

        //Ok so what variables do we need on top of the renderer uhhh
        //Most stuff is specific to particles so
        public float Time { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public float StartSize { get; set; }
        public float EndSize { get; set; }
        public float SizeInLerp { get; set; }
        public bool BumpSize { get; set; }
        public float LifeTime { get; set; }
        public float Speed { get; set; }
        public float RandSize { get; set; }
        public float RandSpawnRadius { get; set; }
        public float RandLifeTime { get; set; }
        public float RandSpeed { get; set; }
        public bool IsAlive { get; set; }
        public static int AliveCount { get; set; }
        public virtual void On_Spawn()
        {
            AliveCount++;
            LifeTime += Main.rand.NextFloat(0, RandLifeTime);
        }

        public virtual void On_Kill()
        {
            AliveCount--;
            IsAlive = false;
        }

        public virtual void AI()
        {
            Time++;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
