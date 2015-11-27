using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CAIRO.ElasticEmail.Tests
{
    [TestClass]
    public class ElasticemailMessageTests
    {
        [TestMethod]
        public void AddAttachment()
        {
            var target = new ElasticemailMessage();
            target.AddAttachment("file1.txt", new byte[100]);
            target.AddAttachment("file2.txt", new byte[200]);

            Assert.AreEqual(2, target.Attachments.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddAttachment_Throws_Exception_If_Filename_Already_Exists()
        {
            var target = new ElasticemailMessage();
            target.AddAttachment("file1.txt", new byte[100]);
            target.AddAttachment("file1.txt", new byte[200]);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void AddAttachment_Throws_Exception_If_File_Not_Found()
        {
            var target = new ElasticemailMessage();
            target.AddAttachment(@"C:\notExistingFile.txt");
        }
    }
}
