# CAIRO.ElasticEmail
Simple wrapper to send emails through elasticemail service


###Sending Email
```c#
using CAIRO.ElasticEmail;

MailSender client = new MailSender("", "");
var mail = new Mail();
mail.ToAddress = "to@example.com";
mail.FromAddress = "from@example.com";
mail.ReplyToAddress = "from@example.com";
mail.Subject = "Simple Message";
mail.Body = "Test Message";
var result = client.Send(mail);
```

###Sending Email with attachments
```c#
using CAIRO.ElasticEmail;

MailSender client = new MailSender("", "");
var mail = new Mail();
mail.ToAddress = "to@example.com";
mail.FromAddress = "from@example.com";
mail.ReplyToAddress = "from@example.com";
mail.Subject = "Message with 2 attachments";
mail.Body = "Test Message";
mail.AddAttachment("C:\temp\file1.txt");          // specifiy an existing filename with full path
mail.AddAttachment("file2.txt", new byte[100]);   // or pass the attachments filename and file content
var result = client.Send(mail);
```
