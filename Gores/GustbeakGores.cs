using Terraria.ModLoader;

namespace Stellamod.Gores
{

    internal abstract class GustbeakGore : ModGore
    {
        protected string BaseGorePath => "Stellamod/NPCs/Bosses/Gustbeak/Gustbeak";
    }
    internal class GustbeakBackLegBackGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BackLegBack";
    }
    internal class GustbeakBackLegFrontGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BackLegFront";
    }
    internal class GustbeakBodyBackGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BodyBack";
    }
    internal class GustbeakBodyFrontGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BodyFront";
    }
    internal class GustbeakBodyMiddleGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BodyMiddle";
    }
    internal class GustbeakFrontLegBackGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_FrontLegBack";
    }
    internal class GustbeakFrontLegFrontGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_FrontLegFront";
    }
    internal class GustbeakHeadGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_HeadGore";
    }
    internal class GustbeakTailGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_Tail";
    }
    internal class GustbeakWingsBackGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_WingsBackGore";
    }
    internal class GustbeakWingsFrontGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_WingsFrontGore";
    }
}
