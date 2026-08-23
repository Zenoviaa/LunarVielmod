using Stellamod.Common.ClassReworkSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Stellamod.Common.ConsoleMenu;

public class ClassCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "class";
    }

    public override Arguments GetArguments()
    {
        Arguments args = new Arguments();
        args.potentialArguments.Add("melee");
        args.potentialArguments.Add("ranger");
        args.potentialArguments.Add("magic");
        args.potentialArguments.Add("mage");
        args.potentialArguments.Add("summon");
        args.potentialArguments.Add("summoner");
        args.potentialArguments.Add("minion");
        args.potentialArguments.Add("omni");
        args.potentialArguments.Add("god");
        return args;
    }
    public override bool Invoke(params string[] args)
    {

        ClassReworkPlayer classReworkPlayer = Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>();
        string className = args[1];
        switch (className)
        {
            case "melee":
                classReworkPlayer.playerClass = PlayerClass.Melee;
                break;
            case "ranger":
                classReworkPlayer.playerClass = PlayerClass.Ranger;
                break;
            case "magic":
            case "mage":
                classReworkPlayer.playerClass = PlayerClass.Mage;
                break;
            case "summon":
            case "summoner":
            case "minion":
                classReworkPlayer.playerClass = PlayerClass.Summoner;
                break;
            case "omni":
                classReworkPlayer.playerClass = PlayerClass.Omni;
                break;
            case "god":
                classReworkPlayer.playerClass = PlayerClass.God;
                break;
        }

        Main.NewText($"You are now {classReworkPlayer.playerClass}!");
        if(Main.netMode != NetmodeID.SinglePlayer)
        {
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.ClassReworkPlayerSync,
                (float)classReworkPlayer.playerClass).Send();
        }
        return true;
    }
}
