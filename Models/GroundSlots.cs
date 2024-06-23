namespace NewHorizon.Models
{

    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class GroundSlots
    {
        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Datum
    {
        [JsonProperty("ground_sports_court_id")]
        public int GroundSportsCourtId { get; set; }

        [JsonProperty("ground_sports_id")]
        public int GroundSportsId { get; set; }

        [JsonProperty("slot_name")]
        public string SlotName { get; set; }

        [JsonProperty("court_title")]
        public string CourtTitle { get; set; }

        [JsonProperty("court_no")]
        public int CourtNo { get; set; }

        [JsonProperty("court_text")]
        public string CourtText { get; set; }

        [JsonProperty("slot_start_time")]
        public string SlotStartTime { get; set; }

        [JsonProperty("slot_end_time")]
        public string SlotEndTime { get; set; }

        [JsonProperty("slot_occupancy")]
        public int SlotOccupancy { get; set; }

        [JsonProperty("is_full_day_slot")]
        public bool IsFullDaySlot { get; set; }

        [JsonProperty("slot_disp_text")]
        public string SlotDispText { get; set; }

        [JsonProperty("booking_id")]
        public int BookingId { get; set; }

        [JsonProperty("court_name")]
        public string CourtName { get; set; }

        [JsonProperty("sports_type")]
        public string SportsType { get; set; }

        [JsonProperty("increment_by")]
        public int IncrementBy { get; set; }

        [JsonProperty("occupied")]
        public string Occupied { get; set; }

        [JsonProperty("is_booked")]
        public bool IsBooked { get; set; }

        [JsonProperty("rate")]
        public int Rate { get; set; }

        [JsonProperty("gssid")]
        public string Gssid { get; set; }

        [JsonProperty("uqid")]
        public string Uqid { get; set; }

        [JsonProperty("uq_cid")]
        public string UqCid { get; set; }

        [JsonProperty("slot_start_time_s")]
        public string SlotStartTimeS { get; set; }

        [JsonProperty("slot_end_time_s")]
        public string SlotEndTimeS { get; set; }

        [JsonProperty("slot_time_half")]
        public int SlotTimeHalf { get; set; }
    }

}
