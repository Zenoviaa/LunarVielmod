using Stellamod.Core.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Scrolls;

[Autoload(false)]
public sealed class ScrollItem : ModItem
{
    protected override bool CloneNewInstances => true;
    public override string Name
    {
        get
        {
            return Ability.ToString()+"Scroll";
        }
    }

    public override string Texture
    {
        get
        {
            int staminaCost = ScrollAbilities.GetStaminaCost(Ability);
            string texturePath = $"{this.GetTypeDirectoryWithSlash()}Scroll_{staminaCost}";
            return texturePath;
        }
    }

    public ScrollItem()
    {

    }
    public ScrollItem(ScrollAbility ability)
    {
        Ability = ability;
    }

    public ScrollAbility Ability;
    protected override void InitTemplateInstance()
    {
        base.InitTemplateInstance();
    }

  
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 62;
        Item.height = 32;
        Item.rare = ItemRarityID.Green;
        Item.useTime = 16;
        Item.useAnimation = 16;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.autoReuse = false;
        Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");
        Item.consumable = true;
    }



    public override ModItem Clone(Item newEntity)
    {
        ScrollItem scrollItem = (ScrollItem)base.Clone(newEntity);
        scrollItem.Ability = Ability;
        return scrollItem;
    }
    public override bool ConsumeItem(Player player)
    {
        return true;
    }

    public override bool? UseItem(Player player)
    {

        ScrollAbilities.enchant = Ability;
        ScrollAbilities.usingScroll = this;
        return true;
        //return base.UseItem(player);
    }
    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);
        tag["ability"] = Ability.ToString();
    }
    public override void LoadData(TagCompound tag)
    {
        base.LoadData(tag);
        string ability = tag.GetString("ability");
        if (string.IsNullOrEmpty(ability))
            return;
        Ability = (ScrollAbility)Enum.Parse(typeof(ScrollAbility), ability);
    }
}
