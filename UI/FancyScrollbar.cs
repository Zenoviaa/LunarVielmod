using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI
{
    public class FancyFixedScrollbar : FixedUIScrollbar
    {

        public FancyFixedScrollbar(UserInterface userInterface) : base(userInterface)
        {
            typeof(UIScrollbar).GetField("_texture", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this,
                ModContent.Request<Texture2D>("Stellamod/UI/FancyScrollbarOuter"));
            typeof(UIScrollbar).GetField("_innerTexture", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this,
                ModContent.Request<Texture2D>("Stellamod/UI/FancyScrollbarInner"));
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
        }
    }
    public class FancyScrollbar : UIScrollbar
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
