using Microsoft.Xna.Framework;
using Stellamod.Core.NPCHelpers;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ClassReworkSystem
{
    public class MeleeShield : ModProjectile
    {
        protected Player Owner => Main.player[Projectile.owner];
        protected ref float Timer => ref Projectile.ai[0];
        protected ref float HoldRotation => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            const float holdDistance = 28;
            const float radius = 32;

            ClassReworkPlayer classReworkPlayer = Owner.GetModPlayer<ClassReworkPlayer>();
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 holdVelocity = (Main.MouseWorld - Owner.Center);
                HoldRotation = holdVelocity.ToRotation();
                Projectile.netUpdate = true;
            }
            if (classReworkPlayer.heldShield == Type && classReworkPlayer.playerClass == PlayerClass.Melee)
            {
                Projectile.timeLeft = 2;

            }


                Projectile.rotation = HoldRotation;
            Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * holdDistance;

            Rectangle myRect = Projectile.getRect();

            Vector2 center = Projectile.Center;
            Vector2 direction = HoldRotation.ToRotationVector2();

            Vector2 bottom = center + direction.RotatedBy(-MathHelper.PiOver2) * radius;
            Vector2 top = center + direction.RotatedBy(MathHelper.PiOver2) * radius;

            Point p1 = bottom.ToPoint();
            Point p2 = top.ToPoint();
            foreach (var npc in Main.ActiveNPCs)
            {
                if (NPCSets.Heavy[npc.type])
                    continue;
                if (npc.friendly)
                    continue;
                if (npc.CountsAsACritter)
                    continue;
                if (npc.boss)
                    continue;
                if (npc.type == NPCID.TargetDummy)
                    continue;

                Rectangle targetRect = npc.getRect();

                if (CollisionHelper.LineIntersectsRect(p1, p2, targetRect))
                {
                    OnBlockMovement(npc);
                }
            }
        }

        public virtual void OnBlockMovement(NPC npc)
        {
            Vector2 pushVelocity = (npc.Center - Owner.Center).SafeNormalize(Vector2.Zero);
            pushVelocity += (Owner.position - Owner.oldPosition);
            npc.velocity = pushVelocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            this.DrawCentered(ref lightColor);
            return false;
        }
    }
}
