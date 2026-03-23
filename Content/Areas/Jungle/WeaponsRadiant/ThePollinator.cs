using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Summons.Orbs;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.WeaponsRadiant;

public class ThePollinatorDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
        // Other mods may check it for different purposes.
        BuffID.Sets.IsATagBuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        if (Main.rand.NextBool(3))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.Goldenrod * 0.4f;
            sp.expand = true;
        }

        if (Main.rand.NextBool(6))
        {
            int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Gold);
            Main.dust[d].noGravity = true;
        }

        //womp womp ig
        if (npc.boss)
        {
            npc.velocity *= 0.99f;
            return;
        }

        //SLOW EM DOWN MWAHAHAH
        npc.velocity *= 0.75f;
    }
}

public class ThePollinator : BaseChainedBallItem
{

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 444;
        Item.shoot = ModContent.ProjectileType<ThePollinatorProj>();
    }

    public override void AddRecipes()
    {
        this.RegisterBrew<RadiantNectar, BlankOrb>();
    }
}

public class ThePollinatorProj : BaseChainedBallProjectile
{
    public enum ActionState : byte
    {
        Orbit,
        Swing_1,
        Swing_2,
        Swing_3
    }

    public const float Swing_Time = 40 * Swing_Speed_Multiplier;
    public const float Swing_Time_2 = 60 * Swing_Speed_Multiplier;
    public const float Final_Swing_Distance = 252;
    public const float Combo_Time = 8;
    public const int Swing_Speed_Multiplier = 8;

    public override float MaxThrowDistance => 512;
    private float ComboCounter;
    private ActionState State;

