using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    internal class DogmaBallsProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float BounceCount => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Timer++;
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Turquoise, Main.rand.NextFloat(1f, 1.5f));
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowHeartDust>(), Projectile.velocity * 0.1f, 0, Color.Turquoise, Main.rand.NextFloat(1f, 1.5f));
                else if (Main.rand.NextBool(2))
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.1f, GoreID.Pages);
            }
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            Lighting.AddLight(Projectile.position, Color.LightBlue.ToVector3() * 0.78f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            BounceCount++;
            if (BounceCount >= 6)
                Projectile.Kill();
            Projectile.velocity *= 0.75f;
            return false;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Turquoise, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LoveTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Slow, 120);
            if (Main.rand.NextBool(4))
            {
                int sentenceType;
                switch (Main.rand.Next(3))
                {
                    default:
                    case 0:
                        sentenceType = ModContent.ProjectileType<DogmaSentence1>();
                        break;
                    case 1:
                        sentenceType = ModContent.ProjectileType<DogmaSentence2>();
                        break;
                    case 2:
                        sentenceType = ModContent.ProjectileType<DogmaSentence3>();
                        break;
                }
                for (float f = 0; f < 6; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Cyan, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Cyan,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(12, 25),
                        baseSize: Main.rand.NextFloat(0.08f, 0.2f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/StarFlower3");
                explosionSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound, Projectile.position);

                FXUtil.ShakeCamera(target.position, 1024, 4);
                List<int> exclude = new List<int>()
                {
                    ProjectileID.Bomb,
                    ProjectileID.BombFish,
                    ProjectileID.RocketIV,
                    ProjectileID.RocketSnowmanIV,
                    ProjectileID.Celeb2RocketExplosive,
                    ProjectileID.Celeb2RocketExplosiveLarge,
                    ProjectileID.ExplosiveBunny,
                    ProjectileID.Explosives,
                    ProjectileID.Dynamite,
                    ProjectileID.DynamiteKitten,
                    ProjectileID.StickyBomb,
                    ProjectileID.StickyDynamite,
                    ProjectileID.DirtStickyBomb,
                    ProjectileID.WetBomb,
                    ProjectileID.WetGrenade,
                    ProjectileID.WetMine,
                    ProjectileID.WetRocket,
                    ProjectileID.WetSnowmanRocket,
                    ProjectileID.LavaBomb,
                    ProjectileID.LavaGrenade,
                    ProjectileID.LavaMine,
                    ProjectileID.LavaRocket,
                    ProjectileID.DirtBomb,
                    ProjectileID.HoneyBomb,
                    ProjectileID.HoneyGrenade,
                    ProjectileID.HoneyMine,
                    ProjectileID.HoneyRocket,
                    ProjectileID.HoneySnowmanRocket,
                    ProjectileID.ClusterFragmentsI,
                    ProjectileID.ClusterFragmentsII,
                    ProjectileID.ClusterGrenadeI,
                    ProjectileID.ClusterGrenadeII,
                    ProjectileID.ClusterMineI,
                    ProjectileID.ClusterMineII,
                    ProjectileID.ClusterRocketI,
                    ProjectileID.ClusterRocketII,
                    ProjectileID.ClusterSnowmanFragmentsI,
                    ProjectileID.ClusterSnowmanFragmentsII,
                    ProjectileID.ClusterSnowmanRocketI,
                    ProjectileID.ClusterSnowmanRocketII,
                    ProjectileID.BouncyBomb,
                    ProjectileID.BouncyDynamite,
                };
                for (int i = 0; i < 7; i++)
                {
                    int id = Main.rand.Next(0, 1001);
                    while (exclude.Contains(id))
                        id = Main.rand.Next(0, 1001);
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, -Vector2.UnitY * Main.rand.NextFloat(3f, 7f),
                                 id, Projectile.damage, 1, Projectile.owner);
                    target.AddBuff(Main.rand.Next(0, 200), 180);
                    if (!p.friendly || p.hostile || p.minion || ProjectileID.Sets.LightPet[p.type] || Main.projPet[p.type])
                        p.active = false;
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, -Vector2.UnitY * Main.rand.NextFloat(3f, 7f),
                    sentenceType, 0, 1, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity, GoreHelper.TypePaper);
            }
            for (float f = 0; f < 16; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<PaperDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

        }
    }
}
