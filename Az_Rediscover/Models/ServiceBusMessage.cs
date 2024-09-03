using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az_Rediscover.Models
{
	public record struct ServiceBusMessage
	{
        public string Message { get; set; }
        public string Title { get; set; }
    }
}
