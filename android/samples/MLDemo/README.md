
<p align="center">
  <h1 align="center">HMS ML Kit Xamarin Android Plugin - Demo</h1>
</p>

This demo project is an example to demonstrate the features of the HMS ML Kit Xamarin Android Plugin.

[> Learn More](https://developer.huawei.com/consumer/en/doc/development/HMS-Plugin-Guides-V1/about-service-0000001052602130-V1)

<img src="../.docs/mldemomainpage.png" width = 30% height = 50% style="margin:1.5em">

## Installation

In the Solution Explorer panel, right click on the solution name and select Manage NuGet Packages. Search for following packages and install the packages into your Xamarin.Android projects.

### ML Kit Base Library

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionCloud   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionCloud"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionCloud?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

### Text services

#### Text Recognition

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionOcr   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionCloud"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionOcr?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionOcrBase  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionOcrBase"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionOcrBase?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionOcrCnModel   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionOcrCnModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionOcrCnModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionOcrJkModel   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionOcrJkModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionOcrJkModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionOcrLatinModel   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionOcrLatinModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionOcrLatinModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Bank Card Recognition

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerCardBcr  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerCardBcr"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerCardBcr?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerCardBcrInner  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerCardBcrInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerCardBcrInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerCardBcrNodynamicModel   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerCardBcrNodynamicModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerCardBcrNodynamicModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerCardBcrNodynamicSdk  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerCardBcrNodynamicSdk"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerCardBcrNodynamicSdk?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### General Card Recognition

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerCardGcrPlugin  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerCardGcrPlugin"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerCardGcrPlugin?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerCardQaPlugin  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerCardQaPlugin"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerCardQaPlugin?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Form Recognition

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionFormrecognition  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFormrecognition"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFormrecognition?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionFormrecognitionInner  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFormrecognitionInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFormrecognitionInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionFormrecognitionModel   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFormrecognitionModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFormrecognitionModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

### Language/Voice-related Services

#### Text Translation

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerTranslate  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerTranslate"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerTranslate?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerTranslateModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerTranslateModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerTranslateModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Language Detection

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerLanguageDetection   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerLanguageDetection"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerLanguageDetection?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerLanguageDetectionModel   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerLanguageDetectionModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerLanguageDetectionModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerLanguageInner   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerLanguageInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerLanguageInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Automatic Speech Recognition

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVoiceAsr  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceAsr*"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceAsr*?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVoiceAsrPlugin  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceAsrPlugin"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceAsrPlugin?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVoiceAsrSdk   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceAsrSdk"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceAsrSdk?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Text to Speech

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVoiceTts  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceTts"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceTts?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVoiceTtsInner  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceTtsInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceTtsInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVoiceTtsModelBee   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceTtsModelBee"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceTtsModelBee?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVoiceTtsModelEagle  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceTtsModelEagle"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceTtsModelEagle?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Audio File Transcription

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVoiceAft  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceAft"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceAft?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Real-Time Transcription

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVoiceRealtimetranscription  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVoiceRealtimetranscription"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVoiceRealtimetranscription?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Sound Detection

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlSpeechSemanticsSounddectSdk  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlSpeechSemanticsSounddectSdk"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlSpeechSemanticsSounddectSdk?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlSpeechSemanticsSounddectInner  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlSpeechSemanticsSounddectInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlSpeechSemanticsSounddectInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlSpeechSemanticsSounddectModel   |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlSpeechSemanticsSounddectModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlSpeechSemanticsSounddectModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

### Image-related Services

#### Image Classification

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionClassification  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionClassification"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionClassification?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionImageClassificationModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionImageClassificationModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionImageClassificationModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Object Detection and Tracking

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionObject  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionObject"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionObject?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionObjectDetectionModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionObjectDetectionModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionObjectDetectionModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Image Segmentation

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionSegmentation  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionSegmentation"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionSegmentation?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionImageSegmentationBase  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionImageSegmentationBase"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionImageSegmentationBase?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionImageSegmentationBodyModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionImageSegmentationBodyModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionImageSegmentationBodyModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionImageSegmentationMulticlassModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionImageSegmentationMulticlassModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionImageSegmentationMulticlassModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Product Visual Search

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlProductVisualSearchPlugin |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlProductVisualSearchPlugin"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlProductVisualSearchPlugin?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Image Super-Resolution

| Library Name  | Nuget |
|--------|-----|
|  Huawei.Hms.MlComputerVisionImagesuperresolution |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionImagesuperresolution"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionImagesuperresolution?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionImagesuperresolutionInner |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionImagesuperresolutionInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionImagesuperresolutionInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionImagesuperresolutionModel |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionImagesuperresolutionModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionImagesuperresolutionModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Document Skew Correction

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionDocumentskew |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionDocumentskew"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionDocumentskew?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionDocumentskewInner  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionDocumentskewInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionDocumentskewInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionDocumentskewModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionDocumentskewModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionDocumentskewModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Text Image Super-Resolution

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionTextimagesuperresolution |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionTextimagesuperresolution"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionTextimagesuperresolution?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionTextimagesuperresolutionInner |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionTextimagesuperresolutionInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionTextimagesuperresolutionInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionTextimagesuperresolutionModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionTextimagesuperresolutionModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionTextimagesuperresolutionModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Scene Detection

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionScenedetection |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionScenedetection"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionScenedetection?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionScenedetectionInner |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionScenedetectionInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionScenedetectionInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionScenedetectionModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionScenedetectionModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionScenedetectionModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

### Face/Body-related Services

#### Face Detection

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionFace |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFace"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFace?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionFaceBase  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFaceBase"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFaceBase?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionFace3dModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFace3dModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFace3dModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionFaceEmotionModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFaceEmotionModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFaceEmotionModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionFaceFeatureModel |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFaceFeatureModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFaceFeatureModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionFaceShapePointModel |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionFaceShapePointModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionFaceShapePointModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Skeleton Detection

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionSkeleton |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionSkeleton"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionSkeleton?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionSkeletonBase |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionSkeletonBase"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionSkeletonBase?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionSkeletonInner  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionSkeletonInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionSkeletonInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionSkeletonModel |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionSkeletonModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionSkeletonModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionYogaModel |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionYogaModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionYogaModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Liveness Detection

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionLivenessdetection |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionLivenessdetection"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionLivenessdetection?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionLivenessdetectionInner |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionLivenessdetectionInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionLivenessdetectionInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionLivenessdetectionNodynamicModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionLivenessdetectionNodynamicModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionLivenessdetectionNodynamicModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

#### Hand Keypoint Detection

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerVisionHandkeypoint |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionHandkeypoint"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionHandkeypoint?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionHandkeypointInner |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionHandkeypointInner"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionHandkeypointInner?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerVisionHandkeypointModel  |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerVisionHandkeypointModel"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerVisionHandkeypointModel?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

### Natural Language Processing Services

#### Text Embedding

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlNlpTextembedding |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlNlpTextembedding"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlNlpTextembedding?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |

### Custom Model Services

#### Custom Model

| Library Name  | Nuget |
|--------|-----|
| Huawei.Hms.MlComputerModelExecutor |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerModelExecutor"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerModelExecutor?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Hms.MlComputerModelDownload |  <a href="https://www.nuget.org/packages/Huawei.Hms.MlComputerModelDownload"><img src="https://img.shields.io/nuget/v/Huawei.Hms.MlComputerModelDownload?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |
| Huawei.Mindspore.MindsporeLite |  <a href="https://www.nuget.org/packages/Huawei.Mindspore.MindsporeLite"><img src="https://img.shields.io/nuget/v/Huawei.Mindspore.MindsporeLite?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a> |


## Documentation

You can follow below links to learn how to set up your environment and project before using HMS ML Kit Xamarin Android Plugin in your application.

- [Quick Start](https://developer.huawei.com/consumer/en/doc/development/HMS-Plugin-Guides-V1/prepare-dev-env-0000001052968081-V1)
- [Integrating the HMS Core SDK](https://developer.huawei.com/consumer/en/doc/development/HMS-Plugin-Guides-V1/integrate-hms-core-0000001052248064-V1) 
- [Setting Package Information in Xamarin](https://developer.huawei.com/consumer/en/doc/development/HMS-Plugin-Guides-V1/setting-package-information-0000001074172648-V1)

### Additional Topics

- [Reference](https://developer.huawei.com/consumer/en/doc/development/HMS-Plugin-References-V1/overview-0000001052991421-V1)

## Supported Environments
 
- Android 4.4 KitKat (API level 19) and later versions

## Questions or Issues

If you have questions about how to use HMS samples, try the following options:
- [Stack Overflow](https://stackoverflow.com/questions/tagged/huawei-mobile-services) is the best place for any programming questions. Be sure to tag your question with 
**huawei-mobile-services**.
- [Huawei Developer Forum](https://forums.developer.huawei.com/forumPortal/en/home?fid=0101187876626530001) HMS Core Module is great for general questions, or seeking recommendations and opinions.
- [Huawei Developer Docs](https://developer.huawei.com/consumer/en/doc/overview/HMS-Core-Plugin) is place to official documentation for all HMS Core Kits, you can find detailed documentations in there.

## License

HMS ML Kit Xamarin Android Plugin - Demo is licensed under [Apache 2.0 license](LICENCE)