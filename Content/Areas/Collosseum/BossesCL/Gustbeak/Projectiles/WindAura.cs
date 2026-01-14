using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak.Projectiles
{
    public class WindAura : AbstractWindProjectile
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
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                float rotation = f * MathHelper.TwoPi;
                Vector2 offset = rotation.ToRotationVector2() * 2;
                drawPos += offset;
                DrawWindBall(drawPos, ref lightColor);
            }

            Texture2D texture = AssetManager.GlowMask.SpiralVortex.Value;
            Vector2 shadowDrawPos = Projectile.Center - Main.screenPosition;
            Vector2 shadowDrawOrigin = texture.Size() / 2f;
            float drawScale = DrawScale * 2;
            drawScale *= MathHelper.Lerp(3f, 1f, EasingFunction.InOutSine(Timer / 30f));

            Color drawColor = Color.White;
            drawColor.A = 0;
            drawColor *= MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
            float rotation2 = Main.GlobalTimeWrappedHourly * 12;
            spriteBatch.Draw(texture, shadowDrawPos, null, drawColor * 0.1f, rotation2, shadowDrawOrigin, drawScale * 0.7f, SpriteEffects.None, layerDepth: 0);
            spriteBatch.Draw(texture, shadowDrawPos, null, drawColor * 0.2f, rotation2 * 1.5f, shadowDrawOrigin, drawScale * 0.15f, SpriteEffects.None, layerDepth: 0);
            return false;
        }
    }
}
