using Microsoft.Xna.Framework.Graphics;
using Stellamod.NPCs.Bosses.GothiviaTheSun.GOS;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;
using Stellamod.NPCs.Bosses.Zui.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.UI.Dialogue
{
    internal class GothiviaBeatDialogue : Dialogue
    {
        //The number of steps in this dialogue
        public override int Length => 15;

        public override void Next(int index)
        {
            base.Next(index);

            //This starts the dialogue
            switch (index)
            {
                case 0:
                    //Set the texture of the portrait
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose1"));
                    break;
                case 1:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose2"));
                    break;
                case 2:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose3"));
                    break;

                case 3:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/SirestiasDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("SireLoose4"));
                    break;

                case 4:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose5"));
                    break;

                case 5:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/SirestiasDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("SireLoose6"));
                    break;

                case 6:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose7"));
                    break;

                case 7:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/SirestiasDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("SireLoose8"));
                    break;

                case 8:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose9"));
                    break;

                case 9:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/SirestiasDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("SireLoose10"));
                    break;

                case 10:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose11"));
                    break;

                case 11:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose12"));
                    break;

                case 12:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/SirestiasDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("SireLoose13"));
                    break;

                case 13:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/SirestiasDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("SireLoose14"));
                    break;

                case 14:
                    DialogueSystem.SetPortrait("Stellamod/UI/Dialogue/GothiviaDialoguePortrait");

                    //Put your dialogue in Mods.Stellamod.Dialogue.hjson, then get it like this
                    DialogueSystem.WriteText(GetLocalizedText("GothLoose15"));
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
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == ModContent.NPCType<GothiviaDeath>())
                {
                    GothiviaDeath zui = npc.ModNPC as GothiviaDeath;
                    zui.DM = true;
                  
                }
            }
            //Do something when the dialogue is completely finished


            base.Complete();
        }




    }
}
