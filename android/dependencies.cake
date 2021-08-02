var AGC_VERSION="1.4.2.301";
var TASK_VERSION="1.4.1.300";
var OPEN_DEVICE_VERSION="5.1.1.301";
var BASE_VERSION="5.2.0.300";
var NETWORK_VERSION="4.0.20.301";
var UPDATE_VERSION="3.0.1.300"; 
var SECURITY_VERSION="1.1.5.302"; 
var COMPONENTVERIFYSDK_VERSION="11.1.0.300";

var MLCOMPUTER_VERSION="2.0.5.300"; 

var DYNAMIC_API_VERSION="1.0.15.301"; 

ArtifactInfo AGCONNECT_CORE_ARTIFACT = new ArtifactInfo ("agconnect-core",AGC_VERSION,"agconnect","aar");
ArtifactInfo AGCONNECT_HTTPS_ARTIFACT = new ArtifactInfo ("agconnect-https",AGC_VERSION,"agconnect","aar");
ArtifactInfo AGCONNECT_CREDENTIAL_ARTIFACT = new ArtifactInfo ("agconnect-credential",AGC_VERSION,"agconnect","aar");
ArtifactInfo DATASTORE_CORE_ARTIFACT = new ArtifactInfo ("datastore-core",AGC_VERSION,"agconnect","aar");
ArtifactInfo DATASTORE_ANNOTATION_ARTIFACT = new ArtifactInfo ("datastore-annotation",AGC_VERSION,"agconnect","jar");
ArtifactInfo TASKS_ARTIFACT = new ArtifactInfo ("tasks",TASK_VERSION,"hmf","aar");


ArtifactInfo OPEN_DEVICE_ARTIFACT = new ArtifactInfo ("opendevice",OPEN_DEVICE_VERSION,"hms","aar");
ArtifactInfo BASE_DEVICE_ARTIFACT = new ArtifactInfo ("base",BASE_VERSION,"hms","aar");
ArtifactInfo AVAILABLE_UPDATE_ARTIFACT = new ArtifactInfo ("availableupdate",BASE_VERSION,"hms","aar");

ArtifactInfo LOG_ARTIFACT = new ArtifactInfo ("log",BASE_VERSION,"hms","aar");
ArtifactInfo DEVICE_ARTIFACT = new ArtifactInfo ("device",BASE_VERSION,"hms","aar");
ArtifactInfo UI_ARTIFACT = new ArtifactInfo ("ui",BASE_VERSION,"hms","aar");
ArtifactInfo STATS_ARTIFACT = new ArtifactInfo ("stats",BASE_VERSION,"hms","aar");
ArtifactInfo HATOOL_ARTIFACT = new ArtifactInfo ("hatool",BASE_VERSION,"hms","aar");

ArtifactInfo NETWORK_COMMON_ARTIFACT = new ArtifactInfo ("network-common",NETWORK_VERSION,"hms","aar");
ArtifactInfo NETWORK_GRS_ARTIFACT = new ArtifactInfo ("network-grs",NETWORK_VERSION,"hms","aar");
ArtifactInfo NETWORK_FRAMEWORK_COMPAT_ARTIFACT = new ArtifactInfo ("network-framework-compat",NETWORK_VERSION,"hms","aar");

ArtifactInfo UPDATE_ARTIFACT = new ArtifactInfo ("update",UPDATE_VERSION,"hms","aar");

ArtifactInfo DYNAMIC_API_ARTIFACT = new ArtifactInfo ("dynamic-api",DYNAMIC_API_VERSION,"hms","aar");

