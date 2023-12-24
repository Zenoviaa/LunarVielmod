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
		// public static bool downedOtherBoss = false;

		public override void OnWorldLoad()
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

		public override void OnWorldUnload()
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
			if (downedVeriBoss)
			{
				tag["downedVeriBoss"] = true;
			}

			if (downedGintzlBoss)
			{
				tag["downedGintzlBoss"] = true;
			}

			if (downedSunsBoss)
			{
				tag["downedSunsBoss"] = true;
			}

			if (downedGothBoss)
			{
				tag["downedGothBoss"] = true;
			}

			if (downedSOMBoss)
			{
				tag["downedSOMBoss"] = true;
			}

			if (downedJackBoss)
			{
				tag["downedJackBoss"] = true;
			}
			if (downedDaedusBoss)
			{
				tag["downedDaedusBoss"] = true;
			}
			if (downedDreadBoss)
			{
				tag["downedDreadBoss"] = true;
			}
			if (downedSyliaBoss)
			{
				tag["downedSyliaBoss"] = true;
			}
            if (downedStoneGolemBoss)
            {
				tag["downedStoneGolemBoss"] = true;
			}

			if (downedSTARBoss)
			{
				tag["downedSTARBoss"] = true;
			}

			if (downedFenixBoss)
			{
				tag["downedFenixBoss"] = true;
			}

			if (downedBlazingSerpent)
			{
				tag["downedBlazingSerpent"] = true;
			}

			if (downedCogwork)
			{
				tag["downedCogwork"] = true;
			}

			if (downedPandorasBox)
			{
				tag["downedPandorasBox"] = true;
			}

			if (downedSparn)
			{
				tag["downedSparn"] = true;
			}

			if (downedWaterJellyfish)
			{
				tag["dowendWaterJellyfish"] = true;
			}

			// if (downedOtherBoss) {
			//	tag["downedOtherBoss"] = true;
			// }
		}

		public override void LoadWorldData(TagCompound tag)
		{
			downedVeriBoss = tag.ContainsKey("downedVerliaBoss");
			downedDreadBoss = tag.ContainsKey("downedDreadBoss");
			downedSOMBoss = tag.ContainsKey("downedSOMBoss");
			downedJackBoss = tag.ContainsKey("downedJackBoss");
			downedDaedusBoss = tag.ContainsKey("downedDaedusBoss");
			downedGothBoss = tag.ContainsKey("downedGothBoss");
			downedSunsBoss = tag.ContainsKey("downedSunsBoss");
			downedGintzlBoss = tag.ContainsKey("downedGintzlBoss");
			downedSyliaBoss = tag.ContainsKey("downedSyliaBoss");
			downedStoneGolemBoss = tag.ContainsKey("downedStoneGolemBoss");
			downedSTARBoss = tag.ContainsKey("downedSTARBoss");
			downedFenixBoss = tag.ContainsKey("downedFenixBoss");
			downedCogwork = tag.ContainsKey("downedCogwork");
			downedWaterJellyfish = tag.ContainsKey("downedWaterJellyfish");
			downedSparn = tag.ContainsKey("downedSparn");
			downedPandorasBox = tag.ContainsKey("downedPandorasBox");
			downedBlazingSerpent = tag.ContainsKey("downedBlazingSerpent");
			// downedOtherBoss = tag.ContainsKey("downedOtherBoss");
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
			//blazing
			//water cogwork

			// flags[1] = downedOtherBoss;
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