﻿namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface ISessionTokenManager
    {
        public Task<string> GenerateSessionToken(string userId);

        public Task<bool> ValidateSessionToken(string userId, string sessionToken);

        public Task<bool> ValidateSessionToken(string sessionToken);

        public Task<string> GetUserNameFromToken(string sessionToken);

        public void DeleteUserSession(string userId, string sessionToken);
    }
}
