using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

	static int maxHeight = 150;
	static float smooth = 0.01f;
	static int octaves = 4;
	static float persistence = 0.5f;

	public static int GenerateStoneHeight(float x, float z){return GenerateHeight(x,z,10,1.2f,0); }

    public static int GenerateDirtHeight(float x, float z){  return GenerateHeight(x, z, 0, 0.7f, 0);}

    public static int GenerateHeight(float x, float z, int heightDelta, float multiple = 1, int octavesAdd = 0)
    {
        float height = Map(0,maxHeight- heightDelta, 0, 1, fBM(x*smooth*multiple,z*smooth*multiple,octaves+octavesAdd,persistence));
        return (int) height;
    }

    public static float fBM3D(float x, float y, float z, float smooth, int octaves)
    {
        float XY = fBM(x*smooth,y*smooth,octaves,0.5f);
        float YZ = fBM(y*smooth,z*smooth,octaves,0.5f);
        float XZ = fBM(x*smooth,z*smooth,octaves,0.5f);

        float YX = fBM(y*smooth,x*smooth,octaves,0.5f);
        float ZY = fBM(z*smooth,y*smooth,octaves,0.5f);
        float ZX = fBM(z*smooth,x*smooth,octaves,0.5f);

        return (XY+YZ+XZ+YX+ZY+ZX)/6.0f;
    }

	static float Map(float newmin, float newmax, float origmin, float origmax, float value)
    {
        return Mathf.Lerp (newmin, newmax, Mathf.InverseLerp (origmin, origmax, value));
    }

    static float fBM(float x, float z, int oct, float pers)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for(int i = 0; i < oct ; i++) 
        {
                total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= pers;
                frequency *= 2;
        }

        return total/maxValue;
    }
}
