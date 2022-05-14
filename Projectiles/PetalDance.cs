using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.Enums;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Buffs;


namespace Stellamod.Projectiles
{
    public class PetalDance : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("death");
            Main.projFrames[Projectile.type] = 14;
        }


        private int ProjectileSpawnedCount = 0;
        private int ProjectileSpawnedMax = 20;
        private bool MouseRightBool = false;
        private bool Morrowflames = false;
        private bool MouseLeftBool = true;
        private object player;

        public override void SetDefaults()
        {
            Projectile.damage = 0;

            Projectile.width = 340;
            Projectile.height = 260;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.3f;
            DrawOriginOffsetX = -90;
            DrawOriginOffsetY = -8;


        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }


        public override void AI()
        {
            Timer++;
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();

            }
            Projectile.velocity = swordRotation.ToRotationVector2();
            if (Timer == 1)
            {
                player.AddBuff(ModContent.BuffType<Elegance>(), 600);
            }
            if (!Projectile.active)
            {
                player.ClearBuff(ModContent.BuffType<Elegance>());
            }








            Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position








            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 14)
                {
                    Projectile.frame = 0;
                }
            }


        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {



            overPlayers.Add(index);
        }

      



    }
}