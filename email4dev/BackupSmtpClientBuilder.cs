using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;

namespace email4dev
{
    public class BackupSmtpClientBuilder
    {
 
        public static SmtpClient Build()
        {
            var smtp = new SmtpClient();
            var section = (SmtpSection)ConfigurationManager.GetSection("backupMailSettings/smtp");

            if (section.Network != null)
            {
                if (section.Network.Host != null)
                    smtp.Host = section.Network.Host;
                smtp.Port = section.Network.Port;
                smtp.EnableSsl = section.Network.EnableSsl;
                if (section.Network.TargetName != null)
                    smtp.TargetName = section.Network.TargetName;
            }
            smtp.DeliveryMethod = section.DeliveryMethod;
            smtp.PickupDirectoryLocation = section.SpecifiedPickupDirectory.PickupDirectoryLocation;
            return smtp;
        }
    }
}
