using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class ToxicHornetMinionProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<ToxicHornetMinionBuff>(owner, Projectile))
                return;

            SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Vector2 targetOffset = new Vector2(0, -128);
            float acceleration = 0.1f;
            float maxSpeed = 12;
            if (!foundTarget)
            {
                AI_Movement(owner.Center + targetOffset, maxSpeed, acceleration);
            }
            else
            {
                Vector2 movementCenter = targetCenter + targetOffset;
                AI_Movement(movementCenter, maxSpeed, acceleration / 2);
                Projectile.ai[0]++;
                if (Projectile.ai[0] > 30)
                {
                    Vector2 bulletVelocity = Projectile.Center.DirectionTo(targetCenter) * 62;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, bulletVelocity,
                        ModContent.ProjectileType<ToxicMissileFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.position);
                    SoundEngine.PlaySound(SoundID.Zombie48, Projectile.position);
                    Projectile.ai[0] = 0;
                }
            }

            Visuals();
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            Projectile.spriteDirection = -Projectile.direction;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(255, 253, 90), new Color(72, 131, 56), ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        Vector2 Drawoffset => new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX * Projectile.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D TextureGlow => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;

        public override void PostDraw(Color lightColorr)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.DarkSeaGreen * num107 * .8f;
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int projFrames = Main.projFrames[Projectile.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * Projectile.frame;

            Rectangle frame = new Rectangle(0, startY, texture.Width, frameHeight);
            spriteBatch.Draw(
                TextureGlow,
                Projectile.Center - Main.screenPosition + Drawoffset,
                frame,
                color1,
                Projectile.rotation,
                frame.Size() / 2,
                Projectile.scale,
                effects,
                0
            );

            SpriteEffects spriteEffects3 = (Projectile.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            //Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Green);
            for (int num103 = 0; num103 < 4; num103++)
            {
                Color color28 = color29;
                color28 = Projectile.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = Projectile.Center + (num103 / (float)num108 * 6.28318548f + Projectile.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - Projectile.velocity * num103;
                Main.spriteBatch.Draw(TextureGlow, vector29, frame, color28, Projectile.rotation, frame.Size() / 2f, Projectile.scale, spriteEffects3, 0f);
            }
        }
    }
}
