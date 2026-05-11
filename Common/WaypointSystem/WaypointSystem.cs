using Microsoft.Xna.Framework.Input;
using MonoMod.Core.Utils;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.BlackSystem;
using Stellamod.Common.Shaders;
using Stellamod.Common.SirestiasShop;
using Stellamod.Common.UI;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner;
using Stellamod.Content.Areas.SpringHills.NPCsSH;
using Stellamod.Content.Armors.Sanctorous;
using Stellamod.Core.Camera;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;
using static Stellamod.WorldG.StructureManager.Snapshot;

namespace Stellamod.Common.WaypointSystem;

public enum OrganWaypoint : byte
{
    WitchTown = 0,
    Marsh = 1,
    Desert = 2,
    Moonspiral = 3,
    ApocalypseTower = 4,
    BloodySanctum = 5,
    Dragonhome = 6,
    Hallowrooms = 7,
    Ishtar = 8,
    Platform = 9,
    RunicaWaterside = 10,
    WonderousDarkspace = 11,
    WorldsEnd = 12,
    MistyDungeon = 13
}

public class OrganDragon : ModNPC
{
    private enum AIState
    {
        SwoopDown,
        Pickup,
        SwoopUp,
        SwoopOut,
                
    }
    private float _mountTimer;
    private ref float Timer => ref NPC.ai[0];
    private int PlayerToTravel => (int)NPC.ai[1];
    private AIState State
    {
        get => (AIState)NPC.ai[2];
        set => NPC.ai[2] = (float)value;
    }
    private ref float TeleportTarget => ref NPC.ai[3];

    private bool _initialized;
    private AnimationFramer _framer;
    private DragonRig _rig;
    private Asset<Texture2D> _headTextureAsset;
    private Asset<Texture2D>[] _bodyTextureAssets;
    private Asset<Texture2D>[] _frontLegTextureAssets;
    private Asset<Texture2D>[] _backLegTextureAssets;
    private Asset<Texture2D>[] _wingTextureAssets;

    private DragonSegment _headSegment;
    private DragonSegment[] _bodySegments;
    private Vector2 _teleportPosition;
    private void LoadTextureAssets()
    {
        if (_initialized)
            return;
        _headTextureAsset = ModContent.Request<Texture2D>(Texture);
        _bodyTextureAssets = new Asset<Texture2D>[8];
        for (int i = 0; i < _bodyTextureAssets.Length; i++)
        {
            _bodyTextureAssets[i] = ModContent.Request<Texture2D>(Texture + "_Body_" + i);
        }
        _frontLegTextureAssets = new Asset<Texture2D>[2];
        for (int i = 0; i < _frontLegTextureAssets.Length; i++)
        {
            _frontLegTextureAssets[i] = ModContent.Request<Texture2D>(Texture + "_FrontLeg_" + i);
        }
        _wingTextureAssets = new Asset<Texture2D>[1];
        for (int i = 0; i < _wingTextureAssets.Length; i++)
        {
            _wingTextureAssets[i] = ModContent.Request<Texture2D>(Texture + "_Wing_" + i);
        }
        _initialized = true;
    }

    private void SetupRig()
    {
        _rig = new DragonRig();

        //Setup head
        _headSegment = new DragonSegment(segmentLength: 48);
        _rig.AddSegment(_headSegment);
        _rig.root = _headSegment;

        //Setup body
        _bodySegments = new DragonSegment[8];
        int[] bodyWidths = new int[8];
        bodyWidths[0] = 30;
        bodyWidths[1] = 20;
        bodyWidths[2] = 20;
        bodyWidths[3] = 20;
        bodyWidths[4] = 20;
        bodyWidths[5] = 20;
        bodyWidths[6] = 20;
        bodyWidths[7] = 20;
        for (int i = 0; i < _bodySegments.Length; i++)
        {
            DragonSegment bodySegment = new DragonSegment(segmentLength: bodyWidths[i]);
            if (i == 0)
            {
                bodySegment.parent = _headSegment;
            }
            else
            {
                bodySegment.parent = _bodySegments[i - 1];
            }
            _rig.AddSegment(bodySegment);
            _bodySegments[i] = bodySegment;
        }

        //_frontLegSegments
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        SetupRig();
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 32;
        NPC.height = 32;
        NPC.aiStyle = 0;
        NPC.damage = 90;
        NPC.defense = 42;
        NPC.lifeMax = 200;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.dontCountMe = true;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
    }

    public override void AI()
    {
        base.AI();
        if (_teleportPosition != Vector2.Zero)
        {
            NPC.position.X = _teleportPosition.X;
            NPC.position.Y = _teleportPosition.Y;
            _teleportPosition = Vector2.Zero;
        }

        switch (State)
        {
            case AIState.SwoopDown:
                AI_SwoopDown();
                break;
            case AIState.Pickup:
                AI_Pickup();
                break;
            case AIState.SwoopUp:
                AI_SwoopUp();
                break;
            case AIState.SwoopOut:
                AI_SwoopOut();
                break;
        }
        _framer.maxFrame = 120;
        _framer.frameSpeed = 1;
        _framer.UpdateTick();
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }



