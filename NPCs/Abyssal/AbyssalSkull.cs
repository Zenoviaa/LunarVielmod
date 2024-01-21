
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.DropRules;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Utilis;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Abyssal
{

    public class AbyssalSkull : ModNPC
    {
        private const int TELEPORT_DISTANCE = 200;
        private float Size;
        private bool CheckSize;

        int chargetimer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shiffting Skull");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
            {
                return spawnInfo.Player.ZoneAbyss() ? 1.5f : 0f;
            }
            return 0f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            int spOff = NPC.alpha / 6;
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            for (float j = -(float)Math.PI; j <= (float)Math.PI / 2f; j += (float)Math.PI / 2f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(base.NPC.rotation + j) - Main.screenPosition, base.NPC.frame, Color.FromNonPremultiplied(255 + spOff * 2, 255 + spOff * 2, 255 + spOff * 2, 100 - base.NPC.alpha), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);
            }
            spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center - Main.screenPosition, base.NPC.frame, base.NPC.GetAlpha(lightColor), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(50, 74, 255), new Color(49, 39, 124), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        public override void SetDefaults()
        {
            NPC.width = 45;
            NPC.height = 45;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 75;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 10;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.10f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void AI()
        {
            if (!CheckSize)
            {
                Size = Main.rand.NextFloat(0.75f, 1f);
                CheckSize = true;
            }
            NPC.scale = Size;
            Player player = Main.player[NPC.target];
            chargetimer++;
            if (chargetimer >= 180 && chargetimer <= 230)
            {
                if (base.NPC.alpha < 255)
                {
                    base.NPC.alpha += 16;
                }
                else
                {

                    base.NPC.alpha = 255;
                }
            }

            if (chargetimer == 230)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 angle = Vector2.UnitX.RotateRandom(Math.PI * 2);
                    NPC.position.X = player.Center.X + (int)(TELEPORT_DISTANCE * angle.X);
                    NPC.position.Y = player.Center.Y + (int)(TELEPORT_DISTANCE * angle.Y);
                    NPC.netUpdate = true;
                }
            }

            if (chargetimer >= 230 && chargetimer <= 300)
            {
                if (base.NPC.alpha > 0)
                {
                    base.NPC.alpha -= 16;
                }
                else
                {
                    base.NPC.alpha = 0;
                }
            }

            if (chargetimer >= 600 && !player.dead)
            {
                chargetimer = 0;
                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                direction.Normalize();
                direction.X = direction.X * Main.rand.Next(8, 10);
                direction.Y = direction.Y * Main.rand.Next(8, 10);
                NPC.velocity.X = direction.X;
                NPC.velocity.Y = direction.Y;
                NPC.velocity.Y *= 0.98f;
                NPC.velocity.X *= 0.995f;
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
                NPC.netUpdate = true;
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            int d = 1;
            int d1 = DustID.BlueCrystalShard;
            for (int k = 0; k < 30; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d1, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), .74f);
            }
            if (NPC.life <= 0)
            {

                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SomethingRed, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConvulgingMater>(), minimumDropped: 1, maximumDropped: 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LunarBand>(), 50));

            LeadingConditionRule hardmodeDropRule = new LeadingConditionRule(new HardmodeDropRule());
            hardmodeDropRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<EldritchSoul>(), minimumDropped: 0, maximumDropped: 1));
            npcLoot.Add(hardmodeDropRule);
        }
    }
}