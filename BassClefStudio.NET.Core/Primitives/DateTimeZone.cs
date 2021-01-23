﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Primitives
{
    /// <summary>
    /// Represents a <see cref="DateTime"/> with knowledge of the time zone it belongs to.
    /// </summary>
    public struct DateTimeZone : IComparable, IComparable<DateTimeOffset>, IEquatable<DateTimeOffset>, IComparable<DateTimeZone>, IEquatable<DateTimeZone>
    {
        /// <summary>
        /// The date-time of this <see cref="DateTimeZone"/>, with no time-zone information attached.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The specific time-zone that this <see cref="DateTimeZone"/> date occurs in.
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }

        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> representing this <see cref="DateTimeZone"/>'s exact point in time with the correct offset from UTC.
        /// </summary>
        public DateTimeOffset OffsetDateTime => new DateTimeOffset(DateTime, (TimeZone ?? TimeZoneInfo.Local).GetUtcOffset(DateTime));

        /// <summary>
        /// Creates a new <see cref="DateTimeZone"/>.
        /// </summary>
        /// <param name="dateTime">The date-time of this <see cref="DateTimeZone"/>, with no time-zone information attached.</param>
        /// <param name="timeZone">The specific time-zone that this <see cref="DateTimeZone"/> date occurs in.</param>
        public DateTimeZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            DateTime = dateTime;
            TimeZone = timeZone;
        }

        /// <summary>
        /// Creates a new <see cref="DateTimeZone"/> for the local time zone.
        /// </summary>
        /// <param name="dateTime">The date-time of this <see cref="DateTimeZone"/>, with no time-zone information attached.</param>
        public DateTimeZone(DateTime dateTime)
        {
            DateTime = dateTime;
            TimeZone = TimeZoneInfo.Local;
        }

        #region Operations

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            return ((IComparable)OffsetDateTime).CompareTo(obj);
        }

        /// <inheritdoc/>
        public int CompareTo(DateTimeOffset other)
        {
            return OffsetDateTime.CompareTo(other);
        }

        /// <inheritdoc/>
        public int CompareTo(DateTimeZone other)
        {
            return OffsetDateTime.CompareTo(other.OffsetDateTime);
        }

        /// <inheritdoc/>
        public bool Equals(DateTimeOffset other)
        {
            return OffsetDateTime.Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(DateTimeZone other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is DateTimeZone dateTimeZone
                && this == dateTimeZone;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Checks if two <see cref="DateTimeZone"/>s represent the same point in time and time-zone.
        /// </summary>
        public static bool operator ==(DateTimeZone a, DateTimeZone b)
        {
            return a.DateTime.Equals(b.DateTime)
                && a.TimeZone.Equals(b.TimeZone);
        }

        /// <summary>
        /// Checks if two <see cref="DateTimeZone"/>s represent different points in time and time-zone.
        /// </summary>
        public static bool operator !=(DateTimeZone a, DateTimeZone b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Adds a <see cref="TimeSpan"/> to a <see cref="DateTimeZone"/>.
        /// </summary>
        public static DateTimeZone operator +(DateTimeZone a, TimeSpan b)
        {
            return new DateTimeZone(a.DateTime + b, a.TimeZone);
        }

        /// <summary>
        /// Adds a <see cref="TimeSpan"/> to a <see cref="DateTimeZone"/>.
        /// </summary>
        public static DateTimeZone operator +(TimeSpan b, DateTimeZone a)
        {
            return new DateTimeZone(a.DateTime + b, a.TimeZone);
        }

        /// <summary>
        /// Subtracts a <see cref="TimeSpan"/> from a <see cref="DateTimeZone"/>.
        /// </summary>
        public static DateTimeZone operator -(TimeSpan b, DateTimeZone a)
        {
            return new DateTimeZone(a.DateTime - b, a.TimeZone);
        }

        /// <summary>
        /// Subtracts two <see cref="DateTimeZone"/>s together, returning the <see cref="TimeSpan"/> difference between them.
        /// </summary>
        public static TimeSpan operator -(DateTimeZone a, DateTimeZone b)
        {
            return a.OffsetDateTime - b.OffsetDateTime;
        }

        #endregion
    }
}