/*
        Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Text;
using Java.Util;

namespace XamarinHmsWalletDemo.Utils
{
    public class BasisTimesUtils
    {
        private static DatePickerDialog datePickerDialog;
        public static long GetLongtimeOfYMD(string time)
        {
            try
            {
                SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd");
                Date date = sdf.Parse(time);
                return date.Time;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static BasisTimesUtils ShowDatePickerDialog(Context context,string title,int year,int month,int day,IOnDatePickerListener onDateTimePickerListener)
        {
            return ShowDatePickerDialog(context, AlertDialog.ThemeHoloLight, title, year, month, day, onDateTimePickerListener);
        }

        private static BasisTimesUtils ShowDatePickerDialog(Context context, int themeId, string title, int year, int month, int day, IOnDatePickerListener onDateTimePickerListener)
        {
            datePickerDialog = new DatePickerDialog(context, themeId, new OnDateSetListener(onDateTimePickerListener),year,month-1,day);
            datePickerDialog.SetOnCancelListener(new OnCancelListener(onDateTimePickerListener));

            if (!TextUtils.IsEmpty(title))
            {
                datePickerDialog.SetTitle(title);
            }
            datePickerDialog.Show();
            return new BasisTimesUtils();

        }

        public interface IOnDatePickerListener
        {
            void OnConfirm(int year, int month, int dayOfMonth);
            void OnCancel();
        }

        public class OnDateSetListener : Java.Lang.Object, DatePickerDialog.IOnDateSetListener
        {
            IOnDatePickerListener onDatePickerListener;
            public OnDateSetListener(IOnDatePickerListener onDatePickerListener)
            {
                this.onDatePickerListener = onDatePickerListener;
            }
            public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
            {
                month = month + 1;
                if(onDatePickerListener != null)
                {
                    onDatePickerListener.OnConfirm(year, month, dayOfMonth);
                }
            }
        }

        public class OnCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
        {
            IOnDatePickerListener onDatePickerListener;
            public OnCancelListener(IOnDatePickerListener onDatePickerListener)
            {
                this.onDatePickerListener = onDatePickerListener;
            }
            public void OnCancel(IDialogInterface dialog)
            {
                if(onDatePickerListener != null)
                {
                    onDatePickerListener.OnCancel();
                }
            }
        }
    }
}