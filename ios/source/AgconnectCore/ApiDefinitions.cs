/*
    Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.
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

using ObjCRuntime;
using Foundation;
using UIKit;

namespace Huawei.Agconnect.AgconnectCore
{

    // @protocol AGCCredentialsProvider
    [Protocol]
    interface AGCCredentialsProvider
    {
        // @required -(id)getToken;
        [Abstract]
        [Export("getToken")]
        //[Verify(MethodToProperty)]
        NSObject Token { get; }

        // @required -(id)getToken:(id)isForceRefresh;
        [Abstract]
        [Export("getToken:")]
        NSObject GetToken(NSObject isForceRefresh);
    }

    // @protocol AGCEndpointService

    [Protocol]
    interface AGCEndpointService
    {
        // @required -(id)getEndpointDomain:(id)forceRefresh;
        [Abstract]
        [Export("getEndpointDomain:")]
        NSObject GetEndpointDomain(NSObject forceRefresh);
    }
    // @interface AGCServicesConfig : NSObject <NSCopying>
    [BaseType(typeof(NSObject))]
    interface AGCServicesConfig : INSCopying
    {
        // @property (nonatomic, strong) NSString * _Nullable productId;
        [NullAllowed, Export("productId", ArgumentSemantic.Strong)]
        string ProductId { get; set; }

        // @property (nonatomic, strong) NSString * _Nullable appId;
        [NullAllowed, Export("appId", ArgumentSemantic.Strong)]
        string AppId { get; set; }

        // @property (nonatomic, strong) NSString * _Nullable cpId;
        [NullAllowed, Export("cpId", ArgumentSemantic.Strong)]
        string CpId { get; set; }

        // @property (nonatomic, strong) NSString * _Nullable clientId;
        [NullAllowed, Export("clientId", ArgumentSemantic.Strong)]
        string ClientId { get; set; }

        // @property (nonatomic, strong) NSString * _Nullable clientSecret;
        [NullAllowed, Export("clientSecret", ArgumentSemantic.Strong)]
        string ClientSecret { get; set; }

        // @property (nonatomic, strong) NSString * _Nullable apiKey;
        [NullAllowed, Export("apiKey", ArgumentSemantic.Strong)]
        string ApiKey { get; set; }

        // -(instancetype _Nonnull)initWithDictionary:(NSDictionary * _Nonnull)dictionary;
        [Export("initWithDictionary:")]
        IntPtr Constructor(NSDictionary dictionary);

        // -(NSString * _Nullable)getString:(NSString * _Nonnull)path;
        [Export("getString:")]
        [return: NullAllowed]
        string GetString(string path);

        // -(void)setValue:(NSString * _Nonnull)value forPath:(NSString * _Nonnull)path;
        [Export("setValue:forPath:")]
        void SetValue(string value, string path);

        // -(void)setCustomCredentialsProvider:(void (^ _Nonnull)(int))credentialsProvider;
        [Export("setCustomCredentialsProvider:")]
        void SetCustomCredentialsProvider(Action<int> credentialsProvider);

        // -(void)setCustomAuthProvider:(void (^ _Nonnull)(int))authProvider;
        [Export("setCustomAuthProvider:")]
        void SetCustomAuthProvider(Action<int> authProvider);

        // +(instancetype _Nonnull)sharedInstance __attribute__((deprecated("Please use [AGCInstance sharedInstance].config")));
        [Static]
        [Export("sharedInstance")]
        AGCServicesConfig SharedInstance();

        // -(BOOL)getBoolean:(NSString * _Nonnull)path __attribute__((deprecated("")));
        [Export("getBoolean:")]
        bool GetBoolean(string path);

        // -(BOOL)getBoolean:(NSString * _Nonnull)path withDefault:(BOOL)def __attribute__((deprecated("")));
        [Export("getBoolean:withDefault:")]
        bool GetBoolean(string path, bool def);

        // -(NSInteger)getInt:(NSString * _Nonnull)path __attribute__((deprecated("")));
        [Export("getInt:")]
        nint GetInt(string path);

        // -(NSInteger)getInt:(NSString * _Nonnull)path withDefault:(NSInteger)def __attribute__((deprecated("")));
        [Export("getInt:withDefault:")]
        nint GetInt(string path, nint def);

        // -(NSString * _Nonnull)getString:(NSString * _Nonnull)path withDefault:(NSString * _Nullable)def __attribute__((deprecated("")));
        [Export("getString:withDefault:")]
        string GetString(string path, [NullAllowed] string def);
    }

    // @interface AGCInstance : NSObject
    [BaseType(typeof(NSObject))]
    interface AGCInstance
    {
        // @property (readonly, nonatomic, strong) AGCServicesConfig * _Nonnull config;
        [Export("config", ArgumentSemantic.Strong)]
        AGCServicesConfig Config { get; }

        // +(void)startUp;
        [Static]
        [Export("startUp")]
        void StartUp();

        // +(void)startUp:(AGCServicesConfig * _Nonnull)config;
        [Static]
        [Export("startUp:")]
        void StartUp(AGCServicesConfig config);

        // +(instancetype _Nonnull)sharedInstance;
        [Static]
        [Export("sharedInstance")]
        AGCInstance SharedInstance();

        // -(id _Nullable)getService:(Protocol * _Nonnull)protocol;
        [Export("getService:")]
        [return: NullAllowed]
        NSObject GetService(Protocol protocol);
    }


    // @interface AGCInstanceId : NSObject
    [BaseType(typeof(NSObject))]
    interface AGCInstanceId
    {
        // @property (readonly, nonatomic) NSString * _Nullable uuid;
        [NullAllowed, Export("uuid")]
        string Uuid { get; }

        // @property (readonly, nonatomic) NSDate * _Nullable createTime;
        [NullAllowed, Export("createTime")]
        NSDate CreateTime { get; }

        // +(instancetype _Nonnull)sharedInstance;
        [Static]
        [Export("sharedInstance")]
        AGCInstanceId SharedInstance();

        // -(id)getUUID;
        [Export("getUUID")]
        NSObject UUID { get; }

        // -(void)deleteUUID;
        [Export("deleteUUID")]
        void DeleteUUID();
    }


    // typedef void (^AGCLoggerCallback)(AGCLoggerLevel, NSString * _Nonnull);
    delegate void AGCLoggerCallback(AGCLoggerLevel arg0, string arg1);

    // @interface AGCLogger : NSObject
    [BaseType(typeof(NSObject))]
    interface AGCLogger
    {
        // +(void)enableLog:(BOOL)enable;
        [Static]
        [Export("enableLog:")]
        void EnableLog(bool enable);

        // +(void)resetLogLevel:(AGCLoggerLevel)level;
        [Static]
        [Export("resetLogLevel:")]
        void ResetLogLevel(AGCLoggerLevel level);
    }

    // @interface AGCService : NSObject
    [BaseType(typeof(NSObject))]
    interface AGCService
    {
        // @property (readonly, nonatomic) Class _Nonnull service;
        [Export("service")]
        Class Service { get; }

        // @property (readonly, nonatomic) Protocol * _Nonnull protocol;
        [Export("protocol")]
        Protocol Protocol { get; }

        // @property (readonly, nonatomic) BOOL isSingleInstance;
        [Export("isSingleInstance")]
        bool IsSingleInstance { get; }

        // @property (readonly, nonatomic) id  _Nonnull (^ _Nullable)(void) instanceInitFunction;
        [NullAllowed, Export("instanceInitFunction")]
        Func<NSObject> InstanceInitFunction { get; }

        // -(instancetype _Nonnull)initWithService:(Class _Nonnull)service protocol:(Protocol * _Nonnull)protocol;
        [Export("initWithService:protocol:")]
        IntPtr Constructor(Class service, Protocol protocol);

        // -(instancetype _Nonnull)initWithService:(Class _Nonnull)service protocol:(Protocol * _Nonnull)protocol isSingleInstance:(BOOL)isSingle;
        [Export("initWithService:protocol:isSingleInstance:")]
        IntPtr Constructor(Class service, Protocol protocol, bool isSingle);

        // -(instancetype _Nonnull)initWithService:(Class _Nonnull)service protocol:(Protocol * _Nonnull)protocol isSingleInstance:(BOOL)isSingle initFunction:(id  _Nonnull (^ _Nullable)(void))initFunction;
        [Export("initWithService:protocol:isSingleInstance:initFunction:")]
        IntPtr Constructor(Class service, Protocol protocol, bool isSingle, [NullAllowed] Func<NSObject> initFunction);
    }

    // @protocol AGCServiceRegistrar <NSObject>
    [Protocol]
    [BaseType(typeof(NSObject))]
    interface AGCServiceRegistrar
    {
        // @optional +(NSArray<AGCService *> * _Nonnull)getService;
        [Static]
        [Export("getService")]
        //[Verify(MethodToProperty)]
        AGCService[] Service { get; }

        // @optional +(void)startUp;
        [Static]
        [Export("startUp")]
        void StartUp();

        // @optional +(NSString * _Nonnull)sdkVersion;
        [Static]
        [Export("sdkVersion")]
        //[Verify(MethodToProperty)]
        string SdkVersion { get; }

        // @optional +(NSString * _Nonnull)sdkName;
        [Static]
        [Export("sdkName")]
        //[Verify(MethodToProperty)]
        string SdkName { get; }
    }

    // @interface AGCServiceRepository : NSObject
    [BaseType(typeof(NSObject))]
    interface AGCServiceRepository
    {
        // +(void)registryService:(Class<AGCServiceRegistrar> _Nonnull)serviceClass;
        [Static]
        [Export("registryService:")]
        void RegistryService(AGCServiceRegistrar serviceClass);
    }

    // @interface AGCToken : NSObject
    [BaseType(typeof(NSObject))]
    interface AGCToken
    {
        // @property (readonly, nonatomic) long expiration;
        [Export("expiration")]
        nint Expiration { get; }

        // @property (readonly, nonatomic) NSString * _Nullable tokenString;
        [NullAllowed, Export("tokenString")]
        string TokenString { get; }

        // -(instancetype _Nonnull)initWithToken:(NSString * _Nonnull)token expiration:(long)expiration;
        [Export("initWithToken:expiration:")]
        IntPtr Constructor(string token, nint expiration);
    }

    // @interface AGCUserDefaults : NSObject
    [BaseType(typeof(NSObject))]
    interface AGCUserDefaults
    {
        // +(instancetype _Nonnull)standard;
        [Static]
        [Export("standard")]
        AGCUserDefaults Standard();

        // -(instancetype _Nonnull)initWithName:(NSString * _Nonnull)name;
        [Export("initWithName:")]
        IntPtr Constructor(string name);

        // -(BOOL)saveObject:(id _Nullable)value forKey:(NSString * _Nonnull)key;
        [Export("saveObject:forKey:")]
        bool SaveObject([NullAllowed] NSObject value, string key);

        // -(BOOL)saveCryptoObject:(id _Nullable)value forKey:(NSString * _Nonnull)key;
        [Export("saveCryptoObject:forKey:")]
        bool SaveCryptoObject([NullAllowed] NSObject value, string key);

        // -(id _Nullable)objectForKey:(NSString * _Nonnull)key;
        [Export("objectForKey:")]
        [return: NullAllowed]
        NSObject ObjectForKey(string key);

        // -(id _Nullable)objectCryptoForKey:(NSString * _Nonnull)key;
        [Export("objectCryptoForKey:")]
        [return: NullAllowed]
        NSObject ObjectCryptoForKey(string key);

        // -(void)removeObjectForKey:(NSString * _Nonnull)key;
        [Export("removeObjectForKey:")]
        void RemoveObjectForKey(string key);
    }

    // @protocol AGCAuthProvider <NSObject>

    [Protocol]
    [BaseType(typeof(NSObject))]
    interface AGCAuthProvider
    {
        // @required -(id)getToken;
        [Abstract]
        [Export("getToken")]
        NSObject Token { get; }

        // @required -(id)getToken:(BOOL)isForceRefresh;
        [Abstract]
        [Export("getToken:")]
        NSObject GetToken(bool isForceRefresh);

        // @required -(NSString * _Nullable)getUid;
        [Abstract]
        [NullAllowed, Export("getUid")]
        string Uid { get; }
    }

           
    // audit-objc-generics: @interface HMFTask<__covariant TResult> : NSObject
    [BaseType(typeof(NSObject))]
    interface HMFTask<TResult> where TResult : NSObject
    {
        // @property (readonly, nonatomic) BOOL isComplete;
        [Export("isComplete")]
        bool IsComplete { get; }

        // @property (readonly, nonatomic) BOOL isSuccessful;
        [Export("isSuccessful")]
        bool IsSuccessful { get; }

        // @property (readonly, nonatomic) BOOL isCanceled;
        [Export("isCanceled")]
        bool IsCanceled { get; }

        // @property (readonly, nonatomic) TResult _Nullable result;
        [NullAllowed, Export("result")]
        NSObject Result { get; }

        // @property (readonly, nonatomic) NSError * _Nullable error;
        [NullAllowed, Export("error")]
        NSError Error { get; }

        // -(HMFTask<TResult> * _Nonnull)addOnSuccessCallback:(void (^ _Nonnull)(TResult _Nullable))successCallback __attribute__((swift_name("onSuccess(callback:)")));
        [Export("addOnSuccessCallback:")]
        [Async]
        void AddOnSuccessCallback(Action<NSObject> successCallback);

        // -(HMFTask<TResult> * _Nonnull)addOnSuccessAt:(id<HMFExecutor> _Nonnull)executor callback:(void (^ _Nonnull)(TResult _Nullable))successCallback __attribute__((swift_name("onSuccess(at:callback:)")));
        [Export("addOnSuccessAt:callback:")]
        [Async]
        HMFTask<TResult> AddOnSuccessAt(HMFExecutor executor, Action<NSObject> successCallback);

        // -(HMFTask<TResult> * _Nonnull)addOnFailureCallback:(void (^ _Nonnull)(NSError * _Nonnull))failureCallback __attribute__((swift_name("onFailure(callback:)")));
        [Export("addOnFailureCallback:")]
        [Async]
        HMFTask<TResult> AddOnFailureCallback(Action<NSError> failureCallback);

        // -(HMFTask<TResult> * _Nonnull)addOnFailureAt:(id<HMFExecutor> _Nonnull)executor callback:(void (^ _Nonnull)(NSError * _Nonnull))failureCallback __attribute__((swift_name("onFailure(at:callback:)")));
        [Export("addOnFailureAt:callback:")]
        [Async]
        HMFTask<TResult> AddOnFailureAt(HMFExecutor executor, Action<NSError> failureCallback);

        // -(HMFTask<TResult> * _Nonnull)addOnCompleteCallback:(void (^ _Nonnull)(HMFTask<TResult> * _Nonnull))completeCallback __attribute__((swift_name("onComplete(callback:)")));
        [Export("addOnCompleteCallback:")]
        [Async]
        HMFTask<TResult> AddOnCompleteCallback(Action<NSObject> completeCallback);

        // -(HMFTask<TResult> * _Nonnull)addOnCompleteAt:(id<HMFExecutor> _Nonnull)executor callback:(void (^ _Nonnull)(HMFTask<TResult> * _Nonnull))completeCallback __attribute__((swift_name("onComplete(at:callback:)")));
        [Export("addOnCompleteAt:callback:")]
        [Async]
        HMFTask<TResult> AddOnCompleteAt(HMFExecutor executor, Action<NSObject> completeCallback);

        // -(HMFTask<TResult> * _Nonnull)addOnCanceledCallback:(void (^ _Nonnull)(void))cancelCallback __attribute__((swift_name("onCancel(callback:)")));
        [Export("addOnCanceledCallback:")]
        HMFTask<TResult> AddOnCanceledCallback(Action cancelCallback);

        // -(HMFTask<TResult> * _Nonnull)addOnCanceledAt:(id<HMFExecutor> _Nonnull)executor callback:(void (^ _Nonnull)(void))cancelCallback __attribute__((swift_name("onCancel(at:callback:)")));
        [Export("addOnCanceledAt:callback:")]
        HMFTask<TResult> AddOnCanceledAt(HMFExecutor executor, Action cancelCallback);

        // -(BOOL)cancel;
        [Export("cancel")]
        bool Cancel { get; }
    }

    // typedef void (^HMFRunnable)();
    delegate void HMFRunnable();

    // @protocol HMFExecutor <NSObject>
    [Protocol]
    [BaseType(typeof(NSObject))]
    interface HMFExecutor
    {
        // @required -(void)run:(HMFRunnable _Nonnull)runnable;
        [Abstract]
        [Export("run:")]
        void Run(HMFRunnable runnable);
    }
}

