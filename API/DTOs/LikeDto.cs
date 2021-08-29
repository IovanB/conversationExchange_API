using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LikeDto
    {
        public  int Id { get; set; }
        public  string UserName { get; set; }
        public  int Age { get; set; }
        public  string KnowAs { get; set; }
        public  string PhotoUrl { get; set; }
        public  string City { get; set; }
    }
}
