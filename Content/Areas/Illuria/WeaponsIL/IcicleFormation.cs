using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class ShortIcicleFormation : ModProjectile
    {
        private Vector2 _parentOffset;
        private IcicleSystem _icicleSystemBackingField;
        private IcicleSystem IcicleSystem
        {
            get
            {
                if (_icicleSystemBackingField == null)
                {
                    _icicleSystemBackingField = new IcicleSystem(2, steps: Steps);
                }
                return _icicleSystemBackingField;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        private int Steps => (int)Projectile.ai[1];
        private int Parent => (int)Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1 && Parent != -1)
            {
                _parentOffset = Projectile.Center - Main.npc[Parent].Center;
            }
            float time = Timer / 30f;
            time = EasingFunction.OutExpo(time);

            float outInterp = (float)Projectile.timeLeft / 30f;
            float outScale = EasingFunction.InOutSine(outInterp);
            time *= outScale;
            IcicleSystem.Update(Projectile.Center, Projectile.velocity, time);
            if (Parent != -1)
            {
                NPC parentNPC = Main.npc[Parent];
                Projectile.Center = parentNPC.Center + _parentOffset;
                if (!parentNPC.active)
                {
                    Projectile.Kill();
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            IceRenderer.QueueDrawAction(DrawPixelIcicles);
            return false;
        }

        private void DrawPixelIcicles(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            IcicleSystem.Draw(spriteBatch, screenPos);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            int rand = Main.rand.Next(0, 2);
            SoundStyle soundStyle;
            switch (rand)
            {
                default:
                case 0:
                    soundStyle = AssetRegistry.Sounds.Illuria.IceImpact1;
                    break;
                case 1:
                    soundStyle = AssetRegistry.Sounds.Illuria.IceImpact2;
                    break;
            }
            soundStyle.PitchVariance = 0.3f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            for (float f = 0; f < 2; f++)
            {
                Vector2 initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                initialVelocity *= 6;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                DustParticle dustParticle = Particle<DustParticle>.Spawn(Projectile.Center, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.2f, 0.5f));
                dustParticle.innerColor = Color.SkyBlue;
                dustParticle.outerColor = Color.Violet;
            }
        }
    }
    public class IcicleFormation : ModProjectile
    {
        private Vector2 _parentOffset;
        private IcicleSystem _icicleSystemBackingField;
        private IcicleSystem IcicleSystem
        {
            get
            {
                if (_icicleSystemBackingField == null)
                {
                    _icicleSystemBackingField = new IcicleSystem(2, steps: Steps);
                }
                return _icicleSystemBackingField;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        private int Steps => (int)Projectile.ai[1];
        private int Parent => (int)Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1 && Parent != -1)
            {
                _parentOffset = Projectile.Center - Main.npc[Parent].Center;
            }
            float time = Timer / 120f;
            time = EasingFunction.OutExpo(time);

            float outInterp = (float)Projectile.timeLeft / 30f;
            float outScale = EasingFunction.InOutSine(outInterp);
            time *= outScale;
            IcicleSystem.Update(Projectile.Center, Projectile.velocity, time);
            if (Parent != -1)
            {
                NPC parentNPC = Main.npc[Parent];
                Projectile.Center = parentNPC.Center + _parentOffset;
                if (!parentNPC.active)
                {
                    Projectile.Kill();
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            IceRenderer.QueueDrawAction(DrawPixelIcicles);
            return false;
        }

        private void DrawPixelIcicles(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            IcicleSystem.Draw(spriteBatch, screenPos);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            int rand = Main.rand.Next(0, 2);
            SoundStyle soundStyle;
            switch (rand)
            {
                default:
                case 0:
                    soundStyle = AssetRegistry.Sounds.Illuria.IceImpact1;
                    break;
                case 1:
                    soundStyle = AssetRegistry.Sounds.Illuria.IceImpact2;
                    break;
            }
            soundStyle.PitchVariance = 0.3f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            for (float f = 0; f < 2; f++)
            {
                Vector2 initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                initialVelocity *= 6;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                DustParticle dustParticle = Particle<DustParticle>.Spawn(Projectile.Center, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.2f, 0.5f));
                dustParticle.innerColor = Color.SkyBlue;
                dustParticle.outerColor = Color.Violet;
            }
        }
    }
}
