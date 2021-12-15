using System;

namespace MySearchEngine.Repository
{
    public interface IPageInfoRepository
    {
        bool Exists(Uri uri);
        bool AddIfNew(Uri uri);
    }
}
