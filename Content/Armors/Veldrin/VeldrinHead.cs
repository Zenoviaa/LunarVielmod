using Stellamod.Common.ArmorRework;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Veldrin;

public class MildCurseGlobalNPC : GlobalNPC
{
    public override bool PreAI(NPC npc)
    {
        if (npc.HasBuff<MildCurse>())
            return false;
        return base.PreAI(npc);
    }
}
public class MildCurse : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.velocity *= 0.8f;
        if (Main.rand.NextBool(3))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width),
                Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.Gold, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.Lerp(Color.DarkGoldenrod, Color.Violet, Main.rand.NextFloat(0f, 1f)) * 0.4f;
            sp.expand = true;
        }
        if (Main.rand.NextBool(3))
        {
            var ember = LegacyParticle.NewParticle<EmberParticle>(npc.position + new Vector2(Main.rand.Next(0, npc.width),
                Main.rand.Next(0, npc.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.Gold, Main.rand.NextFloat(0.9f, 1.5f));
            ember.innerColor = Color.LightPink;
            ember.outerColor = Color.Purple;
        }
    }
}

public class VeldrinPlayer : ModPlayer
{
    public bool hasSetBonus;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSetBonus = false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!hasSetBonus)
            return;
        if (hit.DamageType != DamageClass.Ranged)
            return;
        if (!Main.rand.NextBool(25))
            return;
        if (target.boss)
            return;

        for (float d = 0; d < 12; d++)
        {
            Vector2 vel = (d / 12f * MathHelper.TwoPi).ToRotationVector2();
            vel *= Main.rand.NextFloat(3f, 6f);
            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.DarkGoldenrod);
        }

        CombatText.NewText(target.getRect(), Color.LightGoldenrodYellow, LangText.Common("MildCurse"), dramatic: true);


        var fx = FXUtil.GlowStretch(target.Center, new Vector2(-1, 1));
        fx.VectorScale.X *= 3;
        FXUtil.ShakeCamera(target.Center, 1024, 8);

        SoundStyle inflictSound = new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1");
        inflictSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(inflictSound, target.position);
        target.AddBuff(ModContent.BuffType<MildCurse>(), 180);
    }
}
[AutoloadEquip(EquipType.Head)]
public class VeldrinHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<VeldrinHead, VeldrinBody, VeldrinLegs>();
    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 30;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.rangedStealthtiness += 100;
        stats.defenseBonus += 2;
        stats.accessorySlots++;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<VeldrinBody>() && legs.type == ModContent.ItemType<VeldrinLegs>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<VeldrinPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class VeldrinBody : ModItem
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
        Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
        Item.rare = ItemRarityID.Green; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.defenseBonus += 5;
        stats.rangedDamage += 0.2f;
        stats.stamina += 2;
        stats.accessorySlots += 2;
    }
}
[AutoloadEquip(EquipType.Legs)]
public class VeldrinLegs : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 22;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.meleeArmorPenetration += 5;
        stats.defenseBonus += 4;
        stats.inventorySlots += 5;
        stats.accessorySlots++;
    }
}