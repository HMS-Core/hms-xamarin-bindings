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

using System;
using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Text.Method;
using Android.Widget;
using Huawei.Hms.Hihealth;
using Huawei.Hms.Hihealth.Data;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Support.Hwid.Result;
using Java.Util;
using Java.Util.Concurrent;
using Huawei.Hms.Hihealth.Options;
using Huawei.Hms.Hihealth.Result;

namespace XamarinHmsHealthDemo
{
    [Activity(Label = "DataControllerActivity")]
    public class DataControllerActivity : Activity
    {
        private static string TAG = "DataController";

        // Line separators for the display on the UI
        private static string Split = "*******************************" + System.Environment.NewLine;

        // Object of controller for fitness and health data, providing APIs for read/write, batch read/write
        private DataController MyDataController;

        // Internal context object of the activity
        private Context MyContext;

        // TextView for displaying operation information on the UI
        private TextView LogInfoView;

        private Button InsertBtn;

        private Button ReadBtn;

        private Button UpdateBtn;

        private Button DeleteBtn;

        private Button ReadTodayBtn;

        private Button ReadDailyBtn;

        private Button ClearBtn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DataController_layout);
            MyContext = this;
            LogInfoView = FindViewById<TextView>(Resource.Id.data_controller_log_info);
            LogInfoView.MovementMethod= ScrollingMovementMethod.Instance;

            InsertBtn = FindViewById<Button>(Resource.Id.data_controller_insert);
            UpdateBtn = FindViewById<Button>(Resource.Id.data_controller_update);
            DeleteBtn = FindViewById<Button>(Resource.Id.data_controller_delete);
            ReadBtn = FindViewById<Button>(Resource.Id.data_controller_read);
            ReadTodayBtn = FindViewById<Button>(Resource.Id.data_controller_readtoday);
            ReadDailyBtn = FindViewById<Button>(Resource.Id.data_controller_readdaily);
            ClearBtn = FindViewById<Button>(Resource.Id.data_controller_clearall);

            InitDataController();

