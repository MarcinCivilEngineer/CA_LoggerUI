//using OpenPop.Mime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net;
//using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;

namespace CA_LoggerUI
{
    public class LibMail
    {

        public bool WyslijWiadomosc(string od, string doLista, string dwLista, string dwuLista, string temat, string wiadomosc, List<string> zalacznikiLista, string dir, string pass)
        {

            var message = new MimeMessage();

            if (od.Split(" ").ToList().Count() == 1)
            {
                message.From.Add(new MailboxAddress(od, od));
            }
            else
            {
                message.From.Add(new MailboxAddress(od.Replace(od.Split(" ")[0] + " ", ""), od.Split(" ")[0]));
            }

            if (doLista != "")
            {
                foreach (string mail in doLista.Split(", "))
                {
                    var mailSplit = mail.Split(" ").ToList();
                    if (mailSplit.Count() == 1)
                    {
                        message.To.Add(new MailboxAddress(mail, mail));
                    }
                    else
                    {
                        message.To.Add(new MailboxAddress(mail.Replace(mailSplit[0] + " ", ""), mailSplit[0]));
                    }
                }
            }


            if (dwLista != "")
            {
                foreach (string mail in dwLista.Split(", "))
                {
                    var mailSplit = mail.Split(" ").ToList();
                    if (mailSplit.Count() == 1)
                    {
                        message.Cc.Add(new MailboxAddress(mail, mail));
                    }
                    else
                    {
                        message.Cc.Add(new MailboxAddress(mail.Replace(mailSplit[0] + " ", ""), mailSplit[0]));
                    }
                }
            }

            if (dwuLista != "")
            {
                foreach (string mail in dwuLista.Split(", "))
                {
                    var mailSplit = mail.Split(" ").ToList();
                    if (mailSplit.Count() == 1)
                    {
                        message.Bcc.Add(new MailboxAddress(mail, mail));
                    }
                    else
                    {
                        message.Bcc.Add(new MailboxAddress(mail.Replace(mailSplit[0] + " ", ""), mailSplit[0]));
                    }
                }
            }

            //message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", "chandler@friends.com"));
            message.Subject = temat;

            var body = new TextPart("html")
            {
                Text = wiadomosc
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);

            if (zalacznikiLista.Count() > 0)
            {
                foreach (var zalacznik in zalacznikiLista)
                {
                    var attachment = new MimePart()
                    {
                        Content = new MimeContent(File.OpenRead(zalacznik), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(zalacznik)
                    };
                    multipart.Add(attachment);
                }
            }
            message.Body = multipart;

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);


                // Note: only needed if the SMTP server requires authentication

                if (pass != "") { 
                client.Authenticate(od.Split(" ")[0], pass);
                }
                //client.Authenticate(od, pass);
                client.Send(message);

                client.Disconnect(true);
            }
            return true;

        }






