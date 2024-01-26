using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;

namespace EQC.Services
{
    public class FlowChartDiagramService : BaseService
    {
        //s20230626
        public List<T> GetFlowChartDiagram<T>(int id, string type)
        {
            string sql = @"
                Select
                    DiagramJson,
                    Type
                from FlowChartTpDiagram
                where FlowChartTpSeq=" + id + " and Type like '" + type + "%'";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public string getFlowChartTpDiagramJson(int id, string type)
        {
            string sql = @"Select DiagramJson from FlowChartTpDiagram where FlowChartTpSeq=" + id + " and Type='" + type + "'";
            var result = db.ExecuteScalar(sql);
            
             return (string)db.ExecuteScalar(sql);
        }
        public void storeFlowChartTpDiagramJson(FormCollection value)
        {
            string DiagramStr = Encoding.UTF8.GetString(Convert.FromBase64String(value["DiagramJson"]));
            string sql2 = @"Update FlowChartTpDiagram Set DiagramJson = '" + DiagramStr + "' where FlowChartTpSeq = " + value["Seq"] + " and Type='" + value["Type"] + "'";
            string sql = @"Insert into FlowChartTpDiagram(FlowChartTpSeq, DiagramJson, Type) values (@FlowChartTpSeq, @DiagramJson, @Type)";

            int affect = db.ExecuteNonQuery(sql2);
            if (affect == 0)
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@FlowChartTpSeq", value["Seq"]);
                cmd.Parameters.AddWithValue("@DiagramJson", DiagramStr);
                cmd.Parameters.AddWithValue("@Type", value["Type"]);
                db.ExecuteNonQuery(cmd);
            }

        }

        


        private string getTypeTableName(string type)
        {
            string tableName = "";
            switch (type)
            {
                case "Chapter5": tableName = "EngMaterialDeviceList"; break;
                case "Chapter6": tableName = "EquOperTestList"; break;
                case "Chapter701": tableName = "ConstCheckList"; break;
                case "Chapter702": tableName = "EnvirConsList"; break;
                case "Chapter703": tableName = "OccuSafeHealthList"; break;
                default: tableName = ""; break;
            }
            return tableName;
        }

        internal int getFlowChartTpId(int id, string type)
        {
            string tableName = getTypeTableName(type);
            string sql = @"select b.Seq from " + tableName + @" a 
                inner join " + tableName + @"Tp b 
                on 
				(select top 1 value from string_split(a.FlowCharUniqueFileName,'.') )
				= (select top 1 value from string_split(b.FlowCharUniqueFileName,'.') ) 
                where a.Seq = " + id + " and a.FlowCharUniqueFileName != '' ";
            var result = db.ExecuteScalar(sql);
            if(result == null)
            {
                return -1;
            }   
            return (int)db.ExecuteScalar(sql);

        }

        internal void DeleteFlowChart(int seq, string type)
        {
            string sql = @"delete from FlowChartTpDiagram where FlowChartTpSeq=" + seq + " and Type Like '" + type + "%'";
            db.ExecuteNonQuery(sql);
        }
    }

}