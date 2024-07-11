
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.NPCs.Bosses.DreadMire
{

    public class BloodCystDead : ModNPC
    {
        private bool FalseSpawn;
        private Vector2 BloodCystPos;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Cyst");
            Main.npcFrameCount[NPC.type] = 11;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.2f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        private bool text;
        public override void AI()
        {
            NPC.ai[0]++;

            if (NPC.ai[0] == 2)
            {
                BloodCystPos = NPC.position;
            }
            if (FalseSpawn)
            {
                if (NPC.ai[0] >= 130)
                {
                    Movement(BloodCystPos, 0f, 1000f, 0.02f);
                }
            }
            else
            {
                if (NPC.ai[0] >= 130)
                {
                    Movement(BloodCystPos, 0f, -1000f, 0.02f);
                }
            }

            if (NPC.ai[0] == 130)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 128f);
                if (NPC.AnyNPCs(ModContent.NPCType<DreadMire>()))
                {
                    if (!text)
                    {
                        FalseSpawn = true;
                        CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(255, 44, 44, 44),
                        "...");
                        text = true;
                    }
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && !Main.dayTime)
                    {
                        if (!text)
                        {
                            CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(255, 44, 44, 44),
                            LangText.Misc("BloodCystDead.1"));
                            text = true;
                        }
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.dayTime)
                    {
                        if (!text)
                        {
                            FalseSpawn = true;
                            CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(255, 44, 44, 44),
                            LangText.Misc("BloodCystDead.2"));
                            text = true;
                        }
                    }
                }


            }
            if (NPC.ai[0] == 300)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Laugh"));
                NPC.active = false;
                if (StellaMultiplayer.IsHost)
                { 
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMire>());
                }
            }

            float num = 1f - NPC.alpha / 255f;
            Lighting.AddLight(NPC.Center, 1.6f * num, 0.4f * num, 0.8f * num);
        }

        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override void SetDefaults()
        {
            NPC.noTileCollide = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.width = 25;
            NPC.height = 20;
            NPC.damage = 30;
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Obsidian, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Obsidian, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Obsidian, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Obsidian, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Obsidian, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Obsidian, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.DarkRed.ToVector3() * 2.25f * Main.essScale);
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            int spOff = NPC.alpha / 6;
            for (float j = -(float)Math.PI; j <= (float)Math.PI / 3f; j += (float)Math.PI / 3f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(NPC.rotation + j) - Main.screenPosition, NPC.frame, Color.FromNonPremultiplied(255 + spOff * 2, 255 + spOff * 2, 255 + spOff * 2, 100 - NPC.alpha), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, Effects, 0f);
            }

            spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, Effects, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }
}