using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.QuestSystem;
using Stellamod.Helpers;
using Stellamod.NPCs;
using Stellamod.UI.DialogueTowning;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public abstract class VeilTownNPC : ModNPC
    {

        public bool HasTownDialogue { get; set; }
        public bool SpawnAtPoint { get; set; }
        public virtual string QuestMarkTexture => "Stellamod/Common/QuestSystem/QuestMark";
        public virtual void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {

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

        public void Talk()
        {
            DialogueTowningUISystem uiSystem = ModContent.GetInstance<DialogueTowningUISystem>();
            uiSystem.ChatWith(this);
        }

        public virtual bool HasQuestAvailable()
        {
            List<int> quests = new List<int>();
            SetQuestLine(quests);

            for (int i = 0; i < quests.Count; i++)
            {
                int questType = quests[i];
                Quest quest = QuestLoader.GetQuest(questType);
                if (quest.IsQuestAvailable(Main.LocalPlayer))
                {
                    return true;
                }
            }
            return false;
        }

        public Quest GetNextQuest(List<int> quests)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                int questType = quests[i];
                Quest quest = QuestLoader.GetQuest(questType);
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
        public virtual void SetQuestLine(List<int> quests)
        {

        }

        public void OpenShop()
        {
            NPCHelper.OpenShop(NPC);
        }

        public void GiveQuest()
        {
            List<int> quests = new List<int>();
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
