using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class WillowOfTheSoulsWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 2400;
            Item.shootSpeed = 7;
            Size = 24;
            TrailLength = 55;
            Item.mana = 45;
            Form = FormRegistry.Crescent.Value;
            normalSlotCount = 5;
            timedSlotCount = 7;
        }

       
            public override void ModifyElementPreferences(List<int> elements)
            {
                base.ModifyElementPreferences(elements);
                elements.Add(ModContent.ItemType<UvilisElement>());
                elements.Add(ModContent.ItemType<NaturalElement>());
                elements.Add(ModContent.ItemType<HexElement>());
                elements.Add(ModContent.ItemType<DeeyaElement>());
                elements.Add(ModContent.ItemType<MothlightElement>());
                elements.Add(ModContent.ItemType<GuutElement>());
          }
           public override void AddRecipes()
           {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<GhastlySpirit>());
           }
    }
    }

