using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Core.Bases;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;
namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Phantasmal
{
    public class ShriekerEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<PhantasmalElement>();
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Spawn the explosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY, ModContent.ProjectileType<ShriekerEnchantmentExplosion>(),
              Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            //Spawn the explosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, -Vector2.UnitY, ModContent.ProjectileType<ShriekerEnchantmentExplosion>(),
                Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }
    }

    public class ShriekerEnchantmentExplosion : BaseShriekExplosionProjectile
    {
        private int _trailMode;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileSets.BossMultihitDamageFalloff[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 24;
        }


        protected override void DrawPrims(Vector2[] trailPos)
        {
            base.DrawPrims(trailPos);
            _trailMode = 0;
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, trailPos, Projectile.oldRot,
                ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        protected override void DrawMiniWispPrims(Vector2[] trailPos)
        {
            base.DrawMiniWispPrims(trailPos);
            _trailMode = 1;
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, trailPos, Projectile.oldRot,
                ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        protected override float WispRadiusFunction(float completionRatio)
        {
            return 16 * completionRatio;
        }

        protected override float DistanceFunction(float completionRatio)
        {
            return 300 * completionRatio;
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(69, 196, 182), Color.SpringGreen, completionRatio) * (1.0f - EasingFunction.InCirc(Progress));
        }

        private float WidthFunction(float completionRatio)
        {
            if (_trailMode == 0)
            {
                float width1 = EasingFunction.QuadraticBump(completionRatio) * 128;
                float width2 = EasingFunction.QuadraticBump(completionRatio) * 196;
                float trailWidth = MathHelper.Lerp(width1, width2, EasingFunction.OutExpo(Progress));
                return trailWidth;
            }
            else
            {
                float width = MathHelper.Lerp(64, 80, Progress);
                float trailWidth = MathHelper.Lerp(0, width, EasingFunction.OutExpo(completionRatio));
                return trailWidth;
                /*float w = 80;
                float width = w;

                float p = completionRatio / 0.5f;
                float ep = EasingFunction.OutCirc(p);
                float circleWidth = MathHelper.Lerp(0, w, ep);
                float trailWidth = MathHelper.Lerp(0, width, EasingFunction.OutCirc(completionRatio));
                return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio)) * EasingFunction.SpikeOutCirc(Progress);
                */
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Vector2 upwardVelocity = -Vector2.UnitY * Projectile.knockBack * 2.5f;
            upwardVelocity *= target.knockBackResist;
            target.velocity += upwardVelocity;
        }
    }
}
