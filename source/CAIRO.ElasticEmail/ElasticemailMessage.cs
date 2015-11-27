using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace CAIRO.ElasticEmail
{ 
    public class ElasticemailMessage
    {
        private readonly Dictionary<string, byte[]> _attachments;

        public ElasticemailMessage()
        {
            _attachments = new Dictionary<string, byte[]>();
            To = new List<MailAddress>();
        }

        public MailAddress From { get; set; }
        public MailAddress ReplyTo { get; set; }
        public List<MailAddress> To { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Subject { get; set; }

        public Dictionary<string, byte[]> Attachments => _attachments;

        public void AddAttachment(string filenameWithFullPath)
        {
            if (!File.Exists(filenameWithFullPath))
            {
                throw new FileNotFoundException("", filenameWithFullPath);
            }
            var content = File.ReadAllBytes(filenameWithFullPath);
            _attachments.Add(Path.GetFileName(filenameWithFullPath), content);
        }

        public void AddAttachment(string filename, byte[] content)
        {
            if (_attachments.ContainsKey(filename))
            {
                throw new InvalidOperationException($"An attachment with name {filename} already exists.");
            }
            _attachments.Add(filename, content);
        }
    }
}