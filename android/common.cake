using Cake.Common.IO;
using System.Xml;
using System;
using System.Linq;
 

 public static string FirstCharToUpper(this string input) =>
       input switch
       {
           null => throw new ArgumentNullException(nameof(input)),
           "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
           _ => input.First().ToString().ToUpperInvariant() + input.Substring(1)
       };

public static string SetName(this string text, char seperator = '.')
{
    string name = "";
    var splitted = text.Split('-');
    name = String.Concat(splitted.Select(a => a.FirstCharToUpper()).ToArray());
    return name;
}

 public static string SetGroupIdForMultiple(this string text, char seperator = '.')
{
    string name = "";
    var splitted = text.Split('.');
    name = String.Join(seperator, splitted.Select(a => a.FirstCharToUpper()).ToArray());
    return name;
}

public static string GetNuspecName(ArtifactInfo artifact)
{ 
	return System.IO.Path.Combine("nuspecs",$"Huawei.{artifact.GroupId.SetGroupIdForMultiple().FirstCharToUpper()}.{artifact.ArtifactId.SetName('-').FirstCharToUpper()}.{artifact.Version}.nuspec");
}


public  void Package(List<ArtifactInfo> artifacts){
    foreach (var art in artifacts)
    {
      var csproj =  System.IO.Path.Combine("source", art.ArtifactId , art.ArtifactId+".csproj");
       var nuspec = GetNuspecName(art);
      
      Information ("-------------PACKAGING------------------");
      Information ("-------"+art.ArtifactId+"------");
      Information ("----------------------------------------");
		MSBuild(csproj.ToString(), c => 
			c.SetConfiguration("Release")
			.WithProperty("PackageOutputPath", MakeAbsolute(new FilePath("./output")).FullPath)
			.WithProperty("DesignTimeBuild", "false"));	
			
            

			  NuGetRestore(csproj.ToString());

			   var configuration = Argument("configuration", "Release");

               var rootDir =System.IO.Path.Combine("source" , art.ArtifactId);
			   var dllPath =  System.IO.Path.Combine(rootDir,"bin",configuration , art.ArtifactId+".dll");

			  var nugetPackageDir = "output";

			  var nuGetPackSettings = new NuGetPackSettings
              {   
                 BasePath = rootDir,
                 OutputDirectory = nugetPackageDir,  
              };

	        NuGetPack(nuspec, nuGetPackSettings);
    }  
}

public  void DownloadExternals(List<ArtifactInfo> artifacts){
    foreach (var artifact in artifacts)
    { 
         if(artifact.ArtifactId=="mindspore-lite")
		  REPO_URL="https://developer.huawei.com/repo/";
		  else
		  REPO_URL="https://developer.huawei.com/repo/com/huawei/";  

        var url = $"{REPO_URL}{artifact.GroupId.Replace(".", @"/")}/{artifact.ArtifactId}/{artifact.Version}/{artifact.ArtifactId}-{artifact.Version}.{artifact.Extension}";
		var pomUrl = $"{REPO_URL}{artifact.GroupId.Replace(".", @"/")}/{artifact.ArtifactId}/{artifact.Version}/{artifact.ArtifactId}-{artifact.Version}.pom";
         
    	Information ("Package Url: " + url);

		var aar = $"{"./externals"}/{artifact.ArtifactId}.{artifact.Extension}";
		if (!FileExists (aar))
			DownloadFile(url, aar);

		var pom = $"{"./externals"}/{artifact.ArtifactId}.pom";
		if (!FileExists (pom))
			DownloadFile(pomUrl, pom);
    }
}

public class ArtifactInfo
{
    public ArtifactInfo()
    {
    }

    public ArtifactInfo(string id, string Version, string groupId, string extension = "aar")
    {
        ArtifactId = id;
        GroupId = groupId;
        this.Version = Version;
        Extension = extension; 
    }
    public string GroupId { get; set; }
    public string ArtifactId { get; set; } 
    public string Version { get; set; }
    public string Extension { get; set; }
}