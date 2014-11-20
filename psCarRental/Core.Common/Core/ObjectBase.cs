using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Annotations;
using Core.Common.Contracts;
using Core.Common.Extensions;
using Core.Common.Utils;

namespace Core.Common.Core
{
    public class ObjectBase : INotifyPropertyChanged
    {
        private bool _IsDirty;
        List<PropertyChangedEventHandler> _PropertyChangedSubscribers = 
            new List<PropertyChangedEventHandler>(); 

        private event PropertyChangedEventHandler _PropertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!_PropertyChangedSubscribers.Contains(value))
                {
                    _PropertyChanged += value;
                    _PropertyChangedSubscribers.Add(value);
                }                
            }
            remove
            {                
                _PropertyChanged -= value;
                _PropertyChangedSubscribers.Remove(value);
            }
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, true);
        }

        protected virtual void OnPropertyChanged(string propertyName, bool makeDirty)
        {
            if (_PropertyChanged != null)
            {
                _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            if (makeDirty)
                _IsDirty = true;
        }

        [NotNavigable]
        public bool IsDirty
        {
            get { return _IsDirty; }
            set { _IsDirty = value; }
        }

        public List<ObjectBase> GetDirtyObjects()
        {
            var dirtyObjects = new List<ObjectBase>();

            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                    {
                        dirtyObjects.Add(o);
                    }
                    return false;
                }, coll => 
                { });

            return dirtyObjects;
        }

        public void CleanAll()
        {
            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                    {
                        o.IsDirty = false;
                    }
                    return false;
                }, coll =>
                { });
        }

        public virtual bool IsAnythingDirty()
        {
            bool isDirty = false;

            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                    {
                        isDirty = true;
                        return true; // short circuit
                    }
                    else
                    {
                        return false;
                    }
                }, coll =>
                { });

            return isDirty;
        }

        protected void WalkObjectGraph(Func<ObjectBase, bool> snippetForObject,
            Action<IList> snippetForCollection,
            params string[] exemptProperties)
        {
            var visited = new List<ObjectBase>();
            Action<ObjectBase> walk = null; 

            List<String> exemptions = new List<string>();
            if (exemptProperties != null)
            {
                exemptions = exemptProperties.ToList();
            }

            walk = (o) =>
            {
                if (o != null && !visited.Contains(o))
                {
                    visited.Add(o);
                    bool exitWalk = snippetForObject.Invoke(o);

                    if (!exitWalk)
                    {
                        PropertyInfo[] properties = o.GetBrowsableProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            if (!exemptions.Contains(property.Name))
                            {
                                if (property.PropertyType.IsSubclassOf(typeof (ObjectBase)))
                                {
                                    ObjectBase obj = (ObjectBase) (property.GetValue(o, null));
                                    walk(obj);
                                }
                                else
                                {
                                    IList coll = property.GetValue(o, null) as IList;
                                    if (coll != null)
                                    {
                                        snippetForCollection.Invoke(coll);

                                        foreach (object item in coll)
                                        {
                                            if (item is ObjectBase)
                                            {
                                                walk((ObjectBase) item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        //protected List<ObjectBase> GetDirtyObjects()
        //{
        //    List<ObjectBase> dirtyObjects = new List<ObjectBase>();

        //    List<ObjectBase> visited = new List<ObjectBase>();
        //    Action<ObjectBase> walk = null;

        //    walk = (o) =>
        //    {
        //        if (o != null && !visited.Contains(o))
        //        {
        //            visited.Add(o);
        //            if (o.IsDirty)
        //            {
        //                dirtyObjects.Add(o);
        //            }

        //            bool exitWalk = false;

        //            if (!exitWalk)
        //            {
        //                PropertyInfo[] properties = o.GetBrowsableProperties();
        //                foreach (PropertyInfo property in properties)
        //                {
        //                    if (property.PropertyType.IsSubclassOf(typeof (ObjectBase)))
        //                    {
        //                        ObjectBase objectBase = (ObjectBase) (property.GetValue(o, null));
        //                        walk(objectBase);
        //                    }
        //                    else
        //                    {
        //                        IList coll = property.GetValue(o, null) as IList;
        //                        if (coll != null)
        //                        {
        //                            // in this instance, we don't want to do anything with 'coll' itself.
        //                            foreach (object item in coll)
        //                            {
        //                                if (item is ObjectBase)
        //                                {
        //                                    walk((ObjectBase) item);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    walk(this);

        //    return dirtyObjects;
        //}

        //public List<IDirtyCapable> GetDirtyObjects()
        //{
        //    List<IDirtyCapable> dirtyObjects = new List<IDirtyCapable>();

        //    WalkObjectGraph(o =>
        //    {
        //        if (o.IsDirty)
        //        {
        //            dirtyObjects.Add(o);
        //        }

        //    });
        //} 
    }
}
