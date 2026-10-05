using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Common.AudioRefreshes;

public record struct OneShotSound(
    SoundStyle Sound, 
    Func<bool> TriggerCondition, 
    Func<Vector2> PositionOverride = null)
{
    bool _hasTriggered;
    public void Update(bool enabled)
    {
        if(!_hasTriggered && TriggerCondition())
        {
            var position = Main.LocalPlayer.Center;
            if (PositionOverride != null)
                position = PositionOverride();
            if(enabled)
                SoundEngine.PlaySound(Sound, position);
            _hasTriggered = true;
        } else if (_hasTriggered && !TriggerCondition())
        {
            _hasTriggered = false;
        }

    }
}
