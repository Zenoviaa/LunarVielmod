using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private Rectangle _arenaRectangleToLava;
    private Vector2 _eruptionRight;
    private Vector2 _eruptionLeft;
    private Vector2 EruptionLeft
    {
        get
        {
            if (_eruptionLeft == Vector2.Zero)
                _eruptionLeft = FindEruptionLeft();
            return _eruptionLeft;
        }
    }

    private Vector2 EruptionRight
    {
        get
        {
            if (_eruptionRight == Vector2.Zero)
                _eruptionRight = FindEruptionRight();
            return _eruptionRight;
        }
    }

    private Rectangle ArenaRectangleToLava
    {
        get
        {
            if (_arenaRectangleToLava == Rectangle.Empty)
                _arenaRectangleToLava = ArenaRectangleUpToLava();
            return _arenaRectangleToLava;
        }
    }

    public void CreateFirebreathChargeEffect(Vector2 position)
    {
        for (float f = 0; f < 8; f++)
        {
            Vector2 pos = position + Main.rand.NextVector2Circular(384, 384);
            Vector2 vel = (position - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Turquoise;
            fx.Scale *= 0.5f;
        }

        if (Main.netMode != NetmodeID.Server)
        {
            var screenShader = ModContent.GetInstance<ScreenShaderSystem>();
            screenShader.TintScreen(Color.Red, 0.1f, 15f);
            PixelPrimitiveCircleFactory.CreateRekInwardBoom(position);
        }

        for (float f = 0; f < 12; f++)
        {
            Vector2 pos = position + Main.rand.NextVector2Circular(384, 384);
            Vector2 vel = (position - pos);
            vel *= 0.1f;

            DustParticleSpawnParams spawnparams = DustParticleSpawnParams.Default;
            spawnparams.innerColor = Color.Lerp(Color.White, Color.Red, Main.rand.NextFloat(0f, 1f));
            spawnparams.outerColor = Color.Red;
            var dp = DustParticle.Spawn(pos, vel, spawnparams);
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.Scale *= 0.5f;
        }

        /*
        SoundStyle growSound = AssetRegistry.Sounds.Celestia.BigBowCharge with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(growSound, Projectile.position);
        */
    }
    
    public void CreateSegmentEatEffect(RekSegment segment)
    {
        for (float f = 0; f < 12; f++)
        {
            Vector2 pos = segment.position;
            pos += Main.rand.NextVector2Circular(16, 16);
            Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = pos,
                velocity = Main.rand.NextVector2Circular(18, 18),
                timeLeft = 100,
                innerColor = color.ToVector4(),
                outerColor = Color.Red.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(1f, 2f))
            });
        }

        SoundEngine.PlaySound(SoundID.Item74 with { PitchVariance = 0.6f }, segment.position);
    }
    public Rectangle ArenaRectangleUpToLava()
    {

        Point center = _arenaCenter.ToTileCoordinates();
        int left = center.X;
        int top = center.Y;
        int bottom = center.Y;
        int right = center.X;


        //Find left
        while (left > 0)
        {
            left--;
            Tile tile = Main.tile[left, center.Y];
            if (WorldGen.SolidTile(tile))
                break;
        }

        //Find right
        while (right < Main.maxTilesX - 1)
        {
            right++;
            Tile tile = Main.tile[right, center.Y];
            if (WorldGen.SolidTile(tile))
                break;
        }


        //Find top
        while (top > 0)
        {
            top--;
            Tile tile = Main.tile[center.X, top];
            if (WorldGen.SolidTile(tile))
                break;
        }

        //Find bottom
        while (bottom < Main.maxTilesY - 1)
        {
            bottom++;
            Tile tile = Main.tile[center.X, bottom];
            if (tile.LiquidAmount > 0)
                break;
        }

        Point topLeft = new Point(left, top);
        Point bottomRight = new Point(right, bottom);

        Vector2 topLeftWorld = topLeft.ToWorldCoordinates();
        Vector2 bottomRightWorld = bottomRight.ToWorldCoordinates();

        Point topLeftPoint = topLeftWorld.ToPoint();
        Point bottomRightPoint = bottomRightWorld.ToPoint();
        return  new Rectangle(
            topLeftPoint.X,
            topLeftPoint.Y,
            bottomRightPoint.X - topLeftPoint.X,
            bottomRightPoint.Y - topLeftPoint.Y);
    }
    private void CreateFlameSuckParticles(Vector2 position)
    {
        Vector2 spawnPos = position + Main.rand.NextVector2CircularEdge(444, 444);
        Vector2 spawnVelocity = position - spawnPos;
        spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
        spawnVelocity *= 16;
        /*
        var d = DustParticle.Spawn(spawnPos, spawnVelocity, DustParticleSpawnParams.Default);
        d.Scale *= 0.8f;
        d.gravity = 0;*/
        Particles.BitDust.Spawn(BitDustFactory.Default with { position = spawnPos, velocity = spawnVelocity, timeLeft = 24 });
        if (Main.rand.NextBool(2))
        {
            spawnPos = position + Main.rand.NextVector2CircularEdge(384, 384);
            spawnVelocity = position - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 16;
            var p = FXUtil.GlowStretch(spawnPos, spawnVelocity);
            p.InnerColor = Color.White;
            p.OuterGlowColor = Color.Red;
        }
    }
    /// <summary>
    /// Find the left side of the arena
    /// </summary>
    /// <returns></returns>
    private Vector2 FindEruptionLeft()
    {
        Point centerTile = _arenaCenter.ToTileCoordinates();
        for (int i = 0; i < 200; i++)
        {
            centerTile.Y++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.Y -= 1;
                break;
            }

        }
        for (int i = 0; i < 200; i++)
        {
            centerTile.X--;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.X += 1;
                break;
            }

        }
        return centerTile.ToWorldCoordinates();
    }

    /// <summary>
    /// Find the right side of the arena
    /// </summary>
    /// <returns></returns>
    private Vector2 FindEruptionRight()
    {
        Point centerTile = _arenaCenter.ToTileCoordinates();
        for (int i = 0; i < 200; i++)
        {
            centerTile.Y++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.Y -= 1;
                break;
            }

        }
        for (int i = 0; i < 200; i++)
        {
            centerTile.X++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.X--;
                break;
            }

        }
        return centerTile.ToWorldCoordinates();
    }
}
