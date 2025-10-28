using Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak;
using Stellamod.Helpers;
using Terraria.ModLoader;

namespace Stellamod.Gores
{

    public abstract class GustbeakGore : ModGore
    {
        protected string BaseGorePath => typeof(Gustbeak).DirectoryHere() +"/Gustbeak";
    }
    public class GustbeakBackLegBackGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BackLegBack";
    }
    public class GustbeakBackLegFrontGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BackLegFront";
    }
    public class GustbeakBodyBackGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BodyBack";
    }
    public class GustbeakBodyFrontGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BodyFront";
    }
    public class GustbeakBodyMiddleGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_BodyMiddle";
    }
    public class GustbeakFrontLegBackGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_FrontLegBack";
    }
    public class GustbeakFrontLegFrontGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_FrontLegFront";
    }
    public class GustbeakHeadGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_HeadGore";
    }
    public class GustbeakTailGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_Tail";
    }
    public class GustbeakWingsBackGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_WingsBackGore";
    }
    public class GustbeakWingsFrontGore : GustbeakGore
    {
        public override string Texture => BaseGorePath + "_WingsFrontGore";
    }
}
