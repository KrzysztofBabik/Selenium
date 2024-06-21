using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamplesOfSeleniumTesting
{
    public class TheInternetTests
    {
        IWebDriver driver;
        WebDriverWait wait;
        private string url = "https://the-internet.herokuapp.com/";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void AddRemoveElement()
        {
            driver.Navigate().GoToUrl(url + "add_remove_elements/");

            wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id=\"content\"]/div/button")));
            IWebElement addButton = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/button"));

            addButton.Click();

            wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id=\"elements\"]/button")));
            By elementLocator = By.XPath("//*[@id=\"elements\"]/button");
            IWebElement deleteButton = driver.FindElement(elementLocator);

            Assert.That(deleteButton.Displayed);
            
            deleteButton.Click();

            bool isDeleted = wait.Until(driver => {
                try
                {
                    driver.FindElement(elementLocator);
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return true;
                }
            });

            Assert.IsTrue(isDeleted);
        }

        [Test]
        public void AddRemoveMultipleElements()
        {
            driver.Navigate().GoToUrl(url + "add_remove_elements/");

            wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id=\"content\"]/div/button")));
            IWebElement addButton = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/button"));

            int numberOfButtons = 4;
            for (int i = 0; i < numberOfButtons; i++)
            {
                addButton.Click();
                Thread.Sleep(200);
            }

            wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id=\"elements\"]/button")));
            IReadOnlyCollection<IWebElement> deleteButtons = driver.FindElements(By.XPath("//*[@id=\"elements\"]/button"));

            Assert.That(deleteButtons.Count, Is.EqualTo(4));

            for (int i = numberOfButtons; i > 0; i--)
            {
                By elementLocator = By.XPath("//*[@id=\"elements\"]/button[" + i + "]");
                IWebElement deleteButton = driver.FindElement(elementLocator);

                deleteButton.Click();

                deleteButtons = driver.FindElements(By.XPath("//*[@id=\"elements\"]/button"));
                Assert.That(deleteButtons.Count, Is.EqualTo(i - 1));

                Thread.Sleep(200);
            }
        }

        [Test]
        public void BasicAuth()
        {
            string userAndPass = "admin";
            string authUrl = $"https://{userAndPass}:{userAndPass}@the-internet.herokuapp.com/basic_auth";

            driver.Navigate().GoToUrl(authUrl);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".example h3")));

            IWebElement successMessage = driver.FindElement(By.CssSelector(".example h3"));
            Assert.That(successMessage.Text, Is.EqualTo("Basic Auth"));

            IWebElement welcomeMessage = driver.FindElement(By.CssSelector(".example p"));
            Assert.IsTrue(welcomeMessage.Text.Contains("Congratulations! You must have the proper credentials."));
        }

        #region Private methods

        #endregion

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
