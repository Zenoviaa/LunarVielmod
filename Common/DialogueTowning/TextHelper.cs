using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Common.DialogueTowning;

public struct TextSnippet
{
    public Color characterColor;
    public Vector2 position;
    public Vector2 scale;
    public string text;
}

public static class TextHelper
{
    public static Vector2[] ShadowOffsets
    {
        get
        {
            if(field == null)
            {
                field = new Vector2[4];
                field[0] = -Vector2.UnitY * 2;
                field[1] = Vector2.UnitY * 2;
                field[2] = -Vector2.UnitX  * 2;
                field[3] = Vector2.UnitX * 2;
            }
            return field;
        }
    }
    public static void DrawStringIndividually(DynamicSpriteFont font, SpriteBatch spriteBatch, string text, Vector2 position, Vector2 baseScale, float lineSpacing = 8, int maxWidth = -1)
    {
        var snippets = ParseMessage(font, text, position, baseScale, lineSpacing, maxWidth);
        DrawStringIndividually(font, spriteBatch, snippets);
    }

    public static void DrawStringIndividually(DynamicSpriteFont font, SpriteBatch spriteBatch, TextSnippet[] snippets)
    {
        foreach (var snippet in snippets)
        {
            foreach (var offset in ShadowOffsets)
            {
                spriteBatch.DrawString(font, snippet.text, snippet.position + offset, Color.Black
                    * snippet.characterColor.A, 0, Vector2.Zero, snippet.scale, SpriteEffects.None, 0);
            }
        }
        foreach (var snippet in snippets)
        {
            spriteBatch.DrawString(font, snippet.text, snippet.position, snippet.characterColor, 0, Vector2.Zero, snippet.scale, SpriteEffects.None, 0);
        }
    }

    /// <summary>
    /// Parses a message into individual characters with proper text wrapping
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="position"></param>
    /// <param name="baseScale"></param>
    /// <param name="lineSpacing"></param>
    /// <param name="maxWidth"></param>
    /// <returns></returns>
    public static TextSnippet[] ParseMessage(DynamicSpriteFont font, string text, Vector2 position, Vector2 baseScale, float lineSpacing = 8, int maxWidth = -1)
    {
        List<TextSnippet> wordSnippets = new List<TextSnippet>();
        List<TextSnippet> charSnippets = new List<TextSnippet>();
        var x = font.MeasureString(" ").X * baseScale.X;
        //First we're going to split by full words
        //loop over and text wrap the ones that are too big
        //This should correctly wrap the words to be in the spots they should be in  
        var words = text.Split(' ');
        var currentWordPosition = position;
        var currentLineSize = 0F;
        for (int i = 0; i < words.Length; i++)
        {
            ref var word = ref words[i];
            var wordSize = font.MeasureString(word) * baseScale;
            var nextLineSize = currentLineSize + wordSize.X;
            if (maxWidth != -1 && nextLineSize > maxWidth)
            {
                //wrap
                currentWordPosition.Y += lineSpacing * baseScale.Y;
                currentWordPosition.X = position.X;
                currentLineSize = 0;
            }

            var snippet = new TextSnippet
            {
                characterColor = Color.White,
                position = currentWordPosition,
                text = word,
                scale = baseScale
            };
            wordSnippets.Add(snippet);

            currentWordPosition.X += wordSize.X;
            currentWordPosition.X += x;
            currentLineSize += wordSize.X;
        }

        //Now that the text is properly wrapped
        //We loop over every text snippet and loop over each char in the snippet
        foreach (var wordSnippet in wordSnippets)
        {
            for (var i = 0; i < wordSnippet.text.Length; i++)
            {
                var offset = Vector2.Zero;
                if(i != 0)
                {
                    var charSize = font.MeasureString(wordSnippet.text.Substring(0, i)) * baseScale;
                    offset.X += charSize.X;
                }

                var charPosition = wordSnippet.position + offset;
                var charColor = Color.White;
                charSnippets.Add(new()
                {
                    position = charPosition,
                    characterColor = Color.White,
                    text = wordSnippet.text[i].ToString(),
                    scale = baseScale
                });
            }
        }

        return charSnippets.ToArray();
    }
}
