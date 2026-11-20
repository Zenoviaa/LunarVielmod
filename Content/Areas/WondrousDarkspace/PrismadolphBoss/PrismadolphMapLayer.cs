using Terraria;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss;

public class PrismadolphMapLayer : ModMapLayer
{
    // Here you can define where to put this layer in the vanilla map layer order.
    // In this case we go before the pings layer since all other vanilla map layers are ordered before pings,
    public override Position GetDefaultPosition() => new Before(IMapLayer.Pings);

    // In the Draw method, we draw everything. Consulting vanilla examples in the source code is a good resource for properly using this Draw method.
    public override void Draw(ref MapOverlayDrawContext context, ref string text)
    {
        // Here we define the scale that we wish to draw the icon when hovered and not hovered.
        const float scaleIfNotSelected = 1f;
        const float scaleIfSelected = scaleIfNotSelected * 2f;


        // Here we retrieve the texture of the Skeletron boss head so that we can draw it. Remember that not all textures are loaded by default, so you might need to do something like `Main.instance.LoadItem(ItemID.BoneKey);` in your code to ensure the texture is loaded.
        var prismadolphIcon = AssetReferences.Assets.Textures.Darkspace.NPCs.Prismadolph_BossIcon.Asset;
        if (context.Draw(prismadolphIcon.Value, PrismadolphRoamingSystem.RoamingPosition.ToTileCoordinates().ToVector2(), Color.White,
               new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver)
        {
            // When the icon is being hovered by the users mouse, we set the mouse text to the localized text for "The Dungeon"
            text = LangText.Common("BellFlower");
        }
    }
}
