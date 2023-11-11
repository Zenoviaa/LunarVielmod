namespace Stellamod.Helpers
{
    internal interface IComboProjectile
    {
        int MaxCharges { get; }
        int CurCharges { get; }
        int projectileChargeLoopTime { get; }
    }
}