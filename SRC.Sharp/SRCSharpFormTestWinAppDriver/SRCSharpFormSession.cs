
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using System;
namespace SRCSharpFormTestWinAppDriver
{
    public class SRCSharpFormSession
    {
        // XXX 4723 ローカルではなんか使っていたので
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:14723";
        // XXX いい感じのパス解決
        private const string SRCSPath = @"C:\Users\koudenpa\source\repos\src\SRC\SRC.Sharp\SRCSharpForm\bin\Debug\net5.0-windows\SRCSharpForm.exe";

        protected static WindowsDriver<WindowsElement> session;
        protected static WindowsElement mainForm;
        protected static WindowsElement messageForm;
        protected static WindowsElement listBoxForm;

        public static void Setup(TestContext context)
        {
            // XXX session があったら再構築するとか
            if (session == null)
            {
                var appiumOptions = new OpenQA.Selenium.Appium.AppiumOptions();
                appiumOptions.AddAdditionalCapability("app", SRCSPath);
                // XXX 引数でシナリオファイル受けて起動するようにするなど
                //appiumOptions .AddAdditionalCapability("appArguments", @"MyTestFile.txt");
                //appiumOptions .AddAdditionalCapability("appWorkingDir", @"C:\MyTestFolder\");

                session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appiumOptions);
                Assert.IsNotNull(session);
                Assert.IsNotNull(session.SessionId);

                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);

                // Close open file dialog
                session.FindElementByName("キャンセル")?.Click();
                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0.1);
                Assert.AreEqual("SRC#Form", session.Title);

                //session.FindElementByName("閉じる").Click();

                //mainForm = session.FindElementByClassName("Edit");
                //Assert.IsNotNull(mainForm);
            }
        }

        public static void TearDown()
        {
            if (session != null)
            {
                session.Close();

                try
                {
                    // 終了確認ダイアログ
                    session.FindElementByName("OK").Click();
                }
                catch { }

                session.Quit();
                session = null;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }
    }
}
