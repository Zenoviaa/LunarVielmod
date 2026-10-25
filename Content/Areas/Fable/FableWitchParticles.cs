using Stellamod.Common.Particles;
using Stellamod.Core.Backgrounds;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable;


[Autoload(Side = ModSide.Client)]
public class FableParticles : ModSystem
{
    public static readonly FlyingWitchMiniParticles MiniWitch = new();
    public static readonly FlyingWitchParticles Witch = new();
    public static readonly FlyingLanterns Lantern = new();
    public static readonly FlyingLanterns MiniLantern = new();
    public static readonly FlyingLanternsBig BigLantern = new();
    public static readonly NyoomSparkle NyoomSparkle = new();
    public override void Load()
    {
        base.Load();

        if (Main.netMode == NetmodeID.Server)
            return;

        MiniWitch.Load(Mod);
        Witch.Load(Mod);
        Lantern.Load(Mod);
        MiniLantern.Load(Mod);
        BigLantern.Load(Mod);
        NyoomSparkle.Load(Mod);
    }
    public override void OnModLoad()
    {
        base.OnModLoad();
        CustomBGManager.OnPostDrawCustomBG += DrawWitchParticles;
    }



    public override void OnModUnload()
    {
        base.OnModUnload();
        CustomBGManager.OnPostDrawCustomBG -= DrawWitchParticles;
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        MiniWitch.Update();
        Witch.Update();
        Lantern.Update();
        MiniLantern.Update();
        BigLantern.Update();
        NyoomSparkle.Update();
    }
    private void DrawWitchParticles(OverlayManager manager, SpriteBatch batch, RenderLayers layers, bool arg4)
    {

    }
}
public class FlyingWitchMiniParticles : ParticleUpdater<FlyingWitchMiniParticles.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float scale;
        public float timeLeft;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 2,
        Texture = AssetReferences.Assets.Textures.Fable.Ambient.FlyingWitchSmall.KEY
    };


    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];

            var outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
            var inAlpha = EasingFunction.InOutSine((400 - particle.timeLeft) / 30f);
            var framing = GetParticleFrame(particle.frame);
            var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position + Main.screenPosition);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            drawer.color = particle.color * inAlpha * outAlpha * 0.75F;
            drawer.spriteEffects = particle.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            drawer.rotation = particle.velocity.Y * 0.15f;
            spriteBatch.Draw(drawer);
        }
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        var diff = (Main.screenPosition - Main.screenLastPosition);
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.position += diff * -0.4f * particle.scale;
            particle.velocity.Y = MathF.Sin(particle.timeLeft * 0.03f) * 0.3f;
            particle.timeLeft--;
        }
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(2);
    }

    public override int GetPoolSize()
    {
        return 128;
    }
}
public class FlyingWitchParticles : ParticleUpdater<FlyingWitchParticles.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float scale;
        public float timeLeft;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 4,
        Texture = AssetReferences.Assets.Textures.Fable.Ambient.FlyingWitchBig.KEY
    };

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];

            var outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
            var inAlpha = EasingFunction.InOutSine((400 - particle.timeLeft) / 30f);
            var framing = GetParticleFrame(particle.frame);
            var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position + Main.screenPosition);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            drawer.color = particle.color * inAlpha * outAlpha;
            drawer.spriteEffects = particle.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            drawer.rotation = particle.velocity.Y * 0.15f;
            spriteBatch.Draw(drawer);
        }
    }



    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        var diff = (Main.screenPosition - Main.screenLastPosition);
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.position += diff * -0.7f * particle.scale;
            particle.velocity.Y = MathF.Sin(particle.timeLeft * 0.03f) * 0.3f;
            particle.timeLeft--;
            if (particle.timeLeft % 16 == 0)
            {
                FableParticles.NyoomSparkle.Spawn(new()
                {
                    position = particle.position + new Vector2(0, 16),
                    timeLeft = 60,
                    scale = particle.scale,
                    color = Color.White
                });
            }
        }
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(4);
    }

    public override int GetPoolSize()
    {
        return 128;
    }
}
public class FlyingLanterns : ParticleUpdater<FlyingLanterns.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float scale;
        public float timeLeft;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 3,
        Texture = AssetReferences.Assets.Textures.Fable.Ambient.FloatingFableLantern.KEY
    };



    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (_length <= 0)
            return;
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];

            var outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
            var inAlpha = EasingFunction.InOutSine((1600F - particle.timeLeft) / 30f);
            var framing = GetParticleFrame(particle.frame);
            var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position + Main.screenPosition);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            drawer.color = particle.color * inAlpha * outAlpha * ExtraMath.Osc(0.6f, 1f, speed: 3, offset: i);


            drawer.rotation = particle.rotation;
            spriteBatch.Draw(drawer);
        }


        using(spriteBatch.Ctx(spriteBatch.Parameters with { blendState = CustomBlendStates.Max }))
        {
            for (var i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];

                var outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
                var inAlpha = EasingFunction.InOutSine((1600F - particle.timeLeft) / 30f);

                var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, particle.position + Main.screenPosition);
                drawer.CenterOrigin();
                drawer.color = Color.DarkOrange * inAlpha * outAlpha * ExtraMath.Osc(0.6f, 1f, speed: 3, offset: i) * 0.3f;
                drawer.color.A = 0;
                drawer.rotation = particle.rotation;
                drawer.worldPosition.Y += 8;
                drawer.scale *= 0.4f;
                spriteBatch.Draw(drawer);
            }
        }

    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();

        var diff = (Main.screenPosition - Main.screenLastPosition);
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.position += diff * -0.6f * particle.scale;
            particle.velocity.Y = MathHelper.Lerp(particle.velocity.Y, -1f, 0.1f);
            particle.velocity.X = MathF.Sin(particle.timeLeft * 0.03f) * 0.03f;
            particle.velocity.X += 0.35f;
            particle.rotation = ExtraMath.Osc(-0.05f, 0.05f, speed: 2, offset: i);
            particle.timeLeft--;
        }
    }

    public override void OnSpawn(ref Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(3);
    }

    public override int GetPoolSize()
    {
        return 256;
    }
}
public class FlyingLanternsBig : ParticleUpdater<FlyingLanterns.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float scale;
        public float timeLeft;
        public byte frame;
        public bool IsActive => timeLeft > 0;
    }

    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 2,
        Texture = AssetReferences.Assets.Textures.Fable.Ambient.FloatingFableLanternBig.KEY
    };

    public override void LoadSafe()
    {
        base.LoadSafe();
        On_Main.DrawDust += DrawFrontLanterns;
    }

    public override void OnSpawn(ref FlyingLanterns.Data particle, in int index)
    {
        base.OnSpawn(ref particle, index);
        particle.frame = (byte)Main.rand.Next(2);
    }

    private void DrawFrontLanterns(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_length <= 0)
            return;
        using (Main.spriteBatch.Ctx(SpritebatchParams.InWorldAndZoomed()))
        {
            Draw(Main.spriteBatch, Main.screenPosition);
        }

    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (_length <= 0)
            return;
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];

            var outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
            var inAlpha = EasingFunction.InOutSine((1600F - particle.timeLeft) / 30f);
            var framing = GetParticleFrame(particle.frame);
            var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position + Main.screenPosition);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            drawer.color = particle.color * inAlpha * outAlpha * ExtraMath.Osc(0.6f, 1f, speed: 3, offset: i);


            drawer.rotation = particle.rotation;
            spriteBatch.Draw(drawer);
        }


        using (spriteBatch.Ctx(spriteBatch.Parameters with { blendState = CustomBlendStates.Max }))
        {
            for (var i = 0; i < _length; i++)
            {
                ref var particle = ref _particles[i];

                var outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
                var inAlpha = EasingFunction.InOutSine((1600F - particle.timeLeft) / 30f);

                var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, particle.position + Main.screenPosition);
                drawer.CenterOrigin();
                drawer.color = Color.DarkOrange * inAlpha * outAlpha * ExtraMath.Osc(0.6f, 1f, speed: 3, offset: i) * 0.3f;
                drawer.color.A = 0;
                drawer.rotation = particle.rotation;
                drawer.worldPosition.Y += 8;
                drawer.scale *= 0.7f;
                spriteBatch.Draw(drawer);
            }
        }

    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();

        var diff = (Main.screenPosition - Main.screenLastPosition);
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += particle.velocity;
            particle.position += diff * -0.6f * particle.scale * 3;
            particle.velocity.Y = MathHelper.Lerp(particle.velocity.Y, -1f, 0.1f);
            particle.velocity.X = MathF.Sin(particle.timeLeft * 0.03f) * 0.03f;
            particle.velocity.X += 0.35f;
            particle.rotation = ExtraMath.Osc(-0.05f, 0.05f, speed: 2, offset: i);
            particle.timeLeft--;
        }
    }

    public override int GetPoolSize()
    {
        return 256;
    }
}

public class NyoomSparkle : ParticleUpdater<NyoomSparkle.Data>
{
    public struct Data : IParticleData
    {
        public Color color;
        public Vector2 position;
        public float scale;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }
    public override ParticleFrameData FrameData => base.FrameData with
    {
        FrameCount = 1,
        Texture = AssetReferences.Assets.Textures.Particle.LilSparkle.KEY
    };
    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (_length <= 0)
            return;
        for (var i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];

            var outAlpha = EasingFunction.InOutSine(particle.timeLeft / 30f);
            var framing = GetParticleFrame(0);
            var drawer = SpritebatchDrawer.FromTextureAsset(framing.texture, particle.position + Main.screenPosition);
            drawer.sourceRect = framing.frame;
            drawer.CenterOrigin();
            drawer.color = particle.color * outAlpha * ExtraMath.Osc(0.6f, 1f, speed: 3, offset: i);
            drawer.scale *= outAlpha;
            spriteBatch.Draw(drawer);
        }
    }
    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        var diff = (Main.screenPosition - Main.screenLastPosition);
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            particle.position += diff * -0.7f * particle.scale;
            particle.timeLeft--;
        }
    }

    public override int GetPoolSize()
    {
        return 512;
    }
}
