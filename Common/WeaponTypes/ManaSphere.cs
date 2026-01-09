using Microsoft.Xna.Framework;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes
{
    public class ManaSphereGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isManaSphere;
        public int heldProj;
    }

    public class ManaSphereExpandingTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            ManaSphereGlobalItem manaSphere = item.GetGlobalItem<ManaSphereGlobalItem>();
            if (manaSphere.isManaSphere)
            {
                TooltipLine manaSphereHelp = new TooltipLine(Mod, "ManaSphere", LangText.Common("ManaSphereHelp"));
                lines.Add(manaSphereHelp);
            }
        }
    }
    public abstract class AbstractManaSphereHold : ModProjectile
    {
        protected ref float Timer => ref Projectile.ai[0];
        protected Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public sealed override void AI()
        {
            base.AI();
            Item item = Owner.HeldItem;
            if (item.IsAir || item == null)
                return;
            ManaSphereGlobalItem manaSphere = item.GetGlobalItem<ManaSphereGlobalItem>();
            if (manaSphere.heldProj != Type)
                return;
            Projectile.timeLeft = 2;
            Timer++;
            AI_OrbitPlayer();
        }

        public virtual void AI_OrbitPlayer()
        {
            Vector2 offset = Vector2.UnitY.RotatedBy(Timer * 0.05f) * 64 * ExtraMath.Osc(0.9f, 1f);
            Vector2 positionToMoveTo = Owner.Center + offset;
            Vector2 targetVelocity = positionToMoveTo - Projectile.Center;
            Projectile.velocity = targetVelocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }

    public class ManaSpherePlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            Item item = Player.HeldItem;
            if (item.IsAir || item == null)
                return;
            ManaSphereGlobalItem manaSphere = item.GetGlobalItem<ManaSphereGlobalItem>();
            if (!manaSphere.isManaSphere)
                return;
            if (Player.whoAmI == Main.myPlayer && Player.ownedProjectileCounts[manaSphere.heldProj] == 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, manaSphere.heldProj, 1, 1, Player.whoAmI);
            }
        }
    }
}
