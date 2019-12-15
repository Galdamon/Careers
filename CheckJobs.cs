using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

class CheckJobs
{
    IWebDriver driver = new FirefoxDriver("../../packages/Selenium.WebDriver.GeckoDriver.0.26.0/driver/win64");
    public CheckJobs(string country, string language, int jobCount)
    {
        Console.WriteLine($"Running test 'CheckJobs' with: {country}, {language}, {jobCount}");
        bool construction = Construct(country, language);
        if (construction) { Test(jobCount); } 
        else { Console.WriteLine("CheckJobs test could not run!"); }
    }

    private bool Construct(string country, string language)
    {
        driver.Navigate().GoToUrl("https://careers.veeam.com");
        driver.Manage().Window.Maximize();

        try
        {
            driver.FindElement(By.Id("country-element")).Click();
            driver.FindElement(By.XPath($"//span[@data-value='{country}']")).Click();
        }
        catch (NoSuchElementException) { return false; }

        IWebElement dropdown = driver.FindElement(By.Id("language"));
        dropdown.Click();
        ReadOnlyCollection<IWebElement> options = dropdown.FindElements(By.ClassName("controls-checkbox"));
        foreach (IWebElement option in options)
        { 
            if (option.Text == language) { option.Click(); }
        }
        dropdown.Click();

        return true;
    }
    private void Test(int jobCount)
    {
        IWebElement test = driver.FindElement(By.CssSelector(".content-loader-button.load-more-button"));
        if (test.Displayed) 
        { 
            test.Click();
            Thread.Sleep(1000);
        }

        int displayedJobs = 0;

        ReadOnlyCollection<IWebElement> jobContainers = driver.FindElements(By.CssSelector(".vacancies-blocks-container"));
        foreach(IWebElement jobContainer in jobContainers)
        {
            displayedJobs += jobContainer.FindElements(By.ClassName("vacancies-blocks-col")).Count();
        }

        try
        {
            Assert.AreEqual(jobCount, displayedJobs, "Job count is different than expected!");
        }
        catch (AssertionException e) { Console.WriteLine(e.Message); }

        driver.Quit();
    }
}
