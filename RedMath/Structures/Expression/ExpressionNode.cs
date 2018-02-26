using System;
using System.Collections.Generic;

using RedMath.LinearAlgebra;

namespace RedMath.Structures.Expression
{
    internal static class StringConstants
    {
        internal static string TreeStartString = "\u2514";
        internal static string TreeMiddleString = "\u2500\u2500\u2500";
        internal static string TreePrefixString = TreeStartString + TreeMiddleString;
    }

    public abstract class BaseExpressionNode<T>
    {
        public abstract T Evaluate();
    }

    public abstract class BinaryExpressionNode<T, LChildType, RChildType> : BaseExpressionNode<T>
    {
        public BaseExpressionNode<LChildType> LeftChild { get; set; }
        public BaseExpressionNode<RChildType> RightChild { get; set; }

        protected string wrapString(string str)
        {
            return "(" + LeftChild.ToString() + ")" + " " + str + " " + "(" + RightChild.ToString() + ")";
        }
    }

    public class ConstantNode<T> : BaseExpressionNode<T>
    {
        public T Value { get; set; }

        public ConstantNode(T val)
        {
            Value = val;
        }

        public override T Evaluate()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class AddNode<T> : BinaryExpressionNode<T, T, T> where T : Field<T>
    {
        public override T Evaluate()
        {
            return Field<T>.Add(LeftChild.Evaluate(), RightChild.Evaluate());
        }

        public override string ToString()
        {
            return wrapString("+");
        }
    }

    public class MultiplyNode<T> : BinaryExpressionNode<T, T, T> where T : Field<T>
    {
        public override T Evaluate()
        {
            return Field<T>.Multiply(LeftChild.Evaluate(), RightChild.Evaluate());
        }

        public override string ToString()
        {
            return wrapString("*");
        }
    }

    public class SubtractNode<T> : BinaryExpressionNode<T, T, T> where T : Field<T>
    {
        public override T Evaluate()
        {
            return Field<T>.Subtract(LeftChild.Evaluate(), RightChild.Evaluate());
        }

        public override string ToString()
        {
            return wrapString("-");
        }
    }

    public class DivideNode<T> : BinaryExpressionNode<T, T, T> where T : Field<T>
    {
        public override T Evaluate()
        {
            return Field<T>.Divide(LeftChild.Evaluate(), RightChild.Evaluate());
        }

        public override string ToString()
        {
            return wrapString("/");
        }
    }
}
