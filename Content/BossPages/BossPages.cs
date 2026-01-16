using Stellamod.Common.BossBannerSystem;
using Stellamod.Content.Areas.Abyss.BossesAB.VerlianSingularity;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner;
using Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia;
using Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander;
using Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak;
using Stellamod.Content.Areas.Dock.BossesDK.Jiitas;
using Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine;
using Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted;
using Stellamod.Content.Areas.Fable.BossesFB.JackTheScholar;
using Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER;
using Stellamod.Content.Areas.SpringHills.BossesSH.Minerva;
using Stellamod.Content.Areas.SpringHills.BossesSH.Ravager;
using Stellamod.Content.Areas.SpringHills.BossesSH.StarrVeriplant;
using Stellamod.Content.Armors.Ravaging;
using Stellamod.Helpers;
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
            AddReward<RavagingHelmet>();
            AddReward<RavagingChestplate>();
            AddReward<RavagingLegs>();
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
        }
    }
}
