using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCrawler.Model
{
    public class CourseEntity : BaseModel
    {
        public long CourseId { get; set; }
        public int CategoryId { get; set; }  //类别Id
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }

    
}
