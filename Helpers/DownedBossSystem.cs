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
		public static bool downedGintzlBoss = false;
		public static bool downedSyliaBoss = false;
		public static bool downedStoneGolemBoss = false;
		public static bool downedSTARBoss = false;
		public static bool downedFenixBoss = false;
		public static bool downedPandorasBox = false;
		public static bool downedBlazingSerpent = false;
		public static bool downedWaterJellyfish = false;
		public static bool downedSparn=false;
		public static bool downedCogwork = false;

        public override void ClearWorld()
        {
			downedVeriBoss = false;
			downedJackBoss = false;
			downedDaedusBoss = false;
			downedDreadBoss = false;
			downedSOMBoss = false;
			downedGothBoss = false;
			downedSunsBoss = false;
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
		}

		public override void LoadWorldData(TagCompound tag)
		{
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
		}

		public override void NetSend(BinaryWriter writer)
		{
			// Order of operations is important and has to match that of NetReceive
			var flags = new BitsByte();
			flags[0] = downedVeriBoss;
			flags[1] = downedGintzlBoss;
			flags[2] = downedDaedusBoss;
			flags[3] = downedDreadBoss;
			flags[4] = downedSOMBoss;
			flags[5] = downedGothBoss;
			flags[6] = downedSunsBoss;
			flags[7] = downedJackBoss;
			flags[8] = downedSyliaBoss;
			flags[9] = downedStoneGolemBoss;
			flags[10] = downedSTARBoss;
			flags[11] = downedFenixBoss;
			flags[12] = downedBlazingSerpent;
			flags[13] = downedCogwork;
			flags[14] = downedSparn;
			flags[15] = downedWaterJellyfish;
			flags[16] = downedPandorasBox;
			writer.Write(flags);		
		}

		public override void NetReceive(BinaryReader reader)
		{
			// Order of operations is important and has to match that of NetSend
			BitsByte flags = reader.ReadByte();
			flags[0] = downedVeriBoss;
			flags[1] = downedGintzlBoss;
			flags[2] = downedDaedusBoss;
			flags[3] = downedDreadBoss;
			flags[4] = downedSOMBoss;
			flags[5] = downedGothBoss;
			flags[6] = downedSunsBoss;
			flags[7] = downedJackBoss;
			flags[8] = downedSyliaBoss;
			flags[9] = downedStoneGolemBoss;
			flags[10] = downedSTARBoss;
			flags[11] = downedFenixBoss;
			flags[12] = downedBlazingSerpent;
			flags[13] = downedCogwork;
			flags[14] = downedSparn;
			flags[15] = downedWaterJellyfish;
			flags[16] = downedPandorasBox;
		}
	}
}