using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient;
using TenmoClient.Data;
using TenmoServer.Controllers;

namespace TenmoTest
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly AccountService accountService = new AccountService();
        private static readonly AuthService authService = new AuthService();
        [TestMethod]
        public void TestGetBalance()
        {
            LoginUser user = new LoginUser();

            user.Username = "test";
            user.Password = "test";

            API_User api_user = authService.Login(user);
            UserService.SetLogin(api_user);

            decimal? result = accountService.GetBalance(UserService.GetToken());
            decimal? expected = 1000;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestBadTokenGetBalance()
        {
            string token = "thisIsWrong";
            decimal? result = accountService.GetBalance(token);
            decimal? expected = null;

            Assert.AreEqual(expected, result);
        }
    }
}
