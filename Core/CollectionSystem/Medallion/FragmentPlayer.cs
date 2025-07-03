using Stellamod.Content.Items.Fragments;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.CollectionSystem.Medallion
{
    internal class FragmentPlayer : ModPlayer
    {
        //Weehee
        //Woohoo
        public bool hasMak;
        public bool hasDraekus;
        public bool hasApilithy;
        public bool hasGothivia;
        public bool hasKari;
        public BaseMedallionFragment HeldFragment;

        public override void PostUpdate()
        {
            base.PostUpdate();
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["hasMak"] = hasMak;
            tag["hasDraekus"] = hasDraekus;
            tag["hasApilithy"] = hasApilithy;
            tag["hasGothivia"] = hasGothivia;
            tag["hasKari"] = hasKari;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            hasMak = tag.GetBool("hasMak");
            hasDraekus = tag.GetBool("unlockedAshotiFragment");
            hasApilithy = tag.GetBool("hasApilithy");
            hasGothivia = tag.GetBool("hasGothivia");
            hasKari = tag.GetBool("hasKari");
        }
    }
}
