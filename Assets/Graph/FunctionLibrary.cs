using UnityEngine;
using static UnityEngine.Mathf;

namespace Graph
{
    public static class FunctionLibrary {
        public delegate float Function (float x, float z, float t);

        public enum FunctionName {Wave, MultiWave, Ripple};
        private static Function[] functions = {Wave, MultiWave, Ripple};
        
        public static Function GetFunction (FunctionName name)
        {
            return functions[(int)name];
        }

        private static float Wave(float x, float z, float t)
        {
            return Sin(Mathf.PI * (x + z + t));
        }
        
        private static float MultiWave(float x, float z, float t)
        {
            var y = Sin(Mathf.PI * (x + 0.5f * t));
            y += 0.5f * Sin(2f * PI * (z + t));
            y += Sin(PI * (x + z + 0.25f * t));
            return y * (1f / 2.5f);
        }

        private static float Ripple(float x, float z, float t)
        {
            var d = Sqrt(x * x + z * z);
            var y = Sin(PI * (4f * d - t));
            return y / (1f + 10f * d);
        }
    }
}
