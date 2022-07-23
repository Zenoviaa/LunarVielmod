using System.Collections;
using System.Collections.Generic;
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
		// public static bool downedOtherBoss = false;

		public override void OnWorldLoad()
		{
			downedVeriBoss = false;
			// downedOtherBoss = false;
		}

		public override void OnWorldUnload()
		{
			downedVeriBoss = false;
			// downedOtherBoss = false;
		}

		// We save our data sets using TagCompounds.
		// NOTE: The tag instance provided here is always empty by default.
		public override void SaveWorldData(TagCompound tag)
		{
			if (downedVeriBoss)
			{
				tag["downedVeriBoss"] = true;
			}

			// if (downedOtherBoss) {
			//	tag["downedOtherBoss"] = true;
			// }
		}

		public override void LoadWorldData(TagCompound tag)
		{
			downedVeriBoss = tag.ContainsKey("downedMinionBoss");
			// downedOtherBoss = tag.ContainsKey("downedOtherBoss");
		}

		public override void NetSend(BinaryWriter writer)
		{
			// Order of operations is important and has to match that of NetReceive
			var flags = new BitsByte();
			flags[0] = downedVeriBoss;
			// flags[1] = downedOtherBoss;
			writer.Write(flags);

			
		}

		public override void NetReceive(BinaryReader reader)
		{
			// Order of operations is important and has to match that of NetSend
			BitsByte flags = reader.ReadByte();
			downedVeriBoss = flags[0];
			// downedOtherBoss = flags[1];

			// As mentioned in NetSend, BitBytes can contain up to 8 values. If you have more, be sure to read the additional data:
			// BitsByte flags2 = reader.ReadByte();
			// downed9thBoss = flags2[0];

			// System.Collections.BitArray approach:
			/*
			int length = reader.ReadInt32();
			byte[] bytes = reader.ReadBytes(length);
			BitArray bitArray = new BitArray(bytes);
			downedMinionBoss = bitArray[0];
			downedOtherBoss = bitArray[1];
			*/
		}
	}
}