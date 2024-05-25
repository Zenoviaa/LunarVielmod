using Stellamod.Assets.Biomes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Skies
{
    public class GovheilSkyPlayer : ModPlayer
    {
        public bool VisualsActive;

        public bool MechanicsActive;

        public override void ResetEffects()
        {
            VisualsActive = false;
            MechanicsActive = false;
        }

        public override void UpdateDead()
        {
            VisualsActive = false;
            MechanicsActive = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            // If underwater and not in the last zone of the abyss.
            if (Player.InModBiome<FableBiome>())
                Lighting.AddLight((int)(Player.Center.X / 3f), (int)(Player.Center.Y / 3f), TorchID.Yellow, 10f);
          
        }
    }
}
