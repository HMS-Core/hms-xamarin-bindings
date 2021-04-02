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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlplugin.Productvisionsearch;
using Huawei.Hms.Mlsdk.Productvisionsearch;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.ProductVisionSearch
{
    public class ProductFragment : MLProductVisionSearchCapture.AbstractProductFragment, View.IOnClickListener
    {
        private View root;
        private IList<RealProductBean> mlProducts = new List<RealProductBean>();
        private GridView gridView;
        private BottomSheetAdapter adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            root = View.Inflate(Context, Resource.Layout.fragment_product, null);
            InitView();
            return root;
        }

        public override IList GetProductList(IList<MLProductVisionSearch> list)
        {
            return MLProductVisionSearchToTestBean(list);
        }

        private void InitView()
        {
            gridView = (GridView) root.FindViewById(Resource.Id.gv);
            gridView.SetNumColumns(2);
            adapter = new BottomSheetAdapter(mlProducts, Context);
            root.FindViewById(Resource.Id.img_close).SetOnClickListener(this);
            gridView.Adapter = adapter;
        }

        public override bool OnError(Java.Lang.Exception exception)
        {
            return false;
        }

        public override void OnResult(IList productList)
        {
            IList<RealProductBean> list = productList.Cast<RealProductBean>().ToList();
            if (null == productList)
            {
                return;
            }
            mlProducts.Clear();
            ((List<RealProductBean>)mlProducts).AddRange(list);
            adapter.NotifyDataSetChanged();
        }

        private IList MLProductVisionSearchToTestBean(IList<MLProductVisionSearch> list)
        {
            IList<RealProductBean> productBeans = new List<RealProductBean>();
            foreach (MLProductVisionSearch mlProductVisionSearch in list)
            {
                foreach (MLVisionSearchProduct product in mlProductVisionSearch.ProductList)
                {
                    RealProductBean productBean = new RealProductBean();
                    productBean.ImgUrl = product.ImageList.ElementAt(0).ImageId;
                    productBean.Id = product.ProductId;
                    productBean.Name = product.ProductId;
                    productBeans.Add(productBean);
                }
            }
            return (System.Collections.IList)productBeans;
        }

        public void OnClick(View v)
        {
            Activity.Finish();
        }
    }
}