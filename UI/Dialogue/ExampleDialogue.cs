namespace Stellamod.UI.Dialogue
{
    internal class ExampleDialogue : Dialogue
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
                    DialogueSystem.Panel.Portrait.Texture = "Stellamod/UI/Dialogue/ExampleDialoguePortrait";
                    DialogueSystem.WriteText(GetLocalizedText("ExampleDialogue1"));
                    break;
                case 1:
                    DialogueSystem.WriteText(GetLocalizedText("ExampleDialogue2"));
                    break;
                case 2:
                    DialogueSystem.WriteText(GetLocalizedText("ExampleDialogue3"));
                    break;
            }
        }

        public override void Update(int index)
        {
            base.Update(index);
            //If you want stuff to happen while they're talking you can do it here ig
        }

        public override void Complete()
        {
            base.Complete();
            //Do something when the dialogue is completely finished
            //Maybe you want to summon a boss or play a sound or something something
        }
    }
}
