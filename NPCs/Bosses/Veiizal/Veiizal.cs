
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.Veiizal;

public class VeiizalBossBar : ModBossBar
{
    private int VeiizalBossHead = -1;
    public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
    {
        if (VeiizalBossHead != -1)
        {
            return TextureAssets.NpcHeadBoss[VeiizalBossHead];
        }
        return null;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
    {
        // Make the bar shake the less health the NPC has
        float lifePercent = drawParams.Life / drawParams.LifeMax;
        float shakeIntensity = Utils.Clamp(1f - lifePercent - 0.2f, 0f, 1f);
        drawParams.BarCenter.Y -= 20f;
        drawParams.BarCenter += Main.rand.NextVector2Circular(0.5f, 0.5f) * shakeIntensity * 15f;

        VeiizalBossHead = npc.GetBossHeadTextureIndex();
        return true;
    }
}

[AutoloadBossHead]
public class Veiizal : ModNPC
{
    private float _despawnTimer;
    private int _frameSpeed = 10;
    private int _frame = 0;

    private bool _hasSpawned;
    public int State = 0;
    public int MaxAttac = 0;
    public int Attack;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(Attack);
        writer.Write(_hasSpawned);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        Attack = reader.ReadInt32();
        _hasSpawned = reader.ReadBoolean();
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 31;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        NPC.netAlways = true;
        NPC.scale = 1;
        NPC.width = 80;
        NPC.height = 190;
        NPC.damage = 40;
        NPC.defense = 11;
        NPC.lifeMax = 1800;
        NPC.scale = 1f;
        NPC.value = 60f;
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 10f;        
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
        NPC.BossBar = GetInstance<VeiizalBossBar>();
        NPC.aiStyle = 0;
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VampireDance");
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.frameCounter >= _frameSpeed)
        {
            _frame++;
            NPC.frameCounter = 0;
        }

