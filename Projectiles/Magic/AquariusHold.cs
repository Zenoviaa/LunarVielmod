using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.Thrown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class AquariusHold : ModProjectile
    {
        private float MagicCircleRotation;
        private float MagicCircleScale;

        private ref float Timer => ref Projectile.ai[0];
        private ref float SwordRotation => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 595;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool? CanDamage()
        {
            return false;
        }


        public override void AI()
        {
            Timer++;
            AI_Hold();
        }

        private void AI_Hold()
        {
            //Magic Circle Stuff
            MagicCircleRotation += MathHelper.PiOver4 / 24;
            MagicCircleScale += 0.01f;
            MagicCircleScale = MathHelper.Clamp(MagicCircleScale, 0, 0.4f);

            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.netUpdate = true;
                if (!player.channel)
                    Projectile.Kill();
            }

            if(Timer % 45 == 0)
            {
                int manaChannelCost = player.HeldItem.mana;
                if (!player.CheckMana(manaChannelCost, true))
                {
                    Projectile.Kill();
                }
                else
                {
                    //Make a slash
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                    SoundEngine.PlaySound(SoundID.Item21);
                    float maxSlashDistance = 1;
                    float slashDistance = Math.Min(maxSlashDistance, Vector2.Distance(player.Center, Main.MouseWorld));
    
                    Vector2 slashPosition = player.Center + player.Center.DirectionTo(Main.MouseWorld) * slashDistance;
                    Vector2 velocity = player.Center.DirectionTo(slashPosition) * 4;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), slashPosition, velocity,
                        ModContent.ProjectileType<AquariusSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if(Timer % 8 == 0)
            {
                int manaChannelCost = player.HeldItem.mana / 8;
                if (!player.CheckMana(manaChannelCost, true))
                {
                    Projectile.Kill();
                }
                else
                {
                    //Make a slash
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 2);
                    SoundEngine.PlaySound(SoundID.Item21);
  
                    Vector2 slashPosition = player.Center + Main.rand.NextVector2Circular(80, 80);
                    Vector2 velocity = player.Center.DirectionTo(slashPosition) * 8;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), slashPosition, velocity,
                        ModContent.ProjectileType<AquariusSlashMini>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner);
                }
            }

            Projectile.velocity = SwordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;


            Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
