using System;

namespace DockerCliWrapper.Infrastructure
{
    public static class Guard
    {
        public static void IsNotNullOrEmpty(string argumentName, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("Value cannot be null or empty string.", argumentName);
            }
        }
    }
}