using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.DtoParameters
{
    public class PaperDtoParameters
    {
        private const int defaultNumber = 1;
        private const int defaultSize = 10;
        
        private const int MaxSize = 30;
        private const int MinNumber = 1;

        public string Title { get; set; }
        public string Lable { get; set; }
        /// <summary>
        /// 搜索条件
        /// </summary>
        public string SearchTerm { get; set; }

        private int pageNumber = defaultNumber;
        public int PageNumber
        {
            get => pageNumber;
            set => pageNumber = value < MinNumber ? MinNumber : value;
        }

        private int pageSize = defaultSize;
        public int PageSize
        {
            get =>  pageSize;
            set => pageSize = value > MaxSize ? MaxSize : value;
        }

    }
}
