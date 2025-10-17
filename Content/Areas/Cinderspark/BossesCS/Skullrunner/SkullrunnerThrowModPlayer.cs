using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner
{
    public class SkullrunnerThrowModPlayer : ModPlayer
    {
        public Vector2? targetSuckPosition;
        public Vector2? resetVelocity;
        public Vector2? throwVelocity;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (targetSuckPosition.HasValue)
            {
                Vector2 suckPosition = targetSuckPosition.Value;
                Vector2 velocityToPosition = (suckPosition - Player.Center);
                Player.velocity = Vector2.Lerp(Player.velocity, velocityToPosition, 0.5f);
                targetSuckPosition = null;
            }
            if (resetVelocity.HasValue)
            {
                Player.velocity = resetVelocity.Value;
                resetVelocity = null;
            }
            if (throwVelocity.HasValue)
            {
                Player.velocity = throwVelocity.Value;
                Point point = new Vector2(Player.BottomLeft.X, Player.BottomLeft.Y).ToTileCoordinates();
                Tile? floorTile = Player.GetFloorTile(point.X, point.Y);
                if (floorTile.HasValue)
                {
                    float damage = 32;
                    if (Main.masterMode)
                        damage *= 3;
                    if (Main.expertMode)
                        damage *= 2;

                    if(Main.myPlayer == Player.whoAmI)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - Vector2.UnitY * 128, Vector2.UnitY,
                            ModContent.ProjectileType<ScatterBoom>(), 0, 0, Player.whoAmI);
                    }
                    Player.Hurt(new PlayerDeathReason(), (int)damage, 1);

                    Eruption(Player.Center, -Vector2.UnitY);
                    throwVelocity = null;
                }
          
            }
        }
        private void Eruption(Vector2 position, Vector2 velocity)
        {
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
            }

            FXUtil.ShakeCamera(position, 1024, 98);
            FXUtil.PunchCamera(position, Vector2.UnitY * 2, 8, 8, 32);
            FXUtil.GlowCircleBoom(position,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(position, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(position,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.Red,
                    outerColor: Color.Orange,
                    fadeToColor: Color.Purple,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
                if (Main.rand.NextBool(4))
                {

                    var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                     innerColor: Color.DarkRed,
                     outerColor: Color.DarkBlue,
                     fadeToColor: Color.Black,
                     distortOut: false);
                    part.Scale *= 1.3f;
                }
            }

            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = velocity.RotatedByRandom(MathHelper.PiOver4) * 15 * Main.rand.NextFloat(0.5f, 1f);
                var d = Dust.NewDustPerfect(position, DustID.InfernoFork, speed, Scale: 3f);
            }
            FXUtil.ShakeCamera(position, 1024, 8);
        }
    }
}
