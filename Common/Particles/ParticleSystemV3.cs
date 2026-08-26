using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles;

//Helper methods for spawning particles
[Autoload(Side = ModSide.Client)]
public sealed class Particles : ModSystem
{
    private List<IParticleUpdater> _particleUpdaters;

    public static BitDust BitDust;
    public static RagingFlameDust RagingFlameDust;
    public static FaintSmokeDust FaintSmokeDust;
    public static CinderEmberDust CinderEmberDust;
    public static CinderEmberDustBackground CinderEmberDustBackground;
    public static SwirlingFlameDust SwirlingFlameDust;
    public static RoarDust RoarDust;
    public override void Load()
    {
        base.Load();

        BitDust = new();
        RagingFlameDust = new();
        FaintSmokeDust = new();
        CinderEmberDust = new();
        CinderEmberDustBackground = new();
        SwirlingFlameDust = new();
        RoarDust = new();
        _particleUpdaters = new List<IParticleUpdater>
        {
            BitDust,
            RagingFlameDust,
            FaintSmokeDust,
            CinderEmberDust,
            CinderEmberDustBackground,
            SwirlingFlameDust,
            RoarDust
        };

        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            if (_particleUpdaters[i] is ILoadable loadable)
            {
                loadable.Load(Mod);
            }
        }
    }

    public override void Unload()
    {
        base.Unload();
        if (_particleUpdaters == null)
            return;

        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            if (_particleUpdaters[i] is ILoadable loadable)
            {
                loadable.Unload();
            }
        }
        _particleUpdaters.Clear();
        _particleUpdaters = null;
    }


    private void UpdateParticles(object? state)
    {

        void UpdateParticles_Inner()
        {
            double oldUpdate = Main.GameUpdateCount;

            while (true)
            {
                double newUpdate = Main.GameUpdateCount;
                if (newUpdate != oldUpdate)
                {
                    oldUpdate = newUpdate;
                    for (int i = 0; i < _particleUpdaters.Count; i++)
                    {
                        _particleUpdaters[i].Update();
                    }
                }
            }
        }
        UpdateParticles_Inner();
    }
    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            _particleUpdaters[i].Update();
        }
    }

    private void RagingFlameDustTest()
    {
        if (Main.mouseLeft && Main.GameUpdateCount % 2 == 0)
        {
            RagingFlameDust.Spawn(RagingFlameDustData.Default with { position = Main.MouseWorld, timeleft = 70 });
        }
    }

    private void BitDustPerfTest()
    {
        if (Main.mouseLeft)
        {
            BitDustFactory factory = BitDustFactory.Default;
            factory.position = Main.MouseWorld;
            factory.outerColor = Main.DiscoColor.ToVector4();
            factory.innerColor = factory.outerColor;
            for (int i = 0; i < 100; i++)
            {


                factory.velocity = Main.rand.NextVector2Circular(16, 16);
                BitDust.Spawn(factory);
            }
        }

    }
    public override void PostDrawTiles()
    {
        base.PostDrawTiles();

        //Just for testing the atlas
        /*
        Main.spriteBatch.Begin();
        var time = BitDust.elapsedString;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, time, Main.Camera.Center - Main.screenPosition + new Vector2(-144, -128), Color.White, 0, Vector2.Zero, Vector2.One * 1.2f);
        Main.spriteBatch.End();*/
    }
}
