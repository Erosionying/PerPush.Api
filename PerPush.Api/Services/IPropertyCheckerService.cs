using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public interface IPropertyCheckerService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
