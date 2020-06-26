﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Comander.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserLogin { get; set; }
        public string UserPass { get; set; }
        public string UserName { get; set; }
        public string UserSureName { get; set; }
        public string UserAddress { get; set; }
        public string UserCity { get; set; }
        public string UserZipCode { get; set; }
        public string UserMail { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserPhoneNumber2 { get; set; }
        public string UserRole { get; set; }

    }
}