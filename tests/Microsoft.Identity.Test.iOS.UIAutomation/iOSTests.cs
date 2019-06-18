// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Test.LabInfrastructure;
using NUnit.Framework;
using Microsoft.Identity.Test.UIAutomation.Infrastructure;
using Xamarin.UITest;
using System;
using System.Text;
using System.Collections.Generic;

//NOTICE! Inorder to run UI automation tests for xamarin locally, you may need to upgrade nunit to 3.0 and above for this project and the core ui Automation project.
//It is set to 2.6.4 because that is the maximum version that appcenter can support.
//There is an error in visual studio that can prevent the NUnit test framework from loading the test dll properly.
//Remember to return the version back to 2.6.4 before commiting to prevent appcenter from failing

namespace Test.Microsoft.Identity.UIAutomation
{
    /// <summary>
    /// Configures environment for core/iOS tests to run
    /// </summary>
    [TestFixture(Platform.iOS)]
    public class IOSMsalTests
    {
        private IApp _app;
        private readonly Platform _platform;
        private readonly ITestController _xamarinController = new IOSXamarinUiTestController();
        MobileTestHelper _mobileTestHelper;

        /// <summary>
        /// Initializes Xamarin UI tests
        /// </summary>
        /// <param name="platform">The platform where the tests will be performed</param>
        public IOSMsalTests(Platform platform)
        {
            _platform = platform;
        }

        /// <summary>
        /// Initializes app and test controller before each test
        /// </summary>
        [SetUp]
        public void InitializeBeforeTest()
        {
            _app = AppFactory.StartApp(_platform, "XForms.iOS");
            _xamarinController.Application = _app;
            _mobileTestHelper = new MobileTestHelper(_platform);
        }

        /// <summary>
        /// Test runner to run all tests, as test initialization is expensive.
        /// </summary>
        [Test]
        [Category("FastRun")]
        public void RunAllTests()
        {
            var tests = new List<Action>()
            {
                AcquireTokenTest,
                AcquireTokenSilentTest,
                //AcquireTokenADFSV3InteractiveFederatedTest,
                //AcquireTokenADFSV3InteractiveNonFederatedTest,
                //AcquireTokenADFSV4InteractiveFederatedTest,
                //AcquireTokenADFSV4InteractiveNonFederatedTest,
                //AcquireTokenADFSV2019InteractiveFederatedTest,
                //AcquireTokenADFSV2019InteractiveNonFederatedTest,

                // Ignored tests
                //B2CFacebookB2CLoginAcquireTokenTest,
                //B2CFacebookMicrosoftLoginAcquireTokenTest,
                //B2CGoogleB2CLoginAcquireTokenTest,
                //B2CGoogleMicrosofLoginAcquireTokenTest,
                //B2CLocalAccountAcquireTokenTest,
                //B2CFacebookEditPolicyAcquireTokenTest,
                B2CROPCLocalAccountAcquireTokenTest
            };

            var hasFailed = false;
            var stringBuilderMessage = new StringBuilder();

            foreach (Action test in tests)
            {
                try
                {
                    LogMessage($"Running test: {test.Method.Name}", stringBuilderMessage);
                    test();
                }
                catch (Exception ex)
                {
                    LogMessage($"Fail: {test.Method.Name}, Error: {ex.Message}", stringBuilderMessage);
                    hasFailed = true;
                }
                finally
                {
                    LogMessage($"Complete test: {test.Method.Name}", stringBuilderMessage);
                }
            }

            Assert.IsFalse(hasFailed, $"Test Failed. {stringBuilderMessage}");
        }

        private static void LogMessage(string message, StringBuilder stringBuilderMessage)
        {
            Console.WriteLine(message);
            stringBuilderMessage.AppendLine(message);
        }

