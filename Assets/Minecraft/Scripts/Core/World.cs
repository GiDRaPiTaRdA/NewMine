using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class World
    {
        public static int chunkSize = 16;

        public static int ConvertBlockIndexToLocal(int i)
        {
            if (i == -1)
                i = World.chunkSize - 1;
            else if (i == World.chunkSize)
                i = 0;
            return i;
        }
    }
}
