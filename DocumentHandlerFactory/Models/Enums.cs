namespace DocumentHandlerFactory.Models
{
    /// <summary>
    /// Represents how to handle of document.
    /// 
    /// e.g;
    /// If HandlerType is 'File' then saves it to under the path specified in the operating system.
    /// Or if it is 'Ftp' then uploads the file to a specified url via FTP.
    /// </summary>
    public enum HandlerType
    {
        File,
        Ftp
    }
}