        /// <summary>
        /// Runs through the standard acquire token flow, using the default app configured UiBehavior = Login
        /// </summary>
        [Test]
        public void AcquireTokenTest()
        {
            _mobileTestHelper.AcquireTokenInteractiveTestHelper(_xamarinController, LabUserHelper.GetDefaultUserAsync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Runs through the standard acquire token silent flow
        /// </summary>
        [Test]
        public void AcquireTokenSilentTest()
        {
            _mobileTestHelper.AcquireTokenSilentTestHelper(_xamarinController, LabUserHelper.GetDefaultUserAsync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Runs through the standard acquire token flow
        /// </summary>
        [Test]
        [Ignore("Current web element search implementation is unable to properly wait for select account elements on login page. Will be addressed in future updates.")]
        public void PromptBehaviorConsentSelectAccount()
        {
            var labResponse = LabUserHelper.GetDefaultUserAsync().GetAwaiter().GetResult();

            _mobileTestHelper.PromptBehaviorTestHelperWithConsent(_xamarinController, labResponse);
        }

        /// <summary>
        /// B2C acquire token with Facebook provider
        /// b2clogin.com authority
        /// with subsequent silent call
        /// </summary>
        [Test]
        [Ignore("Facebook does not allow automated test accounts. Tracking here: https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/issues/1026")]
        public void B2CFacebookB2CLoginAcquireTokenTest()
        {
            _mobileTestHelper.B2CFacebookAcquireTokenSilentTest(
                _xamarinController, 
                LabUserHelper.GetB2CFacebookAccountAsync().GetAwaiter().GetResult(), 
                true);
        }

        /// <summary>
        /// B2C acquire token with Facebook provider
        /// login.microsoftonline.com authority
        /// with subsequent silent call
        /// </summary>
        [Test]
        [Ignore("Facebook does not allow automated test accounts. Tracking here: https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/issues/1026")]
        public void B2CFacebookMicrosoftLoginAcquireTokenTest()
        {
            _mobileTestHelper.PerformB2CSelectProviderOnlyFlow(
                _xamarinController, 
                LabUserHelper.GetB2CFacebookAccountAsync().GetAwaiter().GetResult().User, 
                B2CIdentityProvider.Facebook, 
                false);
            _mobileTestHelper.B2CSilentFlowHelper(_xamarinController);
        }

        /// <summary>
        /// B2C acquire token with Facebook provider
        /// b2clogin.com authority
        /// call to edit profile authority with
        ///  UIBehavior none
        /// </summary>
        [Test]
        [Ignore("Facebook does not allow automated test accounts. Tracking here: https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/issues/1026")]
        public void B2CFacebookEditPolicyAcquireTokenTest()
        {
            _mobileTestHelper.PerformB2CSelectProviderOnlyFlow(
                _xamarinController, 
                LabUserHelper.GetB2CFacebookAccountAsync().GetAwaiter().GetResult().User, 
                B2CIdentityProvider.Facebook, 
                true);
            _mobileTestHelper.B2CSilentFlowHelper(_xamarinController);
            _mobileTestHelper.B2CFacebookEditPolicyAcquireTokenInteractiveTestHelper(_xamarinController);
        }

        /// <summary>
        /// B2C acquire token with Google provider
        /// b2clogin.com authority
        /// with subsequent silent call
        /// </summary>
        [Test]
        [Ignore("Google Auth does not support embedded webview from b2clogin.com authority. " +
            "App Center cannot run system browser tests yet, so this test can only be run in " +
            "system browser locally.")]
        public void B2CGoogleB2CLoginAcquireTokenTest()
        {
            _mobileTestHelper.B2CGoogleAcquireTokenSilentTest(
                _xamarinController,
                LabUserHelper.GetB2CGoogleAccountAsync().GetAwaiter().GetResult(),
                true);
        }

        /// <summary>
        /// B2C acquire token with Google provider
        /// login.microsoftonline.com authority
        /// with subsequent silent call
        /// </summary>
        [Test]
        [Ignore("Google Auth does not support embedded webview from b2clogin.com authority. " +
            "App Center cannot run system browser tests yet, so this test can only be run in " +
            "system browser locally.")]
        public void B2CGoogleMicrosoftLoginAcquireTokenTest()
        {
            _mobileTestHelper.B2CGoogleAcquireTokenSilentTest(
                _xamarinController, 
                LabUserHelper.GetB2CGoogleAccountAsync().GetAwaiter().GetResult(),
                false);
        }

        /// <summary>
        /// B2C acquire token with local account
        /// b2clogin.com authority
        /// and subsequent silent call
        /// </summary>
        [Test]
        [Ignore("UI element is different from Android")]
        public void B2CLocalAccountAcquireTokenTest()
        {
            _xamarinController.Tap("Done");
            _mobileTestHelper.B2CLocalAccountAcquireTokenSilentTest(
                _xamarinController,
                LabUserHelper.GetB2CLocalAccountAsync().GetAwaiter().GetResult(),
                true);
        }

        /// <summary>
        /// B2C ROPC acquire token with local account
        /// b2clogin.com authority
        /// </summary>
        [Test]
        public void B2CROPCLocalAccountAcquireTokenTest()
        {
            _xamarinController.Tap("Done");
            _mobileTestHelper.B2CAcquireTokenROPCTest(
                _xamarinController, 
                LabUserHelper.GetB2CLocalAccountAsync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Runs through the standard acquire token ADFSV4 Federated flow
        /// </summary>
        [Test]
        public void AcquireTokenADFSV4InteractiveFederatedTest()
        {
            _mobileTestHelper.AcquireTokenInteractiveTestHelper(
                _xamarinController,
                LabUserHelper.GetAdfsUserAsync(FederationProvider.AdfsV4).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Runs through the standard acquire token ADFSV2019 Federated flow
        /// </summary>
        [Test]
        public void AcquireTokenADFSV2019InteractiveFederatedTest()
        {
            _mobileTestHelper.AcquireTokenInteractiveTestHelper(
                _xamarinController,
                LabUserHelper.GetAdfsUserAsync(FederationProvider.ADFSv2019).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Runs through the standard acquire token ADFSV3 Federated flow
        /// </summary>
        [Test]
        public void AcquireTokenADFSV3InteractiveFederatedTest()
        {
            _mobileTestHelper.AcquireTokenInteractiveTestHelper(
                _xamarinController, 
                LabUserHelper.GetAdfsUserAsync(FederationProvider.AdfsV3).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Runs through the standard acquire token ADFSV4 Non-Federated flow
        /// </summary>
        [Test]
        public void AcquireTokenADFSV4InteractiveNonFederatedTest()
        {
            _mobileTestHelper.AcquireTokenInteractiveTestHelper(
                _xamarinController,
                LabUserHelper.GetAdfsUserAsync(FederationProvider.AdfsV4, false).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Runs through the standard acquire token ADFSV2019 Non-Federated flow
        /// </summary>
        [Test]
        public void AcquireTokenADFSV2019InteractiveNonFederatedTest()
        {
            _mobileTestHelper.AcquireTokenInteractiveTestHelper(
                _xamarinController, 
                LabUserHelper.GetAdfsUserAsync(FederationProvider.ADFSv2019, false).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Runs through the standard acquire token ADFSV3 Non-Federated flow
        /// </summary>
        [Test]
        public void AcquireTokenADFSV3InteractiveNonFederatedTest()
        {
            _mobileTestHelper.AcquireTokenInteractiveTestHelper(
                _xamarinController,
                LabUserHelper.GetAdfsUserAsync(FederationProvider.AdfsV4, false).GetAwaiter().GetResult());
        }
    }
}
