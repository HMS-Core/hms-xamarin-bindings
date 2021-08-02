/*
*       Copyright 2020-2021. Huawei. Technologies Co., Ltd. All rights reserved.

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
using System.Collections.Generic;
using System.Linq;

namespace HiAnalyticsXamarinAndroidDemo.Data
{
    public static class QuizData
    {
        public static List<QuestionModel> Questions { get; set; }
        public static bool IsQuizDone { get; set; }
        public static int Result { get; set; } = 0;
        public static QuestionModel CurrentQuestion { get; set; }

        public static void InitQuiz()
        {
            Result = 0;
            ResetQuestions();
            CurrentQuestion = Questions.FirstOrDefault();
            IsQuizDone = false;

        }

        public static void ResetQuestions()
        {
            Questions = new List<QuestionModel>();
            Questions.AddRange(new List<QuestionModel> {
                new QuestionModel {Id=1, Text="The largest planet in the solar system is Jupiter?" ,Answer="True"},
                new QuestionModel {Id=2, Text="The first Olympic Games were held in Athens, Greece?" ,Answer="True"},
                new QuestionModel {Id=3, Text="The violin has 6 strings?",Answer="False" },
                new QuestionModel {Id=4, Text="Flying bats belong to birds?",Answer="False" },
                new QuestionModel {Id=5, Text="Sound spreads faster in the water than in the air?",Answer="True" },
            });
        }

        public class QuestionModel
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public string Answer { get; set; }
            public bool Check { get; set; }
        }
    }
}