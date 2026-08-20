using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public class LavaArenaCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "lavaarena";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        Point center = Main.LocalPlayer.Center.ToTileCoordinates();
        int width = 145;
        int height = 100;
        var bounds = TileUtilities.CenterTileBoundsTileSpace(Main.LocalPlayer.Center, width + 10, height + 10);

        //Not world gen idc if slow
        for (int x = bounds.topLeft.X; x < bounds.bottomRight.X; x++)
        {
            for (int y = bounds.topLeft.Y; y < bounds.bottomRight.Y; y++)
            {
                WorldGen.PlaceTile(x, y, ModContent.TileType<CindersparkDirt>(), mute: true, forced: true);
            }
        }

        //Not world gen idc if slow
        bounds = TileUtilities.CenterTileBoundsTileSpace(Main.LocalPlayer.Center, width, height);
        for(int x = bounds.topLeft.X; x < bounds.bottomRight.X; x++)
        {
            for(int y = bounds.topLeft.Y; y<  bounds.bottomRight.Y; y++)
            {
                WorldGen.KillTile(x, y, noItem: true);
            }
        }

        //Fill With Lava
        bounds = TileUtilities.CenterTileBoundsTileSpace(Main.LocalPlayer.Center, width, height / 2);

        for (int x = bounds.topLeft.X; x < bounds.bottomRight.X; x++)
        {
            for (int y = bounds.topLeft.Y; y < bounds.bottomRight.Y; y++)
            {
                WorldGen.PlaceLiquid(x, y + height / 2, (byte)LiquidID.Lava, 255);
            }
        }

        NPC.NewNPC(Main.LocalPlayer.GetSource_FromThis(), (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, ModContent.NPCType<BigMoltenPlatform>());

        Vector2 left = center.ToWorldCoordinates() + new Vector2(-width * 16, 0) * new Vector2(0.5f, 0f);
        Vector2 right = center.ToWorldCoordinates() + new Vector2(width * 16, 0) * new Vector2(0.5f, 0f);
        float numSmallerPlatforms = 4;
        void MakeSmallerPlatform(float p)
        {
            Vector2 pos = Vector2.Lerp(left, right, p);

            NPC.NewNPC(Main.LocalPlayer.GetSource_FromThis(), 
                (int)pos.X, 
                (int)pos.Y, 
                ModContent.NPCType<SmallMoltenPlatform>());

        }

        MakeSmallerPlatform(0.1f);
        MakeSmallerPlatform(0.2f);
        MakeSmallerPlatform(0.8f);
        MakeSmallerPlatform(0.9f);
        return true;
    }
}
