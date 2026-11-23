using ReLogic.Content;
using Stellamod.Common.Particles;
using Stellamod.Content.Biomes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD;

public class HangingCageExplosionProjectile : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = true;
        Projectile.timeLeft = 4;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            //EXPLODE
            float boomSize = MoonUtils.Rand.NextFloat(0.15f, 0.2f);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);
            fx.Scale *= 2f;

            FXUtil.ShakeCamera(Projectile.Center, 1024, 24);

            SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/ExplosionBurstBomb");
            explosionSound.PitchVariance = 0.7f;
            SoundEngine.PlaySound(explosionSound, Projectile.position);

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Moonly, new Vector2(5, 35));
            fac.particleCount *= 3;
            ParticleUtils.CreateSwirlingDustCircle(fac);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Pink, 16, 200);
            for (var x = 0; x < 6; x++)
            {
                var pos = Projectile.Center + MoonUtils.Rand.NextVector2CircularEdge(128, 128);
                var vel = (pos - Projectile.Center) * 0.04f;
                var fx2 = FXUtil.GlowStretch(pos, vel);
                fx2.VectorScale.Y *= 0.3F;
            }
            {
                var pos = Projectile.Center;
                var vel = (pos - Projectile.Center) * 0.05f;
                PS.SmokeBoom.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    rot = MoonUtils.Rand.NextFloat(0, 6),
                    color = Color.White
                });
            }

            for (var x = 0; x < 8; x++)
            {
                var vel = MoonUtils.Rand.NextVector2CircularEdge(8, 8);
                PS.SmokeImpact.Spawn(new()
                {
                    position = Projectile.Center + vel * 2,
                    velocity = vel,
                    rot = vel.ToRotation(),
                    color = Color.White
                });
            }
        }
    }
}

public class FreeMoth : ModNPC
{
    int _mothFrame;
    public override string Texture => AssetReferences.Content.Areas.WondrousDarkspace.NPCsWD.HangingCage_Moth.KEY;
    ref float Timer => ref NPC.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 4;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 32;
        NPC.damage = 12;
        NPC.lifeMax = 100;
        NPC.defense = 4;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.dontCountMe = true;
        NPC.friendly = true;
    }
    
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override bool CanHitNPC(NPC target)
    {
        return false;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 5 == 0)
        {
            _mothFrame++;
            _mothFrame %= Main.npcFrameCount[Type];
        }
        NPC.velocity.X = MathF.Sin(Timer * 0.05F);
        NPC.velocity.Y = -1f;
        NPC.rotation = NPC.velocity.X * 0.05F;
        Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.6f);
        if (Timer >= 180)
        {
            NPC.active = false;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D mothTexture = TextureAssets.Npc[Type].Value;
        var drawPos = NPC.Center - Main.screenPosition;
        Rectangle mothFrame = mothTexture.GetFrame(_mothFrame, 4);
        Vector2 hoverOffset = Vector2.Lerp(Vector2.Zero, Vector2.UnitY * 8, ExtraMath.Osc(0f, 1f, speed: 4, offset: NPC.whoAmI));
        var alpha = MathHelper.Lerp(1f, 0f, Timer / 180F);
        spriteBatch.Draw(mothTexture, drawPos + hoverOffset, mothFrame, Color.White.MultiplyRGB(drawColor) * alpha, NPC.rotation, mothFrame.Size() / 2f, Vector2.One, SpriteEffects.None, 0);


        Texture2D texture2D4 = AssetReferences.Assets.NoiseTextures.DimLight.Asset.Value;
        Main.spriteBatch.Draw(texture2D4, drawPos + hoverOffset, null, new Color(255, 128, 125, 0) * alpha, NPC.rotation, new Vector2(32, 32), 0.17f, SpriteEffects.None, 0f);
        return false;
    }
}

