﻿using System;

using Alea;
using Alea.CSharp;
using Alea.cuBLAS;

using RedMath.LinearAlgebra;
using RedMath.Structures;
using RedMath.Utils;
using RedMath.Gpu;

namespace RedMath.HighPerformance
{
    public static partial class MatrixMultiplication
    {
        public static Matrix<FieldType> GpuMultiply<FieldType, GpuStructType>(Matrix<FieldType> left, Matrix<FieldType> right) where FieldType : Field<FieldType>, IGpuCompatibleField<FieldType, GpuStructType>, new() where GpuStructType : struct
        {
            if (left.Width != right.Height)
            {
                throw new InvalidOperationException("Matrices of incompatible sizes can't be multiplied.");
            }

            IGpuStructManager<FieldType, GpuStructType> gpuStructManager = new FieldType().GetDefaultGpuStructManager();

            GpuStructType[,] resultArr = new GpuStructType[left.Rows, right.Columns];
            GpuStructType[,] leftArr = new GpuStructType[left.Rows, left.Columns];
            GpuStructType[,] rightArr = new GpuStructType[right.Rows, right.Columns];

            resultArr.AssignAll(gpuStructManager.GetStructDefaultValue());
            leftArr.AssignAll(ind => gpuStructManager.ToStruct(left[ind[0], ind[1]]));
            rightArr.AssignAll(ind => gpuStructManager.ToStruct(right[ind[0], ind[1]]));


            Alea.Gpu gpu = Alea.Gpu.Default;

            int threadCount = left.Rows * right.Columns;
            int blockDimX = gpu.Device.Attributes.MaxThreadsPerBlock; // Threads per block
            int gridDimX = (int)Math.Ceiling((double)threadCount / blockDimX); // Blocks per thread

            LaunchParam lp = new LaunchParam(gridDimX, blockDimX);
            gpu.Launch(multiplicationKernel, lp, leftArr, rightArr, resultArr, gpuStructManager.GetStructAddition(), gpuStructManager.GetStructMultiplication());

            FieldType[,] fieldResultArr = new FieldType[resultArr.GetLength(0), resultArr.GetLength(1)];
            fieldResultArr.AssignAll(ind => gpuStructManager.ToClass(resultArr[ind[0], ind[1]]));

            return new Matrix<FieldType>(fieldResultArr);
        }

        private static void multiplicationKernel<T>(T[,] leftMat, T[,] rightMat, T[,] resultMat, Func<T, T, T> addOp, Func<T, T, T> mulOp)
        {
            int leftHeight = leftMat.GetLength(0);
            int leftWidth = leftMat.GetLength(1);

            int rightWidth = rightMat.GetLength(0);

            int index = blockDim.x * blockIdx.x + threadIdx.x;
            int rowIndex = index / leftHeight;
            int colIndex = index % rightWidth;

            if (rowIndex >= resultMat.GetLength(0) || colIndex >= resultMat.GetLength(1))
            {
                return;
            }

            for (int k = 0; k < leftWidth; k++)
            {
                resultMat[rowIndex, colIndex] = addOp(resultMat[rowIndex, colIndex], mulOp(leftMat[rowIndex, k], rightMat[k, colIndex]));
            }
        }
    }
}
