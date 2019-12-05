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

        private bool isDefault => !(isLeft || isRight);
        public Either(R right)
        {
            if(EqualityComparer<R>.Default.Equals(right, default))
                throw new ArgumentNullException();
            this.right = right;
            this.left = default;
            this.isRight = true;
            this.isLeft = false;
        }
        public Either(L left)
        {
            if(EqualityComparer<L>.Default.Equals(left, default))
                throw new ArgumentNullException();
            this.left = left;
            this.right = default;
            this.isLeft = true;
            this.isRight = false;
        }

        public T FromLeft<T>(Func<L, T> func, T def)
        {
            if(isDefault)
                throw new ArgumentException("Struct not initialized.");
            return isLeft ? func.Invoke(left) : def;
        }
        public T FromRight<T>(Func<L, T> func, T def)
        {
            if(isDefault)
                throw new ArgumentException("Struct not initialized.");
            return isLeft ? func.Invoke(left) : def;
        }

        public T Match<T>(Func<L, T> l, Func<R, T> r)
        {
            if (isLeft) return l.Invoke(left);
            if (isRight) return r.Invoke(right);
            throw new ArgumentException("Struct not initialized.");
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
}