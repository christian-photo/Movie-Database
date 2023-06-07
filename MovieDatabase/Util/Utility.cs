#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;

namespace MovieDatabase.Util
{
    public static class Utility
    {
        public static HttpClientHandler FastClientHandler = new HttpClientHandler()
        {
            Proxy = null,
            UseProxy = false
        };

        public static bool IsConnectedToInternet()
        {
            try
            {
                Ping myPing = new Ping();
                const string host = "google.com";
                byte[] buffer = new byte[32];
                const int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Logger.Error($"Couldn't check internet connectivity, Message:{e.Message}\nStacktrace: {e.StackTrace}");
                return false;
            }
        }

        public static List<string> KeysToList(ICollection coll)
        {
            List<string> list = new List<string>();
            foreach (var key in coll)
            {
                list.Add(key.ToString());
            }
            return list;
        }
    }
}
