using Stellamod.Content.Areas.Abyss.BossesAB.VerlianSingularity;
using Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia;
using Stellamod.Content.Areas.Fable.BossesFB.JackTheScholar;
using Stellamod.Content.Areas.SpringHills.BossesSH.StarrVeriplant;
using Stellamod.Content.Areas.WaterSide.BossesWS;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace Stellamod.Content.Achievements;

public class RockyBeginnings : ModAchievement
{
    public override void SetStaticDefaults()
    {
        AddNPCKilledCondition(ModContent.NPCType<StarrVeriplant>());
    }

    public override Position GetDefaultPosition() => new Before("TIMBER");
    public override Position GetAdvisorPosition() => new Before("TIMBER");
}


public class Level2 : ModAchievement
{
    public CustomIntCondition LevelCountCondition { get; private set; }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        LevelCountCondition = AddIntCondition("LevelCondition", 2);
    }
    public override Position GetDefaultPosition() => new After("TIMBER");
    public override Position GetAdvisorPosition() => new After("TIMBER");
}


public class EnrolledintheWitchAcademy : ModAchievement
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        AddNPCKilledCondition(ModContent.NPCType<JackTheScholar>());
    }

    public override Position GetDefaultPosition() => new Before("EYE_ON_YOU");
    public override Position GetAdvisorPosition() => new Before("EYE_ON_YOU");
}
public class GintzingWinds : ModAchievement
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        AddNPCKilledCondition(ModContent.NPCType<CommanderGintzia>());
    }

    public override Position GetDefaultPosition() => new Before("EYE_ON_YOU");
    public override Position GetAdvisorPosition() => new Before("EYE_ON_YOU");
}

public class AlcoholicMuch : ModAchievement
{
    public CustomIntCondition DrunkenCountCondition { get; private set; }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrunkenCountCondition = AddIntCondition("DrinkCondition", 25);
    }

    public override Position GetDefaultPosition() => new Before("EYE_ON_YOU");
    public override Position GetAdvisorPosition() => new Before("EYE_ON_YOU");
}

public class FirstSingularity : ModAchievement
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        AddNPCKilledCondition(ModContent.NPCType<VerlianSingularity>());
    }

    public override Position GetDefaultPosition() => new Before("BONED");
    public override Position GetAdvisorPosition() => new Before("BONED");
}
public class WhistlingoftheSeas : ModAchievement
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        AddNPCKilledCondition(ModContent.NPCType<LeviathanEel>());
    }

    public override Position GetDefaultPosition() => new After("BONED");
    public override Position GetAdvisorPosition() => new After("BONED");
}

public class WitchsBabySteps : ModAchievement
{
    public CustomIntCondition BrewCountCondition { get; private set; }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BrewCountCondition = AddIntCondition("WitchCondition", 1);
    }
    public override Position GetDefaultPosition() => new After("TIMBER");
    public override Position GetAdvisorPosition() => new After("TIMBER");
}