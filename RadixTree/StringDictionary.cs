using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadixTreeProject
{
    interface StringDictionary
    {
        bool Search(String key);

        void Insert(String key);

        void Delete(String key);

        String Predecessor(String key);

        String Successor(String key);
    }
}
