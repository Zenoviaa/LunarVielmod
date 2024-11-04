using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class MegaConjureBallLightning : ModProjectile
    {
        private float _scale;
        private float _width;
        private Vector2[] _lightningZaps;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Charge => ref Projectile.ai[1];

        private ref float Parent => ref Projectile.ai[2];

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
            _width = 1;
            _lightningZaps = new Vector2[7];
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.timeLeft = 600;
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

            Lightning.WidthMultiplier = 2;
            Lightning.SetBoltDefaults();
            Lightning.Draw(spriteBatch, _lightningZaps, null);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 16; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                float rot = Main.rand.NextFloat(0f, 3.14f);

                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation + rot, Projectile.Frame().Size() / 2f,
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

                    float osc = VectorHelper.Osc(256, 384, speed: 3);
                    float p = Timer / 300f;
                    osc *= MathHelper.Lerp(1f, 0.5f, p);
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * osc; 
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

            if (Timer <= 300f)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(6, 8), Timer / 300f);
            }

            if(Timer < 300)
            {
                NPC parentNpc = Main.npc[(int)Parent];
                Projectile.Center = parentNpc.Center - new Vector2(0, 256);
                Projectile.velocity = Vector2.Zero;
            }

            if(Timer > 300)
            {
                Projectile.tileCollide = true;
                Projectile.velocity = Vector2.UnitY * 3;
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
            for (int i = 0; i < 16; i++)
            {
                float progress = (float)i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            //EXPLODE
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<ConjureBallExplosionBig>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
