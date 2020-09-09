using System;

namespace MySearchEngine.Repository
{
    public interface ICrawledRepository
    {
        bool Exists(Uri uri);
        bool AddIfNew(Uri uri);
    }
}
