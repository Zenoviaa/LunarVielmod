using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class TorrentialLanceProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private bool SpawnBubbles;

        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float HoldOffset => 30;
        private float SwingTime => 60;
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 48;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.timeLeft = (int)SwingTime;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 48;
            Timer++;
            if(SpawnBubbles && Timer % 8 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item85, Projectile.position);
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, 
                    ModContent.ProjectileType<TorrentialLanceBubbleProj>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner);
            }

            AI_Dash();
            AI_Immune();
            AI_AttachToPlayer();
            Visuals();
        }

        private void AI_Dash()
        {
            Vector2 oldMouseWorld = Main.MouseWorld;
            if (Timer < 3)
            {
                Vector2 directionToMouse = Projectile.DirectionTo(oldMouseWorld);
                Owner.velocity.X = directionToMouse.X * 20f;
                Projectile.velocity.X = directionToMouse.X;
                Projectile.velocity.Y = 0;
            }

            if(Timer > 30)
            {
                Owner.velocity *= 0.9f;
            }
        }

        private void AI_Immune()
        {
            if (Timer < 30)
            {
                int minImmuneTime = 5;
                Owner.immune = true;

                //Using Math.Max returns the highest value, so you won't override any existing immune times.
                //Just prevent it from going low
                Owner.immuneTime = Math.Max(Owner.immuneTime, minImmuneTime);

                //No clue why you need to set this, but if you don't you'll still sometimes take damage
                Owner.hurtCooldowns[0] = Math.Max(Owner.hurtCooldowns[0], minImmuneTime);
                Owner.hurtCooldowns[1] = Math.Max(Owner.hurtCooldowns[1], minImmuneTime);
                Owner.noKnockback = true;
            }
        }

        private void AI_AttachToPlayer()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
                return;

            // the actual rotation it should have
            float rotation = Projectile.velocity.ToRotation();

            // offsetted cuz sword sprite
            Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
            position += rotation.ToRotationVector2() * HoldOffset;
            Projectile.Center = position;
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi - MathHelper.PiOver4;
                Projectile.Center += new Vector2(-30, 0);
            }

            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = rotation * player.direction;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }


        private void Visuals()
        {
            if (Timer % 4 == 0)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                float scale = Main.rand.NextFloat(0.2f, 0.4f);
                ParticleManager.NewParticle(Projectile.Center, velocity, ParticleManager.NewInstance<BubbleParticle>(),
                    Color.White, scale);
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!SpawnBubbles)
            {
                SoundEngine.PlaySound(SoundRegistry.BubbleIn, target.position);
            }

            SpawnBubbles = true;
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                float scale = Main.rand.NextFloat(0.2f, 0.4f);
                ParticleManager.NewParticle(target.Center, velocity, ParticleManager.NewInstance<BubbleParticle>(),
                    Color.White, scale);
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Aquamarine, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 textureSize = texture.Size();
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.CausticTrail);

            Vector2 trailOffset = Vector2.Zero;
            if (Projectile.spriteDirection == 1)
                trailOffset = new Vector2(32, -4);
            else
            {
                trailOffset = new Vector2(-32, -4);
            }

            TrailDrawer.DrawPrims(Projectile.oldPos, textureSize * 0.5f - Main.screenPosition + trailOffset, 155);
            return true;
        }
    }
}
