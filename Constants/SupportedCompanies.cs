namespace NewHorizon.Constants
{
    public static class SupportedCompanies
    {
        private static string[] SupportedCompanyEmailSuffix = { "@microsoft.com" };

        public static bool IsCorporateEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress)) 
            {
                return false;
            }
            emailAddress = emailAddress.Trim().ToLower();
            foreach(string companySuffix in SupportedCompanyEmailSuffix)
            {
                if (emailAddress.EndsWith(companySuffix))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