    private void AI_SwoopDown()
    {
        Timer++;
        if(Timer == 1)
        {
        
            NPC.TargetClosest();
        }

        Player target = Main.player[PlayerToTravel];
        Vector2 targetPosition = target.Center;
        Vector2 targetVelocity = targetPosition - NPC.Center;
        targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
        float speed = 10;
        float distanceToTarget = Vector2.Distance(NPC.Center, targetPosition);
        if (distanceToTarget < speed)
            speed = distanceToTarget;
        speed *= MathHelper.Lerp(0.2f, 1f, EasingFunction.InOutSine(distanceToTarget / 384f));
        targetVelocity *= speed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

        _headSegment.angle = (-Vector2.UnitX).ToRotation();
        float range = MathHelper.ToRadians(3);
        for(int i = 1; i < _bodySegments.Length; i++)
        {
            _bodySegments[i].angle = _bodySegments[i - 1].angle - MathHelper.ToRadians(3) + MathHelper.Lerp(-range, range, ExtraMath.Osc(0f, 1f, offset: i));
        }
        if (distanceToTarget <= 8)
        {
            SwitchState(AIState.Pickup);
        }
    }

    private void MountPlayer()
    {
        _mountTimer++;
        Player target = Main.player[PlayerToTravel];
        SkullrunnerThrowModPlayer throwModPlayer = target.GetModPlayer<SkullrunnerThrowModPlayer>();
        Vector2 mountPosition = NPC.Center;
        mountPosition.X -= 64;
        mountPosition.Y -= 32;
        throwModPlayer.targetSuckPosition = Vector2.Lerp(target.Center, mountPosition, EasingFunction.InOutSine(_mountTimer / 30f));
    }

    private void AI_Pickup()
    {
        Timer++;
        if(Timer < 30)
        {
            NPC.velocity *= 0.9f;
        }

        if (Timer > 30)
        {
            MountPlayer();
        }

        if(Timer >= 90)
        {
            SwitchState(AIState.SwoopUp);
        }
    }

    private Point FindTargetTile()
    {
        int targetTileType = (int)TeleportTarget;
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        var tilePosition = zTileMap.Find((ushort)targetTileType);
        return new Point(tilePosition.x, tilePosition.y);
    }

