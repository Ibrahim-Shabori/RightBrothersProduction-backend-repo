using Google.Apis.Util;

namespace RightBrothersProduction.API.Services
{
    public class SkewedClock : IClock
    {
        private readonly TimeSpan _skew;

        public SkewedClock(TimeSpan skew)
        {
            _skew = skew;
        }

        public DateTime UtcNow => DateTime.UtcNow.Add(_skew);

        public DateTime Now => DateTime.Now.Add(_skew);
    }
}
