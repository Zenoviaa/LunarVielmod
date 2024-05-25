using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Core.Utils;
using Stellamod.Helpers;
using Stellamod.Projectiles.Magic;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    public class SOJGCustomSwingProjectile : ModProjectile
    {
        private bool _init;
        public override string Texture => "Stellamod/Items/Weapons/Melee/SwordOfTheJungleGoddess";

        ref float Dir => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];

        //Swing Stats
        public float SwingDistance;
        public int SwingTime = 90 * Swing_Speed_Multiplier;
        public float holdOffset = 60f;

        //Ending Swing Time so it doesn't immediately go away after the swing ends, makes it look cleaner I think
        public int EndSwingTime = 4 * Swing_Speed_Multiplier;

        //This is for smoothin the trail
        public const int Swing_Speed_Multiplier = 8;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;

            Projectile.extraUpdates = Swing_Speed_Multiplier - 1;
            Projectile.usesLocalNPCImmunity = true;

            //Multiplying by the thing so it's still 10 ticks
            Projectile.localNPCHitCooldown = 90 * Swing_Speed_Multiplier;
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
                float swingProgress = Easing.InExpo(lerpValue, 10f);

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

                Timer++;
                if (Timer >= 6 * Swing_Speed_Multiplier)
                {
                    Vector2 offset = Projectile.rotation.ToRotationVector2();
                    Vector2 starSpawnCenter = Projectile.Center + offset;
                    Timer = 0;
                }


                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = rotation * player.direction;
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 1.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Turquoise, Color.Transparent, completionRatio) * 0.7f;
        }


        public TrailRenderer SwordSlash;
        public TrailRenderer SwordSlash2;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();

            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/BulbTrail").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhispyTrail").Value;

            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);



            /*
            Texture2D Texture2 = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(Texture2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(Texture2.Width / 2, Texture2.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);*/
            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(70f), (p) => new Color(155, 255, 20, 50) * (1f - p));
                SwordSlash.drawOffset = Projectile.Size / 1.8f;
            }

            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(70f), (p) => new Color(255, 255, 255, 4) * (1f - p));
                SwordSlash2.drawOffset = Projectile.Size / 1.9f;
            }

            Main.spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);


            float[] rotation = new float[Projectile.oldRot.Length];
            for (int i = 0; i < rotation.Length; i++)
            {
                rotation[i] = Projectile.oldRot[i] - MathHelper.ToRadians(45);
            }


            SwordSlash.Draw(Projectile.oldPos, rotation);
            SwordSlash2.Draw(Projectile.oldPos, rotation);


            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);


            Main.EntitySpriteDraw(texture,
               Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
               sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itself

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;

        }

    }
}