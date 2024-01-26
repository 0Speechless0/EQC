namespace EQC.Models
{
    public class CECheckTableTreeModel
    {//碳排放量檢核表-樹種
        public int Seq { get; set; }
        public string TreeName { get; set; }
        public bool Checked { get; set; }
        public int? Amount { get; set; }
    }
}
