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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Huawei.Hms.Mlsdk.Textembedding;
using GoogleGson;
using Java.IO;
using Org.Json;

namespace HmsXamarinMLDemo.MLKitActivities.LanguageRelated.TextEmbedding
{
    [Activity(Label = "TextEmbeddingAnalyseActivity")]
    public class TextEmbeddingAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private static readonly string Tag = "TextEmbeddingAnalyseActivity";

        private const string Path = "/sdcard/DCIM/TextEmbedding";       
        private MLTextEmbeddingAnalyzer analyzer;
        private static readonly int PermissionCode = 0x01 << 8;
        private EditText et_word2v;
        private EditText et_sentence2v;
        private EditText et1_word2s;
        private EditText et2_word2s;
        private TextView tv_word2s;
        private EditText et1_sentence2s;
        private EditText et2_sentence2s;
        private TextView tv_sentence2s;
        private EditText et_word_for_simil;
        private EditText et_num_for_simil;
        private TextView tv_simil_words;
        private TextView tv_dic_info;
        private EditText et_words2v;

        private static readonly string[] Permissions = {Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.Internet
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_text_embedding_analyse);

            this.et_word2v = (EditText) FindViewById(Resource.Id.et_word2v);
            this.et_sentence2v = (EditText) FindViewById(Resource.Id.et_sentence2v);

            this.et1_word2s = (EditText) FindViewById(Resource.Id.et1_word2s);
            this.et2_word2s = (EditText) FindViewById(Resource.Id.et2_word2s);
            this.tv_word2s = (TextView) FindViewById(Resource.Id.tv_word2s);

            this.et1_sentence2s = (EditText) FindViewById(Resource.Id.et1_sentence2s);
            this.et2_sentence2s = (EditText) FindViewById(Resource.Id.et2_sentence2s);
            this.tv_sentence2s = (TextView) FindViewById(Resource.Id.tv_sentence2s);

            this.et_word_for_simil = (EditText) FindViewById(Resource.Id.et_word_for_simil);
            this.et_num_for_simil = (EditText) FindViewById(Resource.Id.et_num_for_simil);
            this.tv_simil_words = (TextView) FindViewById(Resource.Id.tv_simil_words);

            this.tv_dic_info = (TextView) FindViewById(Resource.Id.tv_dic_info);
            this.et_words2v = (EditText) FindViewById(Resource.Id.et_words2v);
            this.et_words2v.SetOnClickListener(this);
            this.FindViewById(Resource.Id.bt_dic_info).SetOnClickListener(this);
            this.FindViewById(Resource.Id.bt_word2v).SetOnClickListener(this);
            this.FindViewById(Resource.Id.bt_words2v).SetOnClickListener(this);
            this.FindViewById(Resource.Id.bt_sentence2v).SetOnClickListener(this);
            this.FindViewById(Resource.Id.bt_word2s).SetOnClickListener(this);
            this.FindViewById(Resource.Id.bt_sentence2s).SetOnClickListener(this);
            this.FindViewById(Resource.Id.bt_word_for_simil).SetOnClickListener(this);

            ActivityCompat.RequestPermissions(this, Permissions, PermissionCode);

