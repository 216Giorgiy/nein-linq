﻿using NeinLinq.Fakes.NullsafeQuery;
using NeinLinq.Queryable;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.NullsafeQuery
{
    public class QueryTest
    {
        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldSelectStructMember()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Year = a.OneDay.Year
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal(1, r.Year),
                r => Assert.Equal(1977, r.Year),
                r => Assert.Equal(1980, r.Year),
                r => Assert.Equal(1983, r.Year),
                r => Assert.Equal(2015, r.Year));
        }

        [Fact]
        public void ShouldSelectClassMember()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Numeric = a.SomeOther.SomeNumeric
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(42, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric));
        }

        [Fact]
        public void ShouldSelectMethodCall()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Question = a.SomeText.Contains("?")
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal(false, r.Question),
                r => Assert.Equal(false, r.Question),
                r => Assert.Equal(true, r.Question),
                r => Assert.Equal(false, r.Question),
                r => Assert.Equal(false, r.Question));
        }

        [Fact]
        public void ShouldSelectMethodCallResult()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            FirstWord = a.SomeText.Split(' ')[0]
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal("", r.FirstWord),
                r => Assert.Equal("Narf", r.FirstWord),
                r => Assert.Equal("What", r.FirstWord),
                r => Assert.Equal("", r.FirstWord),
                r => Assert.Equal("", r.FirstWord));
        }

        [Fact]
        public void ShouldSelectEnumerable()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Other = from b in a.SomeOthers
                                    select b.OneDay.Month
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Empty(r.Other),
                r => Assert.Empty(r.Other),
                r => Assert.Equal(new[] { 3, 6 }, r.Other),
                r => Assert.Empty(r.Other),
                r => Assert.Empty(r.Other));
        }

        [Fact]
        public void ShouldSelectCollection()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            More = from c in a.MoreOthers
                                   select c.SomeOther.OneDay.Day
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Empty(r.More),
                r => Assert.Empty(r.More),
                r => Assert.Empty(r.More),
                r => Assert.Equal(new[] { 5, 8 }, r.More),
                r => Assert.Empty(r.More));
        }

        [Fact]
        public void ShouldSelectSet()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Lot = from d in a.EvenLotMoreOthers
                                  select d.SomeOther.OneDay.Day
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Empty(r.Lot),
                r => Assert.Empty(r.Lot),
                r => Assert.Empty(r.Lot),
                r => Assert.Empty(r.Lot),
                r => Assert.Equal(new[] { 4, 7}, r.Lot));
        }

        [Fact]
        public void ShouldSelectNullables()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Numeric = a.DaNullable.Value
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(2017, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric));
        }

        [Fact]
        public void ShouldIgnoreStaticMember()
        {
            var query = from _ in Enumerable.Range(1, 3).AsQueryable().ToNullsafe()
                        select new
                        {
                            Property = Guid.Empty,
                            Method = Guid.NewGuid()
                        };

            var result = query.ToList();

            Assert.Equal(3, result.Count);
        }
    }
}
