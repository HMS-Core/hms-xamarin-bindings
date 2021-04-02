/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

        Licensed under the Apache License, Version 2.0 (the "License");
        you may not use this file except in compliance with the License.
        You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

        Unless required by applicable law or agreed to in writing, software
        distributed under the License is distributed on an "AS IS" BASIS,
        WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
        See the License for the specific language governing permissions and
        limitations under the License.
*/
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using HmsXamarinMLDemo.MLKitActivities.TextRelated.Text;
using HmsXamarinMLDemo.MLKitActivities.TextRelated.Document;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.Landmark;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.Classification;
using HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Asr;
using HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Tts;
using HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Aft;
using HmsXamarinMLDemo.MLKitActivities.TextRelated.BankCard;
using HmsXamarinMLDemo.MLKitActivities.TextRelated.GeneralCard;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.ImageSegmentation;
using HmsXamarinMLDemo.MLKitActivities.LanguageRelated.Translate;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.Object;
using Huawei.Hms.Mlsdk.Common;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.ImageSuperResolution;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.DocumentSkewCorrection;
using HmsXamarinMLDemo.MLKitActivities.BodyRelated.HandKeypoint;
using HmsXamarinMLDemo.MLKitActivities.BodyRelated.LivenessDetection;
using HmsXamarinMLDemo.MLKitActivities.BodyRelated.Skeleton;
using HmsXamarinMLDemo.MLKitActivities.FormRecognition;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.TextImageSuperResolution;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.SceneDetection;
using HmsXamarinMLDemo.MLKitActivities.LanguageRelated.TextEmbedding;
using HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face3d;
using HmsXamarinMLDemo.MLKitActivities.VoiceRelated.SoundDetect;
using HmsXamarinMLDemo.MLKitActivities.VoiceRelated.RealTimeTranscription;
using HmsXamarinMLDemo.MLKitActivities.CustomModel;
using HmsXamarinMLDemo.MLKitActivities.ImageRelated.ProductVisionSearch;
using HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face;

