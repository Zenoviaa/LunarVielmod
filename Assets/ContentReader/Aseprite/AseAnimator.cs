using ReLogic.Content;
using Stellamod.Common.Animations;
using Stellamod.Core.NPCHelpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.ContentReader.Aseprite;

public static class AnimationExtensions
{
    public static AseAnimator GetAnimator(this ModNPC modNpc)
    {
        return modNpc.NPC.GetAnimator();
    }
    public static AseAnimator GetAnimator(this NPC npc)
    {
        return npc.GetGlobalNPC<AnimatorGlobalNPC>().Animator;
    }
    public static void SetDrawOrigin(this ModNPC modNpc, Vector2 drawOrigin)
    {
        ref DrawEffects drawEffects = ref modNpc.GetAnimator().drawEffects;
        drawEffects.DrawOrigin = drawOrigin;
    }
    public static void SetScale(this ModNPC modNpc, Vector2 scale)
    {
        ref DrawEffects drawEffects = ref modNpc.GetAnimator().drawEffects;
        drawEffects.Scale = scale;
    }
    public static void SetSpriteEffects(this ModNPC modNpc, SpriteEffects spriteEffects)
    {
        var Animator = modNpc.GetAnimator().spriteEffects = spriteEffects;
    }
    public static void DrawAnimator(this NPC npc, SpriteBatch spriteBatch, Color drawColor)
    {
        var Animator = npc.GetAnimator();
        SpritebatchDrawer drawer = Animator.GetSprite(npc.Center);
        drawer.spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        drawer.spriteEffects |= Animator.spriteEffects;
        drawer.rotation = npc.rotation;
        drawer.color = drawColor;
        if (npc.spriteDirection == -1)
        {
            drawer.drawOrigin.X = drawer.sourceRect!.Value.Width - drawer.drawOrigin.X;
        }

        //Offset it even with the draw origin so the sprite is still in the center of the hitbox
        Vector2 offset = drawer.drawOrigin - Animator.centerDrawOrigin;
        drawer.worldPosition += offset;
        spriteBatch.Draw(drawer);
    }
    public static void DrawAnimator(this NPC npc, SpriteBatch spriteBatch, Color drawColor, Vector2 position)
    {
        var Animator = npc.GetAnimator();
        SpritebatchDrawer drawer = Animator.GetSprite(npc.Center);
        drawer.spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        drawer.spriteEffects |= Animator.spriteEffects;
        drawer.rotation = npc.rotation;
        drawer.color = drawColor;
        if (npc.spriteDirection == -1)
        {
            drawer.drawOrigin.X = drawer.sourceRect!.Value.Width - drawer.drawOrigin.X;
        }

        //Offset it even with the draw origin so the sprite is still in the center of the hitbox
        Vector2 offset = drawer.drawOrigin - Animator.centerDrawOrigin;
        drawer.worldPosition = position + offset;
        spriteBatch.Draw(drawer);
    }
}

public class AnimatorGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public AseAnimator Animator;
    public override void FindFrame(NPC npc, int frameHeight)
    {
        base.FindFrame(npc, frameHeight);
        if (NPCSets.UseAseprite[npc.type])
        {
            Animator?.Update();
        }
    }

    public override void SetDefaults(NPC entity)
    {
        base.SetDefaults(entity);
        if (AsepriteAssets.Npc == null)
            return;
        Animator = new AseAnimator(AsepriteAssets.Npc[entity.type]);
    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        if (NPCSets.UseAseprite[entity.type])
            return true && lateInstantiation;
        return false;
    }
}

public record struct AnimationParams(bool IsLooping = false)
{
    public static readonly AnimationParams NoLooping = new AnimationParams(IsLooping: false);
    public static readonly AnimationParams Default = new AnimationParams(IsLooping: true);
}
public record struct DrawEffects(Vector2? DrawOrigin, Vector2? Scale)
{

};

public class AseAnimator
{
    private float _frameCounter;
    private int _frameIndex;
    public AseAnimator(Asset<AseSprite> sprite)
    {
        Sprite = sprite;
        drawEffects = default;
        centerDrawOrigin = new Vector2(sprite.Value.FrameWidth * 0.5f, sprite.Value.FrameHeight * 0.5f);
    }
    public readonly Asset<AseSprite> Sprite;
    public AseTags playingTag;
    public string currentAnimation;
    public bool isLooping;
    public DrawEffects drawEffects;
    public SpriteEffects spriteEffects;
    public Vector2 centerDrawOrigin;
    public SpritebatchDrawer GetSprite() => GetSprite(Vector2.Zero);
    public SpritebatchDrawer GetSprite(Vector2 worldPosition)
    {
        var sprite = Sprite.Value.GetSprite(_frameIndex, worldPosition);
        sprite.drawOrigin = drawEffects.DrawOrigin.HasValue ? drawEffects.DrawOrigin.Value : sprite.drawOrigin;
        sprite.scale = drawEffects.Scale.HasValue ? drawEffects.Scale.Value : sprite.scale;
        sprite.spriteEffects = spriteEffects;
        return sprite;
    }
    public void PlayAnimation(string name, AnimationParams? animationParams = null)
    {
        if (animationParams == null)
        {
            animationParams = AnimationParams.Default;
        }


        if (playingTag != null && playingTag.name == name)
            return;
        playingTag = Sprite.Value.tags.Find(x => x.name == name);
        _frameCounter = 0;
        _frameIndex = playingTag.from;
        this.isLooping = animationParams.Value.IsLooping;
    }

    public void Update()
    {
        if (playingTag == null)
            return;
        //TODO: instead take in an elapsed time and calculate the current frame
        //That would be better net synced.

        //rn just increasing by 1 / 60 since that's the game's tick rate
        _frameCounter += 1f / 60f;
        float ft = Sprite.Value.frames[_frameIndex].frameTime;
        while (_frameCounter >= ft)
        {
            _frameCounter -= ft;
            _frameIndex++;
            if (isLooping)
            {
                //0 is infinite, so repeat the animation
                if (_frameIndex > playingTag.to)
                {
                    _frameIndex = playingTag.from;
                }
            }
            else
            {
                if (_frameIndex > playingTag.to)
                {
                    _frameIndex = playingTag.to;
                }
            }
        }
    }
}
