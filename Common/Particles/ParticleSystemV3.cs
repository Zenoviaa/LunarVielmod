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
    public static CloudDust CloudBackgroundDust;
    public static CloudDust CloudBackgroundDust2;
    public static CloudDust CloudBackgroundDust3;
    public override void Load()
    {
        base.Load();

        BitDust = new();
        RagingFlameDust = new();
        CloudBackgroundDust = new();
        CloudBackgroundDust2 = new();
        CloudBackgroundDust3 = new();
        _particleUpdaters = new List<IParticleUpdater>
        {
            BitDust,
            RagingFlameDust,
            CloudBackgroundDust,
            CloudBackgroundDust2,
            CloudBackgroundDust3
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
        //     RagingFlameDustTest();

        return;


        for(int i = 0; i < 6; i++)
        {
            float depth = 0;
            Rectangle rect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            CloudBackgroundDust.bottomLeft = rect.BottomLeft();
            CloudBackgroundDust.bottomRight = rect.BottomRight();
            CloudBackgroundDust.topRight = rect.TopRight();
            CloudBackgroundDust.topRight.X -= 384;

            Color DepthColor()
            {
                return DrawUtilities.InterpolateColorArray(depth, Color.LightPink, Color.DarkRed, Color.Black) * 0.3f;
            }

            depth = 0.2f;
            Color color = DepthColor();

            CloudBackgroundDust.Spawn(CloudDustParticleData.Default with
            {
                position = new Vector2(-256),
                color = color.ToVector4(),
                velocity = -Vector2.UnitY,
                frameIndex = Main.rand.Next(4),
                rotation = Main.rand.NextFloat(0f, 6f),
                timeLeft = 960,
                originPoint = Main.rand.NextFloat(0f, 0.1f)
            });

            CloudBackgroundDust2.bottomLeft = rect.BottomLeft();
            CloudBackgroundDust2.bottomLeft.X -= 333;
            CloudBackgroundDust2.bottomRight = rect.BottomRight();
            CloudBackgroundDust2.bottomRight.X -= 333;
            CloudBackgroundDust2.topRight = rect.TopRight();
            CloudBackgroundDust2.topRight.X -= 666;


            depth = 0.8f;
            color = DepthColor();


            CloudBackgroundDust2.Spawn(CloudDustParticleData.Default with
            {
                position = new Vector2(-256),
                color = color.ToVector4(),
                velocity = -Vector2.UnitY,
                frameIndex = Main.rand.Next(4),
                rotation = Main.rand.NextFloat(0f, 6f),
                timeLeft = 960,
                originPoint = Main.rand.NextFloat(0f, 0.1f)
            });




            CloudBackgroundDust3.bottomLeft = rect.BottomLeft();
            CloudBackgroundDust3.bottomLeft.X -= 444;
            CloudBackgroundDust3.bottomLeft.Y -= 128;
            CloudBackgroundDust3.bottomRight = rect.BottomRight();
            CloudBackgroundDust3.bottomRight.X -= 444;
            CloudBackgroundDust3.bottomRight.Y -= 128;
            CloudBackgroundDust3.topRight = rect.TopRight();
            CloudBackgroundDust3.topRight.X -= 666;
            CloudBackgroundDust3.topRight.Y -= 128;

            depth = 0.5f;
            color = DepthColor();


            CloudBackgroundDust3.Spawn(CloudDustParticleData.Default with
            {
                position = new Vector2(-256),
                color = color.ToVector4(),
                velocity = -Vector2.UnitY,
                frameIndex = Main.rand.Next(4),
                rotation = Main.rand.NextFloat(0f, 6f),
                timeLeft = 960,
                originPoint = Main.rand.NextFloat(0f, 0.1f)
            });
        }
        if(Main.GameUpdateCount % 1 == 0)
        {



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
