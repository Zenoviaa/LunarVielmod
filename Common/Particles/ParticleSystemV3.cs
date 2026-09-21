using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles;

//Helper methods for spawning particles
public sealed class Particles : ModSystem
{
    private List<IParticleUpdater> _particleUpdaters;

    //TODO: look into source genning these

    /// <summary>
    /// A glowy dust that stretches and collides with particles
    /// </summary>
    public static readonly BitDust BitDust = new();

    /// <summary>
    /// A firey dust used for raging flame torches
    /// </summary>
    public static readonly RagingFlameDust RagingFlameDust = new();
    
    public static readonly FaintSmokeDust FaintSmokeDust = new();
    public static readonly CinderEmberDust CinderEmberDust = new();
    public static readonly CinderEmberDustBackground CinderEmberDustBackground = new();
    public static readonly SwirlingFlameDust SwirlingFlameDust = new();
    public static readonly RoarDust RoarDust = new();
    public static readonly FeatherDust FeatherDust = new();
    public static readonly AbyssFloatingFlowerDust AbyssFloatingFlowerDust = new();

    /// <summary>
    /// A circle particle that draws on the water target, creating the illusion of splashing water
    /// </summary>
    public static readonly WaterDust WaterDust = new();
    public static readonly BloodyMurderDust BloodyMurderDust = new();
    public static readonly WaterfallCrashDust WaterfallCrashDust = new();
    public static readonly TinyWhiteMothDust TinyWhiteMothDust = new();
    public static readonly InDonutDust InDonutDust = new();
    public static readonly CometMagicDust CometMagicDust = new();
    public static readonly CrackImpactDust CrackDust = new();
    public static readonly GoldenLeaf GoldenLeaf = new();
    public static readonly GoldenLeaf GoldenLeafTornado = new();
    public static readonly Sparklemist Sparklemist = new();
    public static readonly FallingBigGoldenLeaf FallingBigGoldenLeaf = new();
    public override void Load()
    {
        base.Load();
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
            AbyssFloatingFlowerDust,
            FeatherDust,
            BloodyMurderDust,
            WaterfallCrashDust,
            TinyWhiteMothDust,
            InDonutDust,
            CometMagicDust,
            CrackDust,
            GoldenLeaf,
            GoldenLeafTornado,
            Sparklemist,
            FallingBigGoldenLeaf
        };

        if (Main.netMode == NetmodeID.Server)
            return;

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
        if (Main.netMode == NetmodeID.Server)
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
