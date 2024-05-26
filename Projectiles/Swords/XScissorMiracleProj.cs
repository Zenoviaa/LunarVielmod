using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    internal class XScissorMiracleProj : ModProjectile
    {
        private bool _init;
        public override string Texture => "Stellamod/Items/Weapons/Melee/XScissorMiracle";

        ref float Dir => ref Projectile.ai[0];
        ref float Speed => ref Main.player[Projectile.owner].GetModPlayer<XScissorComboPlayer>().speed;
        ref float Timer => ref Main.player[Projectile.owner].GetModPlayer<XScissorComboPlayer>().timer;

        //Swing Stats
        public float SwingDistance;
        public float SwingRangeOffset;
        public int SwingTime = 30 * Swing_Speed_Multiplier;
        public float holdOffset = 80;

        //Ending Swing Time so it doesn't immediately go away after the swing ends, makes it look cleaner I think
        public int EndSwingTime = 4 * Swing_Speed_Multiplier;

        //This is for smoothin the trail
        public const int Swing_Speed_Multiplier = 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 222;
            Projectile.height = 222;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;
            int swingSpeedMultiplier = Swing_Speed_Multiplier;
            Projectile.extraUpdates = swingSpeedMultiplier - 1;
            Projectile.usesLocalNPCImmunity = true;

            //Multiplying by the thing so it's still 10 ticks
            Projectile.localNPCHitCooldown = 10 * swingSpeedMultiplier;
        }

        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];
            if (!_init)
            {
                SwingTime = (int)(SwingTime / player.GetAttackSpeed(DamageClass.Melee) / Speed);
                SwingRangeOffset = MathHelper.PiOver4 + MathHelper.PiOver2;
                _init = true;
                Projectile.alpha = 255;
                Projectile.timeLeft = SwingTime + EndSwingTime;
            }

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
                float swingRange = SwingRangeOffset;
                float start = defRot - swingRange;

                // ending rotation
                float end = (defRot + swingRange);

                // current rotation obv
                // angle lerp causes some weird things here, so just use a normal lerp
                float rotation = dir == 1 ? MathHelper.Lerp(start, end, swingProgress) : MathHelper.Lerp(end, start, swingProgress);

                // offsetted cuz sword sprite
                Vector2 position = player.RotatedRelativePoint(player.MountedCenter);

                float hProgress = Speed / 3;
                float hOffset = MathHelper.Lerp(holdOffset, 100 + holdOffset, hProgress);
                position += rotation.ToRotationVector2() * hOffset;
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = rotation * player.direction;
                player.itemTime = 2;
                player.itemAnimation = 2;
            
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            //Increase speed when you hit
            Speed += 0.15f;
            Timer = 0;
            if (Speed >= 3)
            {
                Speed = 3;
            }

       
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), 
                    (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, ColorFunctions.MiracleVoid, 1f).noGravity = true;
            }

            for (int i = 0; i < 1; i++)
            {
                Dust.NewDust(target.position, target.width, target.height,
                    ModContent.DustType<GunFlash>(), newColor: ColorFunctions.MiracleVoid, Scale: 0.8f);
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/VoidHit");
            soundStyle.PitchVariance = 0.15f;
            soundStyle.Pitch = 0.75f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            Vector2 velocity = Main.rand.NextVector2Circular(1, 1);
            if(Speed >= 3)
            {
                if (Main.rand.NextBool(5))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<RipperSlashProjBig>(),
                     0, 0, Projectile.owner);
                }
                else
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<RipperSlashProjSmall>(),
       0, 0, Projectile.owner);
                }
         
            
            }
            else
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<RipperSlashProjSmall>(),
            0, 0, Projectile.owner);
            }
        
        }

        public TrailRenderer SwordSlash;
        public TrailRenderer SwordSlash2;
        public TrailRenderer SwordSlash3;

        private float GetAlpha()
        {
            //Get the swing progress
            float lerpValue = Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft, true);

            //Smooth it some more
            float a = Easing.SpikeInOutCirc(lerpValue);
            return a;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();

            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/VortexTrail").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhiteTrail").Value;

            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);

   
            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(4), 
                    (p) => new Color(ColorFunctions.MiracleVoid.R, ColorFunctions.MiracleVoid.G, ColorFunctions.MiracleVoid.B, 255) 
                    * Easing.SpikeInOutCirc(p) * 0.4f * GetAlpha());
                SwordSlash.drawOffset = Projectile.Size / 1.8f;
            }

            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(5),
                    (p) => new Color(0, 0, 0, 255)
                    * Easing.SpikeInOutCirc(p) * 0.4f * GetAlpha());
                SwordSlash2.drawOffset = Projectile.Size / 1.9f;
            }

            if (SwordSlash3 == null)
            {
                SwordSlash3 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(3),
                    (p) => Color.White
                    * Easing.SpikeInOutCirc(p) * GetAlpha());
                SwordSlash3.drawOffset = Projectile.Size / 2f;
            }

            Main.spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);


            float[] rotation = new float[Projectile.oldRot.Length];
            for (int i = 0; i < rotation.Length; i++)
            {
                rotation[i] = Projectile.oldRot[i] - MathHelper.ToRadians(45);
            }
            SwordSlash2.Draw(Projectile.oldPos, rotation);
            SwordSlash3.Draw(Projectile.oldPos, rotation);

            SwordSlash.Draw(Projectile.oldPos, rotation);
        
          
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);



            //drawOrigin.X = projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + Projectile.Size / 2f;// + new Vector2(0f, projectile.gfxOffY);
                Color ccolor = Projectile.GetAlpha(Color.Lerp(Color.Magenta, Color.Transparent, 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                ccolor *= 0.05f;
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, ccolor, Projectile.oldRot[k], origin, Projectile.scale, SpriteEffects.None, 0f);
            }

            /*
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
          sourceRectangle, drawColor * GetAlpha(), Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
            */
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }
    }
}