    private void AI_SwoopUp()
    {
        Timer++;
        if(Timer < 55)
            MountPlayer();
        if(NPC.velocity.Y > -15)
            NPC.velocity.Y -= 1;

        float alpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 50f));
        FullTint.SetColor(Color.Black, alpha);
        if(Timer == 55)
        {

            Point targetTile = FindTargetTile();
            Vector2 worldCoordinates = targetTile.ToWorldCoordinates();

            if (MultiplayerHelper.IsHost)
            {
                _teleportPosition = worldCoordinates;
                NPC.netUpdate = true;
            }

            Player player = Main.player[(int)PlayerToTravel];          
            player.Teleport(worldCoordinates, TeleportationStyleID.DebugTeleport);
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, worldCoordinates.X, worldCoordinates.Y, 1);
        }

        if(Timer >= 60f)
        {
            SwitchState(AIState.SwoopOut);
        }
    }

    private void AI_SwoopOut()
    {
        Timer++;
        if(Timer < 15)
        {
            NPC.velocity *= 0.8f;
            MountPlayer();
        }
        else
        {
            if(NPC.velocity.Y > -15)
                NPC.velocity.Y -= 1;
            if(Timer > 90f)
            {
                NPC.active = false;
            }
        }
    }
    private void ResolveKinematics()
    {
        _rig.ResolveFK(NPC.Center);
    }

    private Vector2 Breathe(float offset)
    {
        Vector2 breathScale = Vector2.Lerp(Vector2.One * 1f, Vector2.One * 1.1f, ExtraMath.Osc(0f, 1f, speed: 2, offset: offset));
        return breathScale;
    }

    private float WingBreathe(float offset)
    {
        float range = MathHelper.ToRadians(5);
        float radians = MathHelper.Lerp(-range, range, ExtraMath.Osc(0f, 1f, speed: 1, offset: offset));
        return radians;
    }

    private float _headAngle;
    private void DrawSegments(SpriteBatch spriteBatch, Vector2 offset)
    {
        //BACK WING

        
        SpritebatchDrawer backWingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAssets[0], _bodySegments[0].a);
        backWingDrawer.drawOrigin = new Vector2(150, 72);
        backWingDrawer.rotation = _bodySegments[0].totalAngle - MathHelper.Pi;
        backWingDrawer.sourceRect = _wingTextureAssets[0].Value.GetFrame(_framer.frame, 10, 12);
        float wingAngle2 = MathHelper.WrapAngle(backWingDrawer.rotation + MathHelper.PiOver2);
        if (wingAngle2 < 0)
        {
            backWingDrawer.spriteEffects = SpriteEffects.FlipVertically;
            backWingDrawer.drawOrigin = new Vector2(backWingDrawer.drawOrigin.X, _wingTextureAssets[0].Height() - backWingDrawer.drawOrigin.Y);
        }
        backWingDrawer.scale = Breathe(0) * 2;
        backWingDrawer.rotation += WingBreathe(0);
        backWingDrawer.worldPosition += offset;
        backWingDrawer.color = backWingDrawer.color.MultiplyRGB(Color.Lerp(Color.White, Color.Black, 0.5f));
        spriteBatch.Draw(backWingDrawer);
        
        //BACK LEG BACK

        /*
        SpritebatchDrawer backLegDrawerBack = SpritebatchDrawer.FromTextureAsset(_backLegTextureAssets[1], _bodySegments[2].a);
        backLegDrawerBack.rotation = _bodySegments[2].totalAngle - MathHelper.Pi;
        backLegDrawerBack.drawOrigin = Vector2.Zero;

        float backLegAngle = MathHelper.WrapAngle(backLegDrawerBack.rotation + MathHelper.PiOver2);
        if (backLegAngle < 0)
        {
            backLegDrawerBack.spriteEffects = SpriteEffects.FlipVertically;
            backLegDrawerBack.drawOrigin = new Vector2(0, _backLegTextureAssets[1].Height() - 0);
        }
        backLegDrawerBack.scale = Breathe(1);
        backLegDrawerBack.worldPosition += offset;
        spriteBatch.Draw(backLegDrawerBack);
        */

        //BACK LEG FRONT
        SpritebatchDrawer frontLegDrawerBack = SpritebatchDrawer.FromTextureAsset(_frontLegTextureAssets[1], _bodySegments[0].a);
        frontLegDrawerBack.drawOrigin = Vector2.Zero;
        frontLegDrawerBack.rotation = _bodySegments[0].totalAngle - MathHelper.Pi;
        float frontLegAngle = MathHelper.WrapAngle(frontLegDrawerBack.rotation + MathHelper.PiOver2);
        if (frontLegAngle < 0)
        {
            frontLegDrawerBack.spriteEffects = SpriteEffects.FlipVertically;
            frontLegDrawerBack.drawOrigin = new Vector2(0, _frontLegTextureAssets[1].Height() - 0);
        }
        frontLegDrawerBack.scale = Breathe(1);
        frontLegDrawerBack.worldPosition += offset;
        spriteBatch.Draw(frontLegDrawerBack);

        //BODY
        for (int i = _bodyTextureAssets.Length - 1; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_bodyTextureAssets[i], _bodySegments[i].a);
            float bodyAngle = _bodySegments[i].totalAngle;
            drawer.rotation = bodyAngle;
            float dir = 1;
            if (i == _bodyTextureAssets.Length - 1)
            {
                drawer.drawOrigin = new Vector2(_bodyTextureAssets[i].Width(), 0);
            }

            bodyAngle = MathHelper.WrapAngle(bodyAngle + MathHelper.PiOver2);
            if (bodyAngle < 0)
            {
                dir = -1;
                drawer.spriteEffects = SpriteEffects.FlipVertically;
                drawer.drawOrigin = new Vector2(drawer.drawOrigin.X, _bodyTextureAssets[i].Height() - drawer.drawOrigin.Y);
            }
            if (i == _bodyTextureAssets.Length - 1)
            {
                drawer.rotation -= MathHelper.ToRadians(65 * dir);
                //         drawer.scale *= 4;
            }
            drawer.scale = Breathe(2 + i);
            drawer.worldPosition += offset;
            spriteBatch.Draw(drawer);
        }


        //HEAD
        SpritebatchDrawer headDrawer = SpritebatchDrawer.FromTextureAsset(_headTextureAsset, _headSegment.b);
        float drawAngle = MathHelper.Pi + _headSegment.angle;

        drawAngle = MathHelper.WrapAngle(drawAngle);


        Player player = Main.player[NPC.target];
        Vector2 lookVectory = player.Center - NPC.Center;
        lookVectory = lookVectory.SafeNormalize(Vector2.Zero);

        Vector2 forwardVectory = (_headSegment.a - _headSegment.b).SafeNormalize(Vector2.Zero);
        float dp = Vector2.Dot(forwardVectory, lookVectory);
        if (dp > 0.25f)
        {
            float lookAngle = lookVectory.ToRotation();
            _headAngle = Utils.AngleLerp(_headAngle, lookAngle, 0.1f);

        }
        else
        {
            _headAngle = Utils.AngleLerp(_headAngle, drawAngle, 0.1f);
        }

        headDrawer.rotation = _headAngle;
        headDrawer.LeftCenterOrigin();
        headDrawer.drawOrigin.X += 48;

        drawAngle = MathHelper.WrapAngle(drawAngle + MathHelper.PiOver2);
        if (drawAngle < 0)
        {
            headDrawer.spriteEffects = SpriteEffects.FlipVertically;
            headDrawer.drawOrigin = new Vector2(headDrawer.drawOrigin.X, _headTextureAsset.Height() - headDrawer.drawOrigin.Y);
        }

        headDrawer.worldPosition += offset;
        headDrawer.scale = Breathe(6);
        spriteBatch.Draw(headDrawer);

        
        //BACK LEG
        SpritebatchDrawer backLegDrawerFront = SpritebatchDrawer.FromTextureAsset(_frontLegTextureAssets[0], _bodySegments[2].a);
        backLegDrawerFront.drawOrigin = Vector2.Zero;
        backLegDrawerFront.rotation = _bodySegments[2].totalAngle - MathHelper.Pi + MathHelper.PiOver2;
        backLegDrawerFront.rotation -= MathHelper.ToRadians(25);
        //backLegDrawerFront.scale *= 4;
        float backDLegAngle = MathHelper.WrapAngle(_bodySegments[3].totalAngle - MathHelper.Pi + MathHelper.PiOver2);
        if (backDLegAngle < 0)
        {
            backLegDrawerFront.rotation += MathHelper.Pi;
            backLegDrawerFront.spriteEffects = SpriteEffects.FlipVertically;
            backLegDrawerFront.drawOrigin = new Vector2(0, _backLegTextureAssets[0].Height() - 0);
        }
        backLegDrawerFront.scale = Breathe(5);
        backLegDrawerFront.worldPosition += offset;
        spriteBatch.Draw(backLegDrawerFront);
        
        //FRONT LEG
        SpritebatchDrawer frontLegDrawerFront = SpritebatchDrawer.FromTextureAsset(_frontLegTextureAssets[0], _bodySegments[0].a);
        frontLegDrawerFront.drawOrigin = Vector2.Zero;
        frontLegDrawerFront.rotation = _bodySegments[0].totalAngle - MathHelper.Pi + MathHelper.PiOver2;
        float frontDLegAngle = MathHelper.WrapAngle(_bodySegments[0].totalAngle - MathHelper.Pi + MathHelper.PiOver2);
        if (frontDLegAngle < 0)
        {
            frontLegDrawerFront.rotation += MathHelper.Pi;
            frontLegDrawerFront.spriteEffects = SpriteEffects.FlipVertically;
            frontLegDrawerFront.drawOrigin = new Vector2(0, _frontLegTextureAssets[0].Height() - 0);
        }
        frontLegDrawerFront.scale = Breathe(5);
        frontLegDrawerFront.worldPosition += offset;
        spriteBatch.Draw(frontLegDrawerFront);


        //FRONT WING
        SpritebatchDrawer frontWingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAssets[0], _bodySegments[2].a);
        frontWingDrawer.drawOrigin = new Vector2(150, 72);
        frontWingDrawer.rotation = _bodySegments[2].totalAngle - MathHelper.Pi;
        frontWingDrawer.scale = Breathe(5) * 2;
        frontWingDrawer.sourceRect = _wingTextureAssets[0].Value.GetFrame(_framer.frame, 10, 12);
        float wingAngle = MathHelper.WrapAngle(frontWingDrawer.rotation + MathHelper.PiOver2);
        if (wingAngle < 0)
        {
            frontWingDrawer.spriteEffects = SpriteEffects.FlipVertically;
            frontWingDrawer.drawOrigin = new Vector2(114, _wingTextureAssets[0].Height() - 84);
        }
        frontWingDrawer.rotation += WingBreathe(0);
        frontWingDrawer.worldPosition += offset;
        spriteBatch.Draw(frontWingDrawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        LoadTextureAssets();
        ResolveKinematics();
        DrawSegments(spriteBatch, Vector2.Zero);
        return false;
    }

}
public class OrganWave : ModProjectile
{
    private float Time => 120;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.timeLeft = (int)Time * 2;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            ModContent.GetInstance<OrganWaypointTracker>().darknessAnimation = 175;
        }
        CameraTargetSystem.AddTarget(Projectile.Center);

        //  ModContent.GetInstance<CameraTargetSystem>().TargetPositions.Add(Projectile.Center);
        if (Timer == 60)
        {
            PixelPrimitiveCircleFactory.CreateOrganBoom(Projectile.Center);
            if (Main.netMode != NetmodeID.Server)
                ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.White, 0.2f, 60);
        }

        if (Timer > 60 && Timer % 4 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(256, 256), ModContent.DustType<MusicDust>(), -Vector2.UnitY, 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            if (Main.rand.NextBool(2))
            {
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(256, 256), -Vector2.UnitY, Color.White, Scale: 0.5f);
                sp.noTileCollide = true;
                sp.gravity = 0;
                sp.outerColor = Color.White;
            }

        }
        for (int i = 0; i < Main.musicFade.Length; i++)
        {
            Main.musicFade[i] = 0;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        float outRatio = Timer / Time;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        float scale = MathHelper.Lerp(1.8f, 0f, EasingFunction.OutExpo(outRatio));
        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * scale;
        waveDrawer.color = Color.Orange;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        //   Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkOrange * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * scale;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MuzzleFlash"), Projectile.Center);
        drawer.scale = new Vector2(3, 10);
        float timer = Timer - 60f;

        drawer.color = Color.Orange * MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(timer / 180f));
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        float alpha = EasingFunction.QuadraticBump(timer / 180f);
        string text = $"Waypoint Unlocked!";
        Vector2 pos = Projectile.Center - Main.screenPosition;
        pos.Y -= 128;

        Vector2 size = FontAssets.DeathText.Value.MeasureString(text);
        float textScale = 1.5f;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, text,
            pos, Color.White * alpha, 0f, size * 0.5f, new Vector2(textScale), -1, textScale);
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class OrganWaypointTracker : ModSystem
{
    public bool[] locations;
    public float darknessAnimation;
    public override void Load()
    {
        base.Load();
        locations = new bool[20];
    }
    public override void Unload()
    {
        base.Unload();
        locations = null;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (Keyboard.GetState().IsKeyDown(Keys.O))
        {
            ResetWaypoints();
        }
        if (darknessAnimation > 0)
            darknessAnimation--;
    }
    public ref bool GetWaypoint(OrganWaypoint waypoint)
    {
        int index = (int)waypoint;
        return ref locations[index];
    }

    public void ActivateWaypoint(OrganWaypoint waypoint, Vector2 worldPosition)
    {
        int index = (int)waypoint;
        locations[index] = true;
        Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), worldPosition, Vector2.Zero,
            ModContent.ProjectileType<OrganWave>(), 1, 1, Main.LocalPlayer.whoAmI);
        ActivateWaypointEffect(worldPosition);
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            //Need to sync the activation across clients
            int clientToIgnore = Main.LocalPlayer.whoAmI;
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.WaypointActivate,
                (byte)waypoint,
                worldPosition.X,
                worldPosition.Y).Send(ignoreClient: clientToIgnore);
        }
    }

    public void HandleWaypointActivatePacket(BinaryReader reader)
    {
        OrganWaypoint waypoint = (OrganWaypoint)reader.ReadByte();
        Vector2 worldPosition = reader.ReadVector2();
        locations[(int)waypoint] = true;
        ActivateWaypointEffect(worldPosition);
    }

    private void ActivateWaypointEffect(Vector2 worldPosition)
    {
        SoundStyle activateSound = AssetRegistry.Sounds.Waypoint.WaypointActivate;
        SoundEngine.PlaySound(activateSound);

        //Bit of screenshake never hurt anyone
        ShakeScreenPosition.Shake = 4;
        FXUtil.ShakeCamera(worldPosition, 1024, 4);

    }

    public void ResetWaypoints()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i] = false;
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        int length = locations.Length;
        writer.Write(length);
        for (int i = 0; i < length; i++)
        {
            writer.Write(locations[i]);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        int length = reader.ReadInt32();
        for (int i = 0; i < length; i++)
        {
            locations[i] = reader.ReadBoolean();
        }
    }
    public override void SaveWorldData(TagCompound tag)
    {
        base.SaveWorldData(tag);
        tag["locations"] = locations;
    }
    public override void LoadWorldData(TagCompound tag)
    {
        base.LoadWorldData(tag);
        bool[] savedLocations = tag.Get<bool[]>("locations");
        if (savedLocations != null)
        {
            locations = savedLocations;
        }
    }
}

