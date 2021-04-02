/*
       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinHmsNearbyDemo
{
    public class DialogLoader
    {
        public Activity ActivityContext;
        public AlertDialog Dialog;
        public int Progress;
        public string Type;
        public long TotalBytes;
        public long TransferredBytes;
        public long PayloadID;
        ProgressBar PB;
        TextView OpType;
        TextView ProgressBytes;


        public DialogLoader(Activity activityContext, int progress, string type, long totalBytes, long transferredBytes, long payloadID) 
        {
            ActivityContext = activityContext;
            Progress = progress;
            Type = type;
            TotalBytes = totalBytes;
            TransferredBytes = transferredBytes;
            PayloadID = payloadID;
        }
        public void UpdateProgress(long transferredBytes, int progress,long totalBytes)
        {
            PB.Progress = progress;
            ProgressBytes.Text = FileUtil.FormatBytes(transferredBytes) + "/" + FileUtil.FormatBytes(totalBytes);

        }
        public void StartDialogLoading()
        {

            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(ActivityContext);
            var inflater = ActivityContext.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
            
            View convertView = inflater.Inflate(Resource.Layout.PBDialog, null);
            PB = (ProgressBar)convertView.FindViewById(Resource.Id.progressBar1);
            OpType = (TextView)convertView.FindViewById(Resource.Id.textView1);
            ProgressBytes = (TextView)convertView.FindViewById(Resource.Id.tv_progress);

            PB.Progress = Progress;
            OpType.Text = Type;
            ProgressBytes.Text= FileUtil.FormatBytes(TransferredBytes)+ "/" + FileUtil.FormatBytes(TotalBytes);
            builder.SetView(convertView);
            builder.SetCancelable(true);
            Dialog = builder.Create();
            Dialog.Show();
        }
        
        public void Dismiss()
        {
            Dialog.Dismiss();

        } 
            

    }
}