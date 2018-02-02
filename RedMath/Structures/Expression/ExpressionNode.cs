using System;
using System.Collections.Generic;

using RedMath.LinearAlgebra;

namespace RedMath.Structures.Expression
{
    public abstract class ExpressionNode<T>
    {
        public abstract T Evaluate<P>(Dictionary<P, object> feedDict = null);
    }

    public abstract class ExpressionNode<T, L, R> : ExpressionNode<T>
    {
        public ExpressionNode<L> Left { get; protected set; }
        public ExpressionNode<R> Right { get; protected set; }
    }

    public class FieldAddNode<T> : ExpressionNode<T, T, T> where T : Field<T>, new()
    {
        public override T Evaluate<P>(Dictionary<P, object> feedDict = null)
        {
            return Left.Evaluate(feedDict).Add(Right.Evaluate(feedDict));
        }
    }
}
