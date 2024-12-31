using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.JackTheScholar.Projectiles
{
    internal class Flamethrow : ModProjectile
    {
        private float _scale;
        private float _targetScale;
        private ref float Timer => ref Projectile.ai[0];

        private float LifeTime = 300;
        private float MaxScale = 0.24f;

        public override void SetDefaults()
        {
            _targetScale = Main.rand.NextFloat(0.75f, 1f);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = 7;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1 && Main.rand.NextBool(8))
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);
            }

            float lifeTime = LifeTime;
            float progress = Timer / LifeTime;
            float easedProgress = Easing.SpikeOutCirc(progress);
            _scale = MathHelper.Lerp(0f, _targetScale, easedProgress);
            Projectile.velocity *= 0.99f;
            Projectile.rotation += 0.05f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Chance to freeze the ground with an icey flower!

            //Return false to not kill itself
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.RoyalBlue.R,
                Color.RoyalBlue.G,
                Color.RoyalBlue.B, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = _scale +  VectorHelper.Osc(-0.015f, 0.015f, speed: 16, offset: Projectile.whoAmI);

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++)
            {
                float rot = (float)i / 4f;
                Vector2 vel = rot.ToRotationVector2() * VectorHelper.Osc(0f, 4f, speed: 16);
                Vector2 flameDrawPos = drawPos + vel + Main.rand.NextVector2Circular(2, 2);
                flameDrawPos -= Vector2.UnitY * 4;
                drawRotation += 0.05f;
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
