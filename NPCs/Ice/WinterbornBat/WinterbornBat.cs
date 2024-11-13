using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Ice.WinterbornBat
{
    internal class WinterbornBat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Winterborn Slime");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float chance = SpawnCondition.Overworld.Chance + SpawnCondition.Underground.Chance + SpawnCondition.Cavern.Chance;
            if (!spawnInfo.Player.ZoneSnow)
                return 0f;
            return chance;
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.1f;
            if (NPC.frameCounter >= 6)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 4)
            {
                frame = 0;
            }
            NPC.frame.Y = frameHeight * frame;
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 28;
            NPC.defense = 3;
            NPC.lifeMax = 40;
            NPC.damage = 13;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.value = 60f;
            NPC.knockBackResist = 0.65f;
            NPC.aiStyle = 14;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.position - screenPos;
            SpriteEffects spriteEffects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float f = 0; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 off = rot.ToRotationVector2() * VectorHelper.Osc(1, 3);
                Vector2 glowDrawPos = NPC.Center + off - screenPos;
                glowDrawPos.Y += 4;
                Color glowColor = Color.White;
                spriteBatch.Draw(texture, glowDrawPos, NPC.frame, glowColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //   spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, Color.Black, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int d = 180;
            for (int k = 0; k < 9; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.Green, 0.7f);
            }
        }

        float alphaCounter;
        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            float num = 1f - NPC.alpha / 255f;
            alphaCounter += 0.04f;
            NPC.rotation = NPC.velocity.X * 0.03f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WinterbornShard>(), minimumDropped: 2, maximumDropped: 4));
        }
    }
}
