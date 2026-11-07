using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class BuildSolarAR
{
    private const string ScenePath = "Assets/SolarAr/Scenes/MainAR.unity";
    private const string OutputPath = "/home/kaori/SolarAR/UnityApp/Builds/Android/SolarAR.apk";

    public static void BuildAndroidApk()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
        EditorUserBuildSettings.buildAppBundle = false;

        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"SolarAR Android build failed: {report.summary.result} " +
                $"({report.summary.totalErrors} errors)."
            );
        }
    }
}
