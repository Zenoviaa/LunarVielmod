using Microsoft.Xna.Framework;

namespace Stellamod.Core.LunarLightingSystem
{
    public interface IBackLightModifier
    {
        void ModifyBackLight(ref Color backLightColor);
    }
}
