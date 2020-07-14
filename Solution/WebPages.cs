using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pages
{
    public abstract class WebPage
    {
        public IWebDriver Driver { get; protected set; }
        public abstract string URL { get; }

        public WebPage(IWebDriver driver)
        {
            Driver = driver;

            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 10);
        }

        public virtual void LoadPage()
        {
            Driver.Navigate().GoToUrl(URL);
            LocateElements();
        }

        public abstract void LocateElements();
    }

    public class SearchPage : WebPage
    {
        public SearchPage(IWebDriver driver) : base(driver)
        {

        }

        private IWebElement SearchBox { get; set; }
        private IWebElement SearchButton { get; set; }

        public override string URL
        {
            get
            {
                return @"https://www.google.com";
            }
        }

        public override void LocateElements()
        {
            SearchBox = Driver.FindElement(By.Name("q"));
            SearchButton = Driver.FindElement(By.Name("btnK"));
        }

        public void InsertTextIntoSearchBox(string txt)
        {
            SearchBox.SendKeys(txt);
        }

        public void ClearSearchBox()
        {
            SearchBox.Clear();
        }

        public string GetTextFromSearchBox()
        {
            return SearchBox.GetAttribute("value");
        }

        public void SearchButtonMouseClick()
        {
            SearchButton.Click();
        }

        public void SearchBoxPressEnter()
        {
            SearchBox.SendKeys(Keys.Enter);
        }

        public bool IsSearchBoxDisplayed()
        {
            return SearchBox.Displayed;
        }

        public bool IsSearchButtonDisplayed()
        {
            return SearchBox.Displayed;
        }
    }

    public class ResultsPage : WebPage
    {
        private ReadOnlyCollection<IWebElement> Results { get; set; }

        public override string URL
        {
            get
            {
                return string.Empty;
            }
        }

        public ResultsPage(IWebDriver driver) : base(driver)
        {

        }

        public override void LocateElements()
        {
            Results = Driver.FindElements(By.XPath(@"//div[@class='g']"));
        }

        public bool IsResultExist(string resultLinkUrl)
        {
            IWebElement item = GetResultLinkElement(resultLinkUrl);
            
            if (item != null)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private IWebElement GetResultLinkElement(string resultLinkUrl)
        {
            IWebElement result = null;

            foreach (IWebElement element in Results)
            {
                try
                {
                    result = element.FindElement(By.XPath($@"//a[@href='{resultLinkUrl}']"));

                    if (result != null)
                    {
                        return result;
                    }
                }

                catch (NoSuchElementException) { }
            }

            return result;
        }

        public bool IsResultSuggestionLinkExist(string resultLinkUrl, string suggestionName)
        {
            IWebElement result = null;

            foreach (IWebElement element in Results)
            {
                try
                {
                    if (element.FindElement(By.XPath($@"//a[@href='{resultLinkUrl}']")) != null)
                    {
                        result = element.FindElement(By.XPath($@"//span[contains(text(), '{suggestionName}')]"));

                        if (result != null)
                        {
                            return true;
                        }
                    }
                }

                catch (NoSuchElementException) { }
            }

            return false;
        }

        public void ResultClick(string resultLinkUrl)
        {
            IWebElement result = GetResultLinkElement(resultLinkUrl);

            if (result != null)
            {
                result.Click();
            }
        }
    }
}