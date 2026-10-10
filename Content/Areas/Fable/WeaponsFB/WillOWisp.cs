using Stellamod.Common;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class WillOWisp : ModItem
{

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<WillOWispMinionProj>());
        Item.damage = 6;
        Item.knockBack = 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}


public class WillOWispMinionProj : AbstractBellSummon,
    IDrawToRenderTarget
{
    private float _scale;
    private Vector2[] _oldPlayerPos;
    Vector2[] OldPlayerPos
    {
        get
        {
            if (field == null)
            {
                field = new Vector2[128];
                for (int i = 0; i < field.Length; i++)
                {
                    field[i] = Owner.Center;
                }
            }
        
            return field;
        }
    }
    private ref float Timer => ref Projectile.ai[0];
    ref float CloneCount => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        // Sets the amount of frames this minion has on its spritesheet
        Main.projFrames[Projectile.type] = 4;
        Projectile.StaticDefaultToMinionProjectile();
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        if(CloneCount != -1)
        {
            CloneCount = 3;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.DefaultToMinionProjectile();
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.light = 0.278f;
        Projectile.tileCollide = false;
        Projectile.StaticPiercingImmunityTime = 20;
        Projectile.minionSlots = 0.33f;
    }

    public override int GetAggro()
    {
        return -100;
    }

    // Here you can decide if your minion breaks things like grass or pots
    public override bool? CanCutTiles()
    {
        return false;
    }

    // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
    public override bool MinionContactDamage()
    {
        return true;
    }

    public override void AI()
    {
        base.AI();


        Timer++;

        if (Timer % 12 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer % 6 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer <= 15)
        {
            _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.15f, 1f), Easing.InCubic(Timer / 5f));
            Projectile.velocity *= 0.75f;
        }

        Vector2 nextPos = Owner.Center;
        for (int i = OldPlayerPos.Length - 1; i > 0; i--)
        {
            OldPlayerPos[i] = OldPlayerPos[i - 1];
        }
        if (OldPlayerPos.Length > 0)
            OldPlayerPos[0] = nextPos;

        //Get the index of this minion
        int minionIndex = (int)Projectile.minionPos * 8;
        Vector2 targetPos;
        if (minionIndex < OldPlayerPos.Length)
        {
            targetPos = OldPlayerPos[minionIndex];
        }
        else
        {
            targetPos = Owner.Center;
        }

        //Move to that position
        Vector2 dirToPos = (targetPos - Projectile.Center) * 0.05f;
        Projectile.velocity = dirToPos;

        //All that's left is to do this ai, uhh
        Projectile.rotation = Projectile.velocity.X * 0.05f;
        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }

    public override void DrawSpectral(SpriteBatch spriteBatch)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() / 2f;
        Color drawColor = Color.White;
        float drawRotation = Projectile.rotation;
        float drawScale = _scale;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        var color = drawColor;
        color.A = 0;
        foreach (var offset in Vector2.CardinalOffsets)
        {
            Vector2 vel = offset * VectorHelper.Osc(0f, 4f, speed: 16);
            Vector2 flameDrawPos = drawPos + vel + Main.rand.NextVector2Circular(2, 2);
            flameDrawPos -= Vector2.UnitY * 4;
            spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), color, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
            spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), color, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
        }


        Texture2D blackTexture = ModContent.Request<Texture2D>(Texture + "_Black").Value;
        spriteBatch.Draw(blackTexture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (Main.rand.NextBool(8))
        {
            target.AddBuff(BuffID.OnFire, 120);
        }
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 24; i++)
        {
            int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, 0f, -2f, 0, default(Color), 1.5f);
            Dust dust = Main.dust[num];
            dust.noGravity = true;
            dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            dust.velocity = Projectile.DirectionTo(dust.position) * 6f;
        }
    }

    void DrawGlow(SpriteBatch sb, Vector2 sp)
    {
        Texture2D dimLightTexture = AssetReferences.Assets.NoiseTextures.DimLight.Asset.Value;//ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
        float drawScale = 1f;
        SpriteBatch spriteBatch = Main.spriteBatch;
        for (int i = 0; i < 3; i++)
        {
            Color glowColor = new Color(85, 45, 15) * 0.5f;
            glowColor.A = 0;
            spriteBatch.Draw(dimLightTexture, Projectile.Center - Main.screenPosition, null, glowColor,
                Projectile.rotation, dimLightTexture.Size() / 2f, drawScale * VectorHelper.Osc(0.75f, 1f, speed: 32, offset: Projectile.whoAmI), SpriteEffects.None, 0f);
        }
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawGlow);
    }
}
