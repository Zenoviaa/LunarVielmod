using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Bosses.Sylia
{
    public class UnderworldRift : ModNPC
    {
        private int _portalFrameCounter;
        private int _portalFrameTick;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 1;
			NPCID.Sets.ActsLikeTownNPC[Type] = true;
			NPCID.Sets.SpawnsWithCustomName[Type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
			NPCID.Sets.NoTownNPCHappiness[NPC.type] = true;
        }

		// AI counter
		public int counter;
		public override void SetDefaults()
		{
			// Sets NPC to be a Town NPC
			NPC.friendly = true; // NPC Will not attack player
			NPC.width = 212;
			NPC.height = 136;
			NPC.aiStyle = -1;
			NPC.damage = 90;
			NPC.defense = 42;
			NPC.lifeMax = 9000;
			NPC.knockBackResist = 0.5f;
			NPC.npcSlots = 0;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = true;
			NPC.dontTakeDamage = true;
			NPC.noTileCollide = true;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanChat()
		{
			return true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A strange rift to void, disturbing it may yield catastrophic results...")),
			});
		}

		private void PreDrawTrail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
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
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(190, 50, 250), new Color(72, 13, 255), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

		private void PreDrawGlow(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
            Texture2D texture = TextureAssets.Projectile[NPC.type].Value;
            int frames = Main.npcFrameCount[NPC.type];
            int frameHeight = texture.Height / frames;

            NPC.frameCounter += 1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int f = (int)NPC.frameCounter;
            int startY = f * frameHeight;

            Rectangle frame = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = frame.Size() / 2f;
            float offsetX = 20f;
            drawOrigin.X = NPC.spriteDirection == 1 ? frame.Width - offsetX : offsetX;

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(NPC.width / 2 - frameOrigin.X, NPC.height - frame.Height);
            Vector2 drawPos = NPC.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = time * 0.04f;

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
                Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), 
                    NPC.rotation, frameOrigin, NPC.scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), 
                    NPC.rotation, frameOrigin, NPC.scale, SpriteEffects.None, 0);
            }
        }

		// The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
		// Returning false will allow you to manually draw your NPC
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// This code slowly rotates the NPC in the bestiary
			// (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
			if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
			{
				drawModifiers.Rotation += 0.001f;

				// Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

            PreDrawTrail(spriteBatch, screenPos, drawColor);
            PreDrawGlow(spriteBatch, screenPos, drawColor);

            Vector2 drawPosition = NPC.Center - screenPos + new Vector2(4, -48);
            float width = 43;
            float height = 126;
            Vector2 origin = new Vector2(width / 2, height / 2);
            Texture2D tex = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/UnderworldRiftPortal").Value;
            int frameSpeed = 1;
            int frameCount = 60;
            spriteBatch.Draw(tex, drawPosition,
                tex.AnimationFrame(ref _portalFrameCounter, ref _portalFrameTick, frameSpeed, frameCount, true),
                drawColor, 0f, origin, 2f, SpriteEffects.None, 0f);
            return true;
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();
			chat.Add("...");
			return chat; // chat is implicitly cast to a string.
		}

		private void AI_FadeIn()
		{
            ref float invisTimer = ref NPC.ai[0];
            if (invisTimer == 0)
            {
                NPC.alpha = 255;
                SoundEngine.PlaySound(SoundID.Item119, NPC.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2048, 32f);
                //Charged Sound thingy
                for (int i = 0; i < 48; i++)
                {
                    Vector2 position = NPC.Center;
                    Vector2 speed = Main.rand.NextVector2CircularEdge(8f, 8f);
                    Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), 1);
                    p.layer = Particle.Layer.BeforeProjectiles;
                }

                invisTimer++;
            }  
          
            if (NPC.alpha > 0)
                NPC.alpha--;
        }

		private void AI_Hover()
		{
            float range = 0.25f;
            float hover = VectorHelper.Osc(-range, range);
            Vector2 targetCenter = NPC.Center + new Vector2(0, hover);
            Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(NPC.Center, targetCenter, 5);
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
        }

		private void Visuals()
		{
            if (Main.rand.NextBool(8))
            {
                int bodyParticleCount = 2;
                float bodyRadius = 32;
                for (int b = 0; b < bodyParticleCount; b++)
                {
                    Vector2 position = NPC.Center + Main.rand.NextVector2Circular(bodyRadius / 2, bodyRadius / 2);
                    Vector2 vel = new Vector2(0, -1);
                    float size = Main.rand.NextFloat(0.25f, 0.3f);
                    Particle p = ParticleManager.NewParticle(position, vel, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), size);

                    p.layer = Particle.Layer.BeforeProjectiles;
                    Particle tearParticle = ParticleManager.NewParticle(position, vel, ParticleManager.NewInstance<VoidTearParticle>(),
                        default(Color), size + 0.025f);

                    tearParticle.layer = Particle.Layer.BeforePlayersBehindNPCs;
                }
            }
        }

        public override void AI()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
            {
                NPC.Kill();
            }
            NPC.TargetClosest();
            AI_FadeIn();
			AI_Hover();
			Visuals();
        }

		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				NPC.FullName,
				NPC.FullName
			};
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = LangText.Chat(this, "Button");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton && !NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 5, 
						ModContent.NPCType<Sylia>());
                }
				else
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return;

                    StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI,
						ModContent.NPCType<Sylia>(), (int)NPC.Center.X, (int)NPC.Center.Y - 5);
                }
			}
		}
	}
}