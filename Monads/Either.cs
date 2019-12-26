using System;
using System.Collections.Generic;

namespace Monads
{
    public struct Either<L, R>
    {
        private readonly L left;
        private readonly R right;
        private readonly bool isRight;
        private readonly bool isLeft;

        public bool IsBottom => !(isLeft || isRight);
        public Either(R right)
        {
            if (right == null)
                throw new ArgumentNullException();
            this.right = right;
            left = default;
            isRight = true;
            isLeft = false;
        }
        public Either(L left)
        {
            if (left == null)
                throw new ArgumentNullException();
            this.left = left;
            right = default;
            isLeft = true;
            isRight = false;
        }

        public T FromLeft<T>(Func<L, T> func, T def)
        {
            if (IsBottom)
                throw Either.isBottomException;
            return isLeft ? func.Invoke(left) : def;
        }
        public T FromRight<T>(Func<L, T> func, T def)
        {
            if (IsBottom)
                throw Either.isBottomException;
            return isLeft ? func.Invoke(left) : def;
        }

        public T Match<T>(Func<L, T> l, Func<R, T> r)
        {
            if (isLeft) return l.Invoke(left);
            if (isRight) return r.Invoke(right);
            throw Either.isBottomException;
        }

        public static implicit operator Either<L, R>(L left)
        {
            return new Either<L, R>(left);
        }
        public static implicit operator Either<L, R>(R right)
        {
            return new Either<L, R>(right);
        }

    }

    static class Either
    {
        public static readonly ArgumentException isBottomException = new ArgumentException("Neither left nor right state");
    }
}