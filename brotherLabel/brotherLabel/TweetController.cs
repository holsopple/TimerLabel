using LinqToTwitter;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

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
    internal class TweetController
    {
        private static IAuthorizer _auth;
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"]; //Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey);
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];//Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret);


        public async Task Tweet(string status)
        {
            try
            {
                var authTask = DoAuthAsync();
                authTask.Wait();

                if (_auth == null) throw new Exception("auth object null");

                var twitterCtx = new TwitterContext(_auth);

                Trace.TraceInformation(status);
                Trace.Flush();

                var tweet = await twitterCtx.TweetAsync(status);

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

        private static async Task DoAuthAsync()
        {
            ICredentialStore credentials;

            var authDateTimeString = ConfigurationManager.AppSettings["AuthDate"];

            if (authDateTimeString != null && authDateTimeString.Length > 1)
            {
                var authDateTime = Convert.ToDateTime(authDateTimeString);
                if (authDateTime == DateTime.Today)
                {
                    // we authorized today.  don't need to reauth. 
                    // load from app settings 


                    // repopulate credentials. 

                    credentials = new InMemoryCredentialStore
                    {
                        ConsumerKey = ConsumerKey,
                        ConsumerSecret = ConsumerSecret,
                        OAuthToken = ConfigurationManager.AppSettings["oauthToken"],
                        OAuthTokenSecret = ConfigurationManager.AppSettings["oauthTokenSecret"],
                        ScreenName = ConfigurationManager.AppSettings["screenName"],
                        UserID = Convert.ToUInt64(ConfigurationManager.AppSettings["userID"])
                    };
                    _auth = new SingleUserAuthorizer {CredentialStore = credentials};
                    return;
                }
            }

            // else fall through

            _auth = DoPinOAuth();
            await _auth.AuthorizeAsync();

            // write stuff to config. 
            credentials = _auth.CredentialStore;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //  config.AppSettings.Settings["ConsumerKey"].Value = credentials.ConsumerKey;
            //  config.AppSettings.Settings["ConsumerSecret"].Value = credentials.ConsumerSecret;
            config.AppSettings.Settings["oauthToken"].Value = credentials.OAuthToken;
            config.AppSettings.Settings["oauthTokenSecret"].Value = credentials.OAuthTokenSecret;
            config.AppSettings.Settings["screenName"].Value = credentials.ScreenName;
            config.AppSettings.Settings["userID"].Value = Convert.ToString(credentials.UserID);
            config.AppSettings.Settings["AuthDate"].Value = Convert.ToString(DateTime.Today, CultureInfo.CurrentCulture);
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

        private static IAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecret
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

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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
