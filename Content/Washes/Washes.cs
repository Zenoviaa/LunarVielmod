using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Washes;

public class WashShaderData : ArmorShaderData
{
    public Vector3[] colors;
    public WashShaderData(Ref<Effect> shader, string passName)
        : base(shader, passName)
    {
    }
    public WashShaderData(Asset<Effect> shader, string passName)
        : base(shader, passName)
    {
    }

    public override void Apply()
    {
        Shader.Parameters["colors"].SetValue(colors);
        Shader.Parameters["length"].SetValue(colors.Length);
        base.Apply();
    }
}

public abstract class AbstractWash : ModItem
{
    private static WashShaderData _armorShader;
    public override void SetStaticDefaults()
    {
        // Avoid loading assets on dedicated servers. They don't use graphics cards.
        if (!Main.dedServ)
        {
            Main.QueueMainThreadAction(RegisterShader);
        }

        Item.ResearchUnlockCount = 3;
    }
    private void RegisterShader()
    {
        Asset<Effect> washShader = Mod.Assets.Request<Effect>("Effects/Palettes/Wash");
        WashShaderData shaderData = new WashShaderData(washShader, "PixelPass");

        Texture2D washTexture = ModContent.Request<Texture2D>(Texture + "_Palette", AssetRequestMode.ImmediateLoad).Value;
        HashSet<Vector3> colors = new HashSet<Vector3>();
        Color[] pixels = new Color[washTexture.Width * washTexture.Height];
        washTexture.GetData(pixels);
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];
            if (pixel.A == 0)
                continue;
            colors.Add(pixel.ToVector3());
        }

        shaderData.colors = colors.ToArray();

        // The following code creates an effect (shader) reference and associates it with this item's type Id.
        _armorShader = GameShaders.Armor.BindShader(
            Item.type,
            shaderData // Be sure to update the effect path and pass name here.
        );
    }

    public override void SetDefaults()
    {
        // Item.dye will already be assigned to this item prior to SetDefaults because of the above GameShaders.Armor.BindShader code in Load().
        // This code here remembers Item.dye so that information isn't lost during CloneDefaults.
        int dye = Item.dye;

        Item.CloneDefaults(ItemID.GelDye); // Makes the item copy the attributes of the item "Gel Dye" Change "GelDye" to whatever dye type you want.
        Item.dye = dye;
    }
}


public class PumpkinSpiceWash : AbstractWash { }

public class SillySweetWash : AbstractWash { }

public class CarrotWash : AbstractWash { }

public class PurpleShadowWash : AbstractWash { }

public class CandyCaneWash : AbstractWash { }

public class GrassyWash : AbstractWash { }

public class VileWash : AbstractWash { }

public class GoldenrodWash : AbstractWash { }

public class DiamondWash : AbstractWash { }

public class SapphireWash : AbstractWash { }

public class LavenderWash : AbstractWash { }

public class SilverWash : AbstractWash { }

public class HalloweenWash : AbstractWash { }

public class SanguineWash : AbstractWash { }

public class MistletoeWash : AbstractWash { }

public class CaramelWash : AbstractWash { }

public class CherryBlossomWash : AbstractWash { }

public class BeachDayWash  : AbstractWash { }

public class ChocolateWash : AbstractWash { }

public class RainbowWash : AbstractWash { }

public class CoralWash : AbstractWash { }

public class AcidWash : AbstractWash { }

public class GroundyWash : AbstractWash { }

public class SeafloorWash : AbstractWash { }

public class AquaWash : AbstractWash { }

public class TurquoiseWash : AbstractWash { }

public class TwinklingWash : AbstractWash { }

public class WheatFieldsWash : AbstractWash { }

public class RedMixWash : AbstractWash { }

public class SunsetWash : AbstractWash { }

public class PeachyWash : AbstractWash { }

public class DarkSunsetWash : AbstractWash { }

public class RoseyWash : AbstractWash { }

public class IllurianWash : AbstractWash { }

public class ShadefulWash : AbstractWash { }

public class CliffsideWash : AbstractWash { }

public class BumbleBeeWash : AbstractWash { }

public class HotrodWash : AbstractWash { }

public class MoodyWash : AbstractWash { }