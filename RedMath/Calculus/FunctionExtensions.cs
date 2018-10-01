using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public static class FunctionExtensions
    {
        /// <summary>
        /// Composes two functions and returns a <see cref="FComposition{IFD, SRD, OFR, PF}"/> object.
        /// </summary>
        /// <typeparam name="IFD">Inner Function Domain - The type of the domain of the inner function.</typeparam>
        /// <typeparam name="SRD">Shared Range/Domain - The type of the range/domain that is shared between the inner and outer function.</typeparam>
        /// <typeparam name="OFR">Outer Function Range - The type of the range of the outer function.</typeparam>
        /// <typeparam name="PF">Parameters Field - The type of the field of the parameters of the function.</typeparam>
        /// <param name="outerFunction">The outer function of the composition.</param>
        /// <param name="innerFunction">The inner function of the composition.</param>
        /// <returns>The composition of <paramref name="outerFunction"/> and <paramref name="innerFunction"/></returns>
        public static FComposition<IFD, SRD, OFR, PF> Compose<IFD, SRD, OFR, PF>(this IOptimizableFunction<SRD, OFR, PF> outerFunction, IOptimizableFunction<IFD, SRD, PF> innerFunction)
            where PF : Field<PF>, new()
        {
            return new FComposition<IFD, SRD, OFR, PF>(outerFunction, innerFunction);
        }

        /// <summary>
        /// Sums two <see cref="IOptimizableFunction{D, R, PF}"/> functions.
        /// </summary>
        /// <typeparam name="D">The domain of the functions.</typeparam>
        /// <typeparam name="R">The range of the functions.</typeparam>
        /// <typeparam name="PF">The type of the field of the parameters of the functions.</typeparam>
        /// <param name="leftFunc">The first function of the sum.</param>
        /// <param name="rightFunc">The second function of the sum.</param>
        /// <returns>The sum of <paramref name="leftFunc"/> and <paramref name="rightFunc"/></returns>
        public static FAddition<D, R, PF> Add<D, R, PF>(this IOptimizableFunction<D, R, PF> leftFunc, IOptimizableFunction<D, R, PF> rightFunc)
            where R : IAddable<R>
            where PF : Field<PF>, new()
        {
            return new FAddition<D, R, PF>(leftFunc, rightFunc);
        }

        /// <summary>
        /// Subtracts two <see cref="IOptimizableFunction{D, R, PF}"/> functions.
        /// </summary>
        /// <typeparam name="D">The domain of the functions.</typeparam>
        /// <typeparam name="R">The range of the functions.</typeparam>
        /// <typeparam name="PF">The type of the field of the parameters of the functions.</typeparam>
        /// <param name="leftFunc">The first function of the subtraction.</param>
        /// <param name="rightFunc">The second function of the subtraction.</param>
        /// <returns>The subtraction of <paramref name="leftFunc"/> and <paramref name="rightFunc"/></returns>
        public static FSubtraction<D, R, PF> Subtract<D, R, PF>(this IOptimizableFunction<D, R, PF> leftFunc, IOptimizableFunction<D, R, PF> rightFunc)
            where R : ISubtractable<R>
            where PF : Field<PF>, new()
        {
            return new FSubtraction<D, R, PF>(leftFunc, rightFunc);
        }

        /// <summary>
        /// Multiplies two <see cref="IOptimizableFunction{D, R, PF}"/> functions.
        /// </summary>
        /// <typeparam name="D">The domain of the functions.</typeparam>
        /// <typeparam name="R">The range of the functions.</typeparam>
        /// <param name="leftFunc">The first function of the product.</param>
        /// <param name="rightFunc">The second function of the product.</param>
        /// <returns>The product of <paramref name="leftFunc"/> and <paramref name="rightFunc"/></returns>
        public static FMultiplication<D, R> Multiply<D, R>(this IOptimizableFunction<D, R, R> leftFunc, IOptimizableFunction<D, R, R> rightFunc)
            where R : Field<R>, new()
        {
            return new FMultiplication<D, R>(leftFunc, rightFunc);
        }

        /// <summary>
        /// Divides two <see cref="IOptimizableFunction{D, R, PF}"/> functions.
        /// </summary>
        /// <typeparam name="D">The domain of the functions.</typeparam>
        /// <typeparam name="R">The range of the functions.</typeparam>
        /// <param name="leftFunc">The first function of the quotient.</param>
        /// <param name="rightFunc">The second function of the quotient.</param>
        /// <returns>The quotient of <paramref name="leftFunc"/> and <paramref name="rightFunc"/></returns>
        public static FDivision<D, R> Divide<D, R>(this IOptimizableFunction<D, R, R> leftFunc, IOptimizableFunction<D, R, R> rightFunc)
            where R : Field<R>, new()
        {
            return new FDivision<D, R>(leftFunc, rightFunc);
        }

        public static (D[], R[]) Plot<D, R>(this IFunction<D, R> func, params D[] plotPoints)
        {
            R[] functionPlot = new R[plotPoints.Length];

            for (int i = 0; i < functionPlot.Length; i++)
            {
                functionPlot[i] = func.Compute(plotPoints[i]);
            }

            return (plotPoints, functionPlot);
        }

        public static Matrix<T> Plot<T>(this IFunction<T, T> func, params T[] plotPoints) where T : Field<T>, new()
        {
            Matrix<T> functionPlot = new Matrix<T>(2, plotPoints.Length);

            for (int i = 0; i < functionPlot.Columns; i++)
            {
                functionPlot[0, i] = plotPoints[i];
                functionPlot[1, i] = func.Compute(plotPoints[i]);
            }

            return functionPlot;
        }
    }
}
