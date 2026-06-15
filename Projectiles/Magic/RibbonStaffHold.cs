using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class RibbonStaffHold : ModProjectile
    {
        private Vector2[] BungeeGumPos = new Vector2[4];
        private ref float SwordRotation => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 62;
            Projectile.aiStyle = 595;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            Aim();
        }

        private void Aim()
        {
            //Aiming Code
            Player player = Main.player[Projectile.owner];


            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.netUpdate = true;
                if (!player.channel)
                    Projectile.Kill();
            }

            Projectile.velocity = SwordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi - MathHelper.PiOver4;


            Projectile.Center = playerCenter + Projectile.velocity * 32;// customization of the hitbox position

            //Interesting trail

            BungeeGumPos[0] = player.MountedCenter + new Vector2(-26, -24) + Projectile.velocity * 48;
            BungeeGumPos[1] = BungeeGumPos[0];
            BungeeGumPos[2] = Main.MouseWorld;
            BungeeGumPos[3] = BungeeGumPos[2];

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool ShouldUpdatePosition()
        {
            //Make velocity not move it
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return baseWidth * VectorHelper.Osc(0.5f, 1f, 3) * 0.2f;
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {


            return false;
        }
    }
}