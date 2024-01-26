
namespace EQC.Models
{
    public class SupPersonEquipmentModel
    {//監造(施工)日誌_工地人員及機具管理
        //public int SupDailyDateSeq { get; set; }
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int FieldEngineer { get; set; }
        public int RebarWorker { get; set; }
        public int TemplateWorker { get; set; }
        public int Plumber { get; set; }
        public int ManualWorker { get; set; }
        public int ConcreteWorker { get; set; }
        public int Excavator { get; set; }
        public int CraneTtruck { get; set; }
        public int PileDriver { get; set; }
        public string Memo { get; set; }
    }
}
