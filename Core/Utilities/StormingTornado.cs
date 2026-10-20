using Stellamod.Common.Shaders;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public class StormingTornado : ModProjectile
{
    TornadoMesh _closeCylinderMesh;
    TornadoMesh _cylinderMesh;
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 600;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;

    }
    public override bool PreDraw(ref Color lightColor)
    {

        var spriteBatch = Main.spriteBatch;
        spriteBatch.EndOut(out var oldParameters);
        _cylinderMesh ??= new TornadoMesh(1024, radius: 127, heightPerBand: 256, sidesPerBand: 16);
        _cylinderMesh.SetColor(Color.Lerp(Color.White, Color.Black, 0.75f));
        _closeCylinderMesh ??= new TornadoMesh(1024, 127, 256, 16);
        
        var gDevice = spriteBatch.GraphicsDevice;
        gDevice.RasterizerState = RasterizerState.CullNone;
        gDevice.BlendState = BlendState.AlphaBlend;
        gDevice.SamplerStates[0] = SamplerState.LinearWrap;

        var pass = AssetReferences.Effects.Weather.ThunderingTornado.CreatePrimitivesPass();

        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = 
            Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), Main.GlobalTimeWrappedHourly * 1)
            * Matrix.CreateTranslation(Projectile.Center.X, Projectile.Center.Y, 0) 
            * TrailDrawer.WorldViewPointCullClose;
        pass.Apply();
        Main.graphics.GraphicsDevice.Textures[0]  = AssetReferences.Assets.Noise.TornadoNoise.Asset.Value;
        _cylinderMesh.SetFades(Main.GlobalTimeWrappedHourly, 1f);
        _cylinderMesh.Draw(gDevice);

        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix =
            Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), Main.GlobalTimeWrappedHourly * 1)
            * Matrix.CreateTranslation(Projectile.Center.X, Projectile.Center.Y, 0)
            * TrailDrawer.WorldViewPointCullFarAway;
        pass.Apply();
        gDevice.BlendState = BlendState.AlphaBlend;
        _closeCylinderMesh.SetFades(Main.GlobalTimeWrappedHourly);
        _closeCylinderMesh.Draw(gDevice);
        spriteBatch.Begin(oldParameters);

        return false;
    }
}
