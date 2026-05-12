using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS;

public class HangingAnchor : ModProjectile
{
    private Asset<Texture2D> _chainTextureAsset;
    protected Player Owner => Main.player[Projectile.owner];
    private Chain _chain;
    private Chain Chain
    {
        get
        {
            _chain ??= new Chain(Projectile.Center, 16, 16);
            return _chain;
        }
    }


    public override void SetDefaults()
    {
        base.SetDefaults();
        //Setup Defaults
        Projectile.ignoreWater = true;
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.timeLeft = int.MaxValue;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
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


    private Vector2 SimulateChain()
    {
        Vector2 root = Owner.Center;
        Vector2 effector = Chain.points[Chain.points.Length - 1];
        Chain.pinned[0] = true;
        Chain.points[0] = root;

        int subdivisions = 16;
        for (int j = 0; j < subdivisions; j++)
        {
            for (int i = 1; i < Chain.points.Length; i++)
            {
                Vector2 gravity = Vector2.UnitY * 0.5f;
                Point point = Chain.points[i].ToTileCoordinates();
                Tile tile = Main.tile[point];
                if (Main.tileSolid[tile.TileType] && tile.HasTile)
                {
                    Chain.points[i] -= gravity;
                }
                else
                {
                    Chain.points[i] += gravity;
                }
            }
            Chain.ResolveRootToBack();
        }

        return effector;
    }

    private void AI_Dragging()
    {
        Vector2 effector = SimulateChain();
        Vector2 targetVelocity = effector - Projectile.Center;
        Projectile.velocity = targetVelocity;
        Projectile.rotation += Projectile.velocity.X * 0.02f;
    }


    public override bool PreDraw(ref Color lightColor)
    {
        _chainTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Chain");
        //Trail, Glow, Chain, Sprite, ye
        SpriteBatch spriteBatch = Main.spriteBatch;
        Vector2[] chainPositions = Chain.points;
        for (int i = 1; i < chainPositions.Length; i++)
        {
            Vector2 position = chainPositions[i];

            float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
            float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / _chainTextureAsset.Value.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

            Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
            Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
            Vector2 origin = new Vector2(_chainTextureAsset.Value.Width / 2, _chainTextureAsset.Value.Height); //Draw from center bottom of texture
            spriteBatch.Draw(_chainTextureAsset.Value, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
        }
        SpritebatchDrawer projDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(projDrawer);
        return false;
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
