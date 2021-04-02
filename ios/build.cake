
#load "common.cake"
#load "poco.cake"
#load "components.cake"

var TARGET = Argument ("t", Argument ("target", "ci"));

var SOURCE_PATH= System.IO.Path.Combine("","source"); 
var EXTERNALS_PATH = new DirectoryPath ("./externals");
var OUTPUT_PATH=System.IO.Path.Combine("","output");
var NUSPEC_PATH=System.IO.Path.Combine("","nuspecs"); 


Information ($"SOURCE_PATH            : {SOURCE_PATH}");
Information ($"EXTERNALS_PATH         : {EXTERNALS_PATH}");
Information ($"OUTPUT_PATH     : {OUTPUT_PATH}");
Information ($"NUSPEC_PATH       : {NUSPEC_PATH}"); 

var IS_LOCAL_BUILD = true;
var BACKSLASH = string.Empty;
  

var ARTIFACTS_TO_BUILD = new List<Artifact> ();

var SOURCES_TARGETS = new List<string> (); 

Setup (context =>
{
	IS_LOCAL_BUILD = string.IsNullOrWhiteSpace (EnvironmentVariable ("AGENT_ID"));
	Information ($"Is a local build? {IS_LOCAL_BUILD}");
	BACKSLASH = IS_LOCAL_BUILD ? @"\\" : @"\";
});

Task("prepare-artifacts")
	.Does(() =>
{
	
	SetArtifactsDependencies ();
	SetArtifactsPodSpecs ();
	 var orderedArtifactsForBuild = new List<Artifact> (); 
	 foreach (var artifact in ARTIFACTS) {
     orderedArtifactsForBuild.Add (artifact.Value);
	 AddArtifactDependencies (orderedArtifactsForBuild, artifact.Value.Dependencies); 
	 }
	 orderedArtifactsForBuild = orderedArtifactsForBuild.Distinct ().ToList (); 
	 orderedArtifactsForBuild.Sort ((f, s) => s.BuildOrder.CompareTo (f.BuildOrder)); 
	 ARTIFACTS_TO_BUILD.AddRange (orderedArtifactsForBuild);
	 Information ("Build order:");
	 foreach (var artifact in ARTIFACTS_TO_BUILD) {
		SOURCES_TARGETS.Add(artifact.CsprojName.Replace ('.', '_'));
		Information (artifact.ArtifactId);
	}
});

Task ("externals")
	.Does (() => 
{
	EnsureDirectoryExists (EXTERNALS_PATH);
	Information ("Updating Cocoapods repo...");
	CocoaPodRepoUpdate ();

	foreach (var artifact in ARTIFACTS_TO_BUILD) {

	   foreach (var podSpec in artifact.PodSpecs) {
			if (podSpec.FrameworkSource != FrameworkSource.Pods)
				continue;
				
			if (DirectoryExists (EXTERNALS_PATH.Combine ($"{podSpec.FrameworkName}.framework")))
				break;

				CreateAndInstallPodfile (artifact);
				BuildSdkOnPodfile (artifact);
			}
		}
});

Task ("samples")
	.Does(() =>
{
	var samples = System.IO.Directory.GetFiles("./samples","*.sln",SearchOption.AllDirectories);
	foreach (var sampleSln in samples) {
		MSBuild(sampleSln, c => 
	 		c.SetConfiguration("Release")
 			.WithTarget("Restore"));

		MSBuild(sampleSln, c => 
			c.SetConfiguration("Release")
			.WithTarget("Build")
			.WithProperty("DesignTimeBuild", "false"));
	}
});

Task ("build")
	.Does(() =>
{
	MSBuild(SLN_PATH, c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Targets.Clear();
		c.Targets.Add("Clean");
		c.Targets.Add("Build");
	}); 
});


Task("nuget")
    .IsDependentOn("build") 
	.Does(() =>
{
	EnsureDirectoryExists("./output"); 

	foreach (var art in ARTIFACTS) {
		
	   var csproj =  System.IO.Path.Combine("","source", art.Key , art.Key+".csproj");
       var nuspec = GetNuspecName(art.Value);
	   
		MSBuild(csproj.ToString(), c => 
			c.SetConfiguration("Release")
			.WithProperty("PackageOutputPath", MakeAbsolute((DirectoryPath)"./output/").FullPath)
			.WithProperty("DesignTimeBuild", "false"));	
			 
			  NuGetRestore(csproj.ToString());

			   var configuration = Argument("configuration", "Release");

			   var dllPath =  System.IO.Path.Combine("source",art.Key,"bin",configuration , art.Key+".dll");
			   var rootDir =  System.IO.Path.Combine("source",art.Key,"bin",configuration);
                Information(dllPath);
			  var nugetPackageDir = Directory("./output");

			  var nuGetPackSettings = new NuGetPackSettings
              {   
                 BasePath = rootDir,
                 OutputDirectory = nugetPackageDir,  
              };

	        NuGetPack(nuspec, nuGetPackSettings); 
	}
});

Task ("clean")
	.Does (() => 
{
	var deleteDirectorySettings = new DeleteDirectorySettings {
		Recursive = true,
		Force = true
	};

	var bins = GetDirectories("./**/bin");
	DeleteDirectories (bins, deleteDirectorySettings);

	var objs = GetDirectories("./**/obj");
	DeleteDirectories (objs, deleteDirectorySettings);

	if (DirectoryExists ("./externals/"))
		DeleteDirectory ("./externals", deleteDirectorySettings);

	if (DirectoryExists ("./output/"))
    	DeleteDirectory ("./output", deleteDirectorySettings);
});

Task ("ci")
	.IsDependentOn("clean")
	.IsDependentOn("prepare-artifacts")
	.IsDependentOn("externals")
	.IsDependentOn("nuget")
	.IsDependentOn("samples");

Teardown (context =>
{
	var artifacts = GetFiles ("./output/**/*");

	if (artifacts?.Count () <= 0)
		return;

	Information ($"Found Packages ({artifacts.Count ()})");
});

RunTarget (TARGET);
