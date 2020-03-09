using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cizeta.TraMaAuth;

namespace TraMaAuth_Test
{
    [TestClass]
    public class UserAuthenticatorTester
    {

        [TestMethod]
        public void TestLocalUserPassword()
        {
            bool actual = false;
            bool expected = true;
            UserAuthenticator ua = new UserAuthenticator(AuthenticationMode.UserPassword);
            UserLoginResult res = ua.Login("Administrator", "admin", string.Empty);
            if (res == UserLoginResult.Ok)
            {
                actual = true;
            }
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void TestLocalBadgeCode()
        {
            bool actual = false;
            bool expected = true;
            UserAuthenticator ua = new UserAuthenticator(AuthenticationMode.BadgeCode);
            UserLoginResult res = ua.Login(string.Empty, string.Empty, "12345");
            if (ua.CurrentUser.LoginName == "Administrator" && res == UserLoginResult.Ok)
            {
                actual = true;
            }
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void TestRemoteUserPassword()
        {
            bool actual = false;
            bool expected = true;
            //string connectionString = "Data Source=192.168.100.250;Initial Catalog=TraMa4_520-334;User ID=TraMa;Password=trama";
            string connectionString = "Data Source=192.168.153.250;Initial Catalog=TraMa4_SLight;User ID=TraMa;Password=trama";
            UserAuthenticator ua = new UserAuthenticator(AuthenticationMode.UserPassword, connectionString);
            UserLoginResult res = ua.Login("md", "md", string.Empty);
            //UserLoginResult res = ua.Login("marco.dallera", "md", string.Empty);
            if (res == UserLoginResult.Ok)
            {
                actual = true;
            }
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void TestRemoteBadgeCode()
        {
            bool actual = false;
            bool expected = true;
            string connectionString = "Data Source=192.168.100.250;Initial Catalog=TraMa4_520-334;User ID=TraMa;Password=trama";
            UserAuthenticator ua = new UserAuthenticator(AuthenticationMode.BadgeCode, connectionString);
            UserLoginResult res = ua.Login(string.Empty, string.Empty, ";80380000100118208054?");
            if (ua.CurrentUser.LoginName == "marco.dallera" && res == UserLoginResult.Ok)
            {
                actual = true;
            }
            Assert.AreEqual(actual, expected);
        }

    }
}
