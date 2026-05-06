using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS;

public class HangingAnchor : ModProjectile
{
    private bool _playedBounceSound;
    protected ref float Timer => ref Projectile.ai[1];
    protected ref float Dir => ref Projectile.ai[2];
    protected Player Owner => Main.player[Projectile.owner];
    public virtual float MaxThrowDistance { get; }
    protected float dragDistance;
    protected float TipDamageMultiplier;
    protected float ExtraUpdateMult => 8;
    protected Texture2D ChainTexture
    {
        get
        {
            return ModContent.Request<Texture2D>(Texture + "_Chain").Value;
        }
    }

    private VerletChain _verletChain;
    private VerletChain VerletChain
    {
        get
        {
            _verletChain ??= new VerletChain(16, Projectile.Center, -Vector2.UnitY);
            return _verletChain;
        }
    }

    public SoundStyle? SwingSound { get; set; }
    public SoundStyle? BounceSound { get; set; }
    public float SwingSoundProgress { get; set; }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        //Setup Defaults
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = int.MaxValue;
        Projectile.timeLeft = int.MaxValue;
        Projectile.extraUpdates = (int)ExtraUpdateMult - 1;
        Projectile.friendly = true;


        dragDistance = 126;

        SoundStyle soundStyle = SoundRegistry.BallSwing;
        soundStyle.PitchVariance = 0.15f;
        soundStyle.Volume = 0.25f;
        SwingSound = soundStyle;

        SoundStyle bounceStyle = SoundID.DD2_WitherBeastCrystalImpact;
        bounceStyle.PitchVariance = 0.15f;
        BounceSound = bounceStyle;
    }

    public override void AI()
    {
        base.AI();
        //Dunno why I didn't just do this with uhhh Gun Holsters lol
        //This is such an easy way to instakill this thing
        if (!Owner.GetModPlayer<SunkenAnchorPlayer>().hasSunkenAnchor)
        {
            Projectile.Kill();
            return;
        }

        AI_Dragging();
    }

    private void AI_Dragging()
    {
        Timer++;
        Projectile.tileCollide = false;

        Vector2 root = Owner.Center;
        VerletChain.gravity = 0f;
        VerletChain.pointRadius = 16;
        VerletChain.points[0].pinned = true;
        VerletChain.points[0].position = root;
        VerletChain.subdivisionCount = 1;
        VerletChain.segmentLength = 16;
        VerletChain.Update();

        Vector2 effector = VerletChain.points[VerletChain.points.Length - 1].position;
        Vector2 targetVelocity = effector - Projectile.Center;
        Projectile.velocity = targetVelocity;
        Projectile.rotation += Projectile.velocity.X * 0.02f;
    }



    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        //Bounce code
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -oldVelocity.Y * 0.7f;

        if (!_playedBounceSound && BounceSound != null)
        {
            SoundEngine.PlaySound(BounceSound, Projectile.position);
            _playedBounceSound = true;
        }
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //Trail, Glow, Chain, Sprite, ye
        SpriteBatch spriteBatch = Main.spriteBatch;
        Vector2[] chainPositions = CalculateChainPositions();
        for (int i = 1; i < chainPositions.Length; i++)
        {
            Vector2 position = chainPositions[i];

            float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
            float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / ChainTexture.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

            Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
            Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
            Vector2 origin = new Vector2(ChainTexture.Width / 2, ChainTexture.Height); //Draw from center bottom of texture
            spriteBatch.Draw(ChainTexture, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
        }
        SpritebatchDrawer projDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(projDrawer);
        return false;
    }


    private Vector2[] CalculateChainPositions()
    {
        Vector2 ownerPos = Owner.Center;
        List<Vector2> controlPoints = new List<Vector2>();
        controlPoints.Add(Owner.Center);

        Vector2 controlPoint1 = Vector2.Lerp(Owner.Center, Projectile.Center, 0.25f);
        controlPoint1.Y += MathHelper.Lerp(64, 0, Easing.SpikeInOutBounce(0f));
        controlPoints.Add(controlPoint1);
        controlPoints.Add(Projectile.Center);

        int numPoints = (int)(Vector2.Distance(Projectile.Center, Owner.Center) / ChainTexture.Height);
        Vector2[] chainPositions = GetBezierApproximation(controlPoints.ToArray(), numPoints);
        return chainPositions;
    }

    //Found some simple bezier curve stuff :DDDD
    //YAAAAAAAAAAAAAY
    private Vector2[] GetBezierApproximation(Vector2[] controlPoints, int outputSegmentCount)
    {
        Vector2[] points = new Vector2[outputSegmentCount + 1];
        for (int i = 0; i <= outputSegmentCount; i++)
        {
            float t = (float)i / outputSegmentCount;
            points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
        }
        return points;
    }

    private Vector2 GetBezierPoint(float t, Vector2[] controlPoints, int index, int count)
    {
        if (count == 1)
            return controlPoints[index];
        var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
        var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
        return new Vector2((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
    }

}
public class SunkenAnchorPlayer : ModPlayer
{
    public bool hasSunkenAnchor;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSunkenAnchor = false;
    }
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (Main.myPlayer != Player.whoAmI)
            return;
        if (!hasSunkenAnchor)
            return;
        if (Player.ownedProjectileCounts[ModContent.ProjectileType<HangingAnchor>()] > 0)
            return;
        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<HangingAnchor>(), 1, 1, Player.whoAmI);
    }
}
public class SunkenAnchor : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.ignoreWater = true;
        player.GetModPlayer<SunkenAnchorPlayer>().hasSunkenAnchor = true;
        player.GetStats().enemyEndurance += 0.2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
    }
}
