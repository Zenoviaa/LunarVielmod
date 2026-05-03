using ReLogic.Content;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace Stellamod.Helpers;

/// <summary>
/// A collection of utility functions for drawing simple visual effects
/// </summary>
public static class DrawUtilities
{
    public delegate Color GetTrailColor(float completionRatio);
    public delegate float GetTrailWidth(float completionRatio);

    public static Vector2 RandomPositionInNPCRect(this NPC npc)
    {
        Vector2 pos = new Vector2();
        pos.X = Main.rand.Next(0, npc.width);
        pos.Y = Main.rand.Next(0, npc.height);
        pos += npc.position;
        return pos;
    }

    public static SpritebatchDrawer GetDrawer(this Asset<Texture2D> textureAsset, Vector2 worldPosition)
    {
        return SpritebatchDrawer.FromTextureAsset(textureAsset, worldPosition);
    }

    public static void Draw(this SpriteBatch spriteBatch, SpritebatchDrawer drawer)
    {
        if (drawer.blackIsTransparency)
            drawer.color.A = 0;
        if (drawer.dstRect.HasValue)
        {
            spriteBatch.Draw(drawer.texture, drawer.dstRect.Value, drawer.sourceRect, drawer.color, drawer.rotation, drawer.drawOrigin, drawer.spriteEffects, 0);
            return;
        }
        spriteBatch.Draw(drawer.texture, drawer.worldPosition - Main.screenPosition, drawer.sourceRect, drawer.color, drawer.rotation, drawer.drawOrigin, drawer.scale, drawer.spriteEffects, 0);
    }

    /// <summary>
    /// Draws an after image trail
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="modProjectile"></param>
    public static void DrawBasicAfterImage(SpriteBatch spriteBatch, Projectile projectile, GetTrailColor getTrailColor, GetTrailWidth getTrailWidth)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        SpritebatchDrawer spritebatchDrawer = SpritebatchDrawer.FromProjectile(projectile);

        //Create an after image effect
        //Gonna extract this to a function
        for (int i = 0; i < projectile.oldPos.Length; i++)
        {
            float ratio = (float)i / (float)projectile.oldPos.Length;
            Color afterImageColor = getTrailColor(ratio);
            float afterImageScale = getTrailWidth(ratio);

            spritebatchDrawer.worldPosition = projectile.oldPos[i] + projectile.Size * 0.5f;
            spritebatchDrawer.color = afterImageColor;
            spritebatchDrawer.scale = Vector2.One * afterImageScale;
            spritebatchDrawer.rotation = projectile.oldRot[i];
            spriteBatch.Draw(spritebatchDrawer);
        }
    }
    public static void DrawBasicAfterImage(SpriteBatch spriteBatch, NPC npc, GetTrailColor getTrailColor, GetTrailWidth getTrailWidth, SpritebatchDrawer spritebatchDrawer)
    {
        //Create an after image effect
        //Gonna extract this to a function
        for (int i = 0; i < npc.oldPos.Length; i++)
        {
            float ratio = (float)i / (float)npc.oldPos.Length;
            Color afterImageColor = getTrailColor(ratio);
            float afterImageScale = getTrailWidth(ratio);

            spritebatchDrawer.worldPosition = npc.oldPos[i] + npc.Size * 0.5f;
            spritebatchDrawer.color = afterImageColor;
            spritebatchDrawer.scale = Vector2.One * afterImageScale;
            spritebatchDrawer.rotation = npc.oldRot[i];
            spriteBatch.Draw(spritebatchDrawer);
        }
    }
    public static void DrawBasicAfterImage(SpriteBatch spriteBatch, Projectile projectile, GetTrailColor getTrailColor, GetTrailWidth getTrailWidth, SpritebatchDrawer spritebatchDrawer)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

        //Create an after image effect
        //Gonna extract this to a function
        for (int i = 0; i < projectile.oldPos.Length; i++)
        {
            float ratio = (float)i / (float)projectile.oldPos.Length;
            Color afterImageColor = getTrailColor(ratio);
            float afterImageScale = getTrailWidth(ratio);

            spritebatchDrawer.worldPosition = projectile.oldPos[i] + projectile.Size * 0.5f;
            spritebatchDrawer.color = afterImageColor;
            spritebatchDrawer.scale = Vector2.One * afterImageScale;
            spritebatchDrawer.rotation = projectile.oldRot[i];
            spriteBatch.Draw(spritebatchDrawer);
        }
    }
} /// <summary>
  /// Helper struct for using the spritebatch to draw things
  /// </summary>
