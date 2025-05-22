using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.RoyalCapital
{
    public class AlcaricVoidBig : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 60;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 99;
            NPC.height = 79;
            NPC.damage = 1000;
            NPC.defense = 30;
            NPC.lifeMax = 1600;
            NPC.HitSound = SoundID.NPCHit56;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = 560f;
            NPC.knockBackResist = .45f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.scale = 2;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<AlcadziaBiome>())
            {
                return 0.3f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 11; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 1, -1f, 1, default, .61f);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
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
        }

        public override void AI()
        {
            ref float ai_Timer = ref NPC.ai[0];
            ref float ai_X = ref NPC.ai[1];
            ref float ai_Y = ref NPC.ai[2];
            NPC.TargetClosest();
            ai_Timer++;
            if (ai_Timer > 120)
            {
                float range = 128;
                ai_X = NPC.Center.X + Main.rand.NextFloat(-range, range);
                ai_Y = NPC.Center.Y + Main.rand.NextFloat(-range, range);
                NPC.netUpdate = true;
                ai_Timer = 0;
            }

            Vector2 positionToMoveTo = new Vector2(ai_X, ai_Y);
            AI_Movement(positionToMoveTo, 1, 0.02f);
        }

        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.GreenYellow.ToVector3() * 1.25f * Main.essScale);
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 5, NPC.height - NPC.frame.Height + 3);
            Vector2 DrawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, 2).RotatedBy(radians) * time, NPC.frame, new Color(53, 10, 112, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, 4).RotatedBy(radians) * time, NPC.frame, new Color(152, 2, 255, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(190, 50, 250), new Color(72, 13, 255), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcaricMush>(), 1, 2, 4));
        }
    }
}