ArtifactInfo MLCOMPUTER_AGC_INNER_ARTIFACT = new ArtifactInfo ("ml-computer-agc-inner",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_AIDLBASE_INNER_ARTIFACT = new ArtifactInfo ("ml-computer-aidlbase-inner",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_CAMERA_INNER_ARTIFACT = new ArtifactInfo ("ml-computer-camera-inner",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_COMMONUTILS_INNER_ARTIFACT = new ArtifactInfo ("ml-computer-commonutils-inner",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_HA_INNER_ARTIFACT = new ArtifactInfo ("ml-computer-ha-inner",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_GRS_INNER_ARTIFACT = new ArtifactInfo ("ml-computer-grs-inner",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_DYNAMIC_ARTIFACT = new ArtifactInfo ("ml-computer-dynamic",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_SDKBASE_INNER_ARTIFACT = new ArtifactInfo ("ml-computer-sdkbase-inner",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_MODELDOWNLOAD_ARTIFACT = new ArtifactInfo ("ml-computer-model-download","2.0.4.300","hms","aar");
ArtifactInfo MLCOMPUTER_NET_ARTIFACT = new ArtifactInfo ("ml-computer-net",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_VISIONCLOUD_ARTIFACT = new ArtifactInfo ("ml-computer-vision-cloud",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_VISIONBASE_ARTIFACT = new ArtifactInfo ("ml-computer-vision-base",MLCOMPUTER_VERSION,"hms","aar");
ArtifactInfo MLCOMPUTER_VISIONINNER_ARTIFACT = new ArtifactInfo ("ml-computer-vision-inner",MLCOMPUTER_VERSION,"hms","aar");


ArtifactInfo SECURITY_BASE_ARTIFACT = new ArtifactInfo ("security-base",SECURITY_VERSION,"android.hms","aar");
ArtifactInfo SECURITY_ENCRYPT_ARTIFACT = new ArtifactInfo ("security-encrypt",SECURITY_VERSION,"android.hms","aar");
ArtifactInfo SECURITY_SSL_ARTIFACT = new ArtifactInfo ("security-ssl",SECURITY_VERSION,"android.hms","aar");
ArtifactInfo SECURITY_ECC_ARTIFACT = new ArtifactInfo ("security-ecc","1.1.5.301","android.hms","aar");
ArtifactInfo SECURITY_INTENT_ARTIFACT = new ArtifactInfo ("security-intent","1.1.5.301","android.hms","aar");

ArtifactInfo COMPONENTVERIFYSDK_ARTIFACT = new ArtifactInfo ("componentverifysdk",COMPONENTVERIFYSDK_VERSION,"hms","aar");


var DEPENDENCY_ARTIFACTS = new List<ArtifactInfo>(); 


public void SetDependencyArtifacts()
{
	 DEPENDENCY_ARTIFACTS.AddRange(new List<ArtifactInfo> {
	 TASKS_ARTIFACT ,
	 DATASTORE_ANNOTATION_ARTIFACT ,
	 DATASTORE_CORE_ARTIFACT ,
	 AGCONNECT_CORE_ARTIFACT,
	 AGCONNECT_HTTPS_ARTIFACT ,
	 AGCONNECT_CREDENTIAL_ARTIFACT  ,
	 NETWORK_FRAMEWORK_COMPAT_ARTIFACT,
	 NETWORK_COMMON_ARTIFACT ,
	 NETWORK_GRS_ARTIFACT ,
	 UPDATE_ARTIFACT,
	 LOG_ARTIFACT,
	 DEVICE_ARTIFACT,
	 STATS_ARTIFACT,
	 UI_ARTIFACT,
	 AVAILABLE_UPDATE_ARTIFACT,
	 HATOOL_ARTIFACT,
	 DYNAMIC_API_ARTIFACT,
	 MLCOMPUTER_AGC_INNER_ARTIFACT,        
     MLCOMPUTER_AIDLBASE_INNER_ARTIFACT   ,
     MLCOMPUTER_CAMERA_INNER_ARTIFACT ,    
     MLCOMPUTER_COMMONUTILS_INNER_ARTIFACT,
     MLCOMPUTER_HA_INNER_ARTIFACT       ,  
     MLCOMPUTER_GRS_INNER_ARTIFACT  ,      
     MLCOMPUTER_DYNAMIC_ARTIFACT  ,        
     MLCOMPUTER_SDKBASE_INNER_ARTIFACT    ,
     MLCOMPUTER_MODELDOWNLOAD_ARTIFACT  ,  
     MLCOMPUTER_NET_ARTIFACT      ,        
     MLCOMPUTER_VISIONCLOUD_ARTIFACT   ,   
     MLCOMPUTER_VISIONBASE_ARTIFACT ,      
     MLCOMPUTER_VISIONINNER_ARTIFACT   ,   
	 SECURITY_ECC_ARTIFACT,
	 SECURITY_INTENT_ARTIFACT,
	 SECURITY_ENCRYPT_ARTIFACT,
	 SECURITY_SSL_ARTIFACT,
	 SECURITY_BASE_ARTIFACT,
	 BASE_DEVICE_ARTIFACT ,
	 OPEN_DEVICE_ARTIFACT
	 });
}