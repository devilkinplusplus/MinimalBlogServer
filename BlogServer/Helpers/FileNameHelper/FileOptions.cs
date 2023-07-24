﻿namespace BlogServer.Helpers.FileNameHelper
{
    public class FileOptions
    {
        protected async Task<string> FileRenameAsync(string pathName, string fileName)
        {
            string newFileName = await Task.Run<string>(async () =>
            {
                string extension = Path.GetExtension(fileName);
                string oldName = Path.GetFileNameWithoutExtension(fileName);
                DateTime datetimenow = DateTime.UtcNow;
                string datetimeutcnow = datetimenow.ToString("yyyyMMddHHmmss");
                string newFileName = $"{datetimeutcnow}{CustomizeFileName.CharacterRegulatory(oldName)}{extension}";

                if (File.Exists($"{pathName}\\{newFileName}"))
                    return await FileRenameAsync("", newFileName);
                else
                    return newFileName;
            });
            return newFileName;
        }
    }
}