public class WaypointButtonsUI : UIPanel
{
    public class WaypointButton
    {
        private Asset<Texture2D> LoadPhotoAsset(string fileName)
        {
            bool succeed = ModContent.RequestIfExists<Texture2D>(WaypointSystem.AssetPath(fileName), out Asset<Texture2D> photoAsset);
            if (!succeed)
            {
                photoAsset = ModContent.Request<Texture2D>(WaypointSystem.AssetPath("Placeholder"));
            }
            return photoAsset;
        }
        public WaypointButton(string textureName, OrganWaypoint WaypointType)
        {
            this.TextureAsset = LoadTextureAsset(textureName);
            this.PhotoTextureAsset = LoadPhotoAsset(textureName + "_Photo");
            this.WaypointType = WaypointType;
        }

        public readonly Asset<Texture2D> TextureAsset;
        public readonly Asset<Texture2D> PhotoTextureAsset;
        public readonly OrganWaypoint WaypointType;
        public Vector2 scale;
        //        Asset<Texture2D> TextureAsset = TextureAs
    }

    private Asset<Texture2D> _photoPanelTextureAsset;
    private WaypointButton[] _waypointButtons;
    private WaypointButton _previewButton;
    private bool _hovering;

    private float _easeInTimer;
    public float alpha;
    public static Asset<Texture2D> LoadTextureAsset(string fileName)
    {
        return ModContent.Request<Texture2D>(WaypointSystem.AssetPath(fileName));
    }

