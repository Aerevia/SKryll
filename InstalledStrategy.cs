using OpenQA.Selenium;
using System;
using System.Text.RegularExpressions;

namespace SKryll
{
    public class InstalledStrategy
    {
        public string Title { get; set; }

        public IWebElement StartButton { get; set; }
        public IWebElement BackTestButton { get; set; }
        public IWebElement InfoButton { get; set; }

        public InstalledStrategy()
        {

        }
    }
}
