using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    public static class HotFixDllUtil
    {
        /// <summary>
        ///  assemblies
        /// </summary>
        private static Type[] assemblies;

        public static Type[] GetHotfixTypes()
        {
            if (assemblies == null)
            {
                assemblies = GameModule.ILRuntime.GetHotfixTypes;
            }

            return assemblies;
        }
    }
}