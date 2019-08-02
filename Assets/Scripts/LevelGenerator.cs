using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public Texture2D mapTexture;

    public ColorToPrefab[] colorMappings;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        for(int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.width; y++)
            {

                GenerateTile(x, y);

            }
        }
    }

    void GenerateTile(int x, int y)
    {
       Color pixelColor = mapTexture.GetPixel(x,y);
        if (pixelColor.a == 0)
        {
            //ignore empty pixel
            return;
        }

        foreach(ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x, y);
                Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
            }
        }

    }


}