namespace HmsXamarinMLDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity , View.IOnClickListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            MLApplication.Instance.ApiKey = "YOUR API KEY";

            this.FindViewById(Resource.Id.btn_face_live).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_face_image).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_text).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_object).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_document).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_classification).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_landmark).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_translate).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_productvisionsearch).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_imgseg_image).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_imgseg_live).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_bcr).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_gcr).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_tts).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_asr).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_aft).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_img_super_res).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_doc_skew_correction).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_handkeypoint_still).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_handkeypoint_live).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_liveness_detection).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_skeleton_image).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_skeleton_live).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_custom_model).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_form_recognition).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_text_img_super_res).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_image_scene_detection).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_live_scene_detection).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_textEmbedding).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_3dface_image).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_live_3dface).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_sound_detect).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_realtime_transcription).SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
                switch (v.Id)
                {
                    case Resource.Id.btn_text:
                        this.StartActivity(new Android.Content.Intent(this, typeof(ImageTextAnalyseActivity)));
                        break;
                    case Resource.Id.btn_document:
                        this.StartActivity(new Android.Content.Intent(this, typeof(ImageDocumentAnalyseActivity)));
                        break;
                    case Resource.Id.btn_landmark:
                        this.StartActivity(new Android.Content.Intent(this, typeof(ImageLandmarkAnalyseActivity)));
                        break;
                    case Resource.Id.btn_productvisionsearch:
                        this.StartActivity(new Android.Content.Intent(this, typeof(ProductVisionSearchAnalyseActivity)));                      
                        break;
                    case Resource.Id.btn_classification:
                        this.StartActivity(new Android.Content.Intent(this, typeof(ImageClassificationAnalyseActivity)));
                        break;
                    case Resource.Id.btn_asr:
                        this.StartActivity(new Android.Content.Intent(this, typeof(AsrAnalyseActivity)));
                        break;
                    case Resource.Id.btn_tts:
                        this.StartActivity(new Android.Content.Intent(this, typeof(TtsActivity)));//typeof(TtsAnalyseActivity)));
                        break;
                    case Resource.Id.btn_aft:
                        this.StartActivity(new Android.Content.Intent(this, typeof(AftAnalyseActivity)));
                        break;
                    case Resource.Id.btn_bcr:
                        this.StartActivity(new Android.Content.Intent(this, typeof(BcrAnalyseActivity)));
                        break;
                    case Resource.Id.btn_gcr:
                        this.StartActivity(new Android.Content.Intent(this, typeof(GcrAnalyseActivity)));
                        break;
                    case Resource.Id.btn_imgseg_live:
                        this.StartActivity(new Android.Content.Intent(this, typeof(ImageSegmentationLiveAnalyseActivity)));
                        break;
                    case Resource.Id.btn_imgseg_image:
                        this.StartActivity(new Android.Content.Intent(this, typeof(ImageSegmentationStillAnalyseActivity)));
                        break;
                    case Resource.Id.btn_translate:
                        this.StartActivity(new Android.Content.Intent(this, typeof(TranslatorActivity)));
                        break;
                    case Resource.Id.btn_object:
                        this.StartActivity(new Android.Content.Intent(this, typeof(LiveObjectAnalyseActivity)));
                        break;
                    case Resource.Id.btn_face_image:
                        this.StartActivity(new Android.Content.Intent(this, typeof(StillFaceAnalyseActivity)));
                        break;
                    case Resource.Id.btn_face_live:
                        this.StartActivity(new Android.Content.Intent(this, typeof(LiveFaceAnalyseActivity)));
                        break;
                    case Resource.Id.btn_img_super_res:
                        this.StartActivity(new Android.Content.Intent(this, typeof(ImageSuperResolutionActivity)));
                        break;
                    case Resource.Id.btn_doc_skew_correction:
                        this.StartActivity(new Android.Content.Intent(this, typeof(DocumentSkewCorrectionActivity)));
                        break;
                    case Resource.Id.btn_handkeypoint_still:
                        this.StartActivity(new Android.Content.Intent(this, typeof(StillHandKeyPointAnalyseActivity)));
                        break;
                    case Resource.Id.btn_handkeypoint_live:
                        this.StartActivity(new Android.Content.Intent(this, typeof(LiveHandKeyPointAnalyseActivity)));
                        break;
                    case Resource.Id.btn_liveness_detection:
                        this.StartActivity(new Android.Content.Intent(this, typeof(LiveLivenessDetectionActivity)));
                        break;
                    case Resource.Id.btn_skeleton_image:
                        this.StartActivity(new Android.Content.Intent(this, typeof(StillSkeletonAnalyseActivity)));
                        break;
                    case Resource.Id.btn_skeleton_live:
                        this.StartActivity(new Android.Content.Intent(this, typeof(LiveSkeletonAnalyseActivity)));
                        break;
                  case Resource.Id.btn_custom_model:
                    this.StartActivity(new Android.Content.Intent(this, typeof(CustomModelActivity)));
                    break;
                    case Resource.Id.btn_form_recognition:
                        this.StartActivity(new Android.Content.Intent(this, typeof(FormRecognitionActivity)));
                        break;
                    case Resource.Id.btn_text_img_super_res:
                        this.StartActivity(new Android.Content.Intent(this, typeof(TextImageSuperResolutionActivity)));
                        break;
                    case Resource.Id.btn_image_scene_detection:
                        this.StartActivity(new Android.Content.Intent(this, typeof(SceneDetectionStillAnalyseActivity)));
                        break;
                    case Resource.Id.btn_live_scene_detection:
                        this.StartActivity(new Android.Content.Intent(this, typeof(SceneDetectionLiveAnalyseActivity)));
                        break;
                    case Resource.Id.btn_textEmbedding:
                        this.StartActivity(new Android.Content.Intent(this, typeof(TextEmbeddingAnalyseActivity)));
                        break;
                    case Resource.Id.btn_3dface_image:
                        this.StartActivity(new Android.Content.Intent(this, typeof(Still3DFaceAnalyseActivity)));
                        break;
                    case Resource.Id.btn_live_3dface:
                        this.StartActivity(new Android.Content.Intent(this, typeof(Live3DFaceAnalyseActivity)));
                        break;
                    case Resource.Id.btn_sound_detect:
                        this.StartActivity(new Android.Content.Intent(this, typeof(SoundDetectActivity)));
                        break;
                    case Resource.Id.btn_realtime_transcription:
                        this.StartActivity(new Android.Content.Intent(this, typeof(RealTimeTranscriptionActivity)));
                        break;
                    default:
                        break;
            }
        }
    }
}