using System.Drawing;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using FluentAssertions;
using FluentAssertions.Execution;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

// using OpenQA.Selenium.Internal.Logging;
// using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTest_Rakhimova;

public class SeleniumTestsForPractice
{
    public ChromeDriver driver;
    public string baseUrl = "https://staff-testing.testkontur.ru";


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
        currentUrl.Should().Be(baseUrl + "/news");
    }

    [Test]
    public void NavigationTest()
    {
        // При запуске теста, у меня отсутвовал бургер (SidebarMenuButton), так как автоматически открывалось окно больше 1280px*945px из-за кода на странице 24
        driver.Manage().Window.Size = new Size(1280, 945);
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();

        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();

        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        Assert.That(driver.Url == baseUrl + "/communities",
            "На странице 'Сообщества' нет https://staff-testing.testkontur.ru/communities");
    }

    // Редактирование профиля
    [Test]
    public void EditProfile()
    {
        //
        driver.FindElement(By.CssSelector("[data-tid='Title']"));

        //Перейти на страницу редактирования профиля
        driver.Navigate().GoToUrl(baseUrl + "/profile/settings/edit");

        //Проверить, что мы находимся на странице с заголовком "Редактирование профиля"
        var pageEditTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        Assert.That(pageEditTitle.Text == "Редактирование профиля",
            "На странице нет заголовка 'Редактирование профиля'");

        //Найти поле Адрес рабочего места и заполнить
        var addressInput = driver.FindElement(By.CssSelector("[data-tid='Address'] [data-tid='Input'] textarea"));
        // .Clear() не работает
        addressInput.SendKeys(Keys.Control + "A");
        addressInput.SendKeys(Keys.Backspace);
        addressInput.SendKeys("Садовая 17");

        //Найти поле Мобильный телефон и заполнить
        var mobilePhoneInput = driver.FindElement(By.CssSelector("[data-tid='MobilePhone'] [data-tid='Input']"));
        mobilePhoneInput.SendKeys(Keys.Control + "A");
        mobilePhoneInput.SendKeys(Keys.Backspace);
        mobilePhoneInput.SendKeys("7777777777");
        Assert.That(mobilePhoneInput.Text == "+7 777 777-77-77", "Номер телефона не отформатировался");

        //Найти кнопку Сохранить и нажать
        var buttonSave = driver.FindElement(By.CssSelector("[data-tid='PageHeader'] button:nth-child(1)"));
        Actions actions = new Actions(driver);
        actions.MoveToElement(buttonSave);
        actions.Perform();
        buttonSave.Click();

        //Проверить, что мы находимся на странице "Профиль"
        var pageProfileTitle = driver.FindElement(By.CssSelector("[data-tid='PageHeader'] [data-tid='Tabs']"));
        Assert.That(pageProfileTitle.Text == "Профиль", "На странице нет заголовка 'Профиль'");
        //Проверить что новые данные есть на странице Профиль

        // Note: порядок элементов может сломать тест
        var actualMobilePhone =
            driver.FindElement(
                By.CssSelector("[data-tid='ContactCard'] > div:nth-child(2) > div:nth-child(1) > div:nth-child(2)"));
        Assert.That(actualMobilePhone.Text == "+7 777 777-77-77",
            "Сохраненный номер телефона не совпадает с введённым");

        var actualAddress =
            driver.FindElement(By.CssSelector("[data-tid='ContactCard'] > div:nth-child(2) > div:nth-child(2)"));
        Assert.That(actualAddress.Text == "Садовая 17", "Сохраненный адрес не совпадает с введённым");
    }


    //Написать сообщение сотруднику
    [Test]
    public void CreateNewMessages()
    {
        //Перейти на страницу Диалоги
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='PageMainMenu'] [data-tid='Messages']"));
        sideMenu.Click();
        //Проверить что мы находимся на странице Диалоги
        var pageMessagesTittle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        Assert.That(driver.Url == baseUrl + "/messages",
            "На странице нет заголовка 'Диалоги'");
        //В поисковой панели ввести фамилию и выбрать
        var searchBar = driver.FindElement(By.CssSelector("[data-tid='PageBody'] [data-tid='SearchBar']"));
        new Actions(driver)
            .MoveToElement(searchBar)
            .Click()
            .SendKeys("Рахимова")
            .Perform();
        
        //На странице личной переписке отправить сообщение
        var message = "Test message: " + Guid.NewGuid();
        var selectEmployees = driver.FindElement(By.CssSelector("[data-tid='ComboBoxMenu__item']"));
        selectEmployees.Click();
        var composingNewMessage = driver.FindElement(By.CssSelector("[data-tid='CommentInput']"));
        composingNewMessage.Click();
        composingNewMessage.SendKeys(message);
        var sendButtonMessage = driver.FindElement(By.CssSelector("[data-tid='SendButton']"));
        sendButtonMessage.Click();
        
        //Тут  есть баг, после отправки сообщения в диалоговом окне не показывается отправленное сообщение. Нужно обновить страницу
        //driver.Navigate().Refresh();
        //Проверить что отправленное сообщение показывается
        var actualMessage =  driver.FindElements(By.CssSelector("[data-tid='Item'] [data-tid='MessageText']")).Last();
        Assert.That(actualMessage.Text, Is.EqualTo(message));
    }

    [Test]
    //Разлогиниться
    public void LogoutUser()
    {
        //Нажать на аватарку в правом верхнем углу страницы.
        var avatarPopupMenu = driver.FindElement(By.CssSelector("[data-tid='ProfileMenu']"));
        avatarPopupMenu.Click();
        
        //Нажать на кнопку Выйти в раскрывшемся списке
        var buttonLogout = driver.FindElement(By.CssSelector("[data-tid='Logout']"));
        buttonLogout.Click();
        
        //На странице Выход из учетной записи нажать на ссылку страницы авторизации. Проверить что пользователь действительно вышел
        var loginPageLink = driver.FindElement(By.CssSelector(".PostLogoutRedirectUri"));
        Assert.That(loginPageLink.GetAttribute("href") == baseUrl + "/login",
            "На странице нет ссылки на страницу авторизации");
        loginPageLink.Click();

        var actualPageLogin = driver.FindElement(By.Name("button"));
        Assert.That(actualPageLogin.Displayed, "На странице нет кнопки 'Войти'");
    }

    public void Authorization()
    {
        driver.Navigate().GoToUrl(baseUrl + "/");
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
