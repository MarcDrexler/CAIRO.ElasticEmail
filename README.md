# CAIRO.ElasticEmail

This is just a simple (inofficial) wrapper to send emails easier with the Elastic Email HTTP API.
Elastic Email is a powerful email platform built from the ground up to send your email efficiently.
Read more on www.elasticemail.com


#Requirements

This library requires .NET 4.5 and above.

#Installation

To use Elasticemail in your C# project, you can download the Elasticemail C# .NET library directly from our Github repository or, if you have the NuGet package manager installed, you can grab them automatically.

```
PM> Install-Package CAIRO.ElasticEmail 
```

Once you have the ElasticEmail library properly referenced in your project, you can include calls to them in your code. 

Add the following namespaces to use the library:
```csharp
using CAIRO.ElasticEmail;
using System.Net.Mail;
```

#How to: Create an email

Use the **new ElasticemailMessage** constructor to create an email message that is of type **ElasticemailMessage**. Once the message is created, you can use **ElasticemailMessage** properties and methods to set values including the email sender, the email recipient, and the subject and body of the email.

The following example demonstrates how to create an email object and populate it:

```csharp
// Create the email object first, then add the properties.
var myMessage = new ElasticemailMessage();

// Add the message properties.
myMessage.From = new MailAddress("john@example.com");
myMessage.To.Add(new MailAddress("anna@example.com");
myMessage.Subject = "Testing the Elasticemail Library";

// Add Text body
myMessage.Body = "Hello World plain text!";

// Or add HTML body
myMessage.Body = "<p>Hello World!</p>";
myMessage.IsHtmlBody = true;
```

#How to: Send an Email

After creating an email message, you can send it using the Web API provided by Elasticemail.

Sending email requires that you supply your Elasticemail username and API Key.

To send an email message, use the **Send** method on the **ElasticemailWebApi** class, which calls the Elasticemail Web API. The following example shows how to send a message. The response is of type **SendResult** and contains a property **transactionId**. You can use this id to query the delivery status at a later time with the **GetDeliveryStatus** Method.

```csharp
// Create the email object first, then add the properties.
Mail myMessage = new ElasticemailMessage();
myMessage.From = new MailAddress("john@example.com");
myMessage.To.Add(new MailAddress("anna@example.com");
myMessage.Subject = "Testing the Elasticemail Library";
myMessage.Text = "Hello World!";

// Create a Client, using API Key.
var client = new ElasticemailWebApi("apiKey");

// Send the email.
SendResult result = client.Send(myMessage);
```

#How to: Add an Attachment

Attachments can be added to a message by calling the **AddAttachment** method and specifying the name and path of the file you want to attach, or by passing a byte array. You can include multiple attachments by calling this method once for each file you wish to attach. The following example demonstrates adding an attachment to a message:

```csharp
Mail myMessage = new ElasticemailMessage();
myMessage.From = new MailAddress("john@example.com");
myMessage.To.Add(new MailAddress("anna@example.com");
myMessage.Subject = "Testing the Elasticemail Library";
myMessage.Text = "Hello World!";

myMessage.AddAttachment(@"C:\file1.txt");
```

You can also add attachments by passing in the **byte[]** of the data. It can be done by calling the same method as above, **AddAttachment**, but by passing two parameters, the data and the filename you want it to show as in the message.

```csharp
Mail myMessage = new ElasticemailMessage();
myMessage.From = new MailAddress("john@example.com");
myMessage.To.Add(new MailAddress("anna@example.com");
myMessage.Subject = "Testing the Elasticemail Library";
myMessage.Text = "Hello World!";

byte[] data = File.ReadAllBytes(@"C:\file1.txt");
myMessage.AddAttachment("MyFile.txt", data);
```

#How to: Get Delivery Status

Use the **GetDeliveryStatus** method to determine if your transactional email delivery was successful. The **DeliveryStatus** object will also tell you how many emails were delivered, failed or still pending.

```csharp
var client = new ElasticemailWebApi("apiKey");
DeliveryStatusResponse response = client.GetDeliveryStatus(Guid.Parse("transactionId"));
if (response.DeliveryStatus.Status == "complete")
{
    response.DeliveryStatus.Delivered
    response.DeliveryStatus.Failed
    response.DeliveryStatus.Pending
    ...
}
```
