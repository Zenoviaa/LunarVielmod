using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.UI.Dialogue
{
    internal class VerliasDialogue : Dialogue
    {
        //The number of steps in this dialogue
        public override int Length => 3;

        public override void Next(int index)
        {
            base.Next(index);

            //This starts the dialogue
            switch (index)
            {
                case 0:
                    //Set the texture of the portrait
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/VerliasDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("VerliaDialogue1"));
                    break;
                case 1:
                    DialogueSystem.WriteText(GetLocalizedText("VerliaDialogue2"));
                    break;
                case 2:
                    DialogueSystem.WriteText(GetLocalizedText("VerliaDialogue3"));
                    break;
            }
        }

        public override void Update(int index)
        {
            base.Update(index);
            //If you want stuff to happen while they're talking you can do it here ig
            //But that might not be a good idea since you can just speed through dialogues
        }

        public override void Complete()
        {

            //Do something when the dialogue is completely finished
            if(Main.netMode != NetmodeID.SinglePlayer)
            {
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), 
                    (byte)MessageType.StartBossFromDialogue, 
                    (int)DialogueType.Start_Verlia).Send(-1);
            }
            else
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type == ModContent.NPCType<StarteV>())
                    {
                        StarteV verlia = npc.ModNPC as StarteV;
                        verlia.State = StarteV.ActionState.Death;
                        verlia.ResetTimers();
                    }
                }
            }
       
            base.Complete();
        }
    }
}
