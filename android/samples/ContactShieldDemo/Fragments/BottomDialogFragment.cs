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
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using XamarinHmsContactShieldDemo.Interfaces;

namespace XamarinHmsContactShieldDemo.Fragments
{
    public class BottomDialogFragment : BottomSheetDialogFragment,View.IOnClickListener
    {
        private IOnItemSelectedListener onItemSelectedListener;
        Button btnStartNormal, btnStartNoPersistent,btnDismiss;
        public static BottomDialogFragment newInstance()
        {
            
            return new BottomDialogFragment();
        }
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            if (context is IOnItemSelectedListener)
            {
                onItemSelectedListener = (IOnItemSelectedListener)context;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.bottom_sheet_dialog_layout, container, false);
            btnStartNormal = view.FindViewById<Button>(Resource.Id.btnStartNormalContactShield);
            btnStartNoPersistent = view.FindViewById<Button>(Resource.Id.btnStartNoPersistentContactShield);
            btnDismiss = view.FindViewById<Button>(Resource.Id.btnDismissDialog);

            Dialog.SetCanceledOnTouchOutside(false);

            btnStartNormal.SetOnClickListener(this);
            btnStartNoPersistent.SetOnClickListener(this);
            btnDismiss.SetOnClickListener(this);
            

            return view;
        }
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btnStartNormalContactShield:
                    onItemSelectedListener.OnItemSelected(Resource.Id.btnStartNormalContactShield);
                    break;
                case Resource.Id.btnStartNoPersistentContactShield:
                    onItemSelectedListener.OnItemSelected(Resource.Id.btnStartNoPersistentContactShield);
                    break;
                case Resource.Id.btnDismissDialog:
                    onItemSelectedListener.OnItemSelected(Resource.Id.btnDismissDialog);
                    break;
                default:
                    break;
            }
            Dismiss();
        }
    }
}