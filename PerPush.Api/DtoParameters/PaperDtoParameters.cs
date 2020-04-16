using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.DtoParameters
{
    public class PaperDtoParameters
    {
        public string Title { get; set; }
        public string Lable { get; set; }
        /// <summary>
        /// 搜索条件
        /// </summary>
        public string SearchTerm { get; set; }

    }
}
