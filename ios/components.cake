var AGC_VERSION = "1.3.2.301";
var ANALYTICS_VERSION = "6.0.0.300";

var SLN_PATH = "./source/HmsCoreXamarinBindings.iOS.sln"; 

Artifact AGC_CORE_ARTIFACT      = new Artifact ("AgconnectCore", "Agconnect",AGC_VERSION ,"10.0"); 
Artifact ANALYTICS_KIT_ARTIFACT      = new Artifact ("HiAnalytics","Hms" ,ANALYTICS_VERSION ,"10.0"); 

var ARTIFACTS = new Dictionary<string, Artifact> {
	{ "AGConnectCore", AGC_CORE_ARTIFACT },
	{ "HiAnalytics", ANALYTICS_KIT_ARTIFACT },
};

void SetArtifactsDependencies ()
{
	ANALYTICS_KIT_ARTIFACT.Dependencies      = new[] {AGC_CORE_ARTIFACT};
	AGC_CORE_ARTIFACT.Dependencies      = null;
}

void SetArtifactsPodSpecs ()
{
	AGC_CORE_ARTIFACT.PodSpecs = new [] { 
		PodSpec.Create ("AGConnectCore",   AGC_VERSION),
		PodSpec.Create ("AGConnectCredential",   AGC_VERSION),
		PodSpec.Create ("HMFoundation",   AGC_VERSION)
	};

	ANALYTICS_KIT_ARTIFACT.PodSpecs = new [] {
		PodSpec.Create ("HiAnalytics",   ANALYTICS_VERSION)
	};
}