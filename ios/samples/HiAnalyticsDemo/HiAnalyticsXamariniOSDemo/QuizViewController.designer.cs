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
    [Register ("QuizViewController")]
    partial class QuizViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnFalse { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTrue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblQuestion { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblQuestionNumber { get; set; }

        [Action ("BtnNext_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnNext_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnFalse != null) {
                btnFalse.Dispose ();
                btnFalse = null;
            }

            if (btnTrue != null) {
                btnTrue.Dispose ();
                btnTrue = null;
            }

            if (lblQuestion != null) {
                lblQuestion.Dispose ();
                lblQuestion = null;
            }

            if (lblQuestionNumber != null) {
                lblQuestionNumber.Dispose ();
                lblQuestionNumber = null;
            }
        }
    }
}