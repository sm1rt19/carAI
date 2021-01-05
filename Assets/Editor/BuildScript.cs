using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Car AI/Build")]
    public static void BuildGame()
    {
        var buildDir = (Application.dataPath + "/../Builds").Replace("Assets/../", "");

        string[] levels =
        {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/UI.unity",
            "Assets/Scenes/Track1.unity",
            "Assets/Scenes/FixedSpeedPlaying.unity",
            "Assets/Scenes/FixedSpeedTraining.unity",
        };


        // Build player.
        BuildPipeline.BuildPlayer(levels, buildDir, BuildTarget.WebGL, BuildOptions.None);

        var folder = buildDir.Replace('/', '\\') + "\\Build";
        Process.Start("explorer.exe", folder);
    }
}
