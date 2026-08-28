using Stellamod.Content.Areas.Terror.WeaponsTR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.AccPT;

public class MechanicalArmsPlayer : ModPlayer
{
    public bool hasMechanicalArms;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasMechanicalArms = false;
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasMechanicalArms)
            return;
        if (Main.myPlayer != Player.whoAmI)
            return;
        int type = ModContent.ProjectileType<MechanicalArmProj>();
        if (Player.ownedProjectileCounts[type] > 0)
            return;


        for(int i = 0; i < 4; i++)
        {
            ProjFirer firer = ProjFirer.From<MechanicalArmProj>(Player);
            firer.ai1 = i;

            firer.damage = 50;
            firer.New();
        }
    }
}

public class MechanicalArms : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
        Item.damage = 50;
        Item.DamageType = DamageClass.Generic;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<MechanicalArmsPlayer>().hasMechanicalArms = true;
    }
}

public class MechanicalArmProj : ModProjectile
{
    private float _arm1Rot;
    private float _arm2Rot;
    private Vector3 _arm1;
    private Vector3 _arm2;
    private Vector3 _hand;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    private ref float ShootTimer => ref Projectile.ai[2];
    private Player Owner => Main.player[Projectile.owner];

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 60;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.light = 0.6f;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        if (Owner.GetModPlayer<MechanicalArmsPlayer>().hasMechanicalArms)
            Projectile.timeLeft = 2;

        Vector2 rootPosition = Owner.Center;
        float arm1Rotation = GetArm1Rotation();
        float arm2Rotation = GetArm2Rotation();
        _arm1Rot = Utils.AngleLerp(_arm1Rot, arm1Rotation, 0.1f);
        _arm2Rot = Utils.AngleLerp(_arm2Rot, arm2Rotation, 0.1f);

        Vector2 armEndEffector = GetEndEffector(rootPosition, _arm1Rot, length: 72);
        _arm1 = new Vector3(rootPosition, _arm1Rot);

   
        Vector2 arm2EndEffector = GetEndEffector(armEndEffector, _arm2Rot, length: 72);
        _arm2 = new Vector3(armEndEffector, _arm2Rot);

        _hand = new Vector3(arm2EndEffector, Projectile.rotation);
        if (Main.myPlayer == Projectile.owner)
        {
            Vector2 vel = (Main.MouseWorld - arm2EndEffector);
            Projectile.velocity = vel;
            Projectile.netUpdate = true;
        }

        if (Owner.controlUseItem)
        {
            Timer++;
            if(Timer >= 30)
            {
                SoundStyle s;
                if (Main.rand.NextBool(2))
                {
                    s = new SoundStyle("Stellamod/Assets/Sounds/XX4160");
                }
                else
                {
                    s = new SoundStyle("Stellamod/Assets/Sounds/XX41602");
                }
                s = s with { PitchVariance = 0.6f, Volume = 0.15f };
                SoundEngine.PlaySound(s, Projectile.position);
                Timer = Main.rand.Next(-15, 0);
                if(Main.myPlayer == Projectile.owner)
                {
                    StatModifier mod = Owner.GetDamage(DamageClass.Generic);
                    float d = 50;
                    float newDamage = mod.ApplyTo(d);

                    ProjFirer firer = ProjFirer.From<TerrorMinigunShot>(Projectile);
                    firer.position = arm2EndEffector;
                    firer.damage = (int)newDamage;
                    firer.ai2 = 1;
                    firer.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 14;
                    firer.New();
                }
                ShootTimer = 15;
            }
        }

        if(ShootTimer > 0)
        {
            ShootTimer--;
        }

        Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
        Projectile.Center = Owner.Center;
        Projectile.rotation = Projectile.velocity.ToRotation();

    }

    private float GetArm1Rotation()
    {
        float a = -135;
        float b = -160;
        float c = -190;
        float d = -230;
        float angle;
        switch (Style)
        {
            default:
            case 0:
                angle = MathHelper.ToRadians(a);
                break;
            case 1:
                angle = MathHelper.ToRadians(b);
                break;
            case 2:
                angle = MathHelper.ToRadians(c);
                break;
            case 3:
                angle = MathHelper.ToRadians(d);
                break;
        }
        if(Projectile.direction == -1)
        {
            return MathHelper.Pi - angle;
        }
        return angle;
    }
    private float GetArm2Rotation()
    {
        float a = -75;
        float b = -105;
        float c = -120;
        float d = -140;
        float angle;
        switch (Style)
        {
            default:
            case 0:
                angle = MathHelper.ToRadians(a);
                break;
            case 1:
                angle = MathHelper.ToRadians(b);
                break;
            case 2:
                angle = MathHelper.ToRadians(c);
                break;
            case 3:
                angle = MathHelper.ToRadians(d);
                break;
        }
 
        if(Projectile.direction == -1)
        {
            return MathHelper.Pi - angle;
        }
        return angle;
    }


    private Vector2 GetEndEffector(in Vector2 rootPosition, in float rotation, in float length)
    {
        return rootPosition + rotation.ToRotationVector2() * length;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Style > 3)
            return false;
        //Draw two arms and attach thgun at the end of the 2nd arm
        SpritebatchDrawer armDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        armDrawer.VerticalFrame(0, 2);
        armDrawer.drawOrigin.X = 16;
        armDrawer.worldPosition = new Vector2(_arm1.X, _arm1.Y);
        armDrawer.rotation = _arm1.Z;
        Main.spriteBatch.Draw(armDrawer);


        armDrawer.worldPosition = new Vector2(_arm2.X, _arm2.Y);
        armDrawer.rotation = _arm2.Z;
        Main.spriteBatch.Draw(armDrawer);


        armDrawer.VerticalFrame(1, 2);
        armDrawer.worldPosition = new Vector2(_hand.X, _hand.Y);
        armDrawer.rotation = _hand.Z;
        armDrawer.worldPosition -= _hand.Z.ToRotationVector2() * ShootTimer;
        Main.spriteBatch.Draw(armDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