    public WaypointButtonsUI()
    {
        _photoPanelTextureAsset = LoadTextureAsset("PhotoFrame");
        _waypointButtons = new WaypointButton[14];
        _waypointButtons[0] = new WaypointButton("ApocalypseTower", OrganWaypoint.ApocalypseTower);
        _waypointButtons[1] = new WaypointButton("BloodySanctum", OrganWaypoint.BloodySanctum);
        _waypointButtons[2] = new WaypointButton("Dragonhome", OrganWaypoint.Dragonhome);
        _waypointButtons[3] = new WaypointButton("GintzeDesert", OrganWaypoint.Desert);
        _waypointButtons[4] = new WaypointButton("Hallowrooms", OrganWaypoint.Hallowrooms);
        _waypointButtons[5] = new WaypointButton("Ishtar", OrganWaypoint.Ishtar);
        _waypointButtons[6] = new WaypointButton("OvergrownMarsh", OrganWaypoint.Marsh);
        _waypointButtons[7] = new WaypointButton("Platform", OrganWaypoint.Platform);
        _waypointButtons[8] = new WaypointButton("RunicaWaterside", OrganWaypoint.RunicaWaterside);
        _waypointButtons[9] = new WaypointButton("Witchtown", OrganWaypoint.WitchTown);
        _waypointButtons[10] = new WaypointButton("WonderousDarkspace", OrganWaypoint.WonderousDarkspace);
        _waypointButtons[11] = new WaypointButton("WorldsEnd", OrganWaypoint.WorldsEnd);
        _waypointButtons[12] = new WaypointButton("MoonlightCathedral", OrganWaypoint.Moonspiral);
        _waypointButtons[13] = new WaypointButton("MistyDungeon", OrganWaypoint.MistyDungeon);
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Width.Pixels = 394 * 2;
        Height.Pixels = 272 * 2;
    }

