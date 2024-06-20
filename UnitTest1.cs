using OpenQA.Selenium;
using NUnit;
using OpenQA.Selenium.Chrome;
using static System.Net.WebRequestMethods;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;

namespace ExamplesOfSeleniumTesting
{
    public class Tests
    {
        IWebDriver driver;
        private string ytUrl = "https://www.youtube.com/";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void URLAndHeader()
        {
            driver.Navigate().GoToUrl(ytUrl);
            Assert.That(driver.Url, Is.EqualTo(ytUrl));
            Assert.That(driver.Title, Is.EqualTo("YouTube"));
        }

        [Test]
        public void YouTubeSearch()
        {
            driver.Navigate().GoToUrl(ytUrl);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            YouTubeCookieAcceptance();

            wait.Until(ExpectedConditions.ElementExists(By.Name("search_query")));
            IWebElement searchBox = driver.FindElement(By.Name("search_query"));

            searchBox.SendKeys("selenium learning");
            searchBox.SendKeys(Keys.Enter);

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("ytd-video-renderer")));
            IReadOnlyCollection<IWebElement> searchResults = driver.FindElements(By.CssSelector("ytd-video-renderer"));

            Assert.Greater(searchResults.Count, 0);

            if (searchResults.Count > 0)
            {
                Console.WriteLine($"Videos found.");
            }
            else
            {
                Console.WriteLine($"No videos found.");
            }

            Assert.That(driver.Title, Is.EqualTo("selenium learning - YouTube"));
        }

        #region Private methods

        private void YouTubeCookieAcceptance()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                IWebElement acceptCookiesButton = wait.Until(drv => drv.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button")));
                acceptCookiesButton.Click();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Cookie acceptance button not found. Moving on without acceptance.");
            }
        }

        #endregion

        [TearDown]
        public void TearDown() 
        { 
            driver.Quit();
        }
    }
}