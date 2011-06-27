﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace BLToolkit.Data.Linq.Builder
{
	using BLToolkit.Linq;
	using Data.Sql;

	class FirstSingleBuilder : MethodCallBuilder
	{
		protected override bool CanBuildMethodCall(ExpressionBuilder builder, MethodCallExpression methodCall, BuildInfo buildInfo)
		{
			return 
				methodCall.IsQueryable("First", "FirstOrDefault", "Single", "SingleOrDefault") &&
				methodCall.Arguments.Count == 1;
		}

		protected override IBuildContext BuildMethodCall(ExpressionBuilder builder, MethodCallExpression methodCall, BuildInfo buildInfo)
		{
			var sequence = builder.BuildSequence(new BuildInfo(buildInfo, methodCall.Arguments[0]));
			var take     = 0;

			if (!buildInfo.IsSubQuery || builder.SqlProvider.IsSubQueryTakeSupported)
				switch (methodCall.Method.Name)
				{
					case "First"           :
					case "FirstOrDefault"  :
						take = 1;
						break;

					case "Single"          :
					case "SingleOrDefault" :
						if (!buildInfo.IsSubQuery)
							take = 2;
						break;
				}

			if (take != 0)
				builder.BuildTake(sequence, new SqlValue(take));

			return new FirstSingleContext(buildInfo.Parent, sequence, methodCall);
		}

		protected override SequenceConvertInfo Convert(
			ExpressionBuilder builder, MethodCallExpression methodCall, BuildInfo buildInfo, ParameterExpression param)
		{
			return null;
		}

		class FirstSingleContext : SequenceContextBase
		{
			public FirstSingleContext(IBuildContext parent, IBuildContext sequence, MethodCallExpression methodCall)
				: base(parent, sequence, null)
			{
				_methodCall = methodCall;
			}

			readonly MethodCallExpression _methodCall;

			public override void BuildQuery<T>(Query<T> query, ParameterExpression queryParameter)
			{
				Sequence.BuildQuery(query, queryParameter);

				switch (_methodCall.Method.Name)
				{
					case "First"           : query.GetElement = (ctx, db, expr, ps) => query.GetIEnumerable(ctx, db, expr, ps).First();           break;
					case "FirstOrDefault"  : query.GetElement = (ctx, db, expr, ps) => query.GetIEnumerable(ctx, db, expr, ps).FirstOrDefault();  break;
					case "Single"          : query.GetElement = (ctx, db, expr, ps) => query.GetIEnumerable(ctx, db, expr, ps).Single();          break;
					case "SingleOrDefault" : query.GetElement = (ctx, db, expr, ps) => query.GetIEnumerable(ctx, db, expr, ps).SingleOrDefault(); break;
				}
			}

			public override Expression BuildExpression(Expression expression, int level)
			{
				if (expression == null)
					//if (Sequence.IsExpression(expression, level, RequestFor.Object))
					//	return Sequence.BuildExpression(expression, level);
					//else
						return Builder.BuildSql(_methodCall.Type, Parent.SqlQuery.Select.Add(SqlQuery));

				throw new NotImplementedException();
				//return Sequence.BuildExpression(expression, level + 1);
			}

			public override SqlInfo[] ConvertToSql(Expression expression, int level, ConvertFlags flags)
			{
				return Sequence.ConvertToSql(expression, level + 1, flags);
			}

			public override SqlInfo[] ConvertToIndex(Expression expression, int level, ConvertFlags flags)
			{
				return Sequence.ConvertToIndex(expression, level, flags);
			}

			public override bool IsExpression(Expression expression, int level, RequestFor requestFlag)
			{
				return Sequence.IsExpression(expression, level, requestFlag);
			}

			public override IBuildContext GetContext(Expression expression, int level, BuildInfo buildInfo)
			{
				throw new NotImplementedException();
			}
		}
	}
}