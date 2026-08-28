using Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.AccPT;

public class BoomboxerGlobalNPC : GlobalNPC
{
    public static bool Cheer;
    public override void OnKill(NPC npc)
    {
        base.OnKill(npc);
        Cheer = true;
    }
}


public class Boomboxer : ModItem
{
    // Names and descriptions of all ExamplePetX classes are defined using .hjson files in the Localization folder
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.ZephyrFish); // Copy the Defaults of the Zephyr Fish Item.
        Item.shoot = ModContent.ProjectileType<BoomboxerPet>(); // "Shoot" your pet projectile.
        Item.buffType = ModContent.BuffType<BoomboxerPetBuff>(); // Apply buff upon usage of the Item.
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            player.AddBuff(Item.buffType, 3600);
        }
        return true;
    }
}
public class BoomboxerPet : ModProjectile
{
    private float _cheerTimer;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_cheerTimer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _cheerTimer = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 1;
        Main.projPet[Type] = true;

        // This code is needed to customize the vanity pet display in the player select screen. Quick explanation:
        // * It uses fluent API syntax, just like Recipe
        // * You start with ProjectileID.Sets.SimpleLoop, specifying the start and end frames as well as the speed, and optionally if it should animate from the end after reaching the end, effectively "bouncing"
        // * To stop the animation if the player is not highlighted/is standing, as done by most grounded pets, add a .WhenNotSelected(0, 0) (you can customize it just like SimpleLoop)
        // * To set offset and direction, use .WithOffset(x, y) and .WithSpriteDirection(-1)
        // * To further customize the behavior and animation of the pet (as its AI does not run), you have access to a few vanilla presets in DelegateMethods.CharacterPreview to use via .WithCode(). You can also make your own, showcased in MinionBossPetProjectile
        ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 6)
            .WithOffset(-10, -20f)
            .WithSpriteDirection(-1)
            .WithCode(DelegateMethods.CharacterPreview.Float);
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.DD2PetGato);
        AIType = ProjectileID.DD2PetGato;
    }

    public override bool PreAI()
    {
        Player player = Main.player[Projectile.owner];

        player.zephyrfish = false; // Relic from AIType

        return true;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        // Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
        if (!player.dead && player.HasBuff(ModContent.BuffType<BoomboxerPetBuff>()))
        {
            Projectile.timeLeft = 2;
        }

        if (BoomboxerGlobalNPC.Cheer)
        {
            _cheerTimer = 60;
            if (this.OwnedByLocalClient())
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Kabloowie>(), 1, 1, Projectile.owner, ai1: 1);
            BoomboxerGlobalNPC.Cheer = false;
        }
        if (_cheerTimer > 0)
        {
            _cheerTimer--;
            if (_cheerTimer <= 0)
            {
                SoundStyle cheering = AssetRegistry.Sounds.Collosseum.GintzeCheer;
                SoundEngine.PlaySound(cheering, Projectile.position);
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
}
public class BoomboxerPetBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.vanityPet[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    { // This method gets called every frame your buff is active on your player.
        bool unused = false;
        player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<BoomboxerPet>());
    }
}