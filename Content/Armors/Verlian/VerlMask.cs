using Stellamod.Common.ArmorRework;
using Stellamod.Common.WeaponTypes;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Verlian;

public class VerlSword : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float ScaleVariance => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.light = 1.5f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle softSummon = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
            softSummon.PitchVariance = 0.3f;
            SoundEngine.PlaySound(softSummon, Projectile.position);
            ScaleVariance = Main.rand.NextFloat(0.8f, 1f);
            Projectile.scale = 0.001f;
        }

        if (Timer % 12 == 0)
        {
            var ds = DustParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f);
            ds.outerColor = Color.White;
            ds.Scale *= 0.75f;
        }

        Projectile.scale = MathHelper.Lerp(Projectile.scale, ScaleVariance, 0.1f);
        Projectile.velocity *= 1.01f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afDrawer.worldPosition = pos;
            afDrawer.color = Color.Lerp(Color.White, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.3f;
            afDrawer.color.A = 0;
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float numDust = 4;
        for (float n = 0; n < numDust; n++)
        {
            var ds = DustParticle.Spawn(Projectile.Center, Projectile.oldVelocity.RotatedByRandom(
                MathHelper.ToRadians(60) * Main.rand.NextFloat(0.2f, 0.6f)));
            ds.outerColor = Color.White;
        }
    }
}

public class VerlPlayer : ModPlayer
{
    public bool hasSetBonus;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSetBonus = false;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPCWithProj(proj, target, hit, damageDone);
        if (!hasSetBonus)
            return;

        if (proj.type == ModContent.ProjectileType<VerlSword>())
            return;

        Item heldItem = Player.HeldItem;
        if (!heldItem.TryGetGlobalItem<ManaSphereGlobalItem>(out var ms))
            return;

        if (!ms.isManaSphere)
            return;

        //Rain swords here
        Vector2 spawnPos = target.Center + new Vector2(0, -500);
        Projectile.NewProjectile(proj.GetSource_FromThis(), spawnPos, Vector2.UnitY,
            ModContent.ProjectileType<VerlSword>(), proj.damage, 1, Player.whoAmI);
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        base.OnHitByNPC(npc, hurtInfo);

    }
}

[AutoloadEquip(EquipType.Head)]
public class VerlMask : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        ArmorSetSystem.RegisterArmorSet<VerlMask, VerlBreastplate, VerlLeggings>();
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Orange; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.artifactManaReduction += 0.3f;
        stats.accessorySlots += 1;
        stats.defenseBonus += 4;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<VerlBreastplate>() && legs.type == ModContent.ItemType<VerlLeggings>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<VerlPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class VerlBreastplate : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Orange; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.magicDamage += 0.1f;
        stats.accessorySlots += 2;
        stats.defenseBonus += 5;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class VerlLeggings : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Orange; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.totalMana += 20;
        stats.accessorySlots += 1;
        stats.defenseBonus += 4;
    }
}