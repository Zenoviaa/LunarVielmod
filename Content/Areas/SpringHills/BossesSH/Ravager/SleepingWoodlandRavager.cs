using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Ravager
{
    public class SleepingWoodlandRavager : ModNPC
    {
        private Vector2 _squishScale;
        private bool _angry;
        private ref float Timer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            Main.npcFrameCount[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            _squishScale = Vector2.One;
            NPC.width = 110;
            NPC.height = 48;
            NPC.damage = 40;
            NPC.defense = 0;
            NPC.lifeMax = 800;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
            drawPos.Y -= 50;

            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawOrigin = NPC.frame.Size() / 2;

            Vector2 drawScale = _squishScale;
            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, NPC.rotation, drawOrigin, drawScale, spriteEffects, 0);
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 30 == 0)
            {
                LegacyParticle.NewParticle<SleepParticle>(NPC.TopRight - new Vector2(30, 60), -Vector2.UnitY, Color.White);
            }
            _squishScale = Vector2.Lerp(new Vector2(1.05f, 0.95f), new Vector2(0.95f, 1.05f), MathUtil.Osc(0f, 1f));
            NPC.spriteDirection = -NPC.direction;
            NPC.velocity.X = 0;

            if (MultiplayerHelper.IsHost && _angry)
            {
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                    ModContent.NPCType<WoodlandRavager>());
                NPC.active = false;
                _angry = false;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            _angry = true;
        }
    }
}
