namespace iSukces.CliTools;

public interface ICacheFileNameProvider
{
    string CreateCacheFileName(string anyText, string subFolder);
}
