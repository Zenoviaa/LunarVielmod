using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
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
    public class CinderBomber : BaseJugglerItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToCombatTool(0.01f, 0.04f, 1);
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 5);
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CinderBomberProj>();
            Item.shootSpeed = 28;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankJuggler>(),
                material: ModContent.ItemType<Cinderscrap>());
        }
    }

    public class CinderBomberProj : BaseJugglerProjectile
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Juggler.combo >= 5)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                SoundStyle fireBomb = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
                fireBomb.PitchVariance = 0.3f;
                fireBomb.Volume = 0.5f;
                SoundEngine.PlaySound(fireBomb, target.Center);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<CinderBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                for (int i = 0; i < 16; i++)
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        innerColor = Color.OrangeRed,
                        outerColor = Color.Red,
                        scaleRange = new Vector2(0.3f, 1f)
                    };
                    DustParticle.Spawn(target.Center, Main.rand.NextVector2Circular(32, 32), spawnParams);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
                }

                var boom  = FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.OrangeRed,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.2f);
                boom.Scale *= 0.6f;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(target.Center, DustID.Torch, speed * 4, Scale: 1f);
                    d.noGravity = true;
                }
            }
        }
    }
}