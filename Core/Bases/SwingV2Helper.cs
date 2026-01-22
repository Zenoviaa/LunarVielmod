using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria.Audio;

namespace Stellamod.Core.Bases
{
    public interface ISwingProjectile
    {
        void Add(ISwing swing);
    }


    public class ComboBuilder
    {
        private List<ISwing> _swings;
        public ComboBuilder()
        {
            SwordSlash1 = AssetRegistry.Sounds.Melee.NormalSwordSlash1;
            SwordSlash1.PitchVariance = 0.25f;

            SwordSlash2 = AssetRegistry.Sounds.Melee.NormalSwordSlash2;
            SwordSlash2.PitchVariance = 0.25f;

            SwordSlash3 = AssetRegistry.Sounds.Melee.SwordSpin1;
            SwordSlash3.PitchVariance = 0.5f;
                
                
            HeavySwordSlash = SoundRegistry.HeavySwordSlash1;
            HeavySwordSlash.PitchVariance = 0.5f;

            LightSpin = AssetRegistry.Sounds.Melee.LightSwordSpin1;
            LightSpin.PitchVariance = 0.5f;


            _swings = new();
        }

        public SoundStyle SwordSlash1;
        public SoundStyle SwordSlash2;
        public SoundStyle SwordSlash3;
        public SoundStyle HeavySwordSlash;
        public SoundStyle LightSpin;
        public void AddToProjectile(ISwingProjectile swingProjectile)
        {
           foreach(var swing in _swings)
            {
                swingProjectile.Add(swing);
            }
        }

        public ComboBuilder AddChakramSpin(float duration = 30, float xSwingRadius = 64, float ySwingRadius = 64, float swingDegrees = 480, int hitCount = 2)
        {
            _swings.Add(new OvalSwing
            {
                Duration = duration,
                XSwingRadius = 4,
                YSwingRadius = 4,
                SwingDegrees = swingDegrees,
                SpinThrowDistance = 32,
                SpinDegrees = 1,
                AlwaysShowTrail = true,
                Easing = (float lerpValue) => lerpValue,
                Sound = LightSpin,
                HitCount = 2
            });
            return this;
        }
        public ComboBuilder AddChakramSpin2(float duration = 30, float xSwingRadius = 128, float ySwingRadius = 64, float swingDegrees = 480, int hitCount = 2)
        {
            _swings.Add(new OvalSwing
            {
                Duration = duration,
                XSwingRadius = xSwingRadius,
                YSwingRadius = ySwingRadius,
                SwingDegrees = swingDegrees,
                SpinThrowDistance = 0,
                SpinDegrees = 32,
                AlwaysShowTrail = true,
                Easing = (float lerpValue) => lerpValue,
                Sound = LightSpin,
                HitCount = 2
            });
            return this;
        }

        public ComboBuilder AddChakramThrow(float duration = 24, float throwDistance = 180)
        {
            SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;

            _swings.Add(new ThrustSwing
            {
                Duration = duration,
                ThrowDistance = throwDistance,
                Easing = (float lerpValue) => EasingFunction.QuadraticBump(lerpValue),
                SpinDegrees = 360,
               
                Sound = spearSlash2
            });
            return this;
        }

        public ComboBuilder AddSpinningSwordSlash(float duration = 45, float xSwingRadius = 1, float ySwingRadius = 1, float swingDegrees = 720, int hitCount = 1)
        {
            _swings.Add(new OvalSwing
            {
                Duration = duration,
                XSwingRadius = xSwingRadius,
                YSwingRadius = ySwingRadius,
                SwingDegrees = swingDegrees,
                SpinThrowDistance = 0,
                SpinDegrees = 1,
                AlwaysShowTrail = true,
                Easing = (float lerpValue) => lerpValue,
                Sound = LightSpin,
                HitCount = 2
            });
            return this;
        }
        public ComboBuilder AddSwordSlash1(float duration = 18, float xSwingRadius = 80, float ySwingRadius = 48, float swingDegrees = 270, int hitCount = 1)
        {
            _swings.Add(new OvalSwing
            {
                Duration = duration,
                XSwingRadius = xSwingRadius,
                YSwingRadius = ySwingRadius,
                SwingDegrees = swingDegrees,
                HitCount = hitCount,
                Easing = EasingFunction.InOutExpo7,
                Sound = SwordSlash1,
            });
            return this;
        }

        public ComboBuilder AddSwordSlash2(float duration = 18, float xSwingRadius = 80, float ySwingRadius = 48, float swingDegrees = 270, int hitCount = 1)
        {
            _swings.Add(new OvalSwing
            {
                Duration = duration,
                XSwingRadius = xSwingRadius,
                YSwingRadius = ySwingRadius,
                SwingDegrees = swingDegrees,
                HitCount = hitCount,
                Easing = EasingFunction.InOutExpo7,
                Sound = SwordSlash2,
            });
            return this;
        }

