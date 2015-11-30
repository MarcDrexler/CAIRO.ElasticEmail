using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Serialization;

namespace CAIRO.ElasticEmail
{
    public class ElasticemailWebApi
    {
        private readonly Configuration _configuration;

        public Configuration Configuration => _configuration;

        public ElasticemailWebApi(string apiKey)
        {
            _configuration = new Configuration();
            _configuration.ApiKey = apiKey;
        }

        public SendResult Send(ElasticemailMessage msg)
        {
            var result = new SendResult();

            try
            {
                string validationErrors;
                if (!IsValid(msg, out validationErrors))
                {
                    result.ResultType = ResultType.Error;
                    result.ErrorMessage = validationErrors;
                    return result;
                }

                var client = new WebClient();
                var values = new NameValueCollection();
                values.Add("api_key", _configuration.ApiKey);
                values.Add("from", msg.From.Address);
                values.Add("from_name", msg.From.DisplayName);
                values.Add("to", string.Join(";", msg.To.Select(x => x.Address)));
                values.Add("subject", msg.Subject);
                values.Add(msg.IsBodyHtml ? "body_html" : "body_text", msg.Body);
                if (msg.ReplyTo != null) values.Add("reply_to", msg.ReplyTo.Address);

                var attachmentIds = new List<string>();
                foreach (var attachment in msg.Attachments)
                {
                    var attId = UploadAttachment(attachment.Key, attachment.Value);
                    attachmentIds.Add(attId);
                }
                if (attachmentIds.Any()) values.Add("attachments", string.Join(";", attachmentIds));

                var response = new RetryLogic(_configuration.PreferredRetryStrategy).Execute(() => client.UploadValues("https://api.elasticemail.com/mailer/send", values));
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
            catch (Exception ex)
            {
                result.ResultType = ResultType.Error;
                result.ErrorMessage = ex.ToString();
            }

            return result;
        }

        public DeliveryStatusResponse GetDeliveryStatus(Guid transactionId)
        {
            var response = new DeliveryStatusResponse();

            try
            {
                var request = WebRequest.Create("https://api.elasticemail.com/mailer/status/" + transactionId);
                Stream stream = new RetryLogic(_configuration.PreferredRetryStrategy).Execute(() => request.GetResponse().GetResponseStream());
                var xmlString = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                if (xmlString.ToLower().StartsWith("no job with transactionid"))
                {
                    response.ErrorMessage = xmlString;
                    response.ResultType = ResultType.Error;
                }
                else
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(job));
                        StringReader rdr = new StringReader(xmlString);
                        var job = (job)serializer.Deserialize(rdr);
                        var deliveryStatus = new DeliveryStatus();
                        deliveryStatus.Id = transactionId;
                        deliveryStatus.Recipients = job.recipients;
                        deliveryStatus.Delivered = job.delivered;
                        deliveryStatus.Failed = job.failed;
                        deliveryStatus.Pending = job.pending;
                        deliveryStatus.Status = job.status;

                        response.DeliveryStatus = deliveryStatus;
                        response.ResultType = ResultType.Success;
                    }
                    catch (Exception ex)
                    {
                        response.ErrorMessage = "Could not deserialize message: " + Environment.NewLine + ex;
                        response.ResultType = ResultType.Error;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.ToString();
                response.ResultType = ResultType.Error;
            }

            return response;
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
            var request = WebRequest.Create("https://api.elasticemail.com/attachments/upload?api_key=" + _configuration.ApiKey + "&file=" + filename);
            request.Method = "PUT";
            request.ContentLength = stream.Length;

            string attachmentId = null;
            new RetryLogic(_configuration.PreferredRetryStrategy).Execute(() =>
            {
                var outstream = request.GetRequestStream();
                stream.CopyTo(outstream, 4096);
                stream.Close();
                var response = request.GetResponse();
                attachmentId = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                response.Close();
            });
            return attachmentId;
        }
    }
}