            InsertBtn.Click += delegate { InsertData(); };
            ReadBtn.Click += delegate { ReadData(); };
            UpdateBtn.Click += delegate { UpdateData(); };
            DeleteBtn.Click += delegate { DeleteData(); };
            ReadTodayBtn.Click += delegate { ReadToday(); };
            ReadDailyBtn.Click += delegate { ReadDaily(); };
            ClearBtn.Click += delegate { ClearCloudData(); };
          
        }
        private void InitDataController()
        {
            // Obtain and set the read & write permissions for DtContinuousStepsDelta and DtInstantaneousHeight.
            // Use the obtained permissions to obtain the data controller object.
            HiHealthOptions hiHealthOptions = HiHealthOptions.HiHealthOptionsBulider()
                .AddDataType(DataType.DtContinuousStepsDelta, HiHealthOptions.AccessRead)
                .AddDataType(DataType.DtContinuousStepsDelta, HiHealthOptions.AccessWrite)
                .AddDataType(DataType.DtInstantaneousHeight, HiHealthOptions.AccessRead)
                .AddDataType(DataType.DtInstantaneousHeight, HiHealthOptions.AccessWrite)
                .Build();
            AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(hiHealthOptions);
            MyDataController = HuaweiHiHealth.GetDataController(this, signInHuaweiId);
        }

        public async void InsertData()
        {
            // 1. Build a DataCollector object.
            DataCollector MyDataCollector = new DataCollector.Builder().SetPackageName(MyContext)
            .SetDataType(DataType.DtContinuousStepsDelta)
            .SetDataStreamName("STEPS_DELTA")
            .SetDataGenerateType(DataCollector.DataTypeRaw)
            .Build();

            // 2. Create a sampling dataset set based on the data collector.
            SampleSet sampleSet = SampleSet.Create(MyDataCollector);

            // 3. Build the start time, end time, and incremental step count for a DT_CONTINUOUS_STEPS_DELTA sampling point.
            DateTime startDate = DateTime.Parse("2020-12-15 09:00:00");
            DateTime endDate = DateTime.Parse("2020-12-15 09:05:00");
            int stepsDelta = 1000;

            // 4. Build a DtContinuousStepsDelta sampling point.
            SamplePoint samplePoint = sampleSet.CreateSamplePoint()
                .SetTimeInterval(GetTime(startDate), GetTime(endDate), TimeUnit.Milliseconds);
            samplePoint.GetFieldValue(Field.FieldStepsDelta).SetIntValue(stepsDelta);

            // 5. Save a DtContinuousStepsDelta sampling point to the sampling dataset.
            // You can repeat steps 3 through 5 to add more sampling points to the sampling dataset.
            sampleSet.AddSample(samplePoint);

            // 6. Call the data controller to insert the sampling dataset into the Health platform.
            // 7. Calling the data controller to insert the sampling dataset is an asynchronous Task.
            var InsertTask = MyDataController.InsertAsync(sampleSet);
 
            try
            {
                await InsertTask;

                if (InsertTask.IsCompleted)
                {
                    if (InsertTask.Exception == null)
                    {
                        Logger("Success insert an SampleSet into HMS core");
                        ShowSampleSet(sampleSet);
                        Logger(Split);
                    }
                    else
                    {
                        PrintFailureMessage(InsertTask.Exception, "Insert");
                    }
                }
                
            }
            catch (Exception ex)
            {
                PrintFailureMessage(ex, "Insert");
            }

        }

        public async void ReadData()
        {
            // 1. Build the time range for the query: start time and end time.
            DateTime startDate = DateTime.Parse("2020-12-15 09:00:00");
            DateTime endDate = DateTime.Parse("2020-12-15 09:05:00");

            // 2. Build the condition-based query objec
            ReadOptions readOptions = new ReadOptions.Builder().Read(DataType.DtContinuousStepsDelta)
                .SetTimeRange(GetTime(startDate), GetTime(endDate), TimeUnit.Milliseconds)
                .Build();

            // 3. Use the specified condition query object to call the data controller to query the sampling dataset.
            // 4. Calling the data controller to query the sampling dataset is an asynchronous Task.
            
            var ReadTask = MyDataController.ReadAsync(readOptions);

            try
            {
                await ReadTask;

                if (ReadTask.IsCompleted && ReadTask.Result != null)
                {
                    ReadReply result = ReadTask.Result;
                    if (ReadTask.Exception == null)
                    {
                        Logger("Success read of SampleSets from HMS core");
                        foreach (SampleSet sampleSet in result.SampleSets)
                        {
                            ShowSampleSet(sampleSet);
                        }
                        Logger(Split);
                    }
                    else
                    {
                        PrintFailureMessage(ReadTask.Exception, "Read");
                    }
                }

            }
            catch (Exception ex)
            {
                PrintFailureMessage(ex, "Read");
            }

        }

        public async void DeleteData()
        {
            // 1. Build a DataCollector object.
            DataCollector MyDataCollector = new DataCollector.Builder().SetPackageName(MyContext)
            .SetDataType(DataType.DtContinuousStepsDelta)
            .SetDataStreamName("STEPS_DELTA")
            .SetDataGenerateType(DataCollector.DataTypeRaw)
            .Build();

            // 2. Build the time range for the deletion: start time and end time.
            DateTime startDate = DateTime.Parse("2020-12-15 09:00:00");
            DateTime endDate = DateTime.Parse("2020-12-15 09:05:00");

            // 3. Build a parameter object as the conditions for the deletion.
            DeleteOptions deleteOptions = new DeleteOptions.Builder().AddDataCollector(MyDataCollector)
            .SetTimeInterval(GetTime(startDate), GetTime(endDate), TimeUnit.Milliseconds)
            .Build();

            // 4. Use the specified condition deletion object to call the data controller to delete the sampling dataset.
            // 5. Calling the data controller to delete the sampling dataset is an asynchronous Task.
          
            var DeleteTask = MyDataController.DeleteAsync(deleteOptions);

            try
            {
                await DeleteTask;

                if (DeleteTask.IsCompleted)
                {
                    if (DeleteTask.Exception == null)
                    {
                        Logger("Success delete sample data from HMS core");
                        Logger(Split);
                    }
                    else
                    {
                        PrintFailureMessage(DeleteTask.Exception, "Delete");
                    }
                }

            }
            catch (Exception ex)
            {
                PrintFailureMessage(ex, "Delete");
            }

        }

        public async void UpdateData()
        {
            // 1. Build a DataCollector object.
            DataCollector MyDataCollector = new DataCollector.Builder().SetPackageName(MyContext)
            .SetDataType(DataType.DtContinuousStepsDelta)
            .SetDataStreamName("STEPS_DELTA")
            .SetDataGenerateType(DataCollector.DataTypeRaw)
            .Build();

            // 2. Build the sampling dataset for the update: create a sampling dataset
            // for the update based on the data collector.
            SampleSet sampleSet = SampleSet.Create(MyDataCollector);

            // 3. Build the start time, end time, and incremental step count for
            // a DtContinuousStepsDelta sampling point for the update.
            DateTime startDate = DateTime.Parse("2020-12-15 09:00:00");
            DateTime endDate = DateTime.Parse("2020-12-15 09:05:00");
            int stepsDelta = 2000;

            // 4. Build a DtContinuousStepsDelta sampling point for the update.
            SamplePoint samplePoint = sampleSet.CreateSamplePoint()
                .SetTimeInterval(GetTime(startDate), GetTime(endDate), TimeUnit.Milliseconds);
            samplePoint.GetFieldValue(Field.FieldStepsDelta).SetIntValue(stepsDelta);

            // 5. Add an updated DtContinuousStepsDelta sampling point to the sampling dataset for the update.
            // You can repeat steps 3 through 5 to add more updated sampling points to the sampling dataset for the update.
            sampleSet.AddSample(samplePoint);

            // 6. Build a parameter object for the update.
            // Note: (1) The start time of the modified object updateOptions cannot be greater than the minimum
            // value of the start time of all sample data points in the modified data sample set
            // (2) The end time of the modified object updateOptions cannot be less than the maximum value of the
            // end time of all sample data points in the modified data sample set
            UpdateOptions updateOptions = new UpdateOptions.Builder().SetTimeInterval(GetTime(startDate), GetTime(endDate), TimeUnit.Milliseconds)
                    .SetSampleSet(sampleSet)
                    .Build();

            // 7. Use the specified parameter object for the update to call the
            // data controller to modify the sampling dataset.
            // 8.Calling the data controller to modify the sampling dataset is an asynchronous Task.
           var UpdateTask = MyDataController.UpdateAsync(updateOptions);

            try
            {
                await UpdateTask;

                if (UpdateTask.IsCompleted)
                {
                    if (UpdateTask.Exception == null)
                    {
                        Logger("Success update sample data from HMS core");
                        Logger(Split);
                    }
                    else
                    {
                        PrintFailureMessage(UpdateTask.Exception, "Update");
                    }
                }

            }
            catch (Exception ex)
            {
                PrintFailureMessage(ex, "Update");
            }
        }

        public async void ReadToday()
        {
            // 1. Use the specified data type (DtContinuousStepsDelta) to call the data controller to query
            // the summary data of this data type of the current day.
            var TodaySummationTask = MyDataController.ReadTodaySummationAsync(DataType.DtContinuousStepsDelta);

            // 2. Calling the data controller to query the summary data of the current day is an
            // asynchronous Task.
            // Note: In this example, the inserted data time is fixed at 2020-12-15 09:05:00.
            // When commissioning the API, you need to change the inserted data time to the current date
            // for data to be queried.
            try
            {
                await TodaySummationTask;

                if (TodaySummationTask.IsCompleted && TodaySummationTask.Result != null)
                {
                    SampleSet result = TodaySummationTask.Result;
                    if (TodaySummationTask.Exception == null)
                    {
                        Logger("Success read today summation from HMS core");
                        ShowSampleSet(result);
                        Logger(Split);
                    }
                    else
                    {
                        PrintFailureMessage(TodaySummationTask.Exception, "ReadTodaySummation");
                    }
                }

            }
            catch (Exception ex)
            {
                PrintFailureMessage(ex, "ReadTodaySummation");
            }
        }

        public async void ReadDaily()
        {
            // 1. Initialization start and end time, The first four digits of the shaping data represent the year,
            // the middle two digits represent the month, and the last two digits represent the day
            int endTime = 20201215;
            int startTime = 20201201;

            // 1. Use the specified data type (DtContinuousStepsDelta), start and end time to call the data
            // controller to query the summary data of this data type of the daily
            var DaliySummationTask = MyDataController.ReadDailySummationAsync(DataType.DtContinuousStepsDelta, startTime, endTime);

            // 2. Calling the data controller to query the summary data of the daily is an
            // asynchronous Task.
            // Note: In this example, the read data time is fixed at 20201215 and 20201201.
            // When commissioning the API, you need to change the read data time to the current date
            // for data to be queried.
            try
            {
                await DaliySummationTask;

                if (DaliySummationTask.IsCompleted && DaliySummationTask.Result != null)
                {
                    SampleSet result = DaliySummationTask.Result;
                    if (DaliySummationTask.Exception == null)
                    {
                        Logger("Success read daily summation from HMS core");
                        ShowSampleSet(result);
                        Logger(Split);
                    }
                    else
                    {
                        PrintFailureMessage(DaliySummationTask.Exception, "ReadDailySummation");
                    }
                }

            }
            catch (Exception ex)
            {
                PrintFailureMessage(ex, "ReadDailySummation");
            }

        }

        public async void ClearCloudData()
        {
            // 1. Call the ClearAll method of the data controller to delete data
            // inserted by the current app from the device and cloud.
            var ClearTask = MyDataController.ClearAllAsync();

            // 2. Calling the data controller to clear user data from the device and cloud is an asynchronous Task.
            try
            {
                await ClearTask;

                if (ClearTask.IsCompleted)
                {
                    if (ClearTask.Exception == null)
                    {
                        Logger("ClearAll success");
                        Logger(Split);
                    }
                    else
                    {
                        PrintFailureMessage(ClearTask.Exception, "ClearAll");
                    }
                }

            }
            catch (Exception ex)
            {
                PrintFailureMessage(ex, "ClearAll");
            }

        }

        public void Logger(string str)
        {
            CommonUtil.Logger(str, TAG, LogInfoView);
        }
            
        
        //Print the SamplePoint in the SampleSet object as an output.
        //@param sampleSet (indicating the sampling dataset)
        
        public  void ShowSampleSet(SampleSet sampleSet)
        {
            SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

            foreach (SamplePoint samplePoint in sampleSet.SamplePoints)
            {
                Logger("Sample point type: " + samplePoint.DataType.Name);
                Logger("Start: " + dateFormat.Format(new Date(samplePoint.GetStartTime(TimeUnit.Milliseconds))));
                Logger("End: " + dateFormat.Format(new Date(samplePoint.GetEndTime(TimeUnit.Milliseconds))));
                foreach (Field field in samplePoint.DataType.Fields)
                {
                    Logger("Field: " + field.Name + " Value: " + samplePoint.GetFieldValue(field));
                }
            }
        }

        
        //Print error code and error information for an exception.
        //@param exception indicating an exception object
        //@param api       api name
        
        public  void PrintFailureMessage(Exception exception, String api)
        {
            CommonUtil.PrintFailureMessage(TAG, exception, api, LogInfoView);
        }

        //Convert Dates to long values 
        private long GetTime(DateTime date )
        {
            var time = (date.ToUniversalTime() - new DateTime(1970, 1, 1));
            return (long)(time.TotalMilliseconds + 0.5);
        }
    }
}