using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.VerliaBoss.Projectiles;

public class VerliaDesperationMoon : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float GrowthCount => ref Projectile.ai[1];
    private ref float ShootFlash => ref Projectile.ai[2];
    private float _scale;
    private float _flashAlpha;
    private float _magicCircleAlpha;
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private Asset<Texture2D> _shadowMoonTextureAsset;
    private Asset<Texture2D> _magicCircleTextureAsset;
    public bool verliaHold;
    public int verliaNpc;
    public bool ready;
    public int holdState;
    public float throwTimer;
    public bool crushMe;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(verliaHold);
        writer.Write(verliaNpc);
        writer.Write(ready);
        writer.Write(holdState);
        writer.Write(throwTimer);
        writer.Write(crushMe);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        verliaHold = reader.ReadBoolean();
        verliaNpc = reader.ReadInt32();
        ready = reader.ReadBoolean();
        holdState = reader.ReadInt32();
        throwTimer = reader.ReadSingle();
        crushMe=reader.ReadBoolean();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _flashAlpha = 1f;
        Projectile.width = 192;
        Projectile.height = 192;
        Projectile.hostile = false;
        Projectile.timeLeft = 2000;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        
        if(GrowthCount < 3)
        {
            Timer++;
            if(Timer == 1)
            {
                if(GrowthCount == 0)
                {
                    SoundStyle e = new SoundStyle($"Stellamod/Assets/Sounds/StarCharge");
                    SoundEngine.PlaySound(e, Projectile.position);
                }

                if(Main.netMode != NetmodeID.Server)
                {
                    ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    shaderSystem.TintScreen(Color.LightBlue, 0.2f, 15);
                    PixelPrimitiveCircleFactory.CreateVerliaMoonBoom(Projectile.Center);
                }




                SoundStyle inSound = AssetRegistry.Sounds.Verlia.BigMoonGrow;
                inSound.Pitch = MathHelper.Lerp(0f, 1f, (GrowthCount + 1f) / 3f);
                SoundEngine.PlaySound(inSound);
                _flashAlpha = 1f;
            }

            if(Timer % 2 == 0)
            {
                float range = Main.rand.NextFloat(252, 512);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
                Vector2 vel = (Projectile.Center - pos);
                vel *= 0.1f;
                FXUtil.GlowStretch(pos, vel);
            }
            if (Timer % 2 == 0)
            {
                float range = Main.rand.NextFloat(384, 666);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
                Vector2 vel = (Projectile.Center - pos);
                vel *= 0.1f;
                var fx = FXUtil.GlowStretch(pos, vel);
                fx.OuterGlowColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
                fx.VectorScale *= 0.5f;
            }
            float maxScale = MathHelper.Lerp(0f, 1f, (GrowthCount+1) / 3f);
            _scale = MathHelper.Lerp(_scale, maxScale, 0.1f);
            _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);
            if (Timer >= 90)
            {
                Timer = 0;
                GrowthCount++;
            }
            return;

        }
        Timer++;
        NPC parent = Main.npc[verliaNpc];
        bool noVerl = !parent.active || parent.type != ModContent.NPCType<Verlia>();
        if (noVerl)
        {
            holdState = 4;
            Timer = 800;
        }

        if (Timer >= 60 && Timer < 700 && !noVerl)
        {
            _magicCircleAlpha = MathHelper.Lerp(_magicCircleAlpha, 1f, 0.1f);
            int divisor = (int)MathHelper.Lerp(40, 20, EasingFunction.InOutSine(Timer / 400));
            if (Timer % divisor == 0)
            {
                if (this.OwnedByLocalClient())
                {
                    Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 4000);
                    if (player != null)
                    {
                        Vector2 velocity = player.Center - Projectile.Center;
                        velocity = velocity.SafeNormalize(Vector2.Zero);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + velocity * 192, velocity * 1400,
                            ModContent.ProjectileType<MoonBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                }
            }
        }
        else
        {

            _magicCircleAlpha = MathHelper.Lerp(_magicCircleAlpha, 0f, 0.1f);
            switch (holdState)
            {
                case 0:
                    {
                        if (verliaHold)
                        {
                            ShakeModSystem.Shake = 2;
                            Vector2 targetPosition = parent.Center;
                            targetPosition.Y -= 262;
                            if (Projectile.Center.Y < targetPosition.Y)
                            {
                                if (Projectile.velocity.Y < 5)
                                {
                                    Projectile.velocity.Y += 0.2f;
                                }
                            }
                            else
                            {
                                holdState++;
                                Projectile.velocity.Y *= 0.5f;
                            }
                        }
          
                    }
                    break;
                case 1:
                    {
                        Projectile.velocity.Y *= 0.5f;
                    }
                    break;
                case 2:
                    {
                        throwTimer++;
                        Projectile.velocity.Y = MathHelper.Lerp(2.5f, -5f, EasingFunction.InOutSine(throwTimer / 30f));
                        if(throwTimer > 30f)
                        {
                            throwTimer = 0;
                            holdState++;
                        }
                    }
                    break;
                case 3:
                    {
                        Player target = Main.player[parent.target];
                        float x = (target.Center.X > parent.Center.X) ? 1 : -1;
                        if(!crushMe)
                            Projectile.velocity.X = x * 5;
                        Projectile.velocity.Y = -16;
                        holdState++;
                    }
                    break;
                case 4:
                    {
           
                        if (Timer % 8 == 0)
                        {
                            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Bottom, -Projectile.velocity);
                            p2.Scale *= 3f;
                        }
                        if (Projectile.velocity.Y < 1)
                            Projectile.velocity.Y += 0.5f;
                        else
                        {
                            throwTimer++;
                            if (throwTimer == 30)
                            {
                                SoundEngine.PlaySound(AssetRegistry.Sounds.Bishinine.BishinineFastfall, Projectile.position);
                            }
                            Projectile.velocity.Y *= 1.05f;
                            Projectile.tileCollide = true;
                        }
                    }
                    break;
            }
        }


        _scale = MathHelper.Lerp(_scale, 1f, 0.1f);
        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);
        ShootFlash = MathHelper.Lerp(ShootFlash, 0f, 0.1f);
    }
    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        Vector2 scale = Vector2.One * _scale;

        SpritebatchDrawer circleSprite = SpritebatchDrawer.FromTextureAsset(_magicCircleTextureAsset, Projectile.Center);
        circleSprite.color = Color.Lerp(Color.Black, Color.White, _flashAlpha + ShootFlash);// * ExtraMath.Osc(0.5f, 1f, speed: 6);
        circleSprite.color.A = 0;
        circleSprite.rotation = Main.GlobalTimeWrappedHourly;
        circleSprite.scale *= 1.2f;
        sb.Draw(circleSprite);

        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);



        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.Blue, Color.White, _flashAlpha + ShootFlash) * 0.8f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.8f;
        glowDrawer.scale *= scale;
        Main.spriteBatch.Draw(glowDrawer);



        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.White * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= scale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 0.6f * 1;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);


        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.White; // Color.Lerp(Color.White, Color.DarkBlue, 0.5f);
        moonSprite.scale *= scale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.White * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= scale * 3f;
        Main.spriteBatch.Draw(glowDrawer);

    }
    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 scale = Vector2.One * _scale;
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        _shadowMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Shadow");
        _magicCircleTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Sigil");

        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowMoonTextureAsset, Projectile.Center);

        Color flashColor = Color.White;
        Color darkColor = Color.Lerp(Color.Blue, Color.Black, 0.8f) * 0.5f;
        shadowDrawer.color = Color.Lerp(darkColor, flashColor, _flashAlpha + ShootFlash);
        shadowDrawer.scale *= scale * 1.05f;
        Main.spriteBatch.Draw(shadowDrawer);

        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Red;
        outlineDrawer.scale *= scale;
     //   Main.spriteBatch.Draw(outlineDrawer);

        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        moonSprite.scale = scale * 1.05f;
        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha + ShootFlash);
        Main.spriteBatch.Draw(moonSprite);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        Point tile = Projectile.Center.ToTileCoordinates();
        tile.Y -= 6;
        tile = TileUtilities.FallToSolidTile(tile);
        tile.Y -= 1;
        Vector2 pos = tile.ToWorldCoordinates();

        if (this.OwnedByLocalClient())
        {
            float numBlades = 12;
            for (float f = 0; f < numBlades; f++)
            {
                float ratio = f / numBlades;
                Vector2 vel = (ratio * MathHelper.TwoPi).ToRotationVector2();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + vel * 128, vel * 15, ModContent.ProjectileType<MoonBlade>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
        fx.Scale *= 8f;
        float numDust = 32;
        for (float f = 0; f < numDust; f++)
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Blue;
            spawnParams.scaleRange *= 2;
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16), spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
        }
        FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY, 32, 2, 32);
    }
}
