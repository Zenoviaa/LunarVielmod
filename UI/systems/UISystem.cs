using Stellamod.UI.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.systems
{
    public class UISystem : ModSystem
    {
        public UserInterface HarvestButtonLayer;
        public HarvestButton HarvestButtonElement;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                HarvestButtonLayer = new UserInterface();
                HarvestButtonElement = new HarvestButton();
                HarvestButtonLayer.SetState(HarvestButtonElement);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer("Stella: Harvest Button", () =>
                {
                    HarvestButtonLayer.Update(Main._drawInterfaceGameTime);
                    HarvestButtonElement.Draw(Main.spriteBatch);
                    return true;
                }));
            }
        }
    }
    }