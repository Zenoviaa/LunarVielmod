using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS
{
    public class RuneOfDetonationBomb : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                Vector2 explosionPosition = Projectile.Center;
                explosionPosition += Main.rand.NextVector2Circular(32, 32);

                for (float f = 0; f < 8; f++)
                {
                    float rot = f / 8f;
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = (rot * MathHelper.TwoPi).ToRotationVector2();
                    velocity *= Main.rand.NextFloat(4, 8);
                    LegacyParticle.NewParticle<ImpactParticle>(Projectile.Center, velocity);
                }

                for (float f = 0; f < 8; f++)
                {
                    float rot = f / 8f;
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = (rot * MathHelper.TwoPi).ToRotationVector2();
                    velocity *= Main.rand.NextFloat(4, 8);
                    LegacyParticle.NewParticle<EmberParticle>(Projectile.Center, velocity);
                }
                FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.Orange, Color.Red);

                //Effects
                SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion");
                explosionSoundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(explosionSoundStyle, Projectile.position);

                FXUtil.ShakeCamera(Projectile.position, 1024, 16);
            }
        }
    }

    public class RuneOfDetonationPlayer : ModPlayer
    {
        public bool hasDetonationRune;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasDetonationRune = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!hasDetonationRune)
                return;
            if (target.HasBuff(BuffID.OnFire))
                return;
            target.AddBuff(BuffID.OnFire, 120);
            if (target.life + damageDone >= target.lifeMax)
            {
                int damage = (int)(target.lifeMax * 0.3f);
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<RuneOfDetonationBomb>(), damage, hit.Knockback, Player.whoAmI);
            }

        }
    }

    public class RuneOfDetonation : BaseRune
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RuneOfDetonationPlayer detonationPlayer = player.GetModPlayer<RuneOfDetonationPlayer>();
            detonationPlayer.hasDetonationRune = true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }
}