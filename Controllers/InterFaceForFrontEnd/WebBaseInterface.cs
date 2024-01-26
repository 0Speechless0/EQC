using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.Controllers.InterFaceForFrontEnd
{
    public interface WebBaseInterface<T>
    {
        void GetPagination(int page, int perPage);

        void Get();
        void Insert(T model);
        void Update(T model);

        void Delete(int id);
        void GetByKeyWord(string keyWord = null);
    }
}
