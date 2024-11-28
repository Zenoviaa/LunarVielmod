using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Common;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.UI.CauldronSystem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Town
{
    internal class WitchesCauldron : VeilTownNPC
    {
        private int _frame;
        private float _frameCounter;
        private enum AnimationState
        {
            Idle,
            Brew
        }

        private AnimationState Animation;
        private ref float Timer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[Type] = 30;
        }

        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Struct/Overworld/WitchTown";
            spawner.spawnTileOffset = new Point(150, -19);
        }
        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            //Set buttons
            buttons.Add(new Tuple<string, Action>("Brew", OpenCauldron));

            portrait = "QuestionMarkPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "CauldronOpenDialogue";
        }

        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 128;
            NPC.aiStyle = -1;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 9000;
            NPC.knockBackResist = 0.5f;
            NPC.npcSlots = 0;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.dontTakeDamage = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.noGravity = true;
            NPC.friendly = true; // NPC Will not attack player
            SpawnAtPoint = true;
            HasTownDialogue = true;
        }

        private void OpenCauldron()
        {
            CauldronUISystem cauldronUISystem = ModContent.GetInstance<CauldronUISystem>();
            cauldronUISystem.OpenUI();
            cauldronUISystem.CauldronPos = NPC.Center;
            Main.CloseNPCChatOrSign();
            Main.playerInventory = true;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Witch's Cauldron"
            };
        }

        public override bool CanChat()
        {
            return true;
        }
        
        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add(LangText.Chat(this, "Basic1"));
            return chat; // chat is implicitly cast to a string.
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI

            button = LangText.Chat(this, "Button");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int totalFrameCount = 30;
            if(Animation == AnimationState.Brew)
            {
                texture = ModContent.Request<Texture2D>(Texture + "_Brew").Value;
                totalFrameCount = 25;
            }


            Vector2 drawPos = NPC.Center - screenPos;
            Rectangle frame = texture.GetFrame(_frame, totalFrameCount);
            Vector2 drawOrigin = frame.Size() / 2f;
            float drawRotation = NPC.rotation;

            SpriteEffects effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            //Ok so we need some glowing huhh
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            Vector2 s = NPC.Size / 2;
            //Trail Code
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Color startColor = new Color(255, 255, 113);
                startColor *= 0.5f;
                Color endColor = new Color(232, 111, 24);
                endColor *= 0.5f;
                Vector2 trailDrawPos = NPC.oldPos[k] - Main.screenPosition + s + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(startColor, endColor, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(texture, trailDrawPos, frame, color, NPC.oldRot[k], frame.Size() / 2, NPC.scale, effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            
            //Draw Frame
            spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, frame.Size() / 2, NPC.scale, effects, 0f);
            
            //We predrawing up in here
            return false;
        }

   
        public override void AI()
        {
            base.AI();

            //This feels kinda dumb but maybe it'll work
            Cauldron.OnBrew -= BrewSomethingAnimation;
            Cauldron.OnBrew += BrewSomethingAnimation;


            AI_Animate();
            Timer++;
            float yOffset = MathF.Sin(Timer * 0.2f);
            NPC.position += new Vector2(0, yOffset);
            Lighting.AddLight(NPC.position, 1, 1, 1);

            if(Timer % 32 == 0)
            {
                Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(-32, -16)),
                    ModContent.DustType<Sparkle>(), new Vector2(Main.rand.NextFloat(-0.02f, 0.4f), -Main.rand.NextFloat(0.1f, 2f)), 0, new Color(0.05f, 0.08f, 0.2f, 0f), Main.rand.NextFloat(0.25f, 2f));
            }
        }

        private void AI_Animate()
        {
            _frameCounter += 0.5f;
            if (_frameCounter >= 1f)
            {
                _frameCounter = 0;
                _frame++;
            }

            switch (Animation)
            {
                case AnimationState.Idle:
                    if (_frame >= 30)
                    {
                        _frame = 0;
                    }
                    break;
                case AnimationState.Brew:
                    if (_frame >= 25)
                    {
                        _frame = 0;
                        Animation = AnimationState.Idle;
                    }
                    break;
            }
        }

        public void BrewSomethingAnimation(CauldronBrew brew)
        {
            _frame = 0;
            Animation = AnimationState.Brew;
            if(brew.result != -1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/CauldronCraft");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }
            if(brew.result == -1)
            {
                //Womp Womp Basically
                int combatText = CombatText.NewText(NPC.getRect(), Color.Gray, "There's nothing inside!", true);
                CombatText numText = Main.combatText[combatText];
                numText.lifeTime = 120;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/CauldronCraft");
                soundStyle.PitchVariance = 0.15f;
                soundStyle.Pitch = -1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }
            if(brew.result == ModContent.ItemType<KaleidoscopicInk>())
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/CauldronCraft");
                soundStyle.PitchVariance = 0.15f;
                soundStyle.Pitch = 1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

        }


    }
}
