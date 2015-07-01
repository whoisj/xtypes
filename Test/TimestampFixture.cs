using System;
using Xunit;
using Verify = Xunit.Assert;

namespace XTypes.Tests
{
    public class TimestampFixture : BaseFixture
    {
        [Fact]
        public void DateTimeInteropWorks()
        {
            DateTime dt1 = DateTime.UtcNow;
            Timestamp ts1 = new Timestamp(dt1);
            DateTime dt2 = (DateTime)ts1;

            Verify.Equal(dt1.Ticks / Timestamp.TicksPerMillisecond, dt2.Ticks / Timestamp.TicksPerMillisecond);

            DateTime dt3 = dt1.AddSeconds(1);
            Timestamp ts2 = new Timestamp(dt3);
            Timestamp ts3 = new Timestamp(dt3.Year, dt3.Month, dt3.Day, dt3.Hour, dt3.Minute, dt3.Second, dt3.Millisecond);

            Verify.True(ts2 > ts1, "Later value not greater than starting value.");
            Verify.Equal(ts2, ts3);
        }
    }
}
