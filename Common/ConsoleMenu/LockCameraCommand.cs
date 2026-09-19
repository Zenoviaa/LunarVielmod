using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public class LockCameraCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "lockcamera";
    }

    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        LunarDebugging.lockCamera = !LunarDebugging.lockCamera;
        LunarDebugging.lockCameraPosition = Main.screenPosition;
        return true;
    }
}


public class LockCameraSystem : ModSystem
{ 
    public override void ModifyScreenPosition()
    {
        base.ModifyScreenPosition();
        if (!LunarDebugging.lockCamera)
            return;
        Main.screenPosition = LunarDebugging.lockCameraPosition;
    }
}