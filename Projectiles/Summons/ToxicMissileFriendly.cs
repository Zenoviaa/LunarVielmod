using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons
{
    internal class ToxicMissileFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Missile");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false; 
            Projectile.timeLeft = 1000;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 1;
            Projectile.damage = 60;
        }

        int timer;
        int colortimer;
        public override bool PreAI()
        {
            timer++;
            if (timer <= 50)
            {
                colortimer++;
            }

            if (timer > 50)
            {
                colortimer--;
            }

            if (timer >= 100)
            {
                timer = 0;
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            SoundEngine.PlaySound(SoundID.Item109, Projectile.position);
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = (height = 8);
            fallThrough = base.Projectile.position.Y <= base.Projectile.ai[1];
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GunFlash>(), (Vector2.One * Main.rand.Next(1, 4)).RotatedByRandom(19.0), newColor: ColorFunctions.AcidFlame);
                d.rotation = (d.position - Projectile.position).ToRotation() - MathHelper.PiOver4;
            }

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, 74, (Vector2.One * Main.rand.Next(1, 4)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
            }
        //    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Missile_Land"), Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150 + colortimer * 2, 150 + colortimer * 2, 150 + colortimer * 2, 100);
        }
    }
}
