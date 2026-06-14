using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles
{
    public class RekFireBlowtorchProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        //Ai
        private ref float Timer => ref Projectile.ai[0];
        private NPC Owner => Main.npc[(int)Projectile.ai[1]];
        private float BlowtorchDistance => 384;

        //Draw Code
        private Vector2[] LinePos;
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 720;
            Projectile.hide = true;
            LinePos = new Vector2[5];
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BlowtorchContinous"), Projectile.position);
            }

            Projectile.Center = Owner.Center;
            Projectile.velocity = Owner.rotation.ToRotationVector2();
            if (Projectile.scale < 1f || Timer <= 1)
            {
                Projectile.scale = MathF.Sin(Timer / 600f * MathHelper.Pi) * 3f;
                if (Projectile.scale > 1f)
                    Projectile.scale = 1f;
            }

            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * BlowtorchDistance, i / 8f));
            }
            LinePos = points.ToArray();
        }

        public override bool ShouldUpdatePosition()
        {
            //Returning false makes velocity not move the projectile
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GothivianFlames>(), 4 * 70);
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = LinePos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }


            //Return false to not use default collision
            return false;
        }


        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCs.Add(index);
        }
    }
}
