using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{


    public class VehementMinionProj : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        private ref float SpeedTimer => ref Projectile.ai[1];
        private ref float HitCount => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Irradiated Creeper");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 1;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
                                            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)// Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage) // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = 1500;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }
        private float alphaCounter = 0;
        public override void AI()
        {
            Timer++;
            if (SpeedTimer > 0)
            {
                SpeedTimer--;
                Projectile.extraUpdates = 3;
            }
            else
            {
                Projectile.extraUpdates = 0;
            }
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Goldenrod, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            Player player = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<VehementMinionBuff>(player, Projectile))
                return;

            NPC target = ProjectileHelper.FindNearestEnemyThroughWalls(Projectile.Center, 1024);
            if (target != null)
            {
                float progress = MathHelper.Clamp(Timer / 35f, 0f, 1f);
                float d = MathHelper.Lerp(3f, 45, progress);
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, target.Center, d);
                if (Projectile.velocity.Length() < 15)
                {
                    Projectile.velocity *= 1.5f;
                }

                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.Y -= 1;
                }
            }
            else
            {
                SummonHelper.CalculateIdleValues(Owner, Projectile, Owner.Center, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
            if (SpeedTimer <= 0)
            {
                HitCount++;
                if (HitCount >= 15)
                {
                    HitCount = 0;
                    SpeedTimer = 240;
                }
            }

            var EntitySource = Projectile.GetSource_Death();
            Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0,
                ModContent.ProjectileType<VehementBoom>(), Projectile.damage, 1, Projectile.owner, 0, 0);
            Projectile.velocity = -Projectile.velocity;
            int Sound = Main.rand.Next(1, 6);
            SoundStyle mySound = new SoundStyle("Stellamod/Assets/Sounds/Rhap1");
            if (Sound == 1)
            {
                mySound = new SoundStyle("Stellamod/Assets/Sounds/Rhap1");
            }
            if (Sound == 2)
            {
                mySound = new SoundStyle("Stellamod/Assets/Sounds/Rhap2");
            }
            if (Sound == 3)
            {
                mySound = new SoundStyle("Stellamod/Assets/Sounds/Rhap3");

            }
            Timer = 1;
            mySound.Volume = 0.3f;
            mySound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(mySound, Projectile.position);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Goldenrod, Color.LightGoldenrodYellow, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }
    }

    public class VehementBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 100;
            Projectile.timeLeft = 15;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                for (float f = 0; f < 6; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MusicDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Orange,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(12, 25),
                        baseSize: Main.rand.NextFloat(0.01f, 0.15f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }
    }
}
