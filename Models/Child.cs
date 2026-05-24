using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdenProject.Models
{
    public class Child
    {
        public string Id { get; set; }

        [JsonProperty("CurrentParentID")]
        public string ParentId { get; set; } // מקשר בין הילד להורה
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

        // נוכל להוסיף כאן הערות כלליות של ההורה
        public string Notes { get; set; }
    }
}
