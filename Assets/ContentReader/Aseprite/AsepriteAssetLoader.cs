using ReLogic.Content;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.ProjectileHelpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Assets.ContentReader.Aseprite;

public static class AsepriteAssets
{
    public static Asset<AseSprite>[] Npc;
    public static Asset<AseSprite>[] Projectile;
}


/// <summary>
/// Fills the Aseprite asset arrays
/// </summary>
internal class AsepriteAssetLoader : ModSystem
{
    public override void PostSetupContent()
    {
        base.PostSetupContent();
        if (Main.netMode == NetmodeID.Server)
            return;

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

        AsepriteAssets.Projectile = new Asset<AseSprite>[ProjectileID.Sets.UsesAseprite.Length];
        for (int i = 0; i < AsepriteAssets.Projectile.Length; i++)
        {
            if (ProjectileID.Sets.UsesAseprite[i])
            {
                var modProj = ModContent.GetModProjectile(i);
                string texture = $"{modProj.GetType().Namespace}.{modProj.Name}".Replace('.', '/');
                AsepriteAssets.Projectile[i] = ModContent.Request<AseSprite>(texture);
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
        if (Main.netMode == NetmodeID.Server)
            return;
        if (AsepriteAssets.Npc != null)
        {
            for (int i = 0; i < AsepriteAssets.Npc.Length; i++)
            {
                AsepriteAssets.Npc[i]?.Dispose();
            }
            AsepriteAssets.Npc = null;
        }
        if (AsepriteAssets.Npc != null)
        {
            for (int i = 0; i < AsepriteAssets.Projectile.Length; i++)
            {
                AsepriteAssets.Projectile[i]?.Dispose();
            }
            AsepriteAssets.Projectile = null;
        }
    }
}
