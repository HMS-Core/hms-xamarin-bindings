// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace HiAnalyticsXamariniOSDemo
{
    [Register ("ResultViewController")]
    partial class ResultViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSubmit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblScore { get; set; }

        [Action ("SubmitScore_Clicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SubmitScore_Clicked (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnSubmit != null) {
                btnSubmit.Dispose ();
                btnSubmit = null;
            }

            if (lblScore != null) {
                lblScore.Dispose ();
                lblScore = null;
            }
        }
    }
}