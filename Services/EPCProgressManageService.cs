using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EPCProgressManageService : BaseService
    {//工程管理-預定進度
        //工程已連接PrjXML標案
        public List<T> GetEngLinkTenderBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    a.EngPeriod,
                    a.TotalBudget,
                    a.SubContractingBudget,
                    a.ApprovedCarbonQuantity,
                    a.StartDate,
                    ISNULL(a.EngChangeSchCompDate, a.SchCompDate) SchCompDate, --s20220902
                    a.PrjXMLSeq,
                    d.DocState,
                    e.TenderNo,
                    e.ExecUnitName,
                    e.TenderName,
                    e.DurationCategory,
                    e.ActualStartDate
                FROM EngMain a
                left join PrjXML e on(e.Seq=a.PrjXMLSeq)
                left outer join SupervisionProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        /// <summary>
        /// 工程進度 包含 工程變更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq">EngMain.Seq</param>
        /// <param name="mode">1:監造 2:施工</param>
        /// <param name="tarDate"></param>
        /// <returns></returns>
        public List<T> GetEngProgressAndEngChange<T>(int seq, int mode, string tarDate)
        {
            string sql;
            bool initEng = true;
            List<EPCProgressEngChangeListVModel> list = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(seq);
            if(list.Count >0)
            {//s20230416
                DateTime td = DateTime.Parse(tarDate);
                initEng = (td.Subtract(list[0].StartDate.Value).Days < 0);
            }
            if (initEng)
            {
                sql = @"
                SELECT 
                    CAST(sum(z1.Price * z1.Quantity * z1.SchProgress) / ISNULL(sum(z1.Amount),1) as decimal(16,2)) SchProgress,
                    CAST(sum(z1.Price * z1.actualQuantity * 100) / ISNULL(sum(z1.Amount),1) as decimal(16,2)) AcualProgress
                from fPayItemProgress(@Seq, @mode, @tarDate) z1
                ";
            } else {//工程變更
                sql = @"
                SELECT 
                    CAST(sum(z1.Price * z1.Quantity * z1.SchProgress) / ISNULL(sum(z1.Amount),1) as decimal(16,2)) SchProgress,
                    CAST(sum(z1.Price * z1.actualQuantity * 100) / ISNULL(sum(z1.Amount),1) as decimal(16,2)) AcualProgress
                from fECPayItemProgress(@Seq, @mode, @tarDate) z1
                ";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@tarDate", tarDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
        /// <summary>
        /// 初始工程進度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq">EngMain.Seq</param>
        /// <param name="mode">1:監造 2:施工</param>
        /// <param name="tarDate"></param>
        /// <returns></returns>
        public List<T> GetEngProgress<T>(int seq, int mode, string tarDate)
        {
            string sql = @"
                SELECT 
                    CAST(sum(z1.Price * z1.Quantity * z1.SchProgress) / ISNULL(sum(z1.Amount),1) as decimal(6,2)) SchProgress,
                    CAST(sum(z1.Price * z1.actualQuantity * 100) / ISNULL(sum(z1.Amount),1) as decimal(6,2)) AcualProgress
                from fPayItemProgress(@Seq, @mode, @tarDate) z1
                ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@tarDate", tarDate);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}