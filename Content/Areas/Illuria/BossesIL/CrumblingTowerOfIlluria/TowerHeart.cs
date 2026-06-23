using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria
{
    public class TowerHeart : ModNPC
    {
        private float _immuneTimer;
        private ref float Timer => ref NPC.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)NPC.ai[1]];
        }

        private float OrbitRadiusX => NPC.ai[2];
        private float OrbitRadiusY => NPC.ai[3];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 100;
            NPC.defense = 33;
            NPC.lifeMax = 2000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (Parent.ai[1] == 5)
            {
                _immuneTimer = 180;
     
            }

            if(_immuneTimer > 0)
            {
                Timer += 0.02f;
                _immuneTimer--;
            }

                Timer += 0.01f;
            float radians = Timer * 0.015f;
            float x = MathF.Sin(Timer) * OrbitRadiusX;
            float y = MathF.Cos(Timer) * OrbitRadiusY;

            Vector2 offset = new Vector2();
            offset.X = x;
            offset.Y = y;
            offset.Y += MathF.Sin(Timer * 0.02f) * 4f;
            Vector2 positionToOrbit = Parent.Center + offset;
            Vector2 velocityToOrbit = positionToOrbit - NPC.Center;
            NPC.velocity = velocityToOrbit;
            NPC.rotation = NPC.velocity.X * 0.02f;
        }

        public override void OnKill()
        {
            base.OnKill();
            NPC.HitInfo hitInfo = NPC.CalculateHitInfo(Parent.lifeMax / 16, 1, true, 0, DamageClass.Generic);
            NPC.StrikeNPC(hitInfo, fromNet: false);
            NetMessage.SendStrikeNPC(Parent, hitInfo);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CrumblingSoul>(), 1, 1, Main.myPlayer,
                ai1: Parent.whoAmI);
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 drawPosition, Color drawColor)
        {
            if (NPC.dontTakeDamage)
            {
                drawColor = Color.Lerp(drawColor, Color.Black, 0.5f);
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int trailLength = NPC.oldPos.Length;
            for (int i = 0; i < trailLength; i++)
            {
                float f = i;
                float numAfterImages = trailLength;
                float completionRatio = f / numAfterImages;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                afterImageColor *= 0.2f;

                Vector2 drawPosition = NPC.oldPos[i] + NPC.Size / 2f;
                DrawSprite(spriteBatch, drawPosition - screenPos, afterImageColor);
            }
            DrawSprite(spriteBatch, NPC.Center - screenPos, drawColor);
            return false;
        }
    }
}
