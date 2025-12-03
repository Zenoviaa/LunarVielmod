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
using Stellamod.Core.BossBannerSystem;
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
        }
    }


    public class JackPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.WitchAcademy;
            bossNPC = ModContent.GetInstance<JackTheScholar>();
        }
    }


    public class DaedusPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.WitchAcademy;
            bossNPC = ModContent.GetInstance<DaedusTheDevoted>();
        }
    }

    public class WoodlandRavagerPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.LifeNPlants;
            bossNPC = ModContent.GetInstance<WoodlandRavager>();
        }
    }

    public class MinervaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.LifeNPlants;
            bossNPC = ModContent.GetInstance<Minerva>();
        }
    }

    public class EliteCommanderPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ColosseumOfProtection;
            bossNPC = ModContent.GetInstance<EliteCommander>();
        }
    }

    public class GustbeakPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ColosseumOfProtection;
            bossNPC = ModContent.GetInstance<Gustbeak>();
        }
    }

    public class GintziaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ColosseumOfProtection;
            bossNPC = ModContent.GetInstance<CommanderGintzia>();
        }
    }

    public class JiitasPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ForgottenWarriors;
            bossNPC = ModContent.GetInstance<Jiitas>();
        }
    }
    public class SkullrunnerPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.ForgottenWarriors;
            bossNPC = ModContent.GetInstance<Skullrunner>();
        }
    }

    public class STARBOMBERPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.FountainOfMagic;
            bossNPC = ModContent.GetInstance<STARBOMBERV2>();
        }
    }

    public class VerlianSingularityPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<VerlianSingularity>();
        }
    }

    public class BishininePage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<Bishinine>();
        }
    }

    public class SanguineSingularityPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<SanguineSingularity>();
        }
    }

    public class PunkerPrimePage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.MechanizedRevivals;
            bossNPC = ModContent.GetInstance<PunkerPrime>();
        }
    }

    public class TowerOfIlluriaPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.IllurianTroupe;
            bossNPC = ModContent.GetInstance<CrumblingTowerOfIlluria>();
        }
    }
}
