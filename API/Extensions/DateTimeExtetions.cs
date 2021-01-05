using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtetions
    {
        public static int CalculateAge(this DateTime date)
        {
            var today = DateTime.Today;
            var age = today.Year - date.Year;
            if (date.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
