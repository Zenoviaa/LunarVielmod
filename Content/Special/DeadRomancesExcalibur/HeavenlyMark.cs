using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class HeavenlyMark : ModBuff
{
    public Asset<Texture2D> SigilTextureAsset;
    public override void Load()
    {
        base.Load();
        SigilTextureAsset = ModContent.Request<Texture2D>(Texture + "_Sigil");
    }
    public override void Unload()
    {
        base.Unload();
        SigilTextureAsset = null;
    }
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
            Vector2 spawnPos = npc.RandomPositionInNPCRect();
            var sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.5f);
            sp.noTileCollide = true;
            sp.outerColor = Color.Goldenrod;
        }
    }
}

