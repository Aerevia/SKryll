using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Serilog;
using Serilog.Core;

namespace SKryll
{
    public class StrategyManager
    {
        string strategiesUrl = @"https://platform.kryll.io/strategies";

        LoggerConfiguration Log = new LoggerConfiguration();
        Logger logger;

        public ChromeOptions BrowserOptions { get; set; }

        public ChromeDriver Browser { get; set; }

        public List<Strategy> Strategies { get; set; }

        public StrategyManager(ChromeOptions chromeOptions)
        {
            logger = Log.CreateLogger();

            Browser = new ChromeDriver(chromeOptions);
        }

        public StrategyManager(bool isHeadless = true)
        {
            logger = Log.CreateLogger();
            BrowserOptions = new ChromeOptions();

            BrowserOptions.AddArguments(new List<string>() { isHeadless ? "headless" : "head", "disable-gpu", @"user-data-dir=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data"});

            Browser = new ChromeDriver(BrowserOptions);

        }

        public List<Strategy> GetStrategies()
        {
            Strategies = new List<Strategy>();

            Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Browser.Navigate().GoToUrl(strategiesUrl);

            var strategiesDiv = Browser.FindElementsByCssSelector("app-card-strategy-dashboard");

            foreach (var item in strategiesDiv)
            {

                Strategy strategy = new Strategy
                {
                    Title = item.FindElement(By.ClassName("title-text")).Text,
                    Status = item.FindElement(By.ClassName("card-dashboard-status")).Text,
                    Fuel = item.FindElement(By.ClassName("battery-krl")).Text,
                    Pair = item.FindElements(By.ClassName("card-dashboard-pairs"))[0].Text,
                    ProfitString = item.FindElement(By.ClassName("card-dashboard-profits")).Text,
                    StartTime = DateTime.Parse(item.FindElements(By.ClassName("card-dashboard-pairs"))[1].Text),
                    //Alt = item.FindElement(By.ClassName("corner")).GetAttribute("alt"),
                    StartingCapitalString = item.FindElements(By.ClassName("card-body"))[1].FindElements(By.ClassName("toolbar-col"))[0].Text,
                    CurrentValueString = item.FindElements(By.ClassName("card-body"))[1].FindElements(By.ClassName("toolbar-col"))[1].Text,
                    WebElement = item
                };

                Strategies.Add(strategy);
            }

            return Strategies;
        }

        public void StopStrategy(Strategy strategy)
        {
            strategy.WebElement.Click();
            Browser.FindElementByClassName("toolbar-actions").FindElements(By.ClassName("btn"))[1].Click();
            Browser.FindElementByClassName("dialog-buttons").FindElements(By.ClassName("btn"))[1].Click();

        }
        public void StopStrategy(string title)
        {
            StopStrategy(Strategies.Where(s => s.Title.Equals(title)).First());
        }

        public void Exit()
        {
            Browser.Close();
            Browser.Quit();
        }

    }
}
