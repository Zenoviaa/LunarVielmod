using Stellamod.UI.Panels;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod
{
	public class Stellamod : Mod
	{
        class MenuBar : UIState
        {

            public HarvestButton playButton;

            public override void OnInitialize()
            {
                playButton = new HarvestButton();

                Append(playButton);

            }


        }

    }
}
