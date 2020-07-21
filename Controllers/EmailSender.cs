using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Comander.Controllers
{
    public class EmailSender
    {
        string from = "john.cornishon@gmail.com";
        string generalAddress = @"https://localhost:44336/";
        string x = "asdasd";

        public MailMessage VeryfiEmail(string to, string code)
        {
            string subject = CreateSubject();
            string mailBody = BodyBuilder(code);
            MailMessage messageDetail = BuildMessage(from, to, subject, mailBody);

            return messageDetail;
        }

        private string CreateSubject()
        {
            return "temat";
        }

        private string CreateBody()
        {
            return "xxx";
        }

        private MailMessage BuildMessage(
                                            string from,
                                            string to,
                                            string subject,
                                            string mailBody)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(from);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = mailBody;
            message.IsBodyHtml = true;
            
            return message;
        }

        private string BodyBuilder(string code)
        {
            string mailBody;
            mailBody = $"Hello!<br>" +
                $"Please click on your validation link:<br> " +
                $"Validation link: <a href = \"{generalAddress}api/login/getCode?code={code}\">cllick here !!</a><br>" +
                $"Thank you.";
            return mailBody;
        }

        private string TakeBaseBody()
        {
            string baseBody;
            baseBody = $"Click to {x} veryfivation link: \n";

            return baseBody;
        }

        private string GenerateCodeLink(string code)
        {  
            string targetAddress = @"api/login/getCode?code=";
            string fullAddrress = generalAddress + targetAddress + code;
            return fullAddrress;
        }

        public string GenerateRawCode()
        {
            Guid validCode = Guid.NewGuid();
            return validCode.ToString();
        }

        private string BuildHyperLink(string link)
        {
            string prefix = "<a href = \"";
            string afterx = "\"> hit </a>";
            string fullAddrress = prefix + link + afterx;
            return fullAddrress;
        }

        public SmtpClient ConfigureSmtp()
        {
            SmtpClient smtpConfiguration = new SmtpClient();
            smtpConfiguration.Host = "smtp.gmail.com";
            smtpConfiguration.Port = 587;
            smtpConfiguration.Credentials = new NetworkCredential("john.cornishon@gmail.com", "Longinusa2");
            smtpConfiguration.EnableSsl = true;
            

            return smtpConfiguration;
        }

        public void SendEmail(
            SmtpClient smtpConfiguration,
            MailMessage message
            )
        {
            smtpConfiguration.Send(message);
            MailMessage mm = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
        }
    }
}
