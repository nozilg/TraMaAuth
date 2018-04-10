using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cizeta.TraMaAuth;

namespace TraMaAuth_Test
{
    [TestClass]
    public class WorkerAuthenticatorTester
    {

        [TestMethod]
        public void TestLocalUserPassword()
        {
            bool actual = false;
            bool expected = true;
            string workerLoginName = "Administrator";
            string workerPassword = "admin";
            string stationName = "ST60.00";
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.UserPassword);
            WorkerLoginResult res = wa.Login(workerLoginName, workerPassword, string.Empty, stationName);
            if (res == WorkerLoginResult.Ok)
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
            string workerLoginName = "Administrator";
            string workerBadgeCode = "12345";
            string stationName = "ST60.00";
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.BadgeCode);
            WorkerLoginResult res = wa.Login(string.Empty, string.Empty, workerBadgeCode, stationName);
            if (wa.CurrentWorker.LoginName == workerLoginName && res == WorkerLoginResult.Ok)
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
            string workerLoginName = "marco.dallera";
            string workerPassword = "md";
            string stationName = "ST60.00";
            string connectionString = "Data Source=192.168.100.250;Initial Catalog=TraMa4_520-334;User ID=TraMa;Password=trama";
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.UserPassword, connectionString);
            WorkerLoginResult res = wa.Login(workerLoginName, workerPassword, string.Empty, stationName);
            if (res == WorkerLoginResult.Ok)
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
            string workerLoginName = "marco.dallera";
            string workerBadgeCode = ";80380000100118208054?";
            string stationName = "ST60.00";
            string connectionString = "Data Source=192.168.100.250;Initial Catalog=TraMa4_520-334;User ID=TraMa;Password=trama";
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.BadgeCode, connectionString);
            WorkerLoginResult res = wa.Login(string.Empty, string.Empty, workerBadgeCode, stationName);
            if (wa.CurrentWorker.LoginName == workerLoginName && res == WorkerLoginResult.Ok)
            {
                actual = true;
            }
            Assert.AreEqual(actual, expected);
        }

    }
}
