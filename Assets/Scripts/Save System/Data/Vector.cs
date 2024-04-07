using System;

namespace Ford.SaveSystem.Data
{
    public struct Vector
    {
        public float x;
        public float y;
        public float z;
        public readonly double magnitude => Math.Sqrt(x * x + y * y + z * z);
    }
}
