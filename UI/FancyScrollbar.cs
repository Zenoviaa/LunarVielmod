using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.UI
{
    internal class FancyScrollbar : UIScrollbar
    {
        public FancyScrollbar() : base()
        {
            typeof(UIScrollbar).GetField("_texture", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this,
                ModContent.Request<Texture2D>("Stellamod/UI/FancyScrollbarOuter"));
            typeof(UIScrollbar).GetField("_innerTexture", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this,
                ModContent.Request<Texture2D>("Stellamod/UI/FancyScrollbarInner"));
        }
    }
}
