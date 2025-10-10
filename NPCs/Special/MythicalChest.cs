using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Special
{
    public class MythicalChest : BaseChest
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            ChestColor = new Color(210, 33, 221);
        }
        public override void AI()
        {
            base.AI();
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadowExplosion"), NPC.position);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            //Ok we gotta add uhh
            //EVERY ENCHANTMENT LOL

        }
    }
}
