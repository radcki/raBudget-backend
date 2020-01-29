using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Sdk;

namespace raBudget.Domain.UnitTests
{
    public class UnitTestsBase
    {
        protected string RandomString(int length)
        {
            const string chars = "abcdefghjklmnoprstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        protected int RandomInt(int min = 0, int max = 100)
        {
            return new Random().Next(min, max);
        }
    }
}
