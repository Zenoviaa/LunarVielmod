using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Helpers
{
    // Acts as a container for "downed boss" flags.
    // Set a flag like this in your bosses OnKill hook:
    //    NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

    // Saving and loading these flags requires TagCompounds, a guide exists on the wiki: https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
    public class DownedBossSystem : ModSystem
	{
		public static bool downedVeriBoss = false;
		public static bool downedJackBoss = false;
		public static bool downedDaedusBoss = false;
		public static bool downedDreadBoss = false;
		public static bool downedSOMBoss = false;
		public static bool downedGothBoss = false;
		public static bool downedSunsBoss = false;
		public static bool downedZuiBoss = false;
		public static bool downedGintzlBoss = false;
		public static bool downedSyliaBoss = false;
		public static bool downedStoneGolemBoss = false;
		public static bool downedSTARBoss = false;
		public static bool downedFenixBoss = false;
		public static bool downedNESTBoss = false;
		public static bool downedPandorasBox = false;
		public static bool downedBlazingSerpent = false;
		public static bool downedWaterJellyfish = false;
		public static bool downedSparn=false;
		public static bool downedCogwork = false;
		public static bool downedAzurewrathBoss = false;

		private static void ResetFlags()
		{
            downedVeriBoss = false;
            downedJackBoss = false;
            downedDaedusBoss = false;
            downedDreadBoss = false;
            downedSOMBoss = false;
            downedGothBoss = false;
            downedSunsBoss = false;
			downedZuiBoss = false;
			downedGintzlBoss = false;
            downedSyliaBoss = false;
            downedStoneGolemBoss = false;
            downedSTARBoss = false;
            downedFenixBoss = false;
            downedPandorasBox = false;
            downedBlazingSerpent = false;
            downedWaterJellyfish = false;
            downedSparn = false;
            downedCogwork = false;
			downedNESTBoss = false;
			downedAzurewrathBoss = false;
		}

        public override void ClearWorld()
        {
			ResetFlags();
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.
        public override void SaveWorldData(TagCompound tag)
		{
			tag["downedVeriBoss"] = downedVeriBoss;
			tag["downedGintzlBoss"] = downedGintzlBoss;
			tag["downedSunsBoss"] = downedSunsBoss;
			tag["downedGothBoss"] = downedGothBoss;
			tag["downedSOMBoss"] = downedSOMBoss;
			tag["downedJackBoss"] = downedJackBoss;
			tag["downedDaedusBoss"] = downedDaedusBoss;
			tag["downedDreadBoss"] = downedDreadBoss;
			tag["downedSyliaBoss"] = downedSyliaBoss;
			tag["downedStoneGolemBoss"] = downedStoneGolemBoss;
			tag["downedSTARBoss"] = downedSTARBoss;
			tag["downedFenixBoss"] = downedFenixBoss;
			tag["downedBlazingSerpent"] = downedBlazingSerpent;
			tag["downedCogwork"] = downedBlazingSerpent;
			tag["downedPandorasBox"] = downedPandorasBox;
			tag["downedSparn"] = downedSparn;
			tag["downedWaterJellyfish"] = downedWaterJellyfish;
			tag["downedZuiBoss"] = downedZuiBoss;
			tag["downedNESTBoss"] = downedNESTBoss;
			tag["downedAzurBoss"] = downedAzurewrathBoss;
		}

		public override void LoadWorldData(TagCompound tag)
		{

			downedZuiBoss = tag.GetBool("downedZuiBoss");
			downedNESTBoss = tag.GetBool("downedNESTBoss");
			downedVeriBoss = tag.GetBool("downedVeriBoss");
			downedDreadBoss = tag.GetBool("downedDreadBoss");
			downedSOMBoss = tag.GetBool("downedSOMBoss");
			downedJackBoss = tag.GetBool("downedJackBoss");
			downedDaedusBoss = tag.GetBool("downedDaedusBoss");
			downedGothBoss = tag.GetBool("downedGothBoss");
			downedSunsBoss = tag.GetBool("downedSunsBoss");
			downedGintzlBoss = tag.GetBool("downedGintzlBoss");
			downedSyliaBoss = tag.GetBool("downedSyliaBoss");
			downedStoneGolemBoss = tag.GetBool("downedStoneGolemBoss");
			downedSTARBoss = tag.GetBool("downedSTARBoss");
			downedFenixBoss = tag.GetBool("downedFenixBoss");
			downedCogwork = tag.GetBool("downedCogwork");
			downedWaterJellyfish = tag.GetBool("downedWaterJellyfish");
			downedSparn = tag.GetBool("downedSparn");
			downedPandorasBox = tag.GetBool("downedPandorasBox");
			downedBlazingSerpent = tag.GetBool("downedBlazingSerpent");
			downedAzurewrathBoss = tag.GetBool("downedAzurBoss");
		}

		public override void NetSend(BinaryWriter writer)
		{
            // Order of operations is important and has to match that of NetReceive
            writer.Write(new BitsByte
            {
                [0] = downedVeriBoss,
                [1] = downedGintzlBoss,
                [2] = downedDaedusBoss,
                [3] = downedDreadBoss,
                [4] = downedSOMBoss,
                [5] = downedGothBoss,
                [6] = downedSunsBoss,
                [7] = downedJackBoss

            });

            writer.Write(new BitsByte
            {
                [0] = downedSyliaBoss,
                [1] = downedStoneGolemBoss,
                [2] = downedSTARBoss,
                [3] = downedFenixBoss,
                [4] = downedBlazingSerpent,
                [5] = downedCogwork,
                [6] = downedSparn,
                [7] = downedWaterJellyfish
            });

            writer.Write(new BitsByte
            {
                [0] = downedPandorasBox,
				[1] = downedZuiBoss,
				[2] = downedNESTBoss,
				[3] = downedAzurewrathBoss
			});	
		}

		public override void NetReceive(BinaryReader reader)
		{
            // Order of operations is important and has to match that of NetSend
            BitsByte flags = reader.ReadByte();
			downedVeriBoss = flags[0];
			downedGintzlBoss = flags[1];
			downedDaedusBoss = flags[2];
			downedDreadBoss = flags[3];
			downedSOMBoss = flags[4];
			downedGothBoss = flags[5];
			downedSunsBoss = flags[6];
			downedJackBoss = flags[7];

            flags = reader.ReadByte();
			downedSyliaBoss = flags[0];
			downedStoneGolemBoss = flags[1];
			downedSTARBoss = flags[2];
			downedFenixBoss = flags[3];
			downedBlazingSerpent = flags[4];
			downedCogwork = flags[5];
			downedSparn = flags[6];
			downedWaterJellyfish = flags[7];

			flags = reader.ReadByte();
			downedPandorasBox = flags[0];
			downedZuiBoss = flags[1];
			downedNESTBoss = flags[2];
			downedAzurewrathBoss = flags[3];
		}
	}
}