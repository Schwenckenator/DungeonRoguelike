using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public Texture2D mapTexture;

    public ColorToPrefab[] colorMappings;
    public float scaleFactor;

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
                //scale the positions down to game proportions
                Vector2 position = new Vector2(x/scaleFactor, y/scaleFactor);
                Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
            }
        }

    }


}
