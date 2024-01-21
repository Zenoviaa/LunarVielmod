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
            float chance = SpawnCondition.OverworldNightMonster.Chance * 1.3f;
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
            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            Lighting.AddLight(NPC.Center, Color.LightSkyBlue.ToVector3() * 0.25f * Main.essScale);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 0, NPC.height - NPC.frame.Height + 0);
            Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, NPC.frame, new Color(100, 70, 255, 50), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(140, 210, 255, 77), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            return true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {

            if (NPC.life <= 0)
            {
            }
            int d = 180;
            for (int k = 0; k < 9; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.Green, 0.7f);
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WinterbornShard>(), minimumDropped: 1, maximumDropped: 3));
        }
    }
}
