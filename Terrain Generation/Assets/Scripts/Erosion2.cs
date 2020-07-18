using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion2 : MonoBehaviour
{
    [Range(1, 16)]
    public int radius = 3; // Radius of the erosion droplets, also causes spikes at 1
    [Range(0, 1)]
    public float inertia = .05f; // Closer to 0 will increase the rate at which is travels to a lower layer, at 1 it will not
    [Range(0, 20)]
    public float sedimentMultiplier = 4; // Multiplier for how much sediment a droplet can carry
    [Range(-1, 1)]
    public float minSediment = .01f; // Stops carrying capacity going lower than 0, causes the spike issues
    [Range(0, 1)]
    public float erosionSpeed = .3f;
    [Range(0, 1)]
    public float depositSpeed = .3f;
    [Range(0, 1)]
    public float evaporateSpeed = .01f;
    [Range(0, 20)]
    public float gravity = 4;
    [Range(0, 200)]
    public int dropletLifetime = 30;

    [Range(0, 5)]
    public float waterVolume = 1;
    [Range(0, 5)]
    public float speed = 1;

    // Weights of erosion for each node/vertex of the map
    int[][] erosionBrushIndices;
    float[][] erosionBrushWeights;
    System.Random prng;

    int currentErosionRadius;
    int currentMapSize;
    float[] currentMap;

    // Creates random value for use in rain fall, Sets values for map, map size and erosion radius
    void Initialize(int mapSize, float[] newMap)
    {
        if (prng == null)
        {
            prng = new System.Random();
        }

        if (erosionBrushIndices == null || currentErosionRadius != radius || currentMapSize != mapSize || currentMap != newMap)
        {
            currentMap = newMap;
            currentErosionRadius = radius;
            currentMapSize = mapSize;
            InitializeBrushIndices();
        }
    }

    // Creates rain based on iterations set
    public void Erode(float[] map, int mapSize, int numIterations = 1)
    {
        Initialize(mapSize, map);
        for (int iteration = 0; iteration < numIterations; iteration++)
        {
            //Create random droplet on map
            Droplet newDrop = CalculateDroplet();
            RunDropletCycle(newDrop);
        }
    }

    // Manages direction and actions of droplet such as direction, lifetime and sediment
    private void RunDropletCycle(Droplet newDrop)
    {
        for (int lifetime = 0; lifetime < dropletLifetime; lifetime++)
        {
            int nodeX = (int)newDrop.posX;
            int nodeY = (int)newDrop.posY;
            int dropletIndex = nodeY * currentMapSize + nodeX;
            float cellOffsetX = newDrop.posX - nodeX;
            float cellOffsetY = newDrop.posY - nodeY;

            // Gets Droplet's height and direction
            HeightAndGradient heightAndGradient = CalculateHeightAndGradient(newDrop.posX, newDrop.posY);

            // Update droplet
            newDrop.directionX = (newDrop.directionX * inertia - heightAndGradient.gradientX * (1 - inertia));
            newDrop.directionY = (newDrop.directionY * inertia - heightAndGradient.gradientY * (1 - inertia));

            float len = Mathf.Sqrt(newDrop.directionX * newDrop.directionX + newDrop.directionY * newDrop.directionY);
            if (len != 0)
            {
                newDrop.directionX /= len;
                newDrop.directionY /= len;
            }
            newDrop.posX += newDrop.directionX;
            newDrop.posY += newDrop.directionY;

            // Check if droplet is on map
            if ((newDrop.directionX == 0 && newDrop.directionY == 0) || newDrop.posX < 0 || newDrop.posX >= currentMapSize - 1 || newDrop.posY < 0 || newDrop.posY >= currentMapSize - 1)
            {
                break;
            }
            CalculateSediment(newDrop, heightAndGradient, dropletIndex, cellOffsetX, cellOffsetY);
        }
    }

    // Calculate sediment capacity based on amount of water and height
    private void CalculateSediment(Droplet newDrop, HeightAndGradient heightAndGradient, int dropletIndex, float cellOffsetX, float cellOffsetY)
    {
        float newHeight = CalculateHeightAndGradient(newDrop.posX, newDrop.posY).height;
        float deltaHeight = newHeight - heightAndGradient.height;
        float sedimentCapacity = Mathf.Max(-deltaHeight * newDrop.newSpeed * newDrop.currentWater * sedimentMultiplier, minSediment); //Combined with minSediment to stop terrain spikes

        if (newDrop.sediment > sedimentCapacity || deltaHeight > 0)
        {
            DropSediment(deltaHeight, sedimentCapacity, dropletIndex, cellOffsetX, cellOffsetY, newDrop);
        }
        else
        {
            float amountToErode = Mathf.Min((sedimentCapacity - newDrop.sediment) * erosionSpeed, -deltaHeight);
            for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[dropletIndex].Length; brushPointIndex++)
            {
                ErodeArea(dropletIndex, brushPointIndex, amountToErode, newDrop);
            }
        }
        newDrop.newSpeed = Mathf.Sqrt(newDrop.newSpeed * newDrop.newSpeed + deltaHeight * gravity);
        newDrop.currentWater *= (1 - evaporateSpeed);
    }

    // Drop/Deposits sediment in droplet
    private void DropSediment(float deltaHeight, float sedimentCapacity, int dropletIndex, float cellOffsetX, float cellOffsetY, Droplet newDrop)
    {
        // Drops sediment if moving up hill
        float amountToDeposit = (deltaHeight > 0) ? Mathf.Min(deltaHeight, newDrop.sediment) : (newDrop.sediment - sedimentCapacity) * depositSpeed;
        newDrop.sediment -= amountToDeposit;

        // Drops sediment around droplet
        currentMap[dropletIndex] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
        currentMap[dropletIndex + 1] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
        currentMap[dropletIndex + currentMapSize] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
        currentMap[dropletIndex + currentMapSize + 1] += amountToDeposit * cellOffsetX * cellOffsetY;
    }

    // Erodes area and set sediment
    private void ErodeArea(int dropletIndex, int brushPointIndex, float amountToErode, Droplet newDrop)
    {
        int nodeIndex = erosionBrushIndices[dropletIndex][brushPointIndex];
        float weighedErodeAmount = amountToErode * erosionBrushWeights[dropletIndex][brushPointIndex];
        float deltaSediment = (currentMap[nodeIndex] < weighedErodeAmount) ? currentMap[nodeIndex] : weighedErodeAmount;
        currentMap[nodeIndex] -= deltaSediment;
        newDrop.sediment += deltaSediment;
    }

    // Sets brush settings
    void InitializeBrushIndices()
    {
        erosionBrushIndices = new int[currentMapSize * currentMapSize][];
        erosionBrushWeights = new float[currentMapSize * currentMapSize][];

        int[] xOffsets = new int[radius * radius * 4];
        int[] yOffsets = new int[radius * radius * 4];
        float[] weights = new float[radius * radius * 4];
        float weightSum = 0;
        int addIndex = 0;

        for (int i = 0; i < erosionBrushIndices.GetLength(0); i++)
        {
            int centreX = i % currentMapSize;
            int centreY = i / currentMapSize;

            if (centreY <= radius || centreY >= currentMapSize - radius || centreX <= radius + 1 || centreX >= currentMapSize - radius)
            {
                weightSum = 0;
                addIndex = 0;
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        float sqrDst = x * x + y * y;
                        if (sqrDst < radius * radius)
                        {
                            int coordX = centreX + x;
                            int coordY = centreY + y;

                            if (coordX >= 0 && coordX < currentMapSize && coordY >= 0 && coordY < currentMapSize)
                            {
                                float weight = 1 - Mathf.Sqrt(sqrDst) / radius;
                                weightSum += weight;
                                weights[addIndex] = weight;
                                xOffsets[addIndex] = x;
                                yOffsets[addIndex] = y;
                                addIndex++;
                            }
                        }
                    }
                }
            }

            int numEntries = addIndex;
            erosionBrushIndices[i] = new int[numEntries];
            erosionBrushWeights[i] = new float[numEntries];

            for (int j = 0; j < numEntries; j++)
            {
                erosionBrushIndices[i][j] = (yOffsets[j] + centreY) * currentMapSize + xOffsets[j] + centreX;
                erosionBrushWeights[i][j] = weights[j] / weightSum;
            }
        }
    }

    // Sets HeightAndGradient struct values
    HeightAndGradient CalculateHeightAndGradient(float posX, float posY)
    {
        int coordX = (int)posX;
        int coordY = (int)posY;

        //Offsets
        float x = posX - coordX;
        float y = posY - coordY;

        int nodeIndex = coordY * currentMapSize + coordX;
        float heightTL = currentMap[nodeIndex]; // Top Left
        float heightTR = currentMap[nodeIndex + 1];
        float heightBL = currentMap[nodeIndex + currentMapSize];
        float heightBR = currentMap[nodeIndex + currentMapSize + 1]; // Bottom Right

        // Get direction
        float gradientX = (heightTR - heightTL) * (1 - y) + (heightBR - heightBL) * y;
        float gradientY = (heightBL - heightTL) * (1 - x) + (heightBR - heightTR) * x;

        float height = heightTL * (1 - x) * (1 - y) + heightTR * x * (1 - y) + heightBL * (1 - x) * y + heightBR * x * y;
       //float height = heightTL * (1 - x) * (1 - y) + heightBL * (1 - x); ///////////////////////////////////////////////////REMOVE

        return new HeightAndGradient() { height = height, gradientX = gradientX, gradientY = gradientY };
    }

    //Sets droplet initital settings
    Droplet CalculateDroplet()
    {
        float posX = prng.Next(0, currentMapSize - 1);
        float posY = prng.Next(0, currentMapSize - 1);
        float directionX = 0;
        float directionY = 0;
        float newSpeed = speed;
        float currentWater = waterVolume;
        float sediment = 0;
        return new Droplet() { posX = posX, posY = posY, directionX = directionX, directionY = directionY, newSpeed = newSpeed, currentWater = currentWater, sediment = sediment };
    }

    struct HeightAndGradient
    {
        public float height;
        public float gradientX;
        public float gradientY;
    }

    struct Droplet
    {
        public float posX;
        public float posY;
        public float directionX ;
        public float directionY;
        public float newSpeed;
        public float currentWater;
        public float sediment;
    }
}
