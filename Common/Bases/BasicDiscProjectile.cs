using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal class BasicDiscProjectile : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private int DiscItemType
        {
            get => (int)Projectile.ai[1];
        }

        public BaseDiscItem DiscItem
        {
            get
            {
                BaseDiscItem discItem = ModContent.GetModItem(DiscItemType) as BaseDiscItem;
                return discItem;
            }
        }
        public Texture2D DiscTexture
        {
            get
            {
                Texture2D texture = ModContent.Request<Texture2D>(DiscItem.Texture).Value;
                return texture;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.light = 0.38f;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                Projectile.penetrate = (int)DiscItem.Penetrate;
                Projectile.netUpdate = true;
            }

            Player owner = Main.player[Projectile.owner];
            SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (!foundTarget)
            {

            }
            else
            {
                float speed = 20;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, VectorHelper.VelocitySlowdownTo(Projectile.Center, targetCenter, speed), 0.1f);
            }

            Visuals();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            direction = direction.RotatedByRandom(MathHelper.ToRadians(30));
            Projectile.velocity = -direction * 71;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<NailKaboom>(), 0, 0, Projectile.owner);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2") with { PitchVariance = 0.1f }, target.position);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.62f;
            return MathHelper.SmoothStep(16, 3f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = DiscItem.TrailColor;
            return Color.Lerp(startColor, Color.Transparent, completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.RestartDefaults();
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            Vector2 trailOffset = -Main.screenPosition + Projectile.Size / 2;
            TrailDrawer.DrawPrims(Projectile.oldPos, trailOffset, 155);

            Texture2D texture = DiscTexture;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White.MultiplyRGB(lightColor);


            spriteBatch.Restart(blendState: BlendState.Additive);

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                Color backGlowColor = Color.White * VectorHelper.Osc(0.5f, 1f);
                float rot = f * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f);
                spriteBatch.Draw(texture, drawPos + offset, null, backGlowColor, 0, drawOrigin, 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.RestartDefaults();


            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);


            spriteBatch.Restart(blendState: BlendState.Additive);

            Color glowColor = Color.White * VectorHelper.Osc(0.15f, 0.75f);
            spriteBatch.Draw(texture, drawPos, null, glowColor, 0, drawOrigin, 1f, SpriteEffects.None, 0f);

            spriteBatch.RestartDefaults();
            return false;
        }

        private void Visuals()
        {
            Projectile.rotation += MathHelper.ToRadians(2);
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;
        }
    }
}
