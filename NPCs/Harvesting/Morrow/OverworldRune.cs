
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Stellamod.NPCs.Bosses.DreadMire;


namespace Stellamod.NPCs.Harvesting.Morrow
{

    public class OverworldRune : ModNPC
    {
        private int timer;
        private Vector2 BloodCystPos;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Cyst");
            Main.npcFrameCount[NPC.type] = 11;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override void AI()
        {
            NPC.damage = 0;
            NPC.ai[0]++;
            if (Main.dayTime)
            {
                if (NPC.ai[0] == 2)
                {
                    BloodCystPos = NPC.position;
                }
                if (NPC.ai[0] >= 2)
                {
                    Movement(BloodCystPos, 0f, 2030f, 0.02f);
                }

            }
            else if (!Main.bloodMoon)
            {
                if (NPC.ai[0] == 2)
                {
                    BloodCystPos = NPC.position;
                }
                if (NPC.ai[0] >= 2)
                {
                    Movement(BloodCystPos, 0f, 2030f, 0.02f);
                }

            }
            else
            {
                if (NPC.ai[0] == 2)
                {
                    BloodCystPos = NPC.position;
                }
                if (NPC.ai[0] >= 3 && NPC.ai[0] <= 50)
                {
                    Movement(BloodCystPos, 0f, 30f, 0.05f);
                }
                if (NPC.ai[0] >= 50 && NPC.ai[0] <= 100)
                {
                    Movement(BloodCystPos, 0f, 0f, 0.05f);
                }
                if (NPC.ai[0] == 100)
                {
                    NPC.ai[0] = 3;
                }
            }


            float num = 1f - NPC.alpha / 255f;
        }
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.2f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }
        public override void SetDefaults()
        {
            NPC.noTileCollide = true;
            NPC.width = 25;
            NPC.height = 20;
            NPC.damage = 1;
            NPC.defense = 19;
            NPC.lifeMax = 200;

            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath23;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;

        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.Green * num107 * .8f;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(
                GlowTexture,
                NPC.Center - Main.screenPosition + Drawoffset,
                NPC.frame,
                color1,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                effects,
                0
            );
            SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);
            for (int num103 = 0; num103 < 4; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.Green.ToVector3() * 2.25f * Main.essScale);
            return true;
        }

        public override void OnKill()  //Npc drop
        {
        }
    }
}