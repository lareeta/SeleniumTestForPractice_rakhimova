using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Seleniumtest_rakhimova;

public class SeleniumTestsForPraktice
{
   [Test]
   public void Authorization()
   {
      // var options = new ChromeOptions();
      // options.AddArgument("--no-sandbox", "--start-maximized", "--disable-extentions");
      
      var driver = new ChromeDriver();
      driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
      Thread.Sleep(5000);
      
      var login = driver.FindElement(By.Id("Username"));
      login.SendKeys("ri.rakhimova@gmail.com");
      
      var password = driver.FindElement(By.Name("Password"));
      password.SendKeys("\\q9gfwPVciY");
      
      Thread.Sleep(3000);

      var enter = driver.FindElement(By.Name("button"));
      enter.Click();
      Thread.Sleep(3000);

      var currentUrl = driver.Url;
      
      Assert.That(currentUrl == "https://staff-testing.testkontur.ru/news");
      
      driver.Quit();
   }
}