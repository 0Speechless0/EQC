
namespace EQC.Models
{
    public class ChapterListModel : ChapterModel
    {//章節
        public string Text
        {
            get { return this.ChapterName; }
        }
        public int Value
        {
            get { return this.Seq; }
        }
    }
}
