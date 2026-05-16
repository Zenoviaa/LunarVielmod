using Stellamod.Assets;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class AgreviPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Percent increase, 1 is +100% damage
            DamageModifier = 5;
            ExplosionType = ModContent.ProjectileType<AgreviBoom>();

            SoundStyle explosionSoundStyle = AssetManager.GetSound("Fire/FireExplosion1");
            explosionSoundStyle.PitchVariance = 0.3f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 8;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }

    public class AgreviBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 15;
        public override void SetDefaults()
        {
            base.SetDefaults();
            DrawScale = 0.5f;
            Projectile.width = 132;
            Projectile.height = 132;
        }

        public override void Start()
        {
            base.Start();
            float numDust = 8;
            for(float n = 0; n < numDust; n++)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Yellow,
                    outerColor = Color.Red
                };
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16) * 1.2f;
                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }
            FXUtil.GlowCircleDetailedBoom1(Projectile.Center, Color.Yellow, Color.OrangeRed, Color.Red);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 120);
            }
        }
    }
}