   private ushort GetTileType(OrganWaypoint waypoint)
    {
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        switch (waypoint)
        {
            default:
            case OrganWaypoint.WitchTown:
                return ModContent.GetInstance<WitchTownOrgan>().type;
            case OrganWaypoint.Marsh:
                return ModContent.GetInstance<MarshOrgan>().type;
            case OrganWaypoint.Moonspiral:
                return ModContent.GetInstance<MoonSpiralTowerOrgan>().type;
            case OrganWaypoint.Desert:
                return ModContent.GetInstance<DesertOrgan>().type;
        }
    }

    private void SummonDragon(OrganWaypoint waypoint)
    {
        int x = (int)Main.LocalPlayer.Center.X;
        int y = (int)Main.LocalPlayer.Center.Y;
        int npcType = ModContent.NPCType<OrganDragon>();
        y -= 1500;

        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            NPC.NewNPC(Main.LocalPlayer.GetSource_FromThis(), x, y, ModContent.NPCType<OrganDragon>(), ai1: Main.LocalPlayer.whoAmI, ai3: (float)GetTileType(waypoint));
            return;
        }

        //Need to spawn it from the server
        int clientToIgnore = Main.LocalPlayer.whoAmI;
        Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.SpawnNPC,
            Main.LocalPlayer.whoAmI,
            x,
            y,
            npcType,
            (float)0,
            (float)Main.LocalPlayer.whoAmI,
            (float)0,
            (float)GetTileType(waypoint)).Send(ignoreClient: clientToIgnore);
    }
    private void PreviewPopup(SpriteBatch spriteBatch, WaypointButton waypointButton, Vector2 position)
    {

        position.Y += ExtraMath.Osc(0f, 4f, speed: 2);
        Vector2 origin = _photoPanelTextureAsset.Value.Size() * 0.5f;
        Vector2 scale = Vector2.One;

        Vector2 previewOrigin = waypointButton.PhotoTextureAsset.Value.Size() * 0.5f;
        Vector2 previewPosition = position;
        previewPosition.Y -= 13;

        Color drawColor = Color.White * alpha;
        spriteBatch.Draw(waypointButton.PhotoTextureAsset.Value, previewPosition, null, drawColor, 0, previewOrigin, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(_photoPanelTextureAsset.Value, position, null, drawColor, 0, origin, scale, SpriteEffects.None, 0);


        string text = LangText.Common(waypointButton.WaypointType.ToString());
        Vector2 size = FontAssets.DeathText.Value.MeasureString(text);
        float textScale = 0.5f;
        Vector2 textPosition = position;
        textPosition.Y -= 100;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, text,
            textPosition, drawColor, 0f, size * 0.5f, new Vector2(textScale), -1, textScale);
    }

    private void DrawButton_Inner(SpriteBatch spriteBatch, WaypointButton waypointButton, Vector2 position)
    {
        Vector2 origin = new Vector2();
        origin.X = waypointButton.TextureAsset.Value.Width * 0.5f;
        origin.Y = waypointButton.TextureAsset.Value.Height * 0.5f;

        Vector2 targetScale = Vector2.One * 2;
        Rectangle intersectRectangle = new Rectangle(
            (int)(position.X - origin.X * 2),
            (int)(position.Y - origin.Y * 2),
            waypointButton.TextureAsset.Value.Width * 2,
            waypointButton.TextureAsset.Value.Height * 2);

        Vector2 mouseScreen = Main.MouseScreen;
        bool isMouseHovering = !_hovering && intersectRectangle.Contains(mouseScreen.ToPoint());
        if (isMouseHovering)
        {
            _previewButton = waypointButton;
            _hovering = true;
            targetScale *= 1.1f;
        }

        waypointButton.scale = Vector2.Lerp(waypointButton.scale, targetScale, 0.2f);
        Vector2 scale = waypointButton.scale;
        spriteBatch.Draw(waypointButton.TextureAsset.Value, position, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
        //   Primitives2D.DrawRectangle(spriteBatch, intersectRectangle, Color.Red);

        if (isMouseHovering && !PlayerInput.IgnoreMouseInterface)
        {
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                SummonDragon(waypointButton.WaypointType);

                ModContent.GetInstance<WaypointSystem>().CloseUI();
                //    Main.NewText("Click");
                Main.mouseLeftRelease = false;
            }
            Main.LocalPlayer.mouseInterface = true;
        }


        if (isMouseHovering)
        {
            SpriteWhiteShader spriteWhiteShader = SpriteWhiteShader.Instance;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                effect: spriteWhiteShader.Effect, Main.UIScaleMatrix);

            Color highlightedColor = Color.Yellow;
            highlightedColor *= ExtraMath.Osc(0.5f, 1f, speed: 6);
            spriteBatch.Draw(waypointButton.TextureAsset.Value, position, null, highlightedColor, 0, origin, scale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                effect: null, Main.UIScaleMatrix);

        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (_hovering)
        {
            _easeInTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //_easeInTimer = MathHelper.Lerp(_easeInTimer, 1f, 0.2f);
        }
        else
        {
            _easeInTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        _easeInTimer = MathHelper.Clamp(_easeInTimer, 0f, 1.5f);
        alpha = EasingFunction.InOutSine(_easeInTimer);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        //So we need to draw ALL of the things in their correct spots 
        //Draw the platform
        Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
        _hovering = false;

        Rectangle rect = GetDimensions().ToRectangle();
        Vector2 platformWaypointPos = topLeft;
        platformWaypointPos.X += Width.Pixels * 0.5f;
        platformWaypointPos.Y += 64;
        //The platform
        DrawButton_Inner(spriteBatch, _waypointButtons[7], platformWaypointPos);

        //Runica Waterside
        Vector2 mistyDungeonPos = platformWaypointPos;
        mistyDungeonPos.X += 111;
        mistyDungeonPos.Y += 111;
        DrawButton_Inner(spriteBatch, _waypointButtons[13], mistyDungeonPos);

        Vector2 witchTownPos = platformWaypointPos;
        witchTownPos.X += 16;
        witchTownPos.Y += 222;
        DrawButton_Inner(spriteBatch, _waypointButtons[9], witchTownPos);

        Vector2 gintzeDesertPos = witchTownPos;
        gintzeDesertPos.X -= 152;
        gintzeDesertPos.Y -= 28;
        DrawButton_Inner(spriteBatch, _waypointButtons[3], gintzeDesertPos);

        Vector2 hallowRoomsPos = gintzeDesertPos;
        hallowRoomsPos.Y += 107;
        hallowRoomsPos.X += 6;
        DrawButton_Inner(spriteBatch, _waypointButtons[4], hallowRoomsPos);

        Vector2 marshPos = gintzeDesertPos;
        marshPos.X -= 100;
        marshPos.Y -= 68;
        DrawButton_Inner(spriteBatch, _waypointButtons[6], marshPos);

        Vector2 worldsEndPos = marshPos;
        worldsEndPos.X -= 50;
        worldsEndPos.Y -= 0;
        DrawButton_Inner(spriteBatch, _waypointButtons[11], worldsEndPos);

        Vector2 ishtarPos = worldsEndPos;
        ishtarPos.Y += 174;
        ishtarPos.X += 24;
        DrawButton_Inner(spriteBatch, _waypointButtons[5], ishtarPos);

        Vector2 wonderousDarkSpacePos = witchTownPos;
        wonderousDarkSpacePos.Y += 100;
        DrawButton_Inner(spriteBatch, _waypointButtons[10], wonderousDarkSpacePos);

        Vector2 bloodySanctumPos = wonderousDarkSpacePos;
        bloodySanctumPos.X += 144;
        bloodySanctumPos.Y += 40;
        DrawButton_Inner(spriteBatch, _waypointButtons[1], bloodySanctumPos);

        Vector2 dragonHomePos = bloodySanctumPos;
        dragonHomePos.X += 128;
        dragonHomePos.Y -= 2;
        DrawButton_Inner(spriteBatch, _waypointButtons[2], dragonHomePos);

        Vector2 moonlightPos = bloodySanctumPos;
        moonlightPos.Y -= 128;
        DrawButton_Inner(spriteBatch, _waypointButtons[12], moonlightPos);

        Vector2 runicaPos = dragonHomePos;
        runicaPos.Y -= 196;
        DrawButton_Inner(spriteBatch, _waypointButtons[8], runicaPos);

        Vector2 apocalypseTowerPos = topLeft;
        apocalypseTowerPos.Y += Height.Pixels - 128;
        apocalypseTowerPos.X += 184;
        DrawButton_Inner(spriteBatch, _waypointButtons[0], apocalypseTowerPos);

        _previewButton ??= _waypointButtons[0];
        Vector2 previewPos = GetDimensions().ToRectangle().TopLeft();
        previewPos.X += Width.Pixels + 196;
        previewPos.Y += Height.Pixels * 0.5f;
        previewPos.Y += MathHelper.Lerp(64, 0, alpha);
        PreviewPopup(spriteBatch, _previewButton, previewPos);

    }
}

