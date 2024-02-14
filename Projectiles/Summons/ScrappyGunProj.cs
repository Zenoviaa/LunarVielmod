using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Items.Armors.Scrappy;
using Stellamod.Projectiles.Gun;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Projectiles.Summons
{
    internal class ScrappyGunProj  : ModProjectile
    {
        private bool _flip;
        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 38;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        private ref float ai_Timer => ref Projectile.ai[0];
        private ref float ai_Rot => ref Projectile.ai[1];
        public override void AI()
        {
            ai_Timer++;
            Player player = Main.player[Projectile.owner];
            bool hasSetBonus = player.GetModPlayer<ScrappyPlayer>().hasSetBonus;
            if (!hasSetBonus)
            {
                Projectile.Kill();
                return;
            }

            SummonHelper.SearchForTargets(player, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Projectile.Center = player.Center + new Vector2(0, -64);
            if (foundTarget)
            {
                Projectile.rotation = Projectile.DirectionTo(targetCenter).ToRotation();
                if (ai_Timer >= 30)
                {
                    Vector2 velocity = Projectile.DirectionTo(targetCenter) * 10;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<ScrappyGunLaser>(), Projectile.damage, 4, Projectile.owner, ai0: Projectile.whoAmI);
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap);
                    ai_Timer = 0;
                }
                if (targetCenter.X < Projectile.Center.X)
                    _flip = true;
            } else
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    ai_Rot = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
                    Projectile.netUpdate = true;
                }
                Projectile.rotation = ai_Rot;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if(_flip)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, 
                new Vector2(78/2, 38/2), 1f, spriteEffects, 0);
            return false;
        }
    }
}
