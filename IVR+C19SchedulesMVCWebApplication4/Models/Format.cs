﻿namespace IVR_C19SchedulesMVCWebApplication4.Models
{
    public static class Format
    {
        public static Json Json;
    }

    public partial class Json
    {
        public string UnStringify(string s)
        {
            var pattern = "\"";
            return s.Replace(pattern, string.Empty);

        }
    }
}