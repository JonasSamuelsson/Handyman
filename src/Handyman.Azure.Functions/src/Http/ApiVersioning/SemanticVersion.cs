namespace Handyman.Azure.Functions.Http.ApiVersioning
{
   internal class SemanticVersion
   {
      public long Major { get; set; }
      public long Minor { get; set; }
      public string PreRelease { get; set; }

      public override string ToString()
      {
         return $"{Major}.{Minor}{(PreRelease == string.Empty ? string.Empty : $"-{PreRelease}")}";
      }
   }
}