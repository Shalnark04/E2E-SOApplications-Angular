using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Core;

namespace Core.Common.Extensions
{
    public static class CoreExtensions
    {
        static Dictionary<string, bool> BrowsableProperties = new Dictionary<string, bool>(); 
        static Dictionary<string, PropertyInfo[]> BrowsablePropertyInfos = new Dictionary<string, PropertyInfo[]>(); 

        public static PropertyInfo[] GetBrowsableProperties(this object obj)
        {
            string key = obj.GetType().ToString();

            if (!BrowsablePropertyInfos.ContainsKey(key))
            {
                List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if((property.PropertyType.IsSubclassOf(typeof(ObjectBase))
                        || property.PropertyType.GetInterface("IList") != null))
                    {
                        if(IsBrowsable(obj, property))
                            propertyInfoList.Add(property);
                    }
                }

                BrowsablePropertyInfos.Add(key, propertyInfoList.ToArray());
            }

            return BrowsablePropertyInfos[key];
        }

        public static bool IsBrowsable(this object obj, PropertyInfo property)
        {
            string key = string.Format("{0}.{1}", obj.GetType(), property.Name);

            if (!BrowsableProperties.ContainsKey(key))
            {
                bool browsable = property.IsNavigable();
                BrowsableProperties.Add(key, browsable);
            }

            return BrowsableProperties[key];
        }

        public static bool IsNavigable(this PropertyInfo property)
        {
            bool navigable = true;

            object[] attributes = property.GetCustomAttributes(typeof (NotNavigableAttribute), true);
            if (attributes.Length > 0)
            {
                navigable = false;
            }

            return navigable;
        }
    }
}