public class HangingCage : ModNPC,
    IDrawToRenderTarget
{
    Asset<Texture2D> _smallTextureAsset;
    Asset<Texture2D> _chainTextureAsset;
    Asset<Texture2D> _mothTextureAsset;
    private int _mothFrame;
    private ref float Timer => ref NPC.ai[0];
    private enum AIState
    {
        Idle,
        Exploding
    }
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float HangingDistance => ref NPC.ai[2];
    private bool IsSmall
    {
        get => NPC.ai[3] == 1;
        set => NPC.ai[3] = value ? 1 : -1;
    }

    private Vector2 _scale;
    private Color OutlineColor;
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _scale = Vector2.One;
        NPC.width = 32;
        NPC.height = 32;
        NPC.lifeMax = 100;
        NPC.defense = 4;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;
        NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
        NPC.damage = 12;
    }

    public override void AI()
    {
        base.AI();
        NPCSets.Heavy[Type] = true;
        NPC.rotation = MathHelper.Lerp(-0.05f, 0.05f, ExtraMath.Osc(0f, 1f, offset: NPC.whoAmI));
        Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.6f);
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Exploding:
                AI_Exploding();
                break;
        }
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }



    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _chainTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Chain");
        string texturePath = IsSmall ? Texture + "_Small" : Texture;
        _smallTextureAsset ??= ModContent.Request<Texture2D>(texturePath);
        _mothTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Moth");
        Texture2D mothTexture = _mothTextureAsset.Value;
        Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);


        float length = HangingDistance;
        int index = 0;
        while (length > 0f)
        {
            Vector2 chainDrawOrigin = _chainTextureAsset.Size() / 2;
            Vector2 offset = -Vector2.UnitY * _chainTextureAsset.Height() * index;
            Vector2 vel = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2();
            spriteBatch.Draw(_chainTextureAsset.Value, drawPos + offset - vel * NPC.frame.Height / 2, null, Color.White.MultiplyRGB(drawColor),
                NPC.rotation, chainDrawOrigin, _scale, SpriteEffects.None, 0);

            length -= _chainTextureAsset.Height();
            index++;
        }

        Vector2 drawOrigin = NPC.frame.Size() / 2;

        SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


        this.DrawOutline(OutlineColor, yOffset: 0, _scale, texturePath, null);

        Rectangle mothFrame = mothTexture.GetFrame(_mothFrame, 4);
        Vector2 hoverOffset = Vector2.Lerp(Vector2.Zero, Vector2.UnitY * 8, ExtraMath.Osc(0f, 1f, speed: 4, offset: NPC.whoAmI));
        spriteBatch.Draw(mothTexture, drawPos + new Vector2(0, 8) + hoverOffset, mothFrame, Color.White.MultiplyRGB(drawColor), NPC.rotation, mothFrame.Size() / 2f, _scale, spriteEffects, 0);
        spriteBatch.Draw(_smallTextureAsset.Value, drawPos, null, Color.White.MultiplyRGB(drawColor), NPC.rotation, _smallTextureAsset.Size() / 2f, _scale, spriteEffects, 0);

        Texture2D texture2D4 = AssetReferences.Assets.NoiseTextures.DimLight.Asset.Value;
        Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color(255, 128, 125, 0), NPC.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
        return false;
    }
    private void AI_Idle()
    {
        Timer++;
        if (MoonUtils.Rand.NextBool(25))
        {
            float xRand = MoonUtils.Rand.NextFloat(-32, 32);
            float yRand = MoonUtils.Rand.NextFloat(-32, 32);
            PS.FallingDarkspaceStar.Spawn(new()
            {
                position = NPC.Center + new Vector2(xRand, yRand),
                timeLeft = 180,
                color = Color.White,
            });
        }

        if (Timer % 10 == 0)
        {
            _mothFrame++;
            if (_mothFrame >= 4f)
            {
                _mothFrame = 0;
            }
        }

        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                float targetBeamLength = ProjectileHelper.PerformBeamHitscan(NPC.Center, -Vector2.UnitY, 1200);
                NPC.Center = NPC.Center - Vector2.UnitY * targetBeamLength;

                HangingDistance = MoonUtils.Rand.NextFloat(300, 650);
                NPC.Center += Vector2.UnitY * HangingDistance;

                if (MoonUtils.Rand.NextBool(3))
                {
                    IsSmall = true;
                    NPC.netUpdate = true;
                }
            }
        }

        NPC.TargetClosest();
        if (NPC.HasValidTarget)
        {
            const float SQR_EXPLODING_RADIUS = 96 * 96;
            Player target = Main.player[NPC.target];
            float distanceToTarget = Vector2.DistanceSquared(NPC.Center, target.Center);
            if (distanceToTarget <= SQR_EXPLODING_RADIUS)
            {
                SwitchState(AIState.Exploding);
            }
        }
    }

    private void AI_Exploding()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundStyle windUpSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_WaveCharge");
            windUpSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(windUpSound, NPC.position);

            PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.Pink, Color.White, 60, 256);
        }

        if (Timer % 6 == 0)
        {
            var offset = MoonUtils.Rand.NextVector2CircularEdge(256, 256);
            var spawnPos = NPC.Center + offset;
            var spawnVelocity = NPC.Center - spawnPos;
            spawnVelocity *= 0.04f;

            var fx = FXUtil.GlowStretch(spawnPos, spawnVelocity);
            fx.InnerColor = Color.White;
            fx.GlowColor = Color.LightPink;
            fx.OuterGlowColor = Color.DarkBlue;
        }

        OutlineColor = Color.Lerp(OutlineColor, Color.Yellow, 0.1f);
        if (Timer >= 60f)
        {
            if (Main.netMode != NetmodeID.Server)
            {

                this.MakeGore("_Gore_Top", NPC.position, NPC.velocity + new Vector2(0, -1));
                this.MakeGore("_Gore_Bottom", NPC.position, NPC.velocity + new Vector2(-1, 0));

            }

            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<HangingCageExplosionProjectile>(), 37, 2, Main.myPlayer);
            }
            NPC.Kill();
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HypnotizedSoul>(), minimumDropped: 2, maximumDropped: 4));
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!spawnInfo.Player.ZoneWonder)
            return 0;
        return ScarletSpawnChance.Wondrous_Spawn_Rate;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);

        if (NPC.life <= 0)
        {
            if (Main.netMode != NetmodeID.Server)
            {

                this.MakeGore("_Gore_Top", NPC.position, NPC.velocity + new Vector2(0, -1));
                this.MakeGore("_Gore_Bottom", NPC.position, NPC.velocity + new Vector2(-1, 0));

            }

            if (MultiplayerHelper.IsHost)
            {
                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FreeMoth>());
            }

        }
    }


    public void DrawToRenderTargets()
    {

    }
}
