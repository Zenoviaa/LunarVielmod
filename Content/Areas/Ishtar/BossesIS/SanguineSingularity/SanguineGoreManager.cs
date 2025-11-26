using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;

/*

- Deer with a singularity for a head, in its spawn animation at first it looks like a normal deer before the head explodes and parts start orbiting it, ooo I know exactly how to code this

- The legs and everything are rigged, we’ll use forward kinematics to animate the boss, so we’ll have to make a run animation and idle animation

- Opens the fight with several exploding blood magic projectiles that loosely track the player

- Winds up a charge and then runs directly at the player really fast, and explodes into bloody bits before merging itself back together elsewhere

- Runs up into the sky and rains down acidic blood

- Walks slowly around the player as bloody boils explode from its body and then home back towards you

- Cracks form in its body and it violently erupts into multiple bloody geysers

- Winds up a charge and then keeps running at you while swerving around and trying to juke you out
 
- In phase 2 every attack gets more deadlier, triggers at under 50% health
 */
namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity
{
    public class SanguineGore
    {
        public SanguineGore(Texture2D texture, Rectangle frame)
        {
            this.position = Vector2.Zero;
            this.texture = texture;
            this.frame = frame;
            this.color = Color.White;
            this.scale = Vector2.One;
            this.oscScale = Vector2.One;
        }
        public Vector2 position;
        public Vector3 initialPosition;
        public Vector2 scale;
        public Vector2 oscScale;
        public Rectangle frame;
        public Texture2D texture;
        public Color color;
        public float rotation;
        public float sortingOrder;
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 drawCenter = position - screenPos;
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = color.MultiplyRGB(lightColor);
            spriteBatch.Draw(texture, drawCenter, frame, drawColor, rotation, drawOrigin, scale * oscScale, SpriteEffects.None, 0);
        }
    }

    public class SanguineGoreManager
    {
        private float _timer;
        private float _orbitRadius;
        public SanguineGoreManager(Texture2D goreTexture, int frameCount)
        {
            gores = new SanguineGore[frameCount];
            int height = goreTexture.Height / frameCount;
            for (int i = 0; i < gores.Length; i++)
            {
                int y = i * height;
                Rectangle frame = new Rectangle(0, y, goreTexture.Width, height);
                gores[i] = new SanguineGore(goreTexture, frame);


                //Calculate the initial position of the orbit
                float f = i;
                float num = gores.Length;
                float completionRatio = f / num;
                float z = MathHelper.Lerp(1f, -1f, EasingFunction.QuadraticBump(completionRatio));
                float x = MathHelper.Lerp(-1f, 1f, completionRatio);
                Vector3 initialPosition = new Vector3(x, 0, z);
                gores[i].initialPosition = initialPosition;
            }
            orbitingRadius = 40f;
        }

        public SanguineGore[] gores;
        public bool draw;
        public float orbitingRadius;
 
        public bool HasTinyOrbit()
        {
            return _orbitRadius <= 0.1f;
        }

        public void Update(Vector2 orbitCenter)
        {
            _orbitRadius = MathHelper.Lerp(_orbitRadius, orbitingRadius, 0.1f);

            //Rotate the damn thing
            _timer++;

            float angle = _timer * 0.1f;
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(new Vector3(0f, 0.5f, 1f), angle);
            Matrix rotationMatrix = Matrix.CreateFromQuaternion(quaternion);
            for(int i = 0; i < gores.Length; i++)
            {
                SanguineGore gore = gores[i];

                //So we need to calculate some type of orbiting movement
                Vector3 newPosition = Vector3.Transform(gore.initialPosition * _orbitRadius, rotationMatrix);
                Vector2 offset = new Vector2(newPosition.X, newPosition.Y);
                Vector2 gorePosition = orbitCenter + offset;
                gore.position = gorePosition;
                gore.rotation = offset.ToRotation();

                float osc = ExtraMath.Osc(0f, 1f, speed: 2f, offset: i);
                Vector2 oscScale = Vector2.Lerp(new Vector2(1.2f, 0.9f), Vector2.One, osc);
                gore.oscScale = oscScale;
                if (Main.rand.NextBool(100))
                {
                    var d = Dust.NewDustPerfect(gore.position, DustID.Blood, Scale: Main.rand.NextFloat(0.5f, 1f));
                    d.noGravity = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (!draw)
                return;

            for(int i = 0; i < gores.Length; i++)
            {
                SanguineGore gore = gores[i];
                gore.Draw(spriteBatch, screenPos, lightColor);
            }
        }
    }
}
