﻿namespace SurveyManagementSystemApi.Securty
{
    public class Jwt
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpiryMinutes { get; set; }
        public required string Key { get; set; }
    }


}
