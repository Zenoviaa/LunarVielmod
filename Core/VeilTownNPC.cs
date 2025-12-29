using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.QuestSystem;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.NPCs;
using Stellamod.UI.DialogueTowning;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public abstract class VeilTownNPC : ModNPC,
        IDrawOutlines
    {
        protected bool _drawOutlines;
        public bool HasTownDialogue { get; set; }
        public bool SpawnAtPoint { get; set; }
        public bool OnlyInteract { get; set; }
        public bool NoSpecialInteract { get; set; }
        public Vector2 DrawOffset { get; set; }
        public virtual string QuestMarkTexture => "Stellamod/Core/QuestSystem/QuestMark";

        public virtual void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {

        }

        public virtual void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (NoSpecialInteract)
                return;
            if (!_drawOutlines)
                return;
            _drawOutlines = false;


            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            float yDiff = MathF.Abs(NPC.frame.Size().Y - NPC.Size.Y);
            drawPos.Y -= yDiff/2;
            drawPos += DrawOffset;

            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            float outlineOffset = 2;
            Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
            Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
            Color outlineColor = Color.White;

            spriteBatch.Draw(texture, left, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, right, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, up, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, down, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if(NoSpecialInteract)
                return PreDraw(spriteBatch, screenPos, drawColor);
            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            float yDiff = MathF.Abs(NPC.frame.Size().Y - NPC.Size.Y);
            drawPos.Y -= yDiff/2;
            drawPos += DrawOffset;

            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float outlineOffset = 2;
            Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
            Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
            Color outlineColor = Color.White;
            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }

        public override bool PreHoverInteract(bool mouseIntersects)
        {
            if (NoSpecialInteract)
                return base.PreHoverInteract(mouseIntersects);

            bool isClose = Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < 200;
            if (!isClose)
                return false;
            if (mouseIntersects)
            {
                _drawOutlines = true;
                SpriteBatch spriteBatch = Main.spriteBatch;
                Vector2 drawPosition = NPC.Center - Main.screenPosition;
                Texture2D chatBubble = TextureAssets.Chat.Value;
                Vector2 drawOrigin = chatBubble.Size() / 2f;
                drawPosition -= new Vector2(0, NPC.frame.Size().Y / 2f);
                spriteBatch.Draw(chatBubble, drawPosition, null, Color.White, 0, drawOrigin, 1, SpriteEffects.None,0);
            }
            if (Main.mouseRight && Main.mouseRightRelease && mouseIntersects )
            {
                DialogueTowningUISystem towningUISystem = ModContent.GetInstance<DialogueTowningUISystem>();
                towningUISystem.Interact(this);
            }
            return false;
        }


        public virtual void OpenTownDialogue(
            ref string text,
            ref string portrait,
            ref float timeBetweenTexts,
            ref SoundStyle? talkingSound,
            List<Tuple<string, Action>> buttons)
        {

        }
        public void CloseTownDialogue()
        {
            DialogueTowningUISystem uiSystem = ModContent.GetInstance<DialogueTowningUISystem>();
            uiSystem.CloseUI();
        }
        public virtual void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {

        }

        public virtual void Talk()
        {

        }

        public void OpenTalkOptions(params BaseDialogue[] dialogues)
        {
            DialogueTowningUISystem uiSystem = ModContent.GetInstance<DialogueTowningUISystem>();
            uiSystem.OpenTalkOptions(dialogues);
        }

        public virtual bool HasQuestAvailable()
        {
            List<Quest> quests = new List<Quest>();
            SetQuestLine(quests);

            for (int i = 0; i < quests.Count; i++)
            {
                Quest quest = quests[i];
                if (quest.IsQuestAvailable(Main.LocalPlayer))
                {
                    return true;
                }
            }
            return false;
        }

        public Quest GetNextQuest(List<Quest> quests)
        {
            for (int i = 0; i < quests.Count; i++)
            { 
                Quest quest = quests[i];
                if (quest.IsQuestAvailable(Main.LocalPlayer))
                {
                    return quest;
                }
            }
            return null;
        }
        /// <summary>
        /// Lets you set the quests that this NPC can give you
        /// </summary>
        /// <param name="quests"></param>
        public virtual void SetQuestLine(List<Quest> quests)
        {

        }

        public void OpenShop()
        {
            NPCHelper.OpenShop(NPC);
            DialogueTowningUISystem uiSystem = ModContent.GetInstance<DialogueTowningUISystem>();
            uiSystem.OnlyCloseWindow();
        }

        public void GiveQuest()
        {
            List<Quest> quests = new List<Quest>();
            SetQuestLine(quests);
            Quest quest = GetNextQuest(quests);
            if (quest == null)
                return;
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            if (questPlayer.GiveQuest(quest))
            {
                DialogueTowningUISystem uiSystem = ModContent.GetInstance<DialogueTowningUISystem>();
                uiSystem.ChatWith(quest);
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NoSpecialInteract)
                return;

            base.PostDraw(spriteBatch, screenPos, drawColor);
            if (HasQuestAvailable())
            {

                Texture2D questMark = ModContent.Request<Texture2D>(QuestMarkTexture).Value;
                Vector2 hoverOffset = Vector2.Lerp(Vector2.Zero, -Vector2.UnitY * 8, VectorHelper.Osc(0f, 1f));
                Vector2 drawPos = NPC.Center + hoverOffset;
                drawPos.Y -= NPC.height;
                drawPos.Y -= 16;
                float drawRotation = 0f;
                float drawScale = 1.25f;
                Vector2 drawOrigin = questMark.Size() / 2;

                Texture2D texture = TextureRegistry.BasicGlow.Value;
                Vector2 shadowDrawOrigin = texture.Size() / 2f;
                Color blackColor = Color.Black.MultiplyRGB(drawColor);
                float shadowDrawScale = 0.66f * drawScale;
                spriteBatch.Draw(texture, drawPos - Main.screenPosition, null, blackColor, 0, shadowDrawOrigin, shadowDrawScale, SpriteEffects.None, layerDepth: 0);


                spriteBatch.Draw(questMark, drawPos - Main.screenPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);

                spriteBatch.Restart(blendState: BlendState.Additive);

                for (float f = 0f; f < 1f; f += 0.2f)
                {
                    float rot = f * MathHelper.TwoPi;
                    rot += Main.GlobalTimeWrappedHourly;
                    Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(0.5f, 1f) * 3;
                    spriteBatch.Draw(questMark, drawPos - Main.screenPosition + offset, null, drawColor * 0.8f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
                }
                spriteBatch.RestartDefaults();

                Lighting.AddLight(drawPos, Color.White.ToVector3() * 0.78f);
            }
        }

    }
}
