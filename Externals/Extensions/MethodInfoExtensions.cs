using System;
using System.Reflection;

namespace Externals
{
    public static class MethodInfoExtensions
    {
        private class MethodRedirection : IDisposable
        {
            private bool _isDisposed = false;

            private MethodInfo _originalMethod;
            private MethodInfo _newMethod;
            private readonly RedirectCallsState _callsState;

            public MethodRedirection(MethodInfo originalMethod, MethodInfo newMethod)
            {
                _originalMethod = originalMethod;
                _newMethod = newMethod;
                _callsState = RedirectionHelper.RedirectCalls(_originalMethod, _newMethod);
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    RedirectionHelper.RevertRedirect(_originalMethod, _callsState);
                    _originalMethod = null;
                    _newMethod = null;
                    _isDisposed = true;
                }
            }
        }

        public static IDisposable RedirectTo(this MethodInfo originalMethod, MethodInfo newMethod)
        {
            return new MethodRedirection(originalMethod, newMethod);
        }
    }
}
