using Android.App;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuaweiTask = Huawei.Hmf.Tasks.Task;

namespace Huawei.Hms.Contactshield
{
	public partial class ContactShieldEngine
	{
		public async Task<IList<PeriodicKey>> GetPeriodicKeyAsync()
			=> await PeriodicKey.CastTask<JavaList<PeriodicKey>>();
		public async Task ClearDataAsync()
			=> await ClearData().CastTask();
		public async Task<IList<ContactDetail>> GetContactDetailAsync(string token)
			=> await GetContactDetail(token).CastTask<JavaList<ContactDetail>>();
		public async Task<ContactSketch> GetContactSketchAsync(string token)
			=> await GetContactSketch(token).CastTask<ContactSketch>();
		public async Task<IList<ContactWindow>> GetContactWindowAsync(string token)
			=> await GetContactWindow(token).CastTask<JavaList<ContactWindow>>();
		public async Task<bool> IsContactShieldRunningAsync()
			=> (bool)await IsContactShieldRunning().CastTask<Java.Lang.Boolean>();
		public async Task PutSharedKeyFilesAsync(IList<Java.IO.File> files, Huawei.Hms.Contactshield.DiagnosisConfiguration config, string token)
			=> await PutSharedKeyFiles(files, config, token).CastTask();
		public async Task PutSharedKeyFilesAsync(PendingIntent pendingIntent, IList<Java.IO.File> files, Huawei.Hms.Contactshield.DiagnosisConfiguration config, string token)
			=> await PutSharedKeyFiles(pendingIntent, files, config, token).CastTask();
		public async Task StartContactShieldAsync(PendingIntent pendingIntent, Huawei.Hms.Contactshield.ContactShieldSetting setting)
			=> await StartContactShield(pendingIntent, setting).CastTask();
		public async Task StartContactShieldAsync(Huawei.Hms.Contactshield.ContactShieldSetting setting)
			=> await StartContactShield(setting).CastTask();
		public async Task StartContactShieldNoPersistentAsync(Huawei.Hms.Contactshield.ContactShieldSetting setting)
			=> await StartContactShieldNoPersistent(setting).CastTask();
		public async Task StopContactShieldAsync()
			=> await StopContactShield().CastTask();
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