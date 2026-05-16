using ReLogic.Content;
using Stellamod.Buffs;
using Stellamod.Common.ArmorRework;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Alcalite;

public class AlcalitePlayer : ModPlayer
{
    private Asset<Texture2D> _cocoonTextureAsset;
    private float _starTimer;
    public bool hasSetBonus;
    public bool hasRevived;
    public float rebirthTimer;
    public float alphaTimer;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSetBonus = false;
    }

    public override void UpdateDead()
    {
        base.UpdateDead();
        hasRevived = false;
    }

    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if (rebirthTimer > 0)
        {
            Player.velocity *= 0.02f;
        }
    }

    public override void UpdateLifeRegen()
    {
        base.UpdateLifeRegen();
        if (rebirthTimer > 0)
        {
            Player.lifeRegen += 32;
            alphaTimer += 0.05f;
            rebirthTimer--;
        }
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (rebirthTimer > 0)
        {
            alphaTimer += 0.05f;
            rebirthTimer--;
        }
        else
        {
            alphaTimer -= 0.05f;
            if (alphaTimer <= 0)
                alphaTimer = 0;
        }

        alphaTimer = MathHelper.Clamp(alphaTimer, 0f, 1f);


        if (Main.myPlayer != Player.whoAmI)
            return;
        if (!hasRevived)
            return;
        Player.GetDamage(DamageClass.Generic) += 0.05f;
        Player.AddBuff(ModContent.BuffType<IridineNecklaceCDBuff>(), 2);
        //Make the thing
        if (Player.ownedProjectileCounts[ModContent.ProjectileType<IlluriaStarGlow>()] == 0)
        {
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<IlluriaStarGlow>(), 0, 0, Player.whoAmI);
        }

        _starTimer--;
        if (_starTimer <= 0)
        {
            int damage = 90;
            int knockback = 1;

            for (int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                            ModContent.ProjectileType<IlluriaStarProjBlue>(), damage, knockback, Player.whoAmI);
                        break;
                    case 1:
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                           ModContent.ProjectileType<IlluriaStarProjCyan>(), damage, knockback, Player.whoAmI);
                        break;
                    case 2:
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                           ModContent.ProjectileType<IlluriaStarProjYellow>(), damage, knockback, Player.whoAmI);
                        break;
                }
            }

            _starTimer = 75;
        }
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        Player player = Player;
        if (player.whoAmI == Main.myPlayer
            && player.statLife <= 0 && !player.HasBuff<IridineNecklaceCDBuff>() && hasSetBonus && !hasRevived)
        {
            Revive(player);
            return false;
        }
        return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
    }

    private void Revive(Player player)
    {
        hasRevived = true;
        rebirthTimer = 7 * 60;
        player.SetImmuneTimeForAllTypes(7 * 60);
        SoundEngine.PlaySound(SoundRegistry.IridineRevive, player.position);
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (drawInfo.shadow != 0f)
            return;

        int maxNumBlades = 6;
        SpriteBatch sb = Main.spriteBatch;
        if (alphaTimer <= 0)
        {
            return;
        }

        _cocoonTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "Cocoon");
        for (int i = 0; i < maxNumBlades; i++)
        {
            float ratio = i / (float)maxNumBlades;
            float radians = ratio * MathHelper.TwoPi;
            radians += Main.GlobalTimeWrappedHourly * 0.5f;
            Vector2 drawCenter = drawInfo.drawPlayer.Center;
            Texture2D texture = _cocoonTextureAsset.Value;
            SpritebatchDrawer swordDrawer = SpritebatchDrawer.FromTextureAsset(texture, drawCenter);
            swordDrawer.color = Color.White * alphaTimer * 0.6f * ExtraMath.Osc(0f, 1f, speed: 2);
            sb.Draw(swordDrawer);
        }
    }
}

[AutoloadEquip(EquipType.Head)]
public class AlcaliteMask : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        ArmorSetSystem.RegisterArmorSet<AlcaliteMask, AlcaliteRobe, AlcaliteTrunks>(ArmorGroup.Act_III);
    }

    public override void SetDefaults()
    {
        Item.width = 40; // Width of the item
        Item.height = 34; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Lime;// The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.healthBonus += 100;
        stats.defenseBonus += 18;
        stats.accessorySlots++;
    }

    public override void UpdateArmorSet(Player player)
    {
        player.maxMinions += 1;
        player.GetModPlayer<AlcalitePlayer>().hasSetBonus = true;

    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<AlcaliteRobe>()
            && legs.type == ModContent.ItemType<AlcaliteTrunks>();
    }
}

[AutoloadEquip(EquipType.Body)]
public class AlcaliteRobe : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 34; // Width of the item
        Item.height = 26; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Lime;// The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.bossEndurance += 0.18f;
        stats.defenseBonus += 25;
        stats.accessorySlots++;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class AlcaliteTrunks : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 26; // Width of the item
        Item.height = 12; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.Lime;// The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.defenseBonus += 10;
        stats.accessorySlots++;
    }
}
