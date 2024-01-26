using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EQC.Controllers.InterFaceForFrontEnd;
namespace EQC.Controllers.Common
{
    public abstract class WebBaseController<T> : MyController, WebBaseInterface<T>
    {
        public virtual void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public virtual void Get()
        {
            throw new NotImplementedException();
        }

        public virtual void GetByKeyWord(string keyWord = null)
        {
            throw new NotImplementedException();
        }

        public virtual void GetPagination(int page, int perPage)
        {
            throw new NotImplementedException();
        }

        public virtual void Insert(T model)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(T model)
        {
            throw new NotImplementedException();
        }
    }
}
