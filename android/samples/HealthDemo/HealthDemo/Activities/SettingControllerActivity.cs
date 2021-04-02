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

using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Text.Method;
using Android.Widget;
using Huawei.Hms.Hihealth;
using Huawei.Hms.Hihealth.Data;
using Huawei.Hms.Hihealth.Options;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Support.Hwid.Result;
using Java.Lang;
using System.Threading.Tasks;

namespace XamarinHmsHealthDemo
{
    [Activity(Label = "SettingControllerActivity")]
    public class SettingControllerActivity : Activity
    {
       
        private static string TAG = "SettingController";

        // The container that stores Field
        private static IList<Field> SpinnerList = new List<Field>();

        // The container that stores Field name
        private static  IList<string> SpinnerListStr = new List<string>();

        // Object of SettingController for fitness and health data, providing APIs for read/write, batch read/write, and listening
        private SettingController MySettingController;

        // EditText for setting data type name information on the UI
        private EditText DataTypeNameView;

        // TextView for displaying operation information on the UI
        private TextView LogInfoView;

        // drop-down box of Field name
        private Spinner Spinner;

        // drop-down box adapter
        private ArrayAdapter<string> Adapter;

        // The field value you choose, default value is Field.FIELD_BPM
        private Field SelectedField = Field.FieldSteps;

        // The field name value you choose
        private string SelectedFieldStr = "FieldSteps";

        private Button AddNew;

        private Button Read;

        private Button CheckAuth;

        private Button GetAuth;

