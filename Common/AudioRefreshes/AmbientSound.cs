using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Common.AudioRefreshes;

public record struct AmbientSound(
    SoundStyle Sound, 
    Func<bool> ActiveCondition, 
    Func<Vector2> PositionOverride = null,
    bool VolumeOsc = false)
{
    enum PlayingState : byte
    {
        Inactive,
        Active
    }
    PlayingState _state;
    SlotId _slot;
    public void Update(bool enabled)
    {
        bool shouldBeActive = ActiveCondition();
     
        if (!enabled)
            shouldBeActive = false;

        switch (_state)
        {
            case PlayingState.Inactive:
    
                if (shouldBeActive)
                {
                 
                    _slot = SoundEngine.PlaySound(Sound, Main.LocalPlayer.position);
                    _state = PlayingState.Active;
                }
                break;
            case PlayingState.Active:
    
                if (!shouldBeActive)
                {
                    if(SoundEngine.TryGetActiveSound(_slot, out var result))
                    {
                
                        result.Stop();
                    }
                    _state = PlayingState.Inactive;
                }
                else
                {
             
                    if (SoundEngine.TryGetActiveSound(_slot, out var result))
                    {
                   
                        result.Position = Main.LocalPlayer.position;
                        if (PositionOverride != null)
                            result.Position = PositionOverride();
                        result.Volume = Sound.Volume * Main.ambientVolume;
                    }
                    else
                    {
             
                        _slot = SoundEngine.PlaySound(Sound, Main.LocalPlayer.position);
                    }
                }
                break;
        }
    }
}
