using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

[Autoload(Side = ModSide.Client)]
public class FenixDomain : ModSystem
{

    public bool drawFenix;
    public override void OnModLoad()
    {
        On_Main.DrawNPCs += DrawBlack;
    }
    public override void Unload()
    {
        base.Unload();
    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        On_Main.DrawNPCs -= DrawBlack;
    }

    private void DrawBlack(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        if (drawFenix)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.Clear(Color.Transparent);
            FenixDomainShader fenixDomainShader = ShaderContent.GetInstance<FenixDomainShader>();
            fenixDomainShader.GradientMap = TextureRegistry.CloudNoise3.Value;
            fenixDomainShader.Time = Main.GlobalTimeWrappedHourly;
            spriteBatch.Restart(effect: fenixDomainShader.Effect);

            Rectangle targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, targetRect, Color.White);

            spriteBatch.RestartDefaults();

        
            DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
            if (singularityFallSystem.hoveringPlatform)
            {
                Texture2D bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
                Vector2 drawOrigin = new Vector2(bloomLine.Size().X / 2, 0);
                float rotation = MathHelper.PiOver2;
                Color drawColor = Color.White;
                drawColor.A = 0;
                drawColor *= 0.5f;
                drawColor *= ExtraMath.Osc(0.5f, 1f);
                Vector2 drawPosition = new Vector2(Main.LocalPlayer.Center.X, singularityFallSystem.hoverPlatformY);
                drawPosition -= Main.screenPosition;
                drawPosition.Y += 48;
                Vector2 drawScale = new Vector2(1, 2);
                spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, -rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

      
            drawFenix = false;
        }

        orig(self, behindTiles);
    }
}
