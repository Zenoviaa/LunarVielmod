using MonoMod.Cil;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Areas.Collosseum.Event.Common;

public class ColosseumGlobalTile : GlobalTile
{
    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, type, spriteBatch, ref drawData);
        var tile = Main.tile[i, j];
        if (Main.tileSolid[tile.type])
            return;
        drawData.finalColor = Color.Lerp(Color.White, Color.Black, ColosseumWallColorEdit.Factor * 0.55f);
    }
}

public class ColosseumWallColorEdit : ModSystem
{


    public static float Factor { get; private set; }
    public static float WallTimer { get; private set; }
    public static bool darkenLights;

    public override void Load()
    {
        base.Load();
        IL_WallDrawing.DrawWalls += DarkenWallsDuringCollosseum;
    }


    static void DarkenWallsDuringCollosseum(ILContext il)
    {
        try
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                MoveType.After,
                // callvirt instance bool Terraria.SceneMetrics::get_HasHeartLantern()
                i => i.MatchCall<Lighting>($"{nameof(Lighting.GetColor)}")
            );
            c.GotoNext(
                MoveType.After,
                i => i.MatchStloc(29));
            c.EmitLdloc(29);
            c.EmitDelegate(() => Color.Black);
            c.EmitDelegate(() => Factor * 0.55f);
            c.EmitDelegate(Color.Lerp);
            c.EmitStloc(29);
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<Stellamod>(), il);
        }
    }

    public override void PostUpdateDusts()
    {

        base.PostUpdateDusts();
        if (NPC.AnyNPCs(ModContent.NPCType<ColosseumWaveManager>()))
        {
            WallTimer++;

        }
        else
        {
            WallTimer--;
        }
        float time = 60f;
        WallTimer = MathHelper.Clamp(WallTimer, 0f, time);
        Factor = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(WallTimer / time));
    }
}

public class ColosseumSystem : ModSystem
{
    public float spawnTimer;
    public bool completedBronzeColosseum;
    public bool completedSilverColosseum;
    public bool completedGoldColosseum;
    public bool completedTrueColosseum;
    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.Write(completedBronzeColosseum);
        writer.Write(completedSilverColosseum);
        writer.Write(completedGoldColosseum);
        writer.Write(completedTrueColosseum);
    }

    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        completedBronzeColosseum = reader.ReadBoolean();
        completedSilverColosseum = reader.ReadBoolean();
        completedGoldColosseum = reader.ReadBoolean();
        completedTrueColosseum = reader.ReadBoolean();
    }

    public override void SaveWorldData(TagCompound tag)
    {
        base.SaveWorldData(tag);
        tag["bronze"] = completedBronzeColosseum;
        tag["silver"] = completedSilverColosseum;
        tag["gold"] = completedGoldColosseum;
        tag["true"] = completedTrueColosseum;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        base.LoadWorldData(tag);
        completedBronzeColosseum = tag.GetBool("bronze");
        completedSilverColosseum = tag.GetBool("silver");
        completedGoldColosseum = tag.GetBool("gold");
        completedTrueColosseum = tag.GetBool("true");
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (!MultiplayerHelper.IsHost)
            return;

        if (NPC.AnyNPCs(ModContent.NPCType<ColosseumWaveManager>()))
        {
            spawnTimer = 0;
            return;
        }

        spawnTimer++;
        if (spawnTimer < 120)
        {
            return;
        }

        Vector2 GongSpawnWorld = ColosseumWaveManager.GongSpawnWorld;
        if (!completedBronzeColosseum)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<BronzeGong>()))
            {
                NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<BronzeGong>());
            }
        }
        else if (!completedSilverColosseum)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<SilverGong>()))
            {
                NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<SilverGong>());
            }
        }
        else if (!completedGoldColosseum)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<GoldGong>()))
            {
                NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<GoldGong>());
            }
        }
    }

    public void Reset()
    {
        completedBronzeColosseum = false;
        completedSilverColosseum = false;
        completedGoldColosseum = false;
        completedTrueColosseum = false;
    }
}
