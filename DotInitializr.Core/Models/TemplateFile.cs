namespace DotInitializr
{
   public class TemplateFile
   {
      public string Name { get; set; }
      public string Content { get; set; }
   }

   public class TemplateFileBinary : TemplateFile
   {
      public byte[] ContentBytes { get; set; }
   }
}