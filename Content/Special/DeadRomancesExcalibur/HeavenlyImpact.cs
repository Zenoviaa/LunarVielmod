using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class HeavenlyImpact : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.IsATagBuff[Type] = true;
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.lifeRegen -= 2;
        if (Main.rand.NextBool(12))
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawnPos = npc.RandomPositionInNPCRect();
                var sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.5f);
                sp.noTileCollide = true;
                sp.outerColor = Color.Goldenrod;
                sp.Velocity += Vector2.UnitY * -3;
            }
        }
        if (Main.rand.NextBool(24))
        {
            Vector2 spawnPos = npc.RandomPositionInNPCRect();
            var sp = SirestiasSmokeParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.5f);
            sp.noTileCollide = true;
            sp.Scale *= 0.5f;
            sp.Velocity += Vector2.UnitY * -3;
        }
    }
}


public class HeavenlyImpactGlobalNPC : GlobalNPC
{
    public override bool PreAI(NPC npc)
    {
        if (npc.HasBuff<HeavenlyImpact>())
        {
            HeavenlyImpactGlobalProjectile.spawnWeakProjectiles = true;
        }
        return base.PreAI(npc);
    }
    public override void PostAI(NPC npc)
    {
        base.PostAI(npc);
        HeavenlyImpactGlobalProjectile.spawnWeakProjectiles = false;
    }
}

public class HeavenlyImpactGlobalProjectile : GlobalProjectile
{
    public bool heavenlyWeak;
    public static bool spawnWeakProjectiles;
    public override bool InstancePerEntity => true;
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        base.OnSpawn(projectile, source);
        if (spawnWeakProjectiles)
        {
            heavenlyWeak = true;
        }
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        base.SendExtraAI(projectile, bitWriter, binaryWriter);
        binaryWriter.Write(heavenlyWeak);
    }
    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        base.ReceiveExtraAI(projectile, bitReader, binaryReader);
        heavenlyWeak = binaryReader.ReadBoolean();
    }
    public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
    {
        base.ModifyHitPlayer(projectile, target, ref modifiers);
        if (heavenlyWeak)
        {
            modifiers.FinalDamage *= 0.5f;
        }
    }
}

