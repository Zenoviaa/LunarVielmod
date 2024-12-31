using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Cinderspark
{
    internal class CharredSoul : ModNPC
    {
        private ref float ai_Counter => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8; // The amount of frames the NPC has
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 22;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.lifeMax = 50;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 1;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.25f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(Color.OrangeRed, Color.Transparent, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.OrangeRed.ToVector3(),
                Color.Red.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.OrangeRed, Color.White, 0);
            Lighting.AddLight(screenPos, Color.OrangeRed.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player
            if (NPC.Center.X < targetCenter.X && NPC.velocity.X < moveSpeed)
            {
                NPC.velocity.X += accel;
            }
            else if (NPC.Center.X > targetCenter.X && NPC.velocity.X > -moveSpeed)
            {
                NPC.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            if (NPC.Center.Y < targetCenter.Y && NPC.velocity.Y < moveSpeed)
            {
                NPC.velocity.Y += accel;
            }
            else if (NPC.Center.Y > targetCenter.Y && NPC.velocity.Y > -moveSpeed)
            {
                NPC.velocity.Y -= accel;
            }

            float targetRotation = NPC.DirectionTo(targetCenter).ToRotation();
            NPC.rotation = MathHelper.WrapAngle(MathHelper.Lerp(NPC.rotation, targetRotation, 0.33f));
        }

        public override void AI()
        {
            ai_Counter++;
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
           
            AI_Movement(target.Center, 5, accel: 0.05f);
            if(ai_Counter % 3 == 0)
            {
                float speedX = Main.rand.NextFloat(-1f, 1f);
                float speedY = Main.rand.NextFloat(-1f, 1f);
                float scale = Main.rand.NextFloat(0.5f, 0.75f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 
                    DustID.InfernoFork, speedX, speedY, Scale: scale);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<CindersparkBiome>())
            {
                return 0.02f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void OnKill()
        {
            for (int i = 0; i < 16; i++)
            {
                float speedX = Main.rand.NextFloat(-1f, 1f);
                float speedY = Main.rand.NextFloat(-1f, 1f);
                float scale = Main.rand.NextFloat(0.66f, 1f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork,
                    speedX, speedY, Scale: scale);
            }
        }
    }
}
