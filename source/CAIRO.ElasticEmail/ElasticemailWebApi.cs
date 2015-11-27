using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;

namespace CAIRO.ElasticEmail
{
    public class ElasticemailWebApi
    {
        private readonly string _username;
        private readonly string _apiKey;

        public ElasticemailWebApi(string username, string apiKey)
        {
            _username = username;
            _apiKey = apiKey;
        }

        public SendResult Send(ElasticemailMessage msg)
        {
            var result = new SendResult();

            try
            {
                string validationErrors;
                if (IsValid(msg, out validationErrors))
                {
                    var client = new WebClient();
                    var values = new NameValueCollection();
                    values.Add("username", _username);
                    values.Add("api_key", _apiKey);
                    values.Add("from", msg.From.Address);
                    values.Add("from_name", msg.From.DisplayName);
                    values.Add("to", string.Join(";", msg.To.Select(x => x.Address)));
                    values.Add("subject", msg.Subject);
                    if (msg.IsBodyHtml)
                    {
                        values.Add("body_html", msg.Body);
                    }
                    else
                    {
                        values.Add("body_text", msg.Body);
                    }

                    if (msg.ReplyTo != null)
                    {
                        values.Add("reply_to", msg.ReplyTo.Address);
                    }
                    
                    var attachmentIds = new List<string>();
                    foreach (var attachment in msg.Attachments)
                    {
                        //once an attachment has been uploaded once you can use it in multiple calls to send elastic email.
                        var attId = UploadAttachment(attachment.Key, attachment.Value);
                        attachmentIds.Add(attId);
                    }
                    if (attachmentIds.Any())
                    {
                        values.Add("attachments", string.Join(";", attachmentIds));
                    }

                    var response = client.UploadValues("https://api.elasticemail.com/mailer/send", values);
                    var responseString = Encoding.UTF8.GetString(response);

                    Guid guid;
                    if (Guid.TryParse(responseString, out guid))
                    {
                        result.TransactionId = guid;
                        result.ResultType = ResultType.Success;
                    }
                    else
                    {
                        result.ErrorMessage = responseString;
                        result.ResultType = ResultType.Error;
                    }
                }
                else
                {
                    result.ResultType = ResultType.Error;
                    result.ErrorMessage = validationErrors;
                }
            }
            catch (Exception ex)
            {
                result.ResultType = ResultType.Error;
                result.ErrorMessage = ex.ToString();
            }

            return result;
        }

        private bool IsValid(ElasticemailMessage msg, out string validationErrors)
        {
            var errors = new List<string>();
            if (msg.From == null)
            {
                errors.Add("sender address is missing");
            }
            if (!msg.To.Any())
            {
                errors.Add("recipient address is missing");
            }

            validationErrors = String.Join(Environment.NewLine, errors);
            return !errors.Any();
        }

        private string UploadAttachment(string filename, byte[] content)
        {
            var stream = new MemoryStream(content);
            var request = WebRequest.Create("https://api.elasticemail.com/attachments/upload?username=" + _username + "&api_key=" + _apiKey + "&file=" + filename);
            
            request.Method = "PUT";
            request.ContentLength = stream.Length;
            var outstream = request.GetRequestStream();
            stream.CopyTo(outstream, 4096);
            stream.Close();
            var response = request.GetResponse();
            var result = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            response.Close();
            return result;
        }
    }
}