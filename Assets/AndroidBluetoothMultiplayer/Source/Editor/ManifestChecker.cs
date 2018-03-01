#if UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace LostPolygon.AndroidBluetoothMultiplayer.Editor {
    [InitializeOnLoad]
    internal class ManifestChecker {
        static ManifestChecker() {
            EditorApplication.playmodeStateChanged += GenerateManifestIfAbsent;
            GenerateManifestIfAbsent();
        }

        [PostProcessScene]
        private static void GenerateManifestIfAbsent() {
            if (ManifestGenerator.IsManifestFileExists()) {
                ManifestGenerator.PatchManifest();
                return;
            }

            ManifestGenerator.GenerateManifest();
            Debug.Log("AndroidManifest.xml was missing, generated new");
        }
    }
}

#endif