        if (State == 0)
        {
            if (_frame >= 4 && _frame <= 6)
            {
                _frame = 0;
            }
            if (_frame <= 28 && _frame >= 4)
            {
                _frame = 29;
            }
            if (_frame >= 31)
            {
                _frame = 0;
            }
        }
        if (State == 1)
        {
            if (_frame >= 10)
            {
                _frame = 9;
            }
            if (_frame == 8)
            {
                _frame = 9;
            }
            if (_frame <= 4)
            {
                _frame = 6;
            }
        }
        if (State == 2)
        {

            if (_frame >= 14)
            {
                State = 0;
                _frame = 0;
            }
            if (_frame <= 10)
            {
                _frame = 11;
            }
        }
        if (State == 3)
        {
            if (_frame >= 19)
            {
                _frame = 18;
            }
            if (_frame == 17)
            {
                _frame = 18;
            }
            if (_frame <= 14)
            {
                _frame = 15;
            }
        }
        if (State == 4)
        {

            if (_frame >= 22)
            {
                _frame = 20;
            }
            if (_frame <= 19)
            {
                _frame = 20;
            }
        }
        if (State == 5)
        {

            if (_frame >= 28)
            {
                _frame = 25;
            }
            if (_frame <= 22)
            {
                _frame = 23;
            }
        }
        NPC.frame.Y = frameHeight * _frame;
    }

    public void CasuallyApproachChild(float AddPosx, float AddPosy)
    {
        Player player = Main.player[NPC.target];
        NPC.velocity.Y *= 0.94f;
        NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, new Vector2(player.Center.X + AddPosx, player.Center.Y + AddPosy), 0.025f), base.NPC.Center.Distance(player.Center) * 0.3f), 0.005f);
    }

    public override void AI()
    {
        if (!_hasSpawned)
        {
            _hasSpawned = true;
            NPC.ai[1] = 4;
            NPC.netUpdate = true;
        }

        MaxAttac = 5;
        var entitySource = NPC.GetSource_FromThis();
        Player player = Main.player[NPC.target];
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest(false);
            if (!NPC.HasValidTarget)
            {
                _despawnTimer++;
            }
        }

        if (_despawnTimer > 0)
        {
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y -= 0.5f;
            _despawnTimer++;
            if (_despawnTimer >= 180)
            {
                NPC.active = false;

            }
            return;
        }

        DrawChargeTrail = false;
        switch (NPC.ai[1])
        {
            case 0:
                // default attack, just moves above player, waits  seconds then does a random attack
                NPC.ai[0]++;
                if (NPC.ai[0] > 2)
                {
                    NPC.ai[0] = 0;
                    Attack++;

                
                    if (Attack >= MaxAttac)
                        Attack = 1;
                    NPC.ai[1] = Attack;
                    NPC.netUpdate = true;

                }

                break;
            case 1:
                // default attack, just moves above player, waits  seconds then does a random attack
                NPC.ai[0]++;
                if (NPC.ai[0] > 2)
                {
                    if (NPC.Center.X >= player.Center.X)
                    {
                        CasuallyApproachChild(300, 0);
                    }
                    else
                    {
                        CasuallyApproachChild(-300, 0);
                    }


                }
                if (NPC.ai[0] == 50)
                {
                    int Sound = Main.rand.Next(0, 3);
                    if (Sound == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen1"), NPC.position);
                    }
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen2"), NPC.position);
                    }
                    if (Sound == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen3"), NPC.position);
                    }
                    State = 1;
                }
                if (NPC.ai[0] == 120)
                {
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 82f);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__UShot1"), NPC.position);
                    State = 2;
                    if (NPC.Center.X >= player.Center.X)
                    {
                        NPC.velocity.X += 20;
                    }
                    else
                    {
                        NPC.velocity.X -= 20;
                    }
                    int damage = Main.expertMode ? 50 : 68;
                    if (NPC.Center.X <= player.Center.X)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X + 40, NPC.Center.Y + 10), new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), damage, 0f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X + 40, NPC.Center.Y + 10), new Vector2(30, 0), ModContent.ProjectileType<VeiizalBeam>(), damage, 0f);
                        }
                    }
                    else
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X - 40, NPC.Center.Y + 10), new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), damage, 0f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X - 40, NPC.Center.Y + 10), new Vector2(-30, 0), ModContent.ProjectileType<VeiizalBeam>(), damage, 0f);
                        }
                    }

                }
                if (NPC.ai[0] >= 130)
                {
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                }

                break;

            case 2:
                // default attack, just moves above player, waits  seconds then does a random attack
                NPC.ai[0]++;
                if (NPC.ai[0] > 2 && NPC.ai[0] < 120)
                {
                    if (NPC.Center.X >= player.Center.X)
                    {
                        CasuallyApproachChild(300, 0);
                    }
                    else
                    {
                        CasuallyApproachChild(-300, 0);
                    }


                }

                if (NPC.ai[0] == 50)
                {
                    int Sound = Main.rand.Next(0, 3);
                    if (Sound == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen1"), NPC.position);
                    }
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen2"), NPC.position);
                    }
                    if (Sound == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen3"), NPC.position);
                    }
                    State = 3;
                }
                if (NPC.ai[0] == 120)
                {
               ;
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__UShot1") with { PitchVariance = 0.3f }, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), 0, 0f);
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 32);
                    if (NPC.Center.X >= player.Center.X)
                    {
                        NPC.velocity.X -= 80;
                    }
                    else
                    {
                        NPC.velocity.X += 80;
                    }

                }
                if (NPC.ai[0] < 120)
                {
                    float distance = 128;
                    float particleSpeed = 8;

                    Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                    Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
                    var d = Dust.NewDustPerfect(position, DustID.GemRuby, speed, Scale: 1f);
                    d.noGravity = true;
                }
                if (NPC.ai[0] >= 120)
                {
                    DrawChargeTrail = true;
                    NPC.velocity *= 0.985f;
                }

                if (NPC.ai[0] >= 200)
                {
                    State = 0;
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                }

                break;
            case 3:
                // default attack, just moves above player, waits  seconds then does a random attack
                NPC.ai[0]++;
                if (NPC.ai[0] > 2 && NPC.ai[0] < 80)
                {
                    if (NPC.Center.X >= player.Center.X)
                    {
                        CasuallyApproachChild(300, 0);
                    }
                    else
                    {
                        CasuallyApproachChild(-300, 0);
                    }


                }
                if (NPC.ai[0] > 80)
                {
                    if (NPC.Center.X >= player.Center.X)
                    {
                        CasuallyApproachChild(100, 0);
                    }
                    else
                    {
                        CasuallyApproachChild(-100, 0);
                    }
                    if (NPC.ai[0] % 7 == 0)
                    {
                        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 18f);
                        int Sound = Main.rand.Next(1, 3);
                        if (Sound == 1)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__MG1"), NPC.position);
                        }
                        if (Sound == 2)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__MG2"), NPC.position);
                        }
                        if (NPC.Center.X >= player.Center.X)
                        {
                            NPC.velocity.X += 5;
                        }
                        else
                        {
                            NPC.velocity.X -= 5;
                        }
                        Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                        direction = direction.SafeNormalize(Vector2.Zero);
                        int damage = 40;
                        if (NPC.Center.X <= player.Center.X)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X + 40, NPC.Center.Y + 10), new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), damage, 0f);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X + 40, NPC.Center.Y + 10), new Vector2(30, 0), ModContent.ProjectileType<VeiizalBullet>(), damage, 0f);
                            }
                        }
                        else
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X - 40, NPC.Center.Y + 10), new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), damage, 0f);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X - 40, NPC.Center.Y + 10), new Vector2(-30, 0), ModContent.ProjectileType<VeiizalBullet>(), damage, 0f);
                            }
                        }
                    }
                }
                if (NPC.ai[0] == 50)
                {
                    int Sound = Main.rand.Next(0, 3);
                    if (Sound == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen1"), NPC.position);
                    }
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen2"), NPC.position);
                    }
                    if (Sound == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen3"), NPC.position);
                    }
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__MG3"), NPC.position);
                    State = 3;
                }
                if (NPC.ai[0] == 80)
                {
                    State = 4;

                    _frameSpeed = 3;
                }
                if (NPC.ai[0] >= 220)
                {
                    _frameSpeed = 10;
                    State = 0;
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                }

                break;
            case 4:
                // default attack, just moves above player, waits  seconds then does a random attack
                NPC.velocity.X *= 1.03f;
                NPC.velocity.Y *= 0.99f;
                NPC.ai[0]++;
                if (NPC.ai[0] == 20)
                {
                    int Sound = Main.rand.Next(1, 3);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__USpawn1"), NPC.position);
                    }
                    if (Sound == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__USpawn2"), NPC.position);
                    }
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-50, 50), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                        NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                        NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                        NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                        NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                        NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                    }

                    State = 5;
                }
                if (NPC.ai[0] == 130)
                {
                    State = 0;
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                }


                break;
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if(Main.bloodMoon && DownedBossSystem.downedDreadBoss && !NPC.AnyNPCs(Type))
        {
            if (!DownedBossSystem.downedUmbrellaBoss)
            {
                return 0.1f;
            }
            return 0.01f;
        }
        return 0f;
    }

    public override void OnKill()
    {
        NPC.SetEventFlagCleared(ref DownedBossSystem.downedUmbrellaBoss, -1);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VampiricVine>()));
    }
    private bool DrawChargeTrail;
    private float ChargeTrailOpacity;
    public PrimDrawer TrailDrawer { get; private set; } = null;
    public float WidthFunctionCharge(float completionRatio)
    {
        return NPC.width * NPC.scale / 0.7f * (1f - completionRatio) * 0.6f;
    }

    public Color ColorFunctionCharge(float completionRatio)
    {
        if (!DrawChargeTrail)
        {
            ChargeTrailOpacity -= 0.05f;
            if (ChargeTrailOpacity <= 0)
                ChargeTrailOpacity = 0;
        }
        else
        {
            ChargeTrailOpacity += 0.05f;
            if (ChargeTrailOpacity >= 1)
                ChargeTrailOpacity = 1;
        }

        Color color = Color.Lerp(Color.White, Color.Red, completionRatio);
        return color * NPC.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f) * ChargeTrailOpacity * (1f - completionRatio);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        if (TrailDrawer == null)
        {
            TrailDrawer = new PrimDrawer(WidthFunctionCharge, ColorFunctionCharge, GameShaders.Misc["VampKnives:BasicTrail"]);
        }

        GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
        Vector2 size = NPC.Size;
        TrailDrawer.DrawPrims(NPC.oldPos, size * 0.5f - screenPos, 155);

        Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
        Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
        // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
        Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
        float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
        Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
        Texture2D texture = Request<Texture2D>(Texture).Value;



        Vector2 frameOrigin = NPC.frame.Size();
        Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 100, NPC.height - NPC.frame.Height + 0);
        Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

        float time = Main.GlobalTimeWrappedHourly;
        float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

        time %= 4f;
        time /= 2f;

        if (time >= 1f)
        {
            time = 2f - time;
        }

        time = time * 0.5f + 0.5f;
        SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        for (float i = 0f; i < 1f; i += 0.25f)
        {
            float radians = (i + timer) * MathHelper.TwoPi;

            spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(99, 39, 51, 50), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
        }

        for (float i = 0f; i < 1f; i += 0.34f)
        {
            float radians = (i + timer) * MathHelper.TwoPi;
            spriteBatch.Draw(texture, drawPos + new Vector2(0f, 16f).RotatedBy(radians) * time, NPC.frame, new Color(255, 8, 55, 77), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
        }

        return true;
    }
    Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
    public virtual string GlowTexturePath => Texture + "_Glow";
    private Asset<Texture2D> _glowTexture;
    public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        float num108 = 4;
        float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
        float num106 = 0f;
        Color color1 = Color.DarkRed * num107 * .8f;
        var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(
            GlowTexture,
            NPC.Center - Main.screenPosition + Drawoffset,
            NPC.frame,
            color1,
            NPC.rotation,
            NPC.frame.Size() / 2,
            NPC.scale,
            effects,
            0
        );
        SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
        Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);
        for (int num103 = 0; num103 < 1; num103++)
        {
            Color color28 = color29;
            color28 = NPC.GetAlpha(color28);
            color28 *= 1f - num107;
            Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
            Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
        }
    }
}