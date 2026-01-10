using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.ArmorsIL
{
    public class IllikenIceFury : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetDamage(DamageClass.Generic) += 0.1f;
            if (Main.rand.NextBool(3))
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY, Color.Red, Main.rand.NextFloat(0.9f, 1.5f));
                sp.initialColor = Color.Lerp(Color.White, Color.Cyan, Main.rand.NextFloat(0f, 1f)) * 0.4f;
                sp.expand = true;
            }

        }
    }
    public class IllikenIceBlast : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {

                for (float n = 0; n < 8; n++)
                {
                    Vector2 velocity = -Vector2.UnitY.RotatedByRandom(4f);
                    velocity *= Main.rand.NextFloat(2, 25f);
                    FlakeParticle fp = FlakeParticle.Spawn(Projectile.Center, velocity);
                    fp.gravity = 0f;
                    fp.Scale *= 0.6f;
                    fp.dampening = 0.1f;
                }

                for (float n = 0; n < 8; n++)
                {
                    SmokeParticle sp = Particle<SmokeParticle>.Spawn(Projectile.Center, -Vector2.UnitY.RotatedByRandom(4f) * Main.rand.NextFloat(0.5f, 15f), Color.White, Scale: Main.rand.NextFloat(0.15f, 1.5f));
                    sp.initialColor = Color.White * 0.4f;
                }

                SoundStyle explosionSound = AssetRegistry.Sounds.Illuria.IceImpact1;
                explosionSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(explosionSound, Projectile.position);
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.DarkBlue, baseSize: 0.16f);
            }
        }
    }
    public class IllikenPlayer : ModPlayer
    {
        public bool hasIllikenSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasIllikenSetBonus = false;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (!hasIllikenSetBonus)
                return;
            DashPlayer dashPlayer = Player.GetModPlayer<DashPlayer>();
            if (Main.myPlayer != Player.whoAmI)
                return;
            if (dashPlayer.DashedThisFrame)
            {
                Player.AddBuff(ModContent.BuffType<IllikenIceFury>(), 60);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<IllikenIceBlast>(), 50, 1, Player.whoAmI);
            }
        }

    }
    [AutoloadEquip(EquipType.Head)]
    public class IllikenHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<IllikenHelmet, IllikenCoat, IllikenLegs>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == Type && body.type == ModContent.ItemType<IllikenCoat>() && legs.type == ModContent.ItemType<IllikenLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.stamina += 2;
            armorStatsPlayer.accessorySlots += 2;
            armorStatsPlayer.inventorySlots += 5;
            armorStatsPlayer.defenseBonus += 5;
        }

        public override void UpdateArmorSet(Player player)
        {
            base.UpdateArmorSet(player);
            IllikenPlayer illikenPlayer = player.GetModPlayer<IllikenPlayer>();
            illikenPlayer.hasIllikenSetBonus = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class IllikenCoat : ModItem
    {

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.accessorySlots += 3;
            armorStatsPlayer.stamina += 2;
            armorStatsPlayer.inventorySlots += 10;
            armorStatsPlayer.defenseBonus += 10;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class IllikenLegs : ModItem
    {

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.accessorySlots += 1;
            armorStatsPlayer.inventorySlots += 5;
            armorStatsPlayer.defenseBonus += 8;
        }
    }
}
