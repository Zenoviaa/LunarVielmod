using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Stellamod.Helpers;
namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc.Projectiles
{
    internal class HavocSignal : ModNPC
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            NPC.width = 1;
            NPC.height = 1;
            NPC.lifeMax = 1;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public static void NewSignal(Havoc.ActionState actionState)
        {
            if (StellaMultiplayer.IsHost)
            {
                var source = new EntitySource_Misc("signal");
                NPC.NewNPC(source, 0, 0, ModContent.NPCType<HavocSignal>(), ai0: (float)actionState);
            }
        }
    }
}
