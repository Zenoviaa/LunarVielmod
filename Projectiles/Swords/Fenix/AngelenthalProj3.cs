using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Fenix
{
    public class AngelenthalProj3 : ModProjectile
    {
        public float holdOffset = 100f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        private Player Owner => Main.player[Projectile.owner];
        private int SwingTime => (int) (30 / Owner.GetAttackSpeed(DamageClass.Melee));

        public override void SetDefaults()
        {
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 90;
            Projectile.width = 98;
            Projectile.friendly = true;
            Projectile.scale = 1f;

            Projectile.usesLocalNPCImmunity = true;

            //Setting this to negative 1 so it only hits once
            Projectile.localNPCHitCooldown = -1;
        }

        public virtual float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }

        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.CCed || Owner.noItems)
            {
                Projectile.Kill();
                return;
            }

            int dir = (int)Projectile.ai[1];
            float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
            // the actual rotation it should have
            float defRot = Projectile.velocity.ToRotation();
            // starting rotation
            float endSet = ((MathHelper.PiOver2) / 0.2f);
            float start = defRot - endSet;

            // ending rotation
            float end = defRot + endSet;
            // current rotation obv
            float rotation = dir == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
            // offsetted cuz sword sprite
            Vector2 position = Main.MouseWorld;
            position += rotation.ToRotationVector2() * holdOffset;
            Projectile.Center = position;
            Projectile.rotation = (position - Owner.Center).ToRotation() + MathHelper.PiOver4;

            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            Owner.itemRotation = rotation * Owner.direction;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Visuals();
        }

        private void Visuals()
        {
            Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
            float multiplier = 0.2f;
            RGB *= multiplier;
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            for (int i = 0; i < 1; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.HeartCrystal, 0, 0, 100, Color.Violet, 1f);
                dust.noGravity = true;
                dust.velocity *= 2f;
            }
        }

        public override bool ShouldUpdatePosition() => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<MyPlayer>().SwordCombo++;
            player.GetModPlayer<MyPlayer>().SwordComboR = 480;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}