//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HydraulicErosion : MonoBehaviour
//{

//    int size;

//    float[,] elevation;
//    float[,] water;
//    float[,] material;
//    float[,] previousMaterial;

//    float[,,] waterFlow;
//    float[,] totalFlow;
//    Vector2[,] waterVelocity;

//    public Vector3[] dropPositions;

//    public float rainfall = 1;

//    public void setValues()
//    {

//    }

//    public void rain(int x, int y)
//    {
//        water[x, y] += rainfall;
//    }

//    public void calculateFlow(int x, int y)
//    {
//        Vector3[] dirns = new Vector3[4];
//        dirns[0] = new Vector3(0, 1, 0);
//        dirns[1] = new Vector3(1, 0, 1);
//        dirns[2] = new Vector3(2, 0, -1);
//        dirns[3] = new Vector3(3, -1, 0);

//        totalFlow[x, y] = 0;
//        for(int i = 0; i < dirns.Length; i++) {
//            //float[,] dh = elevation[x, y] - elevation[x, y];
//        }
//    }

//    private void OnDrawGizmos()
//    {

//    }
//}
