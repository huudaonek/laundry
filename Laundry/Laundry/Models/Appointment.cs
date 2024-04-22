using System;
using System.ComponentModel.DataAnnotations;

namespace Laundry.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide your full name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please provide your phone number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please provide your address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please provide the appointment date")]
        public DateTime AppointmentDate { get; set; }
    }
}
