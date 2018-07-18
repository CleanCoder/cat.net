using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;

namespace Org.Unidal.Cat.Util
{
    public class CatThreadLocal<T>
    {
       // private ThreadLocal<T> threadLocal = new ThreadLocal<T>();

        public T Value
        {
            get
            {
                var wrap = CallContext.LogicalGetData(CatConstants.CAT_CONTEXT) as Wrap;
                return wrap == null ? default(T) : wrap.Value;
            }
            set
            {
                var wrap = CallContext.LogicalGetData(CatConstants.CAT_CONTEXT) as Wrap;
                if (wrap == null)
                {
                    CallContext.LogicalSetData(CatConstants.CAT_CONTEXT, new Wrap(value));
                }
                else
                {
                    wrap.Value = value;
                }
            }
        }

        public void Remove()
        {
            // threadLocal.Value = default(T);

            // CallContext.FreeNamedDataSlot(CatConstants.CAT_CONTEXT);
        }

        // CallContext里面的值不能flow up，所以用一个对象包装，这样对对象里面的对象的修改就可以传递回外层调用了
        private class Wrap
        {
            public T Value { get; set; }
            public Wrap(T t)
            {
                Value = t;
            }

            public void Clear()
            {
                Value = default(T);
            }
        }
    }
}