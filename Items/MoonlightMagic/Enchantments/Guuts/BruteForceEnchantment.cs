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

namespace Stellamod.Items.MoonlightMagic.Enchantments.Guuts
{
    internal class BruteForceEnchantment : BaseEnchantment
    {
        public override int GetElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Spawn the explosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BruteForceEnchantmentExplosion>(),
              Projectile.damage / 2, Projectile.knockBack * 2, Projectile.owner);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            //Spawn the explosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<BruteForceEnchantmentExplosion>(),
                Projectile.damage / 2, Projectile.knockBack * 2, Projectile.owner);
        }
    }

    internal class BruteForceEnchantmentExplosion : BaseSpikeAuraExplosionProjectile
    {
        int rStart = 4;
        int rEnd = 128;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 30;
            rStart = Main.rand.Next(4, 8);
            rEnd = Main.rand.Next(96, 128);
        }

        protected override Color BeamColorFunction(float completionRatio)
        {
            return Color.White;
        }

        protected override float BeamWidthFunction(float completionRatio)
        {

            float trailWidth = MathHelper.Lerp(32, 16, Progress);
            float fadeWidth = MathHelper.Lerp(0, trailWidth,
                Easing.SpikeOutCirc(Progress)) * Main.rand.NextFloat(0.75f, 1.0f);
            return fadeWidth;
        }

        protected override float RadiusFunction(float completionRatio)
        {


            return MathHelper.Lerp(rStart, rEnd, Easing.OutCirc(completionRatio));
        }

        protected override void DrawPrims(Vector2[] trailPos)
        {
            base.DrawPrims(trailPos);
            DrawMainShader(trailPos);
        }

        private void DrawMainShader(Vector2[] trailPos)
        {
            //Trail
            var shader = MagicGuutShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrail;
            shader.NoiseTexture = TrailRegistry.CrystalTrail;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.White;
            shader.NoiseColor = Color.Black;
            shader.OutlineColor = Color.Lerp(Color.Black, Color.DarkGray, 0.3f);
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 2.2f;
            shader.Distortion = 0.05f;
            shader.Power = 0.1f;

            //This just applis the shader changes

            //Main Fill
            TrailDrawer.Draw(Main.spriteBatch, trailPos, Projectile.oldRot,
                BeamColorFunction, BeamWidthFunction, shader, offset: Projectile.Size / 2);
        }
    }
}