public class WaypointUI : UIPanel
{

    private UIImage _background;
    private WaypointButtonsUI _buttonsUI;
    private UIText _titleText;
    public WaypointUI()
    {
        _titleText = new UIText("Lunar Veil");
        _buttonsUI = new WaypointButtonsUI();
        _background = new UIImage(ModContent.Request<Texture2D>(WaypointSystem.AssetPath("WaypointBackground")));
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Width.Pixels = 394 * 2;
        Height.Pixels = 272 * 2;
        Append(_background);
        Append(_buttonsUI);
        _titleText.SetText(LangText.Common("WaypointTitle"), 1, true);
        _titleText.HAlign = 0.5f;
        Append(_titleText);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Vector2 pxOffset = UIHelpers.ScreenOffset(
            new Vector2(Width.Pixels, Height.Pixels),
            normalizedOrigin: new Vector2(0.5f),
            offset: new Vector2(0, -64));
        Left.Pixels = pxOffset.X;
        Top.Pixels = pxOffset.Y;

    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();
    }
}

public class WaypointUIState : UIState
{
    public WaypointUI ui;
    public BackButton backButton;
    public WaypointUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        ui = new WaypointUI();
        Append(ui);

        backButton = new BackButton(ModContent.GetInstance<WaypointSystem>().CloseUI);
        Append(backButton);
    }
}

