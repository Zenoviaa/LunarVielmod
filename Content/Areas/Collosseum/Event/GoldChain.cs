
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.Event
{
    public class GoldChain : ModProjectile
    {
        private Vector2 OriginalCenter;
        private ref float Timer => ref Projectile.ai[0];
        private ref float DeathTimer => ref Projectile.ai[1];

        private bool Die;
        private float Length;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;

            //So you can't dash through it or anything
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.height * 0.8f;
            Vector2 start = Projectile.Center;

            Vector2 direction = Vector2.UnitX;
            Vector2 end = start + direction * (Length);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                OriginalCenter = Projectile.Center;
                SoundStyle gasp = AssetRegistry.Sounds.Collosseum.GintzeGasp;
                SoundEngine.PlaySound(gasp);
            }

            float leftLength = ProjectileHelper.PerformBeamHitscan(OriginalCenter, -Vector2.UnitX, 2400);
            Projectile.Left = OriginalCenter + new Vector2(-leftLength, 0) + new Vector2(8, 0);
            Length = ProjectileHelper.PerformBeamHitscan(Projectile.Left, Vector2.UnitX, 2400);

            if (!ColosseumWaveManager.IsActive())
            {
                Die = true;
            }
            else
            {
                Projectile.timeLeft = 60;
            }

            if (Die)
            {
                DeathTimer++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float deathProgress = MathHelper.Clamp(DeathTimer / 60f, 0f, 1f);
            float inProgress = MathHelper.Clamp(Timer / 60f, 0f, 1f);

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int x = (int)drawPos.X;
            int y = (int)drawPos.Y;

            Rectangle destinationRectangle = new Rectangle(x, y, (int)(Length * inProgress), texture.Height);
            Rectangle sourceRectangle = new Rectangle(0, 0, (int)(Length * inProgress), texture.Height);
            //This should scroll the texture
            sourceRectangle.X += (int)(Main.GlobalTimeWrappedHourly * 32);
            Vector2 drawOrigin = new Vector2(0, texture.Height / 2);

            Color chainColor = Color.White.MultiplyRGB(lightColor);

            chainColor = Color.Lerp(Color.Transparent, chainColor, inProgress);
            chainColor = Color.Lerp(chainColor, Color.Transparent, deathProgress);

            spriteBatch.Restart(samplerState: SamplerState.PointWrap, effect: SpriteWhiteShader.Instance.Effect);
            for (int i = 0; i < 4; i++)
            {
                float ratio = (float)i / 4f;
                Vector2 offset = (ratio * MathHelper.TwoPi).ToRotationVector2() * 2;
                Rectangle drawRectangle = destinationRectangle;
                drawRectangle.X += (int)offset.X;
                drawRectangle.Y += (int)offset.Y;
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.Red, Projectile.rotation, drawOrigin, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, chainColor, Projectile.rotation, drawOrigin, SpriteEffects.None, 0);

            spriteBatch.Restart(samplerState: SamplerState.PointWrap);
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, chainColor, Projectile.rotation, drawOrigin, SpriteEffects.None, 0);   
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
            target.AddBuff(BuffID.Frozen, 24);
        }
    }
}
