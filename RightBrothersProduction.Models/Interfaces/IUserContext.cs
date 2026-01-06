using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.Models.Interfaces
{
    public interface IUserContext
    {
        int TimezoneOffsetInMinutes { get; }
        DateTime ConvertToUserLocalTime(DateTime? utcDate);
    }
}
