using System;
using System.Threading;

namespace Micro.Net.Example.Services
{
    public class ValueHolderService : IValueHolderService
    {
        private static long _value = 0;

        public void ShiftValue(int amount)
        {
            Interlocked.Add(ref _value, amount);
        }

        public int ReadValue()
        {
            return (int)Interlocked.Read(ref _value);
        }
    }
}
