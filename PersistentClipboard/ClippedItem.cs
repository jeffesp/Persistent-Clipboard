namespace PersistentClipboard
{
    public class ClippedItem
    {
        public long Timestamp { get; set; }
        public ContentType ContentType { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return Content;
        }
    }

    public enum ContentType
    {
        Text,
        Image
    }
}
