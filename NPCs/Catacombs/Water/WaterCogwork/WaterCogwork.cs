using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
    internal class WaterCogwork : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.lifeMax = 10000;
        }

        public override void AI()
        {         
            //OK so 
            //Cogwork will move around the arena kinda like a blazing wheel
            //He has contact damage obviously
            //Rotates around the arena and shoots projectiles
            //He'll make gear noises as he moves and have sparke particles coming out from where he touches the ground
            //The cogwork will roll around the arena and every once in a while stop and pull out a different gun to shoot you with
            //He sticks to walls like blazing wheels
            //Also has a ram attack where he revs up and goes around fast, you have to jump over em
            //So 4 attacks
        }
    }
}
