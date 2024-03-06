using System;

namespace Ford.WebApi.Data
{
    public struct Vector
    {
        public float x;
        public float y;
        public float z;
        public readonly double magnitude => Math.Sqrt(x * x + y * y + z * z);
    }
}
