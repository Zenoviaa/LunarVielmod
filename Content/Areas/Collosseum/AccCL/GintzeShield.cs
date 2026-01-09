using Microsoft.Xna.Framework;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Accord.Math.FourierTransform;

namespace Stellamod.Content.Areas.Collosseum.AccCL
{
    public class GintzeShield : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToShield(ModContent.ProjectileType<GintzeShieldHeld>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<GintzlMetal, BlankCard>();
        }
    }

    public class GintzeShieldHeld : AbstractShieldProjectile
    {
        public override void OnBlockMovement(NPC npc)
        {
            base.OnBlockMovement(npc);
            if (npc.boss)
                return;
            if (!npc.HasBuff<GintzeStanceBreak>())
            {
                for(float f = 0; f < 3; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                    DustParticle.Spawn(npc.Center, vel);
                }

                var strike = LegacyParticle.NewParticle<GlowDonutParticle>(npc.Center, Vector2.UnitY.RotatedByRandom(0.5f));
                strike.xMult = 6;
                strike.Scale *= 0.2f;
                strike.rotOffset += MathHelper.PiOver2;

                SoundStyle slashSound = new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlash");
                slashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(slashSound, npc.position);
                npc.AddBuff(ModContent.BuffType<GintzeStanceBreak>(), 60000);
            }
        }
    }

    public class GintzeStanceBreak : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.life > npc.lifeMax * 0.9f)
                npc.life = (int)(npc.lifeMax * 0.9f);
        }
    }
}
