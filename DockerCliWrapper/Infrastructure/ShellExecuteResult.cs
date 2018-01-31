using System.Collections.Generic;

namespace DockerCliWrapper.Infrastructure
{
    public struct ShellExecuteResult
    {
        public bool IsSuccessFull { get; }
        public string Output { get; }
        public string Error { get; }

        public ShellExecuteResult(bool isSuccessFull, string output, string error) : this()
        {
            IsSuccessFull = isSuccessFull;
            Output = output;
            Error = error;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ShellExecuteResult))
            {
                return false;
            }

            var result = (ShellExecuteResult)obj;
            return IsSuccessFull == result.IsSuccessFull &&
                   Output == result.Output &&
                   Error == result.Error;
        }

        public override int GetHashCode()
        {
            var hashCode = 1000931690;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + IsSuccessFull.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Output);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Error);
            return hashCode;
        }
    }
}