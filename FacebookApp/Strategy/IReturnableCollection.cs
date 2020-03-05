using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FacebookApp
{
    public interface IReturnableCollection
    {
        ICollection ReturnCollection(); 
    }
}
