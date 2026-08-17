using ReLogic.Content;
using Stellamod.Core.NPCHelpers;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Assets.ContentReader.Aseprite;

public static class AsepriteAssets
{
    public static Asset<AseSprite>[] Npc;
}


/// <summary>
/// Fills the Aseprite asset arrays
/// </summary>
internal class AsepriteAssetLoader : ModSystem
{
    public override void PostSetupContent()
    {
        base.PostSetupContent();
        AsepriteAssets.Npc = new Asset<AseSprite>[NPCSets.UseAseprite.Length];
        for (int i = 0; i < NPCSets.UseAseprite.Length; i++)
        {
            if (NPCSets.UseAseprite[i])
            {
                ModNPC modNpc = ModContent.GetModNPC(i);
                string texture = $"{modNpc.GetType().Namespace}.{modNpc.Name}".Replace('.', '/');
                AsepriteAssets.Npc[i] = ModContent.Request<AseSprite>(texture);
            }
        }
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        for (int i = 0; i < AsepriteAssets.Npc.Length; i++)
        {
            AsepriteAssets.Npc[i]?.Dispose();
        }
        AsepriteAssets.Npc = null;
    }
}
