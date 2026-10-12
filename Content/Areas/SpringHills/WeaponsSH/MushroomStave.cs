using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;

using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class MushroomStave : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.MushroomStave.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<MushroomStaveMinionProj>());
        Item.damage = 6;
        Item.knockBack = 3;
    }



    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(),
            material: ModContent.ItemType<Mushroom>());
    }
}

public class MushroomStaveMinionProj : AbstractBellSummon,
    IDrawToRenderTarget
{
    Targeter _targeter;
   enum AIState : byte
    {
        Idle,
        SporeRelease
    }
    ref float Timer => ref Projectile.ai[0];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    ref float GlobalTimer => ref Projectile.ai[2];
    public override string Texture => AssetReferences.Assets.Textures.Projectile.MushroomStaveMinionProj.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        _targeter.NetSend(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _targeter.NetReceive(reader);
    }
    public override void SetStaticDefaults()
    {
        Projectile.StaticDefaultToMinionProjectile();
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults()
    {
        Projectile.DefaultToMinionProjectile();
        Projectile.tileCollide = false;
        Projectile.light = 0.28f;
    }


    public override void AI()
    {
        base.AI();
        //Hover above your head, while an enemy is nearby, charge up and release spores
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.SporeRelease:
                AI_SporeRelease();
                break;
        }
        Projectile.rotation = Projectile.velocity.X * 0.05f;
    }
    
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    void AI_Idle()
    {
        GlobalTimer++;
        var xOfffset = MathHelper.Lerp(-64, 64, ExtraMath.Osc(0f, 1f, speed: 0, Projectile.minionPos * 2));
        xOfffset += MathHelper.Lerp(-32f, 32f, MathF.Sin(GlobalTimer * 0.025f) * 0.5f + 0.5f);
        var targetPos = Owner.Center + new Vector2(0, -48) + new Vector2(xOfffset, 0);
        MoonUtils.AI_FloatAbove(Projectile.Center, ref Projectile.velocity, targetPos);
        MoonUtils.SearchForNewTargetByDistance(Projectile.Center, ref _targeter.targetNpc);
        if (_targeter.HasValidTarget)
        {
            Timer++;
            if(Timer >= 30)
            {
                SwitchState(AIState.SporeRelease);
            }
        }
    }
    void AI_SporeRelease()
    {
        Timer++;
        Projectile.velocity *= 0.96f;
        if(Timer == 20)
        {
            Projectile.frame =1;
        }
   
        if(Timer == 40)
        {
            Projectile.frame = 2;
        }
        if(Timer == 60)
        {
            if (this.OwnedByLocalClient())
            {
                for (var i = 0; i < 3; i++)
                {
                    var firer = ProjFirer.From<MushroomStaveMinionProg>(Projectile);
                    firer.velocity.X = Main.rand.NextFloat(-5f, 5f);
                    firer.velocity.Y = Main.rand.NextFloat(-15f, -10f);
                    firer.New();
                }
            }

            var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -2), TriColorPalette.Fiery, new Vector2(5, 15f));
            factory.particleCount *= 2;
            ParticleUtils.CreateSwirlingDustCircle(factory);
            for (var i = 0; i < 4; i++)
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
                Particles.RoughSmoke.Spawn(new()
                {
                    position = pos,
                    timeLeft = 25,
                    color = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(0f, 1f), Color.OrangeRed, Color.DarkRed, Color.Gold, Color.Red, Color.DarkOrange, Color.DarkGoldenrod),
                    scale = Main.rand.NextFloat(0.8f, 1f),
                    rotation = Main.rand.NextFloat(6f)
                });
            }

            SoundStyle gasSound = new SoundStyle("Stellamod/Assets/Sounds/ExplosionGaseous");
            gasSound.PitchVariance = 0.3f;
            gasSound.Volume = 0.5f;
            gasSound.Pitch = 0.75f;
            SoundEngine.PlaySound(gasSound, Projectile.position);
        }
        if(Timer == 60)
        {
            Projectile.frame = 3;
        }
        if (Timer == 67)
        {
            Projectile.frame = 4;
        }
        if (Timer == 74)
        {
            Projectile.frame = 0;
        }
        if (Timer >= 100)
        {
            SwitchState(AIState.Idle);
        }
    }

    public void DrawToRenderTargets()
    {

    }
}

public class MushroomStaveMinionProg : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Projectile.MushroomStaveMinionProg.KEY;
    public float VELXStart;
    private ref float VEL => ref Projectile.ai[0];
    ref float Timer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        Projectile.SetTrailCacheLength(8);
        Main.projFrames[Projectile.type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.light = 0.3f;
        Projectile.LocalPiercingImmunityTime = 30;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 200;
        Projectile.tileCollide = true;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 16; i++)
        {
            int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, 0f, -2f, 0, default, 1.5f);
            Main.dust[num].noGravity = true;
            Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            {
                Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(Color.Lerp(new Color(255, 136, 247), new Color(54, 64, 195), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
            color *= 0.25f;
            color.A = 0;
            Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void AI()
    {

        Timer++;
        Projectile.rotation = Projectile.velocity.X * 0.1f;
        Projectile.ai[1] += 1f;

        Projectile.velocity.X += VEL;
        Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 1.2f, 0.03f);
        if (Projectile.ai[1] >= 60)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                VEL = Main.rand.NextFloat(-0.06f, 0.06f);
                Projectile.ai[1] = 0;
                Projectile.netUpdate = true;
            }
        }

        var nearest = MoonUtils.TargetClosestEnemy(Projectile.Center, 1024);
        if(nearest != null && Timer >= 30)
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (nearest.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 7, 0.035f);
        }
    }
}