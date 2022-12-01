using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Surfaces
{
    public class TakeOffAttriputes
    {
        public int s1 { get; set; }
        public int s2 { get; set; }
        public int s3 { get; set; }
        public int l1 { get; set; }
        public int l2 { get; set; }
        public int l3 { get; set; }

        public TakeOffAttriputes(int s1 ,int s2, int s3, int l1, int l2, int l3)
        {
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.l1 = l1;
            this.l2 = l2;
            this.l3 = l3;
        }
    }
}
