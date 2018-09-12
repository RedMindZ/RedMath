using System;
using System.Collections.Generic;

using RedMath.LinearAlgebra;

namespace RedMath.Structures.Expression
{

    public abstract class BaseExpressionNode<T>
    {
        public abstract T Evaluate();
    }

    public abstract class BinaryExpressionNode<T, LChildType, RChildType> : BaseExpressionNode<T>
    {
        public BaseExpressionNode<LChildType> LeftChild { get; set; }
        public BaseExpressionNode<RChildType> RightChild { get; set; }

        protected string WrapString(string str)
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

    public class AddNode<T> : BinaryExpressionNode<T, T, T> where T : Field<T>, new()
    {
        public override T Evaluate()
        {
            return LeftChild.Evaluate() + RightChild.Evaluate();
        }

        public override string ToString()
        {
            return WrapString("+");
        }
    }

    public class MultiplyNode<T> : BinaryExpressionNode<T, T, T> where T : Field<T>, new()
    {
        public override T Evaluate()
        {
            return LeftChild.Evaluate() * RightChild.Evaluate();
        }

        public override string ToString()
        {
            return WrapString("*");
        }
    }

    public class SubtractNode<T> : BinaryExpressionNode<T, T, T> where T : Field<T>, new()
    {
        public override T Evaluate()
        {
            return LeftChild.Evaluate() - RightChild.Evaluate();
        }

        public override string ToString()
        {
            return WrapString("-");
        }
    }

    public class DivideNode<T> : BinaryExpressionNode<T, T, T> where T : Field<T>, new()
    {
        public override T Evaluate()
        {
            return LeftChild.Evaluate() / RightChild.Evaluate();
        }

        public override string ToString()
        {
            return WrapString("/");
        }
    }
}
