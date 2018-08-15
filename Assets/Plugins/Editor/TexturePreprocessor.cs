using UnityEditor;
using UnityEngine;

// Sets the correct fields for pixel art style sprites so it doesn't get filtered in any way
public class TexturePreprocessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        var textureImporter = (TextureImporter) assetImporter;

        if (textureImporter.textureType == TextureImporterType.Sprite)
        {
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

            textureImporter.filterMode = FilterMode.Point;

            textureImporter.spritePixelsPerUnit = 64;
        }
    }
}