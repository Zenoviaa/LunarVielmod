using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Helpers
{
    public enum DownedBossFlag : byte
    {
        Woodland_Ravager = 0,
        Minerva = 1,
        Jack = 2,
        Daedus = 3,
        Verlian_Singularity = 4,
        Skullrunner = 5,
        Commander_Gintzia = 6,
        EliteCommander = 7,
        Gustbeak = 8,
        StarBomber = 9,
        Bishinine = 10,
        Jiitas = 11,
        SanguineSingularity = 12,
        PunkerPrime = 13,
        CrumblingTowerOfIlluria = 14,
        StoneGolem = 15,
        Steamroller = 16,
        DescendingTwins=17,
        Verlia=18,
        Celestia=19
    }

    public class Flawless : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            if (NPC.AnyDanger())
                return;
            player.DelBuff(buffIndex);
        }
    }

    public class DownedBossRewardPlayer : ModPlayer
    {
        public bool[] claimedRegularRewards = new bool[64];
        public bool[] claimedMasterRewards = new bool[64];
        public bool[] claimedNoHit = new bool[64];
        public bool[] hasNoHit = new bool[64];
        private void InitializeIfNeeded()
        {
            claimedRegularRewards ??= new bool[64];
            claimedMasterRewards ??= new bool[64];
            claimedNoHit ??= new bool[64];
            hasNoHit ??= new bool[64];

            if (hasNoHit.Length < 64)
                hasNoHit = new bool[64];
        }
        public void ResetFlags()
        {
            InitializeIfNeeded();
            for (int i = 0; i < claimedNoHit.Length; i++)
            {
                claimedMasterRewards[i] = false;
                claimedNoHit[i] = false;
                claimedRegularRewards[i] = false;
                hasNoHit[i] = false;
            }
        }


        public override void PostHurt(Player.HurtInfo info)
        {
            base.PostHurt(info);
            Player.ClearBuff(ModContent.BuffType<Flawless>());
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["claimedRegularRewards"] = claimedRegularRewards;
            tag["claimedMasterRewards"] = claimedMasterRewards;
            tag["claimedNoHit"] = claimedNoHit;
            tag["hasNoHit"] = hasNoHit;

        }


        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            claimedRegularRewards = tag.Get<bool[]>("claimedRegularRewards");
            claimedMasterRewards = tag.Get<bool[]>("claimedMasterRewards");
            claimedNoHit = tag.Get<bool[]>("claimedNoHit");
            hasNoHit = tag.Get<bool[]>("hasNoHit");

            InitializeIfNeeded();

        }

        public static void HandleBossDownedMessage(BinaryReader reader, int whoAmI)
        {
            int flag = reader.ReadInt32();
            DisplayNoHit(flag);
        }

        public static void DisplayNoHit(int flag)
        {
            DownedBossRewardPlayer rwardPlayer = Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>();
            if (rwardPlayer.Player.HasBuff(ModContent.BuffType<Flawless>()))
            {
                rwardPlayer.hasNoHit[flag] = true;
                string text = LangText.Common("NoHit");
                int c = CombatText.NewText(rwardPlayer.Player.getRect(), Color.White, text, true);
                Main.combatText[c].lifeTime *= 3;
                rwardPlayer.Player.ClearBuff(ModContent.BuffType<Flawless>());
            }
        }
    }

    public class DownedBossTracker : ModSystem
    {
        public static bool[] downedBossFlags = new bool[64];
        public static void ResetFlags()
        {
            for (int i = 0; i < downedBossFlags.Length; i++)
            {
                downedBossFlags[i] = false;
            }
        }

        public override void ClearWorld()
        {
            base.ClearWorld();
            ResetFlags();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["downedBossFlags"] = downedBossFlags;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedBossFlags = tag.Get<bool[]>("downedBossFlags");
        }

        public static bool IsDowned(DownedBossFlag flag)
        {
            return IsDowned((int)flag);
        }
        public static bool IsNoHit(DownedBossFlag flag)
        {
            return Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>().hasNoHit[(int)flag];
        }

        public static bool IsDowned(int id)
        {
            return downedBossFlags[id];
        }
        public static bool IsNoHit(int id)
        {
            return downedBossFlags[id];
        }
        public static void ClearFlag(DownedBossFlag flag)
        {
            ClearFlag((int)flag);
        }

        public static void ClearFlag(int id)
        {
            NPC.SetEventFlagCleared(ref downedBossFlags[id], -1);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                int clientToIgnore = Main.LocalPlayer.whoAmI;
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(),
                    (byte)MessageType.BossDowned, id).Send(ignoreClient: clientToIgnore);
            }
            else
            {
                DownedBossRewardPlayer.DisplayNoHit(id);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            int numBytes = downedBossFlags.Length / 8;
            int j = 0;
            for (int i = 0; i < numBytes; i++)
            {
                BitsByte b = new BitsByte
                {
                    [0] = downedBossFlags[j],
                    [1] = downedBossFlags[j + 1],
                    [2] = downedBossFlags[j + 2],
                    [3] = downedBossFlags[j + 3],
                    [4] = downedBossFlags[j + 4],
                    [5] = downedBossFlags[j + 5],
                    [6] = downedBossFlags[j + 6],
                    [7] = downedBossFlags[j + 7]
                };
                writer.Write(b);
                j += 8;
            }
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int numBytes = downedBossFlags.Length / 8;
            int j = 0;
            for (int i = 0; i < numBytes; i++)
            {
                BitsByte flags = reader.ReadByte();
                downedBossFlags[j] = flags[0];
                downedBossFlags[j + 1] = flags[1];
                downedBossFlags[j + 2] = flags[2];
                downedBossFlags[j + 3] = flags[3];
                downedBossFlags[j + 4] = flags[4];
                downedBossFlags[j + 5] = flags[5];
                downedBossFlags[j + 6] = flags[6];
                downedBossFlags[j + 7] = flags[7];
                j += 8;
            }
        }
    }
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
        public static bool downedCommanderGintziaBoss = false;
        public static bool downedSyliaBoss = false;
        public static bool downedStoneGolemBoss = false;
        public static bool downedSTARBoss = false;
        public static bool downedFenixBoss = false;
        public static bool downedNESTBoss = false;
        public static bool downedPandorasBox = false;
        public static bool downedBlazingSerpent = false;
        public static bool downedWaterJellyfish = false;
        public static bool downedSparn = false;
        public static bool downedCogwork = false;
        public static bool downedAzurewrathBoss = false;

        public static bool downedDreadMonolith1 = false;
        public static bool downedDreadMonolith2 = false;
        public static bool downedDreadMonolith3 = false;

        public static bool downedIrradiaBoss = false;
        public static bool downedSupernovaFragmentBoss = false;
        public static bool downedNiiviBoss = false;
        public static bool downedRekBoss = false;
        public static bool downedGothiviaBoss = false;
        public static bool downedEreshBoss = false;
        public static bool downedLumiBoss = false;
        public static bool downedVoidBoss = false;
        public static bool downedBishinineBoss = false;
        public static bool downedSkullrunnerBoss = false;
        public static bool downedMinervaBoss = false;
        public static bool downedRavagerBoss = false;
        public static bool downedJiitasBoss = false;
        public static void ResetFlags()
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
            downedDreadMonolith1 = false;
            downedDreadMonolith2 = false;
            downedDreadMonolith3 = false;

            downedIrradiaBoss = false;
            downedSupernovaFragmentBoss = false;
            downedNiiviBoss = false;
            downedRekBoss = false;
            downedGothiviaBoss = false;
            downedEreshBoss = false;
            downedLumiBoss = false;
            downedVoidBoss = false;
            downedCommanderGintziaBoss = false;

            downedBishinineBoss = false;
            downedSkullrunnerBoss = false;
            downedMinervaBoss = false;
            downedRavagerBoss = false;
            downedJiitasBoss = false;
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
            tag["downedRekBoss"] = downedRekBoss;
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
            tag["downedDreadMonolith1"] = downedDreadMonolith1;
            tag["downedDreadMonolith2"] = downedDreadMonolith2;
            tag["downedDreadMonolith3"] = downedDreadMonolith3;
            tag["downedSupernovaFragmentBoss"] = downedSupernovaFragmentBoss;
            tag["downedNiiviBoss"] = downedNiiviBoss;
            tag["downedGothiviaBoss"] = downedGothiviaBoss;
            tag["downedEreshBoss"] = downedEreshBoss;
            tag["downedLumiBoss"] = downedLumiBoss;
            tag["downedVoidBoss"] = downedVoidBoss;
            tag["downedIrradiaBoss"] = downedIrradiaBoss;
            tag["downedCommanderGintziaBoss"] = downedCommanderGintziaBoss;
            tag["downedBishinineBoss"] = downedBishinineBoss;
            tag["downedRavagerBoss"] = downedRavagerBoss;
            tag["downedSkullrunnerBoss"] = downedSkullrunnerBoss;
            tag["downedMinervaBoss"] = downedMinervaBoss;
            tag["downedJiitasBoss"] = downedJiitasBoss;
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
            downedDreadMonolith1 = tag.GetBool("downedDreadMonolith1");
            downedDreadMonolith2 = tag.GetBool("downedDreadMonolith2");
            downedDreadMonolith3 = tag.GetBool("downedDreadMonolith3");
            downedSupernovaFragmentBoss = tag.GetBool("downedSupernovaFragmentBoss");
            downedNiiviBoss = tag.GetBool("downedNiiviBoss");
            downedRekBoss = tag.GetBool("downedRekBoss");
            downedGothiviaBoss = tag.GetBool("downedGothiviaBoss");
            downedEreshBoss = tag.GetBool("downedEreshBoss");
            downedLumiBoss = tag.GetBool("downedLumiBoss");
            downedVoidBoss = tag.GetBool("downedVoidBoss");
            downedIrradiaBoss = tag.GetBool("downedIrradiaBoss");
            downedCommanderGintziaBoss = tag.GetBool("downedCommanderGintziaBoss");

            downedBishinineBoss = tag.GetBool("downedBishinineBoss");
            downedRavagerBoss = tag.GetBool("downedRavagerBoss");
            downedMinervaBoss = tag.GetBool("downedMinervaBoss");
            downedSkullrunnerBoss = tag.GetBool("downedSkullrunnerBoss");
            downedJiitasBoss = tag.GetBool("downedJiitasBoss");
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
                [3] = downedAzurewrathBoss,
                [4] = downedDreadMonolith1,
                [5] = downedDreadMonolith2,
                [6] = downedDreadMonolith3,
                [7] = downedSupernovaFragmentBoss
            });

            writer.Write(new BitsByte
            {
                [0] = downedNiiviBoss,
                [1] = downedGothiviaBoss,
                [2] = downedEreshBoss,
                [3] = downedLumiBoss,
                [4] = downedVoidBoss,
                [5] = downedIrradiaBoss,
                [6] = downedRekBoss,
                [7] = downedCommanderGintziaBoss
            });
            writer.Write(new BitsByte
            {
                [0] = downedBishinineBoss,
                [1] = downedRavagerBoss,
                [2] = downedMinervaBoss,
                [3] = downedSkullrunnerBoss,
                [4] = downedJiitasBoss
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
            downedDreadMonolith1 = flags[4];
            downedDreadMonolith2 = flags[5];
            downedDreadMonolith3 = flags[6];
            downedSupernovaFragmentBoss = flags[7];

            flags = reader.ReadByte();
            downedNiiviBoss = flags[0];
            downedGothiviaBoss = flags[1];
            downedEreshBoss = flags[2];
            downedLumiBoss = flags[3];
            downedVoidBoss = flags[4];
            downedIrradiaBoss = flags[5];
            downedRekBoss = flags[6];
            downedCommanderGintziaBoss = flags[7];

            flags = reader.ReadByte();
            downedBishinineBoss = flags[0];
            downedRavagerBoss = flags[1];
            downedMinervaBoss = flags[2];
            downedSkullrunnerBoss = flags[3];
            downedJiitasBoss = flags[4];
        }
    }
}