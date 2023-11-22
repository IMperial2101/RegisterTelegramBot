using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegBot2
{
    internal class MyLink
    {
        public MyLink()
        {

        }
        public MyLink(MyLink mylink)
        {
            url = mylink.url;
            city = mylink.city;
            name = mylink.name;
            priceTo = mylink.priceTo;
            priceFrom = mylink.priceFrom;
        }
        public string url { get; set; } 
        public string city { get; set; }
        public string name { get; set; }
        public string priceTo { get; set; }
        public string priceFrom { get; set; }
        public bool makeByConstructor { get; set; } = true;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            MyLink other = (MyLink)obj;

            return url == other.url &&
                   city == other.city &&
                   name == other.name &&
                   priceTo == other.priceTo &&
                   priceFrom == other.priceFrom;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(url, city, name, priceTo, priceFrom);
        }




    }
}
