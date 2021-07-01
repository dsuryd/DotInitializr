using System;

namespace DotInitializr
{
   public class TemplateException : Exception
   {
      public TemplateException(string message, Exception innerException) : base(message, innerException)
      {
      }
   }
}