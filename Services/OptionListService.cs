using EQC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class OptionListService : BaseService
    {//選單清單
        public const int _Material = 1; //材料
        public const int _Person = 2; //人員
        public const int _Equipment = 3; //機具
        //清單
        public List<SelectOptionModel> GetList(int itemType)
        {
            string sql = @"SELECT
                    ItemName Text,
                    ItemValue Value
                FROM OptionList
				where ItemType=@ItemType
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ItemType", itemType);

            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
    }
}