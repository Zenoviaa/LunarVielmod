using Microsoft.Xna.Framework;
using Stellamod.Common.ClassReworkSystem;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS
{
    public class HeatedShield : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToShield(ModContent.ProjectileType<HeatedShieldHeld>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankCard>();
        }
    }


    public class HeatedShieldPlayer : ModPlayer
    {
        public bool isShieldActive
        {
            get
            {
                ClassReworkPlayer reworkPlayer = Player.GetModPlayer<ClassReworkPlayer>();
                return reworkPlayer.heldShield == ModContent.ProjectileType<HeatedShieldHeld>();
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (!isShieldActive)
                return;
            bool isResisted = NPCSets.ResistedByFlamecrestShield[npc.type];
            if (!isResisted)
                return;
            modifiers.FinalDamage *= 0.5f;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (!isShieldActive)
                return;
            bool isResisted = ProjectileSets.ResistedByFlamecrestShield[proj.type];
            if (!isResisted)
                return;

            modifiers.FinalDamage *= 0.5f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            ClassReworkPlayer reworkPlayer = Player.GetModPlayer<ClassReworkPlayer>();
            if(reworkPlayer.heldShield == ModContent.ProjectileType<HeatedShieldHeld>() && target.HasBuff<CinderFlame>())
            {
                modifiers.FinalDamage += 0.2f;
            }
        }
    }

    public class CinderFlame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 10;
            if (Main.rand.NextBool(3))
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.Red, Main.rand.NextFloat(0.9f, 1.5f));
                sp.initialColor = Color.Lerp(Color.Red, Color.OrangeRed, Main.rand.NextFloat(0f, 1f)) * 0.4f;
                sp.expand = true;
            }
            if (Main.rand.NextBool(3))
            {
                LegacyParticle.NewParticle<EmberParticle>(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.Red, Main.rand.NextFloat(0.9f, 1.5f));
            }
            if (Main.rand.NextBool(8))
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Yellow,
                    outerColor = Color.Red
                };
                DustParticle.Spawn(npc.Center, -Vector2.UnitY.RotatedByRandom(2f) * Main.rand.NextFloat(0.5f, 5f), spawnParams);
            }
        }
    }

    public class HeatedShieldHeld : AbstractShieldProjectile
    {
        public override void OnBlockMovement(NPC npc)
        {
            base.OnBlockMovement(npc);
            npc.AddBuff(ModContent.BuffType<CinderFlame>(), 100000);
        }
    }
}
