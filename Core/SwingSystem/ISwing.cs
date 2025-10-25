using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Core.SwingSystem
{
    public interface ISwing
    {
        void SetDirection(int direction);
        float GetDuration(float attackSpeedMultiplier);

        int GetHitCount();


        bool CanHurt(BaseSwingProjectileV2 swingProjectile);

        void UpdateSwing(BaseSwingProjectileV2 swingProjectile);
        void CalculateAfterImagePoints(BaseSwingProjectileV2 swingProjectile);
        void CalculateTrailingPoints(BaseSwingProjectileV2 swingProjectile);
    }
}
