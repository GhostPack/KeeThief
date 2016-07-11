using System;

namespace KeeTheft
{
    public interface ILogger
    {
        void WriteLine(object str);
        void Write(object str);
    }

    class NullLogger : ILogger
    {
        public void Write(object str)
        {
        }

        public void WriteLine(object str)
        {
        }
    }

    class ConsoleLogger : ILogger
    {
        public void Write(object str)
        {
            Console.Write(str);
        }

        public void WriteLine(object str)
        {
            Console.WriteLine(str);
        }
    }
}
