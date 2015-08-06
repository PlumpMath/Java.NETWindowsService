using System;

namespace Java.NETWindowsService.Exceptions
{
    public class JavaException : Exception
    {
        public JavaException(string message)
            : base(message)
        {

        }
    }
}