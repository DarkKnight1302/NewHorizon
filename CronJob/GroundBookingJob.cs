using Microsoft.Extensions.Caching.Memory;
using NewHorizon.Models;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using Newtonsoft.Json;
using Quartz;
using System.Globalization;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Web;

namespace NewHorizon.CronJob
{
    public class GroundBookingJob : IJob
    {
        private readonly IMailingService _mailingService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly List<string> grounds = new List<string>()
        {
            "srrc1",
            "srrc2",
            "urban-technology-farms-pt-ltd", // 9km
            "svcg-kokapet", // 9 km
            "vbcc-1-vattinagulapally", // 9km
            "playsfit-cricket-ground-vattinagulapally", // 9.5 km
            "swaroop-cricket-ground-kokapet", // 11 km
            "samruddhi-cricket-ground", //12 km
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
            "msk-cricket-ground-vattinagulapally", // 14 km
            "vvr2-cricket-ground", //22km
            "reet-cricket-club", // 30 km
            "dcc-sports-arena", // 9.5 km
            "albatross-cricket-ground", // 30 km
            "smr-cricket-ground", //30 km
            "golconda-cricket-club", // 30 km
            "ballebaaz-maidan"
        };

        public GroundBookingJob(IMailingService mailingService, ILogger<GroundBookingJob> logger, IMemoryCache memoryCache)
        {
            _mailingService = mailingService;
            _logger = logger;
            this._memoryCache = memoryCache;
        }

        private async Task RunGroundBooking()
        {
            this._logger.LogInformation("Starting booking job");
            string url = "https://www.gwsportsapp.in/ajax-handler?t=gsearch&action=getSlotsForGroundSport";

            // Initialize the HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Set up the headers
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US", 0.9));
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-IN", 0.8));
                client.DefaultRequestHeaders.Add("Origin", "https://www.gwsportsapp.in");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not/A)Brand\";v=\"8\", \"Chromium\";v=\"126\", \"Microsoft Edge\";v=\"126\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");

                for (int i = 3; i < 45; i++)
                {
                    bool daySlotFound = false;

                    DateTime dateTime = DateTime.Now.AddDays(i);
                    if (dateTime.DayOfWeek != DayOfWeek.Sunday && dateTime.DayOfWeek != DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Friday)
                    {
                        continue;
                    }
                    string formatedDate = dateTime.ToString("yyyy-MM-dd");
                    foreach (string grnd in grounds)
                    {
                        this._logger.LogInformation($"Checking for ground {grnd} : {formatedDate}");
                        List<string> addedGrounds = this._memoryCache.GetOrCreate(formatedDate, e =>
                        {
                            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                            return new List<string>();
                        });
                        if (addedGrounds.Contains(grnd))
                        {
                            continue;
                        }
                        if (addedGrounds.Count > 1)
                        {
                            break;
                        }
                        
                        client.DefaultRequestHeaders.Referrer = new Uri($"https://www.gwsportsapp.in/hyderabad/cricket/booking-sports-online-venue/{grnd}");
                        // Create the form URL-encoded content

                        var obj = new { l = "hyderabad", g = grnd, s = "cricket", d = formatedDate };
                        string value = JsonConvert.SerializeObject(obj);
                        string encoded = HttpUtility.UrlEncode(value);
                        var content = new StringContent($"data={encoded}");
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet = "UTF-8" };

                        try
                        {
                            // Send the POST request
                            HttpResponseMessage response = await client.PostAsync(url, content);

                            if (!response.IsSuccessStatusCode)
                            {
                                this._logger.LogError("Failure response from server");
                                continue;
                            }
                            string responseString = string.Empty;
                            using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                            {
                                Stream decompressionStream = null;
                                if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                                {
                                    decompressionStream = new GZipStream(responseStream, CompressionMode.Decompress);
                                }
                                else if (response.Content.Headers.ContentEncoding.Contains("deflate"))
                                {
                                    decompressionStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                                }
                                else if (response.Content.Headers.ContentEncoding.Contains("br"))
                                {
                                    decompressionStream = new BrotliStream(responseStream, CompressionMode.Decompress);
                                }
                                else
                                {
                                    decompressionStream = responseStream; // No compression
                                }

                                using (decompressionStream)
                                using (StreamReader reader = new StreamReader(decompressionStream))
                                {
                                    responseString = await reader.ReadToEndAsync();
                                    // Output the response
                                    _logger.LogInformation(responseString);
                                }
                            }

                            GroundSlots groundSlots = JsonConvert.DeserializeObject<GroundSlots>(responseString);
                            if (groundSlots != null && groundSlots.Status.Equals("success") && groundSlots.Data != null)
                            {
                                foreach (var groundSlot in groundSlots.Data)
                                {

                                    if (dateTime.DayOfWeek != DayOfWeek.Friday && groundSlot != null && !groundSlot.IsBooked && groundSlot.Rate < 9000 && groundSlot.SlotTimeHalf >= 350 && groundSlot.SlotTimeHalf <= 1200)
                                    {
                                        string groundLink = $"https://www.gwsportsapp.in/hyderabad/cricket/booking-sports-online-venue/{grnd}";

                                        this._logger.LogInformation("Sending mail");
                                        // send mail.
                                        _mailingService.SendGroundMail("robin.cool.13@gmail.com", "Ground Available", $"Cricket ground {groundLink} available for date {formatedDate}, timing {groundSlot.SlotStartTime}");

                                        addedGrounds.Add(grnd);
                                        daySlotFound = true;
                                    }
                                    // Friday slot.
                                    if (dateTime.DayOfWeek == DayOfWeek.Friday && groundSlot != null && !groundSlot.IsBooked && groundSlot.Rate <= 12000 && groundSlot.SlotTimeHalf >= 1000 && groundSlot.SlotTimeHalf <= 1200)
                                    {
                                        string groundLink = $"https://www.gwsportsapp.in/hyderabad/cricket/booking-sports-online-venue/{grnd}";
                                        
                                        this._logger.LogInformation("Sending mail");
                                        // send mail.
                                        _mailingService.SendGroundMail("robin.cool.13@gmail.com", "Friday Ground Available", $"Cricket ground {groundLink} available for date {formatedDate}, timing {groundSlot.SlotStartTime}");

                                        addedGrounds.Add(grnd);
                                        daySlotFound = true;
                                    }
                                    if (daySlotFound)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError($"Exception in ground booking {ex.Message} \n {ex.StackTrace}");
                        }
                        await Task.Delay(250);
                    }
                }

            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await RunGroundBooking();
        }
    }
}
