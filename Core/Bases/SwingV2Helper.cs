using Stellamod.Assets;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Terraria.Audio;

namespace Stellamod.Core.Bases
{
    public interface ISwingProjectile
    {
        void Add(ISwing swing);
    }

    public static class SwingV2Helper
    {
        public static void AddSwordSwingStyle(ISwingProjectile swings)
        {
            SoundStyle swingSound1 = AssetRegistry.Sounds.Melee.NormalSwordSlash1;
            swingSound1.PitchVariance = 0.25f;

            SoundStyle swingSound2 = AssetRegistry.Sounds.Melee.NormalSwordSlash2;
            swingSound2.PitchVariance = 0.25f;

            SoundStyle swingSound3 = AssetRegistry.Sounds.Melee.SwordSpin1;
            swingSound3.PitchVariance = 0.5f;

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound2
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound2
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound1
            });

            swings.Add(new OvalSwing
            {
                Duration = 40,
                XSwingRadius = 100,
                YSwingRadius = 40,
                SwingDegrees = 540,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound3
            });
        }
    }
}
