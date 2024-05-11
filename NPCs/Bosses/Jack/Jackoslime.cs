
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Utilis;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Bosses.Jack
{

    public class Jackoslime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 24;
            NPC.damage = 8;
            NPC.defense = 0;
            NPC.lifeMax = 40;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.alpha = 0;
            NPC.knockBackResist = .75f;
            NPC.aiStyle = 1;
            AIType = NPCID.BlueSlime;
            AnimationType = NPCID.BlueSlime;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
            {
                return spawnInfo.Player.ZoneFable() ? 1.6f : 0f;
            }

            if (spawnInfo.Player.InModBiome<MorrowUndergroundBiome>())
            {
                return SpawnCondition.Underground.Chance * 0.5f;
            }

            return 0f;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
            NPC.spriteDirection = NPC.direction;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 50; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    {
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[num].noGravity = false;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Lighting.AddLight(NPC.Center, Color.Orange.ToVector3() * 2.25f * Main.essScale);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 255, 113), new Color(232, 111, 24), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}