using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.WeaponTypes.CombatTools;

#region Game Mechanics
public class CombatToolPlayer : ModPlayer
{
    private Item _selectedToolBackingField;
    public Item SelectedTool
    {
        get
        {
            if (_selectedToolBackingField == null)
            {
                _selectedToolBackingField = new Item();
                _selectedToolBackingField.SetDefaults(ItemID.None);
            }

            return _selectedToolBackingField;
        }
        set
        {
            _selectedToolBackingField = value;
        }
    }
    private List<Item> _unlockedToolsBackingField;
    public List<Item> UnlockedTools
    {
        get
        {
            if (_unlockedToolsBackingField == null)
            {
                _unlockedToolsBackingField = new List<Item>();
            }

            return _unlockedToolsBackingField;
        }
        set
        {
            _unlockedToolsBackingField = value;
        }
    }
    public float carryingCapacity = 1;
    public void Unlock(Item item)
    {
        UnlockedTools.Add(item);
    }

    public bool HasUnlocked(Item item)
    {
        return UnlockedTools.Find(x => x.type == item.type) != null;
    }

    public override bool PreItemCheck()
    {
        if (Main.myPlayer == Player.whoAmI)
        {
            if (LunarVeilKeybinds.ToolKeybind.JustReleased)
            {
                if (SelectedTool.TryGetGlobalItem<CombatTool>(out var combatTool))
                {
                    if (combatTool.isCombatTool)
                    {
                        if (combatTool.ammoCount > 0)
                        {
                            combatTool.ammoCount--;
                            ItemLoader.Shoot(SelectedTool, Player, new EntitySource_ItemUse_WithAmmo(Player, SelectedTool, -1),
                                Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero) * SelectedTool.shootSpeed, SelectedTool.shoot, Player.GetWeaponDamage(SelectedTool), Player.GetWeaponKnockback(SelectedTool));
                        }
                    }

                    carryingCapacity = combatTool.ammoCount / (float)combatTool.maxAmmoCount;
                }
                // CombatTool combatTool = SelectedTool.GetGlobalItem<CombatTool>();

            }
        }
        return base.PreItemCheck();
    }
    public override void UpdateDead()
    {
        base.UpdateDead();
        if (SelectedTool == null)
            return;

        if (SelectedTool.TryGetGlobalItem<CombatTool>(out CombatTool combatTool))
        {
            combatTool.ammoCount = combatTool.maxAmmoCount;
            carryingCapacity = 1;
        }
        ;

    }
    public override void PostItemCheck()
    {
        base.PostItemCheck();

    }
    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);
        tag["tool"] = SelectedTool;
        tag["unlocked"] = UnlockedTools;
    }
    public override void LoadData(TagCompound tag)
    {
        base.LoadData(tag);
        SelectedTool = tag.Get<Item>("tool");
        UnlockedTools = tag.Get<List<Item>>("unlocked");
    }
}
public class CombatToolProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public float bossDamagePercent;
    public float enemyDamagePercent;
    public override void SetDefaults(Projectile entity)
    {
        base.SetDefaults(entity);
        bossDamagePercent = 0;
        enemyDamagePercent = 0;
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        base.SendExtraAI(projectile, bitWriter, binaryWriter);
        binaryWriter.Write(bossDamagePercent);
        binaryWriter.Write(enemyDamagePercent);
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        base.ReceiveExtraAI(projectile, bitReader, binaryReader);
        bossDamagePercent = binaryReader.ReadSingle();
        enemyDamagePercent = binaryReader.ReadSingle();
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(projectile, target, ref modifiers);
        if (target.boss)
        {
            float bonusDamage = target.lifeMax * bossDamagePercent;
            modifiers.FlatBonusDamage += bonusDamage;
        }
        else
        {
            float bonusDamage = target.lifeMax * enemyDamagePercent;
            modifiers.FlatBonusDamage += bonusDamage;
        }
    }
}

public class CombatTool : GlobalItem
{
    public override bool InstancePerEntity => true;
    public bool isCombatTool;
    public float bossDamagePercent;
    public float enemyDamagePercent;
    public int ammoCount;
    public int maxAmmoCount;

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        base.ModifyTooltips(item, tooltips);
        if (isCombatTool)
        {
            TooltipLine line = new TooltipLine(Mod, "CombatToolWeaponType", LangText.Common("CombatTool"));
            line.OverrideColor = Color.LightGoldenrodYellow;
            tooltips.Add(line);

            line = new TooltipLine(Mod, "CarryingCapacity", LangText.Common("CombatToolCount", maxAmmoCount));
            line.OverrideColor = Color.White;

            tooltips.Add(line);

            if (enemyDamagePercent > 0)
            {
                string esp = string.Format("{0:P2}", enemyDamagePercent);
                line = new TooltipLine(Mod, "EnemyDamagePercent", LangText.Common("EnemyDamagePercent", esp));
                line.OverrideColor = Color.IndianRed;
                tooltips.Add(line);
            }

            if (bossDamagePercent > 0)
            {
                string bsp = string.Format("{0:P2}", bossDamagePercent);
                line = new TooltipLine(Mod, "BossDamagePercent", LangText.Common("BossDamagePercent", bsp));
                line.OverrideColor = Color.IndianRed;
                tooltips.Add(line);
            }

        }
    }
    public override bool OnPickup(Item item, Player player)
    {
        if (isCombatTool)
        {
            CombatToolPlayer toolPlayer = player.GetModPlayer<CombatToolPlayer>();
            toolPlayer.Unlock(item);
            PopupText.NewText(PopupTextContext.SonarAlert, item, 1, longText: true);
            return false;
        }
        else
        {
            return base.OnPickup(item, player);
        }

    }
    public override void UpdateInventory(Item item, Player player)
    {
        base.UpdateInventory(item, player);
        if (isCombatTool)
        {
            CombatToolPlayer toolPlayer = player.GetModPlayer<CombatToolPlayer>();
            toolPlayer.Unlock(item);
            //   PopupText.NewText(PopupTextContext.SonarAlert, item, 1, longText: true);
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item inv = player.inventory[i];
                if (item == inv)
                {
                    player.inventory[i] = new Item();
                    player.inventory[i].SetDefaults(ItemID.None);
                }
            }
        }
    }

    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (isCombatTool)
        {

            Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);

            CombatToolProjectile combatToolProjectile = p.GetGlobalProjectile<CombatToolProjectile>();
            combatToolProjectile.bossDamagePercent = bossDamagePercent;
            combatToolProjectile.enemyDamagePercent = enemyDamagePercent;
            return false;
        }
        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }
}

#endregion
