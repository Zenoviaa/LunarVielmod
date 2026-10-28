using Stellamod.Core.SwingSystem;
using Terraria;


namespace Stellamod.Common.MeleeSwingSystem;

public class Swinger
{
    public required ISwordMovement movement;
    public float swingTime;
    public int direction;
    public bool fixRot;
    public SwordMovement GetMovement(Projectile projectile)
    {
        return GetMovement(ref projectile.ai[0], direction);
    }
    public SwordMovement GetMovement(ref float timer, in int dir)
    {
        float uneasedLerpValue = timer / swingTime;
        var input = new SwingInput(uneasedLerpValue, dir);
        return movement.CalculateSwordMovement(input);
    }
}
