using Stellamod.Assets;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.SwingSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
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
                Sound  = swingSound2
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
        }
    }
}
