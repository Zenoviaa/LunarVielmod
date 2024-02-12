using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Materials;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles
{
    internal class AuroreanStar : ModProjectile
    {
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
            Projectile.width = 33;
            Projectile.height = 33;
            Projectile.timeLeft = 250;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
   

            if(Projectile.ai[1] >= 30)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 2212f, 12f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
                if (Main.myPlayer == Projectile.owner)
                {
                    float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                    float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
               
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, 
                        ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
                 
                    int itemIndex = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.getRect(), 
                        ModContent.ItemType<AuroreanStarI>(), Main.rand.Next(1, 1));
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
                }
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
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

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


