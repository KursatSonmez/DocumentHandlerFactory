using System;

namespace DocumentHandlerFactory.Settings
{
    /// <summary>
    /// Contains the necessary settings for operations to be executed via FTP
    /// </summary>
    public class FtpSettings
    {
        public FtpSettings(string url, string username, string password)
        {
            SetFtpSettings(url, username, password);
        }

        public string Url { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }


        private void SetFtpSettings(string url, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(paramName: nameof(url));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(paramName: nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(paramName: nameof(password));

            Url = url;
            Username = username;
            Password = password;
        }
    }
}
