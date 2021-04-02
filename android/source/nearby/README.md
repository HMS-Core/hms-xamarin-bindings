<p align="center">
  <h1 align="center">HMS Nearby Serivce Xamarin Android Plugin</h1>
</p>



<p align="center">
  <a href="https://www.nuget.org/packages/Huawei.Hms.Nearby/"><img src="https://img.shields.io/nuget/dt/Huawei.Hms.Nearby?label=Downloads&color=%23007EC6&style=for-the-badge"alt="downloads"></a>
  <a href="https://www.nuget.org/packages/Huawei.Hms.Nearby/"><img src="https://img.shields.io/nuget/v/Huawei.Hms.Nearby?color=%23ed2a1c&style=for-the-badge" alt="Nuget version"></a>
  <a href="/LICENCE"><img src="https://img.shields.io/badge/License-Apache%202.0-blue.svg?color=%3bcc62&style=for-the-badge" alt="Licence"></a>
</p>

----

Data Communication allows apps to easily discover nearby devices and set up communication with them using technologies such as Bluetooth and Wi-Fi. The service provides Nearby Connection and Nearby Message APIs.

- Nearby Connection

  Discovers devices and sets up secure communication channels with them without connecting to the Internet and transfers byte arrays, files, and streams to them; supports seamless nearby interactions, such as multi-player gaming, real-time collaboration, resource broadcasting, and content sharing.

- Nearby Message

  Allows message publishing and subscription between nearby devices that are connected to the Internet. A subscriber (app) can obtain the message content from the cloud service based on the sharing code broadcast by a publisher (beacon or another app).

The Xamarin SDK wraps the Android SDK with Managed Callable Wrappers through the usage of Android Bindings Library projects. It provides the same APIs as the native SDK.

[> Learn More](https://developer.huawei.com/consumer/en/doc/development/HMS-Plugin-Guides/introduction-0000001062477568)

## Installation

**Huawei.Hms.Nearby** is available on [NuGet](https://www.nuget.org/packages/Huawei.Hms.Nearby). 

In the Solution Explorer panel, right click on the solution name and select Manage NuGet Packages. Search for **Huawei.Hms.Nearby** and install the package into your Xamarin.Android projects.

You can change the `Version` option to specify a [preview version](https://www.nuget.org/packages/Huawei.Hms.Nearby) to install.

## Documentation

- [Quick Start](https://developer.huawei.com/consumer/en/doc/development/HMS-Plugin-Guides/preparedevenv-0000001063115554)
- [Reference](https://developer.huawei.com/consumer/en/doc/development/HMS-Plugin-References-V1/overview-0000001062363591-V1)

## Supported Environments

- Android 5.0 (API level 21) and later versions

## Sample Project

You can find the demo application demonstrating how to use the Nearby SDK on the [HUAWEI Developer website](https://developer.huawei.com/consumer/en/doc/overview/HMS-Core-Plugin).

## Questions or Issues

If you have questions about how to use HMS samples, try the following options:
- [Stack Overflow](https://stackoverflow.com/questions/tagged/huawei-mobile-services) is the best place for any programming questions. Be sure to tag your question with 
**huawei-mobile-services**.
- [Huawei Developer Forum](https://forums.developer.huawei.com/forumPortal/en/home?fid=0101187876626530001) HMS Core Module is great for general questions, or seeking recommendations and opinions.
- [Huawei Developer Docs](https://developer.huawei.com/consumer/en/doc/overview/HMS-Core-Plugin) is place to official documentation for all HMS Core Kits, you can find detailed documentations in there.

## License

HMS Nearby Service Kit Xamarin Android Plugin is licensed under [Apache 2.0 license](LICENCE)