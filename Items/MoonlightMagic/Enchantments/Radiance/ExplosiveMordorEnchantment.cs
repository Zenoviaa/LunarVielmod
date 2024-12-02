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

namespace Stellamod.Items.MoonlightMagic.Enchantments.Radiance
{
    internal class ExplosiveMordorEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 0.8f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RadianceElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.RadianceYellow);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Spawn the explosion
            if (Projectile.owner == Main.myPlayer)
            {
                int explosionRadius = 5;
                int minTileX = (int)(Projectile.Center.X / 16f - explosionRadius);
                int maxTileX = (int)(Projectile.Center.X / 16f + explosionRadius);
                int minTileY = (int)(Projectile.Center.Y / 16f - explosionRadius);
                int maxTileY = (int)(Projectile.Center.Y / 16f + explosionRadius);

                // Ensure that all tile coordinates are within the world bounds
                Utils.ClampWithinWorld(ref minTileX, ref minTileY, ref maxTileX, ref maxTileY);

                // These 2 methods handle actually mining the tiles and walls while honoring tile explosion conditions
                bool explodeWalls = Projectile.ShouldWallExplode(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY);
                Projectile.ExplodeTiles(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY, explodeWalls);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ExplosiveMordorEnchantmentExplosion>(),
              Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            return true;
        }


    }

    internal class ExplosiveMordorEnchantmentExplosion : BaseExplosionProjectile
    {
        int trailMode;
        int rStart = 4;
        int rEnd = 128;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 24;
            rStart = Main.rand.Next(2, 4);
            rEnd = Main.rand.Next(96, 128);
        }

        protected override float BeamWidthFunction(float p)
        {
            //How wide the trail is going to be
            float trailWidth = MathHelper.Lerp(64, 8, p);
            float fadeWidth = MathHelper.Lerp(0, trailWidth, Easing.SpikeOutCirc(p)) * Main.rand.NextFloat(0.75f, 1.0f);
            return fadeWidth;
        }

        protected override Color ColorFunction(float p)
        {
            //Main color of the beam
            Color c;
            switch (trailMode)
            {
                default:
                case 0:
                    c = Color.Lerp(Color.White, new Color(147, 252, 121) * 0.5f, p);
                    break;
                case 1:
                    c = Color.Lerp(Color.White, new Color(147, 252, 121) * 0f, p);
                    break;
                case 2:
                    c = Color.White;
                    c.A = 0;
                    break;
            }

            return c;
        }

        protected override float RadiusFunction(float p)
        {
            //How large the circle is going to be
            return MathHelper.Lerp(rStart, 164, Easing.OutCirc(p));
        }

        protected override BaseShader ReadyShader()
        {
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.GlowTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail;
            shader.PrimaryColor = Color.DarkGoldenrod;
            shader.SecondaryColor = Color.Purple;
            shader.Speed = 20;

            //Alpha Blend/Additive
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.FillShape = true;
            return shader;
        }

        private void DrawMainShader()
        {
            //Trail
            // trailMode = 0;
            trailMode = 0;
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
            shader.NoiseColor = new Color(206, 101, 0);
            shader.OutlineColor = Color.Black;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes

            //Main Fill
            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);

            //Secondary fill
            trailMode = 0;
            shader.PrimaryColor = new Color(206, 101, 0);
            shader.NoiseColor = Color.Red;
            shader.OutlineColor = Color.Black;
            shader.Speed = 2.2f;
            shader.Distortion = 0.3f;
            shader.Power = 1.5f;
            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);


            //This just applis the shader changes
            //Main Fill

        }



        public override bool PreDraw(ref Color lightColor)
        {
            DrawMainShader();
            // DrawOutlineShader();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            //  target.AddBuff(BuffID.OnFire, 90);
        }
    }
}
