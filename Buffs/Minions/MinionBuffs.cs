using Stellamod.Items.Weapons.Summon;
using Stellamod.Projectiles.Summons.Minions;
using Stellamod.Projectiles.Summons.Sentries;
using Stellamod.Projectiles.Summons.Orbs;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;

namespace Stellamod.Buffs.Minions
{
    internal abstract class MinionBuff<T> : ModBuff where T : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<T>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    internal class ArncharMinionBuff : MinionBuff<ArncharMinionProj> { }

    internal class BucketScrapperMinionBuff : MinionBuff<BucketScrapperMinionProj> { }

    internal class AuroranSeekerMinionBuff : MinionBuff<AuroranSeekerMinionProj> { }

    internal class BabySwarmerMinionBuff : MinionBuff<BabySwarmerMinionProj> { }

    internal class ChromaCutterMinionBuff : MinionBuff<ChromaCutterMinionProj> { }

    internal class CloudMinionBuff : MinionBuff<CloudMinionProj> { }

    internal class DripplerMinionBuff : MinionBuff<DripplerMinionProj> { }

    internal class FireflyMinionBuff : MinionBuff<FireflyMinionProj>
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //Call this to keep the buff updated
            if (!SummonHelper.UpdateMinionBuff<FireflyMinionProj>(player, ref buffIndex))
                return;

            //Only work if summoner
            if (player.HeldItem.DamageType != DamageClass.Summon)
                return;

            int fireflyCount = player.ownedProjectileCounts[ModContent.ProjectileType<FireflyMinionProj>()];
            int fireflyMinionType = ModContent.ProjectileType<FireflyMinionProj>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                //Ignore projectiles that are not fireflies and are from a different owner.
                if (other.type != fireflyMinionType)
                    continue;
                if (other.owner != player.whoAmI)
                    continue;


                FireflyMinionProj fireflyMinion = other.ModProjectile as FireflyMinionProj;
                if (fireflyMinion.AttackStyle == FireflyMinionProj.AttackState.Defense_Mode)
                {
                    player.statDefense += fireflyCount * 7;
                    player.lifeRegen += fireflyCount * 3;
                    player.nightVision = true;
                    break;
                }
                else
                {
                    //player.wingTime += fireflyCount * 1;
                    player.moveSpeed += 0.1f * fireflyCount;
                    player.wingRunAccelerationMult += 0.05f * fireflyCount;
                    break;
                }
            }
        }
    }

    internal class FCMinionBuff : MinionBuff<FCMinionProj> { }

    internal class HallowWandMinionBuff : MinionBuff<HallowWandMinionProj> { }

    internal class HMMinionBuff : MinionBuff<HMArncharMinionLeftProj> { }

    internal class IceboundMinionBuff : MinionBuff<IceboundMinionProj> { }

    internal class IrradiatedCreeperMinionBuff : MinionBuff<IrradiatedCreeperMinionProj> { }

    internal class IvyakenMinionBuff : MinionBuff<IvyakenMinionProj> { }

    internal class JellyMinionBuff : MinionBuff<JellyMinionProj> { }

    internal class LifeWandMinionBuff : MinionBuff<LifeWandMinionProj> { }

    internal class ManaWandMinionBuff : MinionBuff<ManaWandMinionProj> { }

    internal class MushroomStaveMinionBuff : MinionBuff<MushroomStaveMinionProj> { }

    internal class ReflectionSeekerMinionBuff : MinionBuff<ReflectionSeekerMinionProj> { }

    internal class SolMothMinionBuff : MinionBuff<SolMothMinionProj> { }

    internal class TheBurningRodMinionBuff : MinionBuff<TheBurningRodMinionProj> { }

    internal class ToxicHornetMinionBuff : MinionBuff<ToxicHornetMinionProj> { }

    internal class VampireTorchMinionBuff : MinionBuff<VampireTorchMinionProj>
    {
        private int _vampiricTimer;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        private void SearchForTargets(Player player, out bool foundTarget, out float distanceFromTarget)
        {
            // Starting search distance
            distanceFromTarget = 700f;
            foundTarget = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    float between = Vector2.Distance(npc.Center, player.Center);
                    bool inRange = between < distanceFromTarget;
                    if (npc.CanBeChasedBy() && inRange)
                    {
                        foundTarget = true;
                        distanceFromTarget = between;
                    }
                }
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VampireTorchMinionProj>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
                player.lifeRegenCount = 0;

                //Health Loss
                SearchForTargets(player, out bool foundTarget, out float distanceFromTarget);
                if (foundTarget)
                {
                    _vampiricTimer++;
                    if (_vampiricTimer >= 9 && player.statLife > 10)
                    {
                        player.statLife += -1;
                        _vampiricTimer = 0;
                    }
                }

                if (player.GetModPlayer<VampirePlayer>().isMagic)
                {
                    player.GetDamage(DamageClass.Magic) += 0.33f;
                    player.GetModPlayer<VampirePlayer>().lifesteal = true;
                }
                else
                {
                    player.GetDamage(DamageClass.Summon) += 0.33f;
                    player.GetModPlayer<VampirePlayer>().lifesteal = true;
                }

            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    internal class VehementMinionBuff : MinionBuff<VehementMinionProj> { }

    internal class VoidMinionBuff : MinionBuff<VoidMinionProj> { }

    internal class ProbeMinionBuff : MinionBuff<ProbeMinionProj> { }

    internal class PotOfGreedMinionBuff : MinionBuff<PotOfGreedMinionProj> { }

    internal class AlcadWandMinionBuff : MinionBuff<AlcadWandMinionProj> { }

    internal class JacksonPollockMinionBuff : MinionBuff<JacksonPollockMinionProj> { }

    internal class XScissorMinionBuff : MinionBuff<XScissorMinionProj> { }

    internal class SerpentMinionBuff : MinionBuff<SerpentMinionProj> { }

    internal class PegasusMinionBuff : MinionBuff<PegasusMinionProj> { }

    internal class CentipedeMinionBuff : MinionBuff<CentipedeMinionProj> { }
}
