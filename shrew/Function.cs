using System;
using System.Collections.Generic;
using System.Text;

namespace shrew
{
    internal class Function
    {
        private bool isEmpty = false;
        private Delegate _func;
        private bool isAction;
        private int parameterCount;
        private Function(Delegate func)
        {
            if (func == null)
            {
                isEmpty = true;
                return;
            }
            _func = func;
            parameterCount = func.Method.GetParameters().Length;
            isAction = func.Method.ReturnType == typeof(void);
        }

        public object Invoke(params object[] parameters)
        {
            if (isEmpty)
                throw new Exception("Function not inited");
            if (parameters.Length != parameterCount)
                throw new Exception("Wrong parameter count");
            if (isAction)
            {
                switch (parameterCount)
                {
                    case 0:
                        ((Action)_func)();
                        break;
                    case 1:
                        ((Action<object>)_func)(parameters[0]);
                        break;
                    case 2:
                        ((Action<object, object>)_func)(parameters[0], parameters[1]);
                        break;
                    case 3:
                        ((Action<object, object, object>)_func)(parameters[0], parameters[1], parameters[2]);
                        break;
                    case 4:
                        ((Action<object, object, object, object>)_func)(
                            parameters[0], parameters[1], parameters[2], parameters[3]);
                        break;
                }
                return null;
            }
            else
            {
                switch (parameterCount)
                {
                    case 0:
                        return ((Func<object>)_func)();
                    case 1:
                        return ((Func<object, object>)_func)(parameters[0]);
                    case 2:
                        return ((Func<object, object, object>)_func)(parameters[0], parameters[1]);
                    case 3:
                        return ((Func<object, object, object, object>)_func)(
                            parameters[0], parameters[1], parameters[2]);
                    case 4:
                        return ((Func<object, object, object, object, object>)_func)(
                            parameters[0], parameters[1], parameters[2], parameters[3]);
                }
            }
            throw new NotImplementedException();
        }

        public static implicit operator Function(Delegate del)
        {
            return new Function(del);
        }
    }
}