        private Button Disable;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SettingController_layout);
            
          
            InitSettingController();
            InitActivityView();
        
        }

        private void InitActivityView()
        {
            LogInfoView = (TextView)FindViewById(Resource.Id.setting_controller_log_info);
            LogInfoView.MovementMethod = ScrollingMovementMethod.Instance;

            foreach (Java.Lang.Reflect.Field field in Class.FromType(typeof(Field)).GetDeclaredFields())
            {
                if (field.Type != Class.FromType(typeof(Field)))
                {
                    continue;
                }
                try
                {
                    var FieldClass = Class.FromType(typeof(Field));
                    SpinnerList.Add((Field)(field.Get(FieldClass)));
                    SpinnerListStr.Add(field.Name);
                }
                catch (IllegalAccessException e)
                {
                    Logger("InitActivityView: catch an IllegalAccessException");
                }
            }

            int size = SpinnerListStr.Count;
            string[] array = SpinnerListStr.ToArray();

            Spinner = (Spinner)FindViewById(Resource.Id.spinner01);
            Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, array);
            Spinner.Adapter = Adapter;
            Spinner.ItemSelected += Spinner_ItemSelected; ;
            Spinner.SetSelection(SpinnerList.IndexOf(SelectedField));

            DataTypeNameView = (EditText)FindViewById(Resource.Id.data_type_name_text);
            DataTypeNameView.Text = (PackageName + ".anyExtendName");
            DataTypeNameView.Focusable = true;
            DataTypeNameView.RequestFocus();
            DataTypeNameView.FocusableInTouchMode = true;

            AddNew = (Button)FindViewById(Resource.Id.setting_controller_add_new_type);
            Read = (Button)FindViewById(Resource.Id.setting_controller_read_data_type);
            CheckAuth = (Button)FindViewById(Resource.Id.setting_controller_check_auth); ;
            GetAuth = (Button)FindViewById(Resource.Id.setting_controller_get_auth);
            Disable = (Button)FindViewById(Resource.Id.setting_controller_Disable);

            AddNew.Click += delegate { AddNewDataType(); };
            Read.Click += delegate { ReadDataType(); };
            CheckAuth.Click += delegate { CheckAuthorization(); };
            GetAuth.Click += delegate { GetAuthorization(); };
            Disable.Click += delegate { DisableHiHealth(); };
            
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (SpinnerList.Count!=0 && e.Position < SpinnerList.Count)
            {
                SelectedField = SpinnerList.ElementAt(e.Position);
                SelectedFieldStr = SpinnerListStr.ElementAt(e.Position);
                Logger("Your choice is ：" + SelectedFieldStr);
            }
        }

        private void InitSettingController()
        {
            // create HiHealth Options, donnot add any datatype here.
            HiHealthOptions hiHealthOptions = HiHealthOptions.HiHealthOptionsBulider().Build();
            // get AuthHuaweiId by HiHealth Options.
            AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(hiHealthOptions);
            MySettingController = HuaweiHiHealth.GetSettingController(this, signInHuaweiId);
        }


        // Add new DataType.
        // you need two object to add new DataType: DataTypeAddOptions and SettingController.
        // specify the field by drop-down box, You cannot add DataType with duplicate DataType's name.
        // You can add multiple field，For simplicity, only one field is added here.

        public async void AddNewDataType()
        {
            // get DataType name from EditText view,
            // The name must start with package name, and End with a custom name.
            string dataTypeName = DataTypeNameView.Text;
            // create DataTypeAddOptions,You must specify the Field that you want to add,
            // You can add multiple Fields here.
            DataTypeAddOptions dataTypeAddOptions =
                    new DataTypeAddOptions.Builder().SetName(dataTypeName).AddField(SelectedField).Build();

            // create SettingController and add new DataType
            // The added results are displayed in the phone screen
            Task<DataType> AddTask = MySettingController.AddDataTypeAsync(dataTypeAddOptions);
            try
            {
                await AddTask;

                if (AddTask.IsCompleted)
                {
                    if (AddTask.Exception == null && AddTask.Result != null)
                    {
                        Logger("Add dataType of " + SelectedFieldStr + " is true ");

                    }
                    else
                    {
                        PrintFailureMessage(AddTask.Exception, "AddNewDataType");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "AddNewDataType");
            }

        }

        // read DataType.
        // Get DataType with the specified name
        public async void ReadDataType()
        {
            // data type name
            string dataTypeName = DataTypeNameView.Text;

            // create SettingController and get DataType with the specified name
            // The results are displayed in the phone screen

            Task<DataType> ReadTask = MySettingController.ReadDataTypeAsync(dataTypeName);
            try
            {
                await ReadTask;

                if (ReadTask.IsCompleted)
                {
                    if (ReadTask.Exception == null && ReadTask.Result != null)
                    {
                        DataType result = ReadTask.Result;
                        Logger("DataType : " + result.Name);

                    }
                    else
                    {
                        PrintFailureMessage(ReadTask.Exception, "readDataType");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "ReadDataType");
            }

        }


        //disable HiHealth.
        //After calling this function, HiHealth will cancel All your Records.
        public async void DisableHiHealth()
        {
            // create SettingController and disable HiHealth (cancel All your Records).
            // The results are displayed in the phone screen.
            var DisableTask = MySettingController.DisableHiHealthAsync();
            try
            {
                await DisableTask;

                if (DisableTask.IsCompleted)
                {
                    if (DisableTask.Exception == null)
                    {
                        Logger("DisableHiHealth is completed ");
                    }
                    else
                    {
                        PrintFailureMessage(DisableTask.Exception, "disableHiHealth");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "disableHiHealth");
            }

        }

        
        //check whether the Huawei Health app authorise access to HealthKit.
        //After calling this function, if you do not authorise, we will pop the windows to Health app authorization windows.
        public async void CheckAuthorization()
        {
            // check whether the Huawei Health app authorise access to HealthKit.
            // if you do not authorise, we will pop the windows to Health app authorization windows.
            var CheckTask = MySettingController.CheckHealthAppAuthorizationAsync();
            try
            {
                await CheckTask;

                if (CheckTask.IsCompleted)
                {
                    if (CheckTask.Exception == null)
                    {
                        Logger("checkHealthAppAuthorisation is completed ");
                    }
                    else
                    {
                        PrintFailureMessage(CheckTask.Exception, "checkHealthAppAuthorization");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "checkHealthAppAuthorization");
            }

        }

        
        //Get whether the Huawei Health app authorise access to HealthKit.
        //After calling this function, return true means authorised, false means not authorised.
        
        public async void GetAuthorization()
        {
            // get whether the Huawei Health app authorise access to HealthKit.
            // After calling this function, return true means authorised, false means not authorised.
            var GetAuthTask = MySettingController.GetHealthAppAuthorizationAsync();

            try
            {
                await GetAuthTask;

                if (GetAuthTask.IsCompleted)
                {
                    if (GetAuthTask.Exception == null)
                    {
                        Logger("getHealthAppAuthorisation is completed ");
                    }
                    else
                    {
                        PrintFailureMessage(GetAuthTask.Exception, "getHealthAppAuthorisation");
                    }
                }

            }
            catch (System.Exception ex)
            {
                PrintFailureMessage(ex, "getHealthAppAuthorisation");
            }

        }

        //TextView to send the operation result logs to the logcat and to the UI

        private void Logger(string str)
        {
            CommonUtil.Logger(str, TAG, LogInfoView);
        }

        
        //Print error code and error information for an exception.
        //@param exception indicating an exception object
        //@param api       api name
        
        private void PrintFailureMessage(System.Exception exception, string api)
        {
            CommonUtil.PrintFailureMessage(TAG, exception, api, LogInfoView);
        }
    }
}