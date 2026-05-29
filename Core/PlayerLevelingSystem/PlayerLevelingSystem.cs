using Stellamod.Common.ArmorRework;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.PlayerLevelingSystem;


public class LevelingPlayer : ModPlayer
{
    public float[] stats = new float[7];
    public ref float Strength => ref stats[0];
    public ref float Endurance => ref stats[1];
    public ref float Agility => ref stats[2];
    public ref float Dexterity => ref stats[3];
    public ref float Focus => ref stats[4];
    public ref float Resourcefulness => ref stats[5];
    public ref float Veil => ref stats[6];

    public float AppliedPoints
    {
        get
        {
            float totalApplied = 0;
            for (int i = 0; i < stats.Length; i++)
            {
                totalApplied += stats[i];
            }
            return totalApplied;
        }
    }

    public float RemainingPoints
    {
        get
        {
            int numBossesDefeated = DownedBossTracker.DownedBossCount;
            return numBossesDefeated - (int)AppliedPoints;
        }
    }

 
    public bool CanApplyPoints(float proposedPoints)
    {
        float diff = RemainingPoints - proposedPoints;
        if (diff <= 0)
            return false;
        return true;
    }

    public bool CanApplyPoints() => RemainingPoints > 0;
    public override void Load()
    {
        base.Load();
        if (stats == null || stats.Length < 7)
            stats = new float[7];
    }

    public void ResetStats()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = 0;
        }

    }

    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        var stats = Player.GetStats();
        //Strength
        Player.GetDamage(DamageClass.Generic) += 0.01f * Strength;


        //Endurance
        stats.generalEndurance += 0.01f * Endurance;

        //Agility
        //We apply this in post update buffs because that happens before armorstats player applies affects
        Player.moveSpeed += 0.01f * Agility;
        Player.runAcceleration += 0.01f * Agility;


        //Dexteriyt
        Player.GetAttackSpeed(DamageClass.Generic) += Dexterity * 0.01f;

        //Focus attributes
        stats.criticalStrikeDamage += 0.05f * Focus;
        stats.criticalStrikeChance += 0.005f * Focus;

        //Veil
        //The debuff reduction time is in a separate class, since we have to override the hook for applying debuff tiem
        Player.statDefense += (int)Veil;
        Player.luck += 0.05f * Veil;
    }

    public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
    {
        base.ModifyWeaponDamage(item, ref damage);
        //TOOD: Apply Elemental Damage Bonus Here
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();


    }

    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);
        tag["stats"] = stats;

    }

    public override void LoadData(TagCompound tag)
    {
        base.LoadData(tag);
        stats = tag.Get<float[]>("stats");
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        base.CopyClientState(targetCopy);
        LevelingPlayer clone = targetCopy as LevelingPlayer;
        clone.stats = stats;
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        base.SyncPlayer(toWho, fromWho, newPlayer);
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.LevelingPlayerSync);
        packet.Write((byte)Player.whoAmI);
        packet.Write(stats.Length);
        for (int i = 0; i < stats.Length; i++)
        {
            packet.Write(stats[i]);
        }
        packet.Send(toWho, fromWho);
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        base.SendClientChanges(clientPlayer);
        LevelingPlayer clone = clientPlayer as LevelingPlayer;
        bool shouldSync = false;
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i] != clone.stats[i])
            {
                shouldSync = true;
                break;
            }
        }
        if (shouldSync)
        {
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (stats == null || stats.Length != length)
            stats = new float[length];
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = reader.ReadSingle();
        }
    }
}
