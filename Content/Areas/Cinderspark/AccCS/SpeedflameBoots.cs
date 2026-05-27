using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;

public class SpeedflameBoots : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
        Item.rare = ModContent.RarityType<CinderscrapRarity>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.maxRunSpeed *= 1.5f;
        player.runAcceleration *= 1.5f;
        if(player.velocity.X != 0 && !hideVisual && Main.rand.NextBool(8))
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            var dp = DustParticle.Spawn(player.Bottom - new Vector2(0, 2), -Vector2.UnitY, spawnParams);
            dp.Scale *= 0.5f;
        }
    }
}