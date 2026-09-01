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

    public static AbyssFloatingFlowerDust AbyssFloatingFlowerDust;
    /// <summary>
    /// A circle particle that draws on the water target, creating the illusion of splashing water
    /// </summary>
    public static WaterDust WaterDust;
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
        WaterDust = new();
        AbyssFloatingFlowerDust = new();
        _particleUpdaters = new List<IParticleUpdater>
        {
            BitDust,
            RagingFlameDust,
            FaintSmokeDust,
            CinderEmberDust,
            CinderEmberDustBackground,
            SwirlingFlameDust,
            RoarDust,
            WaterDust,
            AbyssFloatingFlowerDust
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


    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            _particleUpdaters[i].Update();
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
