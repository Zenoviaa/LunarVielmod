﻿using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Terraria;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class AivanKaboom : BaseIgniterExplosion
    {
        public override int FrameCount => 22;
        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.White, endRadius: 64);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.Knockback += 4;
        }
    }
}