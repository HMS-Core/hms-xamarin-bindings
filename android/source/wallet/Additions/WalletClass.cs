/*Copyright 2020.Huawei Technologies Co., Ltd.All rights reserved.

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
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuaweiTask = Huawei.Hmf.Tasks.Task;

namespace Huawei.Hms.Wallet
{
    public partial class WalletPassClient
    {
		public async Task<AutoResolvableForegroundIntentResult> CreateWalletPassAsync(CreateWalletPassRequest request)
			=> await CreateWalletPass(request).CastTask<AutoResolvableForegroundIntentResult>();
	}

	internal static class HuaweiTaskExtensions
	{
		public static Task CastTask(this HuaweiTask HuaweiTask)
		{
			var tcs = new TaskCompletionSource<bool>();

			HuaweiTask.AddOnCompleteListener(new MyCompleteListener(
				t =>
				{
					if (t.Exception == null)
						tcs.TrySetResult(false);
					else
						tcs.TrySetException(t.Exception);
				}
			));

			return tcs.Task;
		}

		public static Task<TResult> CastTask<TResult>(this HuaweiTask HuaweiTask)
			where TResult : Java.Lang.Object
		{
			var tcs = new TaskCompletionSource<TResult>();

			HuaweiTask.AddOnCompleteListener(new MyCompleteListener(
				t =>
				{
					if (t.Exception == null)
						tcs.TrySetResult(t.Result.JavaCast<TResult>());
					else
						tcs.TrySetException(t.Exception);
				}));

			return tcs.Task;
		}

		class MyCompleteListener : Java.Lang.Object, Huawei.Hmf.Tasks.IOnCompleteListener
		{
			public MyCompleteListener(Action<HuaweiTask> onComplete)
				=> OnCompleteHandler = onComplete;

			public Action<HuaweiTask> OnCompleteHandler { get; }

			public void OnComplete(HuaweiTask task)
				=> OnCompleteHandler?.Invoke(task);
		}

	}
}