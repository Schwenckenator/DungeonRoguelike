using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    public Texture2D mapTexture;

    public ColorToPrefab[] colorMappings;
    public float scalePositionFactor;

    void Start()
    {
        GenerateRoom();
    }

    void GenerateRoom()
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

        Debug.Log(pixelColor);
        foreach(ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                //scale the positions down to game proportions
                Vector2 position = new Vector2(x/scalePositionFactor, y/scalePositionFactor);
                Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
            }

        }

    }


}