[Autoload(Side = ModSide.Client)]
public class WaypointSystem : BaseUISystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    public WaypointUIState uiState;
    public override int uiSlot => Slot_MajorUI;

    /// <summary>
    /// Gets an asset path local to the waypoint system's assets
    /// </summary>
    /// <param name="localPath"></param>
    /// <returns></returns>
    public static string AssetPath(string localPath)
    {
        string rootPath = $"Stellamod/Common/WaypointSystem/UI/";
        string combinedPath = rootPath + localPath;
        return combinedPath;
    }

    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        uiState = new();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if(Main.LocalPlayer.GetModPlayer<SanctorousPlayer>().hasSetBonus && LunarVeilKeybinds.AbilityKeybind.JustPressed)
        {
            ToggleUI();
        }
        _lastUpdateUiGameTime = gameTime;
        if (_userInterface.CurrentState != null)
        {
            _userInterface.Update(gameTime);
        }
    }

    public override void CloseThis()
    {
        base.CloseThis();
        CloseUI();
    }

    public void ToggleUI()
    {
        if (_userInterface.CurrentState != null)
        {
            SoundStyle soundStyle = SoundID.MenuClose;
            SoundEngine.PlaySound(soundStyle);
            CloseUI();
        }
        else
        {
            SoundStyle soundStyle = AssetRegistry.Sounds.Waypoint.OpenWaypointSection;
            SoundEngine.PlaySound(soundStyle);
            OpenUI();
        }
    }

    public void OpenUI()
    {
        //Set State
        TakeSlot();
        _userInterface.SetState(uiState);
    }

    public void CloseUI()
    {
        ClearSlot();
        _userInterface.SetState(null);
    }

    public override void PreSaveAndQuit()
    {
        //Calls Deactivate and drops the item
        if (_userInterface.CurrentState != null)
        {
            CloseUI();
            _userInterface.SetState(null);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Stellamod: Waypoint UI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                    {
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
                            Main.UIScaleMatrix);

                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null,
                            Main.UIScaleMatrix);

                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
