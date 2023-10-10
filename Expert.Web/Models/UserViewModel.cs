﻿using Expert.Web.DTOs;

namespace Expert.Web.Models
{
    public class UserViewModel
    {
        public UserResultDto User { get; set; }
        public IEnumerable<UserResultDto> Users { get; set; }
        public long? UserId { get; set; }
        public int? Score { get; set; }
    }
}
