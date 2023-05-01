namespace NewHorizon.Constants
{
    public static class SupportedCompanies
    {
        private static string[] NotSupportedEmailDomains = { "gmail.com", "yahoo.com", "outlook.com", "live.com", "icloud.com", "yahoo.in", "aol.com", "zoho.com", "protonmail.com", "gmx.com", "tutanota.com", "mail.com", "fastmail.com", "hushmail.com", "yandex.com", "mailbox.org", "runbox.com", "telia.com", "mybluemail.com", "posteo.de", "mail.ru", "inbox.lv", "163.com", "sina.com", "qq.com", "126.com", "mailfence.com", "startmail.com", "kolabnow.com" };

        public static bool IsCorporateEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress)) 
            {
                return false;
            }
            emailAddress = emailAddress.Trim().ToLower();
            foreach(string emailDomain in NotSupportedEmailDomains)
            {
                if (emailAddress.EndsWith(emailDomain))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
