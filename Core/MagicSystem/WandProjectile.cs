using Stellamod.Assets;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.SwingSystem;
using System.Collections.Generic;
using Terraria.Audio;

namespace Stellamod.Core.MagicSystem
{
    internal class WandProjectile : BaseSwingProjectile
    {
        public SlashTrailer SlashTrailer { get; set; }
        public override void SetDefaults2()
        {
            base.SetDefaults2();

        }

        public override void DefineCombo(List<ISwing> swings)
        {
            base.DefineCombo(swings);
            SlashTrailer = new();
            Trailer = SlashTrailer;

            SoundStyle swingSound1 = AssetRegistry.Sounds.Melee.NormalSwordSlash1;
            swingSound1.PitchVariance = 0.25f;

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1,
            });
        }
    }
}
