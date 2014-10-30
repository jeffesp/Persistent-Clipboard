namespace PersistentClipboard
{
    public interface IContentEncoder
    {
        byte[] Decode(byte[] contentBytes);
        byte[] Encode(byte[] contentBytes);
    }
}