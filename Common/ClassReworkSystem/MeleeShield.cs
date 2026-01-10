using Microsoft.Xna.Framework;
using Stellamod.Core.NPCHelpers;
using Stellamod.Helpers;
using Stellamod.Projectiles.Paint;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.GameContent;
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
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (Projectile.Center.X < Owner.Center.X)
                spriteEffects |= SpriteEffects.FlipVertically;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Rectangle drawFrame = Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            float scale = Projectile.scale;
            float rotation = Projectile.rotation;
            spriteBatch.Draw(texture, drawPos, drawFrame, Color.White.MultiplyRGB(lightColor), rotation, drawOrigin, scale, spriteEffects, 0);
            return false;
        }
    }
}
