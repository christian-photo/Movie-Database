#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

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
                string host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 500;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
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