public struct SpritebatchDrawer
{
    public Texture2D texture;
    public Vector2 worldPosition;
    public Rectangle? dstRect;
    public Rectangle? sourceRect;
    public Color color;
    public float rotation;
    public Vector2 drawOrigin;
    public SpriteEffects spriteEffects;
    public Vector2 scale;
    public bool blackIsTransparency;
    public void LeftCenterOrigin()
    {
        Vector2 normalizedOrigin = new Vector2(0f, 0.5f);
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width, rectangle.Height) * normalizedOrigin;
        }
        else
        {
            drawOrigin = new Vector2(texture.Width, texture.Height) * normalizedOrigin;
        }
    }

    public void BottomLeftOrigin()
    {
        Vector2 normalizedOrigin = new Vector2(0f, 1f);
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width, rectangle.Height) * normalizedOrigin;
        }
        else
        {
            drawOrigin = new Vector2(texture.Width, texture.Height) * normalizedOrigin;
        }
    }
    public void RightCenterOrigin()
    {
        Vector2 normalizedOrigin = new Vector2(1f, 0.5f);
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width, rectangle.Height) * normalizedOrigin;
        }
        else
        {
            drawOrigin = new Vector2(texture.Width, texture.Height) * normalizedOrigin;
        }
    }
    public void BottomCenterOrigin()
    {
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width * 0.5f, rectangle.Height);
        }
        else
        {
            drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height);
        }
    }
    public void TopCenterOrigin()
    {
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width * 0.5f, 0);
        }
        else
        {
            drawOrigin = new Vector2(texture.Width * 0.5f, 0);
        }
    }
    public void CenterOrigin()
    {
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width * 0.5f, rectangle.Height * 0.5f);
        }
        else
        {
            drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
        }
    }
    public static SpritebatchDrawer FromTextureAsset(Asset<Texture2D> textureAsset, Vector2 worldPosition)
    {
        SpritebatchDrawer spritebatchDrawer = new SpritebatchDrawer();
        spritebatchDrawer.texture = textureAsset.Value;
        spritebatchDrawer.worldPosition = worldPosition;
        spritebatchDrawer.sourceRect = null;
        spritebatchDrawer.color = Color.White.MultiplyRGB(Lighting.GetColor(worldPosition.ToTileCoordinates()));
        spritebatchDrawer.rotation = 0;
        spritebatchDrawer.drawOrigin = textureAsset.Size() * 0.5f;
        spritebatchDrawer.spriteEffects = SpriteEffects.None;
        spritebatchDrawer.scale = Vector2.One;
        return spritebatchDrawer;
    }
    public static SpritebatchDrawer FromTextureAsset(Texture2D textureAsset, Vector2 worldPosition)
    {
        SpritebatchDrawer spritebatchDrawer = new SpritebatchDrawer();
        spritebatchDrawer.texture = textureAsset;
        spritebatchDrawer.worldPosition = worldPosition;
        spritebatchDrawer.sourceRect = null;
        spritebatchDrawer.color = Color.White.MultiplyRGB(Lighting.GetColor(worldPosition.ToTileCoordinates()));
        spritebatchDrawer.rotation = 0;
        spritebatchDrawer.drawOrigin = textureAsset.Size() * 0.5f;
        spritebatchDrawer.spriteEffects = SpriteEffects.None;
        spritebatchDrawer.scale = Vector2.One;
        return spritebatchDrawer;
    }


    public static SpritebatchDrawer FromProjectile(Projectile projectile)
    {
        SpritebatchDrawer spritebatchDrawer = new SpritebatchDrawer();
        spritebatchDrawer.texture = TextureAssets.Projectile[projectile.type].Value;
        spritebatchDrawer.worldPosition = projectile.Center;
        spritebatchDrawer.sourceRect = projectile.Frame();
        spritebatchDrawer.color = Color.White.MultiplyRGB(Lighting.GetColor(projectile.position.ToTileCoordinates()));
        spritebatchDrawer.rotation = projectile.rotation;
        spritebatchDrawer.drawOrigin = spritebatchDrawer.sourceRect.Value.Size() * 0.5f;
        spritebatchDrawer.spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spritebatchDrawer.scale = Vector2.One * projectile.scale;
        return spritebatchDrawer;
    }


    public static SpritebatchDrawer FromNPC(NPC npc)
    {
        SpritebatchDrawer spritebatchDrawer = new SpritebatchDrawer();
        spritebatchDrawer.texture = TextureAssets.Npc[npc.type].Value;
        spritebatchDrawer.worldPosition = npc.Center;
        spritebatchDrawer.sourceRect = npc.frame;
        spritebatchDrawer.color = Color.White.MultiplyRGB(Lighting.GetColor(npc.position.ToTileCoordinates()));
        spritebatchDrawer.rotation = npc.rotation;
        spritebatchDrawer.drawOrigin = spritebatchDrawer.sourceRect.Value.Size() * 0.5f;
        spritebatchDrawer.spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spritebatchDrawer.scale = Vector2.One * npc.scale;
        return spritebatchDrawer;
    }
}