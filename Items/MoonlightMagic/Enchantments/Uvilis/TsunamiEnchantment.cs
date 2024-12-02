using CrystalMoon.Systems.MiscellaneousMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Uvilis
{
    internal class TsunamiEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<UvilisElement>();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Color.Red);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            //Spawn the explosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                Projectile.velocity.SafeNormalize(Vector2.Zero), ModContent.ProjectileType<TsunamiEnchantmentExplosion>(),
              Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);


            //Spawn the explosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center,
                Projectile.velocity.SafeNormalize(Vector2.Zero), ModContent.ProjectileType<TsunamiEnchantmentExplosion>(),
                Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }
    }


    internal class TsunamiEnchantmentExplosion : BaseExplosionProjectile
    {
        int trailingMode;
        int rStart = 4;
        int rEnd = 128;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 64;
            rStart = Main.rand.Next(4, 8);
            rEnd = Main.rand.Next(96, 164);
            _circlePos = new Vector2[128];
        }

        protected override float BeamWidthFunction(float p)
        {
            float width = 64 * 1.3f;
            p = Easing.SpikeOutCirc(p);

            switch (trailingMode)
            {
                default:
                case 0:
                    return MathHelper.Lerp(0, width, p);
                case 1:
                    return MathHelper.Lerp(0, width, p);
                case 2:
                    return MathHelper.Lerp(0, width + 12, p);
            }
        }

        protected override float WidthFunction(float p)
        {

            float m = 32;
            return base.WidthFunction(p) * Easing.SpikeOutCirc(MathF.Sin(p * 4))
                * MathUtil.Osc(0.75f, 1f, speed: 3, offset: p * 4)
                + MathUtil.Osc(5f, 10f, speed: 3, offset: MathF.Sin(p * 16));
        }

        protected override Color ColorFunction(float p)
        {
            //Main color of the beam
            Color c = Color.Blue;

            switch (trailingMode)
            {
                default:
                case 0:
                    break;
                case 1:
                    c.A = 0;
                    break;
                case 2:
                    c.A = 0;
                    break;
            }


            return c;
        }

        protected override float RadiusFunction(float p)
        {
            //How large the circle is going to be
            return MathHelper.Lerp(rStart, rEnd, Easing.OutCirc(p));// * MathUtil.Osc(0.75f, 1f, speed:12 , offset: p * 4);
        }


        private void DrawMainShader()
        {
            trailingMode = 0;
            var shader = MagicSparkleWaterShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
            shader.NoiseColor = new Color(92, 100, 255);
            shader.OutlineColor = Color.Black;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.248f;
            shader.Distortion = 1.5f;
            shader.Power = 0.5f;
            shader.Threshold = 0.25f;
            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunctionReal, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private void DrawOutlineShader()
        {
            trailingMode = 1;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;

            Color c = new Color(38, 204, 255);
            shader.PrimaryColor = c;
            shader.NoiseColor = c;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.248f;
            shader.Distortion = 1.5f;
            shader.Power = 2.5f;

            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunctionReal, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private void DrawOutlineShader2()
        {
            trailingMode = 2;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;

            Color c = Color.White;
            shader.PrimaryColor = c;
            shader.NoiseColor = c;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.248f;
            shader.Distortion = 1.5f;
            shader.Power = 3.5f;
            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunctionReal, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            DrawMainShader();
            DrawOutlineShader();
            DrawOutlineShader2();
            return base.PreDraw(ref lightColor);
        }
    }
}
