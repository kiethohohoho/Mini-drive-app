using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace miniDriveApp.Services
{
    public class GoogleDriveService
    {
        public GoogleDriveService()
        {
        }
        public async Task<UserCredential> AuthorizeAsync(params string[] Scopes)
        {
            //clear duwx lieeuj ng dung
            var store = new FileDataStore("DriveProfile");
            //await store.ClearAsync();

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = "174888762231-hk2ls565afp1ac6ecr7cqi36r91b5fs1.apps.googleusercontent.com",
                        ClientSecret = "1C0O4D1hy_OsqxvKtmGctFz-"
                    },
                    Scopes.Concat(new string[] { Oauth2Service.Scope.UserinfoEmail, Oauth2Service.Scope.UserinfoProfile }),
                    Environment.UserName,
                    CancellationToken.None,
                    store);
        }
        public async Task<Userinfo> GetUserinfo()
        {
            var Identity = await AuthorizeAsync(DriveService.Scope.Drive);
            var oauthSerivce = new Oauth2Service(
               new BaseClientService.Initializer()
               {
                   HttpClientInitializer = Identity,
                   ApplicationName = "OAuth 2.0 Sample",
               });
            return await oauthSerivce.Userinfo.Get().ExecuteAsync();
        }
        public async Task<DriveService> CreateDriveSevericeAsync(UserCredential userCredential = null)
        {
            var Identity = userCredential == null ? await AuthorizeAsync(DriveService.Scope.Drive) : userCredential;

            return new DriveService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = Identity
            });
        }

    }
}
