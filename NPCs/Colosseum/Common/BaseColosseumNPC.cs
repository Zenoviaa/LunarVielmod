using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Common
{
    public abstract class BaseColosseumNPC : ModNPC
    {
        public override bool CheckActive()
        {
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            return !colosseumSystem.IsActive();
        }

        public override void OnKill()
        {
            base.OnKill();
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            colosseumSystem.Progress();
        }
    }
}
