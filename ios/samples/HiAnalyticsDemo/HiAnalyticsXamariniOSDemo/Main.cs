using UIKit;

namespace HiAnalyticsXamariniOSDemo
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.


//            NSString* identifier = [dict stringForKey: @"identifier"];

//            if ([identifier length] != 0)
//                [postDatas setObject:identifier forKey:@"device_uid"];
//else
//                [postDatas setObject:@"" forKey: @"device_uid"];


            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}