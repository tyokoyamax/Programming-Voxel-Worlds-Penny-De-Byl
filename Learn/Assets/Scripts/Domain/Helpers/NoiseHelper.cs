using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Domain.Helpers
{
    public static class NoiseHelper
    {
        static int maxHeight = 100;
        static float roughness = 0.01f;
        static int octaves = 4;
        static float persistence = 0.5f;

        public static int GenerateStoneHeight(float x, float z, int seed)
        {
            float height = ScaleTo(
                newMin: 0f,
                newMax: maxHeight - 5,
                origMin: 0f,
                origMax: 1f,
                value: fBM(x * roughness * 2, z * roughness * 2, octaves + 1, persistence, seed));

            return (int)height;
        }

        public static int GenerateHeight(float x, float z, int seed)
        {
            float height = ScaleTo(
                 newMin: 0f,
                 newMax: maxHeight,
                 origMin: 0f,
                 origMax: 1f,
                 value: fBM(x * roughness, z * roughness, octaves, persistence, seed));

            return (int)height;
        }

        public static float ScaleTo(float newMin, float newMax, float origMin, float origMax, float value)
        {
            return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(origMin, origMax, value));
        }

        static float fBM(float x, float z, int octaves, float persistence, int seed)
        {
            float total = 0f;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;
            for (int i = 0; i < octaves; i++)
            {
                total += Mathf.PerlinNoise(x * frequency + seed, z * frequency + seed) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2f;

                seed -= seed / 2;
            }

            return total / maxValue;
        }
    }
}
