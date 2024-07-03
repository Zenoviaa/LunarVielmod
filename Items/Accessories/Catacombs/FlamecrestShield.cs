using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Catacombs.Fire;
using Stellamod.NPCs.Catacombs.Fire.BlazingSerpent;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class FlamecrestPlayer : ModPlayer
    {
        public bool hasFlamecrestShield;
        public override void ResetEffects()
        {
            hasFlamecrestShield = false;
        }

        private void BlockVisuals()
        {
            SoundEngine.PlaySound(SoundID.NPCHit42, Player.position);
            SoundEngine.PlaySound(SoundID.Item45, Player.position);

            int combatText = CombatText.NewText(Player.getRect(), Color.OrangeRed, LangText.Misc("FlamecrestPlayer"), true);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 60;

            for (int t = 0; t < 32; t++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                ParticleManager.NewParticle(Player.Center, speed, ParticleManager.NewInstance<UnderworldParticle1>(),
                    default(Color), 1 / 3f);
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (hasFlamecrestShield)
            {
                int[] resistedNPCs = new int[]
                {
                    NPCID.BurningSphere,
                    NPCID.LavaSlime,
                    NPCID.Hellbat,
                    NPCID.Demon,
                    NPCID.VoodooDemon,
                    NPCID.BlazingWheel,
                    NPCID.Lavabat,
                    NPCID.RedDevil,
                    NPCID.HellArmoredBones,
                    NPCID.HellArmoredBonesMace,
                    NPCID.HellArmoredBonesSpikeShield,
                    NPCID.HellArmoredBonesSword,
                    NPCID.SolarCrawltipedeBody,
                    NPCID.SolarCrawltipedeHead,
                    NPCID.SolarCrawltipedeTail,
                    NPCID.SolarDrakomire,
                    NPCID.SolarDrakomireRider,
                    NPCID.SolarSroller,
                    NPCID.SolarCorite,
                    NPCID.SolarSolenian,
                    NPCID.SolarFlare,
                    NPCID.SolarSpearman,
                    NPCID.SolarGoop,
                    NPCID.LunarTowerSolar,
                    NPCID.TorchGod,
                    ModContent.NPCType<PandorasFlamebox>(),
                    ModContent.NPCType<PandorasGuard>(),
                    ModContent.NPCType<PandorasKnife>(),
                    ModContent.NPCType<PandorasSeeker>(),
                    ModContent.NPCType<BlazingSerpentHead>(),
                    ModContent.NPCType<BlazingSerpentBody>(),
                    ModContent.NPCType<BlazingSerpentTail>()
                };

                for (int i = 0; i < resistedNPCs.Length; i++)
                {
                    if (npc.type == resistedNPCs[i])
                    {
                        //50% less damage from fire-based sources
                        modifiers.FinalDamage *= 0.5f;
                        BlockVisuals();
                        break;
                    }
                }
            }

        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (hasFlamecrestShield)
            {
                int[] resistedProjectiles = new int[]
                {
                    ProjectileID.Flames,
                    ProjectileID.FlamethrowerTrap,
                    ProjectileID.Fireball,
                    ProjectileID.EyeBeam,
                    ProjectileID.FlamingScythe,
                    ProjectileID.CultistBossFireBall,
                    ProjectileID.CultistBossFireBallClone,
                    ProjectileID.DD2BetsyFireball,
                    ProjectileID.DD2BetsyFlameBreath,
                    ProjectileID.GreekFire1,
                    ProjectileID.GreekFire2,
                    ProjectileID.GreekFire3,
                    ProjectileID.InfernoHostileBlast,
                    ProjectileID.InfernoHostileBolt,
                    ModContent.ProjectileType<HeatBeam>()
                };

                for (int i = 0; i < resistedProjectiles.Length; i++)
                {
                    if (proj.type == resistedProjectiles[i])
                    {
                        //50% less damage from fire-based sources
                        modifiers.FinalDamage *= 0.5f;
                        BlockVisuals();
                        break;
                    }
                }
            }   
        }
    }

    internal class FlamecrestShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FlamecrestPlayer>().hasFlamecrestShield = true;
            player.ClearBuff(BuffID.OnFire);
            player.ClearBuff(BuffID.OnFire3);
            player.ClearBuff(BuffID.ShadowFlame);
            player.ClearBuff(BuffID.CursedInferno);
            player.ClearBuff(BuffID.Burning);
        }
    }
}
