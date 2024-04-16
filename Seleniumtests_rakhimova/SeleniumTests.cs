using System.Drawing;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using FluentAssertions;
// using OpenQA.Selenium.Internal.Logging;
// using OpenQA.Selenium.Support.UI;
// using SeleniumExtras.WaitHelpers;

namespace SeleniumTest_Rakhimova;

public class SeleniumTestsForPractice
{
   public ChromeDriver driver;

   [SetUp]
   public void SetUp()
   {
      var options = new ChromeOptions();
      options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");

      driver = new ChromeDriver(options);
      driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
      
      Authorization();
      }
   
   [Test]
   public void AuthorizationTest()
   {
      var news = driver.FindElement(By.CssSelector("[data-tid='Title']"));
      var currentUrl = driver.Url;
      currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
   }

   [Test]
   public void NavigationTest()
   {
      driver.Manage().Window.Size = new Size(1280, 945); // При запуске теста, у меня отсутвовал бургер (SidebarMenuButton), так как у меня автоматически открывалось окно больше 1280px*945px 
      var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
      sideMenu.Click();
      
      var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
         .First(element => element.Displayed);
      community.Click();
      
      var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
      Assert.That(driver.Url == "https://staff-testing.testkontur.ru/communities",
         "На странице 'Сообщества' нет https://staff-testing.testkontur.ru/communities");
   }
   

   public void Authorization() // ToDo: rename using a verb
   {
      driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
      var login = driver.FindElement(By.Id("Username"));
      login.SendKeys("ri.rakhimova@gmail.com");
      
      var password = driver.FindElement(By.Name("Password"));
      password.SendKeys("\\q9gfwPVciY");
      
      var enter = driver.FindElement(By.Name("button"));
      enter.Click();
   }
   
   [TearDown]
   public void TearDown()
   {
      driver.Quit();
   }
}