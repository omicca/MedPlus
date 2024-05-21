#nullable enable
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MedPlus.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AdminId { get; set; }
        public string? UniqueRole { get; set; }
    }

    public class Admin : User
    {
        // Navigation properties
        public virtual ICollection<Doctor>? Doctors { get; set; }
        public virtual ICollection<Patient>? Patients { get; set; }

        public Admin()
        {
            UniqueRole = "Admin";
        }
    }

    public class Doctor : User
    {
        // Navigation properties
        public virtual Admin? Admin { get; set; }
        public virtual ICollection<Patient>? Patients { get; set; }

        public Doctor()
        {
            UniqueRole = "Doctor";
        }
    }

    public class Patient : User
    {
        public string? DoctorId { get; set; }

        // Navigation properties
        public virtual Admin? Admin { get; set; }
        public virtual Doctor? Doctor { get; set; }

        public Patient()
        {
            UniqueRole = "Patient";
        }
    }
}
