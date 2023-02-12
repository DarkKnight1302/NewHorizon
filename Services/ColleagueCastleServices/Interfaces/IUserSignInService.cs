namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IUserSignInService
    {
        public bool SignIn(string username, string password);
    }
}
