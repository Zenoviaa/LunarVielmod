using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.ClassSelect;
using Stellamod.Items.Accessories.Players;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.ClassReworkSystem;

public class ClassReworkPlayer : ModPlayer
{
    public PlayerClass playerClass;
    public DamageClass damageClass;
    private Item _quiverAmmoItem;
    public Item QuiverAmmoItem
    {
        get
        {
            if (_quiverAmmoItem == null)
            {
                _quiverAmmoItem = new Item(0);
                _quiverAmmoItem.SetDefaults(0);
            }
            return _quiverAmmoItem;
        }
        set
        {
            _quiverAmmoItem = value;
        }
    }

    public int heldShield;
    public int healTimer;
    public bool hasSpawned;
    public bool defaultShield => heldShield == ModContent.ProjectileType<MeleeShield>();


    public bool HasAmmo(Item item)
    {
        //TODO:
        return true;
    }
    public override void ResetEffects()
    {
        base.ResetEffects();

        heldShield = ModContent.ProjectileType<MeleeShield>();
    }
    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        base.ModifyHitByNPC(npc, ref modifiers);
        if (playerClass != PlayerClass.Melee)
            return;

        //Take less from contact damage
        if (Main.npc.IndexInRange(modifiers.DamageSource.SourceNPCIndex))
        {
            modifiers.IncomingDamageMultiplier *= 0.9f;
        }
    }
    private void MeleeEffects()
    {
        /*
         * 
         *  Be up close and personal with your bosses and enemies, 
         *  taking 35% less contact damage from enemies,
         *  have a shield that you can face around you, 
         *  and block enemies from getting to you and keep them at bay.
         *  Use a variety of close range weapons including greatswords, swords and spears, etc. to destroy enemies. 
         *  +1 increased Stamina, and 30% increased dash distance and speed. Enemies would like to attack you more… (Warrior’s Grace)
         */


        DashPlayer dashPlayer = Player.GetModPlayer<DashPlayer>();
        dashPlayer.DashVelocity += 3;
        dashPlayer.DashDuration += 1;
        dashPlayer.MaxDashCount += 1;


        if (Player.ownedProjectileCounts[heldShield] == 0 && Main.myPlayer == Player.whoAmI)
        {
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                heldShield, 1, 1, Player.whoAmI);
        }
        Player.aggro += 300;
        damageClass = DamageClass.Melee;
    }

    private void RangerEffects()
    {
        /*
         Use bows, guns, powders and machines to shoot your enemies from afar, 
        although has less base health, ranger offers a large selection of items to stay back and have infinite fun. 
        Great at being stealthy. Includes an elemental quiver and musket pouch that has endless ammo 
        but you can add magical elements to change the effects and buffs of your ammo. 
        Enemies are less likely to target you. 
        (The quiver and Ammo thing appears at the top of your inventory) beside the flask (Iron Bow)
         */


        //Less base health
        Player.statLifeMax2 -= 50;

        //Less likely to target
        Player.aggro -= 300;
        damageClass = DamageClass.Ranged;
    }
    private void MageEffects()
    {
        /*
         * Use wands, tomes, artifacts, enchantments, and elements to create magical ways of beating your enemies, create your own magic spells through wands and stray out to do tremendous amounts of damage. 
         * +60 mana, Gilded Staff/Gilded Artifact
         */
        Player.statManaMax2 += 60;
        damageClass = DamageClass.Magic;
    }

    private void SummonerEffects()
    {
        /*
         * You get better dodging options, as well as using Runes, Chakrams and Orbs to empower your friends and attack your foes to keep them alive. 
         * +1 Summon, +1 Stamina +2 Insource Slots
         */
        Player.GetModPlayer<DashPlayer>().MaxDashCount += 1;
        FlaskPlayer flaskPlayer = Player.GetModPlayer<FlaskPlayer>();
        flaskPlayer.maxInsourceCount += 1;
        damageClass = DamageClass.Summon;
    }

    private void OmniEffects()
    {
        //Use anything, but you're much less proficient
        Player.GetDamage(DamageClass.Generic) -= 0.3f;
    }

    public override void PreUpdate()
    {
        base.PreUpdate();

    }
    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();

    }

    public override void OnRespawn()
    {
        base.OnRespawn();
        int healthToHeal = (int)MathF.Max(Player.statLifeMax2, 1000);
        Player.Heal(healthToHeal);
    }

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();

        if (!hasSpawned)
        {
            Item[] startingITems = ModContent.GetInstance<ClassSystem>().GetClassStartingItems((int)playerClass);
            Player.inventory[0] = startingITems[0].Clone();
            for (int i = 1; i < startingITems.Length; i++)
            {
                Item item = startingITems[i];
                Player.QuickSpawnItem(Player.GetSource_FromThis(), item.type);
            }

            hasSpawned = true;
        }
        if (Player.ConsumedLifeCrystals < 5)
        {
            Player.ConsumedLifeCrystals = 5;
            healTimer = 5;

        }

        if (playerClass == PlayerClass.Mage)
        {
            if (Player.ConsumedManaCrystals < 10)
                Player.ConsumedManaCrystals = 10;
        }

        if (healTimer > 0)
        {
            healTimer--;

            if (healTimer <= 0)
            {
                Player.statLife += 100;
            }
        }

        switch (playerClass)
        {
            case PlayerClass.Melee:
                MeleeEffects();
                break;
            case PlayerClass.Ranger:
                RangerEffects();
                break;
            case PlayerClass.Mage:
                MageEffects();
                break;
            case PlayerClass.Summoner:
                SummonerEffects();
                break;
            case PlayerClass.Omni:
                OmniEffects();
                return;
            case PlayerClass.God:
                return;
        }

        if (Player.HeldItem.DamageType != damageClass)
        {
            Player.GetDamage(Player.HeldItem.DamageType) *= 0.05f;
        }
    }

    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);
        tag["playerClass"] = (int)playerClass;
        tag["hasSpawned"] = hasSpawned;
        tag["quiverAmmoItem"] = QuiverAmmoItem;
    }
    public override void LoadData(TagCompound tag)
    {
        base.LoadData(tag);
        playerClass = (PlayerClass)tag.Get<int>("playerClass");
        hasSpawned = tag.Get<bool>("hasSpawned");
        QuiverAmmoItem = tag.Get<Item>("quiverAmmoItem");
    }
}