        /*
public bool WyslijWiadomoscLast(string od, string doLista, string dwLista, string dwuLista, string temat, string wiadomosc, List<Attachment> zalacznikiLista, string dir,string pass)
        {
            

            using (var client = new SmtpClient())
            {

                List<string> mailSplit = new List<string>();
                MailAddressCollection mailAddressesCol = new MailAddressCollection();
                MailAddress mailAddressOd;


                if (od.Split(" ").ToList().Count() == 1)
                {
                    mailAddressOd = new MailAddress(od);
                }
                else
                {
                    mailAddressOd = new MailAddress(od.Split(" ")[0], od.Replace(od.Split(" ")[0] + " ", ""));
                }

                foreach (string mail in doLista.Split(", "))
                {
                    mailSplit = mail.Split(" ").ToList();
                    if (mailSplit.Count() == 1)
                    {
                        mailAddressesCol.Add(new MailAddress(mail, mail));
                    }
                    else
                    {
                        mailAddressesCol.Add(new MailAddress(mailSplit[0], mail.Replace(mailSplit[0] + " ", "")));
                    }
                }

                MailMessage msg = new MailMessage(mailAddressOd, mailAddressesCol.First()) ;

                mailAddressesCol.RemoveAt(0);
                foreach (MailAddress mailAddress in mailAddressesCol)
                {
                    msg.To.Add(mailAddress);
                }

                msg.Subject = temat;

                if(dwLista != "") { 
                foreach (string mail in dwLista.Split(", "))
                {
                    mailSplit = mail.Split(" ").ToList();
                    if (mailSplit.Count() == 1)
                    {
                        msg.CC.Add(new MailAddress(mail, mail));
                    }
                    else
                    {
                        msg.CC.Add(new MailAddress(mailSplit[0], mail.Replace(mailSplit[0] + " ", "")));
                    }
                }
                }

                if (dwuLista != "")
                {
                    foreach (string mail in dwuLista.Split(", "))
                    {
                        mailSplit = mail.Split(" ").ToList();
                        if (mailSplit.Count() == 1)
                        {
                            msg.Bcc.Add(new MailAddress(mail, mail));
                        }
                        else
                        {
                            msg.Bcc.Add(new MailAddress(mailSplit[0], mail.Replace(mailSplit[0] + " ", "")));
                        }
                    }
                }

                if (zalacznikiLista.Count() > 0) { 
                foreach (Attachment attachment in zalacznikiLista)
                {
                    msg.Attachments.Add(attachment);
                }
                }
                msg.IsBodyHtml = true;
                msg.BodyEncoding = System.Text.Encoding.ASCII;
                msg.SubjectEncoding = System.Text.Encoding.ASCII;
                msg.HeadersEncoding = System.Text.Encoding.ASCII;
                msg.ReplyTo=mailAddressOd;
                msg.Body = wiadomosc;

                try
                {
                    client.Host = "smtp.office365.com";
                    client.Credentials = new NetworkCredential(mailAddressOd.Address, pass);
                    client.UseDefaultCredentials = false;
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.PickupDirectoryLocation = dir;
                    //client.DeliveryFormat=
                    //SaveMailIntoFile(msg.ToEml(), dir + @"\" + temat + @".eml");

                    //System.IO.File.WriteAllBytes(dir + @"\" + temat + @".eml", msg.ToEml());

                    client.Send(msg);
                    ///client.Send(msg);
                    //client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    ///client.UseDefaultCredentials = true;
                    
                    //client.Send(msg);


                    

                    return true;
                }
                catch (Exception ex)
                {
                    string tmpError = "";
                    tmpError = "Exception caught: " + ex.ToString();
                    return false;
                }


                

            }


        }
        */


        /// <summary>
        /// Example showing:
        ///  - how to fetch only headers from a POP3 server
        ///  - how to examine some of the headers
        ///  - how to fetch a full message
        ///  - how to find a specific attachment and save it to a file
        /// </summary>
        /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
        /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
        /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
        /// <param name="username">Username of the user on the server</param>
        /// <param name="password">Password of the user on the server</param>
        /// <param name="messageNumber">
        /// The number of the message to examine.
        /// Must be in range [1, messageCount] where messageCount is the number of messages on the server.
        /// </param>
        /// 
        /*
        public static void HeadersFromAndSubject(string hostname, int port, bool useSsl, string username, string password, int messageNumber)
        {
            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect(hostname, port, useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(username, password);

                // We want to check the headers of the message before we download
                // the full message
                MessageHeader headers = client.GetMessageHeaders(messageNumber);

                RfcMailAddress from = headers.From;
                string subject = headers.Subject;

                // Only want to download message if:
                //  - is from test@xample.com
                //  - has subject "Some subject"
                if (from.HasValidMailAddress && from.Address.Equals("test@example.com") && "Some subject".Equals(subject))
                {
                    // Download the full message
                    Message message = client.GetMessage(messageNumber);

                    // We know the message contains an attachment with the name "useful.pdf".
                    // We want to save this to a file with the same name
                    foreach (MessagePart attachment in message.FindAllAttachments())
                    {
                        if (attachment.FileName.Equals("useful.pdf"))
                        {
                            // Save the raw bytes to a file
                            File.WriteAllBytes(attachment.FileName, attachment.Body);
                        }
                    }
                }
            }
        }
        */
        /// <summary>
        /// Example showing:
        ///  - how to save a message to a file
        ///  - how to load a message from a file at a later point
        /// </summary>
        /// <param name="message">The message to save and load at a later point</param>
        /// <returns>The Message, but loaded from the file system</returns>

/*
        public static Message SaveAndLoadFullMessage(Message message)
        {
            // FileInfo about the location to save/load message
            FileInfo file = new FileInfo("someFile.eml");

            // Save the full message to some file
            message.Save(file);

            // Now load the message again. This could be done at a later point
            Message loadedMessage = Message.Load(file);

            // use the message again
            return loadedMessage;
        }

        */






