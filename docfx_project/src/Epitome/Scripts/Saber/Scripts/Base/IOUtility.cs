using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Saber.Base
{
    public static class IOUtility 
    {
        public static void CheckPath(string direPath)
        {
            if (!Directory.Exists(direPath))
            {
                Directory.CreateDirectory(direPath);
            }
        }
    }
}
