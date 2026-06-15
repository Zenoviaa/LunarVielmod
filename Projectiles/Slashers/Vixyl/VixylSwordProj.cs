using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Vixyl
{
    public class VixylSwordProj : ModProjectile
    {
        private bool _init;
        public override string Texture => "Stellamod/Items/Weapons/Melee/Vixyl";

        ref float Dir => ref Projectile.ai[0];

        //Swing Stats
        public float SwingDistance;
        public int SwingTime = 24 * Swing_Speed_Multiplier;
        public float holdOffset = 36;

        //Ending Swing Time so it doesn't immediately go away after the swing ends, makes it look cleaner I think
        public int EndSwingTime = 4 * Swing_Speed_Multiplier;

        //This is for smoothin the trail
        public static int Swing_Speed_Multiplier => 8;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;

            Projectile.extraUpdates = Swing_Speed_Multiplier - 1;
            Projectile.usesLocalNPCImmunity = true;

            //Multiplying by the thing so it's still 10 ticks
            Projectile.localNPCHitCooldown = 10 * Swing_Speed_Multiplier;
        }

        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];
            if (!_init)
            {
                SwingTime = (int)(SwingTime / player.GetAttackSpeed(DamageClass.Melee));
                _init = true;
                Projectile.alpha = 255;
                Projectile.timeLeft = SwingTime + EndSwingTime;
            }
            else if (_init)
            {
                if (!player.active || player.dead || player.CCed || player.noItems)
                {
                    return;
                }

                Projectile.alpha = 0;
                Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
                float multiplier = 0.2f;
                RGB *= multiplier;

                Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

                int dir = (int)Dir;

                //Get the swing progress
                float lerpValue = Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft, true);

                //Smooth it some more
                float swingProgress = Easing.InOutExpo(lerpValue, 10f);

                // the actual rotation it should have
                float defRot = Projectile.velocity.ToRotation();
                // starting rotation

                //How wide is the swing, in radians
                float swingRange = MathHelper.PiOver2 + MathHelper.PiOver4;
                float start = defRot - swingRange;

                // ending rotation
                float end = (defRot + swingRange);

                // current rotation obv
                // angle lerp causes some weird things here, so just use a normal lerp
                float rotation = dir == 1 ? MathHelper.Lerp(start, end, swingProgress) : MathHelper.Lerp(end, start, swingProgress);

                // offsetted cuz sword sprite
                Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
                position += rotation.ToRotationVector2() * holdOffset;
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = rotation * player.direction;
                player.itemTime = 2;
                player.itemAnimation = 2;


                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                Vector2 armPosition = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

                armPosition.Y += player.gfxOffY;
                Projectile.Center = armPosition; // Set projectile to arm position
                Projectile.Center += holdOffset * rotation.ToRotationVector2();
                //     Projectile.Center += new Vector2(-9, -9).RotatedBy(rotation);
                //  Projectile.position -= new Vector2(0, 4);
            }
        }
        
        
        public override bool PreDraw(ref Color lightColor)
        {
 

    
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2;
            Color drawColor = Projectile.GetAlpha(lightColor);


            Main.EntitySpriteDraw(texture,
               Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
               sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itsel
     
            return false;

        }
    }
}
