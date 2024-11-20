using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.NPCs.Bosses.Gustbeak.Projectiles
{
    internal class WindAura : BaseWindProjectile
    {
        private int ParentIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private bool Die;
        private ref float KillTimer => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = true;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            if (ParentIndex != -1)
            {
                NPC npc = Main.npc[ParentIndex];
                //If the npc is not active then yeah
                if (!npc.active)
                {
                    Die = true;
                }
                else
                {
                    Projectile.Center = npc.Center;
                }
            }
            else
            {
                Die = true;
            }

            if (Timer > 240)
            {
                Die = true;
            }

            if (Timer % 8 == 0)
            {
                //Spawn new slashes on our little wind orb
                float range = 80;
                Vector2 offset = Main.rand.NextVector2CircularEdge(range, range);
                float rotation = -offset.ToRotation();
                Wind.NewSlash(offset, rotation);

                offset = Main.rand.NextVector2CircularEdge(range, range);
                rotation = -offset.ToRotation();
                Wind.NewSlash(offset, rotation);
            }

            if (Die)
            {
                KillTimer++;
                DrawScale = MathHelper.Lerp(1f, 0f, KillTimer / 60f);
                if (KillTimer >= 60f)
                {
                    Projectile.Kill();
                }
            }

            Projectile.rotation += 0.025f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive);

            for (float f = 0f; f < 1f; f += 0.25f)
            {
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                float rotation = f * MathHelper.TwoPi;
                Vector2 offset = rotation.ToRotationVector2() * 2;
                drawPos += offset;
                DrawWindBall(drawPos, ref lightColor);
            }

            spriteBatch.RestartDefaults();
            return false;
        }
    }
}
