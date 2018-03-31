using System;
using System.Threading.Tasks;

namespace Slink
{
    public interface IBroadcastNotificaion
    {
        void SendNotificaion(string key);
    }

    public interface IDatabase
    {
        string GetDatabasePath();
    }

    public interface IFacebook
    {
        void Logout();
    }

    public interface IImageDownloader
    {
        void PredownloadImages();
    }


    public interface INavigation
    {
        void GoToMain();
        void GoToLists();
    }

    public interface IPersistantStorage
    {
        string GetUserId();
        void SetUserId(string userid);

        string GetEmail();
        void SetEmail(string email);

        string GetFirstName();
        void SetFirstName(string firstName);

        string GetLastName();
        void SetLastName(string lastName);

        string GetPassword();
        void SetPassword(string password);

        string GetAccessToken();
        void SetAccessToken(string token);

        DateTime GetTokenExpiry();
        void SetTokenExpiry(DateTime expiry);

        string GetAccessScope();
        void SetAccessScope(string scope);

        string GetRefreshToken();
        void SetRefreshToken(string token);

        string GetFacebookToken();
        void SetFacebookToken(string scope);

        DateTime GetFacebookTokenExpiry();
        void SetFacebookTokenExpiry(DateTime expiry);

        string GetDesignType();
        void SetDesignType(string designType);

        string GetPushToken();
        void SetPushToken(string token);

        int GetDiscoverNotificationCount();
        void SetDiscoverNotificaionCount(int count);
        void IncrementDiscoverNotificaionCount(int incrementalValue);

        void RemoveAll();

    }

    public interface IS3Service
    {
        byte[] ResizeImage(byte[] imageData, float width, float height);
    }
}