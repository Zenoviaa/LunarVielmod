using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class Niivi
    {
        private void AI_Phase3_Reset()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 5);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
            ResetShaders();
            ResetState(BossActionState.Swoop_Out);
            NextAttack = BossActionState.Laser_Blast_V2;
        }
    }
}
