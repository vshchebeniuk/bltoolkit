﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

namespace Data.Linq
{
	using Model;

	[TestFixture]
	public class JoinTest : TestBase
	{
		[Test]
		public void InnerJoin1()
		{
			TestJohn(db =>
				from p1 in db.Person
					join p2 in db.Person on p1.ID equals p2.ID
				where p1.ID == 1
				select new Person { ID = p1.ID, FirstName = p2.FirstName });
		}

		[Test]
		public void InnerJoin2()
		{
			TestJohn(db =>
				from p1 in db.Person
					join p2 in db.Person on new { p1.ID, p1.FirstName } equals new { p2.ID, p2.FirstName }
				where p1.ID == 1
				select new Person { ID = p1.ID, FirstName = p2.FirstName });
		}

		[Test]
		public void InnerJoin3()
		{
			TestJohn(db =>
				from p1 in db.Person
					join p2 in
						from p2 in db.Person join p3 in db.Person on new { p2.ID, p2.LastName } equals new { p3.ID, p3.LastName } select new { p2, p3 }
					on new { p1.ID, p1.FirstName } equals new { p2.p2.ID, p2.p2.FirstName }
				where p1.ID == 1
				select new Person { ID = p1.ID, FirstName = p2.p2.FirstName, LastName = p2.p3.LastName });
		}

		[Test]
		public void InnerJoin4()
		{
			TestJohn(db =>
				from p1 in db.Person
					join p2 in db.Person on new { p1.ID, p1.FirstName } equals new { p2.ID, p2.FirstName }
						join p3 in db.Person on new { p2.ID, p2.LastName } equals new { p3.ID, p3.LastName }
				where p1.ID == 1
				select new Person { ID = p1.ID, FirstName = p2.FirstName, LastName = p3.LastName });
		}

		[Test]
		public void InnerJoin5()
		{
			TestJohn(db =>
				from p1 in db.Person
					join p2 in db.Person on new { p1.ID, p1.FirstName } equals new { p2.ID, p2.FirstName }
						join p3 in db.Person on new { p1.ID, p2.LastName } equals new { p3.ID, p3.LastName }
				where p1.ID == 1
				select new Person { ID = p1.ID, FirstName = p2.FirstName, LastName = p3.LastName });
		}

		[Test]
		public void LeftJoin1()
		{
			ForEachProvider(db =>
			{
				var q = 
					from p in db.Parent
						join ch in db.Child on p.ParentID equals ch.ParentID into lj1
					where p.ParentID == 1
					select p;

				var list = q.ToList();

				Assert.AreEqual(1, list.Count);
				Assert.AreEqual(1, list[0].ParentID);
			});
		}

		[Test]
		public void LeftJoin2()
		{
			ForEachProvider(db =>
			{
				var q = 
					from p in db.Parent
						join ch in db.Child on p.ParentID equals ch.ParentID into lj1
					where p.ParentID == 1
					select new { p, lj1 };

				var list = q.ToList();

				Assert.AreEqual(1, list.Count);
				Assert.AreEqual(1, list[0].p.ParentID);
			});
		}

		[Test]
		public void LeftJoin3()
		{
			ForEachProvider(db =>
			{
				var q = 
					from p in db.Parent
						join ch in db.Child on p.ParentID equals ch.ParentID into lj1
						from ch in lj1.DefaultIfEmpty()
					where p.ParentID == 1
					select new { p, ch };

				var list = q.ToList();

				Assert.AreEqual(1, list.Count);
				Assert.AreEqual(1, list[0].p.ParentID);
			});
		}
	}
}