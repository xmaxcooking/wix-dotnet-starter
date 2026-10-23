using App.Abstractions.FileSystem;

namespace App.Updates.Tests;

internal sealed class FakeFileSystem : IFileSystem
{
    private bool _exists;
    private string? _content;

    public List<string> CreatedDirectories { get; } = new();
    public List<string> CreatedFilePaths { get; } = new();
    public string? WrittenContent { get; private set; }
    public bool ThrowOnReadAllText { get; set; }
    public bool ThrowOnWriteAllText { get; set; }

    public void SeedExistingFile(string content)
    {
        _exists = true;
        _content = content;
    }

    public bool FileExists(string path) => _exists;

    public string ReadAllText(string path)
    {
        if (ThrowOnReadAllText)
        {
            throw new IOException("Simulated read failure.");
        }

        if (_content is null)
        {
            throw new FileNotFoundException(path);
        }

        return _content;
    }

    public void WriteAllText(string path, string contents)
    {
        if (ThrowOnWriteAllText)
        {
            throw new IOException("Simulated write failure.");
        }

        WrittenContent = contents;
        _exists = true;
        _content = contents;
    }

    public void CreateDirectory(string path) => CreatedDirectories.Add(path);

    public Stream CreateFile(string path)
    {
        CreatedFilePaths.Add(path);
        return new MemoryStream();
    }
}