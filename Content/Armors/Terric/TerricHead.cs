using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.GunSystem;
using Stellamod.Core.Bases;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Terric;


public class TerricGunHeal : ModSystem
{
    public override void OnModLoad()
    {
        base.OnModLoad();
        GunHoldPlayer.OnReload += HealOnReload;
    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        GunHoldPlayer.OnReload -= HealOnReload;
    }

    private void HealOnReload(Player player, BaseGun gun)
    {
        if (!player.GetModPlayer<TerricPlayer>().hasTerricSetBonus)
            return;
        player.Heal(3);
        for (float f = 0; f < 8; f++)
        {
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.Red,
                outerColor = Color.DarkRed,
                gravity = 0
            };
            Vector2 pos = player.position;
            pos.X += Main.rand.Next(0, player.width);
            pos.Y += Main.rand.Next(0, player.height);
            DustParticle dp = DustParticle.Spawn(pos, -Vector2.UnitY * Main.rand.NextFloat(2f, 7f), spawnParams);
            dp.dampening = 0.05f;
        }

        SoundStyle healSound = new SoundStyle($"Stellamod/Assets/Sounds/Suckler");
        healSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(healSound, player.position);
    }
}

public class TerricGlobalItem : GlobalItem
{
    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(item, player, ref position, ref velocity, ref type, ref damage, ref knockback);
        if (type == ProjectileID.WoodenArrowFriendly && player.GetModPlayer<TerricPlayer>().hasTerricSetBonus)
            type = ModContent.ProjectileType<TerricBloodArrow>();
    }
}

public class TerricBloodArrow : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            Projectile.velocity *= 0.05f;
        }
        if (Timer % 4 == 0)
        {
            var blood = SmokeParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.3f);
            blood.initialColor = Color.DarkRed;
        }
        if (Main.rand.NextBool(4))
        {
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.Red,
                outerColor = Color.DarkRed
            };
            DustParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
        }

        Projectile.velocity.Y += 0.05f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Owner.Heal(1);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class TerricPlayer : ModPlayer
{
    public bool hasTerricSetBonus;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasTerricSetBonus = false;
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (hasTerricSetBonus)
        {
            CrossbowPlayer crossbowPlayer = Player.GetModPlayer<CrossbowPlayer>();
            crossbowPlayer.magicCircleColor = Color.Red;
            crossbowPlayer.magicCircleTextureAsset = AssetManager.GlowMask.MagicBloodCircle;
        }
    }
}

[AutoloadEquip(EquipType.Head)]
public class TerricHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        ArmorSetSystem.RegisterArmorSet<TerricHead, TerricBody, TerricLegs>(ArmorGroup.Act_I);
    }

    public override void UpdateEquip(Player player)
    {
        ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
        stats.rangedGunAmmoAmountPct += 0.25f;
        stats.defenseBonus += 4;
        stats.accessorySlots += 1;
    }

    public override void ArmorSetShadows(Player player)
    {
        player.armorEffectDrawShadow = true;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<TerricBody>() && legs.type == ModContent.ItemType<TerricLegs>();
    }

    public override void UpdateArmorSet(Player player)
    {
        TerricPlayer terricPlayer = player.GetModPlayer<TerricPlayer>();
        terricPlayer.hasTerricSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class TerricBody : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void UpdateEquip(Player player)
    {
        ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
        stats.defenseBonus += 5;
        stats.rangedDamage += 0.15f;
        stats.accessorySlots += 2;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class TerricLegs : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void UpdateEquip(Player player)
    {
        ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
        stats.defenseBonus += 3;
        stats.rangedPiercing += 1;
        stats.accessorySlots += 1;
    }
}