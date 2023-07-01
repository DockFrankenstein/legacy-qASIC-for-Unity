using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

namespace Project
{
    [CreateAssetMenu(fileName = "Package Builder", menuName = "Scriptable Object/Project/Package Builder")]
    public class PackageBuilder : ScriptableObject
    {
        public string generatedCollectionPath = "qASIC";
        public string generatedPackagePath = "qASIC";

        public string systemsRootPath = "qASIC Packages";
        public string persistentFilesPath = "Collection Persistent Files~";

        [FormerlySerializedAs("components")] public List<System> systems = new List<System>();

        [Serializable]
        public class System
        {
            public string name;
            public string path;
            [FormerlySerializedAs("componentDependencies")] public string[] systemDependencies;
        }
    }
}