namespace XTypes
{
    /// <summary>
    /// <para>Structure which represents the absolute date and time relative to 
    /// 00-01-01 00:00:00.0000Z by the number of milliseconds before or after.</para> 
    /// <para>Effective range is roughly 600 million years ranging from 250,000 BCE to 250,000 CE.</para>
    /// <para>All year, month, day, hour, minute, and second values assume Gregorian calendar for values.</para>
    /// </summary>
    /// <remarks>All values of Timestamp are in UTC and stored to the nearest millisecond (10^-3 seconds)</remarks>
    internal struct Timestamp : 
        System.IComparable,                                 
        System.IComparable<Timestamp>, 
        System.IEquatable<Timestamp>, 
        System.IEquatable<System.DateTime>
    {
        private const int MinimumYear = -250000000;
        private const int MaximumYear = 249999999;
        public const int SecondsPerMinute = 60;
        public const int MinutesPerHour = 60;
        public const int HoursPerDay = 24;
        public const int DaysPerYear = 365;
        public const int YearsPerLeapYear = 4;
        public const int YearsPer1CLeapYear = 100;
        public const int YearsPer4CLeapYear = 400;
        public const int YearsPer4MLeapYear = 4000;
        public const int MonthAffectByLeapYear = 1;
        public const int TicksPerMillisecond = 10000;
        public const long MillisecondsPerSecond = 1000;
        public const long MillisecondsPerMinute = MillisecondsPerSecond * SecondsPerMinute;
        public const long MillisecondsPerHour = MillisecondsPerMinute * MinutesPerHour;
        public const long MillisecondsPerDay = MillisecondsPerHour * HoursPerDay;
        public const long MillisecondsPerYear = MillisecondsPerDay * DaysPerYear;
        public static readonly int[] DaysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        public static int MonthsPerYear { get { return DaysPerMonth.Length; } }

        /// <summary>
        /// Represents the smallest possible value of <see cref="Timestamp"/>.
        /// </summary>
        public static readonly Timestamp MinValue = new Timestamp(MinimumYear, 1, 1, 0, 0, 0, 0);
        /// <summary>
        /// Represents the largest possible value of <see cref="Timestamp"/>.
        /// </summary>
        public static readonly Timestamp MaxValue = new Timestamp(MaximumYear, 12, 31, 23, 59, 59, 999);
        /// <summary>
        /// The zero <see cref="Timestamp"/> equal to  0-01-01 CE 00:00:00.0000Z.
        /// </summary>
        public static readonly Timestamp Zero = new Timestamp(0);

        public enum EraType
        {
            /// <summary>
            /// Before the Common Era. All dates previous to 1 Jan 0001.
            /// </summary>
            BCE = -1,
            /// <summary>
            /// Common Era. All dates after and including 1 Jan 0001
            /// </summary>
            CE = 0
        }

        /// <summary>
        /// Creates a <see cref="Timestamp"/> structure from the raw number of microseconds.
        /// </summary>
        /// <param name="milliseconds">The number of microseconds before or after 
        /// 0-01-01 CE 00:00:00.0000Z</param>
        public Timestamp(long milliseconds)
            : this()
        {
            if (milliseconds < MinValue.TotalMilliseconds || milliseconds > MaxValue.TotalMilliseconds)
                throw new System.ArgumentOutOfRangeException("milliseconds", $"The milliseconds parameter is restricted to [{MinValue.TotalMilliseconds}, {MaxValue.TotalMilliseconds}]");
            
            // record the actuall milliseconds passed in as the base value
            this.TotalMilliseconds = milliseconds;
            // get the absolute number of milliseconds because the math works this way
            milliseconds = System.Math.Abs(milliseconds);
            // local variables
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            int millisecond = 0;
            // compute the year
            day = (int)(milliseconds / MillisecondsPerDay);
            milliseconds -= day * MillisecondsPerDay;
            // calculate the year based on the number of days, first by blocks of 400 the by per year
            /**
             * investigated into doing this via a simple math equation with years first, but was unable to because
             * deriving years first gave the wrong number of leap years too frequently
            **/
            const int DaysPer400Years = 146097;
            while (day >= DaysPer400Years)
            {
                year += 400;
                day -= DaysPer400Years;
            }
            const int DaysPer100Years = 36524;
            while (day > DaysPer100Years)
            {
                year += 100;
                day -= DaysPer100Years;
            }
            while (day >= DaysPerYear)
            {
                year++;
                day -= DaysPerYear;

                if (IsLeapYear(year + 1))
                    day -= 1;
            }
            // compute the hours
            hour = (int)(milliseconds / MillisecondsPerHour);
            milliseconds -= hour * MillisecondsPerHour;
            // compute the minutes
            minute = (int)(milliseconds / MillisecondsPerMinute);
            milliseconds -= minute * MillisecondsPerMinute;
            // compute the seconds
            second = (int)(milliseconds / MillisecondsPerSecond);
            milliseconds -= second * MillisecondsPerSecond;
            // collect the left overs
            millisecond = (int)milliseconds;
            // discover the month by subtracting the days per month of each month starting with January
            for (int i = 0; i < DaysPerMonth.Length; i++)
            {
                int daysInTheMonth = DaysPerMonth[i];
                // account for leap years -- annoying 365.2425 day year
                if (i == MonthAffectByLeapYear && IsLeapYear(year + 1))
                {
                    daysInTheMonth += 1;
                }
                // once we find a month with more days that we have left, we've found our month
                if (day < daysInTheMonth)
                {
                    month = i;
                    break;
                }
                day -= daysInTheMonth;
            }
            // assign all public values
            _year = year;
            _month = month;
            _day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
            this.Era = this.TotalMilliseconds < 0 ? EraType.BCE : EraType.CE;
        }
        /// <summary>
        /// Creates a Timestamp structure
        /// </summary>
        /// <param name="year">The year component of the date represented by this instance, 
        /// expressed as a value between -250,000 and 250,000.</param>
        /// <param name="month">The month component of the date represented by this instance, 
        /// expressed as a value between 1 and 12.</param>
        /// <param name="day">The day of the month represented by this instance, expressed as 
        /// value between 1 and the length of the month.</param>
        /// <param name="hour">The hour component of the date represented by this instance, 
        /// expressed as a value between 0 and 23.</param>
        /// <param name="minute">The minute component of the date represented by this instance, 
        /// expressed as a value between 0 and 59.</param>
        /// <param name="second">The seconds component of the date represented by this instance, 
        /// expressed as a value between 0 and 59.</param>
        /// <param name="millisecond">The millisecond component of the date represented by this 
        /// instance, expressed as a value between 0 and 999.</param>
        public Timestamp(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0)
            : this()
        {
            if (year < MinimumYear || year > MaximumYear)
                throw new System.ArgumentOutOfRangeException("year", $"The year parameter is restricted to [{MinimumYear}, {MaximumYear}]");
            if (month < 1 || month > 12)
                throw new System.ArgumentOutOfRangeException("month", "The month parameter is restricted to [1, 12]");
            if (day < 1 || day > DaysPerMonth[month - 1])
                throw new System.ArgumentOutOfRangeException("day", $"The day parameter is restricted to [1, {DaysPerMonth[month]}]");
            if (hour < 0 || hour > 23)
                throw new System.ArgumentOutOfRangeException("hour", "The hour parameter is restricted to [0, 23)");
            if (minute < 0 || minute > 59)
                throw new System.ArgumentOutOfRangeException("minute", "The minute parameter is restricted to [0, 60)");
            if (second < 0 || second > 59)
                throw new System.ArgumentOutOfRangeException("second", "The second parameter is restricted to [0, 60)");
            if (millisecond < 0 || millisecond > 999)
                throw new System.ArgumentOutOfRangeException("millisecond", "The millisecond parameter is restricted to [0, 1000)");
            
            int absyear = System.Math.Abs(year) - 1;
            month -= 1;
            day -= 1;
            long milliseconds = absyear * MillisecondsPerYear;
            milliseconds += (absyear / YearsPerLeapYear) * MillisecondsPerDay;
            milliseconds -= (absyear / YearsPer1CLeapYear) * MillisecondsPerDay;
            milliseconds += (absyear / YearsPer4CLeapYear) * MillisecondsPerDay;
            for (int i = 0; i < month - 1; i++)
            {
                milliseconds += DaysPerMonth[i] * MillisecondsPerDay;
                // if we're in March, add an extra day if it is a leap year
                if (i == MonthAffectByLeapYear && IsLeapYear(year))
                {
                    milliseconds += MillisecondsPerDay;
                }
            }
            milliseconds += day * MillisecondsPerDay;
            milliseconds += hour * MillisecondsPerHour;
            milliseconds += minute * MillisecondsPerMinute;
            milliseconds += second * MillisecondsPerSecond;

            _year = absyear;
            _month = month;
            _day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
            this.Era = year < 0 ? EraType.BCE : EraType.CE;
            this.TotalMilliseconds = year < 0 ? -milliseconds : milliseconds;
        }
        /// <summary>
        /// Creates a <see cref="Timestamp"/> from a S<see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="datetime">The System.DateTime to be converted</param>
        public Timestamp(System.DateTime datetime)
            : this(datetime, EraType.CE)
        { }
        /// <summary>
        /// Creates a <see cref="Timestamp"/> from a <see cref="System.DateTime"/> with date 
        /// before 00-01-01 possible
        /// </summary>
        /// <param name="datetime">The <see cref="System.DateTime"/> to be converted.</param>
        /// <param name="era">Sets the era as <see cref="EraType.CE"/> or <see cref="EraType.BCE"/>.</param>
        public Timestamp(System.DateTime datetime, EraType era)
            : this(datetime.Ticks / TicksPerMillisecond * (era == EraType.CE ? 1 : -1))
        { }

        public Timestamp(Timestamp timestamp)
            : this(timestamp.Ticks / TicksPerMillisecond)
        { }

        /// <summary>
        /// Gets a <see cref="System.DateTimeOffset"/> with the same date as this instance, and the time value set to 12:00:00 midnight (00:00:00).
        /// </summary>
        public System.DateTimeOffset Date { get { return new System.DateTimeOffset(this.Year, this.Month, this.Day, 0, 0, 0, System.TimeSpan.Zero); } }
        /// <summary>
        /// Gets the day of the month represented by this instance, expressed as value between 1 
        /// and the length of the month.
        /// </summary>
        public int Day { get { return _day + 1; } }
        private readonly int _day;
        /// <summary>
        /// Gets the hour component of the date represented by this instance, expressed as a value 
        /// between 0 and 23.
        /// </summary>
        public readonly int Hour;
        /// <summary>
        /// Gets the millisecond component of the date represented by this instance, expressed as a 
        /// value between 0 and 999.
        /// </summary>
        public readonly int Millisecond;
        /// <summary>
        /// Gets the minute component of the date represented by this instance, expressed as a 
        /// value between 0 and 59.
        /// </summary>
        public readonly int Minute;
        /// <summary>
        /// Gets the month component of the date represented by this instance, expressed as a value 
        /// between 1 and 12.
        /// </summary>
        public int Month { get { return _month + 1; } }
        private readonly int _month;
        /// <summary>
        /// Gets the seconds component of the date represented by this instance, expressed as a 
        /// value between 0 and 59.
        /// </summary>
        public readonly int Second;
        /// <summary>
        /// Gets the year component of the date represented by this instance, expressed as a value 
        /// between -250,000 and 250,000.
        /// </summary>
        public int Year { get { return _year + 1; } }
        private readonly int _year;
        /// <summary>
        /// Gets the era component of the date represented by this instance.
        /// </summary>
        public readonly EraType Era;
        /// <summary>
        /// Gets the number of ticks that represent the date and time of this instance either 
        /// before or after 0-1-1 00:00:00Z.
        /// </summary>
        public long Ticks { get { return this.TotalMilliseconds * TicksPerMillisecond; } }
        /// <summary>
        /// Get the number of milliseconds before or after 00-01-01 00:00:00.000Z
        /// </summary>
        public long TotalMilliseconds { get; private set; }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the value of the specified 
        /// <see cref="System.TimeSpan"/> to the value of this instance.
        /// </summary>
        /// <param name="value">A positive or negative time interval.</param>
        /// <returns>A new <see cref="Timestamp"/> whose value is the sum of the date and time 
        /// represented by this instance and the time interval represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value of the resulting 
        /// <see cref="Timestamp"/> would be less than <see cref="MinValue"/> or greater than 
        /// <see cref="MaxValue"/>.</exception>
        public Timestamp Add(System.TimeSpan value)
        {
            return this + value;
        }
        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of milliseconds to 
        /// the value of this instance.
        /// </summary>
        /// <param name="value">
        /// <para>A number of milliseconds to add to the <see cref="Timestamp"/>.</para>
        /// <para>The value parameter can be negative or positive.</para>
        /// </param>
        /// <returns>A new <see cref="Timestamp"/> whose value is the sum of the date and time 
        /// represented by this instance and the number of milliseconds represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value of the resulting 
        /// <see cref="Timestamp"/> would be less than <see cref="MinValue"/> or greater than 
        /// <see cref="MaxValue"/>.</exception>
        public Timestamp AddMilliseconds(int value)
        {
            return this.AddMilliseconds((long)value);
        }
        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of milliseconds to 
        /// the value of this instance.
        /// </summary>
        /// <param name="value">
        /// <para>A number of milliseconds to add to the <see cref="Timestamp"/>.</para>
        /// <para>The value parameter can be negative or positive.</para>
        /// </param>
        /// <returns>A new <see cref="Timestamp"/> whose value is the sum of the date and time 
        /// represented by this instance and the number of milliseconds represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value of the resulting 
        /// <see cref="Timestamp"/> would be less than <see cref="MinValue"/> or greater than 
        /// <see cref="MaxValue"/>.</exception>
        private Timestamp AddMilliseconds(long value)
        {
            return new Timestamp(this.TotalMilliseconds + value);
        }
        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of ticks to the 
        /// value of this instance.
        /// </summary>
        /// <param name="value">A number of 100-nanosecond ticks. The value parameter can be 
        /// positive or negative.</param>
        /// <returns>A new <see cref="Timestamp"/> whose value is the sum of the date and time 
        /// represented by this instance and the time represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value of the resulting 
        /// <see cref="Timestamp"/> would be less than <see cref="MinValue"/> or greater than 
        /// <see cref="MaxValue"/>.</exception>
        public Timestamp AddTicks(long value)
        {
            long milliseconds = value / TicksPerMillisecond;
            // round up the value to make Timestamp compliant with DateTime
            if (value - milliseconds >= TicksPerMillisecond / 2)
            {
                milliseconds += 1;
            }

            return this.AddMilliseconds(milliseconds);
        }
        /// <summary>
        /// Determines if a given year is considered a leap year or not.
        /// </summary>
        /// <param name="year">true if the value is a leap year; otherwise false.</param>
        /// <returns></returns>
        public bool IsLeapYear(int year)
        {
            return year % YearsPerLeapYear == 0 
                && (year % YearsPer1CLeapYear != 0 || year % YearsPer4CLeapYear == 0);
        }
        /// <summary>
        /// Determines whether the specified <see cref="Timestamp"/> is equal to a reference value.
        /// </summary>
        /// <param name="obj">The value to compare with the current value.</param>
        /// <returns> true if the specified value is equal to the value object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Timestamp))
                return false;

            return this.TotalMilliseconds == ((Timestamp)obj).TotalMilliseconds;
        }
        /// <summary>
        /// Serves as a hash function for a <see cref="Timestamp"/>.
        /// </summary>
        /// <returns>A 32-bit hashcode value.</returns>
        public override int GetHashCode()
        {
            return this.TotalMilliseconds.GetHashCode();
        }
        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        /// <returns>A UTC formatted string that represents the current value.</returns>
        public override string ToString()
        {
            return $"{Year:##########00}-{Month:00}-{Day:00} {Era} {Hour:00}:{Minute:00}:{Second:00}Z";
        }
        /// <summary>
        /// Compares the current instance with another object of the same type and returns an 
        /// integer that indicates whether the current instance precedes, follows, or occurs in the 
        /// same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the values being compared. The 
        /// return value has the following possible meanings: A value Less than zero indicates that 
        /// this object is less than object is was compared to. A value of zero indicates that the 
        /// value of this object is equal to object is was compared to. A value greater than zero 
        /// indicates that the value of this object is greater than the value is was compared to.</returns>
        /// <exception cref="System.ArgumentException">The 'obj' parameter is not <see cref="Timestamp"/>.</exception>
        int System.IComparable.CompareTo(object obj)
        {
            if (obj == null)
                throw new System.ArgumentNullException("obj");
            if (!(obj is Timestamp))
                throw new System.ArgumentException("The type of obj is not Timestamp", "obj");
            
            return this.TotalMilliseconds.CompareTo(((Timestamp)obj).TotalMilliseconds);
        }
        /// <summary>
        /// Compares the current value with another value of the same type.
        /// </summary>
        /// <param name="that">A value to compare with this value.</param>
        /// <returns>A value that indicates the relative order of the values being compared. The 
        /// return value has the following possible meanings: A value Less than zero indicates that 
        /// this object is less than object is was compared to. A value of zero indicates that the 
        /// value of this object is equal to object is was compared to. A value greater than zero 
        /// indicates that the value of this object is greater than the value is was compared to.</returns>
        int System.IComparable<Timestamp>.CompareTo(Timestamp that)
        {
            return this.TotalMilliseconds.CompareTo(that.TotalMilliseconds);
        }
        /// <summary>
        /// Indicates whether the current value is equal to another value of the same type.
        /// </summary>
        /// <param name="timestamp">A <see cref="Timestamp"/> value to compare with this value.</param>
        /// <returns>true if the current value is equal to the value parameter; otherwise, false.</returns>
        bool System.IEquatable<Timestamp>.Equals(Timestamp timestamp)
        {
            return this == timestamp;
        }
        /// <summary>
        /// Indicates whether the current value is equal to another value of a <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="datetime">A DateTime value to compare with this value.</param>
        /// <returns>true if the current value is equal to the value parameter; otherwise, false.</returns>
        bool System.IEquatable<System.DateTime>.Equals(System.DateTime datetime)
        {
            return this == datetime;
        }

