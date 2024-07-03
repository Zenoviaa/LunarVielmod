using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.STARBOMBER;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles
{
    internal class AuroreanStarbomber : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        bool Moved;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.timeLeft = 250;
            Projectile.alpha = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            if (Projectile.ai[1] >= 30)
            {
                float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 4000f, 12f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
            }

            for (int i = 0; i < 150; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 3, Scale: 3f);
                d.noGravity = true;
            }

            return false;
        }

        float alphaCounter = 1;
        public override void AI()
        {
            Projectile.velocity /= 0.99f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Projectile.velocity.X = Main.rand.NextFloat(-5, 5);
                Projectile.velocity.Y = 10;
                Projectile.spriteDirection = Projectile.direction;
                Projectile.alpha = 255;
                Projectile.netUpdate = true;
                Moved = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
            if (Projectile.ai[1] <= 1)
            {
                Projectile.scale = 1.5f;
            }

            if (Main.rand.NextBool(3))
            {
                int dustnumber = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].velocity.Y += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].velocity.X += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].noGravity = true;
                Main.dust[dustnumber].noLight = false;
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        public override void OnKill(int timeLeft)
        {
            SpawnStarBomber();
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode"), Projectile.position);

            MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
            myPlayer.ShakeAtPosition(Projectile.position, 6000, 48);

            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), 
                    (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Pink, 1f).noGravity = true;
            }

            for (int i = 0; i < 80; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), 
                    (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (int i = 0; i < 8; i++)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, Color.White, randScale);
            }
        }

        private void SpawnStarBomber()
        {
            if(Main.myPlayer == Projectile.owner)
            {
                Main.NewText(LangText.Misc("AuroreanStarbomber"), Color.Pink);
                int npcID = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, 
                    ModContent.NPCType<STARBOMBER>());
                NetMessage.SendData(MessageID.SyncNPC);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.PaleGoldenrod, Color.Goldenrod, completionRatio) * 0.7f;
        }

        public float WidthFunction2(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }

        public Color ColorFunction2(float completionRatio)
        {
            return Color.Lerp(Color.Pink, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 frameSize = new Vector2(132, 150);
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction2, ColorFunction2, TrailRegistry.StarTrail, frameSize: frameSize);
            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(60f * alphaCounter), (int)(25f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.37f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(60f * alphaCounter), (int)(25f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(60f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(60f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.HotPink.ToVector3() * 2.0f * Main.essScale);

        }
    }
}


