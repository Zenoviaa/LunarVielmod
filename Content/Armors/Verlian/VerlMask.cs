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
        Projectile.extraUpdates = 1;
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

        if (Projectile.Top.Y < Main.player[Projectile.owner].Bottom.Y)
            Projectile.tileCollide = false;
        else
            Projectile.tileCollide = true;

        if (Timer % 12 == 0)
        {
            var ds = DustParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f);
            ds.outerColor = Color.BlueViolet;
            ds.Scale *= 0.5f;
            ds.gravity = 0;
        }

        Projectile.scale = MathHelper.Lerp(Projectile.scale, ScaleVariance, 0.1f);
        if(Projectile.velocity.Length() < 15)
            Projectile.velocity *= 1.1f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer <= 1)
            return false;
        SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        afDrawer.scale.Y *= 0.5f;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afDrawer.worldPosition = pos;
            afDrawer.color = Color.Lerp(Color.BlueViolet, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.3f;
            afDrawer.color.A = 0;
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Lerp(Color.White, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 16));
        drawer.color.A = 0;
        drawer.scale.Y *= 0.5f;
        Main.spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float numDust = 2;
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
        spawnPos += Main.rand.NextVector2Circular(80, 80);
        Vector2 vel = (target.Center - spawnPos).SafeNormalize(Vector2.Zero) * 8;
        Projectile.NewProjectile(proj.GetSource_FromThis(), spawnPos, vel,
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