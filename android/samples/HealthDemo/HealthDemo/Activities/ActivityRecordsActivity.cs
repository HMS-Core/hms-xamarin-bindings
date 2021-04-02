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
using System.Collections.Generic;
using Android.App;
using Android.Icu.Text;
using Android.OS;
using Android.Text.Method;
using Android.Widget;
using Huawei.Hms.Hihealth;
using Huawei.Hms.Hihealth.Data;
using Huawei.Hms.Hihealth.Options;
using Huawei.Hms.Hihealth.Result;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Support.Hwid.Result;
using Java.Util;
using Java.Util.Concurrent;

namespace XamarinHmsHealthDemo
{
    [Activity(Label = "ActivityRecordsActivity")]
    public class ActivityRecordsActivity : Activity
    {
        private static string TAG = "ActivityRecordSample";

        // Line separators for the display on the UI
        private static string Split = "*******************************" + System.Environment.NewLine;

        // ActivityRecordsController for managing activity records
        private ActivityRecordsController MyActivityRecordsController;

        // DataController for deleting activity records
        private DataController MyDataController;

        // Text view for displaying operation information on the UI
        private TextView LogInfoView;

        private Button BeginActivity;

        private Button EndActivity;

        private Button AddActivity;

        private Button GetActivity;

        private Button DeleteActivity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityRecords_layout);
            Init();
        }

        private void Init()
        {
            HiHealthOptions hiHealthOptions = HiHealthOptions.HiHealthOptionsBulider().Build();
            AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(hiHealthOptions);
            MyActivityRecordsController = HuaweiHiHealth.GetActivityRecordsController(this, signInHuaweiId);
            MyDataController= HuaweiHiHealth.GetDataController(this, signInHuaweiId);

            LogInfoView = (TextView)FindViewById(Resource.Id.activity_records_controller_log_info);
            LogInfoView.MovementMethod = ScrollingMovementMethod.Instance;

            BeginActivity = (Button)FindViewById(Resource.Id.records_controller_begin_activity);
            EndActivity = (Button)FindViewById(Resource.Id.records_controller_end_activity);
            AddActivity = (Button)FindViewById(Resource.Id.records_controller_add_activity); 
            GetActivity = (Button)FindViewById(Resource.Id.records_controller_get_activity);
            DeleteActivity = (Button)FindViewById(Resource.Id.records_controller_delete_activity);

            BeginActivity.Click += delegate { BeginActivityRecord(); };
            EndActivity.Click += delegate { EndActivityRecord(); };
            AddActivity.Click += delegate { AddActivityRecord(); };
            GetActivity.Click += delegate { GetActivityRecord(); };
            DeleteActivity.Click += delegate { DeleteActivityRecord(); };
        }

        // Add an activity record to the Health platform
        public async void AddActivityRecord()
        {
            Logger(Split + "Add ActivityRecord");

            // Build the time range of the request object: start time and end time
            Calendar Cal = Calendar.Instance;
            Date Now = new Date();
            Cal.Time = Now;
            long EndTime = Cal.TimeInMillis;
            Cal.Add(CalendarField.HourOfDay, -1);
            long StartTime = Cal.TimeInMillis;

            // Build the data collector object.
            DataCollector dataCollector =
                new DataCollector.Builder().SetDataType(DataType.DtContinuousStepsDelta)
                    .SetDataGenerateType(DataCollector.DataTypeRaw)
                    .SetPackageName(this)
                    .SetDataCollectorName("DataCollector1")
                    .Build();

            // Build the activity statistics and activity record request objects.
            ActivitySummary activitySummary = GetActivitySummary();

            // Create a data collector for total step count statistics.
            // ActivitySummary is used to bear the statistical data.
            DataCollector dataCollector2 =
                new DataCollector.Builder().SetDataType(DataType.DtContinuousStepsTotal)
                    .SetDataGenerateType(DataCollector.DataTypeRaw)
                    .SetPackageName(this)
                    .SetDataCollectorName("DataCollector2")
                    .Build();
            SamplePoint samplePoint = new SamplePoint.Builder(dataCollector2).Build();
            samplePoint.GetFieldValue(Field.FieldSteps).SetIntValue(1024);
            activitySummary.DataSummary = new[] { samplePoint };

            // Build the activity record request object
            ActivityRecord activityRecord = new ActivityRecord.Builder().SetName("AddActivityRecordTest")
                .SetDesc("Add ActivityRecord")
                .SetId("MyAddActivityRecordId")
                .SetActivityTypeId(HiHealthActivities.Running)
                .SetStartTime(StartTime, TimeUnit.Milliseconds)
                .SetEndTime(EndTime, TimeUnit.Milliseconds)
                .SetActivitySummary(activitySummary)
                .SetTimeZone("+0800")
                .Build();

            // Build the sampling sampleSet based on the dataCollector
            SampleSet sampleSet = SampleSet.Create(dataCollector);

            // Build the (DtContinuousStepsDelta) sampling data object and add it to the sampling dataSet
            SamplePoint samplePointDetail =
                sampleSet.CreateSamplePoint().SetTimeInterval(StartTime, EndTime, TimeUnit.Milliseconds);
            samplePointDetail.GetFieldValue(Field.FieldStepsDelta).SetIntValue(1024);
            sampleSet.AddSample(samplePointDetail);

            // Build the activity record addition request object
            ActivityRecordInsertOptions insertRequest =
                new ActivityRecordInsertOptions.Builder().SetActivityRecord(activityRecord).AddSampleSet(sampleSet).Build();

            CheckConnect();

            // Call the related method in the ActivityRecordsController to add activity records
            var AddTask = MyActivityRecordsController.AddActivityRecordAsync(insertRequest);
            try
            {
                await AddTask;

                if (AddTask.IsCompleted)
                {
                    if (AddTask.Exception == null)
                    {
                        Logger("Add ActivityRecord was successful!");
                    }
                    else
                    {
                        PrintFailureMessage(AddTask.Exception, "AddActivityRecord");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "AddActivityRecord");
            }
        }

        // Read historical activity records
        public async void GetActivityRecord()
        {
            Logger(Split + "Get ActivityRecord");

            // Build the time range of the request object: start time and end time
            Calendar Cal = Calendar.Instance;
            Date Now = new Date();
            Cal.Time = Now;
            long EndTime = Cal.TimeInMillis;
            Cal.Add(CalendarField.HourOfDay, -3);
            long StartTime = Cal.TimeInMillis;


            // Build the request body for reading activity records
            ActivityRecordReadOptions readRequest =
                new ActivityRecordReadOptions.Builder().SetTimeInterval(StartTime, EndTime, TimeUnit.Milliseconds)
                    .ReadActivityRecordsFromAllApps()
                    .Read(DataType.DtContinuousStepsDelta)
                    .Build();

            CheckConnect();

            // Call the read method of the ActivityRecordsController to obtain activity records
            // from the Health platform based on the conditions in the request body
            var GetTask = MyActivityRecordsController.GetActivityRecordAsync(readRequest);
            try
            {
                await GetTask;

                if (GetTask.IsCompleted)
                {
                    if (GetTask.Exception == null && GetTask.Result != null)
                    {
                        ActivityRecordReply result = GetTask.Result;
                        Logger("Get ActivityRecord was successful!");
                        // Print ActivityRecord and corresponding activity data in the result
                        IList<ActivityRecord> activityRecordList = result.ActivityRecords;
                        foreach (ActivityRecord activityRecord in activityRecordList)
                        {
                            if (activityRecord == null)
                            {
                                continue;
                            }
                            DumpActivityRecord(activityRecord);
                            foreach (SampleSet sampleSet in result.GetSampleSet(activityRecord))
                            {
                                DumpSampleSet(sampleSet);
                            }
                        }
                    }
                    else
                    {
                        PrintFailureMessage(GetTask.Exception, "GetActivityRecord");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "GetActivityRecord");
            }

        }

        // Start an activity record
        public async void BeginActivityRecord()
        {
            Logger(Split + "Begin ActivityRecord");
            long StartTime = Calendar.Instance.TimeInMillis;

            ActivitySummary activitySummary = new ActivitySummary();

            // Create a data collector for statics data
            // The numbers are generated randomly
            DataCollector dataCollector = new DataCollector.Builder().SetDataType(DataType.DtStatisticsSleep)
                .SetDataGenerateType(DataCollector.DataTypeRaw)
                .SetPackageName(this)
                .SetDataCollectorName("DataCollector1")
                .Build();
            SamplePoint samplePoint = new SamplePoint.Builder(dataCollector).Build();
            samplePoint.GetFieldValue(Field.AllSleepTime).SetIntValue(352);
            samplePoint.GetFieldValue(Field.GoBedTime).SetLongValue(1599580041000L);
            samplePoint.GetFieldValue(Field.SleepEfficiency).SetIntValue(4);
            samplePoint.GetFieldValue(Field.DreamTime).SetIntValue(58);
            samplePoint.GetFieldValue(Field.WakeUpTime).SetLongValue(1599608520000L);
            samplePoint.GetFieldValue(Field.DeepSleepTime).SetIntValue(82);
            samplePoint.GetFieldValue(Field.DeepSleepPart).SetIntValue(64);
            samplePoint.GetFieldValue(Field.AwakeTime).SetIntValue(3);
            samplePoint.GetFieldValue(Field.SleepScore).SetIntValue(73);
            samplePoint.GetFieldValue(Field.LightSleepTime).SetIntValue(212);
            samplePoint.GetFieldValue(Field.SleepLatency).SetIntValue(7487000);
            samplePoint.GetFieldValue(Field.WakeUpCnt).SetIntValue(2);
            samplePoint.GetFieldValue(Field.FallAsleepTime).SetLongValue(1599587220000L);

            activitySummary.DataSummary = new[] { samplePoint };

            // Build an ActivityRecord object
            ActivityRecord activityRecord = new ActivityRecord.Builder().SetId("MyBeginActivityRecordId")
                .SetName("BeginActivityRecord")
                .SetDesc("Begin ActivityRecord")
                .SetActivityTypeId(HiHealthActivities.Sleep)
                .SetStartTime(StartTime, TimeUnit.Milliseconds)
                .SetActivitySummary(activitySummary)
                .SetTimeZone("+0800")
                .Build();

            CheckConnect();

            var BeginTask = MyActivityRecordsController.BeginActivityRecordAsync(activityRecord);

            try
            {
                await BeginTask;

                if (BeginTask.IsCompleted)
                {
                    if (BeginTask.Exception == null)
                    {
                        Logger("BeginActivityRecord  success");
                    }
                    else
                    {
                        PrintFailureMessage(BeginTask.Exception, "BeginActivityRecord");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "BeginActivityRecord");
            }
        }

        // Stop an activity record
        public async void EndActivityRecord()
        {
            Logger(Split + "End ActivityRecord");

            // Call the related method of ActivityRecordsController to stop activity records.
            // The input parameter can be the ID string of ActivityRecord or null
            // Stop an activity record of the current app by specifying the ID string as the input parameter
            // Stop activity records of the current app by specifying null as the input parameter
            var EndTask = MyActivityRecordsController.EndActivityRecordAsync("MyBeginActivityRecordId");
            try
            {
                await EndTask;

                if (EndTask.IsCompleted)
                {
                    if (EndTask.Exception == null && EndTask.Result != null)
                    {
                        // Print ActivityRecord and corresponding activity data in the result
                        IList<ActivityRecord> activityRecords = EndTask.Result;
                        Logger("EndActivityRecord success");

                        // Return the list of activity records that have stopped
                        if (activityRecords.Count > 0)
                        {
                            foreach (ActivityRecord activityRecord in activityRecords)
                            {
                                DumpActivityRecord(activityRecord);
                            }
                        }
                        else
                        {
                            // Null will be returnded if none of the activity records has stopped
                            Logger("EndActivityRecord response is null");
                        }
                    }
                    else
                    {
                        PrintFailureMessage(EndTask.Exception, "EndActivityRecord");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "EndActivityRecord");
            }

        }

        //Delete activity record
        public async void DeleteActivityRecord()
        {
            Logger(Split + "Delete ActivityRecord");


            // Build the time range of the request object: start time and end time
            Calendar Cal = Calendar.Instance;
            Date Now = new Date();
            Cal.Time = Now;
            long EndTime = Cal.TimeInMillis;
            Cal.Add(CalendarField.HourOfDay, -2);
            long StartTime = Cal.TimeInMillis;


            // Build the request body for reading activity records
            ActivityRecordReadOptions readRequest =
                new ActivityRecordReadOptions.Builder().SetTimeInterval(StartTime, EndTime, TimeUnit.Milliseconds)
                    .ReadActivityRecordsFromAllApps()
                    .Read(DataType.DtContinuousStepsDelta)
                    .Build();


            // Call the read method of the ActivityRecordsController to obtain activity records
            // from the Health platform based on the conditions in the request body
            var GetTask = MyActivityRecordsController.GetActivityRecordAsync(readRequest);
            try
            {
                await GetTask;

                if (GetTask.IsCompleted)
                {
                    if (GetTask.Exception == null && GetTask.Result != null)
                    {
                        ActivityRecordReply result = GetTask.Result;
                        Logger("Reading ActivityRecord  response status " + result.Status.StatusCode);
                        IList<ActivityRecord> activityRecordList = result.ActivityRecords;
                        // Get ActivityRecord and corresponding activity data in the result
                        foreach (ActivityRecord activityRecord in activityRecordList)
                        {
                            DeleteOptions deleteOptions = new DeleteOptions.Builder().AddActivityRecord(activityRecord)
                                .SetTimeInterval(activityRecord.GetStartTime(TimeUnit.Milliseconds),
                                    activityRecord.GetEndTime(TimeUnit.Milliseconds), TimeUnit.Milliseconds)
                                .Build();
                            Logger("Begin Delete ActivityRecord is :" + activityRecord.Id);

                            // Delete ActivityRecord
                            var DeleteTask = MyDataController.DeleteAsync(deleteOptions);
                            try
                            {
                                await DeleteTask;

                                if (DeleteTask.IsCompleted)
                                {
                                    if (DeleteTask.Exception == null)
                                    {
                                        Logger("Delete ActivityRecord is Success:" + activityRecord.Id);
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
                    }
                    else
                    {
                        PrintFailureMessage(GetTask.Exception, "GetActivityForDelete");
                    }
                }
            }
            catch (Exception ex)
            {
                PrintFailureMessage(ex, "GetActivityForDelete");
            }
        }

        //return An ActivitySummary object with random values
        private ActivitySummary GetActivitySummary()
        {
            ActivitySummary activitySummary = new ActivitySummary();
            PaceSummary paceSummary = new PaceSummary();
            paceSummary.AvgPace = (Java.Lang.Double)247.27626;
            paceSummary.BestPace = (Java.Lang.Double)212.0;
            IDictionary<string, Java.Lang.Double> britishPaceMap = new Dictionary<string, Java.Lang.Double>();
            britishPaceMap.Add("50001893", (Java.Lang.Double)365.0);
            paceSummary.BritishPaceMap = britishPaceMap;
            IDictionary<string, Java.Lang.Double> partTimeMap = new Dictionary<string, Java.Lang.Double>();
            partTimeMap.Add("1.0", (Java.Lang.Double)456.0);
            paceSummary.PartTimeMap = partTimeMap;
            IDictionary<string, Java.Lang.Double> paceMap = new Dictionary<string, Java.Lang.Double>();
            paceMap.Add("1.0", (Java.Lang.Double)263.0);
            paceSummary.PaceMap = paceMap;
            IDictionary<string, Java.Lang.Double> britishPartTimeMap = new Dictionary<string, Java.Lang.Double>();
            britishPartTimeMap.Add("1.0", (Java.Lang.Double)263.0);
            paceSummary.BritishPartTimeMap = britishPartTimeMap;
            IDictionary<string, Java.Lang.Double> sportHealthPaceMap = new Dictionary<string, Java.Lang.Double>();
            sportHealthPaceMap.Add("102802480", (Java.Lang.Double)535.0);
            paceSummary.SportHealthPaceMap = sportHealthPaceMap;
            activitySummary.PaceSummary = paceSummary;
            return activitySummary;
        }

        //Print the SamplePoint in the SampleSet object as an output.
        //@param sampleSet indicating the sampling dataset)
        private void DumpSampleSet(SampleSet sampleSet)
        {
            Logger("Returned for SamplePoint and Data type: " + sampleSet.DataType.Name);
            foreach (SamplePoint dp in sampleSet.SamplePoints)
            {
                DateFormat dateFormat = DateFormat.TimeInstance;
                Logger("SamplePoint:");
                Logger("DataCollector:" + dp.DataCollector.DataCollectorName);
                Logger("\tType: " + dp.DataType.Name);
                Logger("\tStart: " + dateFormat.Format(dp.GetStartTime(TimeUnit.Milliseconds)));
                Logger("\tEnd: " + dateFormat.Format(dp.GetEndTime(TimeUnit.Milliseconds)));
                foreach (Field field in dp.DataType.Fields)
                {
                    Logger("\tField: " + field.ToString() + " Value: " + dp.GetFieldValue(field));
                }
            }
        }

        // Print the ActivityRecord object as an output.
        private void DumpActivityRecord(ActivityRecord activityRecord)
        {
            DateFormat dateFormat = DateFormat.DateInstance;
            DateFormat timeFormat = DateFormat.TimeInstance;
            Logger("ActivityRecord Printing -------------------------------------");
            Logger("Returned for ActivityRecord: " + activityRecord.Name + "\n\tActivityRecord Identifier is "
                + activityRecord.Id + "\n\tActivityRecord created by app is " + activityRecord.PackageName
                + "\n\tDescription: " + activityRecord.Desc + "\n\tStart: "
                + dateFormat.Format(activityRecord.GetStartTime(TimeUnit.Milliseconds)) + " "
                + timeFormat.Format(activityRecord.GetStartTime(TimeUnit.Milliseconds)) + "\n\tEnd: "
                + dateFormat.Format(activityRecord.GetEndTime(TimeUnit.Milliseconds)) + " "
                + timeFormat.Format(activityRecord.GetEndTime(TimeUnit.Milliseconds)) + "\n\tActivity:"
                + activityRecord.ActivityType);
            if (activityRecord.ActivitySummary != null)
            {
                PrintActivitySummary(activityRecord.ActivitySummary);
            }

            Logger("ActivityRecord Printing End ----------------------------------");
        }

        // Print the ActivitySummary object as an output.
        public void PrintActivitySummary(ActivitySummary activitySummary)
        {
            DateFormat timeFormat = DateFormat.TimeInstance;
            IList<SamplePoint> dataSummary = activitySummary.DataSummary;
            Logger("\nActivitySummary\n\t DataSummary: ");
            foreach (SamplePoint samplePoint in dataSummary)
            {
                Logger("\n\t samplePoint: \n\t DataCollector " + samplePoint.DataCollector.DataCollectorName + "\n\t DataType "
                    + samplePoint.DataType.Name + "\n\t StartTime " +timeFormat.Format(samplePoint.GetStartTime(TimeUnit.Milliseconds))
                    + "\n\t EndTime " + timeFormat.Format(samplePoint.GetEndTime(TimeUnit.Milliseconds)) + "\n\t SamplingTime "
                    + timeFormat.Format(samplePoint.GetSamplingTime(TimeUnit.Milliseconds)) + "\n\t FieldValues"
                    + ShowDictionaryValues(samplePoint.FieldValues));
            }
            // Printing PaceSummary
            PaceSummary paceSummary = activitySummary.PaceSummary;
            Logger("\n\t PaceSummary: \n\t AvgPace" + paceSummary.AvgPace + "\n\t BestPace" + paceSummary.BestPace
                + "\n\t PaceMap" + ShowDictionaryValues(paceSummary.PaceMap) + "\n\t PartTimeMap" + ShowDictionaryValues(paceSummary.PartTimeMap)+ "\n\t BritishPaceMap" + ShowDictionaryValues(paceSummary.BritishPaceMap)+ "\n\t BritishPartTimeMap"
                + ShowDictionaryValues(paceSummary.BritishPartTimeMap)+ "\n\t SportHealthPaceMap" + ShowDictionaryValues(paceSummary.SportHealthPaceMap));
        }

        // Check the connectivity
        private void CheckConnect()
        {
            if (MyActivityRecordsController == null)
            {
                HiHealthOptions hiHealthOptions = HiHealthOptions.HiHealthOptionsBulider().Build();
                AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(hiHealthOptions);
                MyActivityRecordsController = HuaweiHiHealth.GetActivityRecordsController(this, signInHuaweiId);
            }
        }
        
        //Send the operation result logs to the logcat and TextView control on the UI     
        private void Logger(string str)
        {
            CommonUtil.Logger(str, TAG, LogInfoView);
        }

        //Display Map values
        private string ShowDictionaryValues(IDictionary<string, Java.Lang.Double> map)
        {
            string values = "{";
            foreach (KeyValuePair<string, Java.Lang.Double> kvp in map)
            {
                values = values +"( "+ kvp.Key + " : " + kvp.Value.ToString() + " ) ";
            }
            values += "}";
           return values;
        }

        //Display Map values
        private string ShowDictionaryValues(IDictionary<string, Value> map)
        {
            string values = "{";
            foreach (KeyValuePair<string, Value> kvp in map)
            {
                values = values + "( " + kvp.Key + " : " + kvp.Value.ToString() + " ) ";
            }
            values += "}";
            return values;
        }

        //Print error code and error information for an exception.
        //@param exception indicating an exception object
        //@param api       api name
        private void PrintFailureMessage(Exception exception, String api)
        {
            CommonUtil.PrintFailureMessage(TAG, exception, api, LogInfoView);
        }

    }
}