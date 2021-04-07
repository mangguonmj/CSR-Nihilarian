using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nihilarian
{
    public class pack
    {
        public pack(string t)
        {
            this.@event = t;
            this.type = "mcwsMessage";
        }
        /// <summary>
        /// 
        /// </summary>
        public string type;
        /// <summary>
        /// 
        /// </summary>
        public string sender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string @event { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string password { get; set; }
    }
    public class config
    {
        public string password { get; set; }
        public int port { get; set; }
    }
}
