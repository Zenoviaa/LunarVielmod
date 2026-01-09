using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.ItemsShop
{
    public class VoidsGrasp : AbstractMagicTome
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shootSpeed = 25;
            Item.damage = 28;
            Item.shoot = ModContent.ProjectileType<VoidHandSpawn>();
            Item.mana = 25;
        }

        public override Color GetTomeHintColor()
        {
            return Color.Purple;
        }
    }
    public class VoidHandSpawn : ModProjectile
    {
        private Vector2 OldVelocity;
        private ref float Timer => ref Projectile.ai[0];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(OldVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            OldVelocity = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = 4;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 700;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }


        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidHand"), Projectile.position);
                OldVelocity = Projectile.velocity;
            }
            if (Timer == 20)
            {
                SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned, Projectile.position);
            }

            if (Timer >= 20)
            {
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 3;
                    }
                }
            }

            if (Timer == 40)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, OldVelocity.X, OldVelocity.Y,
                          ModContent.ProjectileType<VoidHand>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidHand3"), Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidHand2"), Projectile.position);
                }
                Projectile.timeLeft = 2;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= .86f;
            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 12;
            }
            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 24; i++)
            {
                float progress = (float)i / 24f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 velocity = rot.ToRotationVector2() * 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, velocity);
            }
        }

        protected virtual void DrawSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSprite(ref lightColor);
            return false;
        }
    }

    public class VoidHand : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 1;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 480;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle voidHand = new SoundStyle("Stellamod/Assets/Sounds/VoidHand");
                voidHand.PitchVariance = 0.3f;
                SoundEngine.PlaySound(voidHand, Projectile.position);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Timer % 8 == 0)
            {
                Vector2 pos = Projectile.position;
                pos.X += Main.rand.Next(0, Projectile.width);
                pos.Y += Main.rand.Next(0, Projectile.height);
                DustParticle dp = Particle<DustParticle>.Spawn(pos, Vector2.Zero, Color.Purple, Scale: Main.rand.NextFloat(0.3f, 1f));
                dp.gravity = 0;
            }

            if (Timer >= 20)
            {
                Projectile.tileCollide = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 24; i++)
            {
                float progress = (float)i / 24f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 velocity = rot.ToRotationVector2() * 4;
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, velocity);
            }
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<Skullboom>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner);
        }

        public float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(32, 0, completionRatio);
        }

        public Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.Purple, Color.DarkBlue, completionRatio);
        }

        private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
        {
            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.Violet;
            shader.OuterColor = Color.White;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size / 2f);
        }

        private void DrawSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail, DrawLayer.OverNPCsWithOutline);
            DrawSprite(ref lightColor);
            return false;
        }
    }
}
