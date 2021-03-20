using OpenQA.Selenium;
using System;
using System.Text.RegularExpressions;

namespace SKryll
{
    public class Strategy
    {
        #region properties
        public string Title { get; set; }
        public string Alt { get; set; }
        public string Fuel { get; set; }
        public float Profit 
        {
            get 
            {
                return float.Parse(ProfitString.Remove(0, 2).Replace(" %", "").Replace(".", ","));
            }
        } 
        public string ProfitString { get; set; }
        public DateTime StartTime { get; set; }
        public string Pair { get; set; }
        public string Status { get; set; }
        public string StartingCapitalString { get; set; }
        public float StartingCapital 
        { 
            get
            {
                return float.Parse(Regex.Match(StartingCapitalString.Replace(".", ","), @"[-+]*\d+\,\d+|[-+]*\d+").Value);
            }
        }
        public string CurrentValueString { get; set; }
        public float CurrentValue
        {
            get
            {
                return float.Parse(Regex.Match(CurrentValueString.Replace(".", ","), @"[-+]*\d+\,\d+|[-+]*\d+").Value);
            }
        }

        public string AccumulatedCoin
        { 
            get
            {
                return Pair.Remove(0, Pair.IndexOf("/") + 2);
            }
        }

        public IWebElement WebElement { get; set; }
        #endregion

        public Strategy()
        {

        }

        public override string ToString()
        {
            return Title + Environment.NewLine +
                   Status + Environment.NewLine +
                   Fuel + Environment.NewLine +
                   Pair + Environment.NewLine +
                   Profit.ToString() + Environment.NewLine +
                   StartTime + Environment.NewLine +
                   StartingCapital + Environment.NewLine +
                   CurrentValue + Environment.NewLine +
                   AccumulatedCoin;
        }
    }
}
