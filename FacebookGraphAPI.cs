using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace FB_API_Plugin
{

    /// <summary>
    /// A client for the Facebook Graph API.
    /// 
    /// See http://developers.facebook.com/docs/api for complete documentation
    /// for the API.
    /// 
    /// The Graph API is made up of the objects in Facebook (e.g., people, pages,
    /// events, photos) and the connections between them (e.g., friends,
    /// photo tags, and event RSVPs). This client provides access to those
    /// primitive types in a generic way. For example, given an OAuth access
    /// token, this will fetch the profile of the active user and the list
    /// of the user's friends:
    /// 
    ///    var facebook = new FacebookGraphAPI(args["access_token"]);
    ///    var user = facebook.GetObject("me", null);
    ///    var friends = facebook.GetConnections("me", "friends", null);
    /// 
    /// You can see a list of all of the objects and connections supported
    /// by the API at http://developers.facebook.com/docs/reference/api/.
    /// 
    /// You can obtain an access token via OAuth or by using the Facebook
    /// JavaScript SDK. See http://developers.facebook.com/docs/authentication/
    /// </summary>
    internal class FacebookGraphApi
    {

        string _accessToken = null;

        public FacebookGraphApi(string accessToken)
        {
            _accessToken = accessToken;
            FBversion = "v1.0";
            ProxyStr = "";
        }

        /// <summary>
        /// Fetchs the given object from the graph.
        /// </summary>
        /// <param name="id">Id of the object to fetch</param>
        /// <param name="args">List of arguments</param>
        /// <returns>The required object</returns>
        public JObject GetObject(string id, Dictionary<string, string> args)
        {
            return Request(id, args, null);
        }

        /// <summary>
        /// Fetchs all of the given object from the graph.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ids">Ids of the objects to return</param>
        /// <returns>
        /// A map from ID to object. If any of the IDs are invalid, an exception is raised
        /// </returns>
        public JObject GetObjects(Dictionary<string, string> args, params string[] ids)
        {
            var joinedIds = "";
            for (var i = 0; i < ids.Length; i++) if (i == 0) joinedIds += ids[i]; else joinedIds += "," + ids[i];
            if (args == null) args = new Dictionary<string, string>();
            args["ids"] = joinedIds;

            return Request("", args, null);
        }

        /// <summary>
        /// Fetchs the connections for given object.
        /// </summary>
        /// <param name="id">Id of the object to fetch</param>
        /// <param name="connectionName">Name of the connection</param>
        /// <param name="args">List of arguments</param>
        /// <returns>A JObject containing the required connections</returns>
        public JObject GetConnections(string id, string connectionName, Dictionary<string, string> args)
        {
            return Request(id + "/" + connectionName, args, null);
        }

        /// <summary>
        /// Writes the given object to the graph, connected to the given parent.
        /// 
        /// For example,
        /// 
        ///     var data = new Dictionary&lt;string, string&gt;();
        ///     data.Add("message", "Hello, world");
        ///     facebook.PutObject("me", "feed", data);
        /// 
        /// writes "Hello, world" to the active user's wall.
        /// 
        /// See http://developers.facebook.com/docs/api#publishing for all of
        /// the supported writeable objects.
        /// 
        /// Most write operations require extended permissions. For example,
        /// publishing wall posts requires the "publish_stream" permission. See
        /// http://developers.facebook.com/docs/authentication/ for details about
        /// extended permissions.
        /// </summary>
        /// <param name="parentObject">The parent object</param>
        /// <param name="connectionName">The connection name</param>
        /// <param name="data">Post data</param>
        /// <returns>A JObject with the result of the operation</returns>
        public JObject PutObject(string parentObject, string connectionName, Dictionary<string, string> data)
        {
            if (_accessToken == null) throw new FacebookGraphApiException("Authentication", "Access Token Required");
            return Request(parentObject + "/" + connectionName, null, data);
        }


        /// <summary>
        /// Writes a wall post to current user wall.
        /// 
        /// We default to writing to the authenticated user's wall if no
        /// profile_id is specified.
        /// 
        /// attachment adds a structured attachment to the status message being
        /// posted to the Wall. It should be a dictionary of the form:
        /// 
        ///     {"name": "Link name"
        ///      "link": "http://www.example.com/",
        ///      "caption": "{*actor*} posted a new review",
        ///      "description": "This is a longer description of the attachment",
        ///      "picture": "http://www.example.com/thumbnail.jpg"}
        /// </summary>
        /// <param name="message">The message to put on the wall</param>
        /// <param name="attachment">Optional attachment for the message</param>
        /// <returns>A JObject with the result of the operation</returns>
        public JObject PutWallPost(string message, Dictionary<string, string> attachment)
        {
            if (attachment == null) attachment = new Dictionary<string, string>();
            attachment.Add("message", message);
            return PutObject("me", "feed", attachment);
        }

        /// <summary>
        /// Writes a wall post to the given profile's wall.
        /// 
        /// We default to writing to the authenticated user's wall if no
        /// profile_id is specified.
        /// 
        /// attachment adds a structured attachment to the status message being
        /// posted to the Wall. It should be a dictionary of the form:
        /// 
        ///     {"name": "Link name"
        ///      "link": "http://www.example.com/",
        ///      "caption": "{*actor*} posted a new review",
        ///      "description": "This is a longer description of the attachment",
        ///      "picture": "http://www.example.com/thumbnail.jpg"}
        /// </summary>
        /// <param name="message">The message to put on the wall</param>
        /// <param name="attachment">Optional attachment for the message</param>
        /// <param name="profileId">The profile where the message is goint to be put</param>
        /// <returns>A JObject with the result of the operation</returns>
        public JObject PutWallPost(string message, Dictionary<string, string> attachment, string profileId)
        {
            if (attachment == null) attachment = new Dictionary<string, string>();
            attachment.Add("message", message);
            return PutObject(profileId, "feed", attachment);
        }

        /// <summary>
        /// Writes the given comment on the given post.
        /// </summary>
        /// <param name="objectId">Id of the object</param>
        /// <param name="message">Message</param>
        /// <returns>A JObject with the result of the operation</returns>
        public JObject PutComment(string objectId, string message)
        {
            var args = new Dictionary<string, string>();
            args.Add("message", message);
            return PutObject(objectId, "comments", args);
        }

        /// <summary>
        /// Likes the given post.
        /// </summary>
        /// <param name="objectId">Id of the object to be like</param>
        /// <returns>A JObject with the result of the operation</returns>
        public JObject PutLike(string objectId)
        {
            return PutObject(objectId, "likes", new Dictionary<string, string>());
        }

        /// <summary>
        /// Deletes the object with the given ID from the graph.
        /// </summary>
        /// <param name="id">Id of the object to delete</param>
        /// <returns>A JObject with the result of the operation</returns>
        public JObject DeleteObject(string id)
        {
            var postArgs = new Dictionary<string, string>();
            postArgs.Add("method", "delete");
            return Request(id, null, postArgs);
        }

        /// <summary>
        /// Fetches the given path in the Graph API.
        /// 
        /// Translates args to a valid query string. If post_args is given,
        /// sends a POST request to the given path with the given arguments.
        /// </summary>
        /// <param name="path">The path where the request is to be send</param>
        /// <param name="args">The Query arguments</param>
        /// <param name="postArgs">The POST arguments</param>
        /// <returns>A JObject of the facebook response</returns>
        private JObject Request(string path, Dictionary<string, string> args, Dictionary<string, string> postArgs)
        {
            if (args == null) args = new Dictionary<string, string>();
            if (_accessToken != null)
            {
                if (postArgs != null) postArgs["access_token"] = _accessToken;
                else args["access_token"] = _accessToken;
            }

            string postData = null;
            if (postArgs != null) postData = EncodeDictionary(postArgs);

            var reply = "";
            using (var wc = new WebClient())
            {

                if (ProxyStr != string.Empty)
                {
                    var proxy = ProxyStr.Split(':');
                    wc.Proxy = new WebProxy(proxy[0], Convert.ToInt32(proxy[1]));
                    if (proxy.Length == 4)
                    {
                        wc.Proxy.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                    }
                }

                wc.Encoding = Encoding.UTF8;
                try
                {
                    if (postArgs == null)
                    {
                        reply = wc.DownloadString("https://graph.facebook.com/" + (FBversion == "none" ? string.Empty : FBversion + "/") + path + (EncodeDictionary(args) != string.Empty ? "?" + EncodeDictionary(args) : ""));
                    }
                    else
                    {
                        if (args == null)
                        { 
                            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                            reply = wc.UploadString("https://graph.facebook.com/" + (FBversion == "none" ? string.Empty : FBversion + "/") + path, "POST", postData);
                        }
                        else
                        {
                            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                            reply = wc.UploadString("https://graph.facebook.com/" + (FBversion == "none" ? string.Empty : FBversion + "/") + path + "?" + EncodeDictionary(args), "POST", postData);
                        }
                    }
                }
                catch (WebException ex)
                {
                    var sr = new StreamReader(ex.Response.GetResponseStream(), wc.Encoding);
                    reply = sr.ReadToEnd();
                }
            }
            JsonText = reply;
            if ((reply.Trim() == "true")||(reply.Trim() == "false"))
            {
                var rep = "{\"result\": {\"bool\": \"" + reply.Trim() + "\"}}";
                reply = rep;
            }
            var jo = JObject.Parse(reply);
            if (jo["error"] != null)
                throw new FacebookGraphApiException(jo["error"]["type"].ToString(),
                    jo["error"]["message"].ToString());

            return jo;
        }


        /// <summary>
        /// Encodes a dictionary keys to send them via HTTP request
        /// </summary>
        /// <param name="dict">Dictionary to be encoded</param>
        /// <returns>Encoded dictionary keys</returns>
        private string EncodeDictionary(Dictionary<string, string> dict)
        {
            var ret = "";
            if (dict == null) return ret;
            foreach (var item in dict)
                ret += HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value) + "&";
            ret = ret.TrimEnd('&');
            return ret;
        }

        /// <summary>
        /// 
        /// cookies should be a dictionary-like object mapping cookie names to
        /// cookie values.
        /// 
        /// If the user is logged in via Facebook, we return a dictionary with the
        /// keys "uid" and "access_token". The former is the user's Facebook ID,
        /// and the latter can be used to make authenticated requests to the Graph API.
        /// If the user is not logged in, we return None.
        /// 
        /// </summary>
        /// <param name="cookies">HttpCookieCollection</param>
        /// <param name="appId">Facebook Application Id</param>
        /// <param name="appSecret">Facebook Application Secret</param>
        /// <returns>Dictionary with the keys "uid" and "access_token"</returns>
        public static Dictionary<string, string> GetUserFromCookie(HttpCookieCollection cookies, string appId, string appSecret)
        {

            var args = new Dictionary<string, string>();
            var fbsig = HttpUtility.UrlDecode(cookies["fbs_" + appId].Value.Trim('"')).Split('&');
            foreach (var s in fbsig)
            {
                var tmp = s.Split('=');
                args.Add(tmp[0], tmp[1]);
            }
            var sortedArgs = (from entry in args orderby entry.Key ascending select entry);

            var payload = "";
            foreach (var item in sortedArgs) if (item.Key != "sig") payload += item.Key + "=" + item.Value;

            var sig = Md5Hash(payload + appSecret);
            var expires = int.Parse(args["expires"]);
            var epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            if (sig == args["sig"] && (expires == 0 || epoch < expires)) return args;
            return null;
        }

        /// <summary>
        /// Parses the get parameters sent on canvas iframe.
        /// 
        /// You need to enable the OAuth 2.0 for Canvas option in 
        /// Application -> Edit Settings -> Advanced -> Migrations
        /// 
        /// If the user is logged in via Facebook, we return a dictionary with the
        /// keys "uid" and "access_token". The former is the user's Facebook ID,
        /// and the latter can be used to make authenticated requests to the Graph API.
        /// If the user is not logged in, we return None.
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <param name="appId">Facebook Application Id</param>
        /// <param name="appSecret">Facebook Application Secret</param>
        /// <returns>Dictionary with the keys "uid" and "access_token"</returns>
        public static Dictionary<string, string> GetUserFromQueryString(HttpRequest request, string appId, string appSecret)
        {

            var args = new Dictionary<string, string>();
            //var signed_request = request.QueryString["signed_request"]; // Deprecate, POST for Canvas disabled 
            var signedRequest = request.Form["signed_request"];
            if (signedRequest == null) return null;

            var splitted = signedRequest.Split('.');
            var encodedSig = splitted[0];
            var payload = splitted[1];

            var sig = Base64UrlDecode(encodedSig);
            var jObject = JObject.Parse(Base64UrlDecode(payload));
            var algorithm = jObject["algorithm"].ToString().ToUpper().Trim('"');
            if (algorithm != "HMAC-SHA256") return null;

            var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
            hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var encoding = new UTF8Encoding();
            var expectedSig = encoding.GetString(hmacsha256.Hash);
            if (sig != expectedSig) return null;

            var data = new Dictionary<string, string>();
            data.Add("user_id", (string)jObject["user_id"] ?? "");
            data.Add("uid", (string)jObject["user_id"] ?? "");
            data.Add("oauth_token", (string)jObject["oauth_token"] ?? "");
            data.Add("access_token", (string)jObject["oauth_token"] ?? "");
            var expires = ((long?) jObject["expires"] ?? 0);
            data.Add("expires", expires > 0 ? expires.ToString() : "") ;
            data.Add("profile_id", (string)jObject["profile_id"] ?? "");

            return data;
        }

        /// <summary>
        /// Performs base 64 url safe decoding
        /// </summary>
        /// <param name="str">The string to be decoded</param>
        /// <returns></returns>
        private static string Base64UrlDecode(string str)
        {
            var encoding = new UTF8Encoding();
            var decodedJson = str.Replace("=", string.Empty).Replace('-', '+').Replace('_', '/');
            var base64JsonArray = Convert.FromBase64String(decodedJson.PadRight(decodedJson.Length + (4 - decodedJson.Length % 4) % 4, '='));
            return encoding.GetString(base64JsonArray);
        }

        /// <summary>
        /// Gets the MD5 Hash string for the given input
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Hashed string</returns>
        private static string Md5Hash(string input)
        {
            var data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            for (var i = 0; i < data.Length; i++) sb.Append(data[i].ToString("x2"));
            return sb.ToString();
        }

        public string JsonText { get; set; }

        public string FBversion { get; set; }

        public string ProxyStr { get; set; }
    }

    /// <summary>
    /// FacebookGraphAPIException
    /// </summary>
    internal class FacebookGraphApiException : Exception
    {
        public string Type { get; set; }
        public new string Message { get; set; }

        public FacebookGraphApiException(string type, string message)
        {
            Type = type;
            Message = message;
        }
    }

}