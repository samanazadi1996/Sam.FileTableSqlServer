using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sam.FileTableFramework.Linq
{
    public static class FileTableFrameworkLinq
    {
        public static FtDbSet Skip(this FtDbSet dbset, int skip)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();

            dbset.Query.Skip = skip;
            return dbset;
        }
        public static FtDbSet Take(this FtDbSet dbset, int take)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();

            dbset.Query.Take = take;
            return dbset;
        }
        public static FtDbSet OrderByDescending<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> keySelector)
        {
            return OrderBy(dbset, keySelector, true);
        }
        public static FtDbSet Where(this FtDbSet dbset, params string[] whereClause)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();
            if (whereClause != null)
            {
                dbset.Query.Where ??= new List<string>();
                dbset.Query.Where.AddRange(whereClause);
            }

            return dbset;
        }
        public static FtDbSet OrderBy<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> keySelector, bool orderByDescending = false)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();
            dbset.Query.OrderByDescending = orderByDescending;
            dbset.Query.OrderBy ??= new List<string>();

            // Extract the field name from the keySelector expression
            if (keySelector.Body is MemberExpression memberExpr)
            {
                dbset.Query.OrderBy.Add(memberExpr.Member.Name);
            }
            else if (keySelector.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression unaryMemberExpr)
            {
                dbset.Query.OrderBy.Add(unaryMemberExpr.Member.Name);
            }
            else
            {
                throw new InvalidOperationException("Unsupported expression type for OrderBy.");
            }

            return dbset;
        }
        public static FtDbSet Select<T>(this FtDbSet dbset, Expression<Func<FileEntity, T>> selector)
        {
            dbset.Query ??= new Context.Internall.ContextQuery();

            var selectList = new List<string>();

            void ProcessMemberExpression(MemberExpression memberExpr, string alias = null)
            {
                var fieldName = memberExpr.Member.Name;
                alias ??= memberExpr.Member.Name;
                selectList.Add($"{fieldName} AS {alias}");
            }

            try
            {
                switch (selector.Body)
                {
                    case NewExpression newExpr:
                        for (int i = 0; i < newExpr.Arguments.Count; i++)
                        {
                            if (newExpr.Arguments[i] is MemberExpression memberExpr)
                            {
                                var alias = newExpr.Members[i].Name;
                                ProcessMemberExpression(memberExpr, alias);
                            }
                            else if (newExpr.Arguments[i] is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression unaryMemberExpr)
                            {
                                var alias = newExpr.Members[i].Name;
                                ProcessMemberExpression(unaryMemberExpr, alias);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported expression type in NewExpression at index {i}: {newExpr.Arguments[i].GetType().Name}");
                            }
                        }
                        break;

                    case MemberExpression memberExpr:
                        ProcessMemberExpression(memberExpr);
                        break;

                    case UnaryExpression unaryExpr when unaryExpr.Operand is MemberExpression unaryMemberExpr:
                        ProcessMemberExpression(unaryMemberExpr);
                        break;

                    case MemberInitExpression memberInitExpr:
                        foreach (var binding in memberInitExpr.Bindings)
                        {
                            if (binding is MemberAssignment assignment && assignment.Expression is MemberExpression assignmentMemberExpr)
                            {
                                var alias = binding.Member.Name;
                                ProcessMemberExpression(assignmentMemberExpr, alias);
                            }
                            else if (binding is MemberAssignment assignmentUnary && assignmentUnary.Expression is UnaryExpression unaryAssignmentExpr && unaryAssignmentExpr.Operand is MemberExpression unaryAssignmentMemberExpr)
                            {
                                var alias = binding.Member.Name;
                                ProcessMemberExpression(unaryAssignmentMemberExpr, alias);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported expression type in MemberInitExpression: {binding.GetType().Name}");
                            }
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported expression type: {selector.Body.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to parse selector expression.", ex);
            }

            dbset.Query.Fields = selectList.ToArray();
            return dbset;
        }


    }
}
