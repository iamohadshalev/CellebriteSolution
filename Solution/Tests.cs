using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using NUnit;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using Pages;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture(typeof(ChromeDriver)), Order(1), Timeout(15000)]
    [TestFixture(typeof(FirefoxDriver))]
    public class SearchPage_Tests<TDriver> where TDriver : IWebDriver, new()
    {
        public IWebDriver Driver { get; set; }
        public SearchPage Page { get; set; }

        [OneTimeSetUp]
        public void InitializeEnvironment()
        {
            Driver = new TDriver();
            Page = new SearchPage(Driver);
        }

        [SetUp]
        public void InitializeTest()
        {
            if (Driver.Url != Page.URL)
            {
                Page.LoadPage();
            }
            
            Page.ClearSearchBox();
        }

        [OneTimeTearDown]
        public void Exit()
        {
            Driver.Quit();
        }

        [Test]
        public void SearchBoxDisplay()
        {
            Assert.IsTrue(Page.IsSearchBoxDisplayed());
        }

        [Test]
        public void SearchButtonDisplay()
        {
            Assert.IsTrue(Page.IsSearchButtonDisplayed());
        }

        [Test, Sequential]
        public void SearchBoxFill([Values("Cellebrite", "CELLEBRITE", "cellebrite")] string value)
        {
            Page.InsertTextIntoSearchBox(value);
            Assert.AreEqual(value, Page.GetTextFromSearchBox());
        }

        [Test]
        public void SearchByButton()
        {
            Page.InsertTextIntoSearchBox("Cellebrite");
            Page.SearchButtonMouseClick();
            Assert.IsTrue(Driver.Url.StartsWith(@"https://www.google.com/search"));
        }

        [Test]
        public void SearchByEnter()
        {
            Page.InsertTextIntoSearchBox("Cellebrite");
            Page.SearchBoxPressEnter();
            Assert.IsTrue(Driver.Url.StartsWith(@"https://www.google.com/search"));
        }

        [Test ,Combinatorial]
        public void FindRelevantResults([Values("Cellebrite", "CELLEBRITE", "cellebrite")] string value, [Values(@"https://www.cellebrite.com/")] string resultURL)
        {
            Page.InsertTextIntoSearchBox(value);
            Page.SearchBoxPressEnter();

            ResultsPage p = new ResultsPage(Driver);
            p.LocateElements();
            Assert.IsTrue(p.IsResultExist(resultURL));
        }
    }

    [TestFixture(typeof(ChromeDriver)), Order(2), Timeout(20000)]
    [TestFixture(typeof(FirefoxDriver))]
    public class ResultsPage_Tests<TDriver> where TDriver : IWebDriver, new()
    {
        public IWebDriver Driver { get; set; }
        public ResultsPage Page { get; set; }

        [OneTimeSetUp]
        public void InitializeEnvironment()
        {
            Driver = new TDriver();
        }

        [SetUp]
        public void InitializeTest()
        {
            SearchPage P1 = new SearchPage(Driver);
            P1.LoadPage();
            P1.InsertTextIntoSearchBox("Cellebrite");
            P1.SearchButtonMouseClick();

            Page = new ResultsPage(Driver);
            Page.LocateElements();
        }

        [OneTimeTearDown]
        public void Exit()
        {
            Driver.Quit();
        }

        [Test, Combinatorial]
        public void ResultDirection([Values(@"https://www.cellebrite.com/")] string resultLinkUrl, [Values(@"https://www.cellebrite.com/en/home/")] string expectedDirectionUrl)
        {
            Page.ResultClick(resultLinkUrl);
            Assert.AreEqual(expectedDirectionUrl, Driver.Url);
        }

        [Test, Combinatorial]
        public void ResultSuggestionsExistence([Values(@"https://www.cellebrite.com/")] string resultLinkUrl, [Values ("Contact", "About", "Products", "Law Enforcement", "UFED Ultimate")] string suggestionName)
        {
            Assert.IsTrue(Page.IsResultSuggestionLinkExist(resultLinkUrl, suggestionName));
        }
    }
}