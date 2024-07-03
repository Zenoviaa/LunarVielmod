using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Harvesting.Morrow
{

    public class DesertRune : ModNPC
    {
        private Vector2 BloodCystPos;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Cyst");
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneDesert)
            {
                return SpawnCondition.Overworld.Chance * SpawnRates.Rune_Spawn_Chance * 2.2f;
            }

            return SpawnCondition.DesertCave.Chance * SpawnRates.Rune_Spawn_Chance * 2f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Hit = true;
            if(NPC.life <= 0)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 16f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Harv1"));
                CombatText.NewText(NPC.getRect(), Color.Orange, LangText.Misc("DesertRune"), true, false);
            }
        }

        public override void AI()
        {
            NPC.ai[1]++;
            if (NPC.ai[1] >= 40)
            {
                if (Main.rand.NextBool(9))
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertRuneLightBig>());
                }
            }

            NPC.damage = 0;
            NPC.ai[0]++;
            if (Hit)
            {
                NPC.ai[2]++;

                if (NPC.ai[2] % 2 == 0)
                {
                    HitPos.Y = BloodCystPos.Y + Main.rand.Next(-8, 8);
                    HitPos.X = BloodCystPos.X + Main.rand.Next(-8, 8);
                }

                NPC.position = Vector2.Lerp(NPC.position, HitPos, 0.6f);
                if (NPC.ai[2] >= 10)
                {
                    NPC.ai[2] = 0;
                    Hit = false;
                }
            }
            else
            {
                if (NPC.ai[0] == 2)
                {
                    HitPos = NPC.position;
                    NPC.position.Y -= 100;
                    BloodCystPos = NPC.position;
   
                }
                if (NPC.ai[0] >= 3 && NPC.ai[0] <= 100)
                {
                    Movement(BloodCystPos, 0f, 50f, 0.01f);
                }
                if (NPC.ai[0] >= 100 && NPC.ai[0] <= 200)
                {
                    Movement(BloodCystPos, 0f, 0f, 0.01f);
                }
                if (NPC.ai[0] == 200)
                {
                    NPC.ai[0] = 3;
                }
            }
        }

        private Vector2 HitPos;
        private bool Hit;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.2f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override void SetDefaults()
        {
            NPC.noTileCollide = true;
            NPC.width = 25;
            NPC.height = 25;
            NPC.damage = 0;
            NPC.defense = 999999;
            NPC.lifeMax = 4;
            NPC.HitSound = SoundID.DD2_CrystalCartImpact;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DesertRuneI>(), 1, 1));
        }


        private Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.LightBlue * num107 * .8f;
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
            Lighting.AddLight(NPC.Center, Color.Orange.ToVector3() * 2.25f * Main.essScale);
            return true;
        }

    }
}