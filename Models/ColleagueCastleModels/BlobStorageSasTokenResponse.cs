namespace NewHorizon.Models.ColleagueCastleModels
{
    public class BlobStorageSasTokenResponse
    {
        public BlobStorageSasTokenResponse(string sasToken)
        {
            SasToken = sasToken;
        }

        public string SasToken { get; set; }
    }
}
