using UnityEngine;

public static class Noise3
{

    public static float[] GenerateNoise(int mapSize, int seed, float initScale, float lacunarity, float gain, int numLayers)
    {
        var map = new float[mapSize * mapSize];
        var prng = new System.Random(seed);

        Vector2[] offsets = new Vector2[numLayers];
        for (int i = 0; i < numLayers; i++)
        {
            offsets[i] = new Vector2(prng.Next(-1000, 1000), prng.Next(-1000, 1000));
        }

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float noiseValue = 0;
                float scale = initScale;
                float weight = 1;
                for (int i = 0; i < numLayers; i++)
                {
                    Vector2 p = offsets[i] + new Vector2(x / (float)mapSize, y / (float)mapSize) * scale;
                    noiseValue += Mathf.PerlinNoise(p.x, p.y) * weight;
                    weight *= gain;
                    scale *= lacunarity;
                }
                map[y * mapSize + x] = noiseValue;
                minValue = Mathf.Min(noiseValue, minValue);
                maxValue = Mathf.Max(noiseValue, maxValue);
            }
        }

        // Normalize
        if (maxValue != minValue)
        {
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = (map[i] - minValue) / (maxValue - minValue);
            }
        }
        //for(int i = 0; i < map.Length; i++)
        //{
        //    Debug.Log(map[i]);
        //}
        return map;
    }
}