using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Phantasmal
{
    internal class ShriekerEnchantment : BaseEnchantment
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

    internal class ShriekerEnchantmentExplosion : BaseShriekExplosionProjectile
    {
        private int _trailMode;
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
            var shader = MagicPhantasmalShader.Instance;
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
            var shader = MagicPhantasmalShader.Instance;
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
            return Color.Lerp(new Color(69, 196, 182), Color.SpringGreen, completionRatio) * (1.0f - Easing.InCirc(Progress));
        }

        private float WidthFunction(float completionRatio)
        {
            if (_trailMode == 0)
            {
                float width1 = Easing.SpikeOutCirc(completionRatio) * 128;
                float width2 = Easing.SpikeOutCirc(completionRatio) * 196;
                float trailWidth = MathHelper.Lerp(width1, width2, Easing.OutExpo(Progress));
                return trailWidth;
            }
            else
            {
                float width = MathHelper.Lerp(64, 80, Progress);
                float trailWidth = MathHelper.Lerp(0, width, Easing.OutExpo(completionRatio));
                return trailWidth;
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
