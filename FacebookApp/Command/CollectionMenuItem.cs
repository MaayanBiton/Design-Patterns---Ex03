using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacebookApp
{
    public class CollectionMenuItem
    {
        public CollectionMenuItem(Action i_CollectionOptionMethod, string i_CollectionName)
        {
            this.CollectionName = i_CollectionName;
            this.CollectionOptionMethod = i_CollectionOptionMethod;
        }

        public Action CollectionOptionMethod { get; set; }

        public string CollectionName { get; set; }

        public void Selected()
        {
            if (this.CollectionOptionMethod != null)
            {
                this.CollectionOptionMethod.Invoke();
            }
        }
    }
}