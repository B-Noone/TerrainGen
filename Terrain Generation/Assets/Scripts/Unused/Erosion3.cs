using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion3 : MonoBehaviour
{
    [Range(2, 8)]
    public int erosionRadius = 3;
    [Range(0, 1)]
    public float inertia = .05f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 
    public float sedimentCapacityFactor = 4; // Multiplier for how much sediment a droplet can carry
    public float minSedimentCapacity = .01f; // Used to prevent carry capacity getting too close to zero on flatter terrain
    [Range(0, 1)]
    public float erodeSpeed = .3f;
    [Range(0, 1)]
    public float depositSpeed = .3f;
    [Range(0, 1)]
    public float evaporateSpeed = .01f;
    public float gravity = 4;
    public int maxDropletLifetime = 30;

    public float initialWaterVolume = 1;
    public float initialSpeed = 1;

    // Indices and weights of erosion brush precomputed for every node
    int[][] erosionBrushIndices;
    float[][] erosionBrushWeights;
    System.Random prng;

    int currentSeed;
    int currentErosionRadius;
    int currentMapSize;

    void Initialize(int mapSize)
    {
        prng = new System.Random();
    }

    public void Erode(float[,] map, int mapSize, int numIterations = 1)
    {
        //Random water drop
        for(int lt = 0; lt < numIterations; lt++)
        {
            //calc droplet height & direction. bilinear interpolation

            //update droplet position

            //Find droplet new hieght and calc deltaHeight

            //Calc sediment capacity

            //if too much deposit to surrounding nodes

            //else erode fraction of capacityt from soil

            //update speed + evaporate
        }
    }
}
