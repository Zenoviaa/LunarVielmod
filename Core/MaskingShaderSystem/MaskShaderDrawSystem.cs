using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.WallBackgroundSystem;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.MaskingShaderSystem;

public interface IPreDrawMaskShader
{
    void PreDrawMask(SpriteBatch spriteBatch);
}

public interface IDrawMaskShader
{
    MiscShaderData GetMaskDrawShader();
    void DrawMask(SpriteBatch spriteBatch);
}
