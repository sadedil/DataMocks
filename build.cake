#tool "nuget:?package=GitReleaseNotes"
#tool nuget:?package=GitVersion.CommandLine

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var myProj = "./DataMocks/DataMocks.csproj";
var outputDir = "./artifacts/";
var isAppVeyor = BuildSystem.IsRunningOnAppVeyor;
var isWindows = IsRunningOnWindows();

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore("./DataMocks.sln", new DotNetCoreRestoreSettings{
            Verbosity = DotNetCoreVerbosity.Minimal,
        });
    });

GitVersion versionInfo = null;
DotNetCoreMSBuildSettings msBuildSettings = null;

Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
        
        msBuildSettings = new DotNetCoreMSBuildSettings()
                            .WithProperty("Version", versionInfo.NuGetVersion)
                            .WithProperty("AssemblyVersion", versionInfo.AssemblySemVer)
                            .WithProperty("FileVersion", versionInfo.AssemblySemVer);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild("./DataMocks.sln", new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
            MSBuildSettings = msBuildSettings
        });
    });

// Task test suspended temporarily, because of appveyor problem. (It searches in wrong folder)
// Task("Test")
//     .IsDependentOn("Build")
//     .Does(() => {
//         DotNetCoreTest("./DataMocks.Test/DataMocks.Test.csproj",
// 		new DotNetCoreTestSettings()
// 		{
// 			Configuration = configuration,
// 			NoBuild = true
// 		});
//    });

Task("Package")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCorePack(myProj, new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = outputDir,
            MSBuildSettings = msBuildSettings
        });

        if (!isWindows) return;

        // TODO not sure why this isn't working
        // GitReleaseNotes("outputDir/releasenotes.md", new GitReleaseNotesSettings {
        //     WorkingDirectory         = ".",
        //     AllTags                  = false
        // });

        // var releaseNotesExitCode = StartProcess(
        //     @"tools\gitreleasenotes\GitReleaseNotes\tools\gitreleasenotes.exe", 
        //     new ProcessSettings { Arguments = ". /o artifacts/releasenotes.md" });
        // if (string.IsNullOrEmpty(System.IO.File.ReadAllText("./artifacts/releasenotes.md")))
        //     System.IO.File.WriteAllText("./artifacts/releasenotes.md", "No issues closed since last release");
		// 
        // if (releaseNotesExitCode != 0) throw new Exception("Failed to generate release notes");
		// 
        // System.IO.File.WriteAllLines(outputDir + "artifacts", new[]{
        //     "nuget:DataMocks." + versionInfo.NuGetVersion + ".nupkg",
        //     "nugetSymbols:DataMocks." + versionInfo.NuGetVersion + ".symbols.nupkg",
        //     "releaseNotes:releasenotes.md"
        // });
		// 
        // if (isAppVeyor)
        // {
        //     foreach (var file in GetFiles(outputDir + "**/*"))
        //         AppVeyor.UploadArtifact(file.FullPath);
        // }
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);