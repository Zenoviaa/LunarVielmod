using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Microsoft.Xna.Framework;

namespace Stellamod.Projectiles.Summons.Orbs
{
    internal static class OrbHelper
    {
        public static void PlaySummonSound(Vector2 position, float pitchOffset = 0)
        {
            SoundStyle soundStyle;
            switch (Main.rand.Next(2))
            {
                default:
                case 0:
                    soundStyle = SoundRegistry.OrbSummon1;
                    break;
                case 1:
                    soundStyle = SoundRegistry.OrbSummon2;
                    break;
            }


            soundStyle.PitchVariance = 0.15f;
            soundStyle.Pitch = pitchOffset;
            SoundEngine.PlaySound(soundStyle, position);
        }

        public static void PlaySmashSound(Vector2 position, float pitchOffset = 0)
        {
            SoundStyle soundStyle;
            switch (Main.rand.Next(2))
            {
                default:
                case 0:
                    soundStyle = SoundRegistry.OrbSmash;
                    break;
                case 1:
                    soundStyle = SoundRegistry.OrbSmash;
                    break;
            }


            soundStyle.PitchVariance = 0.15f;
            soundStyle.Pitch = pitchOffset;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }
}