        /// <summary>
        /// Subtracts a specified date and time from another specified date and time and returns a time interval.
        /// </summary>
        /// <param name="timestamp1">The date and time value to subtract from (the minuend).</param>
        /// <param name="timestamp2">The date and time value to subtract (the subtrahend).</param>
        /// <returns>The time interval between a and b; that is, a minus b.</returns>
        public static System.TimeSpan operator -(Timestamp timestamp1, Timestamp timestamp2)
        {
            long ticks = (timestamp1.TotalMilliseconds - timestamp2.TotalMilliseconds) * TicksPerMillisecond;
            return new System.TimeSpan(ticks);
        }
        /// <summary>
        /// Subtracts a specified time interval from a specified date and time and returns a new 
        /// date and time.
        /// </summary>
        /// <param name="timestamp">The date and time value to subtract from.</param>
        /// <param name="timespan">The time interval to subtract.</param>
        /// <returns>A new <see cref="Timestamp"/> whose value is the value of d minus the value of t.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The resulting 
        /// <see cref="Timestamp"/> would be less than <see cref="Timestamp.MinValue"/> or greater 
        /// than <see cref="Timestamp.MaxValue"/>.</exception>
        public static Timestamp operator -(Timestamp timestamp, System.TimeSpan timespan)
        {
            return new Timestamp(timestamp.TotalMilliseconds - (timespan.Ticks / TicksPerMillisecond));
        }
        /// <summary>
        /// Determines whether two specified instances of <see cref="Timestamp"/> are not equal.
        /// </summary>
        /// <param name="timestamp1">The first value to compare.</param>
        /// <param name="timestamp2">The second value to compare.</param>
        /// <returns>true if a and b do not represent the same date and time; otherwise, false.</returns>
        public static bool operator !=(Timestamp timestamp1, Timestamp timestamp2)
        {
            {
                return timestamp1.TotalMilliseconds != timestamp2.TotalMilliseconds;
            }
        }
        /// <summary>
        /// Adds a specified time interval to a specified date and time, yielding a new date and time.
        /// </summary>
        /// <param name="timestamp">The date and time value to add.</param>
        /// <param name="timespan">The time interval to add.</param>
        /// <returns>A new <see cref="Timestamp"/> that is the sum of the values of d and t.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value of the resulting 
        /// <see cref="Timestamp"/> would be less than <see cref="MinValue"/> or greater than 
        /// <see cref="MaxValue"/>.</exception>
        public static Timestamp operator +(Timestamp timestamp, System.TimeSpan timespan)
        {
            return new Timestamp(timestamp.TotalMilliseconds + (timespan.Ticks / TicksPerMillisecond));
        }
        /// <summary>
        /// Determines whether one specified Timestamp is less than another specified Timestamp.
        /// </summary>
        /// <param name="timestamp1">The first value to compare.</param>
        /// <param name="timestamp2">The second value to compare.</param>
        /// <returns>true if a is less than b; otherwise, false.</returns>
        public static bool operator <(Timestamp timestamp1, Timestamp timestamp2)
        {
            return timestamp1.TotalMilliseconds < timestamp2.TotalMilliseconds;
        }
        /// <summary>
        /// Determines whether one specified Timestamp is less than or equal to another specified 
        /// <see cref="Timestamp"/>.
        /// </summary>
        /// <param name="timestamp1">The first value to compare.</param>
        /// <param name="timestamp2">The second value to compare.</param>
        /// <returns>true if a is less than or equal to b; otherwise, false.</returns>
        public static bool operator <=(Timestamp timestamp1, Timestamp timestamp2)
        {
            return timestamp1.TotalMilliseconds <= timestamp2.TotalMilliseconds;
        }
        /// <summary>
        /// Determines whether two specified instances of <see cref="Timestamp"/> are equal.
        /// </summary>
        /// <param name="timestamp1">The first value to compare.</param>
        /// <param name="timestamp2">The second value to compare.</param>
        /// <returns>true if a and b represent the same date and time; otherwise, false.</returns>
        public static bool operator ==(Timestamp timestamp1, Timestamp timestamp2)
        {
            return timestamp1.TotalMilliseconds == timestamp2.TotalMilliseconds;
        }
        /// <summary>
        /// Determines whether an instance of <see cref="Timestamp"/> is equivolent to an instance 
        /// of <see cref="System.DateTime"/> (millisecond granularity).
        /// </summary>
        /// <param name="datetime">The first value to compare.</param>
        /// <param name="timestamp">The second value to compare.</param>
        /// <returns>true if a and b represent the same date and time; otherwise, false.</returns>
        public static bool operator ==(Timestamp timestamp, System.DateTime datetime)
        {
            // need to compare universal time vs timestamp, assume unknown is local (dangerous but datetime is stupid)
            if (datetime.Kind != System.DateTimeKind.Utc)
            {
                datetime = datetime.ToUniversalTime();
            }
            long milliseconds = datetime.Ticks / TicksPerMillisecond;
            // round up the value to make Timestamp compliant with DateTime
            if (timestamp.TotalMilliseconds - milliseconds >= TicksPerMillisecond / 2)
            {
                milliseconds += 1;
            }
            return timestamp.TotalMilliseconds == milliseconds;
        }
        /// <summary>
        /// Determines whether an instance of <see cref="Timestamp"/> is equivolent to an instance 
        /// of <see cref="System.DateTime"/> (millisecond granularity).
        /// </summary>
        /// <param name="datetime">The first value to compare.</param>
        /// <param name="timestamp">The second value to compare.</param>
        /// <returns>true if a and b represent the same date and time; otherwise, false.</returns>
        public static bool operator ==(System.DateTime datetime, Timestamp timestamp)
        {
            return timestamp == datetime;
        }
        /// <summary>
        /// Determines whether an instance of <see cref="Timestamp"/> is equivolent to an instance 
        /// of <see cref="System.DateTime"/> (millisecond granularity).
        /// </summary>
        /// <param name="timestamp">The first value to compare.</param>
        /// <param name="datetime">The second value to compare.</param>
        /// <returns>true if a and b do not represent the same date and time; otherwise, false.</returns>
        public static bool operator !=(Timestamp timestamp, System.DateTime datetime)
        {
            return !(timestamp == datetime);
        }
        /// <summary>
        /// Determines whether an instance of <see cref="Timestamp"/> is equivolent to an instance 
        /// of <see cref="System.DateTime"/> (millisecond granularity).
        /// </summary>
        /// <param name="datetime">The first value to compare.</param>
        /// <param name="timestamp">The second value to compare.</param>
        /// <returns>true if a and b do not represent the same date and time; otherwise, false.</returns>
        public static bool operator !=(System.DateTime datetime, Timestamp timestamp)
        {
            return !(datetime == timestamp);
        }
        /// <summary>
        /// Determines whether one specified <see cref="Timestamp"/> is greater than another 
        /// specified <see cref="Timestamp"/>.
        /// </summary>
        /// <param name="timestamp1">The first value to compare.</param>
        /// <param name="timestamp2">The second value to compare.</param>
        /// <returns> true if a is greater than b; otherwise, false.</returns>
        public static bool operator >(Timestamp timestamp1, Timestamp timestamp2)
        {
            return timestamp1.TotalMilliseconds > timestamp2.TotalMilliseconds;
        }
        /// <summary>
        /// Determines whether one specified <see cref="Timestamp"/> is greater than or equal to 
        /// another specified <see cref="Timestamp"/>.
        /// </summary>
        /// <param name="timestamp1">The first value to compare.</param>
        /// <param name="timestamp2">The second value to compare.</param>
        /// <returns>true if a is greater than or equal to b; otherwise, false.</returns>
        public static bool operator >=(Timestamp timestamp1, Timestamp timestamp2)
        {
            return timestamp1.TotalMilliseconds >= timestamp2.TotalMilliseconds;
        }
        /// <summary>
        /// Casts a <see cref="Timestamp"/> to a <see cref="System.DateTime"/>. Data lost can 
        /// occur as a <see cref="Timestamp"/> ignores values less than a millisecond.
        /// </summary>
        /// <param name="timestamp">The value to cast.</param>
        public static explicit operator System.DateTime(Timestamp timestamp)
        {
            return new System.DateTime(timestamp.TotalMilliseconds * TicksPerMillisecond, System.DateTimeKind.Utc);
        }
        /// <summary>
        /// Casts a <see cref="Timestamp"/> to a <see cref="System.DateTime"/>. Data lost can 
        /// occur as a <see cref="Timestamp"/> ignores values less than a millisecond.
        /// </summary>
        /// <param name="datetime">The value to cast.</param>
        public static explicit operator Timestamp(System.DateTime datetime)
        {
            return new Timestamp(datetime.ToUniversalTime());
        }
        /// <summary>
        /// Casts a <see cref="Timestamp"/> to a <see cref="System.DateTimeOffset"/>. Data lost can 
        /// occur as a <see cref="Timestamp"/> ignores values less than a millisecond.
        /// </summary>
        /// <param name="timestamp">The value to cast.</param>
        public static explicit operator System.DateTimeOffset(Timestamp timestamp)
        {
            return new System.DateTimeOffset(timestamp.Ticks, System.TimeSpan.Zero);
        }
        /// <summary>
        /// Casts a <see cref="Timestamp"/> to a <see cref="System.DateTimeOffset"/>. Data lost can 
        /// occur as a <see cref="Timestamp"/> ignores values less than a millisecond.
        /// </summary>
        /// <param name="datetimeOffset">The value to cast.</param>
        public static explicit operator Timestamp(System.DateTimeOffset datetimeOffset)
        {
            return new Timestamp(datetimeOffset.UtcTicks / TicksPerMillisecond);
        }
    }
}
