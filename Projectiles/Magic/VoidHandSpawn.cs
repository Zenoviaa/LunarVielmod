using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Magic
{
    internal class VoidHandSpawn : ModProjectile
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
            if(Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidHand"), Projectile.position);
                OldVelocity = Projectile.velocity;
            }
            if(Timer == 20)
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

            if(Timer == 40)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                if(Main.myPlayer == Projectile.owner)
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

        private void DrawGlow(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (float f = 0f; f < 1f; f += 0.12f)
            {
                float rot = f * MathHelper.ToRadians(360);
                rot += Main.GlobalTimeWrappedHourly * 8;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(6f, 9f);
                offset -= Projectile.velocity * f * 1;
                Vector2 glowDrawPos = drawPos + offset;
                spriteBatch.Draw(texture, glowDrawPos, frame, drawColor * 0.52f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
            }

            spriteBatch.RestartDefaults();
        }
        protected virtual void DrawSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
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
            DrawGlow(ref lightColor);
            DrawSprite(ref lightColor);
            return false;
        }
    }
}