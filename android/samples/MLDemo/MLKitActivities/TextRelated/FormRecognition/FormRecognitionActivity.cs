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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Fr;
using GoogleGson;

namespace HmsXamarinMLDemo.MLKitActivities.FormRecognition
{
    [Activity(Label = "FormRecognitionActivity")]
    public class FormRecognitionActivity : AppCompatActivity , View.IOnClickListener
    {
        private const string Tag = "FormRecognitionActivity";
        TextView form_result;
        Button form_detect;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_form_recognition);
            form_result = (TextView) FindViewById(Resource.Id.form_result);
            form_detect = (Button) FindViewById(Resource.Id.form_detect);
            form_detect.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            Analyze();
        }

        private async void Analyze()
        {
            // Get bitmap.
            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.form_recognition);
            // Convert bitmap to MLFrame.
            MLFrame frame = MLFrame.FromBitmap(bitmap);
            // Create analyzer.
            MLFormRecognitionAnalyzer analyzer = MLFormRecognitionAnalyzerFactory.Instance.FormRecognitionAnalyzer;

            Task<JsonObject> task = analyzer.AnalyseFrameAsync(frame);

            try
            {
                await task;

                if (task.IsCompleted && task.Result != null)
                {
                    //Recognition success
                    JsonObject jsonObject = task.Result;
                    if(jsonObject.Get("retCode").AsInt == MLFormRecognitionConstant.Success)
                    {
                        string str = jsonObject.ToString();
                        form_result.Text = str;
                        try
                        {
                            Gson gson = new Gson();
                            MLFormRecognitionTablesAttribute attribute = (MLFormRecognitionTablesAttribute)gson.FromJson(str, Java.Lang.Class.FromType(typeof(MLFormRecognitionTablesAttribute)));
                            Log.Debug(Tag, "RetCode: " + attribute.RetCode);
                            MLFormRecognitionTablesAttribute.TablesContent tablesContent = attribute.GetTablesContent();
                            Log.Debug(Tag, "tableCount: " + tablesContent.TableCount);
                            IList<MLFormRecognitionTablesAttribute.TablesContent.TableAttribute> tableAttributeArrayList = tablesContent.TableAttributes;
                            Log.Debug(Tag, "tableID: " + tableAttributeArrayList.ElementAt(0).Id);
                            IList<MLFormRecognitionTablesAttribute.TablesContent.TableAttribute.TableCellAttribute> tableCellAttributes = tableAttributeArrayList.ElementAt(0).TableCellAttributes;
                            for (int i = 0; i < tableCellAttributes.Count; i++)
                            {
                                Log.Debug(Tag, "startRow: " + tableCellAttributes.ElementAt(i).StartRow);
                                Log.Debug(Tag, "endRow: " + tableCellAttributes.ElementAt(i).EndRow);
                                Log.Debug(Tag, "startCol: " + tableCellAttributes.ElementAt(i).StartCol);
                                Log.Debug(Tag, "endCol: " + tableCellAttributes.ElementAt(i).EndCol);
                                Log.Debug(Tag, "textInfo: " + tableCellAttributes.ElementAt(i).TextInfo);
                                Log.Debug(Tag, "cellCoordinate: ");
                                MLFormRecognitionTablesAttribute.TablesContent.TableAttribute.TableCellAttribute.TableCellCoordinateAttribute coordinateAttribute = tableCellAttributes.ElementAt(i).GetTableCellCoordinateAttribute();
                                Log.Debug(Tag, "topLeft_x: " + coordinateAttribute.TopLeftX);
                                Log.Debug(Tag, "topLeft_y: " + coordinateAttribute.TopLeftY);
                                Log.Debug(Tag, "topRight_x: " + coordinateAttribute.TopRightX);
                                Log.Debug(Tag, "topRight_y: " + coordinateAttribute.TopRightY);
                                Log.Debug(Tag, "bottomLeft_x: " + coordinateAttribute.BottomLeftX);
                                Log.Debug(Tag, "bottomLeft_y: " + coordinateAttribute.BottomLeftY);
                                Log.Debug(Tag, "bottomRight_x: " + coordinateAttribute.BottomRightX);
                                Log.Debug(Tag, "bottomRight_y: " + coordinateAttribute.BottomRightY);
                            }
                        }
                        catch(Exception e)
                        {
                            Log.Error(Tag, e.Message);
                        }
                    }
                }
                else
                {
                    //Recognition failed
                    Log.Debug(Tag, "Recognition Failed");
                }
            }
            catch (Exception ex)
            {
                //Operation failed
                Log.Error(Tag, ex.Message);
            }

        }
    }
}