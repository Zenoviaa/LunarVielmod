using Microsoft.Xna.Framework;
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
        public static void AddHammerSwingStyle(ISwingProjectile swings)
        {

            SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
            hammerSlash1.PitchVariance = 0.2f;

            SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
            hammerSlash2.PitchVariance = 0.2f;

            swings.Add(new OvalSwing
            {
                Duration = 90,
                SwingDegrees = 310,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash1,
                HitCount=2
            });

            swings.Add(new OvalSwing
            {
                Duration = 90,
                SwingDegrees = 310,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash2,
                HitCount = 2
            });

            swings.Add(new OvalSwing
            {
                Duration = 90,
                SwingDegrees = 330,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash1,
                HitCount = 2
            });

            swings.Add(new OvalSwing
            {
                Duration = 78,
                SwingDegrees = 330,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash2,
                HitCount = 2
            });

            swings.Add(new OvalSwing
            {
                Duration = 78,
                SwingDegrees = 330,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash1,
                HitCount = 2
            });

            swings.Add(new OvalSwing
            {
                Duration = 78,
                SwingDegrees = 600,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash2,
                HitCount = 2
            });

            swings.Add(new OvalSwing
            {
                Duration = 80,
                SwingDegrees = 600,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash1,
                HitCount = 2
            });

            swings.Add(new OvalSwing
            {
                Duration = 100,
                SwingDegrees = 600,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash2,
                HitCount = 2
            });

            swings.Add(new OvalSwing
            {
                Duration = 120,
                SwingDegrees = 800,
                XSwingRadius = 64,
                YSwingRadius = 64,
                Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
                Sound = hammerSlash1,
                HitCount = 2
            });
        }
        public static void AddKnivesSwingStyle(ISwingProjectile swings)
        {

            SoundStyle swingSound1 = SoundRegistry.NSwordSlash2;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.NSwordSlash2;
            swingSound2.PitchVariance = 0.5f;
            swingSound2.Pitch = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.NSwordSlash1;
            swingSound3.PitchVariance = 0.5f;


            swings.Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 84,
                YSwingRadius = 42,
                SwingDegrees = 315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 72,
                YSwingRadius = 36,
                SwingDegrees = 315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound2,
            });


            swings.Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 84,
                YSwingRadius = 56,
                SwingDegrees = 315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound1,
            });


            swings.Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 96,
                YSwingRadius = 32,
                SwingDegrees = 315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound2,
            });


            swings.Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 76,
                YSwingRadius = 24,
                SwingDegrees =315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound1,
            });


            swings.Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 84,
                YSwingRadius = 32,
                SwingDegrees = 315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound2,
            });

            swings.Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 80,
                YSwingRadius = 16,
                SwingDegrees = 315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 76,
                YSwingRadius = 24,
                SwingDegrees = 315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound2
            });

            swings.Add(new OvalSwing
            {
                Duration = 6,
                XSwingRadius = 76,
                YSwingRadius = 24,
                SwingDegrees = 315,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue),
                Sound = swingSound2
            });
        }
        public static void AddGreatswordSwingStyle(ISwingProjectile swings)
        {
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.HeavySwordSlash2;
            swingSound2.PitchVariance = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.NSwordSpin1;
            swingSound3.PitchVariance = 0.5f;

            swings.Add(new OvalSwing
            {
                Duration = 44,
                XSwingRadius = 64,
                YSwingRadius = 48,
                SwingDegrees=330,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });


            swings.Add(new OvalSwing
            {
                Duration = 90,
                XSwingRadius = 1,
                YSwingRadius = 1,
                SwingDegrees = 2000,
                SpinThrowDistance = 40,
                SpinDegrees=1,
                AlwaysShowTrail=true,
                Easing = (float lerpValue) => lerpValue,
                Sound = swingSound3,
                HitCount = 12
            });

            swings.Add(new OvalSwing
            {
                Duration = 70,
                XSwingRadius = 84 / 1.5f,
                YSwingRadius = 70 / 1.5f,
                SwingDegrees =270,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound2,
            });

            swings.Add(new OvalSwing
            {
                Duration = 70,
                XSwingRadius = 84 / 1.5f,
                YSwingRadius = 70 / 1.5f,
                SwingDegrees = 270,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound2,
            });

            swings.Add(new OvalSwing
            {
                XSwingRadius = 64,
                YSwingRadius=64,
                Duration = 48,
                SwingDegrees = 330,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                XSwingRadius = 64,
                YSwingRadius = 64,
                Duration = 96,
                SwingDegrees = 330,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });


            swings.Add(new OvalSwing
            {
                Duration = 100,
                XSwingRadius = 1,
                YSwingRadius = 1,
                SwingDegrees = 2000,
                SpinThrowDistance = 40,
                SpinDegrees = 1,
                AlwaysShowTrail = true,
                Easing = (float lerpValue) => lerpValue,
                Sound = swingSound3,
                HitCount = 6
            });

            swings.Add(new OvalSwing
            {
                Duration = 115,
                XSwingRadius = 64,
                YSwingRadius = 64,
                SwingDegrees = 770,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 7),
                Sound = swingSound3,
                HitCount=6
            });
        }
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
                Duration = 22,
                XSwingRadius = 100,
                YSwingRadius = 48,
                SwingDegrees = 360,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 22,
                XSwingRadius = 100,
                YSwingRadius = 100,
                SwingDegrees = 360,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound2
            });

            //Pair 2
            swings.Add(new OvalSwing
            {
                Duration = 22,
                XSwingRadius = 100,
                YSwingRadius = 100,
                SwingDegrees = 360,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1
            });

            swings.Add(new OvalSwing
            {
                Duration = 22,
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
                Duration = 18,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 18,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound2
            });

            swings.Add(new OvalSwing
            {
                Duration = 18,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7,
                Sound = swingSound1
            });

            swings.Add(new OvalSwing
            {
                Duration = 18,
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
