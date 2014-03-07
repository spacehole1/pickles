﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NGenerics.DataStructures.Trees;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Autofac;
using PicklesDoc.Pickles.DirectoryCrawler;
using PicklesDoc.Pickles.DocumentationBuilders.JSON;
using PicklesDoc.Pickles.Test.Helpers;
using PicklesDoc.Pickles.TestFrameworks;

namespace PicklesDoc.Pickles.Test.Formatters.JSON
{
    public class when_creating_a_feature_with_meta_info_and_test_result_in_mstest_format : BaseFixture
    {
        public string Setup()
        {
            const string OUTPUT_DIRECTORY = @"JSONFeatureOutput";
            const string ROOT_PATH = @"Formatters\JSON\Features";
            const string testResultFilePath = @"Formatters\JSON\results-example-failing-and-pasing-mstest.trx";
            string filePath = MockFileSystem.Path.Combine(OUTPUT_DIRECTORY, JSONDocumentationBuilder.JsonFileName);

            var resultFile = RetrieveContentOfFileFromResources("PicklesDoc.Pickles.Test.Formatters.JSON.results-example-failing-and-pasing-mstest.trx");
            MockFileSystem.AddFile(testResultFilePath, resultFile);

            GeneralTree<INode> features = Container.Resolve<DirectoryTreeCrawler>().Crawl(ROOT_PATH);

            var outputDirectory = MockFileSystem.DirectoryInfo.FromDirectoryName(OUTPUT_DIRECTORY);
            if (!outputDirectory.Exists) outputDirectory.Create();

            var configuration = new Configuration
                                {
                                    OutputFolder = MockFileSystem.DirectoryInfo.FromDirectoryName(OUTPUT_DIRECTORY),
                                    DocumentationFormat = DocumentationFormat.JSON,
                                    TestResultsFile = MockFileSystem.FileInfo.FromFileName(testResultFilePath),
                                    TestResultsFormat = TestResultsFormat.MsTest
                                };

            ITestResults testResults = new MsTestResults(configuration);
            var jsonDocumentationBuilder = new JSONDocumentationBuilder(configuration, testResults, MockFileSystem);
            jsonDocumentationBuilder.Build(features);
            string content = MockFileSystem.File.ReadAllText(filePath);

            return content;
        }

        private static string RetrieveContentOfFileFromResources(string resourceName)
        {
            string resultFile;

            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            using (var reader = new StreamReader(manifestResourceStream))
            {
                resultFile = reader.ReadToEnd();
            }

            return resultFile;
        }

        [Test]
        public void it_should_contain_result_keys_in_the_json_document()
        {
            string content = this.Setup();

            content.AssertJsonContainsKey("Result");
        }

        [Test]
        public void it_should_indicate_WasSuccessful_is_true()
        {
            string content = this.Setup();

            JArray jsonArray = JArray.Parse(content);


            IEnumerable<JToken> featureJsonElement = from feat in jsonArray
                                                     where
                                                         feat["Feature"]["Name"].Value<string>().Equals(
                                                             "Two more scenarios transfering funds between accounts")
                                                     select feat;

            Assert.IsTrue(featureJsonElement.ElementAt(0)["Result"]["WasSuccessful"].Value<bool>());
        }

        [Test]
        public void it_should_indicate_WasSuccessful_is_true_for_the_other_success_feature()
        {
            string content = this.Setup();

            JArray jsonArray = JArray.Parse(content);


            IEnumerable<JToken> featureJsonElement = from feat in jsonArray
                                                     where
                                                         feat["Feature"]["Name"].Value<string>().Equals(
                                                             "Transfer funds between accounts")
                                                     select feat;

            Assert.IsTrue(featureJsonElement.ElementAt(0)["Result"]["WasSuccessful"].Value<bool>());
        }

        [Test]
        public void it_should_indicate_WasSuccessful_is_false_for_failing_scenario()
        {
            string content = this.Setup();

            JArray jsonArray = JArray.Parse(content);


            IEnumerable<JToken> featureJsonElement = from feat in jsonArray
                                                     where
                                                         feat["Feature"]["Name"].Value<string>().Equals(
                                                             "Transfer funds between accounts onc scenario and FAILING")
                                                     select feat;

            Assert.IsFalse(featureJsonElement.ElementAt(0)["Result"]["WasSuccessful"].Value<bool>());
        }


        [Test]
        public void it_should_indicate_WasSuccessful_is_false_for_another_failing_scenario()
        {
            string content = this.Setup();

            JArray jsonArray = JArray.Parse(content);


            IEnumerable<JToken> featureJsonElement = from feat in jsonArray
                                                     where
                                                         feat["Feature"]["Name"].Value<string>().Equals(
                                                             "Two more scenarios transfering funds between accounts - one failng and one succeding")
                                                     select feat;

            Assert.IsFalse(featureJsonElement.ElementAt(0)["Result"]["WasSuccessful"].Value<bool>());
        }


        [Test]
        public void it_should_contain_WasSuccessful_key_in_Json_document()
        {
            string content = this.Setup();

            JArray jsonArray = JArray.Parse(content);

            Assert.IsNotEmpty(jsonArray[0]["Result"]["WasSuccessful"].ToString());
        }


        [Test]
        public void it_should_WasSuccessful_false_for_feature_X_Json_document()
        {
            string content = this.Setup();

            JArray jsonArray = JArray.Parse(content);

            Assert.IsNotEmpty(jsonArray[0]["Result"]["WasSuccessful"].ToString());
        }
    }
}