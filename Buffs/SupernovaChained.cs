using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Chains;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class SupernovaChained : ModBuff
    {  
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {

            if (StellaMultiplayer.IsHost && !HasChains(npc))
            {
                int npcIndexToFollow = npc.whoAmI;
                Vector2 velocity = Vector2.UnitY * 0.01f;
                velocity = velocity.RotatedBy(MathHelper.Pi + MathHelper.PiOver4);
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity,
                    ModContent.ProjectileType<SupernovaChainCircle>(), 0, 0, Main.myPlayer, npcIndexToFollow);

                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity,
                    ModContent.ProjectileType<SupernovaChainFront>(), 0, 0, Main.myPlayer, npcIndexToFollow);
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity,
                    ModContent.ProjectileType<SupernovaChainBack>(), 0, 0, Main.myPlayer, npcIndexToFollow);

                velocity = velocity.RotatedBy(MathHelper.PiOver2 / 2);
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity,
                    ModContent.ProjectileType<SupernovaChainFront>(), 0, 0, Main.myPlayer, npcIndexToFollow);
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity,
                    ModContent.ProjectileType<SupernovaChainBack>(), 0, 0, Main.myPlayer, npcIndexToFollow);
            }
        }

        private bool HasChains(NPC npc)
        {
            if (!npc.active)
                return true;
            for(int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.ai[0] != npc.whoAmI)
                    continue;
                if (!p.active)
                    continue;

                if(p.type == ModContent.ProjectileType<SupernovaChainBack>() || 
                    p.type == ModContent.ProjectileType<SupernovaChainFront>())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
