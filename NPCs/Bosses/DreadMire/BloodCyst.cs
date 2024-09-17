
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace Stellamod.NPCs.Bosses.DreadMire
{

    public class BloodCyst : ModNPC
    {
        private int _radiusCounter;
        private Vector2 BloodCystPos;

        private const float Immune_Distance = 16 * 9;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Cyst");
            Main.npcFrameCount[NPC.type] = 11;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        private void OnlyTakeDamageWhenClose()
        {
            NPC.dontTakeDamage = true;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (Vector2.Distance(NPC.Center, player.Center) < Immune_Distance)
                {
                    NPC.dontTakeDamage = false;
                    break;
                }
            }
        }

        private void Visuals()
        {
            float radius = Immune_Distance;
            int count = 8;
            _radiusCounter++;
            if(_radiusCounter > 3)
            {
                for (int i = 0; i < count; i++)
                {
                    Vector2 position = NPC.Center + new Vector2(radius, 0).RotatedBy(((i * MathHelper.PiOver2 / count)) * 4);
                    ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<Ink3>(), default(Color), Main.rand.NextFloat(0.2f, 0.8f));
                }
                _radiusCounter = 0;
            }
        }

        public override void AI()
        {
            OnlyTakeDamageWhenClose();
            Visuals();

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
            else if(!Main.bloodMoon)
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
            Lighting.AddLight(NPC.Center, 1.6f * num, 0.4f * num, 0.8f * num);
        }

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
            base.NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, target, 0.5f), base.NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
           /* return Main.bloodMoon && !Main.dayTime && 
                !NPC.AnyNPCs(ModContent.NPCType<BloodCyst>()) &&
                !NPC.AnyNPCs(ModContent.NPCType<BloodCystDead>()) &&
                !NPC.AnyNPCs(ModContent.NPCType<DreadMire>()) && 
                !NPC.AnyNPCs(ModContent.NPCType<DreadMiresHeart>()) ? 1.013f : 0f;*/
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A crystal which holds the power of summoning one of the three deities")),
            });
        }
        
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 128f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BloodCystDeath"));
                var entitySource = NPC.GetSource_FromThis();
                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<BloodCystDead>());
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

        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
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
            SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
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
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
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