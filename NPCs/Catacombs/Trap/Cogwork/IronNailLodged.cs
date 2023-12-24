using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Trap.Cogwork
{
    internal class IronNailLodged : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.light = 0.75f;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            for (int i = 0; i < 1; i++)
            {
                int num7 = 16;
                float num8 = (float)(Math.Cos(Main.GlobalTimeWrappedHourly % 2.4 / 2.4 * MathHelper.TwoPi) / 5 + 0.5);
                SpriteEffects spriteEffects = SpriteEffects.None;
                var vector2_3 = new Vector2((TextureAssets.Projectile[Projectile.type].Value.Width / 2), (TextureAssets.Projectile[Projectile.type].Value.Height / 1 / 2));
                var color2 = new Color(255, 8, 55, 150);
                Rectangle r = TextureAssets.Item[Projectile.type].Value.Frame(1, 1, 0, 0);
                for (int index2 = 0; index2 < num7; ++index2)
                {
                    Color color3 = Projectile.GetAlpha(color2) * (0.85f - num8);
                    Vector2 position2 = Projectile.Center + ((index2 / num7 * MathHelper.TwoPi) + 0f).ToRotationVector2() * (4.0f * num8 + 2.0f) - Main.screenPosition - new Vector2(texture.Width + 8, texture.Height) * Projectile.scale / 2f + vector2_3 * Projectile.scale;
                    Main.spriteBatch.Draw(TextureAssets.Item[Projectile.type].Value, position2, new Microsoft.Xna.Framework.Rectangle?(r), color3, 0f, vector2_3, Projectile.scale * 1.1f, spriteEffects, 0.0f);
                }
            }
            return base.PreDraw(ref lightColor);
        }

		public override void AI()
        {
            //Pretty sure projectiles automatically have regular velocity so we don't need to do anything here.
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Iron, speed, Scale: 2f);
                d.noGravity = true;
            }
        }
    }
}
