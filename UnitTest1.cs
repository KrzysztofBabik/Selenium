using OpenQA.Selenium;
using NUnit;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using static System.Net.WebRequestMethods;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;

namespace ExamplesOfSeleniumTesting
{
    public class Tests
    {
        IWebDriver driver;
        WebDriverWait wait;
        private string ytUrl = "https://www.youtube.com/";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            driver.Navigate().GoToUrl(ytUrl);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            YouTubeCookieAcceptance();
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

        [Test]
        public void YouTubeVideoPlayCheck()
        {
            driver.Navigate().GoToUrl(ytUrl);

            wait.Until(ExpectedConditions.ElementExists(By.Name("search_query")));
            IWebElement searchBox = driver.FindElement(By.Name("search_query"));

            searchBox.SendKeys("selenium learning");
            searchBox.SendKeys(Keys.Enter);

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("ytd-video-renderer")));

            IWebElement firstVideo = driver.FindElement(By.CssSelector("ytd-video-renderer"));
            string videoDesc = firstVideo.Text;
            firstVideo.Click();
            
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.html5-video-container")));

            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".ytp-play-button")));

            IWebElement playButton = driver.FindElement(By.CssSelector(".ytp-play-button"));
            playButton.Click();

            wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id=\"title\"]/h1/yt-formatted-string")));
            IWebElement videoTitle = driver.FindElement(By.XPath("//*[@id=\"title\"]/h1/yt-formatted-string"));

            string title = videoTitle.Text;

            bool containsSubstring = false;
            if (videoDesc.Contains(title))
            {
                containsSubstring = true;
                Console.WriteLine($"Correct title!");
            }
            string[] words = videoDesc.Split(' ');

            Assert.IsTrue(containsSubstring, $"String '{title}' not found in '{videoDesc}'");

            Console.WriteLine($"The video has started.");
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