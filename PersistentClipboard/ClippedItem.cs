namespace PersistentClipboard
{
    public class ClippedItem
    {
        public long Id { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return Content;
        }
    }
}
