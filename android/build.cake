
#addin nuget:?package=Cake.FileHelpers&version=3.2.1 
#addin nuget:?package=Xamarin.FixAars&version=1.0.0
#tool "nuget:?package=nuget.commandline&version=5.3.0"

#load "main-components.cake";
#load "dependencies.cake";
#load "common.cake";

using AarLibsFix;

var TARGET = Argument ("target", Argument ("t", "ci"));
var REPO_URL="https://developer.huawei.com/repo/com/huawei/";  
var COMPONENT = Argument ("component", "");



Task ("clean")
	.Does (() =>
{
    if (DirectoryExists ("./externals"))
		DeleteDirectory ("./externals", new DeleteDirectorySettings { Recursive = true, Force = true });

	if (DirectoryExists ("./output"))
		DeleteDirectory ("./output", new DeleteDirectorySettings { Recursive = true, Force = true });

	CleanDirectories ("./**/packages");
	CleanDirectories("./**/bin");
	CleanDirectories("./**/obj");
});

Task ("build")
	.IsDependentOn("externals")
	.IsDependentOn("ci-setup")
	.Does(() =>
{
	Information ("----------------Build started---------------");
	MSBuild($"./source/HmsCoreXamarinBindings.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Targets.Clear();
		c.Targets.Add("Clean");
		c.Targets.Add("Build");
	});
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


Task ("externals")
	.Does (() => 
{
	EnsureDirectoryExists ("./externals");
	Information ("////////////////////////////////////////");
    Information ("// Downloading Libraries              //");
	Information ("////////////////////////////////////////");
		 
    DownloadExternals(DEPENDENCY_ARTIFACTS);
    DownloadExternals(MAIN_ARTIFACTS);
});



Task("nuget")
    .IsDependentOn("build")
	 .Does(() =>
{
	
	EnsureDirectoryExists("./output"); 
	Package(DEPENDENCY_ARTIFACTS);
	Package(MAIN_ARTIFACTS);
});

Task("ci-setup") 
	.Does(() =>
{
  SetMainArtifacts();
  SetDependencyArtifacts();
});


//Workaround for duplicated r-classes.jar files. Use Xamarin.AarLibFix from Baget.
//R.txt fix
Task("fix-duplicates") 
	.Does(() =>
{ 
  Information("-----------fix-duplicates------------");
  var ToFixClass = MAIN_ARTIFACTS.Where(a=>a.ArtifactId=="drive" ||
   a.ArtifactId=="nearby"||
    a.ArtifactId=="maps").ToList();

	 var ToFixResource = MAIN_ARTIFACTS.Where(a=>
	 a.ArtifactId=="ml-computer-vision-face-feature-model"||
   a.ArtifactId=="ml-computer-vision-face-3d-model"||
    a.ArtifactId=="ml-computer-vision-face-shape-point-model"||
    a.ArtifactId=="ml-computer-vision-face-emotion-model").ToList();

  if(ToFixClass.Count()>0)
  foreach(var art in ToFixClass)
    AndroidAarFixups.FixupAarClass(System.IO.Path.Combine("","externals", art.ArtifactId + "." + art.Extension),art.ArtifactId);

 if(ToFixResource.Count()>0)
	foreach(var art in ToFixResource)
    AndroidAarFixups.FixupAarResource(System.IO.Path.Combine("","externals", art.ArtifactId + "." + art.Extension),art.ArtifactId);
});


Task("ci")
	.IsDependentOn("ci-setup")
	.IsDependentOn("externals") 
	.IsDependentOn("fix-duplicates")
    .IsDependentOn("nuget")
    .IsDependentOn("samples");


RunTarget (TARGET);