using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Movements;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Uvilis
{
    public class BubbletagEnchantment : BaseEnchantment
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 45;
        }

        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;

            //If greater than time then start homing, we'll just swap the movement type of the projectile
            if (Countertimer == time)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Particle.NewParticle<WaterSparkleParticle>(spawnPoint, velocity, Color.White);
                }

                if(Main.myPlayer == Projectile.owner)
                {
                    for(float f = 0; f < 3; f++)
                    {
                        float interpolant = f / 3f;
                        float rot = interpolant * MathHelper.TwoPi;
                        Vector2 vel = rot.ToRotationVector2() * 16;
                        Vector2 pos = Projectile.Center + vel;
                        float damage = Projectile.damage * 0.5f;
                        int bubbleDamage = (int)damage;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, vel, 
                            ModContent.ProjectileType<BubbletagBubble>(), bubbleDamage, 0, Projectile.owner);
                    }
                }
            }
        }

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
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.UvilisLightBlue);
        }
    }

    public class BubbletagBubble : ModProjectile
    {
        private Vector2 _scale;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Scale => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                Scale = Main.rand.NextFloat(0.5f, 1f);
                Projectile.netUpdate = true;
            }

            if (Main.rand.NextBool(60))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBlock, Scale: Main.rand.NextFloat(0.2f, 0.5f));
            }

            float interpolant = Timer / 30f;
            float eased = EasingFunction.OutExpo(interpolant);
            _scale = Vector2.Lerp(Vector2.Zero, Vector2.One, eased);
            _scale *= ExtraMath.Osc(0.9f, 1f, speed: 3, offset: Projectile.whoAmI);
            float maxHomingDetectDistance = 512;
            NPC npcToChase = ProjectileHelper.FindNearestEnemy(Projectile.Center, maxHomingDetectDistance);
            if (npcToChase != null)
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, npcToChase.Center, degreesToRotate: 10);
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundStyle popSound = SoundID.Item54;
            popSound.PitchVariance = 0.15f;
            SoundEngine.PlaySound(popSound, Projectile.position);
            for(float f = 0; f < Main.rand.NextFloat(2, 5); f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.Aqua, Scale: Main.rand.NextFloat(0.2f, 0.5f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            this.DrawCentered(ref lightColor, _scale);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = -Projectile.velocity;
            return false;
        }
    }
}
