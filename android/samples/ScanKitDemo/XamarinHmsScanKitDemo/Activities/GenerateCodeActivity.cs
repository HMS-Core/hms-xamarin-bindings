/**
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */
using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Hmsscankit;
using Huawei.Hms.Ml.Scan;
using Java.IO;
using Java.Lang;
using Console = System.Console;

namespace XamarinHmsScanKitDemo.Activities
{
    [Activity(Label = "GenerateCodeActivity")]
    public class GenerateCodeActivity : Activity
    {
        private static int[] BarcodeTypes = {HmsScan.QrcodeScanType, HmsScan.DatamatrixScanType, HmsScan.Pdf417ScanType, HmsScan.AztecScanType,
            HmsScan.Ean13ScanType, HmsScan.Ean8ScanType, HmsScan.UpccodeAScanType, HmsScan.UpccodeEScanType, HmsScan.CodabarScanType,
            HmsScan.Code39ScanType, HmsScan.Code93ScanType, HmsScan.Code128ScanType, HmsScan.Itf14ScanType};
        private static int[] Color = { Android.Graphics.Color.Black, Android.Graphics.Color.Blue, Android.Graphics.Color.Gray, Android.Graphics.Color.Green, Android.Graphics.Color.Red, Android.Graphics.Color.Yellow };
        private static int[] Background = { Android.Graphics.Color.White, Android.Graphics.Color.Yellow, Android.Graphics.Color.Red, Android.Graphics.Color.Green, Android.Graphics.Color.Gray, Android.Graphics.Color.Blue, Android.Graphics.Color.Black };
        //Define a view.
        private EditText inputContent;
        private Spinner generateType, generateMargin, generateColor, generateBackground;
        private ImageView barcodeImage;
        private EditText barcodeWidth, barcodeHeight;
        private string content;
        private int width, height;
        private Bitmap resultImage;
        private int type = 0;
        private int margin = 1;
        private int color = Android.Graphics.Color.Black;
        private int background = Android.Graphics.Color.White;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_generate);
            //this.Window.DecorView.SystemUiVisibility = (View.SystemUiFlagLayoutFullscreen || View.SystemUiFlagLightStatusBar);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                Window window = Window;
                //window.DecorView.SystemUiVisibility = (View.SystemUiFlagLayoutFullscreen || View.SystemUiFlagLightStatusBar);
                if (window != null)
                {
                    window.AddFlags(WindowManagerFlags.TranslucentStatus | WindowManagerFlags.TranslucentNavigation);
                }
            }
            inputContent = FindViewById<EditText>(Resource.Id.barcode_content);
            generateType = FindViewById<Spinner>(Resource.Id.generate_type);
            generateMargin = FindViewById<Spinner>(Resource.Id.generate_margin);
            generateColor = FindViewById<Spinner>(Resource.Id.generate_color);
            generateBackground = FindViewById<Spinner>(Resource.Id.generate_backgroundcolor);
            barcodeImage = FindViewById<ImageView>(Resource.Id.barcode_image);
            barcodeWidth = FindViewById<EditText>(Resource.Id.barcode_width);
            barcodeHeight = FindViewById<EditText>(Resource.Id.barcode_height);

            generateType.ItemSelected += GenerateType_ItemSelected;
            generateType.NothingSelected += GenerateType_NothingSelected; ;
            generateMargin.ItemSelected += GenerateMargin_ItemSelected;
            generateMargin.NothingSelected += GenerateMargin_NothingSelected;
            generateColor.ItemSelected += GenerateColor_ItemSelected;
            generateColor.NothingSelected += GenerateColor_NothingSelected;
            generateBackground.ItemSelected += GenerateBackground_ItemSelected;
            generateBackground.NothingSelected += GenerateBackground_NothingSelected;


            FindViewById<Button>(Resource.Id.button_generate).Click += GenerateButton_Click; ;
            FindViewById<Button>(Resource.Id.button_save).Click += SaveButton_Click; ;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (resultImage == null)
            {
                Toast.MakeText(this, "Please generate barcode first!", ToastLength.Long).Show();
                return;
            }
            try
            {
                bool isSuccess = false;
                string fileName = DateTime.Now.Millisecond + ".jpg";
                string storePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                File appDir = new File(storePath);
                if (!appDir.Exists())
                {
                    appDir.Mkdir();
                }
                File file = new File(appDir, fileName);
                var fs = new System.IO.FileStream(file.Path, System.IO.FileMode.CreateNew);
                if (fs != null)
                {
                    isSuccess = resultImage.Compress(Bitmap.CompressFormat.Jpeg, 100, fs);
                }
                fs.Flush();
                fs.Close();
                Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
                SendBroadcast(new Intent(Intent.ActionMediaScannerScanFile, uri));

                if (isSuccess)
                {
                    Toast.MakeText(this, "Barcode has been saved locally", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Barcode save failed", ToastLength.Short).Show();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Toast.MakeText(this, "Unkown Error" + ex.Message, ToastLength.Long).Show();
            }
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            content = inputContent.Text;
            string inputWidth = barcodeWidth.Text;
            string inputHeight = barcodeHeight.Text;
            //Set the barcode width and height.
            if (inputWidth.Length <= 0 || inputHeight.Length <= 0)
            {
                width = 700;
                height = 700;
            }
            else
            {
                width = Integer.ParseInt(inputWidth);
                height = Integer.ParseInt(inputHeight);
            }
            //Set the barcode content.
            if (content.Length <= 0)
            {
                Toast.MakeText(this, "Please input content first!", ToastLength.Long).Show();
                return;
            }
            try
            {
                //Generate the barcode.
                HmsBuildBitmapOption options = new HmsBuildBitmapOption.Creator().SetBitmapMargin(margin).SetBitmapColor(color).SetBitmapBackgroundColor(background).Create();
                resultImage = ScanUtil.BuildBitmap(content, type, width, height, options);
                barcodeImage.SetImageBitmap(resultImage);
            }
            catch (WriterException ex)
            {
                Toast.MakeText(this, "Parameter Error!" + ex.Message, ToastLength.Long).Show();
            }
        }

 
        private void GenerateBackground_NothingSelected(object sender, AdapterView.NothingSelectedEventArgs e)
        {
            background = Background[0];
        }

        private void GenerateColor_NothingSelected(object sender, AdapterView.NothingSelectedEventArgs e)
        {
            color = Color[0];
        }

        private void GenerateMargin_NothingSelected(object sender, AdapterView.NothingSelectedEventArgs e)
        {
            margin = 1;
        }

        private void GenerateType_NothingSelected(object sender, AdapterView.NothingSelectedEventArgs e)
        {
            type = BarcodeTypes[0];
        }

        private void GenerateBackground_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            background = Background[e.Position];
        }

        private void GenerateColor_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            color = Color[e.Position];
        }

        private void GenerateMargin_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            margin = e.Position + 1;
        }

        private void GenerateType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            type = BarcodeTypes[e.Position];
        }
    }
}