            this.CreateTextEmbeddingAnalyzer();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.bt_dic_info:
                    DictionaryVersionAction();
                    break;
                case Resource.Id.bt_word2v:
                    WordVectorAction();
                    break;
                case Resource.Id.bt_words2v:
                    WordBatchVectorAction();
                    break;
                case Resource.Id.bt_sentence2v:
                    SentenceVectorAction();
                    break;
                case Resource.Id.bt_word2s:
                    WordSimilarityAction();
                    break;
                case Resource.Id.bt_sentence2s:
                    SentenceSimilarityAction();
                    break;
                case Resource.Id.bt_word_for_simil:
                    SimilarWordsAction();
                    break;
                default:
                    break;
            }
        }

        private async void WordSimilarityAction()
        {
            Task<float> wordsSimilarityTask = analyzer.AnalyseWordsSimilarityAsync(et1_word2s.Text.ToString(), et2_word2s.Text.ToString());
            try
            {
                await wordsSimilarityTask;

                if (wordsSimilarityTask.IsCompleted)
                {
                    // Analyze success
                    var wordsSimilarity = wordsSimilarityTask.Result;
                    Toast.MakeText(this, "analyseWordsSimilarity success", ToastLength.Short).Show();
                    tv_word2s.Text = wordsSimilarity + "";
                }
                else
                {
                    // Analyze failed
                    Toast.MakeText(this, "analyseWordsSimilarity failed", ToastLength.Short).Show();
                }
            }
            catch(Exception ex)
            {
                //Operation failed
                Log.Error(Tag, ex.Message);
            }
        }

        private async void SentenceSimilarityAction()
        {
            Task<float> sentencesSimilarityTask = analyzer.AnalyseSentencesSimilarityAsync(et1_sentence2s.Text.ToString(), et2_sentence2s.Text.ToString());
            try
            {
                await sentencesSimilarityTask;
                if (sentencesSimilarityTask.IsCompleted && sentencesSimilarityTask?.Result != null)
                {
                    // Analyze success
                    var sentencesSimilarity = sentencesSimilarityTask.Result;
                    Toast.MakeText(this, "analyseSentencesSimilarity success", ToastLength.Short).Show();
                    tv_sentence2s.Text = sentencesSimilarity + "";
                }
                else
                {
                    // Analyze failed
                    Log.Debug(Tag, "Analyze Failed");
                }
            }
            catch (Exception ex)
            {
                // Operation failed
                Log.Error(Tag, ex.Message);
            }
        }

        private async void SimilarWordsAction()
        {
            Task<Java.Lang.Object> multipleSimilarityWordsTask = analyzer.AnalyseSimilarWordsAsync(et_word_for_simil.Text.ToString(), Int32.Parse(et_num_for_simil.Text.ToString()));
            try
            {
                await multipleSimilarityWordsTask;
                if (multipleSimilarityWordsTask.IsCompleted && multipleSimilarityWordsTask.Result != null)
                {
                    // Analyze success
                    List<string> words = new List<string>();
                    Java.Util.ArrayList arrayList = multipleSimilarityWordsTask.Result.JavaCast<Java.Util.ArrayList>();
                    var iterator = arrayList.Iterator();
                    while (iterator.HasNext)
                    {
                        words.Add((string)iterator.Next());
                    }

                    Toast.MakeText(this, "analyse multipleSimilarWords success", ToastLength.Short).Show();
                    JSONArray jsonObject = new JSONArray(words);
                    tv_simil_words.Text = jsonObject.ToString();
                }
                else
                {
                    // Analyze failed
                    Log.Debug(Tag, "Analyze Failed");
                }
            }
            catch (Exception ex)
            {
                //Operation failed
                Log.Error(Tag, ex.Message);
            }
        }

        private async void DictionaryVersionAction()
        {
            Task<MLVocabularyVersion> dictionaryVersionTask = analyzer.GetVocabularyVersion();
            try
            {
                await dictionaryVersionTask;
                if (dictionaryVersionTask.IsCompleted && dictionaryVersionTask.Result != null)
                {
                    // Analyze success
                    tv_dic_info.Text = (new Gson().ToJson(dictionaryVersionTask.Result));
                }
                else
                {
                    // Analyze failed
                    Log.Debug(Tag, "Analyze Failed");
                }
            }
            catch (Exception ex)
            {
                // Operation failed
                Log.Error(Tag, ex.Message);
            }
        }

        private async void WordBatchVectorAction()
        {
            string words = this.et_words2v.Text.ToString();
            ICollection<string> stringSet = new List<string>();
            if (!String.IsNullOrEmpty(words))
            {
                ICollection<string> stringList = words.Split(',').ToList();
                foreach(var item in stringList)
                    stringSet.Add(item);
            }

            // Querying Word Vectors in Batches.
            Task<Java.Lang.Object> wordVectorBatchTask = analyzer.AnalyseWordVectorBatchAsync(stringSet);
            Log.Debug(Tag, et_words2v.Text.ToString());
            try
            {
                await wordVectorBatchTask;
                if (wordVectorBatchTask.IsCompleted && wordVectorBatchTask.Result != null)
                {
                    // Analyze success
                    var wordsVector = wordVectorBatchTask.Result;

                    Toast.MakeText(this, "analyseWordBatchVector success", ToastLength.Short).Show();
                    JSONObject jsonObject = new JSONObject((IDictionary)wordsVector);
                    string result = "Words： " + et_words2v.Text.ToString() + "\n" + "Vector value：" + jsonObject.ToString() + "\n\n";
                    WriteTxtToFile(result, true);
                }
                else
                {
                    // Analyze failed
                    Log.Debug(Tag, "Analyze Failed");
                }
            }
            catch (Exception ex)
            {
                //Operation failed
                Log.Error(Tag, ex.Message);
            }
        }

        private async void WordVectorAction()
        {
            Task<Java.Lang.Object> wordVectorTask = analyzer.AnalyseWordVectorAsync(et_word2v.Text.ToString());
            Log.Debug(Tag, et_word2v.Text.ToString());
            try
            {
                await wordVectorTask;
                if (wordVectorTask.IsCompleted && wordVectorTask.Result != null)
                {
                    //Analyze success
                    Toast.MakeText(this, "analyseWordVector success", ToastLength.Short).Show();
                    try
                    {
                        var wordVector = wordVectorTask.Result;
                        JSONArray jsonObject = new JSONArray(wordVectorTask.Result);
                        string result = "word： " + et_word2v.Text.ToString() + "\n" + "\n" + "Vector value： " + jsonObject.ToString() + "\n\n";
                        WriteTxtToFile(result, true);
                    }
                    catch (JSONException e)
                    {
                        Log.Error(Tag, e.Message);
                    }
                }
                else
                {
                    // Analyze failed
                    Log.Debug(Tag, "Analyze Failed");
                }
            }
            catch (Exception ex)
            {
                //Operation failed
                Log.Error(Tag, ex.Message);
            }
        }

        private async void SentenceVectorAction()
        {
            Task<Java.Lang.Object> sentenceVectorTask = analyzer.AnalyseSentenceVectorAsync(et_sentence2v.Text.ToString());
            try
            {
                await sentenceVectorTask;
                if (sentenceVectorTask.IsCompleted && sentenceVectorTask.Result != null)
                {
                    // Analyze success
                    Toast.MakeText(this, "analyseSentenceVector success", ToastLength.Short).Show();
                    try
                    {
                        JSONArray jsonObject = new JSONArray(sentenceVectorTask.Result);
                        string result = "sentence： " + et_sentence2v.Text.ToString() + "\n" + "Vector value： " + jsonObject.ToString() + "\n\n";

                        WriteTxtToFile(result, true);
                    }
                    catch (JSONException e)
                    {
                        Log.Error(Tag, e.Message);
                    }
                }
                else
                {
                    // Analyze failed
                    Log.Debug(Tag, "Analyze Failed");
                }
            }
            catch (Exception ex)
            {
                // Operation failed
                Log.Error(Tag, ex.Message);
            }
        }

        private void CreateTextEmbeddingAnalyzer()
        {
            MLTextEmbeddingSetting setting = new MLTextEmbeddingSetting.Factory()
                    .SetLanguage(MLTextEmbeddingSetting.LanguageEn)
                    .Create();
            this.analyzer = MLTextEmbeddingAnalyzerFactory.Instance.GetMLTextEmbeddingAnalyzer(setting);
        }

        public void WriteTxtToFile(string buffer, bool append)
        {
            Log.Info(Tag, "writeTxtToFile strFilePath =" + Path);
            RandomAccessFile raf = null;
            FileOutputStream outStream = null;
            try
            {

                File dir = new File(Path);
                if (!dir.Exists())
                {
                    dir.Mkdirs();
                }
                File file = new File(Path + "/" + "Result.txt");
                if (!file.Exists())
                {
                    file.CreateNewFile();
                }

                if (append)
                {
                    raf = new RandomAccessFile(file, "rw");
                    raf.Seek(file.Length());
                    raf.Write(Encoding.ASCII.GetBytes(buffer));
                }
                else
                {
                    outStream = new FileOutputStream(file);
                    outStream.Write(Encoding.ASCII.GetBytes(buffer));
                    outStream.Flush();
                }
            }
            catch (IOException e)
            {
                Log.Error(Tag, e.Message);
            }
            finally
            {
                try
                {
                    if (raf != null)
                    {
                        raf.Close();
                    }
                    if (outStream != null)
                    {
                        outStream.Close();
                    }
                }
                catch (IOException e)
                {
                    Log.Error(Tag, e.Message);
                }
            }
        }
    }
}