    float SwingTime;
    float EasedProgress;
    Vector2 SwingStart;
    Vector2 SwingTarget;
    Vector2 SwingVelocity;
    int DustTimer;

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(SwingStart);
        writer.WriteVector2(SwingTarget);
        writer.WriteVector2(SwingVelocity);
        writer.Write(SwingTime);
        writer.Write(ComboCounter);
        writer.Write((byte)State);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        SwingStart = reader.ReadVector2();
        SwingTarget = reader.ReadVector2();
        SwingVelocity = reader.ReadVector2();
        SwingTime = reader.ReadSingle();
        ComboCounter = reader.ReadSingle();
        State = (ActionState)reader.ReadByte();
    }


    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 48;
        Projectile.height = 52;
        //Just having this here in case
        //Iron Ball is just gonna use default stuff htough
        zeroVelocity = true;

        //Variables
        //Easing
        easer = (float lerpValue) => Easing.InOutExpo(lerpValue, 7);

        //How far it drags behind you
        dragDistance = 126;

        //Swing Range (IT USES OVAL SWING)
        swingRange = MathHelper.ToRadians(360);

        //Offst for theoval swing
        ovalRotOffset = MathHelper.ToRadians(-90);

        //Max X Swing Radius
        swingXRadius = 512;

        //Y Swing  Radius
        swingYRadius = 80;

        //How long it takes to swing
        baseSwingTime = 48;

        //Glowing stuff
        glowDistanceOffset = 4;
        glowRotationSpeed = 0.005f;

        //Damage multiplier for hitting the tip
        TipDamageMultiplier = 2;
    }

    public override bool? CanDamage()
    {
        //Only deal damage while swinging
        return State != ActionState.Orbit;
    }

    public override void StartSling()
    {
        base.StartSling();
    }
    private bool ContinueCombo() => this.OwnedByLocalClient() && Main.mouseLeft;

    public override void AI_Sling()
    {
        switch (State)
        {
            case ActionState.Orbit:
                ThrowOutEffect1();
                State = ActionState.Swing_1;
                break;
            case ActionState.Swing_1:
                Swing1();
                break;
            case ActionState.Swing_2:
                Swing2();
                break;
            case ActionState.Swing_3:
                Swing3();
                break;
        }
    }

    private void Reset()
    {
        EasedProgress = 0;
        for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
        {
            Projectile.localNPCImmunity[i] = 0;
        }
    }

    private void ThrowOutEffect1()
    {
        SoundStyle soundStyle = SoundID.Item7;
        soundStyle.PitchVariance = 0.15f;

        SoundEngine.PlaySound(soundStyle, Projectile.position);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Morrowarrow") { PitchVariance = 0.15f }, Projectile.position);
        OrbHelper.PlaySummonSound(Projectile.position);
        Reset();
    }

    private void ThrowOutEffect2()
    {
        //Throw Sounds
        SoundStyle soundStyle = SoundID.Item7;
        soundStyle.PitchVariance = 0.15f;
        soundStyle.Pitch = -0.25f;

        SoundEngine.PlaySound(soundStyle, Projectile.position);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Morrowarrow") { PitchVariance = 0.15f, Pitch = -0.25f }, Projectile.position);
        OrbHelper.PlaySummonSound(Projectile.position);
        Reset();
    }

    private void SwingDusts()
    {
        DustTimer++;
        if (DustTimer >= 4 * Swing_Speed_Multiplier)
        {
            DustTimer = 0;
            DustParticle dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero);
            dp.Scale *= 0.5f;
            dp.innerColor = Color.Goldenrod;
            dp.outerColor = Color.Goldenrod;
            dp.Velocity = -Vector2.UnitY;
            dp.noTileCollide = true;
            dp.gravity = 0;
        }
    }

    private void Swing1()
    {
        SwingDusts();
        Timer++;
        if(Timer == 1)
        {
            SwingTarget= GetSwingTarget();
        }
        float progress = Timer / SwingTime;
        EasedProgress = EasingFunction.QuadraticBump(progress);


        
        Vector2 start = Owner.Center;
        Vector2 dir = (SwingTarget - start).SafeNormalize(Vector2.Zero);
        start -= dir * 100;
        Vector2 end = SwingTarget + dir * Final_Swing_Distance;
        Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);

        Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
        if (Timer > SwingTime)
        {
            if (ContinueCombo())
            {
                ThrowOutEffect1();
                SwingVelocity = Owner.DirectionTo(SwingTarget);
                float distance = 180;
                SwingStart = Owner.Center + SwingVelocity.RotatedByRandom(MathHelper.TwoPi) * distance;
                if (Main.myPlayer == Projectile.owner)
                {
                    SwingTarget = GetSwingTarget();
                    Projectile.netUpdate = true;
                }

                SwingTime = Swing_Time;
                State = ActionState.Swing_2;
                ComboCounter = 0;
                Timer = 0;
            }
            else if (Timer > SwingTime + Combo_Time)
            {
                Drop();
            }
        }
    }
    private void Drop()
    {
        ComboCounter = 0;
        Timer = 0;
        State = ActionState.Orbit;
        LetGo();
    }

    private void Swing2()
    {
        SwingDusts();
        Timer++;

        float progress = Timer / SwingTime;
        EasedProgress = EasingFunction.QuadraticBump(progress);

        Vector2 start = Owner.Center;
        Vector2 dir = (SwingTarget - start).SafeNormalize(Vector2.Zero);
        start -= dir * 100;
        Vector2 end = SwingTarget + dir * Final_Swing_Distance;
        Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);

        Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
        if (Timer > SwingTime)
        {
            if (ContinueCombo())
            {
                ThrowOutEffect2();
                SwingVelocity = Owner.DirectionTo(SwingTarget);
                SwingStart = Projectile.Center;
                if (Main.myPlayer == Projectile.owner)
                {
                    SwingTarget = GetSwingTarget();
                    Projectile.netUpdate = true;
                }

                SwingTime = Swing_Time_2;
                State = ActionState.Swing_3;
                ComboCounter = 0;
                Timer = 0;
            }
            else if (Timer > SwingTime + Combo_Time)
            {
                Drop();
            }
        }
    }

    private void Swing3()
    {
        SwingDusts();
        Timer++;
        float progress = Timer / SwingTime;
        EasedProgress = EasingFunction.QuadraticBump(progress);

 
        Vector2 start = Owner.Center;
        Vector2 dir = (SwingTarget - start).SafeNormalize(Vector2.Zero);
        start -= dir * 100;
        Vector2 end = SwingTarget + dir * Final_Swing_Distance;
        Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);

        Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
        if (Timer > SwingTime)
        {
            Drop();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(ModContent.BuffType<ThePollinatorDebuff>(), 240);
        SoundStyle soundStyle;

        int gore1 = GoreHelper.TypeFallingLeafWhite;
        int gore2 = GoreHelper.TypeFallingLeafRed;
        for (int i = 0; i < 2; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
            Gore.NewGore(Projectile.GetSource_FromThis(), target.position, velocity, gore1);

            velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
            Gore.NewGore(Projectile.GetSource_FromThis(), target.position, velocity, gore2);
        }

        switch (State)
        {
            case ActionState.Swing_1:
            case ActionState.Swing_2:
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDust(target.position, Projectile.width, Projectile.height, ModContent.DustType<GunFlash>(), Scale: 0.8f);
                    Dust.NewDustPerfect(target.position, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
                }

                soundStyle = SoundID.Grass;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MorrowExp") { PitchVariance = 0.15f }, Projectile.position);
                break;

            case ActionState.Swing_3:
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
                }

                for (int i = 0; i < 14; i++)
                {

                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
                }

                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                target.SimpleStrikeNPC(Projectile.damage, hit.HitDirection);
                soundStyle = SoundID.Grass;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/OverGrowth_Thorn1") { PitchVariance = 0.15f }, Projectile.position);
                break;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
            Color.LightGoldenrodYellow.ToVector3(),
            Color.DarkGoldenrod.ToVector3(),
            new Vector3(3, 3, 3), 0);
        DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
        DrawHelper.DrawAdditiveAfterImage(Projectile, Color.DarkGoldenrod, Color.Transparent, ref lightColor);
        return base.PreDraw(ref lightColor);
    }
}