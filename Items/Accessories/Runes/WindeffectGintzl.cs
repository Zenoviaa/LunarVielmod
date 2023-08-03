using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Runes
{
    public class WindeffectGintzl : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("windeffect");
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.scale = 0.7f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 400;
            DrawOriginOffsetX = -140;
            DrawOriginOffsetY = -38;
        }
        public override void AI()
        {

            if (Projectile.alpha >= 125)
            {
                Projectile.alpha -= 2;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            if (!player.GetModPlayer<MyPlayer>().WindRune)
            {
                Projectile.timeLeft = 1;
            }
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 9)
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.timeLeft <= 20)
            {
                Projectile.alpha += 16;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100) * (1f - Projectile.alpha / 255f);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}