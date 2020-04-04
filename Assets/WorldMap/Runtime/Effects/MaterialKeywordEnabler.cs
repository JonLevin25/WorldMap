using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap.Effects
{
    [Serializable]
    public class MaterialKeywordEnabler
    {
        // Fix for emission/keywords not working in editor until playing with emission in material manually
        [SerializeField] private bool _enableEmission;
        [SerializeField] private bool _enableNormalMap;


        public IEnumerable<string> GetKeywords()
        {
            if (_enableEmission) yield return "_EMISSION";
            if (_enableNormalMap) yield return "_NORMALMAP";
        }
    }
}