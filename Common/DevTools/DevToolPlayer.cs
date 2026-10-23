using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Common.DevTools;
public class DevToolPlayer : ModPlayer
{
    public static bool left;
    public static bool right;
    public static bool up;
    public static bool down;
    public override void SetControls()
    {
        base.SetControls();
        if (DevTools.freeCamera == FreeCamera.Free)
        {
            left = Player.controlLeft;
            right = Player.controlRight;
            up = Player.controlUp;
            down = Player.controlDown;

            Player.controlDown = false;
            Player.controlUp = false;
            Player.controlLeft = false;
            Player.controlRight = false;

            Player.controlMount = false;
            Player.controlHook = false;
            Player.controlThrow = false;
            //	player.controlJump = false;
  
            Player.controlTorch = false;
        }
        if (DevTools.infiniteReach)
            Player.controlSmart = false;
    }

    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
    }

    public override void ModifyScreenPosition()
    {
        base.ModifyScreenPosition();
        if (DevTools.freeCamera == FreeCamera.Normal)
            return;
        Main.screenPosition = DevTools.freeCameraPosition;
    }

    public override void ResetEffects()
    {

        if (Player.whoAmI == Main.myPlayer)
        {
            if (DevTools.infiniteReach)
            {
                if (Main.SmartCursorWanted || Main.SmartCursorIsUsed)
                {
                    Main.SmartCursorWanted_Mouse = false;
                    Main.SmartCursorWanted_GamePad = false;
                }
                else
                {
                    Player.tileRangeX = int.MaxValue / 32 - 20;
                    Player.tileRangeY = int.MaxValue / 32 - 20;
                }
            }
        }
    }
    public override void PreUpdate()
    {
        if (DevTools.godMode == GodMode.NoDeath)
        {
            Player.statLife = Player.statLifeMax2;
        }
        if (DevTools.infiniteFlight)
        {
            Player.wingTime = Player.wingTimeMax;
        }
        if (DevTools.infiniteMana)
        {
            Player.statMana = Player.statManaMax2;
        }
    }
    public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
    {
        if (DevTools.godMode == GodMode.NoHit)
            return true;
        return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
    }
    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        if (DevTools.godMode == GodMode.NoDeath)
        {
            return false;
        }
        return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
    }
}
