﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5Semester.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public decimal Password { get; set; }
        public string DisplayName { get; set; }
    }
}