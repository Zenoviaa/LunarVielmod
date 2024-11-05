using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class ElectricNode : ModProjectile
    {
        private float _scale;
        private float _width;
        private Vector2[] _lightningZaps;
        private ref float Timer => ref Projectile.ai[0];
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningZaps = new Vector2[6];
            Projectile.width = 49;
            Projectile.height = 49;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.timeLeft = 420;
            Projectile.light = 0.48f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;

            SpriteBatch spriteBatch = Main.spriteBatch;

            Lightning.Draw(spriteBatch, _lightningZaps, Projectile.oldRot);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 16; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                float rot = Main.rand.NextFloat(0f, 3.14f);

                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor * 0.5f, drawRotation + rot, Projectile.Frame().Size() / 2f,
                    drawScale * VectorHelper.Osc(0.5f, 1f, speed: 6, offset: i), SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Wave");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if (Timer % 3 == 0)
            {
                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    float progress = (float)i / (float)_lightningZaps.Length;
                    float rot = progress * MathHelper.TwoPi * 1 + (Timer * 0.05f);
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * VectorHelper.Osc(0, 8, speed: 3);
                    _lightningZaps[i] = Projectile.Center + offset;
                }

                Lightning.RandomPositions(_lightningZaps);
            }

            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer <= 15)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.2f, 0.3f), Easing.InCubic(Timer / 15f));
            }

            Projectile.velocity *= 0.96f;

            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, float.MaxValue);
            if (player != null)
            {
                Vector2 dirToNpc = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, degreesToRotate: 1f);
            }


            if (Projectile.velocity.Length() < 1)
            {
                Projectile.Kill();
            }

            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            SoundStyle zapSound = SoundID.DD2_LightningBugZap;
            zapSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(zapSound, target.Center);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<ElectricField>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
