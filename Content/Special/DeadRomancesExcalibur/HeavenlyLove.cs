using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class HeavenlyLove : ModBuff
{
    public Asset<Texture2D> SigilTextureAsset;
    public override void Load()
    {
        base.Load();
        SigilTextureAsset = ModContent.Request<Texture2D>(Texture + "_Halo");
    }
    public override void Unload()
    {
        base.Unload();
        SigilTextureAsset = null;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
    }
}