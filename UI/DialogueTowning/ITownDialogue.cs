using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Stellamod.UI.DialogueTowning
{
    public interface ITownDialogue
    {
        void SetTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons);
    }
}
