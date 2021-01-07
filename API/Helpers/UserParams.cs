using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;

        public int Page
        {
            get => pageSize;
            set => pageSize = (value > MaxPageSize)? MaxPageSize : value;
        }
    }
}
