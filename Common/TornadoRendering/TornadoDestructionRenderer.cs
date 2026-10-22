using Stellamod.Common.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.TornadoRendering;

[Autoload(Side = ModSide.Client)]
public class TornadoDestructionRenderer : ModSystem
{
    public class DestructonMaskParticle : ParticleUpdater<DestructonMaskParticle.Data>
    {
        public struct Data : IParticleData
        {
            public Color color;
            public Vector2 originalPosition;
            public Vector2 position;
            public Vector2 upPosition;
            public Vector2 scale;
            public float rotation;
            public float timeLeft;
            public bool IsActive => timeLeft > 0;
        }
        public override void OnSpawn(ref Data particle, in int index)
        {
            base.OnSpawn(ref particle, index);
            particle.originalPosition = particle.position;

        }

        protected override void UpdateParticles()
        {
            base.UpdateParticles();
          
            for(var i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];
                particle.timeLeft--;
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            spriteBatch.EndOut(out var oldParameters);
     
            using var tileMask = RT.Context(RenderTargets.ScreenTarget);
            using var destructionMask = RT.Context(RenderTargets.ScreenTarget);
            using(RT.Clear(tileMask, Color.Transparent))
            {
                using(spriteBatch.Ctx(SpritebatchParams.InWorldAndZoomed() with {  matrix = Matrix.Identity }))
                {
                    spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);
                }
            }
          //  Main.NewText("whoa");
            using(RT.Clear(destructionMask, Color.Transparent))
            {
                var texAsset = AssetReferences.Assets.Textures.Particle.TornadoDestructionParticle.Asset;
                var pass = AssetReferences.Effects.Weather.TornadoDestructionMaskWrite.CreatePixelPass();
                pass.Parameters.spriteSize = texAsset.Value.Size();
                pass.Parameters.texelSize = texAsset.GetTexelSize();
                
                pass.Apply();
                using (spriteBatch.Ctx(SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.Identity, effect = pass.Shader, blendState = CustomBlendStates.Max }))
                {
                    var drawer = SpritebatchDrawer.FromTextureAsset(texAsset, Vector2.Zero);
                    for (var i = 0; i < _length; i++)
                    {
                        ref var particle = ref _particles[i];
          
                        var ratio = (float)particle.timeLeft / 120f;
                        var inEase = EasingFunction.OutExpo(ratio);
                        var ease = MathHelper.Lerp(0, inEase, EasingFunction.InOutSine((120F - particle.timeLeft) / 15F));
                        drawer.worldPosition = particle.position;
                        drawer.color = particle.color * ease;
                        drawer.scale = Vector2.One * particle.scale;
                        drawer.rotation = particle.rotation;
                        spriteBatch.Draw(drawer);
                    }
                }
            }

            var maskPass = AssetReferences.Effects.Weather.TornadoDestructionMaskMerge.CreatePixelPass();
            maskPass.Parameters.spriteSize = tileMask.Size();
            maskPass.Parameters.texelSize = Vector2.One / tileMask.Size();
            maskPass.Parameters.tileSampler = new()
            {
                Texture = tileMask,
                Sampler = SamplerState.LinearClamp
            };
            maskPass.Apply();
            using (spriteBatch.Ctx(SpritebatchParams.InWorldAndZoomed() with { effect = maskPass.Shader }))
            {
                spriteBatch.Draw(destructionMask, Vector2.Zero, Color.White);
            }
            spriteBatch.Begin(oldParameters);
          //  spriteBatch.Draw(destructionMask, Vector2.Zero, Color.White);
        }


    }
    public static readonly DestructonMaskParticle DestructionMaskParticleSystem = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += PrepareMaskDrawing;
    }
    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        DestructionMaskParticleSystem.Update();
    }

    private void PrepareMaskDrawing(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        if (DestructionMaskParticleSystem.Length <= 0)
            return;

        PixelationManager.QueueSpritebatchDrawAction(DestructionMaskParticleSystem.Draw, DrawLayer.OverWater);
    }
}