        /*
            public static List<Message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password)
            {
                // The client disconnects from the server when being disposed
                using (Pop3Client client = new Pop3Client())
                {
                    // Connect to the server
                    client.Connect(hostname, port, useSsl);

                    // Authenticate ourselves towards the server
                    client.Authenticate(username, password);

                    // Get the number of messages in the inbox
                    int messageCount = client.GetMessageCount();

                    // We want to download all messages
                    List<Message> allMessages = new List<Message>(messageCount);

                    // Messages are numbered in the interval: [1, messageCount]
                    // Ergo: message numbers are 1-based.
                    // Most servers give the latest message the highest number
                    for (int i = messageCount; i > 0; i--)
                    {
                        allMessages.Add(client.GetMessage(i));
                    }

                    // Now return the fetched messages
                    return allMessages;
                }
            }
        */

        /*
            /// <summary>
            /// Example showing:
            ///  - how to a find plain text version in a Message
            ///  - how to save MessageParts to file
            /// </summary>
            /// <param name="message">The message to examine for plain text</param>
            public static void FindPlainTextInMessage(Message message)
            {
                MessagePart plainText = message.FindFirstPlainTextVersion();
                if (plainText != null)
                {
                    // Save the plain text to a file, database or anything you like
                    plainText.Save(new FileInfo("plainText.txt"));
                }
            }

            /// <summary>
            /// Example showing:
            ///  - how to find a html version in a Message
            ///  - how to save MessageParts to file
            /// </summary>
            /// <param name="message">The message to examine for html</param>
            public static void FindHtmlInMessage(Message message)
            {
                MessagePart html = message.FindFirstHtmlVersion();
                if (html != null)
                {
                    // Save the plain text to a file, database or anything you like
                    html.Save(new FileInfo("html.txt"));
                }
            }

            /// <summary>
            /// Example showing:
            ///  - how to find a MessagePart with a specified MediaType
            ///  - how to get the body of a MessagePart as a string
            /// </summary>
            /// <param name="message">The message to examine for xml</param>
            public static void FindXmlInMessage(Message message)
            {
                MessagePart xml = message.FindFirstMessagePartWithMediaType("text/xml");
                if (xml != null)
                {
                    // Get out the XML string from the email
                    string xmlString = xml.GetBodyAsText();

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    // Load in the XML read from the email
                    doc.LoadXml(xmlString);

                    // Save the xml to the filesystem
                    doc.Save("test.xml");
                }
            }

        */


        /*
        public static void SaveMailIntoFile(MailMessage Message, string FileName)
        {
            Assembly assembly = typeof(SmtpClient).Assembly;
            Type _mailWriterType =
              assembly.GetType("System.Net.Mail.MailWriter");

            using (FileStream _fileStream =
                   new FileStream(FileName, FileMode.Create))
            {
                // Get reflection info for MailWriter contructor
                ConstructorInfo _mailWriterContructor =
                    _mailWriterType.GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new Type[] { typeof(Stream) },
                        null);

                // Construct MailWriter object with our FileStream
                object _mailWriter =
                  _mailWriterContructor.Invoke(new object[] { _fileStream });

                // Get reflection info for Send() method on MailMessage
                MethodInfo _sendMethod =
                    typeof(MailMessage).GetMethod(
                        "Send",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                // Call method passing in MailWriter
                _sendMethod.Invoke(
                    Message,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { _mailWriter, true },
                    null);

                // Finally get reflection info for Close() method on our MailWriter
                MethodInfo _closeMethod =
                    _mailWriter.GetType().GetMethod(
                        "Close",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                _closeMethod.Invoke(
                    _mailWriter,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { },
                    null);
            }
        }
        */


    }
}
/*
public static class MailMessageExtensions
{
    public static byte[] ToEml(this MailMessage message)
    {
        var assembly = typeof(SmtpClient).Assembly;
        var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

        using (var memoryStream = new MemoryStream())
        {
            // Get reflection info for MailWriter contructor
            var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

            // Construct MailWriter object with our FileStream
            var mailWriter = mailWriterContructor.Invoke(new object[] { memoryStream });

            // Get reflection info for Send() method on MailMessage
            var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

            // Call method passing in MailWriter
            sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true, true }, null);

            // Finally get reflection info for Close() method on our MailWriter
            var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

            // Call close method
            closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);

            return memoryStream.ToArray();
        }
    }
}
*/