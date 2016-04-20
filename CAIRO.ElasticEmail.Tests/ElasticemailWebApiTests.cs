using System;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CAIRO.ElasticEmail.Tests
{
    [TestClass]
    public class ElasticemailWebApiTests
    {
        private string _apiKey = "";

        [TestMethod]
        public void SendUnauthorized()
        {
            var target = new ElasticemailWebApi("invalidApiKey");
            var mail = new ElasticemailMessage();
            mail.From = new MailAddress("john@example.com", "John");
            mail.To.Add(new MailAddress("anna@example.com", "Anna"));
            var actual = target.Send(mail);

            Assert.AreEqual(ResultType.Error, actual.ResultType);
            Assert.IsTrue(actual.ErrorMessage.Contains("Unauthorized"));
        }

        [TestMethod]
        public void ErrorMessage_If_From_Address_Is_Missing()
        {
            var target = new ElasticemailWebApi(_apiKey);
            var actual = target.Send(new ElasticemailMessage());

            Assert.AreEqual(ResultType.Error, actual.ResultType);
            Assert.IsTrue(actual.ErrorMessage.Contains("sender address is missing"));
        }

        [TestMethod]
        public void ErrorMessage_If_To_Address_Is_Missing()
        {
            var target = new ElasticemailWebApi(_apiKey);
            var mail = new ElasticemailMessage();
            mail.From = new MailAddress("john@example.com", "John");
            var actual = target.Send(mail);

            Assert.AreEqual(ResultType.Error, actual.ResultType);
            Assert.IsTrue(actual.ErrorMessage.Contains("recipient address is missing"));
        }

        [TestMethod]
        public void Send_Email_Returns_TransactionId()
        {
            var target = new ElasticemailWebApi(_apiKey);
            var mail = new ElasticemailMessage();
            mail.To.Add(new MailAddress("md@cairo.ag"));
            mail.From = new MailAddress("md@cairo.ag", "Marc");
            mail.ReplyTo = new MailAddress("md@cairo.ag", "Marc");
            mail.Subject = "Test";
            mail.Body = "Body";
            var actual = target.Send(mail);

            Assert.AreEqual(ResultType.Success, actual.ResultType);
            Assert.IsNotNull(actual.TransactionId);
        }

        [TestMethod]
        public void Send_Email_With_Attachment()
        {
            var target = new ElasticemailWebApi(_apiKey);
            var mail = new ElasticemailMessage();
            mail.To.Add(new MailAddress("md@cairo.ag", "Marc"));
            mail.From = new MailAddress("md@cairo.ag", "Marc");
            mail.ReplyTo = new MailAddress("md@cairo.ag", "Marc");
            mail.Subject = "Test";
            mail.Body = "Body";
            mail.AddAttachment("file.txt", new byte[100]);
            mail.AddAttachment("file2.txt", new byte[200]);
            var actual = target.Send(mail);

            Assert.AreEqual(ResultType.Success, actual.ResultType);
            Assert.IsNotNull(actual.TransactionId);
        }

        [TestMethod]
        public void GetDeliveryStatus_Valid_TransactionId()
        {
            Guid id = Guid.Parse("53b12541-210e-49b3-b57a-dd64e09cde5f");
            var target = new ElasticemailWebApi(_apiKey);

            var actual = target.GetDeliveryStatus(id);

            Assert.AreEqual(ResultType.Success, actual.ResultType);
            Assert.AreEqual(id, actual.DeliveryStatus.Id);
            Assert.AreEqual(1, actual.DeliveryStatus.Delivered);
            Assert.AreEqual("complete", actual.DeliveryStatus.Status);
        }

        [TestMethod]
        public void GetDeliveryStatus_Invalid_TransactionId()
        {
            var target = new ElasticemailWebApi(_apiKey);

            var actual = target.GetDeliveryStatus(Guid.Parse("53b12541-1234-49b3-b57a-dd64e09cde5f"));

            Assert.AreEqual(ResultType.Error, actual.ResultType);
            Assert.AreEqual("No job with transactionId 53b12541-1234-49b3-b57a-dd64e09cde5f could be found.", actual.ErrorMessage);
        }
    }
}
