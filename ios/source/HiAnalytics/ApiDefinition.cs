/*
 * Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Foundation;
using Security;
using CoreFoundation;
using Network;
using ObjCRuntime;

namespace Huawei.Hms.Analytics
{
	// @interface HAReportPolicy : NSObject
	[BaseType(typeof(NSObject))]
	interface HAReportPolicy
	{
		// +(HAReportPolicy * _Nonnull)onScheduledTimePolicy:(NSInteger)seconds;
		[Static]
		[Export("onScheduledTimePolicy:")]
		HAReportPolicy OnScheduledTimePolicy(nint seconds);

		// +(HAReportPolicy * _Nonnull)onAppLaunchPolicy;
		[Static]
		[Export("onAppLaunchPolicy")]
		//[Verify(MethodToProperty)]
		HAReportPolicy OnAppLaunchPolicy { get; }

		// +(HAReportPolicy * _Nonnull)onMoveBackgroundPolicy;
		[Static]
		[Export("onMoveBackgroundPolicy")]
		//[Verify(MethodToProperty)]
		HAReportPolicy OnMoveBackgroundPolicy { get; }

		// +(HAReportPolicy * _Nonnull)onCacheThresholdPolicy:(NSInteger)threshold;
		[Static]
		[Export("onCacheThresholdPolicy:")]
		HAReportPolicy OnCacheThresholdPolicy(nint threshold);
	}

	// @interface HiAnalytics : NSObject
	[BaseType(typeof(NSObject))]
	interface HiAnalytics
	{
		// +(void)config;
		[Static]
		[Export("config")]
		//[DllImport("/externals/HiAnalytics.Framework", CallingConvention = CallingConvention.Cdecl)]
		void Config();

		// +(void)setReportPolicies:(NSArray<HAReportPolicy *> * _Nullable)policies;
		[Static]
		[Export("setReportPolicies:")]
		void SetReportPolicies([NullAllowed] HAReportPolicy[] policies);

		// +(void)onEvent:(NSString * _Nonnull)eventId setParams:(NSDictionary<NSString *,id> * _Nonnull)params;
		[Static]
		[Export("onEvent:setParams:")]
		void OnEvent(string eventId, NSDictionary<NSString, NSObject> @params);

		// +(void)setUserProfile:(NSString * _Nonnull)name setValue:(NSString * _Nonnull)value;
		[Static]
		[Export("setUserProfile:setValue:")]
		void SetUserProfile(string name, string value);

		// +(NSDictionary<NSString *,id> * _Nullable)userProfiles:(BOOL)preDefined;
		[Static]
		[Export("userProfiles:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> UserProfiles(bool preDefined);

		// +(void)setAnalyticsEnabled:(BOOL)enabled;
		[Static]
		[Export("setAnalyticsEnabled:")]
		void SetAnalyticsEnabled(bool enabled);

		// +(NSString * _Nonnull)AAID;
		[Static]
		[Export("AAID")]
		//[Verify(MethodToProperty)]
		string AAID { get; }

		// +(void)setUserId:(NSString * _Nullable)userId;
		[Static]
		[Export("setUserId:")]
		void SetUserId([NullAllowed] string userId);

		// +(void)setSessionDuration:(NSTimeInterval)milliseconds;
		[Static]
		[Export("setSessionDuration:")]
		void SetSessionDuration(double milliseconds);

		// +(void)clearCachedData;
		[Static]
		[Export("clearCachedData")]
		void ClearCachedData();
	}
}
