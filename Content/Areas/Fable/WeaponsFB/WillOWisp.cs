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
        Item.damage = 12;
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
        Projectile.width = 12;
        Projectile.height = 12;
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

        if(CloneCount > 0 && this.OwnedByLocalClient())
        {
            var firer = ProjFirer.Copy(Projectile);
            firer.ai1 = -1;
            firer.New();
            CloneCount--;
        }
        Timer++;
        if (Timer == 1)
        {
            _oldPlayerPos = new Vector2[32];
            for (int i = 0; i < _oldPlayerPos.Length; i++)
            {
                _oldPlayerPos[i] = Owner.Center;
            }
        }

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
        float distanceToCurrent = Vector2.Distance(nextPos, _oldPlayerPos[0]);
        if (distanceToCurrent > 64)
        {
            for (int i = _oldPlayerPos.Length - 1; i > 0; i--)
            {
                _oldPlayerPos[i] = _oldPlayerPos[i - 1];
            }
            if (_oldPlayerPos.Length > 0)
                _oldPlayerPos[0] = nextPos;
        }

        //Get the index of this minion
        int minionIndex = (int)Projectile.minionPos;
        Vector2 targetPos;
        if (minionIndex < _oldPlayerPos.Length)
        {
            targetPos = _oldPlayerPos[minionIndex];
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
