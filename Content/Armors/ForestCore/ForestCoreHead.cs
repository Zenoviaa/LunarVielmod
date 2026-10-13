using ReLogic.Content;
using Stellamod.Buffs.Minions;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.ClassReworkSystem;
using Stellamod.Common.Particles;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.ForestCore;

public class ForestCoreBow : ModProjectile
{
    Targeter _targeter;
    enum AIState : byte
    {
        Idle,
        DrawShoot
    }
    private ref float Timer => ref Projectile.ai[0];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    ref float GlobalTimer => ref Projectile.ai[2];
    Player Owner => Main.player[Projectile.owner];
    float IdleTime => 60;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 6;
    }

    public sealed override void SetDefaults()
    {
        Projectile.width = Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        _targeter.NetSend(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        _targeter.NetReceive(reader);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void AI()
    {

        Player player = Main.player[Projectile.owner];
        ForestCorePlayer forestCorePlayer = player.GetModPlayer<ForestCorePlayer>();
        if (!forestCorePlayer.hasForestCoreSet)
            return;
        Projectile.timeLeft = 2;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.DrawShoot:
                AI_DrawShoot();
                break;
        }
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }
    void AI_Idle()
    {
        Projectile.frame = 0;
        GlobalTimer++;
        var xOfffset = MathHelper.Lerp(-64, 64, ExtraMath.Osc(0f, 1f, speed: 0, Projectile.minionPos * 2));

        var targetPos = Owner.Center + new Vector2(0, -128) + new Vector2(xOfffset, MathF.Sin(GlobalTimer * 0.03F) * 32);
        MoonUtils.AI_FloatAbove(Projectile.Center, ref Projectile.velocity, targetPos);
        MoonUtils.SearchForNewTargetByDistance(Projectile.Center, ref _targeter.targetNpc);
        if (_targeter.HasValidTarget)
        {
            Timer++;
            if(Timer >= IdleTime)
            {
                SwitchState(AIState.DrawShoot);
            }
        }
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.03f);
    }

    void AI_DrawShoot()
    {
        Timer++;
        if(Timer == 10)
        {
            Projectile.frame = 1;
        }
        if(Timer == 20)
        {
            Projectile.frame = 2;
        }
        if(Timer == 30)
        {
            Projectile.frame = 3;
        }
        var posToAimAt = _targeter.Target.Center;
        Projectile.velocity *= 0.96f;
        var targetVel = (posToAimAt - Projectile.Center).SafeNormalize(Vector2.Zero);
        var targetRot = targetVel.ToRotation();
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, targetRot, 0.08f);
        if(Timer == 60)
        {
            var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, posToAimAt, TriColorPalette.Foresty, new Vector2(5, 15f));
            factory.particleCount = 5;
            ParticleUtils.CreateSwirlingDustBurst(factory);
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), 
                    Projectile.Center, 
                    targetVel * 20,
                    ProjectileID.WoodenArrowFriendly,
                    Owner.GetModPlayer<ClassReworkPlayer>().ScalingWeaponDamage(), 1, Projectile.owner, 0, 0);
            }
            Projectile.velocity += targetVel * 4;
        }

        if(Timer == 60)
        {
            Projectile.frame = 4;
        }

        if(Timer == 65)
        {
            Projectile.frame = 5;
        }
        if (Timer >= 70)
        {
            Projectile.frame = 0;
        }
        if(Timer >= 90)
        {
            SwitchState(AIState.Idle);
        }
    }
}


public class ForestCoreDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.legs == ModContent.GetInstance<ForestCoreLegs>().Item.legSlot;
    }

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Leggings);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y += 8;

        var texAsset = AssetReferences.Content.Armors.ForestCore.ForestTail.Asset;
        Rectangle bodyFrame = drawInfo.drawPlayer.legFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += yOsc * 2;
      //  position.X -= 6;
        float rotation = yOsc * MathHelper.ToRadians(5) + ExtraMath.Osc(-0.05f, 0.05f) - 0.06f;
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        var origin = new Vector2(39, 22);
        if (drawInfo.drawPlayer.direction == -1)
            origin.X = 40 - 35;

        var drawData = new DrawData(
            texAsset.Value,
            position,
            null,
            drawInfo.colorArmorLegs,
            rotation,
            origin,
            1f,
           spriteEffects,
            0
        );

        drawData.shader = drawInfo.cLegs;
        drawInfo.DrawDataCache.Add(drawData);
    }
}
public class ForestCorePlayer : ModPlayer
{
    public bool hasForestCoreSet;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasForestCoreSet = false;
    }
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (Main.myPlayer == Player.whoAmI && hasForestCoreSet)
        {
            int type = ModContent.ProjectileType<ForestCoreBow>();
            if (Player.ownedProjectileCounts[type] == 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, type, 6, 1, Player.whoAmI);
            }
        }
        if (hasForestCoreSet)
        {
            Player.AddBuff(ModContent.BuffType<FCMinionBuff>(), 2);
        }
    }
}
[AutoloadEquip(EquipType.Head)]
public class ForestCoreHead : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorSetSystem.RegisterArmorSet<ForestCoreHead, ForestCoreBody, ForestCoreLegs>(ArmorGroup.Act_I);
    }

    public override void SetDefaults()
    {

    }

    public override void UpdateEquip(Player player)
    {
        ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
        stats.insourceTimeFlatBonus += 2;
        stats.accessorySlots++;
        stats.defenseBonus += 2;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<ForestCoreBody>() && legs.type == ModContent.ItemType<ForestCoreLegs>();
    }

    public override void ArmorSetShadows(Player player)
    {

    }

    public override void UpdateArmorSet(Player player)
    {
        ForestCorePlayer forestCorePlayer = player.GetModPlayer<ForestCorePlayer>();
        forestCorePlayer.hasForestCoreSet = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class ForestCoreBody : ModItem
{
    public override void SetDefaults()
    {

    }

    public override void UpdateEquip(Player player)
    {
        ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
        stats.stamina += 1;
        stats.accessorySlots++;
        stats.defenseBonus += 2;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class ForestCoreLegs : ModItem
{
    public override void SetDefaults()
    {

    }

    public override void UpdateEquip(Player player)
    {
        ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
        stats.inventorySlots += 3;
        stats.insourceSlots+=2;
        stats.defenseBonus += 1;
    }
}
