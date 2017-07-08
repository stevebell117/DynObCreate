using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynObCreate.Helper
{
    internal static class Lists
    {
        internal static IList InstantiateDynamicList(object objToDetermineType)
        {
            Type itemType = objToDetermineType.GetType();
            Type listType = typeof(List<>).MakeGenericType(new[] { itemType });

            return (IList)Activator.CreateInstance(listType);
        }
    }
}
