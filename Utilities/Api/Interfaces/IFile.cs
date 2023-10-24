
namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IFile
    {
        long Id { get; set; }

        long Size { get; set; }

        string Name { get; set; }

        string PublicUrl { get; set; }

        string UrlThumbnail { get; set; }
    }
}
