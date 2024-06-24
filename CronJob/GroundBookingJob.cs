using GoogleApi.Entities.Maps.DistanceMatrix.Response;
using Microsoft.AspNetCore.Http;
using NewHorizon.Models;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using Newtonsoft.Json;
using System.Globalization;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Web;

namespace NewHorizon.CronJob
{
    public class GroundBookingJob : IGroundBookingJob
    {
        private const double IntervalInMilliseconds = 3 * 60 * 60 * 1000;
        private readonly IMailingService _mailingService;
        private Dictionary<string, List<string>> AddedGrounds = new Dictionary<string, List<string>>();
        private readonly List<string> grounds = new List<string>()
        {
            "srrc1",
            "srrc2",
            "urban-technology-farms-pt-ltd", // 9km
            "svcg-kokapet", // 9 km
            "vbcc-1-vattinagulapally", // 9km
            "dcc-sports-arena", // 9.5 km
            "playsfit-cricket-ground-vattinagulapally", // 9.5 km
            "swaroop-cricket-ground-kokapet", // 11 km
            "samruddhi-cricket-ground", //12 km
            "msk-cricket-ground-vattinagulapally", // 14 km
            "the-pavilion-cricket-stadium", // 14 km
            "vscg-cricket-ground", // 14 km
            "thrill-cricket-ground-1-appa-junction", // 15 km
            "am-cricket-ground", // 19 km
            "harsha-cricket-ground-aziznagar", // 19.5 km
            "abhiram-cricket-county-donthanpalli-shankarpally", // 20 km
            "gurukul-ground-moinabad", // 20km
            "ycc-cricket-ground", // 20 km
            "onechampion1-cricket-grounds", //21 km
            "azpro-cricket-ground", // 21km
            "scg-cricket-ground", // 22 km
            "brindaground-3", // 22 km
            "brindaground-1", // 22 km
            "brindaground-2", // 22 km
            "suresh-cricket-ground", // 22 km
            "sz-cricket-ground", // 22 km
            "beside-arun-gardens,chilukuru-balaji-temple-road,hyderabad-500075", // 22 km
            "olympus-zeus", // 25 km
            "mps-cricket-ground-shamshabad", // 25km
            "sr-cricket-ground", //25 km
            "sr2-cricket-ground", // 25 km
            "s.a.r-cricket-arena", // 25 km
            "s.a.r-cricket-arena--ground---1", // 25 km
            "olympus-cricket-ground", // 25 km
            "aston-cricket-ground", // 26 km
            "azkit-cricket-ground", // 26 km
            "sukruth-cricket-ground", // 27 km
            "hitman-azhit-cricket-ground", // 27 km
            "rao's-cricket-ground", // 27km
            "vvr2-cricket-ground", //22km
            "reet-cricket-club", // 30 km
            "albatross-cricket-ground", // 30 km
            "smr-cricket-ground", //30 km
            "golconda-cricket-club", // 30 km
            "ballebaaz-maidan"
        };

        public GroundBookingJob(IMailingService mailingService)
        {
            _mailingService = mailingService;
            System.Timers.Timer t = new System.Timers.Timer();
            t.AutoReset = true;
            t.Interval = IntervalInMilliseconds;
            t.Enabled = true;
            t.Elapsed += T_Elapsed;
            Task.Run(() => Run());
        }

        private void T_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _ = Run();
        }

        public async Task Run()
        {

            string url = "https://www.gwsportsapp.in/ajax-handler?t=gsearch&action=getSlotsForGroundSport";

            CleanUp(DateTime.Now.AddDays(-60));

            // Initialize the HttpClient
            using (HttpClient client = new HttpClient())
            {
                for (int i = 3; i < 60; i++)
                {
                    DateTime dateTime = DateTime.Now.AddDays(i);
                    if (dateTime.DayOfWeek != DayOfWeek.Sunday && dateTime.DayOfWeek != DayOfWeek.Saturday)
                    {
                        continue;
                    }
                    string formatedDate = dateTime.ToString("yyyy-MM-dd");
                    foreach (string grnd in grounds)
                    {
                        if (!AddedGrounds.ContainsKey(formatedDate))
                        {
                            AddedGrounds[formatedDate] = new List<string>();
                        }
                        if (AddedGrounds[formatedDate].Count >= 3)
                        {
                            break;
                        }

                        // Set up the headers
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
                        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US", 0.9));
                        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-IN", 0.8));
                        client.DefaultRequestHeaders.Add("Origin", "https://www.gwsportsapp.in");
                        client.DefaultRequestHeaders.Referrer = new Uri($"https://www.gwsportsapp.in/hyderabad/cricket/booking-sports-online-venue/{grnd}");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                        client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not/A)Brand\";v=\"8\", \"Chromium\";v=\"126\", \"Microsoft Edge\";v=\"126\"");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                        client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");

                        // Create the form URL-encoded content

                        var obj = new { l = "hyderabad", g = grnd, s = "cricket", d = formatedDate };
                        string value = JsonConvert.SerializeObject(obj);
                        string encoded = HttpUtility.UrlEncode(value);
                        var content = new StringContent($"data={encoded}");
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet = "UTF-8" };

                        // Send the POST request
                        HttpResponseMessage response = await client.PostAsync(url, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            continue;
                        }

                        await Task.Delay(500);
                        try
                        {
                            using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                            using (GZipStream decompressionStream = new GZipStream(responseStream, CompressionMode.Decompress))
                            using (StreamReader reader = new StreamReader(decompressionStream))
                            {
                                string responseString = await reader.ReadToEndAsync();
                                // Output the response
                                Console.WriteLine(responseString);
                                GroundSlots groundSlots = JsonConvert.DeserializeObject<GroundSlots>(responseString);
                                if (groundSlots != null && groundSlots.Status.Equals("success") && groundSlots.Data != null)
                                {
                                    foreach (var groundSlot in groundSlots.Data)
                                    {
                                        if (groundSlot != null && !groundSlot.IsBooked && groundSlot.Rate < 9000 && groundSlot.SlotTimeHalf >= 400 && groundSlot.SlotTimeHalf <= 1200)
                                        {
                                            string groundLink = $"https://www.gwsportsapp.in/hyderabad/cricket/booking-sports-online-venue/{grnd}";

                                            // send mail.
                                            _mailingService.SendGroundMail("robin.cool.13@gmail.com", "Ground Available", $"Cricket ground {groundLink} available for date {formatedDate}, timing {groundSlot.SlotStartTime}");
                                            
                                            AddedGrounds[formatedDate].Add(grnd);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // do nothing.
                            Console.WriteLine("Exception");
                        }
                    }
                }

            }
        }

        private void CleanUp(DateTime pastDate)
        {
            List<string> removeKeys = new List<string>();
            string format = "yyyy-MM-dd";
            foreach (var kv in AddedGrounds)
            {
                DateTime date = DateTime.ParseExact(kv.Key, format, CultureInfo.InvariantCulture);
                if (date < pastDate)
                {
                    removeKeys.Add(kv.Key);
                }
            }
            foreach (var key in removeKeys)
            {
                AddedGrounds.Remove(key);
            }
        }
    }
}
