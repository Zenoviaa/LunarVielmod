using Stellamod.Common.ArmorRework;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Jackler
{
    public class JacklerBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 24;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 16);
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.Red,
                    glowColor: Color.DarkRed,
                    outerGlowColor: Color.Black, duration: 25, baseSize: 0.28f);

                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                for (float f = 0; f < 4; f++)
                {
                    Particle<DustParticle>.Spawn(Projectile.Center,
                        Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                for (float f = 0; f < 6; f++)
                {
                    var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center,
                        -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                    smoke.initialColor = Color.DarkGray;
                }

                for (float f = 0; f < 8; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    var rose = Particle<RosePetalParticle>.SpawnInAlphaLayer(Projectile.Center, velocity, Scale: Main.rand.NextFloat(0.8f, 2.5f));
                }

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.Red,
                        glowColor: Color.DarkRed,
                        outerGlowColor: Color.Black,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }
    }
    public class JacklerGlobalProjectile : GlobalProjectile
    {
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            base.OnKill(projectile, timeLeft);
            if (!projectile.minion)
                return;

            Player owner = Main.player[projectile.owner];
            JacklerPlayer jacklerPlayer = owner.GetModPlayer<JacklerPlayer>();
            Console.WriteLine(jacklerPlayer.hasJacklerSetBonus);
            if (!jacklerPlayer.hasJacklerSetBonus)
                return;

            if (Main.myPlayer != projectile.owner)
                return;
            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<JacklerBoom>(), projectile.damage * 2, 1, projectile.owner);
        }
    }
    public class JacklerPlayer : ModPlayer
    {
        public bool hasJacklerSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasJacklerSetBonus = false;
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class JacklerHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<JacklerHat, JacklerCoat, JacklerPants>(ArmorGroup.Act_I);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.accessorySlots++;
            stats.summonCastTime += 0.25f;
            stats.defenseBonus += 2;

        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == Type && body.type == ModContent.ItemType<JacklerCoat>() && legs.type == ModContent.ItemType<JacklerPants>();
        }
        public override void UpdateArmorSet(Player player)
        {
            base.UpdateArmorSet(player);
            player.GetModPlayer<JacklerPlayer>().hasJacklerSetBonus = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class JacklerCoat : ModItem
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.accessorySlots++;
            stats.minionSlots++;
            stats.defenseBonus += 4;

        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class JacklerPants : ModItem
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.accessorySlots++;
            stats.minionSummonHealth += -0.20f;
            stats.defenseBonus += 3;
        }
    }
}