        public ComboBuilder AddSwordSlash3(float duration = 40, float xSwingRadius = 100, float ySwingRadius = 40, float swingDegress=540, int hitCount = 1)
        {
            _swings.Add(new OvalSwing
            {
                Duration = duration,
                XSwingRadius = xSwingRadius,
                YSwingRadius = ySwingRadius,
                SwingDegrees = swingDegress,
                HitCount = hitCount,
                Easing = EasingFunction.InOutExpo7,
                Sound = SwordSlash3
            });
            return this;
        }

        public ComboBuilder AddSpearSlash1(float duration = 22, float xSwingRadius = 100, float ySwingRadius = 50, float swingDegrees = 90)
        {
            SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;
            _swings.Add(new OvalSwing
            {
                Duration = duration,
                XSwingRadius = xSwingRadius,
                YSwingRadius = ySwingRadius,
                SwingDegrees = swingDegrees,
                Easing = (float lerpValue) => EasingFunction.Anticipation(lerpValue),
                Sound = spearSlash1,
            });
            return this;
        }
        public ComboBuilder AddSpearThrust1(float duration = 15, float throwDistance = 120)
        {
            SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;

            _swings.Add(new ThrustSwing
            {
                Duration = duration,
                ThrowDistance = throwDistance,
                Easing = (float lerpValue) => EasingFunction.QuadraticBump(lerpValue),
                Sound = spearSlash2
            });
            return this;
        }
        public ComboBuilder AddSpearSpin1(float duration = 60, float swingDegrees = 360 * 4, float xSwingRadius = 64, float ySwingRadius = 64, int hitCount = 8)
        {
            SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            spearSlash1.PitchVariance = 0.25f;
            spearSlash2.PitchVariance = 0.25f;
            nSpin.PitchVariance = 0.2f;

            _swings.Add(new OvalSwing
            {
                Duration = duration,
                SwingDegrees = swingDegrees,
                XSwingRadius = xSwingRadius,
                YSwingRadius = ySwingRadius,
                HitCount = hitCount,
                Easing = (float lerpValue) => lerpValue,
                Sound = nSpin
            });

            return this;
        }
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
                Easing = EasingFunction.Anticipation,
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
                Easing = EasingFunction.Anticipation,
                Sound = swingSound2,
            });

            swings.Add(new OvalSwing
            {
                Duration = 70,
                XSwingRadius = 84 / 1.5f,
                YSwingRadius = 70 / 1.5f,
                SwingDegrees = 270,
                Easing = EasingFunction.Anticipation,
                Sound = swingSound2,
            });

            swings.Add(new OvalSwing
            {
                XSwingRadius = 64,
                YSwingRadius=64,
                Duration = 48,
                SwingDegrees = 330,
                Easing = EasingFunction.Anticipation,
                Sound = swingSound1,
            });

            swings.Add(new OvalSwing
            {
                XSwingRadius = 64,
                YSwingRadius = 64,
                Duration = 96,
                SwingDegrees = 330,
                Easing = EasingFunction.Anticipation,
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
                Easing = EasingFunction.Anticipation,
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
                Easing = (float lerpValue) => EasingFunction.Anticipation(lerpValue),
                Sound = spearSlash1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 22,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = (float lerpValue) => EasingFunction.Anticipation(lerpValue),
                Sound = spearSlash1,
            });


            swings.Add(new ThrustSwing
            {
                Duration = 15,
                ThrowDistance = 120,
                Easing = (float lerpValue) => EasingFunction.QuadraticBump(lerpValue),
                Sound = spearSlash2
            });


            swings.Add(new ThrustSwing
            {
                Duration = 15,
                ThrowDistance = 120,
                Easing = (float lerpValue) => EasingFunction.QuadraticBump(lerpValue),
                Sound = spearSlash2
            });


            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = (float lerpValue) => EasingFunction.Anticipation(lerpValue),
                Sound = spearSlash1,
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 100,
                YSwingRadius = 50,
                SwingDegrees = 90,
                Easing = (float lerpValue) => EasingFunction.Anticipation(lerpValue),
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
                Duration = 32,
                ThrowDistance = 128,
                Easing = (float lerpValue) => EasingFunction.QuadraticBump(lerpValue),
                Sound = spearSlash2
            });

            swings.Add(new ThrustSwing
            {
                Duration = 60,
                ThrowDistance = 200,
                Easing = (float lerpValue) => EasingFunction.QuadraticBump(lerpValue),
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
