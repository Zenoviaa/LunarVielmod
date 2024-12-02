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

namespace Stellamod.Items.MoonlightMagic.Enchantments.Hex
{
    internal class HexCloudEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<HexElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Color.LightPink);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Spawn the explosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HexCloudEnchantmentExplosion>(),
              Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            //Spawn the explosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<HexCloudEnchantmentExplosion>(),
                Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }
    }

    internal class HexCloudEnchantmentExplosion : BaseExplosionProjectile
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
                    c = Color.Lerp(Color.White, new Color(147, 72, 121) * 0.5f, p);
                    break;
                case 1:
                    c = Color.Lerp(Color.White, new Color(147, 72, 121) * 0f, p);
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
            return MathHelper.Lerp(rStart, 100, Easing.OutCirc(p));
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
            trailMode = 0;
            var shader = MagicHexShader.Instance;

            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.PrimaryColor = new Color(195, 158, 255);
            shader.NoiseColor = new Color(78, 76, 180);//new Color(78, 76, 180);
            shader.OutlineColor = Color.White;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.1f;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
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
