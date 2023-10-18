var tempFilePath = Path.GetTempFileName();

// Make a file stuffed with guids
var fileWriteSteam = File.OpenWrite(tempFilePath);
foreach (var _ in Enumerable.Range(1, 100))
{
    await fileWriteSteam.WriteAsync(Guid.NewGuid().ToByteArray());
}
fileWriteSteam.Close();

// Stream the file into memory
var fileReadStream = File.OpenRead(tempFilePath);
using var memoryStream = new MemoryStream(128 * 100);
while (fileReadStream.CanRead && fileReadStream.Position < fileReadStream.Length)
{
    fileReadStream.CopyTo(memoryStream, 128);
}
memoryStream.Position = 0;

// Read the memory stream guid-by-guid
GetGuidsFromStream(fileReadStream);

fileReadStream.Close();
File.Delete(tempFilePath);

void GetGuidsFromStream(Stream stream)
{
    var buffer = new byte[16];
    while (stream.CanRead && stream.CanSeek && stream.Position < stream.Length)
    {
        var bytesRead = stream.Read(buffer, 0, 16);
        Console.WriteLine(new Guid(buffer));

        if (bytesRead == 0)
            break;
    }
}
