using Cizeta.TraMaAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TraMaAuth_Test
{
    [TestClass]
    public class WorkerTester
    {
        string ConnectionString;

        private void Init_PB139()
        {
            ConnectionString = @"Data Source=.\SQLEXPRESS15;Initial Catalog=TraMa_PB139;User ID=TraMa;Password=trama";
            Authentication.Init(ConnectionString);
        }

      [TestMethod]
        public void TestLocalUserPasswordOk()
        {
            bool actual = false;
            bool expected = true;
            string workerLoginName = "Administrator";
            string workerPassword = "Administrator";
            string stationName = "ST98C";
            Init_PB139();
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.UserPassword);
            WorkerLoginResult res = wa.Login(workerLoginName, workerPassword, string.Empty, stationName);
            if (res == WorkerLoginResult.Ok)
                actual = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLocalUserPasswordKo()
        {
            bool actual = false;
            bool expected = true;
            string workerLoginName = "FakeUser";
            string workerPassword = "fake";
            string stationName = "ST104F";
            Init_PB139();
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.UserPassword);
            WorkerLoginResult res = wa.Login(workerLoginName, workerPassword, string.Empty, stationName);
            if (res == WorkerLoginResult.Failed)
                actual = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLocalBadgeCodeOk()
        {
            bool actual = false;
            bool expected = true;
            string workerLoginName = "Administrator";
            string workerBadgeCode = "Administrator";
            string stationName = "ST104F";
            Init_PB139();
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.BadgeCode);
            WorkerLoginResult res = wa.Login(string.Empty, string.Empty, workerBadgeCode, stationName);
            if (wa.CurrentWorker.LoginName == workerLoginName && res == WorkerLoginResult.Ok)
                actual = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLocalBadgeCodeKo()
        {
            bool actual = false;
            bool expected = true;
            string workerLoginName = string.Empty;
            string workerBadgeCode = "12345";
            string stationName = "ST99C";
            Init_PB139();
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.BadgeCode);
            WorkerLoginResult res = wa.Login(string.Empty, string.Empty, workerBadgeCode, stationName);
            if (wa.CurrentWorker.LoginName == workerLoginName && res == WorkerLoginResult.Failed)
                actual = true;
            Assert.AreEqual(expected, actual);
        }
    }
}
