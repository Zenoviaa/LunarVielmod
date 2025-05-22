
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public class Card5 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.JavelinFriendly);
            AIType = ProjectileID.JavelinFriendly;
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 35; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                var d = Dust.NewDustPerfect(Main.LocalPlayer.Center, DustID.FireworkFountain_Pink, speed * 6, Scale: 0.9f);
                
                d.noGravity = true;
                d.velocity *= 0.3f;
            }

            NPC npc = target;
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Poisoned, 180);
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire, 180);
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Frostburn2, 180);
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Lovestruck, 180);
            if (Main.rand.NextBool(4))
                target.AddBuff(BuffID.Ichor, 180);
            if (Main.rand.NextBool(4))
                target.AddBuff(BuffID.Confused, 180);
            if (Main.rand.NextBool(4))
                target.AddBuff(BuffID.OnFire3, 180);
            if (Main.rand.NextBool(6))
                target.AddBuff(ModContent.BuffType<Alcadded>(), 180);
            if (Main.rand.NextBool(10))
            {
                npc.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
                float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Projectile.Center);
                //  Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<AivanKaboom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Projectile.Center);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
            }

            return false;
        }

        public override bool PreAI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
            }

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
            }
        }
    }
}