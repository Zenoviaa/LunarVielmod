using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class ProbeMinionProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float ShootRotation => ref Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jelly Minion");
            // Sets the amount of frames this minion has on its spritesheet
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Type] = 4;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                for (float f = 0; f < 8; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Red, Main.rand.NextFloat(2f, 3f)).noGravity = true;
                }


            }
            if (Owner.dead || !Owner.active)
            {
                Owner.ClearBuff(ModContent.BuffType<ProbeMinionBuff>());
            }

            if (!Owner.HasBuff(ModContent.BuffType<ProbeMinionBuff>()))
            {
                Projectile.Kill();
            }

            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Red, Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
            }

            if (Timer % 10 == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    NPC nearest = ProjectileHelper.FindNearestEnemyUnderneath(Projectile.position, 1024, 256);
                    if (nearest != null)
                    {
                        Vector2 velocity = (nearest.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        ShootRotation = velocity.ToRotation() - MathHelper.ToRadians(90);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity.RotatedByRandom(MathHelper.ToRadians(7)),
                            ModContent.ProjectileType<ProbeLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.netUpdate = true;
                    }

                }

            }

            Projectile.velocity.Y = MathF.Sin(Timer * 0.1f);
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = ShootRotation;
            Color finalColor = Color.White.MultiplyRGB(lightColor);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, Projectile.Frame(), Color.White, rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            float rotation = ShootRotation;
            for (float f = 0f; f < 4f; f++)
            {
                Vector2 offset = ((f / 4f) * MathHelper.ToRadians(360) + Main.GlobalTimeWrappedHourly * 8).ToRotationVector2() * VectorHelper.Osc(3f, 4f);
                spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition + offset,
                    Projectile.Frame(), Color.White * VectorHelper.Osc(0f, 0.15f), rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
        }
    }


    public class ProbeLaser : ModProjectile, IPixelPrimitiveDrawer
    {
        private PrimitiveTrail BeamDrawer;
        private float BeamLength;
        private Vector2 EndPoint => Projectile.position + Projectile.velocity * BeamLength + Projectile.Size / 2 - new Vector2(0, 8);
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 7;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            base.AI();
            Timer++;

            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.position, Projectile.velocity, 2400);
            BeamLength = targetBeamLength;
            if (Timer == 1)
            {
                for (float f = 0; f < 2; f++)
                {
                    Dust.NewDustPerfect(EndPoint, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(0.2f, 1f)).noGravity = true;
                }

                SoundStyle mySound = new SoundStyle($"Stellamod/Assets/Sounds/GunShootNew5");
                mySound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mySound, Projectile.position);

                FXUtil.GlowCircleBoom(EndPoint,
                    innerColor: Color.White,
                    glowColor: Color.Red,
                    outerGlowColor: Color.Black, duration: 5, baseSize: Main.rand.NextFloat(0.02f, 0.2f));

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(EndPoint,
                        innerColor: Color.White,
                        glowColor: Color.Red,
                        outerGlowColor: Color.Black,
                        baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                        duration: Main.rand.NextFloat(5, 10));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * (BeamLength);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale * 1.3f * Easing.SpikeOutCirc(Timer / 7f);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.White, Color.Red, 0.65f);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.White);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);


            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength, i / 8f));
            }

            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }

    }
}
