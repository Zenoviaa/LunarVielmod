using Stellamod.Common.BossBannerSystem;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner;
using Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia;
using Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander;
using Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak;
using Stellamod.Content.Areas.Dock.BossesDK.Jiitas;
using Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine;
using Stellamod.Content.Areas.EveroseVillage.CelestiaBoss;
using Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted;
using Stellamod.Content.Areas.Fable.BossesFB.JackTheScholar;
using Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria;
using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;
using Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER;
using Stellamod.Content.Areas.SpringHills.AccSH;
using Stellamod.Content.Areas.SpringHills.BossesSH.Minerva;
using Stellamod.Content.Areas.SpringHills.BossesSH.Ravager;
using Stellamod.Content.Areas.SpringHills.BossesSH.StarrVeriplant;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.CariyaBoss;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Content.Areas.Underground.BunnyStormBoss;
using Stellamod.Content.Areas.WaterSide.BossesWS;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Content.Armors.Ravaging;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Currencies;
using Stellamod.Content.Relics;
using Stellamod.Content.Vanity.IllurianGeneralHat;
using Stellamod.Items.Consumables;
using Stellamod.Items.Insources;
using Terraria.ModLoader;

namespace Stellamod.Content.BossPages
{

    public class StoneGolemPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.WitchAcademy;
            bossNPC = ModContent.GetInstance<StarrVeriplant>();
            progression = 0;
            flag = DownedBossFlag.StoneGolem;
            StarRanking = 1;
            AddMasterModeReward<StoneGolemRelicItem>();
        }
    }


    public class JackPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.WitchAcademy;
            bossNPC = ModContent.GetInstance<JackTheScholar>();
            StarRanking = 1;
            progression = 1;
            flag = DownedBossFlag.Jack;
            StarRanking = 1;
            AddMasterModeReward<JackRelicItem>();
        }
    }


    public class DaedusPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.WitchAcademy;
            bossNPC = ModContent.GetInstance<DaedusTheDevoted>();
            progression = 4;
            flag = DownedBossFlag.Daedus;
            StarRanking = 2;
            AddMasterModeReward<DaedusRelicItem>();
        }
    }
    
    public class BunnyStormPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.Miniboss;
            bossNPC = ModContent.GetInstance<BunnyStorm>();
            progression = 4;
            flag = DownedBossFlag.BunnyStorm;
            StarRanking = 2;
            AddMasterModeReward<BunnyStormRelicItem>();
        }
    }

    public class WoodlandRavagerPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.LifeNPlants;
            bossNPC = ModContent.GetInstance<WoodlandRavager>();
            progression = 3;
            flag = DownedBossFlag.Woodland_Ravager;
            StarRanking = 2;

            AddReward<DragonShard>(stack: 1);
            AddReward<RuinMedal>(stack: 10);
            AddReward<BeastRage>();
            AddReward<RavagingHelmet>();
            AddReward<RavagingChestplate>();
            AddReward<RavagingLegs>();

            AddMasterModeReward<WoodlandRavagerRelicItem>(stack: 1);
            AddMasterModeReward<DragonShard>(stack: 1);
            AddMasterModeReward<GlisteningPearl>(stack: 3);

            AddNoHitReward<BeastInsource>();
        }
    }

    public class MinervaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.LifeNPlants;
            bossNPC = ModContent.GetInstance<Minerva>();
            progression = 8;
            flag = DownedBossFlag.Minerva;
            AddMasterModeReward<MinervaRelicItem>();
            StarRanking = 3;
        }
    }

    public class EliteCommanderPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ColosseumOfProtection;
            bossNPC = ModContent.GetInstance<EliteCommander>();
            progression = 5;
            flag = DownedBossFlag.EliteCommander;
            StarRanking = 2;
            AddMasterModeReward<EliteCommanderRelicItem>();
        }
    }

    public class GustbeakPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ColosseumOfProtection;
            bossNPC = ModContent.GetInstance<Gustbeak>();
            progression = 6;
            flag = DownedBossFlag.Gustbeak;
            AddReward<DragonShard>(stack: 2);
            StarRanking = 3;
            AddMasterModeReward<GustbeakRelicItem>();
        }
    }

    public class GintziaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ColosseumOfProtection;
            bossNPC = ModContent.GetInstance<CommanderGintzia>();
            progression = 7;
            flag = DownedBossFlag.Commander_Gintzia;
            AddReward<VoidKey>();
            StarRanking = 3;
            AddMasterModeReward<CommanderGintziaRelicItem>();
        }


    }

    public class JiitasPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ForgottenWarriors;
            bossNPC = ModContent.GetInstance<Jiitas>();
            progression = 2;
            flag = DownedBossFlag.Jiitas;
            AddMasterModeReward<JiitasRelicItem>();
            StarRanking = 1;
        }
    }
    public class CelestiaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ForgottenWarriors;
            bossNPC = ModContent.GetInstance<Celestia>();
            progression = 3;
            flag = DownedBossFlag.Celestia;
            StarRanking = 3;
            AddMasterModeReward<CelestiaRelicItem>();
        }
    }
    public class SkullrunnerPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ForgottenWarriors;
            bossNPC = ModContent.GetInstance<Skullrunner>();
            progression = 11;
            flag = DownedBossFlag.Skullrunner;
            StarRanking = 4;
            AddMasterModeReward<SkullrunnerRelicItem>();
        }
    }
    public class EPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<E>();
            progression = 11;
            flag = DownedBossFlag.E;
            StarRanking = 7;
            AddMasterModeReward<ERelicItem>();
        }
    }
    public class STARBOMBERPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.FountainOfMagic;
            bossNPC = ModContent.GetInstance<STARBOMBERV2>();
            progression = 12;
            flag = DownedBossFlag.StarBomber;
            StarRanking = 3;
            AddMasterModeReward<StarbomberRelicItem>();
        }
    }

    public class VerlianSingularityPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<VerlianSingularity>();
            progression = 9;
            flag = DownedBossFlag.Verlian_Singularity;
            AddMasterModeReward<SingularityRelicItem>();
            StarRanking = 4;
        }
    }
    public class CariyaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ForgottenWarriors;
            bossNPC = ModContent.GetInstance<Cariya>();
            progression = 9;
            flag = DownedBossFlag.Cariya;
            StarRanking = 3;
            AddMasterModeReward<CariyaRelicItem>(stack: 1);
        }
    }
    public class KingJellyfishPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.Miniboss;
            bossNPC = ModContent.GetInstance<KingJellyfish>();
            progression = 4;
            flag = DownedBossFlag.KingJellyfish;
            StarRanking = 2;
            AddMasterModeReward<KingJellyfishRelicItem>(stack: 1);
        }
    }

    public class LeviathanEelPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.LifeNPlants;
            bossNPC = ModContent.GetInstance<LeviathanEel>();
            progression = 8;
            flag = DownedBossFlag.LeviathanEel;
            StarRanking = 5;
            AddMasterModeReward<LeviathanEelRelicItem>(stack: 1);
        }
    }
    public class GothiviaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.LifeNPlants;
            bossNPC = ModContent.GetInstance<Gothivia>();
            progression = 25;
            flag = DownedBossFlag.Gothivia;
            StarRanking = 8;
            AddMasterModeReward<GothiviaRelicItem>(stack: 1);
        }
    }
    public class RoyalFoxPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.FountainOfMagic;
            bossNPC = ModContent.GetInstance<RoyalFox>();
            progression = 15;
            flag = DownedBossFlag.RoyalFox;
            StarRanking = 8;
            AddMasterModeReward<RoyalFoxRelicItem>(stack: 1);
        }
    }

    public class VerliaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<Verlia>();
            progression = 10;
            flag = DownedBossFlag.Verlia;

            StarRanking = 6;
            AddMasterModeReward<VerliaRelicItem>(stack: 1);
        }
    }


    public class BishininePage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<Bishinine>();
            progression = 13;
            flag = DownedBossFlag.Bishinine;
            AddNoHitReward<IllurianGeneralHat>();
            AddMasterModeReward<BishinineRelicItem>();
            StarRanking = 6;
        }
    }

    public class SanguineSingularityPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<SanguineSingularity>();
            progression = 20;
            flag = DownedBossFlag.SanguineSingularity;
            StarRanking = 6;
            AddMasterModeReward<SanguineSingularityRelicItem>();
        }
    }

    public class PunkerPrimePage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.MechanizedRevivals;
            bossNPC = ModContent.GetInstance<PunkerPrime>();
            progression = 15;
            flag = DownedBossFlag.PunkerPrime;

            StarRanking = 4;
            AddMasterModeReward<PunkerPrimeRelicItem>(stack: 1);
        }
    }
    public class DescendingTwinsPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.MechanizedRevivals;
            bossNPC = ModContent.GetInstance<DescendingTwins>();
            progression = 16;
            flag = DownedBossFlag.DescendingTwins;

            StarRanking = 5;
            AddMasterModeReward<DescendingTwinsRelicItem>(stack: 1);
        }
    }
    public class SteamrollerPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.MechanizedRevivals;
            bossNPC = ModContent.GetInstance<Steamroller>();
            progression = 17;
            flag = DownedBossFlag.Steamroller;

            StarRanking = 6;
            AddMasterModeReward<SteamrollerRelicItem>(stack: 1);
        }
    }


    public class TowerOfIlluriaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<CrumblingTowerOfIlluria>();
            progression = 18;
            flag = DownedBossFlag.CrumblingTowerOfIlluria;
            StarRanking = 4;
            AddMasterModeReward<TowerofIlluriaRelicItem>();
        }
    }
}
