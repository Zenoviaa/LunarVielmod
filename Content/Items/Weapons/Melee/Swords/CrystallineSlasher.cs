using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.SwingSystem;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Melee.Swords
{
    internal class CrystallineSlasher : BaseSwingItem
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CrystallineSwordSlash>();
        }
    }

    internal class CrystallineSwordSlash : BaseSwingProjectile
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
            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7
            });

            swings.Add(new OvalSwing
            {
                Duration = 24,
                XSwingRadius = 80,
                YSwingRadius = 48,
                SwingDegrees = 270,
                Easing = EasingFunction.InOutExpo7
            });

            swings.Add(new OvalSwing
            {
                Duration = 40,
                XSwingRadius = 80,
                YSwingRadius = 100,
                SwingDegrees = 360,
                Easing = EasingFunction.InOutExpo7
            });
        }
    }
}
