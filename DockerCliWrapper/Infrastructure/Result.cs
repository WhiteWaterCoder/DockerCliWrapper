using System.Collections.Generic;

namespace DockerCliWrapper.Infrastructure
{
    public struct Result
    {
        public bool IsSuccessFull { get; }
        public string ErrorMessage { get; }

        public Result(bool isSuccessFull, string errorMessage) 
            : this()
        {
            IsSuccessFull = isSuccessFull;
            ErrorMessage = errorMessage;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Result))
            {
                return false;
            }

            var result = (Result)obj;
            return IsSuccessFull == result.IsSuccessFull &&
                   ErrorMessage == result.ErrorMessage;
        }

        public override int GetHashCode()
        {
            var hashCode = 476852437;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + IsSuccessFull.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ErrorMessage);
            return hashCode;
        }
    }
}