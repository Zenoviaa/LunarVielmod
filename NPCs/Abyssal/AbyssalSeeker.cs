

using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Utilities;
using Stellamod.Utilis;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;


namespace Stellamod.NPCs.Abyssal
{

    public class AbyssalSeeker : ModNPC
    {
        private int timer;
        protected float speed = 2f;
        protected float acceleration = 0.1f;
        protected float speedY = 1.5f;
        protected float accelerationY = 0.04f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Abyssal Seeker");
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
            {
                return spawnInfo.Player.ZoneAbyss() && Main.hardMode ? 1.0f : 0f;
            }


            return 0f;
        }
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(1, 4), false, 0, false, false);
            if (Main.rand.Next(30) == 0)
            {
                Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ModContent.ItemType<LunarBand>(), 1, false, 0, false, false);
            }

        }
        public override void SetDefaults()
        {

            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 80;
            NPC.defense = 18;
            NPC.lifeMax = 240;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath29;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 10;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            Main.npcFrameCount[NPC.type] = 3;
            AIType = NPCID.GreenSlime;  //npc behavior
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.Next(1) == 0)
                target.AddBuff(Mod.Find<ModBuff>("AbyssalFlame").Type, 200);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(50, 74, 255), new Color(49, 39, 124), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        public override bool PreAI()
        {

            if (Main.rand.Next(3) == 1)
            {
                int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, 206, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                int dust1 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, 206, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust1].velocity *= 0f;
                Main.dust[dust].scale = 2.5f;
                Main.dust[dust1].scale = 2.5f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust1].noGravity = true;
            }
            return true;
        }
    }
}