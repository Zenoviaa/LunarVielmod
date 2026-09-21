using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS;

public class OceanShieldPlayer : ModPlayer
{
    private int _cooldown;
    public bool hasOceanShield;

    public override void ResetEffects()
    {
        hasOceanShield = false;
    }

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasOceanShield)
            return;
        _cooldown--;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (!hasOceanShield)
            return;
        if (drawInfo.shadow != 0)
            return;
        if (_cooldown > 0)
            return;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.WaterSide.AccWS.WaterShield.Asset, drawInfo.drawPlayer.Center);
        drawer.scale *= ExtraMath.Osc(0.9f, 1f, speed: 3);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
     
        if (hasOceanShield && modifiers.Dodgeable && _cooldown <= 0)
        {
            int cooldownInSeconds = 45;
            int cooldownInTicks = cooldownInSeconds * 60;

            _cooldown = cooldownInTicks;
            modifiers.FinalDamage *= 0.5f;

            int count = 48;
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 4;
                Dust.NewDust(Player.Center, 1, 1, DustID.Water, vel.X, vel.Y);
            }
            PixelPrimitiveCircleFactory.CreateGenericBoom(Player.Center, Color.White, Color.SkyBlue, 25, 64);
            for(float f = 0; f < 16; f++)
            {
                var pos = Player.Center;
                pos += Main.rand.NextVector2Circular(24, 24);
                var vel = Main.rand.NextVector2Circular(8, 8);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    innerColor = Color.SkyBlue.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
                });
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Player.Center);
        }
    }
}

public class OceancrestShield : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 42;
        Item.accessory = true;
        Item.defense = 4;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<OceanShieldPlayer>().hasOceanShield = true;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MusicalHarmonise, BlankAccessory>();
    }
}
