using Stellamod.NPCs.Town;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.NPCHelpers
{
    public class NPCSets : ModSystem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            int[] resistedNPCs = new int[]
{
                    NPCID.BurningSphere,
                    NPCID.LavaSlime,
                    NPCID.Hellbat,
                    NPCID.Demon,
                    NPCID.VoodooDemon,
                    NPCID.BlazingWheel,
                    NPCID.Lavabat,
                    NPCID.RedDevil,
                    NPCID.HellArmoredBones,
                    NPCID.HellArmoredBonesMace,
                    NPCID.HellArmoredBonesSpikeShield,
                    NPCID.HellArmoredBonesSword,
                    NPCID.SolarCrawltipedeBody,
                    NPCID.SolarCrawltipedeHead,
                    NPCID.SolarCrawltipedeTail,
                    NPCID.SolarDrakomire,
                    NPCID.SolarDrakomireRider,
                    NPCID.SolarSroller,
                    NPCID.SolarCorite,
                    NPCID.SolarSolenian,
                    NPCID.SolarFlare,
                    NPCID.SolarSpearman,
                    NPCID.SolarGoop,
                    NPCID.LunarTowerSolar,
                    NPCID.TorchGod
};
            for (int n = 0; n < resistedNPCs.Length; n++)
            {
                NPCSets.ResistedByFlamecrestShield[resistedNPCs[n]] = true;
            }


        }

        public override void ResizeArrays()
        {
            base.ResizeArrays();
            Heavy = NPCID.Sets.Factory.CreateBoolSet();
            ResistedByFlamecrestShield = NPCID.Sets.Factory.CreateBoolSet();
            CannotBeBubbled = NPCID.Sets.Factory.CreateBoolSet();
            UseAseprite = NPCID.Sets.Factory.CreateBoolSet(false);
        }
        public static bool[] Heavy;
        public static bool[] ResistedByFlamecrestShield;
        public static bool[] CannotBeBubbled;

        /// <summary>
        /// When set to true, attempts to load an aseprite asset for the associated NPC
        /// </summary>
        public static bool[] UseAseprite;
    }

}
