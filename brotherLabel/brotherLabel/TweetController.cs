using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Configuration;

/******************************************************************************
 * Copyright Holsopple 2017                                                    *
 * All rights reserved                                                         *
 * Req Id       : Autocorss Timing                                             *
 * Program Name : Twitter class                                                *
 * File Name    : TweetController.cs                                           *
 * Purpose      : Initialization for the twitter to make                       *
 *                   tweets to the twitter account                             *
 *******************************************************************************
 * Program History:                                                            *
 * ---------------                                                             *
 * Version     Date       Author          Modification                         *
 * -------- -----------  -----------  ----------------                         *
 *  1.0      2/11/2017   Gary Holsopple Created                                *
 ******************************************************************************/



namespace com.holsopple.BrotherLabel
{
    class TweetController
    {
        static IAuthorizer auth;

        public async Task Tweet(String status)
        {
            try
            {

                Task authTask = DoAuthAsync();
                authTask.Wait();

                if (auth == null) throw new Exception("auth object null");

                var twitterCtx = new TwitterContext(auth);

                Trace.TraceInformation(status);
                Trace.Flush();

                Status tweet = await twitterCtx.TweetAsync(status);

                Trace.TraceInformation("success:" + tweet.CreatedAt);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Trace.TraceInformation(ex.Message);
                Trace.Flush();
                throw;
            }
        }


        static async Task DoAuthAsync()
        {

            ICredentialStore credentials;
            string oauthToken;
            string oauthTokenSecret;
            string screenName;
            ulong userID;


            String authDateTimeString = ConfigurationManager.AppSettings["AuthDate"];
            DateTime authDateTime;

            if (authDateTimeString != null && authDateTimeString.Length > 1)
            {
                authDateTime = Convert.ToDateTime(authDateTimeString);
                if (authDateTime == DateTime.Today)
                {
                    // we authorized today.  don't need to reauth. 
                    // load from app settings 
                    oauthToken = ConfigurationManager.AppSettings["oauthToken"];
                    oauthTokenSecret = ConfigurationManager.AppSettings["oauthTokenSecret"];
                    screenName = ConfigurationManager.AppSettings["screenName"];
                    userID = Convert.ToUInt64(ConfigurationManager.AppSettings["userID"]);

                    // repopulate credentials. 

                    credentials = new InMemoryCredentialStore();
                    credentials.ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey);
                    credentials.ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret);
                    credentials.OAuthToken = oauthToken;
                    credentials.OAuthTokenSecret = oauthTokenSecret;
                    credentials.ScreenName = screenName;
                    credentials.UserID = userID;
                    auth = new SingleUserAuthorizer();
                    auth.CredentialStore = credentials;
                    return;
                }
            }

            // else fall through

            auth = DoPinOAuth();
            await auth.AuthorizeAsync();

            // write stuff to config. 
            credentials = auth.CredentialStore;

            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["oauthToken"].Value= credentials.OAuthToken;
            config.AppSettings.Settings["oauthTokenSecret"].Value= credentials.OAuthTokenSecret;
            config.AppSettings.Settings["screenName"].Value=credentials.ScreenName;
            config.AppSettings.Settings["userID"].Value= Convert.ToString(credentials.UserID);
            config.AppSettings.Settings["AuthDate"].Value=Convert.ToString(DateTime.Today);
            //save to apply changes
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            // This is how you access credentials after authorization.
            // The oauthToken and oauthTokenSecret do not expire.
            // You can use the userID to associate the credentials with the user.
            // You can save credentials any way you want - database, isolated storage, etc. - it's up to you.
            // You can retrieve and load all 4 credentials on subsequent queries to avoid the need to re-authorize.
            // When you've loaded all 4 credentials, LINQ to Twitter will let you make queries without re-authorizing.
        }

        static IAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };

            return auth;
        }



        public static void ResetAuthorization()
        {

            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["oauthToken"].Value = "";
            config.AppSettings.Settings["oauthTokenSecret"].Value = "";
            config.AppSettings.Settings["screenName"].Value = "";
            config.AppSettings.Settings["userID"].Value = "";
            config.AppSettings.Settings["AuthDate"].Value = "";
            //save to apply changes
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

        }



    }
}
