using System.Configuration;

namespace Projekat
{
    public static class Config
    {
        public static string ConnectionString =>
            ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
            ?? throw new System.InvalidOperationException("Connection string 'DefaultConnection' not found in App.config.");
    }
}
