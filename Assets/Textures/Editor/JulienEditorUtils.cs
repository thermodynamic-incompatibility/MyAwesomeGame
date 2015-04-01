using UnityEditor;
using UnityEngine;



class JulienEditorUtils : EditorWindow
{
    [MenuItem("Utils/Combine Textures")]
    static void combineTexturesMenu()
    {
        EditorWindow.GetWindow(typeof(JulienEditorUtils));
    }

    Texture2D RGBTexture;
    Texture2D alphaTexture;

    bool alphaFromGreyscale = true;

    string outputName = "Combined Texture";

    void OnGUI()
    {
        RGBTexture = (Texture2D)EditorGUILayout.ObjectField("RGB Texture", RGBTexture, typeof(Texture2D), true);
        
        alphaTexture = (Texture2D)EditorGUILayout.ObjectField("Alpha Texture", alphaTexture, typeof(Texture2D), true);

        alphaFromGreyscale = EditorGUILayout.Toggle("Alpha from Greyscale", alphaFromGreyscale, GUILayout.ExpandWidth(true));

        outputName = EditorGUILayout.TextField("Output name: ", outputName);

        if (GUILayout.Button("Combine"))
        {
            Debug.Log("Combining the textures.");
            combineTextures();
        }
    }

    void combineTextures()
    {
        //test the texture sizes - for simplicity, require the sizes to match
        if (RGBTexture.width != alphaTexture.width || RGBTexture.height != alphaTexture.height)
        {
            Debug.LogError("Couldn't combine the textures - textures must have the same dimensions to be combined.");
            return;
        }

        //get handles for the textures
        string rgbPath = AssetDatabase.GetAssetPath(RGBTexture.GetInstanceID());
        string alphaPath = AssetDatabase.GetAssetPath(alphaTexture.GetInstanceID());

        //check to see if the textures are readable
        TextureImporter rgbTextureImporter = AssetImporter.GetAtPath(rgbPath) as TextureImporter;
        TextureImporterSettings rgbSettings = new TextureImporterSettings();
        rgbTextureImporter.ReadTextureSettings(rgbSettings);
        TextureImporter alphaTextureImporter = AssetImporter.GetAtPath(alphaPath) as TextureImporter;
        TextureImporterSettings alphaSettings = new TextureImporterSettings();
        alphaTextureImporter.ReadTextureSettings(alphaSettings);
        if (!rgbSettings.readable || !alphaSettings.readable)
        {
            Debug.LogError("To combine textures, they must be readable.  In the import settings, change the texture type to 'Advanced' and choose 'Read/Write Enabled'.  Then Apply the new settings.");
            return;
        }

        //Get the output path
        Texture2D outputTexture = new Texture2D(RGBTexture.width, RGBTexture.height);
        string path = Application.dataPath + "/" + outputName + ".png";

        //combine the textures
        Color32[] outputColors = RGBTexture.GetPixels32();
        Color32[] alphaColors = alphaTexture.GetPixels32();
        for (int i = 0; i < outputColors.Length; i++)
        {
            if (alphaFromGreyscale)
            {
                //calculate the luminosity and save as alpha
                outputColors[i].a = (byte)((alphaColors[i].r + alphaColors[i].g + alphaColors[i].b) / 3);
            }
            else
                outputColors[i].a = alphaColors[i].a;
        }
        outputTexture.SetPixels32(outputColors);

        //Save out the new texture
        byte[] bytes = outputTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);

        AssetDatabase.ImportAsset("Assets/" + outputName + ".png", ImportAssetOptions.ForceUpdate);
    }
}