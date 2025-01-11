using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MyHealthcareApp.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
         public DateTime CreatedAt { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public Chat Chat { get; set; }
        public string Sender { get; set; } // "Doctor" or "Patient"
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
