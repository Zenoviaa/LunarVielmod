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
        public static void AddSpearSwingStyle(ISwingProjectile swings)
        {
            SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;
            swings.Add(new OvalSwing
            {
                Duration = 22,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = spearSlash1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 22,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = spearSlash1,
            });


            swings.Add(new ThrustSwing
            {
                Duration = 12,
                ThrowDistance = 90,
                Easing = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                Sound = spearSlash2
            });


            swings.Add(new ThrustSwing
            {
                Duration = 12,
                ThrowDistance = 90,
                Easing = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                Sound = spearSlash2
            });


            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = spearSlash1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = spearSlash1,
            });
            swings.Add(new OvalSwing
            {
                Duration = 60,
                SwingDegrees = 360 * 4,
                XSwingRadius = 64,
                YSwingRadius = 64,
                HitCount = 8,
                Easing = (float lerpValue) => lerpValue,
                Sound = nSpin
            });

            swings.Add(new ThrustSwing
            {
                Duration = 30,
                ThrowDistance = 128,
                Easing = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                Sound = spearSlash2
            });

            swings.Add(new ThrustSwing
            {
                Duration = 60,
                ThrowDistance = 200,
                Easing = (float lerpValue) => Easing.SpikeOutExpo(lerpValue),
                Sound = spearSlash2
            });
        }
        public static void AddScytheSwingStyle(ISwingProjectile swings)
        {
            SoundStyle swingSound1 = AssetRegistry.Sounds.Melee.ScytheWindSlash1;
            swingSound1.PitchVariance = 0.25f;

            SoundStyle swingSound2 = AssetRegistry.Sounds.Melee.ScytheWindSlash2;
            swingSound2.PitchVariance = 0.25f;

            SoundStyle swingSound3 = AssetRegistry.Sounds.Melee.ScytheBigSlash;
            swingSound3.PitchVariance = 0.5f;


            //Pair 1
            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 48,
                SwingDegrees = 360,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 100,
                SwingDegrees = 360,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound2
            });

            //Pair 2
            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 100,
                SwingDegrees = 360,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 100,
                SwingDegrees = 360,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound2
            });

           //Throw
            swings.Add(new OvalSwing
            {
                Duration = 40,
                XSwingRadius = 120,
                YSwingRadius = 120,
                SwingDegrees = 540,
                ThrowRadius = 64,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound3,
                HitCount=3,
            });
        }
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
