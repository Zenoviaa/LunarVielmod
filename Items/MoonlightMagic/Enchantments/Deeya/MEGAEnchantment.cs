using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Deeya
{
    internal class MEGAEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 2f;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 240;
        }

        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;

            if (Countertimer == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<MEGAEnchantmentExplosion>(),
                Projectile.damage, Projectile.knockBack, ai1: Projectile.whoAmI);
            }
            //If greater than time then start homing, we'll just swap the movement type of the projectile
            if (Countertimer == time)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Particle.NewParticle<SparkleHexParticle>(spawnPoint, velocity, Color.White);

                }

            }
        }
        public override int GetElementType()
        {
            return ModContent.ItemType<DeeyaElement>();
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


    }

    internal class MEGAEnchantmentExplosion : BaseExplosionProjectile
    {
        int trailMode;
        int rStart = 4;
        int rEnd = 428;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 240;
            rStart = Main.rand.Next(4, 4);
            rEnd = Main.rand.Next(120, 120);
        }

        public override void AI()
        {
            base.AI();
            int projectileID = (int)Projectile.ai[1];
            Projectile projectile = Main.projectile[projectileID];
            var magicProj = projectile.ModProjectile as AdvancedMagicProjectile;

            if (!projectile.active || magicProj == null)
            {
                Projectile.Kill();
            }
            Projectile.velocity = projectile.velocity;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && !npc.boss)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance <= 400)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        npc.velocity -= direction * 0.5f;
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
            Projectile.ownerHitCheck = true;

            int radius = 250;

            // Damage enemies within the splash radius
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];
                if (target.active && !target.friendly && Vector2.Distance(Projectile.Center, target.Center) < radius)
                {
                    int damage = Projectile.damage * 2;
                    target.SimpleStrikeNPC(damage: 12, 0);
                }
            }

            for (int i = 0; i < 50; i++)
            {
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                Particle.NewParticle<SparkleHexParticle>(spawnPoint, speed, Color.White);
            }
        }

        protected override float BeamWidthFunction(float p)
        {

            //How wide the trail is going to be
            float trailWidth = MathHelper.Lerp(125, 13, p);
            float fadeWidth = MathHelper.Lerp(0, trailWidth, Easing.OutExpo(p)) * Main.rand.NextFloat(0.85f, 1.0f);

            int projectileID = (int)Projectile.ai[1];
            Projectile projectile = Main.projectile[projectileID];
            var magicProj = projectile.ModProjectile as AdvancedMagicProjectile;
            fadeWidth *= magicProj.ScaleMultiplier;
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
            int projectileID = (int)Projectile.ai[1];
            Projectile projectile = Main.projectile[projectileID];
            var magicProj = projectile.ModProjectile as AdvancedMagicProjectile;

            //How large the circle is going to be
            return MathHelper.Lerp(rStart, rEnd, Easing.InQuint(p) * magicProj.ScaleMultiplier);
        }

        protected override BaseShader ReadyShader()
        {
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.SimpleTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.SimpleTrail;

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
            trailMode = 0;
            var shader = MagicBloodletShader.Instance;
            shader.PrimaryTexture = TrailRegistry.BloodletTrail;
            shader.NoiseTexture = TrailRegistry.Clouds3;
            shader.PrimaryColor = Color.Black;
            shader.NoiseColor = Color.Black;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.5f;
            shader.Distortion = 2.5f;
            shader.Alpha = 0.25f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private void DrawOutlineShader()
        {
            trailMode = 1;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            Color pink = new Color(255, 59, 247);
            Color c = Color.White;
            c = pink;
            pink = Color.Lerp(pink, Color.Black, 0.5f);
            shader.PrimaryColor = Color.White;
            shader.NoiseColor = pink;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5f;
            shader.Distortion = 0.85f;
            shader.Power = 2.5f;

            TrailDrawer.Draw(Main.spriteBatch, _circlePos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            DrawMainShader();
            DrawOutlineShader();
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
