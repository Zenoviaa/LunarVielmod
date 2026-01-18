using Microsoft.Xna.Framework;
using Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;
using Stellamod.Content.Armors.ForestCore;
using Stellamod.Helpers;
using Stellamod.Projectiles.Summons.Minions;
using Stellamod.Projectiles.Summons.Sentries;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Minions
{
    public abstract class MinionBuff<T> : ModBuff where T : ModProjectile
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
    public class RevengenceMinionBuff : MinionBuff<RevengenceMinion> { }
    public class StringMinionBuff : MinionBuff<StringMinion> { }

    public class BucketScrapperMinionBuff : MinionBuff<BucketScrapperMinionProj> { }

    public class AuroranSeekerMinionBuff : MinionBuff<AuroranSeekerMinionProj> { }

    public class BabySwarmerMinionBuff : MinionBuff<BabySwarmerMinionProj> { }

    public class ChromaCutterMinionBuff : MinionBuff<ChromaCutterMinionProj> { }

    public class CloudMinionBuff : MinionBuff<CloudMinionProj> { }

    public class DripplerMinionBuff : MinionBuff<DripplerMinionProj> { }

    public class FireflyMinionBuff : MinionBuff<LilFly>
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //Call this to keep the buff updated
            if (!SummonHelper.UpdateMinionBuff<LilFly>(player, ref buffIndex))
                return;

            //Only work if summoner
            if (player.HeldItem.DamageType != DamageClass.Summon)
                return;

            int fireflyCount = player.ownedProjectileCounts[ModContent.ProjectileType<LilFly>()];
            int fireflyMinionType = ModContent.ProjectileType<LilFly>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                //Ignore projectiles that are not fireflies and are from a different owner.
                if (other.type != fireflyMinionType)
                    continue;
                if (other.owner != player.whoAmI)
                    continue;

                if (other.ai[1] == (float)LilFly.AIState.Defense)
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

    public class FCMinionBuff : MinionBuff<ForestCoreBow> { }

    public class HallowWandMinionBuff : MinionBuff<HallowWandMinionProj> { }


    public class IrradiatedCreeperMinionBuff : MinionBuff<IrradiatedCreeperMinionProj> { }

    public class JellyMinionBuff : MinionBuff<JellyMinionProj> { }

    public class TheBurningRodMinionBuff : MinionBuff<TheBurningRodMinionProj> { }

    //  public class ToxicHornetMinionBuff : MinionBuff<ToxicHornetMinionProj> { }

    public class VampireTorchMinionBuff : MinionBuff<VampireTorchMinionProj>
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
                player.statLifeMax2 /= 2;
                player.lifeRegenCount = 0;
                _vampiricTimer++;
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (!npc.CanBeChasedBy())
                        continue;

                    float distanceToNpc = Vector2.Distance(player.Center, npc.Center);
                    if (distanceToNpc < 320)
                    {
                        if (_vampiricTimer % 24 == 0)
                        {
                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.Heal(Main.rand.Next(2, 4));
                            }

                        }
                        npc.AddBuff(ModContent.BuffType<VampiricFlames>(), 10);
                    }
                }
                player.GetDamage(DamageClass.Summon) += 0.3f;
                player.GetDamage(DamageClass.Magic) += 0.3f;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public class VoidMinionBuff : MinionBuff<VoidMinionProj> { }

    public class ProbeMinionBuff : MinionBuff<ProbeMinionProj> { }

    public class PotOfGreedMinionBuff : MinionBuff<PotOfGreedMinionProj> { }

    public class AlcadWandMinionBuff : MinionBuff<AlcadWandMinionProj> { }

    public class JacksonPollockMinionBuff : MinionBuff<JacksonPollockMinionProj> { }

    public class XScissorMinionBuff : MinionBuff<XScissorMinionProj> { }

    public class SerpentMinionBuff : MinionBuff<SerpentMinionProj> { }

    public class PegasusMinionBuff : MinionBuff<PegasusMinionProj> { }

    public class CentipedeMinionBuff : MinionBuff<CentipedeMinionProj> { }
}
