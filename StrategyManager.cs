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

        public List<Strategy> RunningStrategies { get; set; }
        public List<InstalledStrategy> InstalledStrategies { get; set; }

        public StrategyManager(ChromeOptions chromeOptions)
        {
            logger = Log.CreateLogger();

            Browser = new ChromeDriver(chromeOptions);
            Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        public StrategyManager(bool isHeadless = true)
        {
            logger = Log.CreateLogger();
            BrowserOptions = new ChromeOptions();

            BrowserOptions.AddArguments(new List<string>() { isHeadless ? "headless" : "head", "disable-gpu", @"user-data-dir=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data"});

            Browser = new ChromeDriver(BrowserOptions);
            Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);


        }

        public List<Strategy> GetRunningStrategies()
        {
            RunningStrategies = new List<Strategy>();

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

                RunningStrategies.Add(strategy);
            }

            return RunningStrategies;
        }
        public List<InstalledStrategy> GetInstalledtrategies()
        {
            InstalledStrategies = new List<InstalledStrategy>();

            Browser.Navigate().GoToUrl(strategiesUrl);

            var installedStrategiesDiv = Browser.FindElementsByCssSelector("app-card-strategy-picked");

            foreach (var item in installedStrategiesDiv)
            {
                var buttonElements = item.FindElement(By.ClassName("card-actions")).FindElements(By.TagName("a"));
                InstalledStrategy installedStrategy = new InstalledStrategy
                {
                    Title = item.FindElement(By.ClassName("card-name")).Text,
                    InfoButton = buttonElements[0],
                    BackTestButton = buttonElements[1],
                    StartButton = buttonElements[2]
                };

                InstalledStrategies.Add(installedStrategy);
            }

            return InstalledStrategies;
        }

        public void StopStrategy(Strategy strategy)
        {
            strategy.WebElement.Click();
            Browser.FindElementByClassName("toolbar-actions").FindElements(By.ClassName("btn"))[1].Click();
            Browser.FindElementByClassName("dialog-buttons").FindElements(By.ClassName("btn"))[1].Click();
        }

        public void StopStrategy(string title)
        {
            StopStrategy(RunningStrategies.Where(s => s.Title.Equals(title)).First());
        }

        public void StartStrategy(InstalledStrategy installedStrategy, Exchange exchange, string pair,  bool isAllIn, bool isLive, float fuel = 25, float tradedAmount = 0, float baseAmount = 0, string instanceName = "")
        {
            installedStrategy.StartButton.Click();
   
            var startElement = Browser.FindElementByCssSelector("app-dialog-strategy-start");

            var wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(5)).Until(drv => drv.FindElement(By.CssSelector("select[name='multi_exchange']")));


            if (instanceName != "")
            {
                var nameElement = startElement.FindElement(By.CssSelector("input[name='name']"));
                nameElement.Clear();
                nameElement.SendKeys(instanceName);
            }

            SelectElement exchangeSelect = new SelectElement(startElement.FindElement(By.CssSelector("select[name='multi_exchange']")));
            exchangeSelect.SelectByText(exchange.Value);

            SelectElement pairSelect = new SelectElement(startElement.FindElement(By.CssSelector("select[name='pair']")));
            pairSelect.SelectByText(pairSelect.Options.Where(o => o.Text.Contains(pair)).First().Text);

            if (isAllIn)
            {
                startElement.FindElement(By.CssSelector("a[class='underline']")).Click();
            }
            else
            {
                var startingTradedElement = startElement.FindElement(By.CssSelector("input[name='strategy_traded']"));
                startingTradedElement.Clear();
                startingTradedElement.SendKeys(tradedAmount.ToString()); 
                var startingBaseElement = startElement.FindElement(By.CssSelector("input[name='strategy_base']"));
                startingBaseElement.Clear();
                startingBaseElement.SendKeys(baseAmount.ToString());

            }

            var fuelElement = startElement.FindElement(By.CssSelector("input[name='krl_to_lock']"));
            fuelElement.Clear();
            fuelElement.SendKeys(fuel.ToString());

            if (!isLive)
            {
                startElement.FindElement(By.CssSelector("input[id='switchMode']")).Click();
            }


            startElement.FindElement(By.CssSelector("button[type='submit']")).Click();

        }

        public void Exit()
        {
            Browser.Close();
            Browser.Quit();
        }

    }
}
