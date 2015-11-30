
You can send emails through "elastic email" service with the following snippet:


using CAIRO.ElasticEmail;
using System.Net.Mail;

var api = new ElasticemailWebApi("apiKey");
var mail = new ElasticemailMessage();
mail.To.Add(new MailAddress("john@example.com"));
mail.From = new MailAddress("anna@example.com", "Anna");
mail.ReplyTo = new MailAddress("anna@example.com", "Anna");
mail.Subject = "Subject";
mail.Body = "Test Message";
var result = api.Send(mail);


For more examples, please visit:
http://github.com/MarcDrexler/CAIRO.ElasticEmail 