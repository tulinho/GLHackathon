using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Resiliens.Repositorios
{
    public class EmailRepositorio
    {
        public void EnviarEmail(string assunto, string remetente, string destinatario, string emCopia, string corpoEmail, bool corpoHtml)
        {
            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EmailSmtp"], ConfigurationManager.AppSettings["SenhaSmtp"]);
                MailMessage mail = new MailMessage();
                mail.Sender = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EmailSmtp"]);
                mail.From = new MailAddress(remetente);
                mail.To.Add(destinatario);
                mail.CC.Add(emCopia);
                mail.Subject = assunto;
                mail.Body = corpoEmail;
                mail.IsBodyHtml = corpoHtml;
                client.Send(mail);
            }
        }
    }
}