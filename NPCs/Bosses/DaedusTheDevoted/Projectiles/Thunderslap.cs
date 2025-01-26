using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Projectiles.Visual;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class ThunderSlap : ModProjectile
    {
        public float BeamLength;
        public Vector2[] BeamPoints;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];

        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 30;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            if (Timer == 1)
            {
                Player targetPlayer = PlayerHelper.FindClosestPlayer(Projectile.position, 1680);
                float offset = ProjectileHelper.PerformBeamHitscan(targetPlayer.Bottom - Vector2.UnitY, -Vector2.UnitY, 2400);
                Projectile.position = targetPlayer.Bottom + new Vector2(0, -offset * 0.7f);
            }
            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.position, Vector2.UnitY, 2400);
            BeamLength = targetBeamLength;
            if (Timer == 1)
            {
                //Sound Effect Goooo
                SoundStyle lightningSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
                lightningSoundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 dustSpawnPoint = Projectile.Center + direction * BeamLength;
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(8, 8);
                    Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, dustVelocity, Scale: 0.5f);
                    d.noGravity = true;
                }


                Vector2 lightningHitPos = Projectile.position + new Vector2(0, BeamLength);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(lightningHitPos, 1024, 32);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), lightningHitPos, velocity,
                        ModContent.GoreType<FableRock1>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), lightningHitPos, velocity,
                        ModContent.GoreType<FableRock2>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), lightningHitPos, velocity,
                        ModContent.GoreType<FableRock3>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), lightningHitPos, velocity,
                        ModContent.GoreType<FableRock4>());
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), lightningHitPos + new Vector2(0, 24), Vector2.Zero,
                        ModContent.ProjectileType<GroundCracking>(), 0, 0, Projectile.owner);
                }
            }


            //Setup lightning stuff
            //Should make it scale in/out
            float lightningProgress = Timer / 30f;
            float easedLightningProgress = Easing.SpikeOutCirc(lightningProgress);
            Lightning.WidthMultiplier = MathHelper.Lerp(0f, 24, easedLightningProgress);
            if (Timer % 3 == 0)
            {
                List<Vector2> beamPoints = new List<Vector2>();
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i <= 8; i++)
                {
                    float maxProgress = MathHelper.Lerp(0f, 1f, Easing.OutExpo(Timer / 15f));
                    float progress = MathHelper.Lerp(0f, maxProgress, i / 8f);
                    beamPoints.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + direction * BeamLength, progress));
                }
                BeamPoints = beamPoints.ToArray();
                Lightning.RandomPositions(BeamPoints);
            }
        }

        public override bool? CanDamage()
        {
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
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

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.SetBoltDefaults();
            Lightning.Draw(spriteBatch, BeamPoints, Projectile.oldRot);
            return false;
        }
    }
}
