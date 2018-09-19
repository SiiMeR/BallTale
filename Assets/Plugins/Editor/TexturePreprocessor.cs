using UnityEditor;
using UnityEngine;

// Sets the correct fields for pixel art style sprites so it doesn't get filtered in any way
public class TexturePreprocessor : AssetPostprocessor
{
    
    
    private void OnPreprocessTexture()
    {    
        var textureImporter = (TextureImporter) assetImporter;

        var asset = AssetDatabase.LoadAssetAtPath(textureImporter.assetPath, typeof(Texture2D));
        
        // if not in asset database then it's a new asset and needs processing
        if (!asset && textureImporter.textureType == TextureImporterType.Sprite)
        {
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

            textureImporter.filterMode = FilterMode.Point;

            textureImporter.spritePixelsPerUnit = 64;
        }
    }
}